using System.Collections.Generic;

namespace XmlParser
{
    public interface IDefaultSettings
    {
        const string DefaultPath = "../../../Resources/";

        static string NameExcelFile => IDefaultSettings.DefaultPath + "declarationInfo.xlsx";
        static string IndexFileName => IDefaultSettings.DefaultPath + "links";
        static string DataFileName => IDefaultSettings.DefaultPath + "db";

        private static readonly List<string> TransportDocumentCodes = new()
        {
            "02011", "02012", "02013",
            "02014", "02015", "02016",
            "02017", "02018", "02019",
            "02020", "02021", "02022",
            "02024", "02025", "02099"
        };
        
        static void AddTransportCodes()
        {
            using var data = new DataBase();
            foreach (var code in TransportDocumentCodes)
                data.CreateTransportCodeLink(code);
        }
    }
}
