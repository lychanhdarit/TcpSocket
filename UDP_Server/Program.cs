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
            PrintWithColorGreen("--------------------------------------------------------------------------");
            PrintWithColorGreen(IP_CORE_ADDRESS + ": Wating for a client connect..."  );

            IPEndPoint sender = new IPEndPoint(IPAddress.Parse(IP_CORE_ADDRESS), int.Parse(PORT));
            EndPoint tmpRemote = (EndPoint)sender;
            recv = newSocket.ReceiveFrom(data, 0, ref tmpRemote);
            PrintWithColorGreen("--------------------------------------------------------------------------");
            PrintWithColor("Received from: "+ tmpRemote.ToString());
            PrintWithColor(Encoding.ASCII.GetString(data, 0, recv));
            PrintWithColorSilver("-----------------------------------------------------------");

            
            //string welcome = "Welcome to server!";
            //data = Encoding.ASCII.GetBytes(welcome);

            if (newSocket.Connected)
            {
               
                SendSocketData(Encoding.ASCII.GetString(data, 0, recv), newSocket, tmpRemote);

                //newSocket.Send(data);
            }
            while (true)
            {
                //if (!newSocket.Connected)
                //{
                //    Console.WriteLine("Client Disconnected!");
                //    break;
                //}
                try
                {
                    data = new byte[1024];
                    recv = newSocket.ReceiveFrom(data, ref tmpRemote);

                    if (recv == 0)
                        break;

                    string dataRecv = Encoding.ASCII.GetString(data, 0, recv);
                    PrintWithColor("Receive "+DateTime.Now.ToString()+":" );
                    PrintWithColor(dataRecv);
                    PrintWithColorSilver("-----------------------------------------------------------");
                    Utilities.WriteLog("Receive " + DateTime.Now.ToString() + ":" + dataRecv);

                    SendSocketData(dataRecv, newSocket,tmpRemote);
                }
                catch (Exception c){

                    PrintWithColorRed(c.Message);
                    Utilities.WriteLog(c.Message);
                }

              

               

            }
            newSocket.Close();

            Console.ReadKey();
        }
        static string TypeDevices(string data)
        {
            if(data.IndexOf("UP") > -1)
            {
                return "UP";
            }
            if (data.IndexOf("ML") > -1)
            {
                return "ML";
            }
            if (data.IndexOf("ST") > -1)
            {
                return "ST";
            }
            return "";
        }
        static void SendSocketData(string data,Socket socket,EndPoint ep)
        {
            DbClass _db = new DbClass();
            string sendString = "";
            
            string[] Adta = data.Split(',');
            switch (TypeDevices(data))
            {
                case "UP":
                    if(Adta.Length>5)
                    {
                        _db.excuteMsgToDB(9999, data);

                        sendString = Adta[0] + "," + Adta[1] + "," + Adta[2] + "," + Adta[4] + "," + Adta[5] + "#";
                        socket.SendTo(Encoding.ASCII.GetBytes(sendString), ep);
                        PrintWithColorGreen("Send:" + sendString);
                        PrintWithColorSilver("-----------------------------------------------------------");
                        Utilities.WriteLog("Send:" + sendString);
                    }
                    break;
                case "ML":
                    if (Adta.Length > 7)
                    {
                        _db.excuteMsgToDB(9999, data);

                        sendString = Adta[0] +","+ Adta[1] + "," + Adta[2] + "," + Adta[6] + "," + Adta[7] + "#";
                        socket.SendTo(Encoding.ASCII.GetBytes(sendString), ep);
                        PrintWithColorGreen("Send:" + sendString);
                        PrintWithColorSilver("-----------------------------------------------------------");
                        Utilities.WriteLog("Send:" + sendString);
                    }
                    break;
                case "ST":
                    if (Adta.Length > 8)
                    {
                        _db.excuteMsgToDB(9999, data);

                        sendString = Adta[0] + "," + Adta[1] + "," + Adta[2] + "," + Adta[8] + "," + Adta[8] + "#";
                        socket.SendTo(Encoding.ASCII.GetBytes(sendString), ep);
                        PrintWithColorGreen("Send:" + sendString);
                        PrintWithColorSilver("-----------------------------------------------------------");
                        Utilities.WriteLog("Send:" + sendString);
                    }
                    break;
            } 
        }
        #region Color


        static void PrintWithColor(string data)
        {
            //CyAn = 11
            ConsoleColor c = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(data);
            Console.ForegroundColor = c;
        }

        static void PrintWithColorSilver(string data)
        {
            //CyAn = 11
            ConsoleColor c = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(data);
            Console.ForegroundColor = c;
        }
        static void PrintWithColorGreen(string data)
        {
            //CyAn = 11
            ConsoleColor c = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(data);
            Console.ForegroundColor = c;
        }
        static void PrintWithColorRed(string data)
        {
            //CyAn = 11
            ConsoleColor c = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(data);
            Console.ForegroundColor = c;
        }


        #endregion
    }
}
