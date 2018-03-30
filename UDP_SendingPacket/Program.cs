﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace UDP_SendingPacket
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] packetData = ASCIIEncoding.ASCII.GetBytes("<The Packet Here>");

            //Port and Ip Data for Socket Client
            string IpAdress = "192.168.2.124";
            int port = 904;
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(IpAdress), port);
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            //client.send
            //client.SendTimeout(1);
            client.SendTo(packetData,ep);
        }
    }
}