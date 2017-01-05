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
using log4net;
using MySql.Data.MySqlClient;
using mysqlDao_v1;
using System.Configuration;

namespace sc_dbmgr_v1
{
    /// <summary>
    /// uiDbConnector.xaml 的交互逻辑
    /// </summary>
    public partial class uiDbConnector : UserControl
    {
        ILog log;
        public uiDbConnector()
        {
            InitializeComponent();
            log = LogManager.GetLogger("log");
            button_conn.Click += Button_conn_Click;
        }

        private void Button_conn_Click(object sender, RoutedEventArgs e)
        {
           // throw new NotImplementedException();
        }

        private void button_new_Click(object sender, RoutedEventArgs e)
        {
            //string msg = "新建连接";
            //log.Info(msg);
            //log.Debug(msg);
            //log.Warn(msg);
            //log.Error(msg)

            ClearEditable();
            
        }

        private void ClearEditable()
        {
            comboBox_dbip.Text = comboBox_dbname.Text = comboBox_user.Text = "";
            radioButton_mysql.IsChecked = true;
            checkBox_pw.IsChecked = false;
        }

        private void LoadConnInfo()
        {
            string conn = ConfigurationManager.ConnectionStrings["mysql_nttbl"].ToString();
        }

        private void SaveConnInfo()
        {
            //Server=127.0.0.1;Database=nttbl; User=root;Password=root;Charset=utf8; Pooling=true; Max Pool Size=16;
            StringBuilder sb = new StringBuilder();

        }
    }
}
