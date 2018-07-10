using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ufms;
using System.IO;
using RabbitMQ.Client;
using ProtoBuf;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Web;
using System.Web.Caching;
using System.Data;

namespace GSNReceiver.GTVT_N
{
    class SendDataTracker
    {
      

        public static void sendTracking(string str,IConnection conn)
        {
            //str = "$STM,5001000419,090414092847,,,0,,,000121,5,5,0.0,0,0.0,,,0,*C21F";
            try
            {
                string path = ConfigurationManager.AppSettings["RegisterVenhicle"];
                string[] data = str.Split(',');
                if (data.Length < 13)
                    return;
                string id = "", datetime = "", speed = "", course = "", hd = "", status = "", runtime = "", day_Runtime = "", totalKm = "", fuel = "";
                double lon = 0, lat = 0;
                string vehicle = "", driver = "";
                
                #region gan du lieu nhan tu tracker
                DateTime date = new DateTime();
                if (data.Length.Equals(18))
                {
                    id = data[1].ToString();
                    datetime = data[2].ToString();
                    lon = Common.convertToDouble(data[3].ToString());
                    lat = Common.convertToDouble(data[4].ToString());
                    speed = data[5].ToString();
                    course = data[6].ToString();
                    hd = data[7].ToString();
                    status = data[8].ToString();
                    runtime = data[9].ToString();
                    day_Runtime = data[10].ToString();
                    totalKm = data[11].ToString();
                    fuel = data[12].ToString();

                }
                else
                {
                    id = data[1].Substring(1, data[1].Length - 1).ToString();
                    datetime = data[2].ToString();
                    lon = Common.convertToDouble(data[3].ToString());
                    lat = Common.convertToDouble(data[4].ToString());
                    speed = data[5].ToString();
                    course = data[6].ToString();
                    hd = data[7].ToString();
                    status = data[8].ToString();
                    runtime = data[9].ToString();
                    day_Runtime = data[10].ToString();
                    totalKm = data[11].ToString();
                    fuel = data[12].ToString();
                }
                #endregion
               
                #region chuyen status sang bit va kiem tra xe co trong file khong 
                string bit = Common.convertHextoBit(status);
                if (HttpRuntime.Cache["venhicle"] == null) { CreateCachedTable.createCachedDatatabe("venhicle");}
                var  list_venhicle = ((DataTable)HttpRuntime.Cache["venhicle"]).Select("M_ThietBi="+id).Take(1).ToList();
                if (list_venhicle.Count > 0)
                {
                    DataRow dr = list_venhicle[0];
                    vehicle = dr["BSX"].ToString();
                    driver = dr["MBL"].ToString();
                }
                else
                {
                    return;
                }
                #endregion

                #region convert datetime
                if (datetime != "" && datetime.Length <= 12 && datetime != null)
                {
                    int y = int.Parse("20" + datetime.Substring(4, 2));
                    int m = int.Parse(datetime.Substring(2, 2));
                    int d = int.Parse(datetime.Substring(0, 2));
                    int h = int.Parse(datetime.Substring(6, 2));
                    int ms = int.Parse(datetime.Substring(8, 2));
                    int sc = int.Parse(datetime.Substring(10, 2));
                    date = new DateTime(y, m, d, h, ms, sc, DateTimeKind.Local);
                }
                #endregion

               #region tao goi du lieu de gui WayPoint va gui cho GTVT
                
                WayPoint wp = new WayPoint();
                wp.vehicle = vehicle;//bien so xe ,viet lien ko dau
                wp.datetime = Common.DateTimeToUnixTimestamp(date);//ngay TBGS gui tin 
                wp.driver = driver;//so bang lai xe
                wp.x = Convert.ToDouble(lon);//kinh do
                wp.y = Convert.ToDouble(lat);//vi do
                wp.speed = float.Parse(speed);//toc do
                wp.door = bit.Substring(bit.Length - 2, 1) == "0" ? false : true; // dong mo cua
                wp.ignition = bit.Substring(bit.Length - 6, 1) == "0" ? false : true;// bat tat may
                wp.aircon = bit.Substring(bit.Length - 7, 1) == "0" ? false : true;//may lanh
                IModel channel = conn.CreateModel();
                BaseMessage msg = new BaseMessage();
                msg.msgType = BaseMessage.MsgType.WayPoint;
                Extensible.AppendValue<WayPoint>(msg, BaseMessage.MsgType.WayPoint.GetHashCode(), wp);
                byte[] b = Common.Serialize(msg);
                channel.BasicPublish("ufms.all", "", null, b);
                channel.Close();
                //conn.Close();
                #endregion
               
                #region ghi log lai du lieu da gui cho GTVT
                    string pathLog = ConfigurationManager.AppSettings["LogSendData"];
                    string fileName = Common.getFileName(pathLog + "Data_Tracker_");
                    string text = "#" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff") + ";" + id + ";" + wp.vehicle + ";" + wp.driver + ";" + wp.datetime + ";" + wp.x + ";" +
                                 wp.y + ";" + wp.speed + ";" + bit.Substring(bit.Length - 2, 1) + ";" + bit.Substring(bit.Length - 6, 1) + ";" + bit.Substring(bit.Length - 7, 1);
                    Common.writeLog(fileName, text);
                #endregion



            }
            catch (Exception ex)
            {
                string pathLog = ConfigurationManager.AppSettings["LogError"];
                string fileName = Common.getFileName(pathLog + "Log_Error_");
                string text = "Date:" + DateTime.Now + "       ;Error : " + ex.Message.ToString();
                Common.writeLog(fileName, text);
            }

        }
    }
}
