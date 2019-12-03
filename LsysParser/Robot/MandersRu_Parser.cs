using LsysParser.Robot.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LsysParser.Data.Model;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using LsysParser.Robot.Helper;
using System.IO;

namespace LsysParser.Robot
{
    class MandersRu_Parser : BasicParser
    {
        const string START_URL = "http://manders.ru";

        public MandersRu_Parser(Project project) : base(START_URL, project)
        {
        }

        protected override string GetCategoryPageUrl(string parsedUrl, int currPageIndex)
        {
            return parsedUrl + $"?PAGEN_1={currPageIndex}";
        }

        protected override int ParseCategoryProductCount()
        {
            var node = currHtml.DocumentNode.SelectSingleNode("//div[@class = 'nav-title']");
            var match = Regex.Match(node.InnerHtml, @"из\s(\d+)");

            return int.Parse(match.Groups[1].Value);
        }

        protected override List<string> ParseProductLinks(HtmlDocument html)
        {
            var nodes = html.DocumentNode
                .SelectNodes("//div[@class = 'filter_item']/div[@class = 'filter_item_photo']/a");

            return nodes.Select(x => START_URL + x.Attributes["href"].Value).ToList();
        }

        protected override Product ParseProduct(HtmlDocument html, string productLink, Category category)
        {
            var product = new Product();
            var pp = new HtmlPropertyParser(html, productLink);

            if (category.Name.Equals("ERROR"))
            {
                var tmp = html.DocumentNode.SelectSingleNode("//div[@class = 'breadcrumbs']/div[2]/a/span");
                var categoryName = pp.RemoveSpecialSymbols(tmp.InnerText);

                product.Category = db.Categorys.Find(x => x.Name.Equals(categoryName)).FirstOrDefault();
            }

            product.Name = pp.GetAsString("//div[@class = 'wrap_catalog clearfix ']/h1");
            product.Article = pp.GetAsString("//span[text() = 'Артикул:']/../../td[2]/span");
            if (string.IsNullOrEmpty(product.Article))
            {
                product.Article = pp.GetAsString("//div[@class = 'card_chrctr_top']/span/span");
            }

            product.Price = pp.GetAsDouble("//div[@class = 'card_form_price']/div");

            var propVal = pp.GetAsString("//span[text() = 'Тип товара:']/../../td[2]/span");
            if (propVal != null)
                product.Propertyes.Add(new Property()
                {
                    NameObj = new PropertyName("Тип товара"),
                    ValueObj = new PropertyValue(propVal)
                });

            var nodes = html.DocumentNode.SelectNodes("//table[@class = 'chrctr_table']//tr");
            if (nodes != null && nodes.Count != 0)
                foreach (var rowNode in nodes)
                {
                    var prop = new Property();
                    prop.NameObj = new PropertyName(pp.GetAsString(rowNode, "./td[1]/span"));
                    prop.ValueObj = new PropertyValue(pp.GetAsString(rowNode, "./td[2]"));

                    if (prop.NameObj.Name.Contains("Бренд"))
                        continue;

                    if (prop.NameObj.Name.Contains("Уход"))
                    {
                        var imgs = rowNode.SelectNodes("./td[2]//div[@class = 'img-care']/img");
                        if (imgs != null)
                            foreach (var img in imgs)
                            {
                                var prop2 = new Property();
                                prop2.NameObj = new PropertyName(pp.RemoveSpecialSymbols(img.Attributes["title"].Value));
                                prop2.ValueObj = new PropertyValue("1");

                                product.Propertyes.Add(prop2);
                            }

                        continue;
                    }

                    product.Propertyes.Add(prop);
                }
            else
                project.Error($"Для товара: {productLink} не найдены характеристики");

            var node = html.DocumentNode.SelectSingleNode("//div[@class = 'card_slick_links']/a[@class = 'download']");
            if (node != null)
            {
                var file = new ProductFile();
                file.Url = START_URL + node.Attributes["href"].Value;

                var brand = GetProductBrand(product, html, productLink);
                var collection = product.Propertyes.Where(x => x.NameObj.Name.Contains("Коллекция")).FirstOrDefault().ValueObj.Value;

                //todo удалить запрещенные символы
                file.Name = Path.Combine("images", brand.Name, collection, product.Article, Path.GetFileName(file.Url));

                product.Files.Add(file);
            }

            return product;
        }

        protected override Brand GetProductBrand(Product product, HtmlDocument html, string productLink)
        {
            var pp = new HtmlPropertyParser(html, productLink);

            var nodes = html.DocumentNode.SelectNodes("//table[@class = 'chrctr_table']//tr");
            if (nodes != null && nodes.Count != 0)
                foreach (var rowNode in nodes)
                {
                    var prop = new Property();
                    prop.NameObj = new PropertyName(pp.GetAsString(rowNode, "./td[1]/span"));
                    prop.ValueObj = new PropertyValue(pp.GetAsString(rowNode, "./td[2]"));

                    if (!prop.NameObj.Name.Contains("Бренд"))
                        continue;

                    return new Brand()
                    {
                        Name = prop.ValueObj.Value
                    };
                }
            else
                project.Error($"Для товара: {productLink} не найден бренд");

            return null;
        }

        protected override bool IsNextPageExist(int currPageIndex, HtmlDocument html)
        {
            var check = html.DocumentNode.SelectSingleNode($"//a[text() = '{currPageIndex + 1}']");
            return check != null;
        }

        protected override List<Category> ParseCategorys()
        {
            throw new NotImplementedException();
        }
    }
}
