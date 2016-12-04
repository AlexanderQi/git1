using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using System.Configuration;
using DevExpress.Xpf.Core;

namespace datashow
{
    /// <summary>
    /// PageEquip.xaml 的交互逻辑
    /// </summary>
    public partial class PageEquip : Page
    {
        Page_JL instance;
        public static int DataErr = 0;
        DataTable mydt = new DataTable();
        public Action act_load;


        public PageEquip()
        {
            ThemeManager.SetThemeName(this, "Office2013DarkGray");
            InitializeComponent();
            Loaded += Page_Loaded;
            act_load = new Action(LoadData);
        }

        private void load_callback(IAsyncResult ar)
        {
            (ar.AsyncState as Action).EndInvoke(ar);
            if (DataErr == 0)
                Dispatcher.Invoke(RefreshData);
        }

        private void RefreshData()
        {
            //throw new NotImplementedException();  //dev grid控件无法异步载入数据源
           
            dg.ItemsSource = mydt;
           
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //Action_Data();  //异步调用
            dg.ShowLoadingPanel = true;
            LoadData();
            RefreshData();
            dg.ShowLoadingPanel = false;

        }

        public void Action_Data()
        {
            act_load.BeginInvoke(load_callback, act_load);
        }

        public void LoadData()
        {
            string conn = ConfigurationManager.ConnectionStrings["mysql_nttbl"].ToString();
            MySqlConnection myconn = new MySqlConnection(conn);
            try
            {
                myconn.Open();
                string sql = "select t.NAME 设备名称,t.YCVALUE 量测值,t.REFRESHTIME 刷新时间 from tblycvalue t where t.YCVALUE>0;";
                MySqlDataAdapter adapter = new MySqlDataAdapter(sql, myconn);
                adapter.Fill(mydt);
                myconn.Close();
                DataErr = 0;
                // Thread.Sleep(10000);   
            }
            catch (Exception e)
            {
                DataErr = -1;
                MessageBox.Show(e.ToString());
            }
        }
    }
}
