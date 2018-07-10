using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ufms;
using ProtoBuf;
using RabbitMQ.Client;
using System.IO;
using System.Configuration;
namespace GSNReceiver.GTVT_N
{
    class SendReport
    {
        IConnection conn;
        IModel chanel;
        BaseMessage msg;
        byte[] b;

        /// <summary>
        /// gui thong tin xe qua toc do cho phep 
        /// su dung trong  truong ban tin tracker yeu khong du de tinh toan
        /// </summary>
        public void sendOverSpeed()
        {
            //>80 km/h

            try
            {
                string path = ConfigurationManager.AppSettings["OverSpeed"];
                string[] lines = Common.readFile(path);
                OverSpeed os;
                conn = Connection.createConnection();
                int counter = 0;
                foreach (var line in lines)
                {
                    string[] osDetails = line.Replace("\r", "").Replace("\n", "").Split(';');
                    if (osDetails.Length == 11 && !osDetails[0].ToString().Trim().Equals("") && !osDetails.ToString().Trim().Equals(null))
                    {
                        os = new OverSpeed();
                        os.vehicle = osDetails[0];//bien so xe
                        os.driver = osDetails[1];//ma bang lai
                        os.speedMax = float.Parse(osDetails[2]);//toc do toi da
                        os.speedLimit = float.Parse(osDetails[3]);//toc do gioi han
                        os.speedAvg = float.Parse(osDetails[4]);//toc do trung binh
                        DateTime dateBegin = Convert.ToDateTime(osDetails[5]);
                        int iDateBegin = 0;
                        if (dateBegin != null)
                        {
                            int d = dateBegin.Day;
                            int m = dateBegin.Month;
                            int y = dateBegin.Year;
                            int h = dateBegin.Hour;
                            int ms = dateBegin.Minute;
                            int sc = dateBegin.Second;
                            DateTime date = new DateTime(y, m, d, h, ms, sc, DateTimeKind.Local);
                            iDateBegin = Common.DateTimeToUnixTimestamp(date);
                        }

                        DateTime dateEnd = Convert.ToDateTime(osDetails[6]);
                        int idateEnd = 0;
                        if (dateEnd != null)
                        {
                            int d = dateEnd.Day;
                            int m = dateEnd.Month;
                            int y = dateEnd.Year;
                            int h = dateEnd.Hour;
                            int ms = dateEnd.Minute;
                            int sc = dateEnd.Second;
                            DateTime date = new DateTime(y, m, d, h, ms, sc, DateTimeKind.Local);
                            idateEnd = Common.DateTimeToUnixTimestamp(date);
                        }
                        os.datetimeBegin = iDateBegin;//thoi gian bat dau
                        os.datetimeEnd = idateEnd;//thoi gian ket thuc
                        os.xBegin = Common.convertToDouble(osDetails[7]);//toa do x bat dau
                        os.yBegin = Common.convertToDouble(osDetails[8]);//toa do y  bat dau
                        os.xEnd = Common.convertToDouble(osDetails[9]);//toa do x ket thuc
                        os.yEnd = Common.convertToDouble(osDetails[10]);//toa do y ket thuc

                        if (!conn.IsOpen) conn = Connection.createConnection();
                        chanel = conn.CreateModel();
                        msg = new BaseMessage();
                        msg.msgType = BaseMessage.MsgType.OverSpeed;
                        Extensible.AppendValue<OverSpeed>(msg, BaseMessage.MsgType.OverSpeed.GetHashCode(), os);
                        b = Common.Serialize(msg);
                        chanel.BasicPublish("ufms.all", "", null, b);
                        chanel.Close();
                        counter++;
                    }
                }
                //thoat ket noi khi het vong lap
                if (conn.IsOpen) conn.Close();
                Console.WriteLine(counter);
                
            }
            catch (Exception ex)
            {
                if (conn.IsOpen) conn.Close();
                //create to log error
            }
       
         
            
        }
        /// <summary>
        /// Gửi dữ liệu khi  xe dừng đỗ 
        /// sử dụng trong trường hợp bản tin tracker yeu khong đủ dữ liệu tín toán
        /// </summary>
        /// <param name="str"></param>
        public  void sendStop()
        {
            try
            {
                string path = ConfigurationManager.AppSettings["Stop"];
                string[] lines = Common.readFile(path);
                Stop s;
                conn = Connection.createConnection();
                foreach (var line in lines)
                {
               
                     s = new Stop();
                     string[] stopDetails = line.Replace("\r", "").Replace("\n", "").Split(';');
                     if (stopDetails.Length.Equals(6) && !stopDetails[0].Trim().Equals("") && !stopDetails[0].Trim().Equals(null))
                     {
                         s.vehicle = stopDetails[0];//bien so xe
                         s.driver = stopDetails[1];//bang lai

                         DateTime dateBegin = Convert.ToDateTime(stopDetails[2]);
                         int iDateBegin = 0;
                         if (dateBegin != null)
                         {
                             int d = dateBegin.Day;
                             int m = dateBegin.Month;
                             int y = dateBegin.Year;
                             int h = dateBegin.Hour;
                             int ms = dateBegin.Minute;
                             int sc = dateBegin.Second;
                             DateTime date = new DateTime(y, m, d, h, ms, sc, DateTimeKind.Local);
                             iDateBegin = Common.DateTimeToUnixTimestamp(date);
                         }
                         DateTime dateEnd = Convert.ToDateTime(stopDetails[3]);
                         int idateEnd = 0;
                         if (dateEnd != null)
                         {
                             int d = dateEnd.Day;
                             int m = dateEnd.Month;
                             int y = dateEnd.Year;
                             int h = dateEnd.Hour;
                             int ms = dateEnd.Minute;
                             int sc = dateEnd.Second;
                             DateTime date = new DateTime(y, m, d, h, ms, sc, DateTimeKind.Local);
                             idateEnd = Common.DateTimeToUnixTimestamp(date);
                         }

                         s.datetimeBegin =iDateBegin;//thoi gian bat dau
                         s.datetimeEnd = idateEnd;//thoi gian ket thuc
                         s.x = Common.convertToDouble(stopDetails[4]);//toa do x
                         s.y = Common.convertToDouble(stopDetails[5]);//toa do y
                         if (!conn.IsOpen)
                         conn = Connection.createConnection();
                         
                         chanel = conn.CreateModel();
                         msg = new BaseMessage();
                         msg.msgType = BaseMessage.MsgType.Stop;
                         Extensible.AppendValue<Stop>(msg, BaseMessage.MsgType.Stop.GetHashCode(),s);
                         b = Common.Serialize(msg);
                         chanel.BasicPublish("ufms.all", "", null, b);
                         chanel.Close();
                         
                     }
                }
                if(conn.IsOpen) conn.Close();
            }
            catch (Exception ex)
            {
                if (conn.IsOpen) conn.Close();
                //write to log error
            }  
            
            

        }
        /// <summary>
        /// gửi dữ liệu khi mở cửa xe
        /// sử dụng trong trường hợp bản tin tracker yếu không đủ dữ liệu tính toán
        /// </summary>
        /// <param name="str"></param>
        public void sendDoorOpen()
        {
            try
            {
                string path = ConfigurationManager.AppSettings["DoorOpen"];
                string[] lines = Common.readFile(path);
                conn = Connection.createConnection();

                foreach (var line in lines)
                {
                    string[] doorOpen = line.Replace("\r", "").Replace("\n", "").Split(';');
                    DoorOpen op;
                    if (doorOpen.Length.Equals(9) && !doorOpen[0].Trim().Equals("") && !doorOpen[0].Trim().Equals(null))
                    {
                        op = new DoorOpen();
                        op.vehicle = doorOpen[0];//bien so xe
                        op.driver = doorOpen[1];//ma bang lai
                        DateTime dateBegin = Convert.ToDateTime(doorOpen[2]);
                        int iDateBegin = 0;
                        if (dateBegin != null)
                        {
                            int d = dateBegin.Day;
                            int m = dateBegin.Month;
                            int y = dateBegin.Year;
                            int h = dateBegin.Hour;
                            int ms = dateBegin.Minute;
                            int sc = dateBegin.Second;
                            DateTime date = new DateTime(y, m, d, h, ms, sc, DateTimeKind.Local);
                            iDateBegin = Common.DateTimeToUnixTimestamp(date);
                        }
                        DateTime dateEnd = Convert.ToDateTime(doorOpen[3]);
                        int idateEnd = 0;
                        if (dateEnd != null)
                        {
                            int d = dateEnd.Day;
                            int m = dateEnd.Month;
                            int y = dateEnd.Year;
                            int h = dateEnd.Hour;
                            int ms = dateEnd.Minute;
                            int sc = dateEnd.Second;
                            DateTime date = new DateTime(y, m, d, h, ms, sc, DateTimeKind.Local);
                            idateEnd = Common.DateTimeToUnixTimestamp(date);
                        }
                        op.datetimeBegin =iDateBegin;//thoi gian bat dau
                        op.datetimeEnd = idateEnd;//thoi gian ket thuc
                        op.xBegin = Common.convertToDouble(doorOpen[4]); ;//toa do x bat dau
                        op.yBegin = Common.convertToDouble(doorOpen[5]); ;//toa do y bat dau
                        op.xEnd = Common.convertToDouble(doorOpen[6]); ;//toa do x ket thuc
                        op.yEnd = Common.convertToDouble(doorOpen[7]); ;//toa do y ket thuc
                        op.speedMax =float.Parse(doorOpen[8]);//toc do 
                        if (!conn.IsOpen) conn = Connection.createConnection();

                        chanel = conn.CreateModel();
                        msg = new BaseMessage();
                        msg.msgType = BaseMessage.MsgType.DoorOpen;
                        Extensible.AppendValue<DoorOpen>(msg, BaseMessage.MsgType.DoorOpen.GetHashCode(),op);
                        b = Common.Serialize(msg);
                        chanel.BasicPublish("ufms.all", "", null, b);
                        chanel.Close();
                    }
                }
                //thoat  connectioin khi het vong lap
                if (conn.IsOpen) conn.Close();
                
            }
            catch (Exception ex)
            {
                if (conn.IsOpen) conn.Close();
                //create to log error 
            }
            
           

        }
        /// <summary>
        /// gửi dữ liệu quá thời gian chạy liên tục 4h
        /// sử dụng trong trường hợp tracker yếu không đủ để tính toán
        /// </summary>
        /// <param name="str"></param>
        public void sendoverTimeDetailsDriving()
        {
            try
            {
                string path = ConfigurationManager.AppSettings["OverTimeDriving"];
                string[] lines = Common.readFile(path);
                conn = Connection.createConnection();
                foreach (var line in lines)
                {
                    string[] overTimeDetails = line.Replace("\r", "").Replace("\n", "").Split(';');
                    if (overTimeDetails.Length.Equals(6) && !overTimeDetails[0].Trim().Equals("") && !overTimeDetails[0].Trim().Equals(null))
                    {
                        OverTimeDriving otd = new OverTimeDriving();
                        
                        
                        otd.vehicle = overTimeDetails[0];//bien so xe
                        otd.driver = overTimeDetails[1];//giay phep lai xe

                        DateTime dateBegin = Convert.ToDateTime(overTimeDetails[2]);
                        int iDateBegin = 0;
                        if (dateBegin != null)
                        {
                            int d = dateBegin.Day;
                            int m = dateBegin.Month;
                            int y = dateBegin.Year;
                            int h = dateBegin.Hour;
                            int ms = dateBegin.Minute;
                            int sc = dateBegin.Second;
                            DateTime date = new DateTime(y, m, d, h, ms, sc, DateTimeKind.Local);
                            iDateBegin = Common.DateTimeToUnixTimestamp(date);
                        }

                        otd.datetime = iDateBegin;//ngay chay
                        otd.x =Common.convertToDouble(overTimeDetails[3]);//toa do x
                        otd.y = Common.convertToDouble(overTimeDetails[4]); ;//toa do y
                        otd.duration =int.Parse(overTimeDetails[5]);//so gio chay

                        chanel = conn.CreateModel();
                        msg = new BaseMessage();
                        msg.msgType = BaseMessage.MsgType.OverTimeDriving;
                        Extensible.AppendValue<OverTimeDriving>(msg, BaseMessage.MsgType.OverTimeDriving.GetHashCode(), otd);
                        b = Common.Serialize(msg);
                        chanel.BasicPublish("ufms.all", "", null, b);
                        chanel.Close();
                    }
                }

                //ket thuc vong lap thoat connectio
                if (conn.IsOpen) conn.Close();
            }
            catch (Exception ex)
            {
                if (conn.IsOpen) conn.Close();
                //write to log error
            }
        }
        /// <summary>
        /// gửi dữ liệu quá thời gian cho phép chạy trong ngày 10h.
        /// sử dụng trong trường hợp tracker yếu không đủ để tính toán dữ liệu
        /// </summary>
        /// <param name="str"></param>
        public void sendOverTotalTimeDriving()
        {
            try
            {
                string path = ConfigurationManager.AppSettings["OverTotalTimeDriving"];
                string[] lines = Common.readFile(path);
                //conn = Connection.createConnection();
                foreach (var line in lines)
                {
                    string[] totalOverTimeDetails = line.Replace("\r", "").Replace("\n", "").Split(';');
                    if (totalOverTimeDetails.Length.Equals(6) && !totalOverTimeDetails[0].Trim().Equals("") && !totalOverTimeDetails[0].Equals(null))
                    {

                        OverTotalTimeDriving ottd = new OverTotalTimeDriving();
                        ottd.vehicle = totalOverTimeDetails[0];//bien so xe
                        ottd.driver = totalOverTimeDetails[1];//giay phep lai xe

                        DateTime dateBegin = Convert.ToDateTime(totalOverTimeDetails[2]);
                        int iDateBegin = 0;
                        if (dateBegin != null)
                        {
                            int d = dateBegin.Day;
                            int m = dateBegin.Month;
                            int y = dateBegin.Year;
                            int h = dateBegin.Hour;
                            int ms = dateBegin.Minute;
                            int sc = dateBegin.Second;
                            DateTime date = new DateTime(y, m, d, h, ms, sc, DateTimeKind.Local);
                            iDateBegin = Common.DateTimeToUnixTimestamp(date);
                        }

                        ottd.datetime = iDateBegin;//ngay 
                        ottd.x =Common.convertToDouble(totalOverTimeDetails[3]);//toa do x
                        ottd.y = Common.convertToDouble(totalOverTimeDetails[4]);//toa do y
                        ottd.duration =int.Parse(totalOverTimeDetails[5]);//so gio chay
                        if (!conn.IsOpen) conn = Connection.createConnection();

                        chanel = conn.CreateModel();
                        msg = new BaseMessage();
                        msg.msgType = BaseMessage.MsgType.OverTotalTimeDriving;
                        Extensible.AppendValue<OverTotalTimeDriving>(msg, BaseMessage.MsgType.OverTotalTimeDriving.GetHashCode(), ottd);
                        b = Common.Serialize(msg);
                        chanel.BasicPublish("ufms.all", "", null, b);
                        chanel.Close();
                    
                    }
                }
                if (conn.IsOpen) conn.Close();
              
            }
            catch (Exception ex)
            {
                if (conn.IsOpen) conn.Close();
                //write to log error
            }
            
            
        }
    }
}
