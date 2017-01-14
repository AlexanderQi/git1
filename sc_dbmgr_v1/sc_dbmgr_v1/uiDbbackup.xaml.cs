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
using System.Configuration;
using System.Collections;
using mysqlDao_v1;
using log4net;
using System.Data;
using Microsoft.Win32;
using System.Diagnostics;
using System.Threading;
using System.Runtime.Remoting.Messaging;

namespace sc_dbmgr_v1
{
    /// <summary>
    /// uiDbbackup.xaml 的交互逻辑
    /// </summary>
    public partial class uiDbbackup : UserControl
    {
        mysqlDAO mdao = null;
        ILog log;
        public uiDbbackup()
        {
            InitializeComponent();
            log = LogManager.GetLogger("log");
            button_connect.Click += Button_connect_Click;
            button_backup.Click += Button_backup_Click;
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
                
                int i = comboBox_db.Items.Add(cs.Name);
               
            }
            if (comboBox_db.Items.Count > 0)
                comboBox_db.SelectedIndex = 0;
        }

        Action backup = null;
        IAsyncResult backup_call_result = null;
        private void Button_backup_Click(object sender, RoutedEventArgs e)
        {
           
            if (listBox_table.Items.Count == 0)
            {
                MessageBox.Show("请先连接数据库服务器，并选择要备份的数据库.");
                return;
            }
            
            SaveFileDialog sf = new SaveFileDialog();
            sf.DefaultExt = "sql";
            sf.Filter = "数据库文件|*.sql";
            sf.FilterIndex = 0;
            sf.RestoreDirectory = true;
            sf.FileName = "ABackup" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".sql";
            string fn;
            if (sf.ShowDialog() == false)
                return;
            fn = sf.FileName;
            textBox_info.AppendText("备份任务开始\n数据将保存到："+fn+"\n");
            log.Debug("backup file: " + fn);
            StringBuilder sb = new StringBuilder();
            string appdir = AppDomain.CurrentDomain.BaseDirectory;
            sb.Append(appdir).Append("tool\\mysqldump -h").Append(curip).Append(" -u").Append(curuser).Append(" -p").Append(curpw).Append(" ").Append(curbase).Append(" > ")
                .Append(fn);
            CmdInfo = sb.ToString();

            
            Wait4sth.showing();
            log.Debug("backup cmd: " + CmdInfo);
            backup = new Action(RunCmd);
            backup_call_result = backup.BeginInvoke(new AsyncCallback(backupCompleted), null);
            //backup.EndInvoke(backup_call_result); //阻塞UI 
            
        }

        
        private void backupCompleted(IAsyncResult i)
        {
            Application.Current.Dispatcher.Invoke(new Action(backupCompleted_dispatch));
            Wait4sth.stopshowing();
        }

        private void backupCompleted_dispatch()
        {
            textBox_info.AppendText("任务完成.\n" + CmdResult + "\n");
            log.Debug("backup cmd result: " + CmdResult);
        }

        private void RunCmd_()
        {
            Thread.Sleep(10000);
        }

        private string CmdResult = null;
        private string CmdInfo = null;
        private void RunCmd()
        {
            Process p = new Process();
            string info = null;
            string err = null;
            try
            {
                p.StartInfo.FileName = "cmd.exe";           //确定程序名
                p.StartInfo.Arguments = "/c " + CmdInfo;    //确定程式命令行
                p.StartInfo.UseShellExecute = false;        //Shell的使用
                p.StartInfo.RedirectStandardInput = true;   //重定向输入
                p.StartInfo.RedirectStandardOutput = true; //重定向输出
                p.StartInfo.RedirectStandardError = true;   //重定向输出错误
                p.StartInfo.CreateNoWindow = true;          //设置置不显示示窗口
                p.Start();   //00
                //p.StandardInput.WriteLine(command);       //也可以用这种方式输入入要行的命令
                //p.StandardInput.WriteLine("exit");        //要得加上Exit要不然下一行
                p.WaitForExit();
                info = p.StandardOutput.ReadToEnd();        //输出出流取得命令行结果果
                err = p.StandardError.ReadToEnd();
                CmdResult = info + "\n" + err;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                p.Close();
            }
        }

        private string curuser = null;
        private string curpw = null;
        private string curip = null;
        private string curbase = null;
        private void Button_connect_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox_db.SelectedItem.ToString().IndexOf("O-") >= 0)
            {
                MessageBox.Show("不支持ORACLE数据库备份，请选择ORACLE客户端工具备份.");
                return;
            }
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
