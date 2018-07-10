using ProtoBuf;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ufms;

namespace SendDataMQ
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void btnSendAMQP_Click(object sender, EventArgs e)
        {
            //DbClass _db = new DbClass();
            //DataTable data = new DataTable();
            //data = _db.GetDataSMS("select * from GPS_REALTIME_SEND_DATA");
            //dataGridView1.DataSource = data;
            timer1 = new System.Windows.Forms.Timer();
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Interval = 1000; // 1 second
            timer1.Start();
            lblCountDown.Text = counter.ToString();
            btnSendAMQP.Enabled = false;
            btnStop.Enabled = true;
            lbLog.Text = "Đang chạy ...";
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            btnSendAMQP.Enabled = true;
            btnStop.Enabled = false;
            timer1.Stop();
        }
        private System.Windows.Forms.Timer timer1;
        private int counter = 30, countForAll = 180;//speed 0: 180 
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
                    lbLog.Text = "Chỉ gửi IGNITION = 1 lúc " + DateTime.Now.ToString();
                    counter = 30;
                }
            }
            //timer1.Stop();
            lblCountDown.Text = counter.ToString();
            lbCountAll.Text = countForAll.ToString();
        }
        private void CallSend(int all)
        {
            //all :1 - where :0
            DateTime nowDate = DateTime.Now;
            WayPoint wp = new WayPoint();
            List<WayPoint> wpoints = new List<WayPoint>();
          
            DbClass _db = new DbClass();
            DataTable data = new DataTable();
            if (all == 0)// chỉ gửi speed > 0
            {
                data = _db.GetDataSMS("select * from GPS_REALTIME_SEND_DATA where IGNITION = 1");
            }
            else// gửi tất cả > 0
            {
                data = _db.GetDataSMS("select * from GPS_REALTIME_SEND_DATA");
            }
            List<WayPoint> listWP = new List<WayPoint>();
            foreach (DataRowView row in data.Rows)
            {
                wp = new WayPoint();
                wp.vehicle = Commond.GetStringFieldValue(row, "Vehicle");
                wp.driver = Commond.GetStringFieldValue(row, "Driver");
                nowDate = Commond.GetDateTimeFieldValue(row, "Datetime"); ;
                wp.datetime = DateTimeToUnixTimestamp(new DateTime(nowDate.Year, nowDate.Month, nowDate.Day, nowDate.Hour, nowDate.Minute, nowDate.Second, nowDate.Millisecond, DateTimeKind.Local));
                wp.speed = Commond.GetFloatFieldValue(row, "Speed");
                wp.x = Commond.GetDoubleFieldValue(row, "x");
                wp.y = Commond.GetDoubleFieldValue(row, "y");
                wp.z = Commond.GetFloatFieldValue(row, "z");
                wp.heading = Commond.GetFloatFieldValue(row, "Heading");
                wp.ignition = Commond.GetBooleanFieldValue(row, "Ignition");
                wp.door = Commond.GetBooleanFieldValue(row, "Door");
                wp.aircon = Commond.GetBooleanFieldValue(row, "AirCon");
                wp.maxvalidspeed = Commond.GetDoubleFieldValue(row, "MaxValidSpeed");
                wp.vss = Commond.GetFloatFieldValue(row, "VSS");
                wp.location = Commond.GetStringFieldValue(row, "Location");
                listWP.Add(wp);
                SendData(wp);
            }
           
            dataGridView1.DataSource = data;//listWP
            lbDS.Text = "Đã gửi: " + data.Rows.Count + " bản tin.";
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
                channel.Close();
                conn.Close();
                return 1;
            }
            catch (Exception c)
            {
                lbLog.Text = "Lỗi kết nối: " + c.Message;
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



        private DateTime UnixTimeStampToDateTime(int unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp);
            return dtDateTime.ToLocalTime();
        }
    }
}
