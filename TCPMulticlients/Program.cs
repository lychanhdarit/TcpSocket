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

namespace TCPMulticlients
{
    public class StateObject
    {
        // Client  socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 65507;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }

    public class AsynchronousSocketListener
    {
        // Thread signal.
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public AsynchronousSocketListener()
        {
        }

        public static void StartListening()
        {
            // Data buffer for incoming data.
            byte[] bytes = new Byte[65507];

            // Establish the local endpoint for the socket.
            // The DNS name of the computer
            // running the listener is "host.contoso.com".
            string PORT = ConfigurationManager.AppSettings["Port"].ToString();
            string IP_CORE_ADDRESS = ConfigurationManager.AppSettings["IPAddress"].ToString();
            //IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            //IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(IP_CORE_ADDRESS), int.Parse(PORT));

            // Create a TCP/IP socket.
            Socket listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (true)
                {
                    // Set the event to nonsignaled state.
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connections.
                    Console.WriteLine("Waiting for a connection...");
                    listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        listener);

                    // Wait until a connection is made before continuing.
                    allDone.WaitOne();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();

        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.
            allDone.Set();

            // Get the socket that handles the client request.
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        public static void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;
            // Read data from the client socket. 
            try
            {
                int bytesRead = handler.EndReceive(ar);
                string DeviceID = ConfigurationManager.AppSettings["deviceID"].ToString();
                string DeCODE = ConfigurationManager.AppSettings["DeCODE"].ToString();
                string StringT = ConfigurationManager.AppSettings["StringT"].ToString();
                string[] StringTFirstKey = StringT.Split(',');
                if (bytesRead > 0)
                {

                    // There  might be more data, so store the data received so far.
                    state.sb.Append(Encoding.ASCII.GetString(
                        state.buffer, 0, bytesRead));
                    // Check for end-of-file tag. If it is not there, read 
                    // more data.

                    //-
                    Console.WriteLine("Mã linh tinh---------------");
                    Console.WriteLine(state.sb);
                    Console.WriteLine("Mã Hex----------------------");
                    Console.WriteLine(GetHexStringFrom(state.buffer));
                    Console.WriteLine("Mã String------------------");
                    content = state.sb.ToString();
                    //if (content.IndexOf("<EOF>") > -1)
                    //{
                    // All the data has been read from the 
                    // client. Display it on the console.
                    if (DeCODE == "1")
                    {
                        content = GetHexStringFrom(state.buffer);
                    }
                    //else
                    //{
                    //    var byteArray = bytesRead.TakeWhile((v, index) => Buffer.Skip(index).Any(w => w != 0x00)).ToArray();
                    //  content = Encoding.ASCII.GetString(byteArray, 0, byteArray.Length);

                    //}

                    Console.WriteLine("Revc " + DateTime.Now.ToString() + " " + handler.RemoteEndPoint + ": Read {0} bytes from socket. \n Data : {1}",
                            content.Length, content);
                    Utilities.WriteLog(content);
                    DbClass _db = new DbClass();
                    if (StringTFirstKey.Length > 0)
                    {

                        for (int i = 0; i < StringTFirstKey.Length; i++)
                        {
                            //Console.WriteLine("ST: "+StringTFirstKey[i] );
                            if (content.IndexOf(StringTFirstKey[i]) > -1 && StringTFirstKey[i].Replace(" ", "") != "")
                            {
                                _db.excuteMsgToDB(int.Parse(DeviceID), content);
                                break;
                            }
                        }

                    }
                    else
                    {

                        //if (msg.IndexOf(StringT) > -1 && StringT != "")
                        //{
                        _db.excuteMsgToDB(int.Parse(DeviceID), content);

                        //}
                    }
                    // Echo the data back to the client.
                    Send(handler, 0x9001);
                    //}
                    //else
                    //{
                    //    // Not all data received. Get more.
                    //    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    //    new AsyncCallback(ReadCallback), state);
                    //}
                }
            }
            catch (Exception)
            {
                //handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                //new AsyncCallback(ReadCallback), state);
            }

        }
        public static string GetHexStringFrom(byte[] data)
        {
            byte[] byteArray = data.TakeWhile((v, index) => data.Skip(index).Any(w => w != 0x00)).ToArray();
            return BitConverter.ToString(byteArray).Replace("-", string.Empty); //To convert the whole array
        }

        private static void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.


            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }
        private static void Send(Socket handler, int data)
        {
            // Convert the string data to byte data using ASCII encoding.


            byte[] byteData = Encoding.ASCII.GetBytes(data.ToString());

            // Begin sending the data to the remote device.
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static int Main(String[] args)
        {
            //string StringT = ConfigurationManager.AppSettings["StringT"].ToString();
            //string[] StringTFirstKey = StringT.Split(',');
            //for(int i=0;i<StringTFirstKey.Length;i++)
            //{
            //    Console.WriteLine(StringTFirstKey[i]);
            //}
            //Console.ReadKey();
           StartListening();
            return 0;
        }
    }

}
