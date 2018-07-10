using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendDataMQ
{
    class DbClass
    {
        public OracleConnection getConn()
        {
            return new OracleConnection(ConfigurationManager.AppSettings["connectionString"].ToString());
        }

        #region Process Data
        public DataTable GetDataSMS(string sqlCommand)
        {

           
            OracleConnection con = getConn();
            con.Open();
            //Console.WriteLine("Connect data Ok!");
            OracleCommand cmd = new OracleCommand(sqlCommand,con);
            OracleDataAdapter adp = new OracleDataAdapter(cmd);

            //table to hold data
            DataTable dt = new DataTable();
            //select command fire here
            DataSet ds = new DataSet();
            adp.Fill(ds);
            dt = ds.Tables[0];

            con.Close();
            return dt;

        }
        #endregion

    }
}
