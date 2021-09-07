using System.Collections.Generic;
namespace database
{
    public interface IDefaultSettings
    {
        const string defaultPath = "../../../Resources/";

        const string nameExcelFile = "declarationInfo.xlsx"; 

        const string indexFileName = "links";
        const string dataFileName = "db";
        private static readonly List<string> transportDocumentCodes = new List<string>()
        {
            "02011", "02012", "02013",
            "02014", "02015", "02016",
            "02017", "02018", "02019",
            "02020", "02021", "02022",
            "02024", "02025", "02099"
        };

        static void AddTransportCodes(string indexFileName, string dataFileName, string defaultPath)
        {
            using(DataBase data = new DataBase(indexFileName, dataFileName, defaultPath))
                foreach (string code in transportDocumentCodes)
                    data.CreateTransportCodeLink(code);

        }
    }
}
