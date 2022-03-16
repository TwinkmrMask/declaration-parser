﻿using System.Collections.Generic;
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

        //получает путь к обрабаываемому файлу
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
                if (MessageBox.Show($"Ошибка - {exception1.HResult}\nНажмите OK чтобы скопировать код ошибки, или нажмите Отмена чтобы переписать самостоятельно",
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
                if (new[] { _directoryName, _fileName }.Any(string.IsNullOrWhiteSpace))
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
