using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ufms;
using ProtoBuf;

namespace GSNReceiver.GTVT_N
{
    class Common
    {

        /// <summary>
        /// get file name log
        /// </summary>
        /// <param name="str"></param>
        public static string getFileName(string path)
        {

            DateTime dateLog = DateTime.Now;
            return (path + dateLog.Day + dateLog.Month + dateLog.Year + ".txt").ToString();
           
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">duong dan cua file</param>
        /// <param name="str">chuoi tra ve cua thiet bi giam sat hinh trinh</param>
        public static bool writeLog(string path, string str)
        {

            TextWriter tw = new StreamWriter(path, true);
            tw.WriteLine(str);
            tw.Close();
            return true;
        }
        
        /// <summary>
        /// doc du lieu tu mot file txt tra ve la mot mang du lieu
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string[] readFile(string path)
        {
            string[] lines = null; 
            StreamReader sr = new StreamReader(path, true);
            lines=sr.ReadToEnd().Split('#');
            sr.Close();
            return lines;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="anwers"></param>
        public static void Continute_Exit(string anwers)
        {
            if (anwers.ToLower().Equals("y"))
            {
                Console.Clear();
                GSNReceiver.AsynchronousSocketListener.StartListening();
            }
            else if (anwers.ToLower().Equals("n")) { }

        }
        /// <summary>
        /// 
        /// </summary>
        public static void question()
        {
            Console.Write("Tro ve man hinh chinh y/n :");
            string read = Console.ReadLine().Split(':')[0].ToString();
            Continute_Exit(read);
        }
        /// <summary>
        /// convert datetime to int
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static int DateTimeToUnixTimestamp(DateTime dateTime)
        {
            TimeSpan span = (dateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
            double unixTime = span.TotalSeconds;
            return (int)unixTime; // 1371802570000

        }

        public static byte[] Serialize(BaseMessage wp)
        {
            byte[] b = null;
            using (var ms = new MemoryStream())
            {
                Serializer.Serialize<BaseMessage>(ms, wp);
                b = new byte[ms.Position];
                var fullB = ms.GetBuffer();
                Array.Copy(fullB, b, b.Length);
            }
            return b;
        }
        /// <summary>
        /// convert hex to bit
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static string convertHextoBit(string hex)
        {
            return Convert.ToString(Convert.ToInt32(hex, 16), 2).PadLeft(hex.Length * 2, '0');
        }
        /// <summary>
        /// convert to double
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double convertToDouble(string value)
        {
            double returnValue = 0;
            if (value != "")
            {

                value = value.Substring(0, value.Length - 1);
                int startLatitudeSecond = value.IndexOf(".") + 1;
                var strLatitudeSecond = value.Substring(startLatitudeSecond, value.Length - startLatitudeSecond);
                value = value.Substring(0, value.IndexOf("."));
                var strLatMinute = value.Substring(value.Length - 2, 2);
                var strLatHour = value.Substring(0, value.Length - 2);
                returnValue = Convert.ToDouble(strLatHour) + Convert.ToDouble(strLatMinute) / 60 + Convert.ToDouble(strLatitudeSecond) * 60 / 36000000;
            }

            return returnValue;
        }
    }
}
