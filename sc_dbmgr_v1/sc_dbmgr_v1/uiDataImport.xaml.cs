using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Collections;
using System.Configuration;

namespace sc_dbmgr_v1
{
    /// <summary>
    /// uiDataImport.xaml 的交互逻辑
    /// </summary>
    public partial class uiDataImport : UserControl
    {
        public uiDataImport()
        {
            InitializeComponent();
            button_file.Click += Button_file_Click;
            button_import.Click += Button_import_Click;
            uiDbConnector.Instance.ConnectStringsChanged += Instance_ConnectStringsChanged;

        }

        private void Instance_ConnectStringsChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            comboBox_dbc.Items.Clear();
            IEnumerator ie = ConfigurationManager.ConnectionStrings.GetEnumerator();
            while (ie.MoveNext())
            {
                ConnectionStringSettings cs = ie.Current as ConnectionStringSettings;
                if (cs.Name.IndexOf("SqlServer") >= 0) continue;

                comboBox_dbc.Items.Add(cs.Name);
            }
            if (comboBox_dbc.Items.Count > 0)
                comboBox_dbc.SelectedIndex = 0;
        }

        private void Button_import_Click(object sender, RoutedEventArgs e)
        {
            textBox_info.AppendText("数据库尚未准备好\n");
        }

        private void Button_file_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "数据库文件|*.sql";
            of.FilterIndex = 0;
            if (of.ShowDialog() == false)
                return;
            textBox_file.Text = of.FileName;

        }
    }
}
