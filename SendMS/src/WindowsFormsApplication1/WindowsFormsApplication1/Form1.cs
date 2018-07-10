using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RabbitMQ.Client;
using System.IO;
using ProtoBuf;
using System.Threading;
using ufms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void testPB_Click(object sender, EventArgs e)
        {
            ////////////////////////////////////////
            // test write protobol-buf message to file and read it back.


            // create WayPoint
            WayPoint wp = new WayPoint();
            wp.vehicle = "11A1234"; // chu y bien so viet lien, ko ky tu dac biet

            // 1371802570000
            DateTime date = new DateTime(2013, 6, 21, 15, 16, 10, DateTimeKind.Local);

            //date.get
            wp.datetime = this.DateTimeToUnixTimestamp(date); wp.driver = "SOBANGLAIXE"; wp.x = 100; wp.y = 50; // latitude
            wp.speed = 90; // toc do
            wp.door = false; // dong mo cua
            wp.ignition = true; // bat tat may

            BaseMessage msg = new BaseMessage();
            msg.msgType = BaseMessage.MsgType.WayPoint;
            Extensible.AppendValue<WayPoint>(msg, ufms.BaseMessage.MsgType.WayPoint.GetHashCode(), wp);
            byte[] b = this.Serialize(msg);

            // write to file
            string path = @"C:\tmp\wp.msg";
            File.WriteAllBytes(path, b);

            // NOW - read it back from file
            byte[] bReadBack = File.ReadAllBytes(path);
            BaseMessage msgReadBack = this.DeSerialize(bReadBack);
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
        private System.Windows.Forms.Timer timer1;
        private int counter = 30, countForAll = 120;//speed 0: 120 
        private void timer1_Tick(object sender, EventArgs e)
        {
            counter--;
            countForAll--;
            if (countForAll == 0)
            {
                CallSend(1);
                lbLog.Text = "Đã gửi tất cả lúc " + DateTime.Now.ToString();
                countForAll = 180;
                if (counter == 0)
                {
                    counter = 30;
                }
            }
            else
            {
                if (counter == 0)
                {
                    CallSend(0);
                    lbLog.Text = "Chỉ gửi tốc độ lớn hơn 0 lúc " + DateTime.Now.ToString();
                    counter = 30;
                }
            }
            //timer1.Stop();
            lblCountDown.Text = counter.ToString();
            lbCountAll.Text = countForAll.ToString();
        }
        private void CallSend(int all)
        {
            //all :1 - all :0
            DateTime nowDate = DateTime.Now;
            //MessageBox.Show(DateTimeToUnixTimestamp(new DateTime(nowDate.Year, nowDate.Month, nowDate.Day, nowDate.Hour, nowDate.Minute, nowDate.Second, nowDate.Millisecond, DateTimeKind.Local)) +"-"+DateTime.Now);
            WayPoint wp = new WayPoint();
            List<WayPoint> wpoints = new List<WayPoint>();

            wp.datetime = DateTimeToUnixTimestamp(new DateTime(nowDate.Year, nowDate.Month, nowDate.Day, nowDate.Hour, nowDate.Minute, nowDate.Second, nowDate.Millisecond, DateTimeKind.Local)); // - 1371802570000 - unix time - see .proto;
            wp.door = false; wp.driver = "23232333"; wp.heading = 90; wp.ignition = true; wp.speed = 20; wp.vehicle = "43C14390"; wp.x = 15.711218333; wp.y = 108.383445;
            wpoints.Add(wp);
            //Add row 2
            wp = new WayPoint();
            wp.datetime = DateTimeToUnixTimestamp(new DateTime(nowDate.Year, nowDate.Month, nowDate.Day, nowDate.Hour, nowDate.Minute, nowDate.Second, nowDate.Millisecond, DateTimeKind.Local)); // - 1371802570000 - unix time - see .proto;
            wp.door = false; wp.driver = "658255"; wp.heading = 90; wp.ignition = true; wp.speed = 0; wp.vehicle = "51D11935"; wp.x = 15.711218333; wp.y = 108.383445;
            wpoints.Add(wp);
            //Add row 3
            wp = new WayPoint();
            wp.datetime = DateTimeToUnixTimestamp(new DateTime(nowDate.Year, nowDate.Month, nowDate.Day, nowDate.Hour, nowDate.Minute, nowDate.Second, nowDate.Millisecond, DateTimeKind.Local)); // - 1371802570000 - unix time - see .proto;
            wp.door = false; wp.driver = "3565656"; wp.heading = 90; wp.ignition = true; wp.speed = 70; wp.vehicle = "51C12889"; wp.x = 15.711218333; wp.y = 108.383445;
            wpoints.Add(wp);
            //Add row 4
            wp = new WayPoint();
            wp.datetime = DateTimeToUnixTimestamp(new DateTime(nowDate.Year, nowDate.Month, nowDate.Day, nowDate.Hour, nowDate.Minute, nowDate.Second, nowDate.Millisecond, DateTimeKind.Local)); // - 1371802570000 - unix time - see .proto;
            wp.door = false; wp.driver = "7878789"; wp.heading = 90; wp.ignition = true; wp.speed = 0; wp.vehicle = "51C22817"; wp.x = 15.711218333; wp.y = 108.383445;
            wpoints.Add(wp);
            //Add row 5
            wp = new WayPoint();
            wp.datetime = DateTimeToUnixTimestamp(new DateTime(nowDate.Year, nowDate.Month, nowDate.Day, nowDate.Hour, nowDate.Minute, nowDate.Second, nowDate.Millisecond, DateTimeKind.Local)); // - 1371802570000 - unix time - see .proto;
            wp.door = false; wp.driver = "9898456"; wp.heading = 90; wp.ignition = true; wp.speed = 60; wp.vehicle = "51C05875"; wp.x = 15.711218333; wp.y = 108.383445;
            wpoints.Add(wp);
            //process data

            #region data
            DataTable data = new DataTable();
            foreach (DataRow row in data.Rows)
            {
                wp = new WayPoint();
                wp.datetime = DateTimeToUnixTimestamp(new DateTime(nowDate.Year, nowDate.Month, nowDate.Day, nowDate.Hour, nowDate.Minute, nowDate.Second, nowDate.Millisecond, DateTimeKind.Local)); // - 1371802570000 - unix time - see .proto;
                wp.door = false; wp.driver = "23232333"; wp.heading = 90; wp.ignition = true; wp.speed = 40; wp.vehicle = "51D11935"; wp.x = 15.711218333; wp.y = 108.383445;
                wpoints.Add(wp);
            }
            #endregion
            //end
            List<WayPoint> tempdata = new List<WayPoint>();
            if (all == 0)// chỉ gửi speed > 0
            {
                foreach (WayPoint w in wpoints)
                {
                    if (w.speed > 0)
                    {
                        //SendData(w);
                        WayPoint itemPoint = new WayPoint();
                        itemPoint.datetime = w.datetime; // - 1371802570000 - unix time - see .proto;
                        itemPoint.door = w.door; itemPoint.driver = w.driver; itemPoint.heading = w.heading; itemPoint.ignition = w.ignition; itemPoint.speed = w.speed; itemPoint.vehicle = w.vehicle; itemPoint.x = w.x; itemPoint.y = w.y;
                        tempdata.Add(itemPoint);
                    }
                }
                dataGridView1.DataSource = tempdata;
            }
            else// gửi tất cả > 0
            {
                foreach (WayPoint w in wpoints)
                {
                    //SendData(w);
                    WayPoint itemPoint = new WayPoint();
                    itemPoint.datetime = w.datetime; // - 1371802570000 - unix time - see .proto;
                    itemPoint.door = w.door; itemPoint.driver = w.driver; itemPoint.heading = w.heading; itemPoint.ignition = w.ignition; itemPoint.speed = w.speed; itemPoint.vehicle = w.vehicle; itemPoint.x = w.x; itemPoint.y = w.y;
                    tempdata.Add(itemPoint);
                }
                dataGridView1.DataSource = tempdata;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // create WayPoint
            timer1 = new System.Windows.Forms.Timer();
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Interval = 1000; // 1 second
            timer1.Start();
            lblCountDown.Text = counter.ToString();
            btnSendAMQP.Enabled = false;
            btnStop.Enabled = true;
            lbLog.Text = "Đang chạy ...";
        }
        private int SendData(WayPoint wp)
        {
            try
            {
                BaseMessage msg = new BaseMessage();
                msg.msgType = BaseMessage.MsgType.WayPoint;
                Extensible.AppendValue<WayPoint>(msg, ufms.BaseMessage.MsgType.WayPoint.GetHashCode(), wp);
                byte[] b = this.Serialize(msg);
                // connection
                ConnectionFactory factory = new ConnectionFactory();
                factory.UserName = "atg";
                factory.Password = "VZxp85_RZ";
                factory.VirtualHost = "/";
                factory.Protocol = Protocols.FromEnvironment();
                factory.HostName = "27.118.27.118";// "210.211.102.123";
                factory.Port = 5674;// 5672;
                IConnection conn = factory.CreateConnection();

                // The IConnection interface can then be used to open a channel:
                IModel channel = conn.CreateModel();

                channel.BasicPublish("tracking.atg", "track1", null, b);
                //channel.BasicPublish("ufms.all", "", null, b);Exchange,/routingkey, 27.118.27.118:5677 User / pass atg / atg Exchange = tracking.atg/ routingkey = dangkiem

                //
                channel.Close();
                conn.Close();
                return 1;
            }
            catch (Exception c)
            {
                lbLog.Text =  "Lỗi kết nối: " + c.Message;
                return 1;
            }
        }
      
        public byte[] Serialize(BaseMessage wp)
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

        public BaseMessage DeSerialize(byte[] bytes)
        {
            BaseMessage msg;
            using (var ms = new MemoryStream(bytes))
            {
                msg = Serializer.Deserialize<BaseMessage>(ms);
            }

            return msg;
        }

        private int DateTimeToUnixTimestamp(DateTime dateTime)
        {
            TimeSpan span = (dateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
            double unixTime = span.TotalSeconds;
            return (int)unixTime; // 1371802570000
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            btnSendAMQP.Enabled = true;
            btnStop.Enabled = false;
            timer1.Stop();
        }

        private DateTime UnixTimeStampToDateTime(int unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp);
            return dtDateTime.ToLocalTime();
        }

       

    }
}
