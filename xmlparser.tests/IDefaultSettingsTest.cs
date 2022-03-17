using System.IO;
namespace xmlparser.tests
{
    internal interface IDefaultSettingsTest
    {
        static string DefaultPath
        {
            get
            {
                string path = "../../Resources/";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
        }
        static string DefaultXmlFileName
        {
            get { return DefaultXmlFileName; }
            set { DefaultXmlFileName = $"TestFile {value}.xml"; }
        }

        const string DefaultDatabaseFilename = "db.test";
        const string DefaultDatabaseLinks = "links.test";
    }
}
