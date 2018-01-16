using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Data.OleDb;
using System.Collections;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using System.Data.Common;


namespace Test
{
    public class DBUtil
    {

  

        //SqlServer
        //static String dbType = "SqlServer";
        //public static String connectionString = "server=localhost; database=plusoft_test; Integrated Security=True; ";

        //Oracle
        //static String dbType = "Oracle";
        //public static String connectionString = "Provider=OraOLEDB.Oracle.1;Data Source=XE;User Id=plus;Password=sa";

        //MySql
        static String dbType = "SqlServer";
        public String connectionString = "server=localhost; user id=sa; password=123; database=plusoft_test;";

        private DbConnection conn;
        private DbConnection getConn()
        {
            if (conn == null)
            {
                if (dbType == "MySql")
                {
                    conn = new MySqlConnection(connectionString);
                }
                else if (dbType == "Oracle")
                {
                    conn = new OleDbConnection(connectionString);
                }
                else if (dbType == "SqlServer")
                {
                    conn = new SqlConnection(connectionString);
                }
                //conn.Open();
            }
            

            return conn;
        }
        //public static void BeginConn()
        //{
        //    getConn();
        //}
        //public static void EndConn()
        //{
        //    if (conn != null)
        //    {
        //        conn.Close();
        //        conn = null;
        //    }
        //}

        public ArrayList Select(string sql)
        {
            return Select(sql, null);
        }
        public ArrayList Select(string sql, Hashtable args)
        {
            DataTable data = new DataTable();

            bool isConn = conn != null;

            DbConnection con = getConn();

            try
            {
                con.Open();
            }
            catch (Exception e) { }

            using (con)
            {

                if (dbType == "MySql")
                {
                    MySqlCommand cmd = new MySqlCommand(sql, (MySqlConnection)con);
                    if (args != null) SetArgs(sql, args, cmd);

                    MySqlDataAdapter adapter = new MySqlDataAdapter(sql, connectionString);
                    adapter.Fill(data);
                }
                else if (dbType == "Oracle")
                {
                    OleDbCommand cmd = new OleDbCommand(sql, (OleDbConnection)con);
                    if (args != null) SetArgs(sql, args, cmd);

                    OleDbDataAdapter adapter = new OleDbDataAdapter(sql, connectionString);
                    adapter.Fill(data);
                }
                else if (dbType == "SqlServer")
                {
                    SqlCommand cmd = new SqlCommand(sql, (SqlConnection)con);
                    if (args != null) SetArgs(sql, args, cmd);

                    SqlDataAdapter adapter = new SqlDataAdapter(sql, connectionString);
                    adapter.Fill(data);
                }
            }

            //if (isConn == false)
            //{
            //    EndConn();
            //}


            return DataTable2ArrayList(data);
        }
        public void Execute(string sql)
        {
            Execute(sql, null);
        }
        public void Execute(string sql, Hashtable args)
        {
            bool isConn = conn != null;
            DbConnection con = getConn();

            try
            {
                con.Open();
            }
            catch (Exception e) { }

            using (con)
            {

                if (dbType == "MySql")
                {
                    MySqlCommand cmd = new MySqlCommand(sql, (MySqlConnection)con);
                    if (args != null) SetArgs(sql, args, cmd);
                    cmd.ExecuteNonQuery();
                }
                else if (dbType == "Oracle")
                {
                    OleDbCommand cmd = new OleDbCommand(sql, (OleDbConnection)con);
                    if (args != null) SetArgs(sql, args, cmd);
                    cmd.ExecuteNonQuery();
                }
                else if (dbType == "SqlServer")
                {
                    SqlCommand cmd = new SqlCommand(sql, (SqlConnection)con);
                    if (args != null) SetArgs(sql, args, cmd);
                    cmd.ExecuteNonQuery();
                }

            }

            //if (isConn == false)
            //{
            //    EndConn();
            //}
        }
        #region 私有
        private void SetArgs(string sql, Hashtable args, IDbCommand cmd)
        {
            if (dbType == "MySql")
            {
                MatchCollection ms = Regex.Matches(sql, @"@\w+");
                foreach (Match m in ms)
                {
                    string key = m.Value;
                    string newKey = "?" + key.Substring(1);
                    sql = sql.Replace(key, newKey);

                    Object value = args[key];
                    if (value == null)
                    {
                        value = args[key.Substring(1)];
                    }

                    cmd.Parameters.Add(new MySqlParameter(newKey, value));
                }
                cmd.CommandText = sql;
            }
            else if (dbType == "Oracle")
            {
                MatchCollection ms = Regex.Matches(sql, @"@\w+");
                int i = 1;
                foreach (Match m in ms)
                {
                    string key = m.Value;
                    string newKey = "@P" + i++;
                    sql = sql.Replace(key, "?");

                    Object value = args[key];
                    if (value == null)
                    {
                        value = args[key.Substring(1)];
                    }

                    cmd.Parameters.Add(new OleDbParameter(newKey, value));
                }
                cmd.CommandText = sql;
            }
            else if (dbType == "SqlServer")
            {
                MatchCollection ms = Regex.Matches(sql, @"@\w+");
                int i = 1;
                foreach (Match m in ms)
                {
                    string key = m.Value;

                    Object value = args[key];
                    if (value == null)
                    {
                        value = args[key.Substring(1)];
                    }
                    if (value == null) value = DBNull.Value;

                    cmd.Parameters.Add(new SqlParameter(key, value));
                }
                cmd.CommandText = sql;
            }
        }
        private ArrayList DataTable2ArrayList(DataTable data)
        {
            ArrayList array = new ArrayList();
            for (int i = 0; i < data.Rows.Count; i++)
            {
                DataRow row = data.Rows[i];

                Hashtable record = new Hashtable();
                for (int j = 0; j < data.Columns.Count; j++)
                {
                    object cellValue = row[j];
                    if (cellValue.GetType() == typeof(DBNull))
                    {
                        cellValue = null;
                    }
                    record[data.Columns[j].ColumnName] = cellValue;
                }
                array.Add(record);
            }
            return array;
        }
        #endregion
    }
}