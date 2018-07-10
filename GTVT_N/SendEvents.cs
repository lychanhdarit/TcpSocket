using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ufms;
using ProtoBuf;
using RabbitMQ.Client;
namespace GSNReceiver.GTVT_N
{
    class SendEvents
    {
        
        /// <summary>
        /// sử dụng gửi khi xe thay doi trạng thái
        /// </summary>
        /// <param name="str">chuoi tracker tra ve</param>
        public void sendOverTimeDrivingBegin(string str)
        {
            OverTimeDrivingBegin otdb = new OverTimeDrivingBegin();
            otdb.vehicle = "";
            otdb.driver = "";
            otdb.datetime = 0;
            otdb.x = 0;
            otdb.y = 0;
        }
        /// <summary>
        /// sử dụng gửi khi xe thay doi trạng thái
        /// </summary>
        /// <param name="str">chuoi tracker tra ve</param>
        public void sendOverTotalTimeDrivingBegin(string str)
        {
            OverTotalTimeDrivingBegin ottdb = new OverTotalTimeDrivingBegin();
            ottdb.vehicle = "";
            ottdb.driver = "";
            ottdb.datetime = 0;
            ottdb.x = 0;
            ottdb.y = 0;
        }
    }
}
