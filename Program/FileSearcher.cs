using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
namespace XmlParser
{
    public class FileSearcher
    {
        private static void Copy(string text) => Clipboard.SetText(text);
        
        //TryMethods
        public List<string> TryGetFiles(string directory, string format)
        {
            try { return GetFiles(directory, format); }
            catch (Exception ex)
            {
                if (IDefaultSettings.Exception($"Ошибка - {ex.HResult}\nНажмите OK чтобы скопировать код ошибки, или нажмите Отмена чтобы переписать самостоятельно",
                    "Неизвестная ошибка", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    Copy(ex.HResult.ToString());
                return default;
            }
        }
        public (string directoryName, string fileName) TryGetPath()
        {
            try
            {
                return GetPath();
            }
            catch (Exception ex)
            {
                if (IDefaultSettings.Exception($"Ошибка - {ex.HResult}\nНажмите OK чтобы скопировать код ошибки, или нажмите Отмена чтобы переписать самостоятельно",
                    "Ошибка получения пути", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    Copy(ex.HResult.ToString());
                return default;
            }
        }

        //MainMethods
        protected List<string> GetFiles(string directory, string format)
        {
            List<string> _fileNames = new();
            var allFiles = Directory.EnumerateFiles(directory, format);
            foreach (var filename in allFiles) _fileNames.Add(filename);
            return _fileNames;
        }
        protected (string directoryName, string fileName) GetPath()
        {
            var openDialog = new Microsoft.Win32.OpenFileDialog { Filter = "Файл Xml|*.xml" };
            openDialog.ShowDialog();
            return (Path.GetDirectoryName(openDialog.FileName), openDialog.FileName);
        }
    }
}