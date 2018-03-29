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
    class Server
    {
        static Socket listenerSocket;
        static List<ClientData> _clients;
        static void Main(string[] args)
        {
            listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _clients = new List<ClientData>();

            string PORT = ConfigurationManager.AppSettings["Port"].ToString();
            string IP_CORE_ADDRESS = ConfigurationManager.AppSettings["IPAddress"].ToString();

            IP_CORE_ADDRESS = Packet.GetIp4Address();

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
            Socket clientSocket = (Socket)cSocket;
            byte[] Buffer = new byte[1024];
            int readBytes = 0;
            for (;;)
            {

                Buffer = new byte[clientSocket.SendBufferSize];
                try
                {
                    //try
                    //StateObject state = (StateObject)ar.AsyncState;
                    readBytes = clientSocket.Receive(Buffer);
                    //PrintWithColorRed("Disconnect! ");
                    //return;
                    //catch

                    //PrintWithColor("Done!");

                    if (readBytes > 0)
                    {
                        //Packet packet = new Packet(Buffer);
                        //string sl = Encoding.UTF8.GetString(Buffer, 0, Buffer.Length);
                        
                        //string sl = Encoding.ASCII.GetString(Buffer, 0, Buffer.Length);

                        StringBuilder MsgBuilder = new StringBuilder();

                       
                        String msgFull = MsgBuilder.Append(Encoding.ASCII.GetString(Buffer, 0, Buffer.Length-1)).ToString();

                        string msg = GetHexStringFrom(Buffer).Replace("-00", string.Empty);// msgFull.Replace("\0", string.Empty);

                        msg = BitConvert(Buffer);




                        Console.WriteLine(DateTime.Now.ToString() + ": ");
                        PrintWithColor(msg);
                        PrintWithColorSilver("---------------------------");
                        Utilities.WriteLog(msg);

                        ////DataManager(packet);
                        ////Print  Color
                        //PrintWithColor(packet.ToString());

                    }
                }
                catch(Exception c)
                {
                    PrintWithColorRed(c.Message);
                    PrintWithColorRed("Disconnect! ");
                    return;
                }
            }
        }
        public static string BitConvert(byte[] byteArray)
        {
            string result = "";
            for(int i=0;i< byteArray.Length;i++ )
            {
                result += Convert.ToString(byteArray[i], 2);
            }
            return result;
        }
        public static string GetHexStringFrom(byte[] byteArray)
        {
            return BitConverter.ToString(byteArray); //To convert the whole array
        }
        public static string BytesToString(byte[] Buffer)
        {
            return (Encoding.Default.GetString(Buffer,0,Buffer.Length - 1)).Split(new string[] { "\r\n", "\r", "\n" },StringSplitOptions.None)[0];
        }
        static string BytesToStringConverted(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                using (var streamReader = new StreamReader(stream))
                {
                    return streamReader.ReadToEnd();
                }
            }
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
            clientThread = new Thread(Server.Data_IM);
            clientThread.Start(clientSocket);
            SendRegistrationPacket();
        }
        public ClientData(Socket _clientSocket)
        {
            this.clientSocket = _clientSocket;
            id = Guid.NewGuid().ToString();
            clientThread = new Thread(Server.Data_IM);
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
