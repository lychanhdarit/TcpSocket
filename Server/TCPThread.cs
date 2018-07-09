using ServerData;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class TCPThread
    {
        static Socket listenerSocket;
        static List<ClientData> _clients;
        static void Main(string[] args)
        {

            //string StringT = ConfigurationManager.AppSettings["StringT"].ToString();
            //string[] StringTFirstKey = StringT.Split(',');
            //Console.WriteLine(StringTFirstKey.Length + " - " + StringT);
            //if (StringTFirstKey.Length > 0)
            //{

            //    for (int i = 0; i < StringTFirstKey.Length -1; i++)
            //    {
            //        Console.WriteLine("ST: " + StringTFirstKey[i]);
            //        //if (msg.IndexOf(StringTFirstKey[i]) > -1 && StringTFirstKey[i] != "")
            //        //{
            //        //    _db.excuteMsgToDB(int.Parse(DeviceID), msg);
            //        //    PrintWithColorSilver("-----------------------------------------------------------");
            //        //    break;
            //        //}
            //    }

            //}
            //Console.ReadLine();

            listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _clients = new List<ClientData>();

            string PORT = ConfigurationManager.AppSettings["Port"].ToString();
            string IP_CORE_ADDRESS = ConfigurationManager.AppSettings["IPAddress"].ToString();

            //IP_CORE_ADDRESS = Packet.GetIp4Address();

            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(IP_CORE_ADDRESS), int.Parse(PORT));
            listenerSocket.Bind(ip);


            PrintWithColorGreen("---------------------------------------------------------------------------");
            PrintWithColorGreen("IP Host: " + IP_CORE_ADDRESS + " - Port: " + PORT);
            PrintWithColorGreen("Waiting for a connection from " + IP_CORE_ADDRESS + " on port " + PORT + " connection ...");
            PrintWithColorGreen("---------------------------------------------------------------------------");

            Thread listenThread = new Thread(ListenThread);
            listenThread.Start();
        }

        static void ListenThread()
        {
            for (;;)
            {
                listenerSocket.Listen(0);
                _clients.Add(new ClientData(listenerSocket.Accept()));
            }

        }
       
        public static void Data_IM(object cSocket)
        {
            DbClass _db = new DbClass();
            Socket clientSocket = (Socket)cSocket;
            byte[] Buffer = new byte[65507];
            int readBytes = 0;
            string StringT = ConfigurationManager.AppSettings["StringT"].ToString();
            string[] StringTFirstKey = StringT.Split(',');
            string DeviceID = ConfigurationManager.AppSettings["deviceID"].ToString();
            string DeCODE = ConfigurationManager.AppSettings["DeCODE"].ToString();
            for (;;)
            {

                Buffer = new byte[clientSocket.SendBufferSize];
                try
                {
                    readBytes = clientSocket.Receive(Buffer);
                    
                    if (readBytes > 0)
                    {
                        //Packet packet = new Packet(Buffer);

                        string msg = "";
                        if(DeCODE == "1")
                        {
                            msg = GetHexStringFrom(Buffer);
                        }
                        else
                        {
                            var byteArray = Buffer.TakeWhile((v, index) => Buffer.Skip(index).Any(w => w != 0x00)).ToArray();
                            msg = Encoding.ASCII.GetString(byteArray, 0, byteArray.Length);
                            
                        }

                        Console.WriteLine(DateTime.Now.ToString() + ": ");
                        PrintWithColor(msg);
                        //PrintWithColor("Bit: " + BitConvert(Buffer));
                        PrintWithColorSilver("---------------------------");
                        Utilities.WriteLog(msg);

                        if (StringTFirstKey.Length > 0)
                        {

                            for (int i = 0; i < StringTFirstKey.Length; i++)
                            {
                                //Console.WriteLine("ST: "+StringTFirstKey[i] );
                                if (msg.IndexOf(StringTFirstKey[i]) > -1 && StringTFirstKey[i].Replace(" ","") !="")
                                {
                                    _db.excuteMsgToDB(int.Parse(DeviceID), msg);
                                    PrintWithColorSilver("-----------------------------------------------------------");
                                    break;
                                }
                            }

                        }
                        else
                        {
                            
                            //if (msg.IndexOf(StringT) > -1 && StringT != "")
                            //{
                                _db.excuteMsgToDB(int.Parse(DeviceID), msg);
                                PrintWithColorSilver("-----------------------------------------------------------");
                            //}
                        }
                        

                        ////DataManager(packet);
                        ////Print  Color

                    }
                }
                catch(Exception c)
                {
                    PrintWithColorRed(c.Message);
                    //PrintWithColorRed("Disconnect! ");
                    return;
                }
            }
        }
        public static string GetHexStringFrom(byte[] data)
        {
            byte[] byteArray=  data.TakeWhile((v, index) => data.Skip(index).Any(w => w != 0x00)).ToArray();
            return BitConverter.ToString(byteArray).Replace("-",string.Empty); //To convert the whole array
        }
        public static void DataManager(Packet p)
        {
            switch (p.packetType)
            {
                case PacketType.Chat:
                    foreach (ClientData c in _clients)
                    {
                        c.clientSocket.Send(p.ToBytes());
                    }
                    //Console.WriteLine("Received a packet for registration Responding...");
                    //Packet ps = new Packet(PacketType.Registration);
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
        //start 
        //listener 
        //client data
        //data manager
    }
    class ClientData
    {
        public Socket clientSocket;
        public Thread clientThread;
        public string id;

        public ClientData()
        {
            id = Guid.NewGuid().ToString();
            clientThread = new Thread(TCPThread.Data_IM);
            clientThread.Start(clientSocket);
            SendRegistrationPacket();
        }
        public ClientData(Socket _clientSocket)
        {
            this.clientSocket = _clientSocket;
            id = Guid.NewGuid().ToString();
            clientThread = new Thread(TCPThread.Data_IM);
            clientThread.Start(clientSocket);
            SendRegistrationPacket();
        }

        public void SendRegistrationPacket()
        {
            Packet p = new Packet(PacketType.Registration, "server");
            p.Gdata.Add(id);
            clientSocket.Send(p.ToBytes());
        }
    }
}
