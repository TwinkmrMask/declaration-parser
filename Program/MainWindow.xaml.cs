using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.IO;
using System.Xml;
using database;
using System;

#pragma 

namespace xmlparser
{
    public partial class MainWindow : IDefaultSettings
    {
        List<string> FileNames = new List<string>();
        string directoryName;
        string fileName;

        private void copy(string text) => Clipboard.SetText(text);

        private void getPath(out string directoryName, out string fileName)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog openDialog = new Microsoft.Win32.OpenFileDialog();
                openDialog.Filter = "Файл Xml|*.xml";
                var result = openDialog.ShowDialog();
                directoryName = System.IO.Path.GetDirectoryName(openDialog.FileName);
                fileName = openDialog.FileName;
            }
            catch (Exception exception1)
            {
                if (MessageBox.Show( $"Ошибка - {exception1.HResult}\nНажмите OK чтобы скопировать код ошибки, или нажмите Отмена чтобы переписать самостоятельно",
                    "Ошибка получения пути", MessageBoxButton.OKCancel,
                    MessageBoxImage.Error) == MessageBoxResult.OK)
                    copy(exception1.HResult.ToString());
                directoryName = default;
                fileName = default;
            }
        }
        
        private void getFile()
        {
            try
            {
                IEnumerable<string> allfiles = Directory.EnumerateFiles(directoryName, "*.xml");
                foreach (string filename in allfiles) FileNames.Add(filename);
            }

            catch (Exception exception4)
            {
                if (MessageBox.Show($"Ошибка - {exception4.HResult}\nНажмите OK чтобы скопировать код ошибки, или нажмите Отмена чтобы переписать самостоятельно",
                    "Неизвестная ошибка", MessageBoxButton.OKCancel,
                    MessageBoxImage.Error) == MessageBoxResult.OK)
                    copy(exception4.HResult.ToString());

            }
        }
        private void setSource()
        {
            try
            {
                getPath(out this.directoryName, out this.fileName);
                if ( new string[] { directoryName, fileName }.Any(values => string.IsNullOrWhiteSpace(values)))
                    throw new System.IO.IOException("Null path");
                else
                {
                    getFile();
                    List<Content> declarations = new List<Content>();
                    foreach (string item in FileNames)
                        declarations.Add(new Content { FileName = item });
                    Data.ItemsSource = declarations;
                }
            }

            catch (System.IO.IOException e) when (e.Message == "Null path")
            {
                if (MessageBox.Show("Выберите файл или закройте приложение", "Пустой путь", MessageBoxButton.OKCancel, MessageBoxImage.Error) == MessageBoxResult.OK)
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
            addFile(path.FileName);
            info.Show();
        }

        private void addFile(string name)
        {
            XmlDocument document = new XmlDocument();
            document.Load(name);
            XmlElement root = document.DocumentElement;
            var adapter = new XmlAdapter(IDefaultSettings.indexFileName, IDefaultSettings.dataFileName, IDefaultSettings.defaultPath);
            if (root != null) adapter.CreateLink(Path.GetFileName(name), root.InnerXml);
        }
    }
}
