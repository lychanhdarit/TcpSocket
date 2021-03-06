﻿using ServerData;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UDPThread
{
    class UDPThreadProgram
    {

        public Thread sampleUdpThread;
        public Thread restartThread;
        static string PORT = ConfigurationManager.AppSettings["Port"].ToString();
        static string IP_CORE_ADDRESS = ConfigurationManager.AppSettings["IPAddress"].ToString();
        public UDPThreadProgram()
        {
            try
            {
                //Starting the UDP Server thread.
                sampleUdpThread = new Thread(new ThreadStart(StartReceive));
                sampleUdpThread.Start();

                PrintWithColorGreen("-----------------------------------------------------------");
                PrintWithColorGreen(IP_CORE_ADDRESS + ": Wating for a client connect...");
                PrintWithColorGreen("-----------------------------------------------------------\n");

                //Console.WriteLine("Started SampleTcpUdpServer's UDP Receiver Thread!\n");

                try
                {
                    restartThread = new Thread(new ThreadStart(RestartApp));
                    restartThread.Start();
                }
                catch (Exception e)
                {
                    Utilities.WriteLog(e.ToString());
                    sampleUdpThread.Abort();
                }
            }
            catch (Exception e)
            {
                PrintWithColorRed("An UDP Exception has occurred!" + e.ToString());
                Utilities.WriteLog(e.ToString());
                sampleUdpThread.Abort();
            }

            
        }
        static void Main(string[] args)
        {
            UDPThreadProgram sts = new UDPThreadProgram(); 
        }
        private static void TimerCallback(Object o)
        {
            // Display the date/time when this method got called.
            //Console.WriteLine("In TimerCallback: " + DateTime.Now);
            // Force a garbage collection to occur for this demo.
            GC.Collect();
            int hourRestart = 12;
            try
            {
                hourRestart = int.Parse(ConfigurationManager.AppSettings["timeRestart"].ToString());
            }
            catch  {}

            DateTime nowdate = DateTime.Now;
            if (nowdate.Hour == hourRestart && nowdate.Minute == 1 && nowdate.Second == 1)
            {
                Utilities.WriteLog("Restart App!");
                //sampleUdpThread.Abort();
                //Start process, friendly name is something like MyApp.exe (from current bin directory)
                System.Diagnostics.Process.Start(System.AppDomain.CurrentDomain.FriendlyName);
                //Close the current process
                Environment.Exit(0);
            }
            
        }
        public void RestartApp()
        {
            Timer t = new Timer(TimerCallback, null, 0, 1000); 
        }
       public void StartReceive()
        {
            try
            {
                //IP_CORE_ADDRESS = "192.168.2.123";
                //Create a UDP socket.
                Socket soUdp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPEndPoint localIpEndPoint = new IPEndPoint(IPAddress.Parse(IP_CORE_ADDRESS), int.Parse(PORT));
                //soUdp.Listen(500);
                soUdp.Bind(localIpEndPoint);
               
                string DeCODE = ConfigurationManager.AppSettings["DeCODE"].ToString();
                while (true)
                {
                    try
                    {
                        
                        Byte[] received = new Byte[65507];
                        IPEndPoint tmpIpEndPoint = new IPEndPoint(IPAddress.Parse(IP_CORE_ADDRESS), int.Parse(PORT));
                        EndPoint remoteEP = (tmpIpEndPoint);
                      

                        int bytesReceived = 0;
                        try
                        {
                            bytesReceived = soUdp.ReceiveFrom(received, ref remoteEP);

                            String dataReceived = System.Text.Encoding.ASCII.GetString(received);


                            if (DeCODE == "1")
                            {
                                dataReceived = GetHexStringFrom(received);
                            }
                            else
                            {
                                var byteArray = received.TakeWhile((v, index) => received.Skip(index).Any(w => w != 0x00)).ToArray();
                                dataReceived = Encoding.ASCII.GetString(byteArray, 0, byteArray.Length);

                            }

                            PrintWithColor("Revc " + DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss tt") + " from " + remoteEP.ToString() + ": " + dataReceived);
                            Utilities.WriteLog("Revc from " + remoteEP.ToString() + ": " + dataReceived);
                            SendSocketData(dataReceived, soUdp, remoteEP);

                        }
                        catch (Exception C)
                        {
                            PrintWithColorRed("Error Revc: " + C.Message);
                            Utilities.WriteLogSocketError("Error Revc: " + C.Message);
                        }


                    }
                    catch (SocketException se)
                    {
                        PrintWithColorRed("SocketException: " + se.ToString());
                        Utilities.WriteLogSocketError("SocketException: " + se.ToString());

                    }
                }

            }

            catch (Exception se)
            {
                PrintWithColorRed("A Socket Exception has occurred!" + se.ToString());
                Utilities.WriteLog(se.ToString());
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
            //if (data.IndexOf("ST") > -1)
            //{
            //    return "ST";
            //}
            return "";
        }
        public static string GetHexStringFrom(byte[] data)
        {
            byte[] byteArray = data.TakeWhile((v, index) => data.Skip(index).Any(w => w != 0x00)).ToArray();
            return BitConverter.ToString(byteArray).Replace("-", string.Empty); //To convert the whole array
        }
        static void SendSocketData(string data, Socket socket, EndPoint ep)
        {
            DBClass _db = new DBClass();
            string sendString = "";
            string[] Adta = data.Split(',');

            string[] StringTFirstKey = ConfigurationManager.AppSettings["StringT"].ToString().Split(',');
            string DeviceID = ConfigurationManager.AppSettings["deviceID"].ToString();

            // Send Back
            switch (TypeDevices(data))
            {
                case "UP":
                    if (Adta.Length > 5)
                    {


                        sendString = Adta[0] + "," + Adta[1] + "," + Adta[2] + "," + Adta[4] + "," + Adta[5] + "#";
                        socket.SendTo(Encoding.ASCII.GetBytes(sendString), ep);
                        PrintWithColorGreen("Send " + DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss tt") + ":" + sendString);
                        PrintWithColorSilver("-----------------------------------------------------------");
                        Utilities.WriteLog("Send:" + sendString);
                    }
                    break;
                case "ML":
                    if (Adta.Length > 7)
                    {


                        sendString = Adta[0] + "," + Adta[1] + "," + Adta[2] + "," + Adta[6] + "," + Adta[7] + "#";
                        socket.SendTo(Encoding.ASCII.GetBytes(sendString), ep);
                        PrintWithColorGreen("Send "+ DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss tt") + ":" + sendString);
                        PrintWithColorSilver("-----------------------------------------------------------");
                        Utilities.WriteLog("Send:" + sendString);
                    }
                    break;
                //case "ST":
                //    if (Adta.Length > 8)
                //    {


                //        sendString = Adta[0] + "," + Adta[1] + "," + Adta[2] + "," + Adta[8] + "," + Adta[9] + "#";
                //        socket.SendTo(Encoding.ASCII.GetBytes(sendString), ep);
                //        PrintWithColorGreen("Send " + DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss tt") + ":" + sendString);
                //        PrintWithColorSilver("-----------------------------------------------------------");
                //        Utilities.WriteLog("Send:" + sendString);
                //    }
                //    break;
            }

            //Excute to DB
            if (StringTFirstKey.Length > 0)
            {

                for (int i = 0; i < StringTFirstKey.Length; i++)
                {
                    if (data.IndexOf(StringTFirstKey[i]) > -1 && !String.IsNullOrEmpty(StringTFirstKey[i]))
                    {
                        _db.excuteMsgToDB(int.Parse(DeviceID), data);
                        PrintWithColorSilver("-----------------------------------------------------------");
                        break;
                    }
                }

            }
            else
            {
                _db.excuteMsgToDB(int.Parse(DeviceID), data);
                PrintWithColorSilver("-----------------------------------------------------------");
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
