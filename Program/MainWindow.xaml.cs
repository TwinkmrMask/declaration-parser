using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.IO;
using System.Xml;

namespace XmlParser
{
    public partial class MainWindow
    {
        private  List<string> _fileNames = new();
        private bool _dataBaseFlag = false;

        private void SetSource()
        {
            try
            {
                FileSearcher file = new();
                var fileData = file.TryGetPath();
                if (new[] { fileData.directoryName, fileData.fileName}.Any(string.IsNullOrWhiteSpace))
                    throw new IOException("Null path");
                _fileNames = file.TryGetFiles(fileData.directoryName, "*.xml");
                Data.ItemsSource = OpenFromDirectory();
            }

            catch (IOException e) when (e.Message == "Null path")
            {
                if (IDefaultSettings.Exception("Выберите файл или закройте приложение", "Пустой путь", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    SetSource();
                else
                    Close();
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            SetSource();
        }
        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            Information info;
            if (Data.SelectedItem is not Content path) return;

            if (_dataBaseFlag)
            {
                using XmlAdapter adapter = new();
                string path1 = adapter.GetContent(path.FileName);
                info = new(path1);
            }
            else
            {
                info = new(path.FileName);
                AddFile(path.FileName);
            }
            info.Show();
        }
        private static void AddFile(string name)
        {
            using XmlAdapter adapter = new();
            if (adapter.IsLinks(name))
                return;

            XmlDocument document = new();
            document.Load(name);
            XmlElement root = document.DocumentElement;
            
            string xml = root.OuterXml;
            string filename = Path.GetFileName(name);
            adapter.CreateLink(xml, filename);
        }

        private List<Content> OpenFromDatabase()
        {
            List<Content> contents = new();
            using XmlAdapter @base = new();
            var data = @base.GetAllFileNames();
            if (data != default) contents.AddRange(data.Select(para => new Content { FileName = para }));
            return contents;
        }
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            _dataBaseFlag = !_dataBaseFlag;
            Data.ItemsSource = OpenData(_dataBaseFlag);
        }
        private List<Content> OpenData(bool _dataBaseFlag) => _dataBaseFlag ? OpenFromDatabase() : OpenFromDirectory();
        private List<Content> OpenFromDirectory() => _fileNames.Select(item => new Content { FileName = item }).ToList();
    }
}
