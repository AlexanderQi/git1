using System;
using System.Collections;
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
//using mysqlDao_v1;
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
            Loaded += UiDbConnector_Loaded;
            listBox_conn.SelectionChanged += ListBox_conn_SelectionChanged;
        }

        private void ListBox_conn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = listBox_conn.SelectedIndex + 1;
            string str = ConfigurationManager.ConnectionStrings[index].ConnectionString;
            textBox_conn.Text = str;
        }

        private void UiDbConnector_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConnInfo();
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
            listBox_conn.ItemsSource = null;
            IEnumerator ie = ConfigurationManager.ConnectionStrings.GetEnumerator();
            while (ie.MoveNext())
            {
                ConnectionStringSettings cs = ie.Current as ConnectionStringSettings;
                if (cs.Name.IndexOf("SqlServer") >= 0) continue;
                listBox_conn.Items.Add(cs.Name);
            }
        }

        private void SaveConnInfo()
        {
            //Server=127.0.0.1;Database=nttbl; User=root;Password=root;Charset=utf8; Pooling=true; Max Pool Size=16;
            StringBuilder sb = new StringBuilder();
            sb.Append(comboBox_dbip.Text).Append(";Database=").Append(comboBox_dbname.Text).Append(";User=").Append(comboBox_user.Text)
                .Append(";Password=").Append(passwordBox.Password).Append(";Charset=utf8; Pooling=true; Max Pool Size=16");
            string connStr = sb.ToString();
            textBox_conn.Text = connStr;
             
            sb.Clear();
            string connName = sb.Append(comboBox_dbip.Text).Append("-").Append(comboBox_dbname.Text).ToString();

            sb.Append("--").Append(connStr);
            log.Debug(sb.ToString());

            try
            {
                Configuration cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);   
                //cfg.AppSettings.Settings.Add(new KeyValueConfigurationElement(connName, connStr));
                cfg.ConnectionStrings.ConnectionStrings.Add(new ConnectionStringSettings(connName, connStr));
                cfg.Save();
                //ConfigurationManager.RefreshSection("appSettings");
                ConfigurationManager.RefreshSection("connectionStrings");
            }
            catch (Exception e)
            {
                log.Error(e);
                MessageBox.Show(e.ToString(), "An error occurred.");
            }

        }

        private void button_save_Click(object sender, RoutedEventArgs e)
        {
            SaveConnInfo();
        }
    }
}
