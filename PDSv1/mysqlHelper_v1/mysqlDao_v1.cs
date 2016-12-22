using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;

namespace mysqlDao_v1
{
    public class mysqlConnectPool
    {
        public string connString;
        public MySqlConnection conn;
        private List<MySqlConnection> connList = new List<MySqlConnection>();
       
        private int PoolMax = 3;
        private static mysqlConnectPool instance;
        private mysqlConnectPool()
        {
            instance = this;
            
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
                return false;
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

        public mysqlDAO(string connectString)
        {
            connStr = connectString;
            try
            {
                conn = new MySqlConnection(connStr);
            }
            catch (Exception)
            {
                 
                throw;
            }
        }


        public bool ConnectTest()
        {
            return conn.Ping();
        }

        public void ConnectClose()
        {
            conn.Close();
        }

        public int Execute(String sql)
        {
            return MySqlHelper.ExecuteNonQuery(conn, sql, null);
        }

        public DataTable Query(String sql)
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                MySqlDataAdapter mda = new MySqlDataAdapter(sql, conn);
                mda.Fill(dt);
                return dt;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
