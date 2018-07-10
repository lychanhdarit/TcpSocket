using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ufms;
using RabbitMQ.Client;
using ProtoBuf;
using System.Configuration;
namespace GSNReceiver.GTVT_N
{
    class SendRegistrator
    {
        
        /// <summary>
        /// gui danh sach cong ty dang ky su dung GPS tracker cho bo giao thong van tai
        /// </summary>
        public void sendRegisCom()
        {
            string path = ConfigurationManager.AppSettings["RegisterCompany"];
            string[] lines = Common.readFile(path);
            int count = 0;
            foreach (var line in lines)
            {
                if (line != "")
                {
                    string[] comInfo = line.Replace("/r", "").Replace("/n", "").Split(';');
                    RegCompany rg = new RegCompany();
                  
                    rg.company = comInfo[0];
                    rg.name = comInfo[1];
                    rg.address = comInfo[2];
                    rg.tel = comInfo[3];

                    //IConnection conn= GTVT_N.Connection.createConnection();
                    //IModel chanel = conn.CreateModel();
                    //BaseMessage msg = new BaseMessage();
                    //msg.msgType = BaseMessage.MsgType.RegCompany;
                    //Extensible.AppendValue<RegCompany>(msg, BaseMessage.MsgType.RegCompany.GetHashCode(), rg);
                    //byte[] b = Serialize(msg);
                    //chanel.BasicPublish("ufms.all", "", null, b);
                    //chanel.Close();
                    //conn.Close();

                    //StreamWriter sw = new StreamWriter(@"E:\GSTCLog\Testlog_regCompany.txt", true);
                    //sw.WriteLine("Dia chi" + rg.address + " ; Ma cong ty" + rg.company + " ; Ten cong ty" +
                    //             rg.name + " ; So dien thoai cong ty" + rg.tel);
                    //sw.Close();

                    Console.WriteLine(line);
                    count++;
                }
            }

            Console.WriteLine("Da gui " + count + "doanh nghiep .");
            Common.question();

        }
        /// <summary>
        /// gui danh sach tai xe  dang ki cho bo giao thong van tai
        /// </summary>
        public void sendRegDriver()
        {
            string path = ConfigurationManager.AppSettings["RegisterDriver"];
            string[] lines = Common.readFile(path);
            foreach (var line in lines)
            {
                if (line != "")
                {
                    int dateIssue = 0;
                    int dateExprice = 0;
                    string[] driverInfo = line.Split(';');
                    DateTime issue =Convert.ToDateTime(driverInfo[2]);
                    DateTime expire =Convert.ToDateTime(driverInfo[3]);

                    if (issue != null)
                    {
                        int d = issue.Day;
                        int m = issue.Month;
                        int y =issue.Year;
                        int h =issue.Hour;
                        int ms =issue.Minute;
                        int sc = issue.Second;
                        DateTime date = new DateTime(y, m, d, h, ms, sc, DateTimeKind.Local);
                        dateIssue = DateTimeToUnixTimestamp(date);
                    }
                    if (expire != null)
                    {
                        int d = expire.Day;
                        int m = expire.Month;
                        int y = expire.Year;
                        int h = expire.Hour;
                        int ms = expire.Minute;
                        int sc = expire.Second;
                        DateTime date = new DateTime(y, m, d, h, ms, sc, DateTimeKind.Local);
                        dateExprice = DateTimeToUnixTimestamp(date);
                    }

                    RegDriver regDriver = new RegDriver();
                    regDriver.driver = driverInfo[0];
                    regDriver.name = driverInfo[1];
                    regDriver.datetimeIssue = dateIssue;
                    regDriver.datetimeExpire = dateExprice;
                    regDriver.regPlace = driverInfo[4];
                    regDriver.license = driverInfo[5].Replace("\r", "").Replace("\n", "");

                    //IConnection conn= GTVT_N.Connection.createConnection();
                    //IModel chanel = conn.CreateModel();
                    //BaseMessage msg = new BaseMessage();
                    //msg.msgType = BaseMessage.MsgType.RegDriver;
                    //Extensible.AppendValue<RegDriver>(msg, BaseMessage.MsgType.RegDriver.GetHashCode(), regDriver);
                    //byte[] b = Serialize(msg);
                    //chanel.BasicPublish("ufms.all", "", null, b);
                    //chanel.Close();
                    //conn.Close();

                    Console.WriteLine(line);
                }

            }
            Common.question();

        }
        /// <summary>
        /// gui thong tin xe dang ki tracker
        /// </summary>
        public void sendRegVenhicle()
        {
            string path = ConfigurationManager.AppSettings["RegisterVenhicle"];
            string[] lines = Common.readFile(path);
            foreach (var line in lines)
            {
                if (line != "")
                {
                    string[] vehicle = line.Replace("\r","").Replace("\n","").Split(';');
                    RegVehicle regVehicle = new RegVehicle();
                    //Khach = 100; Bus = 200;HopDong = 300;DuLich = 400;Container = 500;
                    DateTime regDate =Convert.ToDateTime(vehicle[7].ToString());
                    int regDateVehince = 0;
                    if (regDate != null)
                    {
                        int d =regDate.Day;
                        int m = regDate.Month;
                        int y =regDate.Year;
                        int h =regDate.Hour;
                        int ms =regDate.Minute;
                        int sc =regDate.Second;
                        DateTime date = new DateTime(y, m, d, h, ms, sc, DateTimeKind.Local);
                        regDateVehince = DateTimeToUnixTimestamp(date);
                    }
                    regVehicle.vehicle =vehicle[0].ToString();
                    regVehicle.vehicleType = RegVehicle.VehicleType.Khach;
                    regVehicle.driver = vehicle[2].ToString();
                    regVehicle.company = vehicle[3].ToString();
                    regVehicle.deviceModelNo =Convert.ToInt32(vehicle[4].ToString());
                    regVehicle.deviceId = vehicle[5].ToString();
                    regVehicle.sim = vehicle[6].ToString();
                    regVehicle.datetime = regDateVehince;

                    Console.WriteLine(line);
                    //IConnection conn= GTVT_N.Connection.createConnection();
                    //IModel chanel = conn.CreateModel();
                    //BaseMessage msg = new BaseMessage();
                    //msg.msgType = BaseMessage.MsgType.RegVehicle;
                    //Extensible.AppendValue<RegVehicle>(msg, BaseMessage.MsgType.RegVehicle.GetHashCode(), regVehicle);
                    //byte[] b = Serialize(msg);
                    //chanel.BasicPublish("ufms.all", "", null, b);
                    //chanel.Close();
                    //conn.Close();
                    
                }
            }
            Common.question();
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
