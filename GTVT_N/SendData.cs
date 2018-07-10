using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ufms;
using System.IO;
using RabbitMQ.Client;
using ProtoBuf;
namespace GSNReceiver.GTVT
{
    class SendData
    {
        /// <summary>
        /// gui tracking 
        /// </summary>
        /// <param name="str"></param>
        public static void sendTracking(string str)
        {
            //str = "$STD,5001000419,240414135810,10541.1744E,1913.1382N,0,245,1.02,000001,0,0,0.0,13,0.0,,,0,*8E1C";

            #region
            string[] data = str.Split(',');
            if (data.Length < 13)
                return;
            string id = "", datetime = "",  speed = "", course = "", hd = "", status = "", runtime = "", day_Runtime = "", totalKm = "", fuel = "";
            double lon = 0, lat = 0;
            string vehicle = "", driver = "";
            DateTime date=new DateTime();

            if (data.Length.Equals(18))
            {
                id = data[1].ToString();
                datetime = data[2].ToString();
                lon = convertToDouble(data[3].ToString());
                lat = convertToDouble(data[4].ToString());
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
                id = data[1].Substring(1,data[1].Length-1).ToString();
                datetime = data[2].ToString();
                lon = convertToDouble(data[3].ToString());
                lat = convertToDouble(data[4].ToString());
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
           
            #region chuyen status kieu hex sang kieu bit 
            //0000000000000000
            string bit = convertHextoBit(status);
            #endregion
            #region doc file du lieu luu thong tin cua xe
            StreamReader sr = new StreamReader(@"..\..\Info_N\Info_Vehicle.txt", true);
            string[] lines = sr.ReadToEnd().Split('#');
            string[] vh={};
            foreach (var line in lines)
            {
                if (line.Contains(id))
                {
                    vh = line.Replace("\r\n","").Split(',');
                    vehicle = vh[1].ToString();
                    driver = vh[2].ToString();
                    break;
                }
               
            }
            sr.Close();
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
           
            #region goi du lieu
                WayPoint wp = new WayPoint();
            
            //bien so xe ,viet lien ko dau
            wp.vehicle = vehicle;
             //ngay TBGS gui tin 
            wp.datetime = DateTimeToUnixTimestamp(date);
            //so bang lai xe
            wp.driver = driver;
            //kinh do
            wp.x = Convert.ToDouble(lon);
            //vi do
            wp.y = Convert.ToDouble(lat); 
            //toc do
            wp.speed = float.Parse(speed);
            // dong mo cua
            wp.door = bit.Substring(bit.Length-2, 1) == "0" ? false : true;
            // bat tat may
            wp.ignition = bit.Substring(bit.Length - 6, 1) == "0" ? false : true; 
            //may lanh
            wp.aircon = bit.Substring(bit.Length - 7, 1) == "0" ? false : true;

            //ConnectionFactory factory = new ConnectionFactory();
            //factory.UserName = "";
            //factory.Password = "";
            //factory.VirtualHost = "/";
            //factory.Protocol = Protocols.FromEnvironment();
            //factory.HostName = "210.211.102.123";
            //factory.Port = 5672;
            //IConnection conn = factory.CreateConnection();
            //IModel channel = conn.CreateModel();
            //BaseMessage msg = new BaseMessage();
            //msg.msgType = BaseMessage.MsgType.WayPoint;
           
            //Extensible.AppendValue<WayPoint>(msg, BaseMessage.MsgType.WayPoint.GetHashCode(), wp);
            //byte[] b = Serialize(msg);
            //channel.BasicPublish("ufms.all", "", null, b);
            //channel.Close();
            //conn.Close();

            string path = @"E:\GSTCLog\test.txt";
            TextWriter tw = new StreamWriter(path, true);
            string text = "bien so: " + wp.vehicle +
                          "ngay: " + wp.datetime +
                          "ma bang lai: " + wp.driver +
                         "kinh do: " + wp.x +
                         "vi do: " + wp.y +
                         "toc do: " + wp.speed +
                         "dong mo cua: " + wp.door +
                         "may lanh: " + wp.aircon+
                          "bat tat may: " + wp.ignition;

            tw.WriteLine(text);
            tw.Close();
         #endregion
        }

        public static byte[] Serialize(BaseMessage wp)
        {
            byte[] b = null;
            using (var ms = new MemoryStream())
            {
                Serializer.Serialize<BaseMessage>(ms, wp);
                b = new byte[ms.Position];
                var fullB = ms.GetBuffer();
                Array.Copy(fullB, b, b.Length);
            }
            return b;
        }

        public static BaseMessage DeSerialize(byte[] bytes)
        {
            BaseMessage msg;
            using (var ms = new MemoryStream(bytes))
            {
                msg = Serializer.Deserialize<BaseMessage>(ms);
            }

            return msg;
        }

        /// <summary>
        /// chuyen dinh dang ngay 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static int DateTimeToUnixTimestamp(DateTime dateTime)
        {
            TimeSpan span = (dateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
            double unixTime = span.TotalSeconds;
            return (int)unixTime; // 1371802570000
                                  
        }
        /// <summary>
        /// convert hex sang bit
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static string convertHextoBit(string hex)
        {
            return Convert.ToString(Convert.ToInt32(hex, 16), 2).PadLeft(hex.Length * 2, '0');
        }
        /// <summary>
        /// convert lat ,lon sang kieu du lieu double
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double convertToDouble(string value)
        {
            double returnValue = 0;
            if (value != "")
            {

                value = value.Substring(0, value.Length - 1);
                int startLatitudeSecond = value.IndexOf(".") + 1;
                var strLatitudeSecond = value.Substring(startLatitudeSecond, value.Length - startLatitudeSecond);
                value = value.Substring(0, value.IndexOf("."));
                var strLatMinute = value.Substring(value.Length - 2, 2);
                var strLatHour = value.Substring(0, value.Length - 2);
                returnValue = Convert.ToDouble(strLatHour) + Convert.ToDouble(strLatMinute) / 60 + Convert.ToDouble(strLatitudeSecond) * 60 / 36000000;
            }

            return returnValue;
        }
    }
}
