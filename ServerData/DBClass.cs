using Microsoft.ApplicationBlocks.Data;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerData
{
    class DBClass 
    {
        public OracleConnection getConn()
        {
            return new OracleConnection(ConfigurationManager.AppSettings["connectionString"].ToString());
        }

        #region Process Data

        internal void excuteMsgToDB(ArrayList filterLstMsg, int Device_ID)
        {
            ArrayList lstCMD = new ArrayList();

            for (Int32 i = 0; i < filterLstMsg.Count; i++)
            {
                OracleCommand cmd = new OracleCommand();
                string msg = (string)filterLstMsg[i];
                cmd.CommandText = "INSERTEVENTLOG_G3_1";
                cmd.CommandType = CommandType.StoredProcedure;

                //deviceID
                OracleParameter deviceID = new OracleParameter("pDeviceID", OracleDbType.Long);
                deviceID.Value = Device_ID;
                cmd.Parameters.Add(deviceID);
                //StringT
                OracleParameter StringT = new OracleParameter("pStringT", OracleDbType.Varchar2);
                StringT.Value = msg;
                cmd.Parameters.Add(StringT);
                lstCMD.Add(cmd);
            }
            OracleConnection con = getConn();
            try
            {
                //Console.WriteLine("Duc" + con.ToString());
                con.Open();
                //Console.WriteLine("Start Insert DB");
                int j = 0;
                for (Int32 i = 0; i < lstCMD.Count; i++)
                {
                    OracleCommand cmd = (OracleCommand)lstCMD[i];

                    cmd.Connection = con;

                    cmd.ExecuteNonQuery();
                    //con.Close();
                    j++;

                }
                Console.WriteLine("Execute " + j + " Proc Done !");
                //logger.Info("Execute " + j + " Proc Done !");
                con.Close();
            }

            catch (OracleException e)
            {
                Console.WriteLine(e.ToString());
                //logger.Error(e.ToString());
                // con.Close();

            }
        }


        #endregion


    }
}
