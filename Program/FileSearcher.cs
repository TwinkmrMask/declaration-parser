using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

    class FileSearcher
    {
        private static void Copy(string text) => Clipboard.SetText(text);
        public List<string> GetFiles(string directory, string format) 
        {
            if (string.IsNullOrEmpty(directory))
            {
                throw new ArgumentException($"\"{nameof(directory)}\" не может быть неопределенным или пустым.", nameof(directory));
            }

            if (string.IsNullOrEmpty(format))
            {
                throw new ArgumentException($"\"{nameof(format)}\" не может быть неопределенным или пустым.", nameof(format));
            }

            List<string> _fileNames = new();
            try
            {
                var allFiles = Directory.EnumerateFiles(directory, format);
                foreach (var filename in allFiles) _fileNames.Add(filename);
            }

            catch (Exception exception4)
            {
                if (MessageBox.Show($"Ошибка - {exception4.HResult}\nНажмите OK чтобы скопировать код ошибки, или нажмите Отмена чтобы переписать самостоятельно",
                    "Неизвестная ошибка", MessageBoxButton.OKCancel,
                    MessageBoxImage.Error) == MessageBoxResult.OK)
                    Copy(exception4.HResult.ToString());
                    
            }

            return _fileNames;
        }
        public (string directoryName, string fileName)  GetPath()
        {
            try
            {
                var openDialog = new Microsoft.Win32.OpenFileDialog { Filter = "Файл Xml|*.xml" };
                openDialog.ShowDialog();
                return (Path.GetDirectoryName(openDialog.FileName), openDialog.FileName);
            }
            catch (Exception exception1)
            {
                if (MessageBox.Show($"Ошибка - {exception1.HResult}\nНажмите OK чтобы скопировать код ошибки, или нажмите Отмена чтобы переписать самостоятельно",
                    "Ошибка получения пути", MessageBoxButton.OKCancel,
                    MessageBoxImage.Error) == MessageBoxResult.OK)
                    Copy(exception1.HResult.ToString());
            return default;
            }
        }
    }

