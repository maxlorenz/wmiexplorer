using System;
using System.Collections.Generic;
using System.Management;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace WMI_Explorer
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            foreach (string ns in WMI.getNameSpaces())
            {
                lstNS.Items.Add(ns);
            }

            lstNS.SelectedItem = "CIMV2";
        }

        public class TwoValueItem
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            if (txtSearch.Text == "" || lstNS.SelectedItem == null)
                return;

            lstClasses.Items.Clear();

            foreach (string wmiClass in WMI.searchClasses(lstNS.SelectedItem.ToString(), txtSearch.Text))
            {
                lstClasses.Items.Add(wmiClass);
            }
        }

        private void lstClasses_DoubleClick(object sender, SelectionChangedEventArgs e)
        {
            if (lstNS.SelectedItem == null || lstClasses.SelectedItem == null)
                return;

            lstProperties.Items.Clear();
            lstClasses.IsEnabled = false;

            string selectedClass = lstClasses.SelectedItem.ToString();

            lstProperties.Items.Add(new TwoValueItem { Name = "Searching in " + selectedClass, Value = "" });

            Thread bgThread = new Thread(new ThreadStart((Action) (() => {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * from " + selectedClass);
                ManagementObjectCollection oc = searcher.Get();

                List<TwoValueItem> results = new List<TwoValueItem>();

                foreach (ManagementObject mo in oc)
                {
                    foreach (PropertyData prop in mo.Properties)
                    {
                        if (prop.Value != null) {
                            if (prop.Value.GetType().BaseType == typeof(Array)) {
                                foreach (object value in (prop.Value as Array))
                                {
                                    results.Add(new TwoValueItem { Name = prop.Name, Value = value.ToString() });
                                }    
                            }
                            else
                            {
                                results.Add(new TwoValueItem { Name = prop.Name, Value = prop.Value.ToString() });
                            }
                        }
                        else
                        {
                            results.Add(new TwoValueItem { Name = prop.Name, Value = "{empty}" });
                        }
                              
                    }
                }

                lstProperties.Dispatcher.BeginInvoke((Action)(() =>
                {
                    lstProperties.Items.Clear();

                    foreach (TwoValueItem item in results)
                    {
                        lstProperties.Items.Add(item);
                    }

                    lstClasses.IsEnabled = true;
                }));
            })));

            bgThread.IsBackground = true;
            bgThread.Start();
        }

        private void lstNS_DoubleClick(object sender, SelectionChangedEventArgs e)
        {
            if (lstNS.SelectedItem == null)
                return;

            lstClasses.Items.Clear();
            lstProperties.Items.Clear();

            foreach (string wmiClass in WMI.getClasses(lstNS.SelectedItem.ToString()))
            {
                lstClasses.Items.Add(wmiClass);
            }
        }

        private void lstProperties_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var gv = (lstProperties.View as GridView);
            var margin = 20; // hides the scrollbar

            foreach (var col in gv.Columns)
            {
                col.Width = lstProperties.ActualWidth / gv.Columns.Count - margin;
            }
        }

        private void CopyPropertyQueryToClipboard_Click(object sender, RoutedEventArgs e)
        {
            string properties = "";

            if (lstProperties.SelectedItems.Count > 1) {

                var propertyList = new List<string>();

                foreach (var property in lstProperties.SelectedItems)
                    propertyList.Add((property as TwoValueItem).Name);

                properties = String.Join(", ", propertyList);
            }
            else
                properties = (lstProperties.SelectedItem as TwoValueItem).Name;

            string query = "wmic path " + lstClasses.SelectedItem.ToString() + " get " + properties;

            ExportQuery eq = new ExportQuery(query);
            eq.Show();
        }

        private void CopyClassQueryToClipboard_Click(object sender, RoutedEventArgs e)
        {
            var wmiClass = lstClasses.SelectedItem.ToString();

            string query = "wmic path " + wmiClass + " get *";
            ExportQuery eq = new ExportQuery(query);
            eq.Show();
        }


    }
}
