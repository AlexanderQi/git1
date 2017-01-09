using log4net;
using mysqlDao_v1;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
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
            Wait4sth.showing();
            Action act = new Action(TableConvert);
            act.BeginInvoke(ConvertCompleted, this);

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

        private void showinfo(string info)
        {
            log.Info(info);
            textBox_info.AppendText(info + '\n');
        }
        private void async_showinfo(string info)
        {
            Application.Current.Dispatcher.Invoke(new Action<string>(showinfo), info);
        }
        private void TableConvert()
        {
            async_showinfo("配网AVC库结构转换任务开始.");
            //Thread.Sleep(7000);
            try
            {
                async_showinfo("将转换老数据库:" + curbase);
                mdao.Execute("use " + curbase);

                add_feedid();
                add_tblcmdcheck();
                add_tblfeeder();
                add_tblfeedermeasure();
                add_tblprogram();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                MessageBox.Show(ex.ToString());
            }
        }

        private void add_tblprogram()
        {
            try
            {
                string sql = @"CREATE TABLE IF NOT EXISTS `tblprogram` (
  `ID` int(11) NOT NULL COMMENT '唯一标识',
  `NAME` varchar(100) DEFAULT NULL COMMENT '名称',
  `ALIASNAME` varchar(100) DEFAULT NULL COMMENT '简称',
  `DESCRIPTION` varchar(200) DEFAULT NULL COMMENT '描述',
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;";
                int i = mdao.Execute(sql);
                async_showinfo("CREATE TABLE IF NOT EXISTS `tblprogram` return:" + i);
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                async_showinfo(ex.Message);
            }
        }

        private void add_tblfeedermeasure()
        {
            try
            {
                string sql = @"CREATE TABLE IF NOT EXISTS `tblfeedermeasure` (
  `ID` varchar(100) NOT NULL COMMENT '编号',
  `PYCID` int(11) DEFAULT NULL COMMENT '有功遥测编号',
  `QYCID` int(11) DEFAULT NULL COMMENT '无功遥测编号',
  `IYCID` int(11) DEFAULT NULL COMMENT '电压遥测编号',
  `UABYCID` int(11) DEFAULT NULL COMMENT '首端母线线电压AB遥测编号',
  `UACYCID` int(11) DEFAULT NULL COMMENT '首端母线线电压AC遥测编号',
  `UBCYCID` int(11) DEFAULT NULL COMMENT '首端母线线电压BC遥测编号',
  `ZXYGDLYMID` int(11) DEFAULT NULL COMMENT '正向有功电量遥脉编号',
  `FXYGDLYMID` int(11) DEFAULT NULL COMMENT '反向有功电量遥脉编号',
  `ZXWGDLYMID` int(11) DEFAULT NULL COMMENT '正向无功电量遥脉编号',
  `FXWGDLYMID` int(11) DEFAULT NULL COMMENT '反向无功电量遥脉编号',
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='主馈线量测定义表';";
                int i = mdao.Execute(sql);
                async_showinfo("CREATE TABLE IF NOT EXISTS `tblfeedermeasure` return:" + i);
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                async_showinfo(ex.Message);
            }
        }

        private void add_tblfeeder()
        {
            try
            {
                string sql = @"CREATE TABLE IF NOT EXISTS `tblfeeder` (
  `ID` varchar(100) NOT NULL COMMENT '编号',
  `NAME` varchar(100) NOT NULL COMMENT '名称',
  `ALIASNAME` varchar(100) DEFAULT NULL COMMENT '别名',
  `DESCRIPTION` varchar(250) DEFAULT NULL COMMENT '描述',
  `SUBSTATIONID` varchar(100) DEFAULT NULL COMMENT '所属厂站编号',
  `VOLTAGELEVELID` varchar(100) DEFAULT NULL COMMENT '电压等级编号',
  `ZONEID` varchar(100) DEFAULT NULL COMMENT '分区编号',
  `ZONEBELONG` varchar(100) DEFAULT NULL COMMENT '网络区划代码\n            城网、农网',
  `GRAPHID` int(11) DEFAULT NULL COMMENT '所属图号',
  PRIMARY KEY(`ID`)
) ENGINE = InnoDB DEFAULT CHARSET = utf8 COMMENT = '主馈线表'; ";
                int i = mdao.Execute(sql);
                async_showinfo("CREATE TABLE IF NOT EXISTS `tblfeeder` return:" + i);
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                async_showinfo(ex.Message);
            }
        }

        private void add_tblcmdcheck()
        {
            try
            {
                string sql = "CREATE TABLE IF NOT EXISTS `tblcmdcheck` ( `ID` int(11) NOT NULL COMMENT '唯一标识',`DTIME` datetime DEFAULT NULL COMMENT '记录插入时间',`CMDID` varchar(100) DEFAULT NULL COMMENT '遥控遥调命令ID',`ELEMENTID` varchar(100) DEFAULT NULL COMMENT '设备ID',`ACTIONFIELD` varchar(100) DEFAULT NULL COMMENT '动作次数字段', `ACTIONTOTALFIELD` varchar(100) DEFAULT NULL COMMENT '动作总次数字段', PRIMARY KEY(`ID`)) ENGINE = InnoDB DEFAULT CHARSET = utf8;";
                int i = mdao.Execute(sql);
                async_showinfo("CREATE TABLE IF NOT EXISTS `tblcmdcheck` return:" + i);
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                async_showinfo(ex.Message);
            }
        }

        private void add_feedid()
        {
            string sql = "SELECT table_name FROM information_schema.COLUMNS WHERE TABLE_SCHEMA='" + curbase + "' AND COLUMN_NAME='graphid'";
            DataTable dt = mdao.Query(sql);
            foreach (DataRow dr in dt.Rows)
            {
                string fn = dr[0].ToString();
                async_showinfo("将表(+feedid字段):" + fn);
                string exes = "ALTER TABLE " + fn + " ADD FEEDID int(11) DEFAULT NULL COMMENT '所属馈线'";
                try
                {
                    int i = mdao.Execute(exes);
                    async_showinfo("update rows:" + i);
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    async_showinfo(ex.Message);
                }
                try
                {
                    exes = "UPDATE " + fn + " SET feedid = graphid;";
                    int i = mdao.Execute(exes);
                    async_showinfo("updates values:" + i);
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    async_showinfo(ex.Message);
                }
            }
        }

        private void ConvertCompleted(IAsyncResult i)
        {
            async_showinfo("配网AVC库结构转换任务结束.");
            Wait4sth.stopshowing();
        }

    }
}
