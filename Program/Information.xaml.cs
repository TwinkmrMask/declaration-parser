using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml;
using System;

namespace WpfApp3
{
    /// <summary>
    /// Логика взаимодействия для Information.xaml
    /// </summary>
    public partial class Information : Window
    {
        
        double netWeightQuantity;
        double grossWeightQuantity;
        double positions;
        List<string> transportDocumentCodes;


        //pepper with crutches
        List<(string, string)> awb = new List<(string,string)>();
        void add(string[] pair)
        {
            if(!awb.Contains((pair[0], pair[1])))
                awb.Add((pair[0], pair[1]));  
        }
        bool checkDocumentCode(string[] number) 
        {
            if (transportDocumentCodes.Contains(number[1]))
                return true;
            else return false;
        }
        void calc(string[] value, ref double result) 
        {
            result +=double.Parse(value[1], System.Globalization.CultureInfo.InvariantCulture);
        }
        void print(string[] value)
        {
            if(value != null)
                block.Text += $"{value[0]} - {value[1]}\n";
        }
        string[] search(string value, XmlNode collection, string name)
        {
            foreach (XmlNode element in collection.ChildNodes)
                if (element.Name == value)
                    return new string[] { name, element.InnerText };
            return default;
        }
        void XmlHandler(string path)
        {
            XmlDocument document = new XmlDocument();
            document.Load(path);

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
                                            {
                                                print(search("catESAD_cu:TotalPackageNumber", general, "Количество мест"));
                                                print(search("catESAD_cu:TotalCustCost", general, "Итоговая таможенная стоимоть"));
                                            }
                                            foreach (XmlNode info in general.ChildNodes)
                                            {
                                                if (info.Name == "ESADout_CUGoodsLocation") print(search("CustomsOffice", info, "Пост прибытия"));
                                                if (info.Name == "ESADout_CUConsigment") print(search("catESAD_cu:TransportIdentifier", info, "Транспорт"));
                                                if (info.Name == "ESADout_CUMainContractTerms")
                                                {
                                                    print(search("catESAD_cu:ContractCurrencyCode", info, "Валюта"));
                                                    print(search("catESAD_cu:ContractCurrencyRate", info, "Курс"));
                                                    print(search("ESADout_CUMainContractTerms", info, "Итоговая фактурная стоимоть"));
                                                }
                                                if (info.Name == "ESADout_CUGoods")
                                                {
                                                    {
                                                        calc(search("catESAD_cu:GrossWeightQuantity", info, "Масса брутто"), ref grossWeightQuantity);
                                                        calc(search("catESAD_cu:NetWeightQuantity", info, "Масса нетто"), ref netWeightQuantity);
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
                                                            catch (System.ArgumentException exp)
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
                }
            }
            {
                print(new string[] { "Общая масса брутто", grossWeightQuantity.ToString() });
                print(new string[] { "Общая масса нетто", netWeightQuantity.ToString() });
                print(new string[] { "Всего позиций", positions.ToString() });
            }

            foreach ((string, string) pair in awb.Distinct())
            {
                print(new string[] { pair.Item1, pair.Item2});
            }
        }
        public Information(string path)
        {
            try
            {
                this.grossWeightQuantity = 0;
                this.netWeightQuantity = 0;
                this.positions = 0;
                this.transportDocumentCodes = new List<string>()
                {
                    "02011", "02012", "02013", 
                    "02014", "02015", "02016", 
                    "02017", "02018", "02019",
                    "02020", "02021", "02022",
                    "02024", "02025", "02099"
                };
                InitializeComponent();
                XmlHandler(path);
            }
            finally 
            {
                this.grossWeightQuantity = 0;
                this.netWeightQuantity = 0;
                this.positions = 0;
            }
        }
    }
}