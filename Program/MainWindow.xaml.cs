using System.Collections.Generic;
using System.Windows;
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
        string[] getPath()
        {
            Microsoft.Win32.OpenFileDialog openDialog = new Microsoft.Win32.OpenFileDialog();
            openDialog.Filter = "Файл Xml|*.Xml";
            var result = openDialog.ShowDialog();
            string fileName = System.IO.Path.GetFileName(openDialog.FileName);
            string path = openDialog.FileName;
            return new string[] { path, fileName };
        }
        private void sort(in string[] path)
        {
            GetId(in path);
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
        private void GetId(in string[] path)
        {
            GetFile(in path);
            foreach (string item in FileNames) ID.Add(GetName(in path, item));

        }
        private string GetName(in string[] path, string name)
        {
            XmlDocument document = new XmlDocument();
            
            document.Load(path[0]);

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
        private void GetFile(in string[] path)
        {
            IEnumerable<string> allfiles = Directory.EnumerateFiles(path[0].Replace(path[1],""), "*.xml");
            foreach (string filename in allfiles) FileNames.Add(filename);
        }
        private void setSource()
        {
            string[] path = getPath();
            sort(in path);
            List<Declarations> declarations = new List<Declarations>();
            foreach (string item in FileNames)
                declarations.Add(new Declarations { FileName = item, DocumentID = GetName(in path, item) });
            Data.ItemsSource = declarations;
        }
        public MainWindow()
        {
            InitializeComponent();
            setSource();
        }
        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            Declarations path = Data.SelectedItem as Declarations;
            Information info = new Information(path.FileName);
            info.Show();
        }
    }
}