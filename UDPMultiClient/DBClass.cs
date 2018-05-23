using Oracle.ManagedDataAccess.Client;
using ServerData;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDPMultiClient
{
    class DBClass
    {
        public OracleConnection getConn()
        {
            return new OracleConnection(ConfigurationManager.AppSettings["connectionString"].ToString());
        }

        #region Process Data

        public void excuteMsgToDB(int Device_ID, string data)
        {
            try
            {
                OracleCommand cmd = new OracleCommand();
                OracleConnection con = getConn();
                con.Open();
                //Console.WriteLine("Connect data Ok!");
                cmd.CommandText = ConfigurationManager.AppSettings["PROCEDURE"].ToString();
                cmd.CommandType = CommandType.StoredProcedure;

                //deviceID
                OracleParameter deviceID = new OracleParameter("pDeviceID", OracleDbType.Int32);
                deviceID.Value = Device_ID;
                cmd.Parameters.Add(deviceID);
                //StringT
                OracleParameter StringT = new OracleParameter("pStringT", OracleDbType.Varchar2);
                StringT.Value = data;
                cmd.Parameters.Add(StringT);

                cmd.Connection = con;
                cmd.ExecuteNonQuery();

                con.Close();
                Console.WriteLine("Execute DB: " + data + " DeviceId: " + Device_ID);

                Utilities.WriteLog("Execute DB: " + data + " DeviceId: " + Device_ID);
            }

            catch (OracleException e)
            {
                getConn().Close();
                Console.WriteLine(e.ToString());
                Utilities.WriteLogDatabase("Data: " + data + " DeviceID: " + Device_ID);
                Utilities.WriteLogDatabase(e.ToString());
            }
        }





        #endregion


    }
}
