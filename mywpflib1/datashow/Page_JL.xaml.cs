using System;
using System.Collections.Generic;
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
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Data;
using System.Windows.Threading;

namespace datashow
{
    /// <summary>
    /// Page_JL.xaml 的交互逻辑
    /// </summary>
    public partial class Page_JL : Page
    {
        Page_JL instance;
        public static int DataErr = 0;
        DataTable mydt = new DataTable();
        public Action act_load;
       


        
        public Page_JL()
        {
            instance = this;
            InitializeComponent();
            act_load = new Action(LoadData);
                   
        }

        private void load_callback(IAsyncResult ar)
        {
            (ar.AsyncState as Action).EndInvoke(ar);
            if (DataErr == 0)
                Dispatcher.Invoke(RefreshData);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //mainpad.caption0 = g1.ActualWidth.ToString();
            vb.Width = g1.ActualWidth; //  缩放方法
            vb.Height = g1.ActualHeight;
            //LoadData();   //同步调用
             Action_Data();  //异步调用
          
        }

        public void Action_Data()
        {
            mainpad.caption0 = DateTime.Now.ToString();
            act_load.BeginInvoke(load_callback, act_load);
        }

        private MySqlConnection myconn = null;
        public void LoadData()
        {
            
            string conn = ConfigurationManager.ConnectionStrings["mysql_nttbl"].ToString();
            if (conn == null || conn.Equals("")) return;
            try
            {
                if (myconn != null)
                {
                    myconn.Close();
                }
                myconn = new MySqlConnection(conn);
                myconn.Open();
                string sql = "select t.YCVALUE,t.NAME,t.REFRESHTIME from tblycvalue t where t.NAME = '系统累计节能量' " +
"union select t.YCVALUE,t.NAME,t.REFRESHTIME from tblycvalue t where t.NAME = '节约标准煤' " +
"union select t.YCVALUE,t.NAME,t.REFRESHTIME from tblycvalue t where t.NAME = '系统累计经济效益' " +
"union select t.YCVALUE,t.NAME,t.REFRESHTIME from tblycvalue t where t.NAME = '二氧化碳减排量' "+
"union select t.YCVALUE,t.NAME,t.REFRESHTIME from tblycvalue t where t.NAME = '系统实时节能' " + 
"union select t.YCVALUE,t.NAME,t.REFRESHTIME from tblycvalue t where t.NAME = '系统当前电压合格率';";

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

        private string d2s(object o)
        {
            return ((double)o).ToString("f2");
        }
        public void RefreshData()
        {
            //MainWindow.instance.Title = "YC=" + mydt.Rows[0][0].ToString();
            mainpad.caption1 = mydt.Rows[0][1].ToString() + "：" + d2s(mydt.Rows[0][0]) + " kWh";
            mainpad.caption11 = "更新：" + mydt.Rows[0][2].ToString();

            mainpad.caption2 = mydt.Rows[1][1].ToString() + "：" + d2s(mydt.Rows[1][0]) + " kg";
            mainpad.caption22 = "更新：" + mydt.Rows[1][2].ToString();

            mainpad.caption3 = mydt.Rows[2][1].ToString() + "：" + d2s(mydt.Rows[2][0]) + " 元";
            mainpad.caption33 = "更新：" + mydt.Rows[2][2].ToString();

            mainpad.caption4 = mydt.Rows[3][1].ToString() + "：" + d2s(mydt.Rows[3][0]) + " kg";
            mainpad.caption44 = "更新：" + mydt.Rows[3][2].ToString();

            mainpad.caption5 = mydt.Rows[4][1].ToString() + "：" + d2s(mydt.Rows[4][0]) + " kW";
            mainpad.caption55 = "更新：" + mydt.Rows[2][2].ToString();

            mainpad.caption6 = mydt.Rows[5][1].ToString() + "：" + d2s(mydt.Rows[5][0]) + "%";
            mainpad.caption66 = "更新：" + mydt.Rows[5][2].ToString();

        }
    }
}
