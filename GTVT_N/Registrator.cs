using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ufms;
using RabbitMQ.Client;
using ProtoBuf;
namespace GSNReceiver.GTVT_N
{
    class Registrator
    {
        
        /// <summary>
        /// gui danh sach cong ty dang ky su dung GPS tracker cho bo giao thong van tai
        /// </summary>
        public void sendRegisCom()
        {
            StreamReader sr = new StreamReader("../../Info_N/Business.txt", true);
            string[] lines = sr.ReadToEnd().Split('#');
            int count = 0;
            foreach (var line in lines)
            {
                if (line != "")
                {
                    string[] comInfo = line.Split(';');
                    RegCompany rg = new RegCompany();
                  
                    rg.company = comInfo[0];
                    rg.name = comInfo[1];
                    rg.address = comInfo[2];
                    rg.tel = comInfo[3];

                    //ConnectionFactory factory = new ConnectionFactory();
                    //factory.UserName = "";
                    //factory.Password = "";
                    //factory.VirtualHost = "/";
                    //factory.Protocol = Protocols.FromEnvironment();
                    //factory.HostName = "210.211.102.123";
                    //factory.Port = 5672;
                    //IConnection conn = factory.CreateConnection();
                    //IModel chanel = conn.CreateModel();
                    //BaseMessage msg = new BaseMessage();
                    //msg.msgType = BaseMessage.MsgType.RegCompany;
                    //Extensible.AppendValue<RegCompany>(msg, BaseMessage.MsgType.RegCompany.GetHashCode(), rg);
                    //byte[] b = Serialize(msg);
                    //chanel.BasicPublish("ufms.all", "", null, b);
                    //chanel.Close();
                    //conn.Close();

                    StreamWriter sw = new StreamWriter(@"E:\GSTCLog\Testlog_regCompany.txt", true);
                    sw.WriteLine("Dia chi" + rg.address + " ; Ma cong ty" + rg.company + " ; Ten cong ty" +
                                 rg.name + " ; So dien thoai cong ty" + rg.tel);
                    sw.Close();
                    count++;
                }
            }

            Console.WriteLine("Da gui " + count + "doanh nghiep .");
            Console.Read();

        }
        /// <summary>
        /// gui danh sach thiet bi dang ki cho bo giao thong van tai
        /// </summary>
        public void sendRegDriver()
        {
            StreamReader sr = new StreamReader("../../Info_N/Driver.txt", true);
            string[] lines = sr.ReadToEnd().Split('#');
            foreach (var line in lines)
            {
                int dateIssue = 0;
                int dateExprice = 0;
                string[] driverInfo = line.Split(',');


                string issue = driverInfo[2];
                string expire = driverInfo[3];
                if (issue.Length == 12 && issue != null && issue != "")
                {
                    int d = Convert.ToInt16(issue.Substring(0, 2));
                    int m = Convert.ToInt16(issue.Substring(4, 2));
                    int y = Convert.ToInt16(issue.Substring(7, 4));
                    int h = Convert.ToInt16(issue.Substring(12, 2));
                    int ms = Convert.ToInt16(issue.Substring(15, 2));
                    int sc = Convert.ToInt16(issue.Substring(18, 3));
                    DateTime date = new DateTime(y, m, d, h, ms, sc, DateTimeKind.Local);
                    dateIssue = DateTimeToUnixTimestamp(date);
                }
                if (expire.Length == 12 && expire != null && expire != "")
                {
                    int d = Convert.ToInt16(issue.Substring(0, 2));
                    int m = Convert.ToInt16(issue.Substring(4, 2));
                    int y = Convert.ToInt16(issue.Substring(7, 4));
                    int h = Convert.ToInt16(issue.Substring(12, 2));
                    int ms = Convert.ToInt16(issue.Substring(15, 2));
                    int sc = Convert.ToInt16(issue.Substring(18, 3));
                    DateTime date = new DateTime(y, m, d, h, ms, sc, DateTimeKind.Local);
                    dateExprice = DateTimeToUnixTimestamp(date);
                }

                RegDriver regDriver = new RegDriver();
                regDriver.driver = driverInfo[0];
                regDriver.name = driverInfo[1];
                regDriver.datetimeIssue = dateIssue;
                regDriver.datetimeExpire = dateExprice;
                regDriver.regPlace = driverInfo[4];
                regDriver.license = driverInfo[5];
            }

        }
        /// <summary>
        /// gui thong tin xe dang ki tracker
        /// </summary>
        public void sendRegVenhicle()
        {
            StreamReader sr = new StreamReader("../../Info_N/Vehicle.txt", true);
            string[] lines = sr.ReadToEnd().Split('#');
            foreach (var line in lines)
            {
                string[] vehicle = line.Split(',');
                RegVehicle regVehicle = new RegVehicle();
                regVehicle.vehicle = "";
                regVehicle.vehicleType = RegVehicle.VehicleType.Khach;
                regVehicle.deviceModel = "";
                regVehicle.deviceModelNo = 0;
                regVehicle.driver = "";
                regVehicle.sim = "";
                regVehicle.datetime = 0;
                regVehicle.deviceId = "";
                regVehicle.company = "";

            }
        }

        public int DateTimeToUnixTimestamp(DateTime dateTime)
        {
            TimeSpan span = (dateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
            double unixTime = span.TotalSeconds;
            return (int)unixTime; // 1371802570000

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
    }
}
