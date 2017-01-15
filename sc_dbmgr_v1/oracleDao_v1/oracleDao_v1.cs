using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;
using log4net;
using System.Data;

namespace oracleDao_v1
{
    public class oraConnInfo
    {
        public string User;
        public string ServerIp;
        public string Password;
        public string DatabaseName;
        public string DatabaseType;
    }
    public class oracleDao
    {
        private string connStr;
        private OracleConnection conn;
        private DataTable dt;
        ILog log;


        public static oraConnInfo getConnInfo(string ConnectString)
        {
            //SERVER=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.1.99)(PORT=1152))(CONNECT_DATA=(SERVICE_NAME=ORCL)));uid=SYSTEM;pwd=ORCL"
            oraConnInfo cinfo = null;
            if (ConnectString.IndexOf("ADDRESS=(PROTOCOL=TCP)") >= 0)  //Oracle connect
            {
                cinfo = new oraConnInfo();
                string[] ss = ConnectString.Split('=', '(', ')', ';');
                for(int i = 0; i < ss.Length; i++)
                {
                    string tmp = ss[i].ToUpper();
                    if (tmp.Equals("HOST"))
                    {
                        cinfo.ServerIp = ss[i + 1];
                    } else if (tmp.Equals("SERVICE_NAME")) 
                    {
                        cinfo.DatabaseName = ss[i + 1];
                    }else if (tmp.Equals("USER ID")) 
                    {
                        cinfo.User = ss[i + 1];
                    }else if (tmp.Equals("PASSWORD")) 
                    {
                        cinfo.Password = ss[i + 1];
                    }
                }
            }
            return cinfo;
        }



        public oracleDao(string connectString)
        {

            log = LogManager.GetLogger("log");
            connStr = connectString;
            try
            {
                conn = new OracleConnection(connStr);
                log.Debug("*** 新建数据源: " + conn.ConnectionString);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }



        public void TestConnect()
        {
            try
            {
                conn.Close();
                conn.Open();
                conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void ConnectClose()
        {
            conn.Close();
        }

        public int Execute(String sql)
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            OracleCommand oc = new OracleCommand(sql, conn);
            int r = oc.ExecuteNonQuery();
            log.Info("*** " + conn.DataSource+ "  " + sql + " return=" + r);
            return r;
        }

        public DataTable Query(String sql)
        {
            lock (this)
            {
                try
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    log.Debug(conn.DataSource + "  " + sql);
                    OracleDataAdapter oda = new OracleDataAdapter(sql, conn);
                    if (dt == null)
                    {
                        dt = new DataTable();
                    }
                    dt.Clear();
                    dt.Columns.Clear();
                    oda.Fill(dt);
                    return dt;
                }
                catch (Exception ex)
                {
                    log.Error(ex);

                }
                return null;
            }
        }


    }
}
