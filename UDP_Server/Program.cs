using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using ServerData;
using System.Configuration;

namespace UDP_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            int recv;
            byte[] data = new byte[1024];

            //string IP_CORE_ADDRESS = Packet.GetIp4Address();

            string PORT = ConfigurationManager.AppSettings["Port"].ToString();
            string IP_CORE_ADDRESS = ConfigurationManager.AppSettings["IPAddress"].ToString();

            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(IP_CORE_ADDRESS), int.Parse(PORT));
            Socket newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            newSocket.Bind(ep);
            Console.WriteLine( "{0}: Wating for a client connect...", IP_CORE_ADDRESS );

            IPEndPoint sender = new IPEndPoint(IPAddress.Parse(IP_CORE_ADDRESS), int.Parse(PORT));
            EndPoint tmpRemote = (EndPoint)sender;
            recv = newSocket.ReceiveFrom(data, 0, ref tmpRemote);

            Console.Write("Received from: {0}", tmpRemote.ToString());
            Console.WriteLine(Encoding.ASCII.GetString(data, 0, recv));

            string welcome = "Welcome to server!";

            data = Encoding.ASCII.GetBytes(welcome);
            if (newSocket.Connected)
            {
                newSocket.Send(data);
            }
            while (true)
            {
                if (!newSocket.Connected)
                {
                    Console.WriteLine("Client Disconnected!");
                    break;
                }

                data = new byte[1024];
                recv = newSocket.ReceiveFrom(data, ref tmpRemote);

                if (recv == 0)
                    break;

                
                Console.WriteLine("Receive:" + Encoding.ASCII.GetString(data, 0, recv));

                newSocket.Send(Encoding.ASCII.GetBytes("#1:359672050411365:2:*,00000000,UP,12031e,0f1806#"));

                Console.WriteLine("Send:" + Encoding.ASCII.GetString(data, 0, recv));

            }
            newSocket.Close();

            Console.ReadKey();
        }
    }
}
