using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;
using log4net;
using log4net.Appender;

namespace mysqlDao_v1
{
    public class mysqlConnectPool
    {
        public string connString;
        public MySqlConnection conn;
        private List<MySqlConnection> connList = new List<MySqlConnection>();
       
        private int PoolMax = 3;
        private static mysqlConnectPool instance;
        private ILog log;
        private mysqlConnectPool()
        {
            instance = this;
            log = LogManager.GetLogger("log");
            
        }
        
        public mysqlConnectPool Instance()
        {
            lock (this)
            {
                if(instance == null)
                {
                    new mysqlConnectPool();
                }
                return instance;
            }
        }

        public bool Init(string ConnectString,int ConnectionPoolMax=3)
        {
            PoolMax = ConnectionPoolMax;
            connString = ConnectString;
            MySqlConnection conn = new MySqlConnection(connString);
            if (!conn.Ping())
            {
                log.Debug("数据库不能访问-" + connString);
                return false;
            }
            connList.Add(conn);
            if (PoolMax > 20) PoolMax = 20;
            
            for(int i = 1; i <= PoolMax; i++)
            {
                connList.Add(new MySqlConnection(connString));
            }

            return true;
        }

        public MySqlConnection getIdleConn()
        {
            for (int i = 0; i <= PoolMax; i++)
            {
                if (connList[i].State == ConnectionState.Closed)
                    return connList[i];
            }
            return null;
        }

    }

    public class mysqlDAO
    {
        private string connStr;
        private MySqlConnection conn;
        private DataTable dt;
        ILog log;
        public mysqlDAO(string connectString)
        {

            log = LogManager.GetLogger("log");
            connStr = connectString;
            try
            {
                conn = new MySqlConnection(connStr);
                log.Debug("*** 新建数据源: "+conn.ConnectionString);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        


        private bool Ping()
        {
            bool b = conn.Ping();
            log.Debug("Ping DB:" + conn.DataSource + conn.Database+" return=" + b);
            return b;
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
                throw;
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
            int r = MySqlHelper.ExecuteNonQuery(conn, sql, null);
            log.Info("*** "+conn.DataSource + conn.Database + "  " + sql + " return="+r);
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
                    log.Debug(conn.DataSource+conn.Database+"  "+sql);
                    MySqlDataAdapter mda = new MySqlDataAdapter(sql, conn);
                    if(dt == null)
                    {
                        dt = new DataTable();
                    }
                    dt.Clear();
                    dt.Columns.Clear();
                    mda.Fill(dt);
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
