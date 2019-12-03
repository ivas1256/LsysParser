using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using LsysParser.Robot.Helper;
using LsysParser.Data.Model;
using System.IO;
using NLog;
using LsysParser.CustomException;
using LsysParser.Data;
using System.Diagnostics;
using System.Threading;
using System.Net;
using System.Reflection;
using System.Drawing;

namespace LsysParser.Robot.Abstract
{
    abstract class BasicParser
    {
        protected HtmlDocument currHtml;
        protected string currUrl;
        protected HttpHelper web;
        protected HtmlPropertyParser htmlProp;
        protected Project project;
        protected UnitOfProductsWork db;
        protected ErrorsRegister errorRegister;

        Logger logger = LogManager.GetCurrentClassLogger();
        Uri startUrl;
        TaskManager taskManager;
        Uri rootUrl;
        ResultReport report;

        public BasicParser(string startUrl, Project project)
        {
            this.project = project;
            this.startUrl = new Uri(startUrl);
            rootUrl = new Uri($"{this.startUrl.Scheme}://{this.startUrl.Host}/");
        }

        public void Start()
        {
            project.Info($"Парсинг запущен");

            try
            {
                db = new UnitOfProductsWork();
                web = new HttpHelper();
                web.Log += (ex, message) =>
                {
                    project.Error(message, ex);
                    logger.Error(ex);
                };
                report = new ResultReport(rootUrl.ToString());
                taskManager = new TaskManager(2, true);
                errorRegister = new ErrorsRegister(db.Context.Database.Connection.ConnectionString);

                if (MessageProvider.Confirm("Удалить все данные?"))
                {
                    errorRegister.RemoveAll();
                    db.Products.RemoveAll();
                    db.Brands.RemoveAll();
                    db.Propertyes.RemoveAll();
                    db.Files.RemoveAll();
                }

                report.Start = DateTime.Now;
                Parsing();
            }
            catch (Exception ex)
            {
                project.UnknownError(ex);
                logger.Error(ex);
            }
            finally
            {
                report.CheckedAmount = project.TotalCheckedProducts_Counter;
                report.SavedAmount = project.SavedProducts_Counter;
                report.ErrorsAmount = project.Errors_Counter;
                report.End = DateTime.Now;
                report.Save();

                db.Dispose();
                project.Info("Парсинг завершен");
            }
        }

        void Parsing()
        {
            List<int> categorysIds = new List<int>();
            currHtml = web.GetHtml(startUrl.ToString());
            if (currHtml == null)
                return;

            #region parse catagoryes
            try
            {
                if (db.Categorys.Count() == 0)
                {
                    var categorys = ParseCategorys();
                    if (categorys == null || categorys.Count == 0)
                        throw new ParserException("Категории не найдены");

                    foreach (var category in categorys)
                        db.Categorys.Add(category);
                    db.Complete();

                    project.Info($"Найдено {categorys.Count} категорий");

                    categorysIds = categorys.Select(x => x.Id).ToList();
                }
                else
                    categorysIds = db.Context.Category.Select(x => x.Id).ToList();
            }
            catch (ParserException ex)
            {
                project.Error("Ошибка парсинга категорий", ex);
                logger.Error(ex);
            }
            catch (Exception ex)
            {
                project.Error("Не удалось загрузить кэш", ex);
                logger.Error(ex);
            }
            #endregion

            foreach (var categoryId in categorysIds)
            {
                var category = db.Categorys.Get(categoryId);
                int currPageIndex = 0;

                do
                {
                    currPageIndex++;
                    project.Status = $"Парсинг категории: {category.Name}, страница: {currPageIndex}";
                    string currCategoryUrl = GetCategoryPageUrl(category.Url, currPageIndex);

                    currHtml = web.GetHtml(currCategoryUrl);
                    if (currHtml == null)
                    {
                        RegisterError(currCategoryUrl, web.LastError, ErrorSource.CatalogPage);
                        continue;
                    }

                    int categoryCount = -1;
                    try
                    {
                        categoryCount = ParseCategoryProductCount();
                    }
                    catch (Exception ex)
                    {
                        project.Error($"Не удалось спарсить кол-во товаров в категории: {category.Name}, {category.Url}", ex);
                        logger.Error(ex);
                    }

                    var productLinks = new List<string>();
                    try
                    {
                        productLinks = ParseProductLinks(currHtml);

                        if (productLinks == null || productLinks.Count == 0)
                            throw new ParserException($"Страница пуста или верстка сайта изменилась");
                    }
                    catch (Exception ex)
                    {
                        RegisterError(currCategoryUrl, ex, ErrorSource.ProductLinksParser, "Не удалось найти товары в категории");
                        project.Error($"Не удалось найти товары в категории: {category.Name}", ex);
                        logger.Error(ex);
                        continue;
                    }

                    foreach (var productLink in productLinks)
                    {
                        project.Counter(CounterType.TotalCheckedProducts);

                        try
                        {
                            if (!project.Settings.IsIncludeDuplicatesProducts)
                                if (db.Products.ExistWithUrl(productLink))
                                    continue;
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex);
                        }

                        taskManager.NewTask((string link, Category currCategory) =>
                        {
                            try
                            {
                                ParseProduct(link, currCategory);
                            }
                            catch (Exception ex)
                            {
                                RegisterError(link, ex, ErrorSource.ProductParser, "Неизвестная ошибка");
                                project.UnknownError(ex);
                                logger.Error(ex);
                            }
                        }, productLink, category);
                    }
                } while (IsNextPageExist(currPageIndex, currHtml));
            }
        }

