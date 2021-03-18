using System;
using System.Collections.Generic;
using System.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Xml;

namespace WpfApp3
{
    class Handler : DefaultSettings
    {
        double netWeightQuantity;
        double grossWeightQuantity;
        double positions;
        DataBase data;
        Information information;
        List<(string, string)> awb;
        public Handler(string path)
        {
            try
            {
                this.grossWeightQuantity = 0;
                this.netWeightQuantity = 0;
                this.positions = 0;
                this.awb = new List<(string, string)>();
                this.information = new Information();
                xmlHandler(path);
            }
            finally
            {
                this.grossWeightQuantity = default;
                this.netWeightQuantity = default;
                this.positions = default;
            }
        }

        void close(IWorkbook wb, string path, string name)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            using (FileStream fs = new FileStream(path + name, FileMode.Create, FileAccess.Write))
                wb.Write(fs);
            wb.Close();
        }
        (IWorkbook, ISheet) open()
        {
            const string sheetName = "info";
            IWorkbook wb = new XSSFWorkbook();
            ISheet sh = wb.CreateSheet(sheetName);
            return (wb, sh);
        }
        IWorkbook save(in string[] row, (IWorkbook, ISheet) book)
        {
            if (row != null)
            {
                IRow currentRow = book.Item2.CreateRow(book.Item2.LastRowNum + 1);
                const int countColumn = 2;
                for (int j = 0; j < countColumn; j++)
                {
                    currentRow.CreateCell(j).SetCellValue(row[j]);
                    book.Item2.AutoSizeColumn(j);
                }
                return book.Item1;
            }
            return default;
        }
        void add(string[] pair)
        {
            if (!awb.Contains((pair[0], pair[1])))
                awb.Add((pair[0], pair[1]));
        }
        bool checkDocumentCode(string[] number)
        {
            return data.TransportCodeEach(number[1]);
        }
        void calc(string[] value, ref double result)
        {
            result += double.Parse(value[1], System.Globalization.CultureInfo.InvariantCulture);
        }   
        string[] search(string value, XmlNode collection, string name)
        {
            foreach (XmlNode element in collection.ChildNodes)
                if (element.Name == value)
                    return new string[] { name, element.InnerText };
            return default;
        }
        void xmlHandler(string path)
        {
            XmlDocument document = new XmlDocument();
            document.Load(path);

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
                                            save(search("catESAD_cu:TotalPackageNumber", general, "Количество мест"), book);
                                            save(search("catESAD_cu:TotalCustCost", general, "Итоговая таможенная стоимоть"), book);
                                        }
                                        foreach (XmlNode info in general.ChildNodes)
                                        {
                                            switch (info.Name)
                                            {
                                                case "ESADout_CUGoodsLocation":
                                                    save(search("CustomsOffice", info, "Пост прибытия"), book);
                                                    break;

                                                case "ESADout_CUConsigment":
                                                    foreach (XmlNode transport in info.ChildNodes)
                                                        save(search("catESAD_cu:TransportIdentifier", info, "Транспорт"), book);
                                                    break;
                                                case "ESADout_CUMainContractTerms":
                                                    save(search("catESAD_cu:ContractCurrencyCode", info, "Валюта"), book);
                                                    save(search("catESAD_cu:ContractCurrencyRate", info, "Курс"), book);
                                                    save(search("ESADout_CUMainContractTerms", info, "Итоговая фактурная стоимоть"), book);
                                                    break;
                                                case "ESADout_CUGoods":
                                                    calc(search("catESAD_cu:GrossWeightQuantity", info, "Масса брутто"), ref grossWeightQuantity);
                                                    calc(search("catESAD_cu:NetWeightQuantity", info, "Масса нетто"), ref netWeightQuantity);
                                                    break;
                                                default: break;
                                            }
                                            foreach (XmlNode product in info.ChildNodes)
                                            {
                                                foreach (XmlNode count in product.ChildNodes)
                                                    if (count.Name == "catESAD_cu:GoodsGroupInformation")
                                                        foreach (XmlNode about in count)
                                                            if (about.Name == "catESAD_cu:GoodsGroupQuantity")
                                                                calc(search("cat_ru:GoodsQuantity", about, "Количество товара"), ref positions);
                                            }
                                            foreach (XmlNode doc in info)
                                            {
                                                if (doc.Name == "ESADout_CUPresentedDocument")
                                                {
                                                    try
                                                    {
                                                        if (checkDocumentCode(search("catESAD_cu:PresentedDocumentModeCode", doc, "Классификационный номер документа")))
                                                        {
                                                            add(search("cat_ru:PrDocumentName", doc, "Документ"));
                                                            add(search("cat_ru:PrDocumentNumber", doc, "Номер документа"));
                                                        }
                                                    }
                                                    catch (System.ArgumentException)
                                                    {
                                                        /*
                                                        ╲╲┏━╮╲╲╲╱╱╱╭━┓╱╱
                                                        ╲╲┣╮┃╲╲╲╱╱╱┃╭┫╱╱
                                                        ╲╲┣╯╰┻┻┻┻┻┻╯╰┫╱╱
                                                        ╲╲┃╱┈┈╲┊┊╱┈┈╲┃╱╱
                                                        ╲╲┃╰┳┳╯◢◣╰┳┳╯┃╱╱
                                                        ╲╲╰━━━╰━━╯━━━╯╱╱
                                                        */
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
                save(new string[] { "Общая масса брутто", grossWeightQuantity.ToString() }, book);
                save(new string[] { "Общая масса нетто", netWeightQuantity.ToString() }, book);
                save(new string[] { "Всего позиций", positions.ToString() }, book);
            }
            foreach ((string, string) pair in awb.Distinct())
                save(new string[] { pair.Item1, pair.Item2 }, book);
            close(book.Item1, path, nameExcelFile);
        }
        
    }
}
