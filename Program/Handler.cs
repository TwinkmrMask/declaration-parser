using System.Collections.Generic;
using System.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;
using System.Xml;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;

namespace XmlParser
{
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    internal class Handler
    {
        private double _netWeightQuantity;
        private double _grossWeightQuantity;
        private double _positions;
        private readonly List<(string, string)> _data;
        private readonly List<(string, string)> _awb;

        public static string GetPath () => IDefaultSettings.NameExcelFile;

        public Handler()
        {
            try
            {
                _grossWeightQuantity = 0;
                _netWeightQuantity = 0;
                _positions = 0;
                _awb = new List<(string, string)>();
                _data = new List<(string, string)>(); 
                IDefaultSettings.AddTransportCodes();
            }
            catch (Exception exception) { MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
            finally
            {
                this._grossWeightQuantity = default;
                this._netWeightQuantity = default;
                this._positions = default;
            }
        }
        public List<(string, string)> XmlHandler(string path)
        {
            var document = new XmlDocument();
            
            try { document.Load(path); }
            catch { return new List<(string, string)>() { ("Файл повреждён", "Неудалось прочитать файл") } ; }
            var book = Open();

            var root = document.DocumentElement ?? throw new ArgumentNullException(nameof(path));
            foreach (var general in from XmlElement item in root from XmlNode child in item.ChildNodes where child is { HasChildNodes: true } from XmlNode declaration in child.ChildNodes where declaration.Name == "ESADout_CU" from XmlNode general in declaration.ChildNodes select general)
            {
                if (general.Name == "ESADout_CUGoodsShipment")
                {
                    Collect(Search("catESAD_cu:TotalPackageNumber", general, "Количество мест"), book);
                    Collect(Search("catESAD_cu:TotalCustCost", general, "Итоговая таможенная стоимоть"), book);
                }
                foreach (XmlNode info in general.ChildNodes)
                {
                    switch (info.Name)
                    {
                        case "ESADout_CUGoodsLocation":
                            Collect(Search("CustomsOffice", info, "Пост прибытия"), book);
                            break;
                        case "ESADout_CUConsigment":
                            foreach (XmlNode transport in info.ChildNodes)
                                foreach(XmlNode unused in transport)
                                    Collect(Search("catESAD_cu:TransportIdentifier", info, "Транспорт"), book);
                            break;
                        case "ESADout_CUMainContractTerms":
                            Collect(Search("catESAD_cu:ContractCurrencyCode", info, "Валюта"), book);
                            Collect(Search("catESAD_cu:ContractCurrencyRate", info, "Курс"), book);
                            Collect(Search("catESAD_cu:TotalInvoiceAmount", info, "Итоговая фактурная стоимоть"), book);
                            break;
                        case "ESADout_CUGoods":
                            Calc(Search("catESAD_cu:GrossWeightQuantity", info, "Масса брутто").Item2, ref _grossWeightQuantity);
                            Calc(Search("catESAD_cu:NetWeightQuantity", info, "Масса нетто").Item2, ref _netWeightQuantity);
                            break;
                    }
                    foreach (XmlNode product in info.ChildNodes)
                    {
                        foreach (XmlNode count in product.ChildNodes)
                            if (count.Name == "catESAD_cu:GoodsGroupInformation")
                                foreach (var about in count.Cast<XmlNode>().Where(about => about.Name == "catESAD_cu:GoodsGroupQuantity"))
                                    Calc(Search("catESAD_cu:GoodsQuantity", about, "Количество товара").Item2, ref _positions);
                    }
                    foreach (XmlNode doc in info)
                    {
                        if (doc.Name == "ESADout_CUPresentedDocument")
                        {
                            if (CheckDocumentCode(Search("catESAD_cu:PresentedDocumentModeCode",
                                doc, "Классификационный номер документа").Item2))
                            {
                                Add(Search("cat_ru:PrDocumentName", doc, "Документ"));
                                Add(Search("cat_ru:PrDocumentNumber", doc, "Номер документа"));
                            }
                        }
                    }
                }
            } 
            Collect(("Общая масса брутто", this._grossWeightQuantity.ToString(CultureInfo.InvariantCulture)), book);
            Collect(("Общая масса нетто", this._netWeightQuantity.ToString(CultureInfo.InvariantCulture)), book); 
            Collect(("Всего позиций", this._positions.ToString(CultureInfo.InvariantCulture)), book);
            foreach (var pair in _awb.Distinct()) Collect(pair, book);
            Close(book.Item1);
            return this._data;
        }
        
        //auxiliary methods
        private static void Close(IWorkbook wb)
        {
            if (!Directory.Exists(IDefaultSettings.DefaultPath)) Directory.CreateDirectory(IDefaultSettings.DefaultPath);
            using (var fs = new FileStream(GetPath(), FileMode.Create, FileAccess.Write))
            {
                wb.Write(fs);
                fs.Close();
            }
            wb.Close();
        }
        private static (IWorkbook, ISheet) Open()
        {
            const string sheetName = "info";
            IWorkbook wb = new XSSFWorkbook();
            var sh = wb.CreateSheet(sheetName);
            return (wb, sh);
        }
        private void save(in (string, string) row, (IWorkbook, ISheet) book)
        {
            if (!Validation(row)) return;
            var currentRow = book.Item2.CreateRow(book.Item2.LastRowNum + 1);
            currentRow.CreateCell(0).SetCellValue(row.Item1);
            book.Item2.AutoSizeColumn(0);
            currentRow.CreateCell(1).SetCellValue(row.Item2);
            book.Item2.AutoSizeColumn(1);
        }
        private void Add((string, string) pair)
        {
            if (!_awb.Contains(pair))
                _awb.Add(pair);
        }
        private static bool CheckDocumentCode(string number)
        {
            using var @base = new DataBase();
            return @base.TransportCodeEach(number);
        }
        private static void Calc(string value, ref double result)
        {
            if(value != null)
                result += double.Parse(value, CultureInfo.InvariantCulture);
        }

        public static (string, string) Search(string value, XmlNode collection, string name)
        {
            foreach (XmlNode element in collection.ChildNodes)
                if (element.Name == value)
                    return (name, element.InnerText);
            return default;
        }
        private void Collect((string, string) value, (IWorkbook, ISheet) book)
        {
            _data.Add(value);
            save(value, book);
        }
        private static bool Validation((string, string) value)
        {
            return !string.IsNullOrWhiteSpace(value.Item1) || !string.IsNullOrWhiteSpace(value.Item2);
        }
    }
}
