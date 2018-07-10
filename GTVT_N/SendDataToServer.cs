using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Configuration;

namespace GSNReceiver.GTVT_N
{
    class SendDataToServer
    {
        static string ipAddress = ConfigurationManager.AppSettings["ipServerRecieve"];
        static int port = int.Parse(ConfigurationManager.AppSettings["portServerRecieve"]);
        static Socket server; 

        public static void _SendDataToServer(string str,Socket server)
        {
                try
                {
                        server.Send(Encoding.ASCII.GetBytes(str));
                }
                catch (Exception ex)
                {
                    string pathLog = ConfigurationManager.AppSettings["LogError"];
                    string fileName = Common.getFileName(pathLog + "Log_Error_");
                    string text = "Date:" + DateTime.Now + ";SendDataToServer._SendDataToServer();Error : " + ex.Message.ToString();
                    Common.writeLog(fileName, text);
                    if (!server.Connected) _ConnectServer();
                } 

        }
        /// <summary>
        /// ham su dung khi connect den mot host nhan du lieu ma bi ngat ket noi ,phai ket noi lai
        /// </summary>
        public static void _ConnectServer()
        {
            try
            {
                IPEndPoint ipEndpoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server.Connect(ipEndpoint);
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                string pathLog = ConfigurationManager.AppSettings["LogError"];
                string fileName = Common.getFileName(pathLog + "Log_Error_");
                string text = "Date:" + DateTime.Now + ";SendDataToServer._ConnectServer();Error : " + ex.Message.ToString();
                Common.writeLog(fileName, text);
            }
        }
    }
}
