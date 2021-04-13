using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.IO;
using System.Xml;
using database;
using System;

namespace xmlparser
{
    public partial class MainWindow
    {
        List<string> FileNames = new List<string>();
        List<string> ID = new List<string>();

        void copy(string text) => Clipboard.SetText(text);

        (string, string) getPath()
        {
            try
            {
                Microsoft.Win32.OpenFileDialog openDialog = new Microsoft.Win32.OpenFileDialog();
                openDialog.Filter = "Файл Xml|*.Xml";
                var result = openDialog.ShowDialog();
                string fileName = System.IO.Path.GetFileName(openDialog.FileName);
                string path = openDialog.FileName;
                return new (path, fileName);
            }
            catch (Exception exception1)
            {
                if (MessageBox.Show( $"Ошибка - {exception1.HResult}\nНажмите OK чтобы скопировать код ошибки, или нажмите Отмена чтобы переписать самостоятельно",
                    "Ошибка получения пути", MessageBoxButton.OKCancel,
                    MessageBoxImage.Error) == MessageBoxResult.OK)
                    copy(exception1.HResult.ToString());
                return default;
            }
        }
        
        private void sort(in (string, string) path)
        {
            try
            {
                getId(in path);
                ID.Distinct();
            }
            catch (Exception exception2)
            {
                if (MessageBox.Show($"Ошибка - {exception2.HResult}\nНажмите OK чтобы скопировать код ошибки, или нажмите Отмена чтобы переписать самостоятельно",
                    "Неизвестная ошибка", MessageBoxButton.OKCancel,
                    MessageBoxImage.Error) == MessageBoxResult.OK)
                    copy(exception2.HResult.ToString());
            }
        }
        private void getId(in (string, string) path)
        {
            try
            {
                getFile(in path);
                foreach (string item in FileNames) ID.Add(path.Item2);
            }
            catch (Exception exception3)
            {
                if (MessageBox.Show($"Ошибка - {exception3.HResult}\nНажмите OK чтобы скопировать код ошибки, или нажмите Отмена чтобы переписать самостоятельно",
                    "Неизвестная ошибка", MessageBoxButton.OKCancel,
                    MessageBoxImage.Error) == MessageBoxResult.OK)
                    copy(exception3.HResult.ToString());
            }
        }
        
        private void getFile(in (string, string) path)
        {
            try
            {
                IEnumerable<string> allfiles = Directory.EnumerateFiles(path.Item1.Replace(path.Item2, ""), "*.xml");
                foreach (string filename in allfiles) FileNames.Add(filename);
            }

            catch (Exception exception4)
            {
                if (MessageBox.Show("Нажмите OK чтобы скопировать код ошибки, или нажмите Отмена чтобы переписать самостоятельно",
                    "Ошибка получения файла", MessageBoxButton.OKCancel,
                    MessageBoxImage.Error) == MessageBoxResult.OK)
                    copy(exception4.HResult.ToString());

            }
        }


        private void setSource()
        {
            try
            {
                (string, string) path = getPath();
                if (string.IsNullOrWhiteSpace(path.Item1) && string.IsNullOrWhiteSpace(path.Item2))
                    throw new System.IO.IOException("Null path");
                else
                {
                    sort(in path);
                    List<Content> declarations = new List<Content>();
                    foreach (string item in FileNames)
                        declarations.Add(new Content { FileName = item, DocumentID = path.Item2 });
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
            info.Show();
        }
    }
}