using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.IO;
using System.Xml;
using TryMethods;

namespace XmlParser
{
    public partial class MainWindow
    {
        private  List<string> _fileNames = new();
        private bool _dataBaseFlag;

        private void SetSource()
        {
            try
            {
                var fileName = TryMethods.TryMethods.TryGetFilename();
                if (new[] { Path.GetDirectoryName(fileName), fileName}.Any(string.IsNullOrWhiteSpace))
                    throw new IOException("Null path");
                _fileNames = TryMethods.TryMethods.TryGetFiles(Path.GetDirectoryName(fileName), "*.xml");
                Data.ItemsSource = OpenFromDirectory();
            }

            catch (IOException e) when (e.Message == "Null path")
            {
                if (ExceptionMethods.ActionException("Выберите файл или закройте приложение", "Пустой путь"))
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
                using XmlAdapter adapter = new(IDefaultSettings.DataFileName, IDefaultSettings.IndexFileName);
                var path1 = adapter.GetContent(path.FileName);
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
            using XmlAdapter adapter = new(IDefaultSettings.DataFileName, IDefaultSettings.IndexFileName);
            if (adapter.IsLinks(name))
                return;

            XmlDocument document = new();
            document.Load(name);
            var root = document.DocumentElement;
            
            var xml = root!.OuterXml;
            var filename = Path.GetFileName(name);
            adapter.CreateLink(filename, xml);
        }
        
        private static List<Content> OpenFromDatabase()
        {
            List<Content> contents = new();
            using XmlAdapter adapter = new(IDefaultSettings.DataFileName, IDefaultSettings.IndexFileName);
            var data = adapter.GetAllFileNames();
            if (data != default) contents.AddRange(data.Select(para => new Content { FileName = para }));
            return contents;
        }
        private void Open_Click(object sender, RoutedEventArgs e) => Data.ItemsSource = OpenData(!_dataBaseFlag);
        private IEnumerable<Content> OpenData(bool dataBaseFlag) => dataBaseFlag ? OpenFromDatabase() : OpenFromDirectory();
        private List<Content> OpenFromDirectory() => _fileNames.Select(item => new Content { FileName = item }).ToList();
    }
}
