using System.Collections.Generic;
using System.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;
using System.Xml;
using database;
using System;
using System.Windows;
namespace xmlparser
{
    class Handler : IDefaultSettings
    {
        private double netWeightQuantity;
        private double grossWeightQuantity;
        private double positions;
        private List<(string, string)> data;
        private List<(string, string)> awb;

        public Func<string> GetPath = () => IDefaultSettings.DefaultPath + IDefaultSettings.NameExcelFile;

        public Handler()
        {
            try
            {
                this.grossWeightQuantity = 0;
                this.netWeightQuantity = 0;
                this.positions = 0;
                this.awb = new List<(string, string)>();
                this.data = new List<(string, string)>(); 
                //IDefaultSettings.AddTransportCodes();
            }
            finally
            {
                this.grossWeightQuantity = default;
                this.netWeightQuantity = default;
                this.positions = default;
            }
        }
        public List<(string, string)> XmlHandler(string path)
        {
            
            XmlDocument document = new XmlDocument();
            try
            {
                document.Load(path);
            }
            catch
            {
               return new List<(string, string)>() { ("Файл повреждён", "Неудалось прочитать файл") } ;
            }
            (IWorkbook, ISheet) book = open();

            XmlElement root = document.DocumentElement;
            foreach (XmlElement item in root)
            { 
                foreach (XmlNode child in item.ChildNodes)
                {
                    if (child != null)
                        if (child.HasChildNodes)
                            foreach (XmlNode declaration in child.ChildNodes)
                            {
                                if (declaration.Name == "ESADout_CU")
                                {
                                    foreach (XmlNode general in declaration.ChildNodes)
                                    {
                                        if (general.Name == "ESADout_CUGoodsShipment")
                                        {
                                            collect(search("catESAD_cu:TotalPackageNumber", general, "Количество мест"), book);
                                            collect(search("catESAD_cu:TotalCustCost", general, "Итоговая таможенная стоимоть"), book);
                                        }
                                        foreach (XmlNode info in general.ChildNodes)
                                        {
                                            switch (info.Name)
                                            {
                                                case "ESADout_CUGoodsLocation":
                                                    collect(search("CustomsOffice", info, "Пост прибытия"), book);
                                                    break;
                                                case "ESADout_CUConsigment":
                                                    foreach (XmlNode transport in info.ChildNodes)
                                                        foreach(XmlNode transportCode in transport)
                                                            collect(search("catESAD_cu:TransportIdentifier", info, "Транспорт"), book);
                                                    break;
                                                case "ESADout_CUMainContractTerms":
                                                    collect(search("catESAD_cu:ContractCurrencyCode", info, "Валюта"), book);
                                                    collect(search("catESAD_cu:ContractCurrencyRate", info, "Курс"), book);
                                                    collect(search("catESAD_cu:TotalInvoiceAmount", info, "Итоговая фактурная стоимоть"), book);
                                                    break;
                                                case "ESADout_CUGoods":
                                                    calc(search("catESAD_cu:GrossWeightQuantity", info, "Масса брутто").Item2, ref this.grossWeightQuantity);
                                                    calc(search("catESAD_cu:NetWeightQuantity", info, "Масса нетто").Item2, ref this.netWeightQuantity);
                                                    break;
                                                default: break;
                                            }
                                            foreach (XmlNode product in info.ChildNodes)
                                            {
                                                foreach (XmlNode count in product.ChildNodes)
                                                    if (count.Name == "catESAD_cu:GoodsGroupInformation")
                                                        foreach (XmlNode about in count)
                                                            if (about.Name == "catESAD_cu:GoodsGroupQuantity")
                                                                calc(search("catESAD_cu:GoodsQuantity", about, "Количество товара").Item2, ref this.positions);
                                            }
                                            foreach (XmlNode doc in info)
                                            {
                                                if (doc.Name == "ESADout_CUPresentedDocument")
                                                {
                                                    //if (checkDocumentCode(search("catESAD_cu:PresentedDocumentModeCode", doc, "Классификационный номер документа").Item2))
                                                    {
                                                        add(search("cat_ru:PrDocumentName", doc, "Документ"));
                                                        add(search("cat_ru:PrDocumentNumber", doc, "Номер документа"));
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                }
            }
            {
                collect(("Общая масса брутто", this.grossWeightQuantity.ToString()), book);
                collect(("Общая масса нетто", this.netWeightQuantity.ToString()), book);
                collect(("Всего позиций", this.positions.ToString()), book);
            }
            foreach ((string, string) pair in awb.Distinct())
                collect(pair, book);
            close(book.Item1, IDefaultSettings.DefaultPath, IDefaultSettings.NameExcelFile);
            return this.data;
        }
        
        //auxiliary methods
        private void close(IWorkbook wb, string path, string name)
        {
            if (!Directory.Exists(IDefaultSettings.DefaultPath)) Directory.CreateDirectory(IDefaultSettings.DefaultPath);
            using (FileStream fs = new FileStream(GetPath(), FileMode.Create, FileAccess.Write))
            {
                wb.Write(fs);
                fs.Close();
            }
            wb.Close();
        }
        private (IWorkbook, ISheet) open()
        {
            const string sheetName = "info";
            IWorkbook wb = new XSSFWorkbook();
            ISheet sh = wb.CreateSheet(sheetName);
            return (wb, sh);
        }
        private IWorkbook save(in (string, string) row, (IWorkbook, ISheet) book)
        {
            if (validation(row))
            {
                IRow currentRow = book.Item2.CreateRow(book.Item2.LastRowNum + 1);
                currentRow.CreateCell(0).SetCellValue(row.Item1);
                book.Item2.AutoSizeColumn(0);
                currentRow.CreateCell(1).SetCellValue(row.Item2);
                book.Item2.AutoSizeColumn(1);
                return book.Item1;
            }
            return default;
        }
        private void add((string, string) pair)
        {
            if (!awb.Contains(pair))
                awb.Add(pair);
        }
        /*
        private bool checkDocumentCode(string number)
        {
            using (var data = new database.DataBase())
                return data.TransportCodeEach(number);
        }
        */
        private void calc(string value, ref double result)
        {
            if(value != null)
                result += double.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
        }
        private (string, string) search(string value, XmlNode collection, string name)
        {
            foreach (XmlNode element in collection.ChildNodes)
                if (element.Name == value)
                    return (name, element.InnerText);
            return default;
        }
        private IWorkbook collect((string, string) value, (IWorkbook, ISheet) book)
        {
            data.Add(value);
            return save(value, book);
        }
        private bool validation((string, string) value)
        {
            if (string.IsNullOrWhiteSpace(value.Item1) && string.IsNullOrWhiteSpace(value.Item2))
                return false;
            else return true;
        }
    }
}
