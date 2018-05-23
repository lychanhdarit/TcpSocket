using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerData
{
    public class Utilities
    {
        public static void WriteLogError(Exception ex)
        {
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "LogFile.txt", true);
                sw.WriteLine(DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss tt")  + ": " + ex.Source + "; " + ex.Message);
                sw.Flush();
                sw.Close();

            }
            catch
            {
                // ignored
            }
        }
        public static void WriteLogError(string message)
        {
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "LogFile.txt", true);
                sw.WriteLine(DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss tt")  + ": " + message);
                sw.Flush();
                sw.Close();
            }
            catch
            {
                // ignored
            }
        }

        public static void WriteLog(string message)
        {
            StreamWriter sw = null;
            try
            {
                string path = ConfigurationManager.AppSettings["PathSave"].ToString();
                if (path == "")
                {
                    path = AppDomain.CurrentDomain.BaseDirectory;
                }

                string filename = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
                sw = new StreamWriter(path + "Log-"+ filename + ".txt", true);
                sw.WriteLine(DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss tt") + ": " + message);
                sw.Flush();
                sw.Close();
            }
            catch
            {
                PrintWithColorRed("Error: Invalid path!");
                
            }

        }
        public static void WriteLogSocketError(string message)
        {
            StreamWriter sw = null;
            try
            {
                string path = ConfigurationManager.AppSettings["PathSave"].ToString();
                if (path == "")
                {
                    path = AppDomain.CurrentDomain.BaseDirectory;
                }

                string filename = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + "-SOCKET";
                sw = new StreamWriter(path + "Log-" + filename + ".txt", true);
                sw.WriteLine(DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss tt") + ": " + message);
                sw.Flush();
                sw.Close();
            }
            catch
            {
                PrintWithColorRed("Error: Invalid path!");

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
        public static void WriteLogDatabase(string message)
        {
            StreamWriter sw = null;
            try
            {
                string path = ConfigurationManager.AppSettings["PathSave"].ToString();
                if(path == "")
                {
                    path = AppDomain.CurrentDomain.BaseDirectory;
                }
                string filename = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString()+"-DATABASE";
                sw = new StreamWriter( path + "Log-" + filename + "-lost.txt", true);
                sw.WriteLine(DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss tt")  + ": " + message);
                sw.Flush();
                sw.Close();
            }
            catch
            {
                PrintWithColorRed("Error: Invalid path!");
            }
        }
    }
}
