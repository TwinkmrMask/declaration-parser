using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace XmlParser
{
    public partial class Information
    {
        private readonly Handler _handler;
        public Information(string path, bool flag)
        {
            InitializeComponent();
            _handler = new();
            Start(path);
        }
        private void ToolbarStatus(string status) => toolbar.Content = status;
        private void Start(string path) => Print(this._handler.XmlHandler(path));
        private void Print(IEnumerable<(string, string)> value)
        {
            foreach (var (item1, item2) in value.Where(item => (item.Item1 != null) || (item.Item2 != null)))
                block.Text += $"{item1} | {item2}\n";
        }
        private void OpenInExcel_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e) => ToolbarStatus("Открыть в MS Excel");
        private void OpenInExcel_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e) => ToolbarStatus(null);
        private void OpenInExcel_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var prc = new Process();
            prc.StartInfo.FileName = @Path.GetFullPath(Handler.GetPath());
            prc.StartInfo.UseShellExecute = true;
            prc.Start();
        }
    }
}