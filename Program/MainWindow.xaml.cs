using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.IO;
using System.Xml;
using System;

namespace XmlParser
{
    public partial class MainWindow
    {
        private readonly List<string> _fileNames = new();
        private string _directoryName;
        private string _fileName;
        private bool _dataBaseFlag = false;
        private static void Copy(string text) => Clipboard.SetText(text);
        
        //получает путь к лбрабаываемому файлуки
        private static void GetPath(out string directoryName, out string fileName)
        {
            try
            {
                var openDialog = new Microsoft.Win32.OpenFileDialog { Filter = "Файл Xml|*.xml" };
                openDialog.ShowDialog();
                directoryName = Path.GetDirectoryName(openDialog.FileName);
                fileName = openDialog.FileName;
            }
            catch (Exception exception1)
            {
                if (MessageBox.Show( $"Ошибка - {exception1.HResult}\nНажмите OK чтобы скопировать код ошибки, или нажмите Отмена чтобы переписать самостоятельно",
                    "Ошибка получения пути", MessageBoxButton.OKCancel,
                    MessageBoxImage.Error) == MessageBoxResult.OK)
                    Copy(exception1.HResult.ToString());
                directoryName = default;
                fileName = default;
            }
        }
        private void GetFile()
        {
            try
            {
                var allFiles = Directory.EnumerateFiles(_directoryName, "*.xml");
                foreach (var filename in allFiles) _fileNames.Add(filename);
            }

            catch (Exception exception4)
            {
                if (MessageBox.Show($"Ошибка - {exception4.HResult}\nНажмите OK чтобы скопировать код ошибки, или нажмите Отмена чтобы переписать самостоятельно",
                    "Неизвестная ошибка", MessageBoxButton.OKCancel,
                    MessageBoxImage.Error) == MessageBoxResult.OK)
                    Copy(exception4.HResult.ToString());

            }
        }
        private void SetSource()
        {
            try
            {
                GetPath(out this._directoryName, out this._fileName);
                if ( new[] { _directoryName, _fileName }.Any(string.IsNullOrWhiteSpace))
                    throw new IOException("Null path");
                GetFile();
                var declarations = _fileNames.Select(item => new Content 
                    { FileName = item }).ToList();
                Data.ItemsSource = declarations;
            }

            catch (IOException e) when (e.Message == "Null path")
            {
                if (MessageBox.Show("Выберите файл или закройте приложение", "Пустой путь", MessageBoxButton.OKCancel, MessageBoxImage.Error) == MessageBoxResult.OK)
                    SetSource();
                else
                    this.Close();
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
                info = new(adapter.GetContent(path.FileName), _dataBaseFlag);
            }
            else
            {                
                info = new(path.FileName, _dataBaseFlag);
                AddFile(path.FileName);
            }
            info.Show();
        }

        private static void AddFile(string name)
        {
            using var reader = XmlReader.Create(name);
            if (reader.IsEmptyElement) return;
            using XmlAdapter adapter = new();
            adapter.CreateLink("<a/>", Path.GetFileName(name));
        }

        private void OpenFromDatabase()
        {
            if (_dataBaseFlag == false)
                _dataBaseFlag = true;
            else _dataBaseFlag = false;

            List<Content> contents = new();
            using XmlAdapter @base = new();
            var data = @base.GetAllFileNames();
            if(data != default) contents.AddRange(data.Select(para => new Content { FileName = para }));
            Data.ItemsSource = contents;
            Data.Items.Refresh();
        }
        private void Change_Click(object sender, RoutedEventArgs e) {}
        private void Open_Click(object sender, RoutedEventArgs e) => OpenFromDatabase();
    }
}
