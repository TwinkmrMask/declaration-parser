using System.Collections.Generic;
namespace xmlparser
{
    public partial class Information
    {
        string path;
        Handler handler;
        public Information(string path)
        {
            InitializeComponent();
            this.path = path;
            this.handler = new Handler();
            start(this.path);
        }

        void toolbarStatus(string status) => toolbar.Content = status;
        void start(string path) => print(this.handler.XmlHandler(path));
        void print(List<(string, string)> value)
        {
            foreach((string, string) item in value)
                if ((item.Item1 != null) || (item.Item2 != null))
                    block.Text += $"{item.Item1} | {item.Item2}\n";
        }

        //toolbar status
        private void OpenInExcel_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e) => toolbarStatus("Открыть в MS Excel");
        private void OpenInExcel_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e) => toolbarStatus(null);

    }
}