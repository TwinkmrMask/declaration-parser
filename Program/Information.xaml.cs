using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;

namespace xmlparser
{
    public partial class Information
    {
        string path;
        Handler handler;
        public Information(string path)
        {
            InitializeComponent();
            this.path = path;
            this.handler = new Handler();
            start(this.path);
        }

        void toolbarStatus(string status) => toolbar.Content = status;
        void start(string path) => print(this.handler.XmlHandler(path));
        void print(List<(string, string)> value)
        {
            foreach((string, string) item in value)
                if ((item.Item1 != null) || (item.Item2 != null))
                    block.Text += $"{item.Item1} | {item.Item2}\n";
        }

        //toolbar status
        private void OpenInExcel_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e) => toolbarStatus("Открыть в MS Excel");
        private void OpenInExcel_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e) => toolbarStatus(null);

        private void OpenInExcel_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-ru");
            try
            {
                Process.Start(Path.GetFullPath(this.handler.Path()));
            }
            catch (System.ComponentModel.Win32Exception)
            {
                var result = MessageBox.Show(
                    "Невозможно запустить файл на этом устройстве,\n" +
                    "Вы можете скопировать данные из текстового окна\n" +
                    "Да - скопировать текст в буфер обмена\n" +
                    "Нет - текст не скопируется, а это сообщение закроется\n" +
                    "Отмена - текст не скопируется, а приложение закроется",
                    "Error", 
                    MessageBoxButton.YesNoCancel, 
                    MessageBoxImage.Error);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        Clipboard.SetText(block.Text);
                        break;
                    case MessageBoxResult.No:
                        //nothing
                        break;
                    case MessageBoxResult.Cancel:
                        this.Close();
                        break;
                }
                    
            }
        }
    }
}