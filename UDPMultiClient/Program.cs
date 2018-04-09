using ServerData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UDPMultiClient;

namespace UDPMultiClient
{
    enum Command
    {
        Login,      //Log into the server
        Logout,     //Logout of the server
        Message,    //Send a text message to all the chat clients
        List,       //Get a list of users in the chat room from the server
        Null        //No command
    }
    class Program
    {
        struct ClientInfo
        {
            public EndPoint endpoint;   //Socket of the client
            //public string strName;      //Name by which the user logged into the chat room
        }

        //The collection of all clients logged into the room (an array of type ClientInfo)
        static ArrayList clientList;

        //The main socket on which the server listens to the clients
        static Socket serverSocket;

        static byte[] byteData = new byte[1024];

        static string PORT = ConfigurationManager.AppSettings["Port"].ToString();
        static string IP_CORE_ADDRESS = ConfigurationManager.AppSettings["IPAddress"].ToString();



        static void Main(string[] args)
        {
            try
            {
                //CheckForIllegalCrossThreadCalls = false;
                clientList = new ArrayList();
                //We are using UDP sockets
                serverSocket = new Socket(AddressFamily.InterNetwork,
                    SocketType.Dgram, ProtocolType.Udp);

                //Assign the any IP of the machine and listen on port number 1000
                //IP_CORE_ADDRESS = "192.168.2.123";
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(IP_CORE_ADDRESS), int.Parse(PORT));
                //Bind this address to the server
                serverSocket.Bind(ipEndPoint);
                PrintWithColorGreen("-----------------------------------------------------------");
                PrintWithColorGreen(IP_CORE_ADDRESS + ": Wating for a client connect...");
                PrintWithColorGreen("-----------------------------------------------------------");
                //new IPEndPoint(IPAddress.Parse(IP_CORE_ADDRESS), int.Parse(PORT));
                IPEndPoint ipeSender = new IPEndPoint(IPAddress.Any, 0);
                //The epSender identifies the incoming clients
                EndPoint epSender = (EndPoint)ipeSender;

                //Start receiving data
                serverSocket.BeginReceiveFrom(byteData, 0, byteData.Length,
                    SocketFlags.None, ref epSender, new AsyncCallback(OnReceive), epSender);

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                PrintWithColorRed("Loi 1: " + ex.Message);
            }
        }

        private static void OnReceive(IAsyncResult ar)
        {
            try
            {
                IPEndPoint ipeSender = new IPEndPoint(IPAddress.Any, 0);
                EndPoint epSender = (EndPoint)ipeSender;

                serverSocket.EndReceiveFrom(ar, ref epSender);

                //Transform the array of bytes received from the user into an
                //intelligent form of object Data
                Data msgReceived = new Data(byteData);

                //We will send this object in response the users request
                Data msgToSend = new Data();

                byte[] message;

                //If the message is to login, logout, or simple text message
                //then when send to others the type of the message remains the same
                msgToSend.cmdCommand = msgReceived.cmdCommand;
                msgToSend.strName = msgReceived.strName;
                message = msgToSend.ToByte();



                ClientInfo clientInfo = new ClientInfo();
                clientInfo.endpoint = epSender;

                int clientExist = 0;
                foreach (ClientInfo cl in clientList)
                {
                    if (clientInfo.endpoint.ToString() == cl.endpoint.ToString())
                    {
                        clientExist = 1;
                        break;
                    }
                }
                if (clientExist == 0)
                {
                    
                    clientList.Add(clientInfo);
                    PrintWithColorGreen("<<<" + clientInfo.endpoint.ToString() + " has joined>>>");
                    PrintWithColorSilver("-----------------------------------------------------------");
                    PrintWithColor("Revc " + DateTime.Now.ToString() + " : " + msgReceived.strMessage);

                    SendSocketData(msgReceived.strMessage, serverSocket, clientInfo.endpoint);
                }
                else
                {
                    PrintWithColorSilver("-----------------------------------------------------------");
                    PrintWithColor("Revc " + DateTime.Now.ToString() + " : " + msgReceived.strMessage);
                    
                    SendSocketData(msgReceived.strMessage, serverSocket, clientInfo.endpoint);
                }

               
                //If the user is logging out then we need not listen from her
                if (msgReceived.cmdCommand != Command.Logout)
                {
                    //Start listening to the message send by the user
                    serverSocket.BeginReceiveFrom(byteData, 0, byteData.Length, SocketFlags.None, ref epSender,
                        new AsyncCallback(OnReceive), epSender);
                }

            }
            catch (Exception ex)
            {
                PrintWithColorRed("Loi 2: " + ex.Message);

            }
        }

