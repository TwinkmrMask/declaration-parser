using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml;
using System;

namespace WpfApp3
{
    public partial class Information : Window
    {
        public Information()
        {
            InitializeComponent();
        }
        void print(string[] value)
        {
            if (value != null)
                block.Text += $"{value[0]} | {value[1]}\n";
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
                    Process.Start(Path.GetFullPath(defaultSettings.path + defaultSettings.nameExcelFile));
                }
                catch (System.ComponentModel.Win32Exception)
                {
                    MessageBox.Show("Вы прекратили установку MS Excel", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }


        }
    }
}