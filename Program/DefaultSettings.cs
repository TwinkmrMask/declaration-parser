using System.Collections.Generic;
namespace database
{
    public abstract class DefaultSettings
    {
        protected readonly string defaultPath = "../../Resources/";
        protected readonly string nameExcelFile = "declarationInfo.xlsx";
        protected readonly string indexFileName = "links";
        protected readonly string dataFileName = "db";
        protected readonly List<string> transportDocumentCodes = new List<string>()
        {
            "02011", "02012", "02013",
            "02014", "02015", "02016",
            "02017", "02018", "02019",
            "02020", "02021", "02022",
            "02024", "02025", "02099"
        };
        protected void addTransportCodes()
        {
            using(DataBase data = new DataBase(indexFileName, dataFileName, defaultPath))
                foreach (string code in transportDocumentCodes)
                    data.CreateTransportCodeLink(code);
        }
    }
}
