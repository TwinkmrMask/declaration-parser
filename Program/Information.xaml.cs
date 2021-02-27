using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using System.IO;
namespace WpfApp3
{
    /// <summary>
    /// Логика взаимодействия для Information.xaml
    /// </summary>
    public partial class Information : Window
    {
        public Information(string path)
        {
            InitializeComponent();
            XmlDocument document = new XmlDocument();
            document.Load("../../../Resources/"+path);

            XmlElement root = document.DocumentElement;

            block.Text += root.Name + "\n";

            foreach (XmlElement item in root)
            {
                if (item.HasChildNodes)
                {

                    if (item.Name == "RussianHolderAddress")
                    {
                        foreach (XmlNode son in item.ChildNodes)
                        {

                            block.Text += son.Name + " - " + son.InnerText + "\n";
                        }
                    }
                    else if (item.Name == "GeneralList")
                    {
                        block.Text += "<---------------------------------------------------------------------------------------------------->\n" + item.Name + "\n";
                    }
                    else block.Text += item.Name + " - " + item.InnerText + "\n";
                }

                foreach (XmlNode child in item.ChildNodes)
                {
                    if (child != null)
                    {
                        if (child.HasChildNodes)
                        {

                            foreach (XmlNode son in child.ChildNodes)
                            {
                                if (son.Name != "#text")
                                {
                                    if (son.Name == "ListNumber")
                                    {
                                        block.Text += "<---------------------------------------------------------------------------------------------------->\n";
                                    }
                                    block.Text += son.Name + " - " + son.InnerText + "\n";

                                }
                            }
                        }
                    }
                }




            }
        }
    }
}