        public static void OnSend(IAsyncResult ar)
        {
            try
            {
                serverSocket.EndSend(ar);
            }
            catch (Exception ex)
            {
                PrintWithColorRed(ex.Message);
                //MessageBox.Show(ex.Message, "SGSServerUDP", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        static string TypeDevices(string data)
        {
            if (data.IndexOf("UP") > -1)
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
        static void SendSocketData(string data, Socket socket, EndPoint ep)
        {
            DBClass _db = new DBClass();
            string sendString = "";
            string[] Adta = data.Split(',');

            string[] StringTFirstKey = ConfigurationManager.AppSettings["StringT"].ToString().Split(',');
            string DeviceID = ConfigurationManager.AppSettings["deviceID"].ToString();
            //Excute to DB
            if (StringTFirstKey.Length > 0)
            {

                for (int i = 0; i < StringTFirstKey.Length; i++)
                {
                    if (data.IndexOf(StringTFirstKey[i]) > -1)
                    {
                        _db.excuteMsgToDB(int.Parse(DeviceID), data);
                        break;
                    }
                }

            }


            // Send Back
            switch (TypeDevices(data))
            {
                case "UP":
                    if (Adta.Length > 5)
                    {


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


                        sendString = Adta[0] + "," + Adta[1] + "," + Adta[2] + "," + Adta[6] + "," + Adta[7] + "#";
                        socket.SendTo(Encoding.ASCII.GetBytes(sendString), ep);
                        PrintWithColorGreen("Send:" + sendString);
                        PrintWithColorSilver("-----------------------------------------------------------");
                        Utilities.WriteLog("Send:" + sendString);
                    }
                    break;
                case "ST":
                    if (Adta.Length > 8)
                    {


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

    class Data
    {
        //Default constructor
        public Data()
        {
            this.cmdCommand = Command.Null;
            this.strMessage = null;
            this.strName = null;
        }

        //Converts the bytes into an object of type Data
        public Data(byte[] data)
        {
            //The first four bytes are for the Command
            //this.cmdCommand = (Command)BitConverter.ToInt32(data, 0);

            ////The next four store the length of the name
            //int nameLen = BitConverter.ToInt32(data, 4);

            ////The next four store the length of the message
            //int msgLen = BitConverter.ToInt32(data, 8);

            ////This check makes sure that strName has been passed in the array of bytes
            //if (nameLen > 0)
            //    this.strName = Encoding.UTF8.GetString(data, 12, nameLen);
            //else
            //    this.strName = null;

            //This checks for a null message field
            //if (msgLen > 0)
            //    this.strMessage = Encoding.UTF8.GetString(data, 12 + nameLen, msgLen);
            //else
            //    this.strMessage = null;


            var byteArray = data.TakeWhile((v, index) => data.Skip(index).Any(w => w != 0x00)).ToArray();
            strMessage = Encoding.ASCII.GetString(byteArray, 0, byteArray.Length);
        }


        //Converts the Data structure into an array of bytes
        public byte[] ToByte()
        {
            List<byte> result = new List<byte>();

            //First four are for the Command
            result.AddRange(BitConverter.GetBytes((int)cmdCommand));

            //Add the length of the name
            if (strName != null)
                result.AddRange(BitConverter.GetBytes(strName.Length));
            else
                result.AddRange(BitConverter.GetBytes(0));

            //Length of the message
            if (strMessage != null)
                result.AddRange(BitConverter.GetBytes(strMessage.Length));
            else
                result.AddRange(BitConverter.GetBytes(0));

            //Add the name
            if (strName != null)
                result.AddRange(Encoding.UTF8.GetBytes(strName));

            //And, lastly we add the message text to our array of bytes
            if (strMessage != null)
                result.AddRange(Encoding.UTF8.GetBytes(strMessage));

            return result.ToArray();
        }

        public string strName;      //Name by which the client logs into the room
        public string strMessage;   //Message text
        public Command cmdCommand;  //Command type (login, logout, send message, etcetera)
    }

}
