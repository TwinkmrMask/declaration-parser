using System;
using System.Windows;
using System.Xml;
namespace WpfApp3
{
    /// <summary>
    /// Логика взаимодействия для Information.xaml
    /// </summary>
    public partial class Information : Window
    {
        void print(string name, string value)
        {
            block.Text += $"{name} - {value}\n";
        }
        void search(string value, XmlNode collection, string name)
        {
            foreach (XmlNode element in collection.ChildNodes)
            {
                if (element.Name == value)
                    print(name, element.InnerText);
            }
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
                                            search("catESAD_cu:TotalPackageNumber", general, "Количество мест");
                                            foreach (XmlNode info in general.ChildNodes)
                                            {
                                                if (info.Name == "ESADout_CUGoodsLocation") search("CustomsOffice", info, "Пост прибытия");
                                                if (info.Name == "ESADout_CUConsigment") search("catESAD_cu:TransportIdentifier", info, "Транспорт");
                                                if (info.Name == "ESADout_CUMainContractTerms")
                                                {
                                                    search("catESAD_cu:ContractCurrencyCode", info, "Валюта");
                                                    search("catESAD_cu:ContractCurrencyRate", info, "Курс");
                                                }
                                                if (info.Name == "ESADout_CUGoods")
                                                {                                                    search("catESAD_cu:GrossWeightQuantity", info, "Масса брутто");
                                                    search("catESAD_cu:NetWeightQuantity", info, "Масса нетто");
                                                    search("catESAD_cu:InvoicedCost", info, "Фактурная стоимость");
                                                    search("catESAD_cu:CustomsCost", info, "Таможенная стоимость");
                                                    foreach (XmlNode product in info.ChildNodes)
                                                    {
                                                        search("catESAD_cu:GoodsDescription", product, "Описание товара");
                                                        foreach (XmlNode count in product.ChildNodes)
                                                        {
                                                            if (count.Name == "catESAD_cu:GoodsGroupInformation")
                                                                foreach (XmlNode about in count)
                                                                    if (about.Name == "catESAD_cu:GoodsGroupQuantity")
                                                                        search("cat_ru:GoodsQuantity", about, "Количество товара");
                                                        }
                                                    }
                                                    foreach (XmlNode doc in info)
                                                    {
                                                        if (doc.Name == "ESADout_CUPresentedDocument")
                                                        {
                                                            search("cat_ru:PrDocumentName", doc, "Документ");
                                                            search("cat_ru:PrDocumentNumber", doc, "Номер документа");

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
        public Information(string path)
        {
            InitializeComponent();
            XmlHandler(path);
        }
    }
}