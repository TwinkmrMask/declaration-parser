using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using database;

namespace xmlparser
{
    public partial class Information 
    {
        private readonly Handler _handler;
        public Information(string path)
        {
            InitializeComponent();
            this._handler = new Handler();
            Start(path);
        }

        private void ToolbarStatus(string status) => toolbar.Content = status;
        private void Start(string path) => Print(this._handler.XmlHandler(path));

        private void Print(IEnumerable<(string, string)> value)
        {
            foreach (var item in value.Where(item => 
                (item.Item1 != null) || (item.Item2 != null)))
                block.Text += $"{item.Item1} | {item.Item2}\n";
        }

        //toolbar status
        private void OpenInExcel_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e) => ToolbarStatus("Открыть в MS Excel");
        private void OpenInExcel_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e) => ToolbarStatus(null);
        private void OpenInExcel_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-ru");
            try
            {
                Process.Start(Path.GetFullPath(this._handler.GetPath()));
            }
            catch (System.ComponentModel.Win32Exception)
            {
                var result = MessageBox.Show(
                    "Невозможно запустить файл на этом устройстве,\n" +
                    "Вы можете скопировать данные из текстового окна\n" +
                    "Да - скопировать текст в буфер обмена\n" +
                    "Нет - текст не скопируется, а это сообщение закроется\n" +
                    "Отмена - текст не скопируется, а окно закроется",
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