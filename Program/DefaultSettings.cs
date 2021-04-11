using System.Collections.Generic;
namespace database
{
    public abstract class DefaultSettings
    {
        protected static readonly string defaultPath = "../../Resources/";
        protected static readonly string nameExcelFile = "declarationInfo.xlsx";
        protected static readonly string indexFileName = "links";
        protected static readonly string dataFileName = "db";
        private static readonly List<string> transportDocumentCodes = new List<string>()
        {
            "02011", "02012", "02013",
            "02014", "02015", "02016",
            "02017", "02018", "02019",
            "02020", "02021", "02022",
            "02024", "02025", "02099"
        };
        protected void addTransportCodes(string indexFileName, string dataFileName, string defaultPath)
        {
            using(DataBase data = new DataBase(indexFileName, dataFileName, defaultPath))
                foreach (string code in transportDocumentCodes)
                    data.CreateTransportCodeLink(code);

        }
    }
}
