using System.Windows;

namespace WMI_Explorer
{
    /// <summary>
    /// Interaktionslogik für ExportQuery.xaml
    /// </summary>
    public partial class ExportQuery : Window
    {
        string query;

        public ExportQuery(string query)
        {
            InitializeComponent();

            this.query = query;
            lblQuery.Content = query;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string exportOption = "";

            if (optCSV.IsChecked.Value)
                exportOption = " /format:CSV";

            if (optList.IsChecked.Value)
                exportOption = " /format:List";

            Clipboard.SetText(query + exportOption);
            this.Close();
        }
    }
}