        object locker = new object();
        void ParseProduct(string productLink, Category category)
        {
            var html = web.GetHtml(productLink);
            if (html == null)
            {
                RegisterError(productLink, web.LastError, ErrorSource.ProductPage);
                return;
            }

            htmlProp = new HtmlPropertyParser(html, productLink);
            htmlProp.OnMessage += (message) => project.Info(message);

            var product = new Product();
            try
            {
                product = ParseProduct(html, productLink, category);

                if (product.Category == null)
                    product.Category = category;
                product.Url = productLink;


                var brand = GetProductBrand(product, html, productLink);
                if (brand != null)
                {
                    int id = 0;
                    lock (locker)
                    {
                        id = db.Brands.GetIdByName(brand.Name);
                    }
                    if (id != 0)
                        product.BrandId = id;
                    else
                    {
                        product.BrandId = 0;
                        product.Brand = brand;
                    }
                }

                if (product.Propertyes != null)
                {
                    foreach (var prop in product.Propertyes)
                    {
                        int id = 0;
                        lock (locker)
                        {
                            id = db.PropertyNames.GetIdByName(prop.NameObj.Name);
                        }
                        if (id != 0)
                        {
                            prop.NameObj = null;
                            prop.NameId = id;
                        }
                        else
                            prop.NameId = 0;

                        lock (locker)
                        {
                            id = db.PropertyValues.GetIdByValue(prop.ValueObj.Value);
                        }
                        if (id != 0)
                        {
                            prop.ValueObj = null;
                            prop.ValueId = id;
                        }
                        else
                            prop.ValueId = 0;
                    }
                }

                try
                {
                    db.Products.SaveDirectly(product);
                    project.Counter(CounterType.SavedProducts);
                }
                catch (Exception ex)
                {
                    RegisterError(productLink, ex, ErrorSource.ProductParser, "Не удалось сохранить товар");
                    project.Error($"Не удалось сохранить товар: {product.Url}", ex);
                    logger.Error(ex);
                }
            }
            catch (Exception ex)
            {
                RegisterError(productLink, ex, ErrorSource.ProductParser, "Не удалось спарсить товар");
                project.Error($"Не удалось спарсить товар: {productLink}", ex);
                logger.Error(ex);
            }
        }

        bool isStop;
        public void Stop()
        {
            isStop = true;
        }

        bool isPause;
        public void Pause()
        {
            isPause = true;
        }

        public bool ResetPauseIfNeeded()
        {
            return false;
        }

        void RegisterError(string url, Exception ex, ErrorSource errorSource, string message = "")
        {
            ErrorRuntimeObj err = null;
            if (ex is WebException)
                err = new ErrorRuntimeObj(url, (WebException)ex, errorSource);
            else
                err = new ErrorRuntimeObj(url, ex, errorSource, message);

            errorRegister.RegisterError(err);
        }

        public void CheckErrors()
        {
            web = new HttpHelper();
            db = new UnitOfProductsWork();
            errorRegister = new ErrorsRegister(db.Context.Database.Connection.ConnectionString);
            var errors = db.Errors.GetAll();

            foreach (var err in errors)
            {
                if ((ErrorSource)err.ErrorSourceId == ErrorSource.ProductParser ||
                    (ErrorSource)err.ErrorSourceId == ErrorSource.ProductPage)
                {
                    try
                    {
                        project.Counter(CounterType.TotalCheckedProducts);

                        if (db.Products.ExistWithUrl(err.Url))
                        {
                            db.Errors.Remove(err);
                            db.Complete();
                            continue;
                        }

                        ParseProduct(err.Url, new Category() { Name = "ERROR" });
                    }
                    catch (Exception ex)
                    {
                        project.Error("error", ex);
                        logger.Error(ex);
                    }
                }
            }
        }

