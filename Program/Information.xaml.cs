using System.Windows;
using System.Diagnostics;
using System.IO;
using System;
using System.Linq;
namespace xmlparser
{
    public partial class Information
    {
        string name;
        string path;
        public Information(string path, string name)
        {
            InitializeComponent();
            this.path = path;
            this.name = name;
        }
        void print((string, string) value)
        {
            if ((value.Item1 != null) || (value.Item2 != null))
                block.Text += $"{value.Item1} | {value.Item2}\n";
        }
        private void btn_Click(object sender, RoutedEventArgs e)
        {
            Type officeType = Type.GetTypeFromProgID("Excel.Application");
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
                try
                {
                    Process.Start(Path.GetFullPath(path + name));
                }
                catch (System.ComponentModel.Win32Exception ex) when (ex.Message == "Не удается найти указанный файл.")
                {
                    MessageBox.Show(ex.Message);
                    //MessageBox.Show("Вы прекратили установку MS Excel", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}