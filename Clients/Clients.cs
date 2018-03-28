using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Threading;
using System.Net.Sockets;
using ServerData;

namespace Clients
{
    class Clients
    {
        public static Socket master;
        public static string name;
        public static string id;
        static void Main(string[] args)
        {
            //Console.Write("Eneter your name:");
            name = "DR";// Console.ReadLine();
            A: Console.Clear();
            //Console.Write("Enter host Ip:");
            string ip = "192.168.2.124";// Console.ReadLine();

            master = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint IP = new IPEndPoint(IPAddress.Parse(ip),4242);
            try
            {
                master.Connect(IP);
                Console.WriteLine("Connected: "+IP);
            }
            catch
            {
                Console.WriteLine("Could not connect to host!");
                Thread.Sleep(1000);
                goto A;
            }

            Thread t = new Thread(Data_IM);
            t.Start();
            for(;;)
            {
                Console.Write("::>");
                string input = Console.ReadLine();
                Packet p = new Packet(PacketType.Chat, id);
                p.Gdata.Add(name);
                p.Gdata.Add(input);
                master.Send(p.ToBytes());
            }
        }
        static void Data_IM()
        {
            byte[] Buffer;
            int readBytes;
            for (;;)
            {
                Buffer = new byte[master.SendBufferSize];
                readBytes = master.Receive(Buffer);
                if (readBytes > 0)
                {
                    //Packet packet = new Packet(Buffer);
                    DataManager(new Packet(Buffer));
                }
            }
        }
        public static void DataManager(Packet p)
        {
            switch(p.packetType)
            {
                case PacketType.Registration:
                    //Console.WriteLine("Received a packet for registration Responding...");
                    id = p.Gdata[0];
                    //Packet ps = new Packet(PacketType.Registration);
                    break;
                case PacketType.Chat:
                    ConsoleColor c = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(p.Gdata[0] + ":"+ p.Gdata[p.Gdata.Count-1]);
                    Console.ForegroundColor = c;
                    break;
            }
        }
    }
}
