using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LsysParser.Data
{
    class ResultReport
    {
        public int SavedAmount { get; set; }
        public int CheckedAmount { get; set; }
        public int ErrorsAmount { get; set; }

        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        List<CategoryReport> categoryReports = new List<CategoryReport>();
        string siteName;

        public ResultReport(string siteName)
        {
            this.siteName = siteName;
        }

        CategoryReport currCategoryReport;
        public void AddCategory(CategoryReport categoryReport)
        {
            if (currCategoryReport == null)//зашли сюда 1ый раз
                currCategoryReport = categoryReport;
            else//все следующие вызовы
            {
                categoryReports.Add(currCategoryReport);
                currCategoryReport = null;
                currCategoryReport = categoryReport;
            }
        }

        public void IncreaseCheckedAmount()
        {
            currCategoryReport.CheckedAmount++;
        }

        public void IncreaseSavedAmount()
        {
            currCategoryReport.SavedAmount++;
        }

        public void Save()
        {
            if (currCategoryReport != null)
            {
                categoryReports.Add(currCategoryReport);
                currCategoryReport = null;
            }

            if (!Directory.Exists("Отчеты"))
                Directory.CreateDirectory("Отчеты");

            string fileName = Path.Combine("Отчеты", $"Парсинг за {DateTime.Now.ToLongTimeString()}.txt");

            var strBuilder = new StringBuilder();
            //using (var file = File.CreateText(fileName))
            //{
            strBuilder.AppendLine($"Название сайта: {siteName}");
            strBuilder.AppendLine($"Начало в: {Start}; Конец в: {End}");
            strBuilder.AppendLine($"============================================");
            strBuilder.AppendLine($"Обнаружено {CheckedAmount} товаров");
            strBuilder.AppendLine($"Сохранено {SavedAmount} товаров");
            strBuilder.AppendLine($"Ошибок: {SavedAmount}");
            strBuilder.AppendLine($"Категорий на сайте: {categoryReports.Count}");
            strBuilder.AppendLine("============================================");
            strBuilder.AppendLine("<Название категории>: <обнаружено>; <сохранено>/<отмечено на сайте>");

            //foreach (var cr in categoryReports)
            //{
            //    file.WriteLine($"{cr.Name}: {cr.CheckedAmount}; {cr.SavedAmount}/{cr.SiteAmount}");
            //}
            //}
            File.WriteAllText(fileName, strBuilder.ToString());
        }
    }

    class CategoryReport
    {
        public int CheckedAmount { get; set; }//то что обнаружили и попытались спарсить
        public int SavedAmount { get; set; }//то что успешно спарсили и сохранили
        public int SiteAmount { get; set; }// кол-во товаров, заявленное сайтом
        public string Name { get; set; }
    }
}
