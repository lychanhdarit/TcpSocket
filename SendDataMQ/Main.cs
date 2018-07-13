using ProtoBuf;
using RabbitMQ.Client;
using ServerData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
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
        private int counter = int.Parse(ConfigurationManager.AppSettings["Timer30s"].ToString()), countForAll = int.Parse(ConfigurationManager.AppSettings["Timer180s"].ToString());//speed 0: 180 
        private void timer1_Tick(object sender, EventArgs e)
        {
            counter--;
            countForAll--;
            if (countForAll == 0)
            {
                int result = CallSend(1);
                if (result == 1)
                {
                    lbLog.Text = "Đã gửi tất cả lúc " + DateTime.Now.ToString();
                }
                else
                {
                    lbLog.Text = "Không gửi được.";
                }
                countForAll = int.Parse(ConfigurationManager.AppSettings["Timer180s"].ToString());
                if (counter == 0)
                {
                    counter = int.Parse(ConfigurationManager.AppSettings["Timer30s"].ToString());
                }
            }
            else
            {
                if (counter == 0)
                {
                    int result = CallSend(0);
                    if (result == 1)
                    {
                        lbLog.Text = "Chỉ gửi IGNITION = 1 lúc " + DateTime.Now.ToString();
                    }
                    else
                    {
                        lbLog.Text = "Không gửi được.";
                    }
                    counter = int.Parse(ConfigurationManager.AppSettings["Timer30s"].ToString());
                }
            }
            //timer1.Stop();
            lblCountDown.Text = counter.ToString();
            lbCountAll.Text = countForAll.ToString();
        }
        private int CallSend(int all)
        {
            //all :1 - where :0
            DateTime nowDate = DateTime.Now;
            WayPoint wp = new WayPoint();
            List<WayPoint> wpoints = new List<WayPoint>();

            DbClass _db = new DbClass();
            DataTable data = new DataTable();
            if (all == 0)// chỉ gửi speed > 0
            {
                data = _db.GetDataSMS(ConfigurationManager.AppSettings["SqlCommand30s"].ToString());
            }
            else// gửi tất cả > 0
            {
                data = _db.GetDataSMS(ConfigurationManager.AppSettings["SqlCommand180s"].ToString());
            }
            List<WayPoint> listWP = new List<WayPoint>();
            if (data != null)
            {
                try
                {
                    #region connection

                    ConnectionFactory factory = new ConnectionFactory();
                    factory.UserName = ConfigurationManager.AppSettings["UserName"].ToString();
                    factory.Password = ConfigurationManager.AppSettings["Password"].ToString();
                    factory.VirtualHost = ConfigurationManager.AppSettings["VirtualHost"].ToString();
                    factory.Protocol = Protocols.FromEnvironment();
                    factory.HostName = ConfigurationManager.AppSettings["HostName"].ToString();// "210.211.102.123";
                    factory.Port = int.Parse(ConfigurationManager.AppSettings["Port"].ToString());// 5672;
                    IConnection conn = factory.CreateConnection();
                    // The IConnection interface can then be used to open a channel:
                    IModel channel = conn.CreateModel();

                    #endregion
                    foreach (DataRow row in data.Rows)
                    {
                        wp = new WayPoint();
                        wp.vehicle = Commond.GetStringFieldValue(row, "Vehicle");
                        wp.driver = Commond.GetStringFieldValue(row, "Driver");
                        nowDate = Commond.GetDateTimeFieldValue(row, "Datetime"); ;
                        wp.datetime = DateTimeToUnixTimestamp(new DateTime(nowDate.Year, nowDate.Month, nowDate.Day, nowDate.Hour, nowDate.Minute, nowDate.Second, nowDate.Millisecond, DateTimeKind.Local));
                        wp.speed = Commond.GetFloatFieldValue(row, "Speed");
                        wp.x = Commond.GetDoubleFieldValue(row, "y");
                        wp.y = Commond.GetDoubleFieldValue(row, "x");
                        // Change lat lng
                        wp.z = Commond.GetFloatFieldValue(row, "z");
                        wp.heading = Commond.GetFloatFieldValue(row, "Heading");
                        wp.ignition = Commond.GetBooleanFieldValue(row, "Ignition");
                        wp.door = Commond.GetBooleanFieldValue(row, "Door");
                        wp.aircon = Commond.GetBooleanFieldValue(row, "AirCon");
                        wp.maxvalidspeed = Commond.GetDoubleFieldValue(row, "MaxValidSpeed");
                        wp.vss = Commond.GetFloatFieldValue(row, "VSS");
                        wp.location = Commond.GetStringFieldValue(row, "Location");
                        listWP.Add(wp);
                        //SendData(wp);
                        BaseMessage msg = new BaseMessage();
                        msg.msgType = BaseMessage.MsgType.WayPoint;
                        Extensible.AppendValue<WayPoint>(msg, ufms.BaseMessage.MsgType.WayPoint.GetHashCode(), wp);
                        byte[] b = this.Serialize(msg);
                        channel.BasicPublish(ConfigurationManager.AppSettings["Exchange"].ToString(), ConfigurationManager.AppSettings["RoutingKey"].ToString(), null, b);
                    }


                    #region Endconnection
                    channel.Close();
                    conn.Close();
                    #endregion
                }
                catch (Exception c)
                {
                    lbLog.Text = "Lỗi kết nối: " + c.Message;
                    Utilities.WriteLog("Lỗi kết nối: " + c.Message);
                }
                dataGridView1.DataSource = data;//listWP
                dataWP.DataSource = listWP;
                lbDS.Text = "Đã gửi: " + data.Rows.Count + " bản tin.";
                return 1;
            }
            else
            {
                lbDS.Text = "Kiểm tra kết nối database.";
                return 0;
            }


        }


        private int SendData(WayPoint wp)
        {
            try
            {

                ConnectionFactory factory = new ConnectionFactory();
                factory.UserName = ConfigurationManager.AppSettings["UserName"].ToString();
                factory.Password = ConfigurationManager.AppSettings["Password"].ToString();
                factory.VirtualHost = ConfigurationManager.AppSettings["VirtualHost"].ToString();
                factory.Protocol = Protocols.FromEnvironment();
                factory.HostName = ConfigurationManager.AppSettings["HostName"].ToString();// "210.211.102.123";
                factory.Port = int.Parse(ConfigurationManager.AppSettings["Port"].ToString());// 5672;
                IConnection conn = factory.CreateConnection();
                // The IConnection interface can then be used to open a channel:
                IModel channel = conn.CreateModel();

                BaseMessage msg = new BaseMessage();
                msg.msgType = BaseMessage.MsgType.WayPoint;
                Extensible.AppendValue<WayPoint>(msg, ufms.BaseMessage.MsgType.WayPoint.GetHashCode(), wp);
                byte[] b = this.Serialize(msg);
                // connection

                channel.BasicPublish(ConfigurationManager.AppSettings["Exchange"].ToString(), ConfigurationManager.AppSettings["RoutingKey"].ToString(), null, b);

                channel.Close();
                conn.Close();
                return 1;
            }
            catch (Exception c)
            {
                lbLog.Text = "Lỗi kết nối: " + c.Message;
                Utilities.WriteLog("Lỗi kết nối: " + c.Message);
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
