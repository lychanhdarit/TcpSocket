using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Data;
using System.Configuration;

namespace GSNReceiver.GTVT_N
{
    class CreateCachedTable
    {
        public CreateCachedTable() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cachedName">
        /// la ten cua cached can luu ,
        /// 1.cached cho venhicle.txt
        /// 
        /// </param>
        public static void createCachedDatatabe(string cachedName)
        {
            try
            {
                switch (cachedName)
                {
                    case "venhicle":
                        HttpRuntime.Cache[cachedName] = createTableVenhicle();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                string pathLog = ConfigurationManager.AppSettings["LogError"];
                string fileName = Common.getFileName(pathLog + "Log_Error_");
                string text = "Date:" + DateTime.Now + "function :CreateCachedTable.createCachedDatatabe()" + ";Error : " + ex.Message.ToString();
                Common.writeLog(fileName, text);
            }
            
        }
        /// <summary>
        /// tao cached cho du lieu cua venhicle
        /// </summary>
        /// <returns></returns>
        private static DataTable createTableVenhicle()
        {
            DataTable dt = new DataTable("venhicle");
            string[] columns = { "BSX", "LHX", "MBL", "MDN", "D_ModeNo", "M_ThietBi", "SIM", "Ngay_DK", "VIN","Bat_Tat_May", "Dong_Mo_Cua", "Qua_TG", "BD_DiChuyen"};

            for (int i = 0; i < columns.Length; i++)
            {
                dt.Columns.Add(columns[i]);
            }

            //doc file du lieu add vao datatable 
            string path = ConfigurationManager.AppSettings["RegisterVenhicle"];
            string[] lines = Common.readFile(path);

            foreach (var line in lines)
            {
                try
                {
                    string[] venhicleDetail = line.ToString().Replace("\r", "").Replace("\n", "").Split(';');
                    if (venhicleDetail.Length.Equals(9))
                    {
                        DataRow dr = dt.NewRow();
                        dr["BSX"] = venhicleDetail[0].Trim();
                        dr["LHX"] = venhicleDetail[1].Trim();
                        dr["MBL"] = venhicleDetail[2].Trim();
                        dr["MDN"] = venhicleDetail[3].Trim();
                        dr["D_ModeNo"] = venhicleDetail[4].Trim();
                        dr["M_ThietBi"] = venhicleDetail[5].Trim();
                        dr["SIM"] = venhicleDetail[6].Trim();
                        dr["Ngay_DK"] = venhicleDetail[7].Trim();
                        dr["VIN"] = venhicleDetail[8].Trim();
                        dr["Bat_Tat_May"] = 0;//bit
                        dr["Dong_Mo_Cua"] = 0;//bit
                        dr["Qua_TG"] = 0;//int
                        dr["BD_DiChuyen"] = 0;//bit
                        dt.Rows.Add(dr);
                    }
                    dt.AcceptChanges();

                }
                catch (Exception ex)
                {
                    string pathLog = ConfigurationManager.AppSettings["LogError"];
                    string fileName = Common.getFileName(pathLog + "Log_Error_");
                    string text = "Date:" + DateTime.Now + "function :CreateCachedTable.createTableVenhicle()" + ";Error : " + ex.Message.ToString();
                    Common.writeLog(fileName, text);
                }
                
            }

         return dt;
        }
    }
}
