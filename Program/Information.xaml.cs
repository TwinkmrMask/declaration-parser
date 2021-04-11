using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            try
            {
                MessageBox.Show(Path.GetFullPath(this.handler.Path()));
                Process.Start(Path.GetFullPath(this.handler.Path()));
            }
            /*
            if (officeType == null)
            {
                MessageBox.Show(
                    "Похоже MS Excel не установлен на Вашем пк,\n Вы можете скопировать содержимое текстового поля\nили прекратить операцию",
                    "Warning",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            
            else
            {
            */
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
            //}
        }
    }
}