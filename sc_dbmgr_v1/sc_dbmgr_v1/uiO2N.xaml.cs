using log4net;
using mysqlDao_v1;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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

namespace sc_dbmgr_v1
{
    /// <summary>
    /// uiO2N.xaml 的交互逻辑
    /// </summary>
    public partial class uiO2N : UserControl
    {
        mysqlDAO mdao = null;
        ILog log;
        public uiO2N()
        {
            InitializeComponent();
            log = LogManager.GetLogger("log");
            button_connect.Click += Button_connect_Click;
            button_convert.Click += Button_convert_Click;
            uiDbConnector.Instance.ConnectStringsChanged += Instance_ConnectStringsChanged;
            listBox_base.SelectionChanged += ListBox_base_SelectionChanged;

        }

        private void ListBox_base_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mdao == null) return;
            listBox_table.Items.Clear();
            try
            {
                curbase = listBox_base.SelectedItem.ToString();
                string sql = "use " + curbase + ";";
                mdao.Execute(sql);
                DataTable dt = mdao.Query("show tables;");
                IEnumerator ie = dt.Rows.GetEnumerator();
                while (ie.MoveNext())
                {
                    DataRow dr = ie.Current as DataRow;
                    listBox_table.Items.Add(dr[0]);
                }

            }
            catch (Exception ex)
            {
                log.Error(ex);
                MessageBox.Show(ex.ToString());
            }
        }

        private void Instance_ConnectStringsChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            comboBox_db.Items.Clear();
            IEnumerator ie = ConfigurationManager.ConnectionStrings.GetEnumerator();
            while (ie.MoveNext())
            {
                ConnectionStringSettings cs = ie.Current as ConnectionStringSettings;
                if (cs.Name.IndexOf("SqlServer") >= 0) continue;

                comboBox_db.Items.Add(cs.Name);
            }
            if (comboBox_db.Items.Count > 0)
                comboBox_db.SelectedIndex = 0;
        }

        private void Button_convert_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox_db.SelectedItem == null || mdao == null)
            {
                MessageBox.Show("请先选择连接数据库.");
                return;
            }


        }

        private string curuser = null;
        private string curpw = null;
        private string curip = null;
        private string curbase = null;
        private void Button_connect_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox_db.SelectedItem == null) return;
            try
            {
                if (mdao != null)
                    mdao.ConnectClose();
                string cstr = ConfigurationManager.ConnectionStrings[comboBox_db.SelectedItem.ToString()].ConnectionString;
                mdao = new mysqlDAO(cstr);
                DataTable dt = mdao.Query("show databases;");
                IEnumerator ie = dt.Rows.GetEnumerator();
                listBox_base.Items.Clear();
                while (ie.MoveNext())
                {
                    DataRow dr = ie.Current as DataRow;
                    string cs = dr[0].ToString();
                    if (cs.IndexOf("_schema") >= 0 || cs.IndexOf("mysql") >= 0) continue;
                    listBox_base.Items.Add(cs);
                }
                string[] ss = cstr.Split('=', ';');
                curip = ss[1];
                curuser = ss[3];
                curpw = ss[5];
                curbase = ss[7];

                log.Debug(cstr);
                log.Debug(curip + "^" + curuser + "^" + curpw + "^" + curbase);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
