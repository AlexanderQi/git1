using System;
using System.Collections.Generic;
using System.Configuration;
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
using System.Data;

namespace datashow
{
    /// <summary>
    /// PageHGL.xaml 的交互逻辑
    /// </summary>
    public partial class PageHGL : Page
    {
        DataTable mydt = new DataTable();
        static int DataErr = 0;

        public PageHGL()
        {
            InitializeComponent();
            s1.ArgumentDataMember = "DTIME";
            s1.ValueDataMember = "YCVALUE";
            s1.ArgumentScaleType = DevExpress.Xpf.Charts.ScaleType.DateTime;

            this.Loaded += PageHGL_Loaded;
        }

        private void PageHGL_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

  

        public void DataAct(Action act)
        {
            Dispatcher.BeginInvoke(act, System.Windows.Threading.DispatcherPriority.Background);
        }

        public void LoadData()
        {
            string conn = ConfigurationManager.ConnectionStrings["mysql_nthis"].ToString();
            MySqlConnection myconn = new MySqlConnection(conn);
            try
            {
                myconn.Open();
                string sql = "select t.DTIME,t.YCVALUE from hisycvalue t where t.ID = 15373615 and t.DTIME > date_add(now(), interval - 7 day);";
                MySqlDataAdapter adapter = new MySqlDataAdapter(sql, myconn);
                adapter.Fill(mydt);
                myconn.Close();
                DataErr = 0;
                // Thread.Sleep(10000); 
                s1.BeginInit();  
                s1.DataSource = mydt;
                s1.EndInit();
            }
            catch (Exception e)
            {
                DataErr = -1;
                MessageBox.Show(e.ToString());
            }
        }

    
    }
}
