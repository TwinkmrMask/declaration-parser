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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.IO;

namespace WpfApp3
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        List<string> FileNames = new List<string>();
        List<string> ID = new List<string>();

        private void sort()
        {
            GetId();
            for(int i = 0; i < ID.Count-1;)
            {
                for(int j = i; j < ID.Count-1;)
                {
                    if(ID[i] == ID[j + 1])
                    {
                        ID.Remove(ID[j + 1]);
                        FileNames.Remove(FileNames[j + 1]);
                    }
                    j++;
                }
                i++;
            }
        }
        private void GetId()
        {
            GetFile();
            foreach (string item in FileNames) ID.Add(GetName(item));

        }
        private string GetName(string name)
        {
            XmlDocument document = new XmlDocument();
            document.Load("../../../Resources/" + name);

            XmlElement root = document.DocumentElement;

            foreach (XmlElement item in root)
            {
                if (item.HasChildNodes)
                {

                    if (item.Name == "cat_ru:DocumentID")
                    {
                        return item.InnerText;
                    }
                    else return null;


                }
                else return null;
            }
            return null;
        }
        private void GetFile()
        {

            IEnumerable<string> allfiles = Directory.EnumerateFiles("../../../Resources/", "*.xml");
            foreach (string filename in allfiles) FileNames.Add(filename.Remove(0, 19));
        }
        public MainWindow()
        {
            InitializeComponent();
            sort();
            List<Declarations> declarations = new List<Declarations>();
            foreach (string item in FileNames)
                declarations.Add(new Declarations { FileName = item, DocumentID = GetName(item)});
            Data.ItemsSource = declarations;

        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            Declarations path = Data.SelectedItem as Declarations;
            Information info = new Information(path.FileName);
            info.Show();
        }
    }
}