using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace FileSearcher
{
    public abstract class FileSearcher
    {
        protected static string GetFilename()
        {
            var openDialog = new Microsoft.Win32.OpenFileDialog { Filter = "Файл Xml|*.xml" };
            openDialog.ShowDialog();
            return openDialog.FileName;
        }
        protected static List<string> GetFiles(string directory, string format) => Directory.EnumerateFiles(directory, format).ToList();
    }
}