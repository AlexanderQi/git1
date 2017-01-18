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
        DataSet myset = new DataSet();
        static int DataErr = 0;

        public PageHGL()
        {
            InitializeComponent();
            s6.ArgumentDataMember = s5.ArgumentDataMember = s4.ArgumentDataMember = s3.ArgumentDataMember = s2.ArgumentDataMember = s1.ArgumentDataMember = "DTIME";
            s6.ValueDataMember = s5.ValueDataMember = s4.ValueDataMember = s3.ValueDataMember = s2.ValueDataMember = s1.ValueDataMember = "YCVALUE";
            //s1.ArgumentScaleType = DevExpress.Xpf.Charts.ScaleType.DateTime;

            comboBox.SelectionChanged += ComboBox_SelectionChanged;
            this.Loaded += PageHGL_Loaded;
        }

        DataTable dt;
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox.SelectedItem == null) return;
            string n = mydt.Rows[comboBox.SelectedIndex][0].ToString();
            string conn2 = ConfigurationManager.ConnectionStrings["mysql_nttbl"].ToString();
            if (conn2 == null || conn2.Equals("")) return;
            try
            {
                if (myconn2 != null)
                {
                    myconn2.Close();
                }
                myconn2 = new MySqlConnection(conn2);
                myconn2.Open();
                string sql = "select name 测点,ycvalue 值 from tblycvalue where name like '%"+n+"%'";
                MySqlDataAdapter ada = new MySqlDataAdapter(sql, myconn2);
                if(dt == null)
                    dt = new DataTable();
                dt.Clear();
                ada.Fill(dt);
               // dataGrid.ItemsSource = null;
                 //comboBox.DisplayMemberPath = "NAME";
                dataGrid.ItemsSource = dt.DefaultView;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void PageHGL_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }



        public void DataAct(Action act)
        {
            Dispatcher.BeginInvoke(act, System.Windows.Threading.DispatcherPriority.Background);
        }

        private MySqlConnection myconn = null;
        public void LoadData()
        {
            LoadData2();

            if (myconn != null) return; //演示用，只加载一次。
            string conn = ConfigurationManager.ConnectionStrings["mysql_nthis"].ToString();
            if (conn == null || conn.Equals("")) return;
            try
            {
                if (myconn != null)
                {
                    myconn.Close();
                }
                myconn = new MySqlConnection(conn);
                myconn.Open();
                //string sql = "select t.DTIME,t.YCVALUE from hisycvalue t where t.ID = 15373615 and t.DTIME > date_add(now(), interval - 7 day);";

                //原始SQL 见项目\南通\首页查询.sql
                string sql = @"select t.DTIME,t.YCVALUE from hisycvalue t where t.ID = 15373615 and
 DATEDIFF(t.DTIME, '2016-07-22')=0;
select t.DTIME,t.YCVALUE from hisycvalue t where t.ID = 15487245 and
 DATEDIFF(t.DTIME,'2016-11-22')=0;
select t.DTIME,t.YCVALUE from hisycvalue t where t.ID = 15373625 and
 DATEDIFF(t.DTIME,'2016-11-22')=0;
select t.DTIME,t.YCVALUE from hisycvalue t where t.ID = 15373635 and
 DATEDIFF(t.DTIME,'2016-11-22')=0;
select t.DTIME,t.YCVALUE from hisycvalue t where t.ID = 15373645 and
 DATEDIFF(t.DTIME,'2016-11-22')=0;
select t.DTIME,t.YCVALUE from hisycvalue t where t.ID = 15487235 and
 DATEDIFF(t.DTIME,'2016-11-22')=0;";

                MySqlDataAdapter adapter = new MySqlDataAdapter(sql, myconn);
                adapter.Fill(myset);
                var max2 = myset.Tables[1].AsEnumerable().Max(x => x.Field<double>(1));
                var min2 = myset.Tables[1].AsEnumerable().Min(x => x.Field<double>(1));

                var max4 = myset.Tables[3].AsEnumerable().Max(x => x.Field<double>(1));
                var min4 = myset.Tables[3].AsEnumerable().Min(x => x.Field<double>(1));

                var max5 = myset.Tables[4].AsEnumerable().Max(x => x.Field<double>(1));
                var min5 = myset.Tables[4].AsEnumerable().Min(x => x.Field<double>(1));

                var max6 = myset.Tables[5].AsEnumerable().Max(x => x.Field<double>(1));
                var min6 = myset.Tables[5].AsEnumerable().Min(x => x.Field<double>(1));

                y2.VisualRange.MaxValue = max2;
                y2.VisualRange.MinValue = min2;


                y4.VisualRange.MaxValue = max4;
                y4.VisualRange.MinValue = min4;

                y5.VisualRange.MaxValue = max5;
                y5.VisualRange.MinValue = min5;

                y6.VisualRange.MaxValue = max6;
                y6.VisualRange.MinValue = min6;

                DataErr = 0;
                // Thread.Sleep(10000); 
                s1.BeginInit();  //合格率  
                s1.DataSource = myset.Tables[0];

                s1.EndInit();

                s2.BeginInit();
                s2.DataSource = myset.Tables[1];
                s2.EndInit();

                s3.BeginInit();
                s3.DataSource = myset.Tables[2];
                s3.EndInit();

                s4.BeginInit();
                s4.DataSource = myset.Tables[3];
                s4.EndInit();

                s5.BeginInit();
                s5.DataSource = myset.Tables[4];
                s5.EndInit();

                s6.BeginInit();
                s6.DataSource = myset.Tables[5];
                s6.EndInit();
            }
            catch (Exception e)
            {
                DataErr = -1;
                MessageBox.Show(e.ToString());
            }
        }

        private MySqlConnection myconn2 = null;

        public void LoadData2()
        {
            if (myconn2 != null) return; //演示用，只加载一次。
            string conn = ConfigurationManager.ConnectionStrings["mysql_nttbl"].ToString();
            if (conn == null || conn.Equals("")) return;
            try
            {
                if (myconn2 != null)
                {
                    myconn2.Close();
                }
                myconn2 = new MySqlConnection(conn);
                myconn2.Open();
                string sql = "select t.NAME from tblelement t;";
                MySqlDataAdapter ada = new MySqlDataAdapter(sql, myconn2);
                ada.Fill(mydt);
                comboBox.Items.Clear();
                comboBox.DisplayMemberPath = "NAME";
                comboBox.ItemsSource = mydt.DefaultView;
                comboBox.SelectedIndex = 2;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
    }
}
