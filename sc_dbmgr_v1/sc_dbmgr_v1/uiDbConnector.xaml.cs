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
using mysqlDao_v1;

namespace sc_dbmgr_v1
{
    /// <summary>
    /// uiDbConnector.xaml 的交互逻辑
    /// </summary>
    public partial class uiDbConnector : UserControl
    {
        ILog log;
        static string Txt_connectionStrings = "connectionStrings";
        public uiDbConnector()
        {
            InitializeComponent();
            log = LogManager.GetLogger("log");
            button_conn.Click += Button_conn_Click;
            Loaded += UiDbConnector_Loaded;
            listBox_conn.SelectionChanged += ListBox_conn_SelectionChanged;
            button_del.Click += Button_del_Click;
            button_save.Click += button_save_Click;
            button_test.Click += Button_test_Click;
        }

        private char getDbSign()
        {
            if (radioButton_mysql.IsChecked == true)
                return 'M';
            else
                return 'O';
            
        }
        private void Button_test_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                mysqlDAO dao = new mysqlDAO(textBox_conn.Text);
                dao.TestConnect();
                log.Info("连接成功");
                MessageBox.Show("连接成功");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                MessageBox.Show("连接不成功,请检查网络地址，用户名，密码和数据库名。");
            }
        
        }

        private void Button_del_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (listBox_conn.SelectedItem == null) return;
                string str = listBox_conn.SelectedItem.ToString();
                Configuration cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                cfg.ConnectionStrings.ConnectionStrings.Remove(str);
                cfg.Save();
                ConfigurationManager.RefreshSection(Txt_connectionStrings);
                ClearEditable();
                LoadConnInfo();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                MessageBox.Show(ex.ToString());
            }
        }



        private void ListBox_conn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (listBox_conn.SelectedItem == null) return;
                int index = listBox_conn.SelectedIndex + 1;
                string str = ConfigurationManager.ConnectionStrings[index].ConnectionString;
                textBox_conn.Text = str;
                string[] sp = str.Split('=', ';');
                str = ConfigurationManager.ConnectionStrings[index].Name;
                string[] ss = str.Split('-');
                radioButton_mysql.IsChecked = (ss[0] == "M") ? true : false;
                radioButton_ora.IsChecked = (ss[0] == "M") ? false : true;
                comboBox_dbip.Text = ss[1];
                comboBox_dbname.Text = ss[2];
                comboBox_user.Text = ss[3];
                passwordBox.Password = sp[7];
            }
            catch (Exception ex)
            {
                log.Error(ex);
                MessageBox.Show(ex.ToString());
                
            }
        }

        private bool IsFirstLoad = true;
        private void UiDbConnector_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsFirstLoad)
            {
                IsFirstLoad = false;
                LoadConnInfo();
            }
        }

        private void Button_conn_Click(object sender, RoutedEventArgs e)
        {
           // throw new NotImplementedException();
        }

        private void button_new_Click(object sender, RoutedEventArgs e)
        {
            ClearEditable();
        }

        private void ClearEditable()
        {
            textBox_conn.Text = comboBox_dbip.Text = comboBox_dbname.Text = comboBox_user.Text = "";
            radioButton_mysql.IsChecked = true;
            passwordBox.Password = "";
        }

        private void LoadConnInfo()
        {
            listBox_conn.Items.Clear();
            comboBox_dbip.Items.Clear();
            comboBox_dbname.Items.Clear();
            comboBox_user.Items.Clear();
            textBox_conn.Text = "";
            IEnumerator ie = ConfigurationManager.ConnectionStrings.GetEnumerator();
            while (ie.MoveNext())
            {
                ConnectionStringSettings cs = ie.Current as ConnectionStringSettings;
                if (cs.Name.IndexOf("SqlServer") >= 0) continue;
                listBox_conn.Items.Add(cs.Name);
                string[] strs = cs.ConnectionString.Split('=', ';');
                comboBox_dbip.Items.Add(strs[1]);
                comboBox_dbname.Items.Add(strs[3]);
                comboBox_user.Items.Add(strs[5]);
            }
        }

        private void ClearConnInfo()
        {
            listBox_conn.Items.Clear();
        }

        private void SaveConnInfo()
        {
            //Server=127.0.0.1;Database=nttbl; User=root;Password=root;Charset=utf8; Pooling=true; Max Pool Size=16;
            StringBuilder sb = new StringBuilder();
            sb.Append("Server=").Append(comboBox_dbip.Text).Append(";Database=")
                .Append(comboBox_dbname.Text).Append(";User=").Append(comboBox_user.Text)
                .Append(";Password=").Append(passwordBox.Password).Append(";Charset=utf8; Pooling=true; Max Pool Size=16;");
            string connStr = sb.ToString();
            textBox_conn.Text = connStr;
             
            sb.Clear();
            string connName = sb.Append(getDbSign()).Append('-').Append(comboBox_dbip.Text).Append('-').Append(comboBox_dbname.Text).Append('-').Append(comboBox_user.Text).ToString();

            sb.Append("--").Append(connStr);
            log.Debug(sb.ToString());

            try
            {
                Configuration cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);   
                //cfg.AppSettings.Settings.Add(new KeyValueConfigurationElement(connName, connStr));
                if(cfg.ConnectionStrings.ConnectionStrings[connName] != null)
                {
                    MessageBox.Show("连接记录已保存");
                    return;
                }
                cfg.ConnectionStrings.ConnectionStrings.Add(new ConnectionStringSettings(connName, connStr));
                cfg.Save();
                //ConfigurationManager.RefreshSection("appSettings");
                ConfigurationManager.RefreshSection(Txt_connectionStrings);
                LoadConnInfo();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                MessageBox.Show(ex.ToString());
            }

        }

        private void button_save_Click(object sender, RoutedEventArgs e)
        {
            SaveConnInfo();
        }
    }
}
