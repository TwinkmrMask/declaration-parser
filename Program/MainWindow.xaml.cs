using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.IO;
using System.Xml;
using database;
namespace xmlparser
{
    public partial class MainWindow
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
        //void put(string fileName) => XmlAdapter adapter = new XmlAdapter(fileName);
        private void sort(in string[] path)
        {
            getId(in path);
            ID.Distinct();
        }
        private void getId(in string[] path)
        {
            getFile(in path);
            foreach (string item in FileNames) ID.Add(getName(in path, item));
        }
        private string getName(in string[] path, string name)
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
        private void getFile(in string[] path)
        {
            IEnumerable<string> allfiles = Directory.EnumerateFiles(path[0].Replace(path[1], ""), "*.xml");
            foreach (string filename in allfiles) FileNames.Add(filename);
        }
        private void setSource()
        {
            try
            {
                string[] path = getPath();
                if (path.Any(values => string.IsNullOrWhiteSpace(values)))
                {
                    throw new System.IO.IOException("Null path");
                }
                else
                {
                    sort(in path);
                    //put(path[0]);
                    List<Content> declarations = new List<Content>();
                    foreach (string item in FileNames)
                        declarations.Add(new Content { FileName = item, DocumentID = getName(in path, item) });
                    Data.ItemsSource = declarations;
                }
            }

            catch (System.IO.IOException e) when (e.Message == "Null path")
            {
                if (MessageBox.Show("Выберите файл или закройте приложение", "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error) == MessageBoxResult.OK)
                    setSource();
                else
                    this.Close();
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            setSource();
        }
        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            Content path = Data.SelectedItem as Content;
            Information info = new Information(path.FileName);
            info.Show();
        }
    }
}