        public void DownloadImages()
        {
            web = new HttpHelper();
            db = new UnitOfProductsWork();
            taskManager = new TaskManager(10, true);
            var files = db.Files.GetAll();
            var currDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            foreach (var file in files)
            {
                try
                {
                    project.Counter(CounterType.TotalCheckedProducts);

                    var arr = file.Name.Split('\\');
                    for (int i = 0; i < arr.Count(); i++)
                    {
                        foreach (var ch in Path.GetInvalidFileNameChars())
                            arr[i] = arr[i].Replace(ch, '-');

                        foreach (var ch in Path.GetInvalidPathChars())
                            arr[i] = arr[i].Replace(ch, '-');
                    }

                    if (!file.Name.Equals(Path.Combine(arr)))
                    {
                        file.Name = Path.Combine(arr);
                        db.Files.UpdateName(file);
                    }

                    var savedfileName = Path.Combine(currDir, file.Name);
                    if (File.Exists(savedfileName))
                        continue;

                    if (!Directory.Exists(Path.GetDirectoryName(savedfileName)))
                        Directory.CreateDirectory(Path.GetDirectoryName(savedfileName));

                    taskManager.NewTask((string url, string fileName) =>
                    {
                        try
                        {
                            using (var client = new WebClient())
                                client.DownloadFile(url, fileName);

                            project.Counter(CounterType.SavedProducts);
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex);
                            project.Error(file.Name, ex);
                        }
                    }, file.Url, savedfileName);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    project.Error(file.Name, ex);
                }
            }

            project.Info("done");
        }

        public void ParseMissedProducts()
        {
            db = new UnitOfProductsWork();
            web = new HttpHelper();
            web.Log += (ex, message) =>
            {
                project.Error(message, ex);
                logger.Error(ex);
            };
            report = new ResultReport(rootUrl.ToString());
            taskManager = new TaskManager(10, true);
            errorRegister = new ErrorsRegister(db.Context.Database.Connection.ConnectionString);


            var categorysIds = db.Context.Category.Select(x => x.Id).ToList();

            foreach (var categoryId in categorysIds)
            {
                if (categoryId != 93)
                    return;

                var category = db.Categorys.Get(categoryId);

                var urls = new List<string>();

                for (int i = 0; i < 913; i++)
                    urls.Add(GetCategoryPageUrl(category.Url, i + 1));

                var allLinks = new List<string>();
                foreach (var url in urls)
                {
                    taskManager.NewTask((string currCategoryUrl) =>
                    {
                        project.Counter(CounterType.TotalCheckedProducts);
                        var html = web.GetHtml(currCategoryUrl);
                        if (html == null)
                        {
                            RegisterError(currCategoryUrl, web.LastError, ErrorSource.CatalogPage);
                            return;
                        }

                        try
                        {
                            var productLinks = ParseProductLinks(html);

                            if (productLinks == null || productLinks.Count == 0)
                                throw new ParserException($"Страница пуста или верстка сайта изменилась");

                            lock (locker)
                            {
                                foreach (var link in productLinks)
                                    allLinks.Add(link);
                            }
                        }
                        catch (Exception ex)
                        {
                            RegisterError(currCategoryUrl, ex, ErrorSource.ProductLinksParser, "Не удалось найти товары в категории");
                            project.Error($"Не удалось найти товары в категории: {category.Name}", ex);
                            logger.Error(ex);
                            return;
                        }
                    }, url);
                }
                taskManager.WaitAllTasks();

                var allProducts = db.Products.GetAll().Select(x => x.Url).ToList();
                var missed = allProducts.Except(allLinks);

                foreach (var link in missed)
                    RegisterError(link, new Exception(), ErrorSource.ProductParser, "Спарсить этот товар");

                project.Info("done");
            }
        }

        #region абстрактные методы, реализающие логику парсинга сайта
        protected abstract List<Category> ParseCategorys();
        protected abstract List<string> ParseProductLinks(HtmlDocument html);
        protected abstract Product ParseProduct(HtmlDocument html, string pageUrl, Category category);
        protected abstract Brand GetProductBrand(Product product, HtmlDocument html, string pageUrl);

        protected abstract bool IsNextPageExist(int currPageIndex, HtmlDocument html);
        protected abstract string GetCategoryPageUrl(string parsedUrl, int currPageIndex);

        protected abstract int ParseCategoryProductCount();
        #endregion;
    }
}
