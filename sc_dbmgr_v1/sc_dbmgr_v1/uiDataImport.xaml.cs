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
using Microsoft.Win32;
using System.Collections;
using System.Configuration;
using mysqlDao_v1;
using log4net;
using System.Data;
using System.Diagnostics;
using System.Threading;
using System.IO;
using Microsoft.VisualBasic;
using MySql.Data.MySqlClient;
using oracleDao_v1;

namespace sc_dbmgr_v1
{
    /// <summary>
    /// uiDataImport.xaml 的交互逻辑
    /// </summary>
    public partial class uiDataImport : UserControl
    {
        mysqlDAO mdao = null;
        oracleDao odao = null;
        ILog log;
        public uiDataImport()
        {
            InitializeComponent();
            log = LogManager.GetLogger("log");
            button_file.Click += Button_file_Click;
            button_import.Click += Button_import_Click;
            uiDbConnector.Instance.ConnectStringsChanged += Instance_ConnectStringsChanged;

        }

       

        private void Instance_ConnectStringsChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            comboBox_dbc.Items.Clear();
            IEnumerator ie = ConfigurationManager.ConnectionStrings.GetEnumerator();
            while (ie.MoveNext())
            {
                ConnectionStringSettings cs = ie.Current as ConnectionStringSettings;
                if (cs.Name.IndexOf("SqlServer") >= 0) continue;

                comboBox_dbc.Items.Add(cs.Name);
            }
            if (comboBox_dbc.Items.Count > 0)
                comboBox_dbc.SelectedIndex = 0;
        }

        private string curuser = null;
        private string curpw = null;
        private string curip = null;
        private string curbase = null;
        private string cstr = null;
        private int dbtype = 0;
        private const int dbtype_mysql = 1;
        private const int dbtype_ora = 2;
        private void Button_import_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox_dbc.SelectedItem == null) return;
            try
            {
                string dbn = comboBox_dbc.SelectedItem.ToString();
                cstr = ConfigurationManager.ConnectionStrings[dbn].ConnectionString;
                if (dbn.IndexOf("MY-") >= 0)
                {
                    dbtype = dbtype_mysql;
                    myConnInfo ci = mysqlDAO.getConnInfo(cstr);
                    curip = ci.ServerIp;
                    curuser = ci.User;
                    curpw = ci.Password;
                    curbase = ci.DatabaseName;
                }
                else { 
                    dbtype = dbtype_ora;
                    oraConnInfo ci = oracleDao.getConnInfo(cstr);
                    curip = ci.ServerIp;
                    curuser = ci.User;
                    curpw = ci.Password;
                    curbase = ci.DatabaseName;
                }                
                showinfo(curip + "^" + curuser + "^" + curpw + "^" + curbase);
                async_task_import();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                MessageBox.Show(ex.ToString());
            }
        }

        private string sqlfile = null;
        private void Button_file_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "数据库文件|*.sql";
            of.FilterIndex = 0;
            if (of.ShowDialog() == false)
                return;
            textBox_file.Text = of.FileName;
            sqlfile = of.FileName;
        }

        private string new_base_name = null;
        private void async_task_import()
        {
            if (!File.Exists(textBox_file.Text))
            {
                MessageBox.Show("文件不存在.");
                return;
            }

            new_base_name = Interaction.InputBox("请输入要创建的库名", "创建库", "");
            if (new_base_name == "") return;

            
            showinfo("将向数据库源:" + curip + new_base_name+ " 导入文件:" + textBox_file.Text);
            Wait4sth.showing();
            
            Action act = new Action(task_import);
            act.BeginInvoke(task_import_callback, this);
        }

        private void task_import()
        {
            try
            {
                if (dbtype == dbtype_mysql)
                {
                    if (mdao != null)
                    {
                        mdao.ConnectClose();
                    }
                    mdao = new mysqlDAO(cstr);

                    string exes = "CREATE DATABASE " + new_base_name + ";use " + new_base_name + ";";
                    int r = mdao.Execute(exes);
                    async_showinfo(exes + " return:" + r);
                    ExecuteSqlFile();
                }
                else
                {
                    if (odao != null)
                        odao.ConnectClose();
                    odao = new oracleDao(cstr);
                    ExecuteSqlFile();
                }

            }
            catch (Exception ex)
            {
                log.Error(ex);
                MessageBox.Show(ex.ToString());
            }
        }

        private bool ExecuteSqlFile()
        {
            using (StreamReader reader = new StreamReader(sqlfile, System.Text.Encoding.GetEncoding("utf-8")))
            {
                try
                {
                    string line = "";
                    string line_add;
                    while (true)
                    {
                        // 如果line被使用，则设为空  
                        if (line.EndsWith(";"))
                            line = "";

                        line_add = reader.ReadLine();

                        // 如果到了最后一行，则退出循环  
                        if (line_add == null) break;
                        // 去除空格  
                        line_add = line_add.TrimEnd();
                        // 如果是空行，则跳出循环  
                        if (line_add == "") continue;
                        // 如果是注释，则跳出循环  
                        if (line_add.StartsWith("--")) continue;

                          
                        line += line_add;
                        // 如果不是完整的一条语句，则继续读取  
                        if (!line.EndsWith(";")) continue;
                        if (line.StartsWith("/*!"))
                        {
                            continue;
                        }

                        //执行当前行  
                        try
                        {
                            if (dbtype == dbtype_mysql)
                                mdao.Execute(line);
                            else
                                odao.Execute(line);
                        }
                        catch (MySql.Data.MySqlClient.MySqlException ex)
                        {
                            async_showinfo(ex.Message);
                        }
                       
                    }
                }
                finally
                {
                    if (dbtype == dbtype_mysql)
                        mdao.ConnectClose();
                    else
                        odao.ConnectClose();
                }
            }

            return true;
        }

        private void task_import_callback(IAsyncResult i)
        {
            async_showinfo("导入任务结束.");
            Wait4sth.stopshowing();
        }

        private void showinfo(string info)
        {
            textBox_info.AppendText(info + "\n");
        }
        private void async_showinfo(string info)
        {
            Application.Current.Dispatcher.Invoke(new Action<string>(showinfo), info);
        }
    }
}
