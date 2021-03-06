using System;
using System.Collections.Generic;
using System.Windows;
using System.Xml;

namespace WpfApp3
{
    /// <summary>
    /// Логика взаимодействия для Information.xaml
    /// </summary>
    public partial class Information : Window
    {
        //pepper with crutches
        Dictionary<string, string> awb = new Dictionary<string, string>();
        void add(string[] pair)
        {
            awb.Add(pair[0], pair[1]);
            
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
                                            print(search("catESAD_cu:TotalPackageNumber", general, "Количество мест"));
                                            foreach (XmlNode info in general.ChildNodes)
                                            {
                                                if (info.Name == "ESADout_CUGoodsLocation") print(search("CustomsOffice", info, "Пост прибытия"));
                                                if (info.Name == "ESADout_CUConsigment") print(search("catESAD_cu:TransportIdentifier", info, "Транспорт"));
                                                if (info.Name == "ESADout_CUMainContractTerms")
                                                {
                                                    print(search("catESAD_cu:ContractCurrencyCode", info, "Валюта"));
                                                    print(search("catESAD_cu:ContractCurrencyRate", info, "Курс"));
                                                }
                                                if (info.Name == "ESADout_CUGoods")
                                                {                                                  
                                                    print(search("catESAD_cu:GrossWeightQuantity", info, "Масса брутто"));
                                                    print(search("catESAD_cu:NetWeightQuantity", info, "Масса нетто"));
                                                    print(search("catESAD_cu:InvoicedCost", info, "Фактурная стоимость"));
                                                    print(search("catESAD_cu:CustomsCost", info, "Таможенная стоимость"));
                                                    foreach (XmlNode product in info.ChildNodes)
                                                    {
                                                        print(search("catESAD_cu:GoodsDescription", product, "Описание товара"));
                                                        foreach (XmlNode count in product.ChildNodes)
                                                            if (count.Name == "catESAD_cu:GoodsGroupInformation")
                                                                foreach (XmlNode about in count)
                                                                    if (about.Name == "catESAD_cu:GoodsGroupQuantity")
                                                                        print(search("cat_ru:GoodsQuantity", about, "Количество товара"));
                                                        
                                                    }
                                                    foreach (XmlNode doc in info)
                                                    {
                                                        if (doc.Name == "ESADout_CUPresentedDocument")
                                                        {
                                                            try
                                                            {
                                                                add(search("cat_ru:PrDocumentName", doc, "Документ"));
                                                                add(search("cat_ru:PrDocumentNumber", doc, "Номер документа"));
                                                                break;
                                                            }
                                                            catch (System.ArgumentException exp)
                                                            {
                                                                break;
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
            foreach (KeyValuePair<string,string> pair in awb)
                block.Text += $"{pair.Key} {pair.Value}\n";
        }
        public Information(string path)
        {
            InitializeComponent();
            XmlHandler(path);
        }
    }
}