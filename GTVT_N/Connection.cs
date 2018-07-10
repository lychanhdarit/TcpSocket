using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RabbitMQ.Client;

namespace GSNReceiver.GTVT_N
{
    class Connection
    {
        
        /// <summary>
        /// tao ket noi den server GTVT
        /// </summary>
        /// <returns></returns>
        public static IConnection createConnection()
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.UserName ="gstc";
            factory.Password = "d154623c";
            factory.VirtualHost = "/";
            factory.Protocol = Protocols.FromEnvironment();
            factory.HostName = "210.211.102.123";
            factory.Port = 5727;
            IConnection conn = factory.CreateConnection();
            return conn;
        }
    }
}
