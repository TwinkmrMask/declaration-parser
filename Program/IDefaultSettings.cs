using System.Collections.Generic;
using System.IO;

namespace XmlParser
{
    public interface IDefaultSettings
    {
        static string DefaultPath {
            get
            {
                string path = "../../../Resources/";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
        }
        static string NameExcelFile => DefaultPath + "declarationInfo.xlsx";
        static string IndexFileName => DefaultPath + "links";
        static string DataFileName => DefaultPath + "db";

        static readonly List<string> TransportDocumentCodes = new()
        {
            "02011", "02012", "02013",
            "02014", "02015", "02016",
            "02017", "02018", "02019",
            "02020", "02021", "02022",
            "02024", "02025", "02099"
        };
    }
}
