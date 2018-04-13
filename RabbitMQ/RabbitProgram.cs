using ProtoBuf;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ
{
    class RabbitProgram
    {
        static void Main(string[] args)
        {

        }
        public static void CheckBP()
        {
            // create WayPoint
            WayPoint wp = new WayPoint();
            wp.vehicle = "11A1234"; // chu y bien so viet lien, ko ky tu dac biet

            // 1371802570000
            DateTime date = new DateTime(2013, 6, 21, 15, 16, 10, DateTimeKind.Local);

            //date.get
            wp.datetime = DateTimeToUnixTimestamp(date);
            wp.driver = "SOBANGLAIXE";
            wp.x = 100; // longitude
            wp.y = 50; // latitude
            wp.speed = 90; // toc do
            wp.door = false; // dong mo cua
            wp.ignition = true; // bat tat may

            BaseMessage msg = new BaseMessage();
            msg.msgType = BaseMessage.MsgType.WayPoint;
            Extensible.AppendValue<WayPoint>(msg, RabbitMQ.BaseMessage.MsgType.WayPoint.GetHashCode(), wp);
            byte[] b = Serialize(msg);

            // write to file
            string path = @"C:\tmp\wp.msg";
            File.WriteAllBytes(path, b);

            // NOW - read it back from file
            byte[] bReadBack = File.ReadAllBytes(path);
            BaseMessage msgReadBack = DeSerialize(bReadBack);
            WayPoint wpReadBack = msgReadBack.msgWayPoint;

            // compare values
            if (wp.vehicle == wpReadBack.vehicle)
            {
                Console.WriteLine("wp.vehicle == wpReadBack.vehicle = " + wpReadBack.vehicle);
            }
            else
            {
                Console.WriteLine("Bien so sai!");
            }

            if (wp.datetime == wpReadBack.datetime)
            {
                Console.WriteLine("wp.datetime == wpReadBack.datetime == " + wp.datetime + " = " + UnixTimeStampToDateTime(wp.datetime));
            }
            else
            {
                Console.WriteLine("Thoi gian sai!");
            }

            if (wp.driver == wpReadBack.driver)
            {
                Console.WriteLine("wp.driver == wpReadBack.driver == " + wp.driver);
            }
            else
            {
                Console.WriteLine("Bang lai xe sai!");
            }

            if (wp.x == wpReadBack.x)
            {
                Console.WriteLine("wp.x == wpReadBack.x == " + wp.x);
            }
            else
            {
                Console.WriteLine("Toa do X sai!");
            }

            if (wp.y == wpReadBack.y)
            {
                Console.WriteLine("wp.y == wpReadBack.y == " + wp.y);
            }
            else
            {
                Console.WriteLine("Toa do Y sai!");
            }

            if (wp.speed == wpReadBack.speed)
            {
                Console.WriteLine("wp.speed == wpReadBack.speed == " + wp.speed);
            }
            else
            {
                Console.WriteLine("Van toc sai!");
            }

            if (wp.door == wpReadBack.door)
            {
                Console.WriteLine("wp.door == wpReadBack.door == " + wp.door);
            }
            else
            {
                Console.WriteLine("IO Dong/Mo cua sai!");
            }

            if (wp.ignition == wpReadBack.ignition)
            {
                Console.WriteLine("wp.ignition == wpReadBack.ignition == " + wp.ignition);
            }
            else
            {
                Console.WriteLine("IO Bat/Tat may sai!");
            }
        }
        public static void ProcessMQ()
        {
            // connection
            ConnectionFactory factory = new ConnectionFactory();
            factory.UserName = "username";
            factory.Password = "password";
            factory.VirtualHost = "/";
            factory.Protocol = Protocols.FromEnvironment();
            factory.HostName = "210.211.102.123";
            factory.Port = 5672;
            IConnection conn = factory.CreateConnection();

            // The IConnection interface can then be used to open a channel:
            IModel channel = conn.CreateModel();

            // create WayPoint
            WayPoint wp = new WayPoint();
            wp.datetime = DateTimeToUnixTimestamp(new DateTime(2013, 6, 21, 15, 16, 10, DateTimeKind.Local)); // - 1371802570000 - unix time - see .proto;
            wp.door = false; // dong mo cua
            wp.driver = "SOBANGLAI";
            //wp.heading = 90;
            wp.ignition = true; // bat tat may
            wp.speed = 40; // 40kmh
            wp.vehicle = "BIENSO";
            wp.x = 102.123456;
            wp.y = 10.123456;
            //wp.z = 20;


            BaseMessage msg = new BaseMessage();
            msg.msgType = BaseMessage.MsgType.WayPoint;

            Extensible.AppendValue<WayPoint>(msg, RabbitMQ.BaseMessage.MsgType.WayPoint.GetHashCode(), wp);


            byte[] b = Serialize(msg);
            channel.BasicPublish("ufms.all", "", null, b);

            //
            channel.Close();
            conn.Close();
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

        private static int DateTimeToUnixTimestamp(DateTime dateTime)
        {
            TimeSpan span = (dateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
            double unixTime = span.TotalSeconds;
            return (int)unixTime; // 1371802570000
        }

        private static DateTime UnixTimeStampToDateTime(int unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp);
            return dtDateTime.ToLocalTime();
        }
    }
}
