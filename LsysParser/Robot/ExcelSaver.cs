using LsysParser.Data;
using LsysParser.Data.Model;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LsysParser.Robot
{
    class ExcelSaver
    {
        Project project;

        public ExcelSaver(Project project)
        {
            this.project = project;
        }

        public void Save(string fileName)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);

            var db = new UnitOfProductsWork();
            var products = db.Products.GetAllForSave();

            using (var excel = new ExcelPackage(new FileInfo(fileName)))
            {
                var sheet = excel.Workbook.Worksheets.Add("Лист 1");
                sheet.Cells[1, 1].Value = "Ссылка";
                sheet.Cells[1, 2].Value = "Наименование";
                sheet.Cells[1, 3].Value = "Артикул";
                sheet.Cells[1, 4].Value = "Цена";
                sheet.Cells[1, 5].Value = "Бренд";
                sheet.Cells[1, 6].Value = "Фото (Оригинал)";
                sheet.Cells[1, 7].Value = "Фото (Файл)";
                sheet.Cells[1, 8].Value = "Категория";



                var propColumn = new Dictionary<string, int>();
                int column = 9;
                var propNames = db.PropertyNames.GetAll().Select(x => x.Name).Distinct();
                foreach (var name in propNames)
                {
                    sheet.Cells[1, column].Value = name;
                    propColumn.Add(name, column);
                    column++;
                }

                int row = 2;
                foreach (var prod in products)
                {
                    project.Counter(CounterType.TotalCheckedProducts);
                    var files = db.Files.Find(x => x.ProductId == prod.Id).ToList();

                    sheet.Cells[row, 1].Value = prod.Url;
                    sheet.Cells[row, 2].Value = prod.Name;
                    sheet.Cells[row, 3].Value = prod.Article;
                    sheet.Cells[row, 4].Value = prod.Price;
                    sheet.Cells[row, 5].Value = prod.Brand.Name;
                    sheet.Cells[row, 6].Value = files.FirstOrDefault()?.Url ?? "";
                    sheet.Cells[row, 7].Value = files.FirstOrDefault()?.Name ?? "";
                    sheet.Cells[row, 8].Value = prod.Category.Name;

                    var props = db.Propertyes.FindWithData(x => x.ProductId == prod.Id);
                    foreach (var prop in props)
                    {
                        sheet.Cells[row, propColumn[prop.NameObj.Name]].Value = prop.ValueObj.Value;
                    }

                    row++;
                }

                excel.Save();
            }
        }
    }
}
