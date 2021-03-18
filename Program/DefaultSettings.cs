using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp3
{
    public abstract class DefaultSettings
    {
        protected readonly string path = "../../Resources/";
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
        DataBase data;
        protected void SetDefaultSettings()
        {
            this.data = new DataBase(indexFileName, dataFileName, path);
            addTransportCodes(this.data);
        }
        void addTransportCodes(DataBase data)
        {
            foreach (string code in transportDocumentCodes)
                data.CreateTransportCodeLink(code);
        }
    }
}
