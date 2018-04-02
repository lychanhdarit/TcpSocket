using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerData
{
    public class ConvertType
    {
        public static string BitConvert(byte[] data)
        {
            string result = "";
            for (int i = 0; i < data.Length; i++)
            {
                result += Convert.ToString(data[i], 2);
            }
            return result;
        }
        public static string BitConvertRemoveNullObject(byte[] data)
        {
            string result = "";

            var byteArray = data.TakeWhile((v, index) => data.Skip(index).Any(w => w != 0x00)).ToArray();

            for (int i = 0; i < byteArray.Length; i++)
            {
                result += Convert.ToString(byteArray[i], 2);
            }
            return result;
        }
        public static string GetHexStringFrom(byte[] data)
        {
            return BitConverter.ToString(data).Replace("-", string.Empty); //To convert the whole array
        }
        public static string GetHexStringFromRemoveNullObject(byte[] data)
        {
            var byteArray = data.TakeWhile((v, index) => data.Skip(index).Any(w => w != 0x00)).ToArray();
            return BitConverter.ToString(byteArray).Replace("-", string.Empty); //To convert the whole array
        }
        public static string BytesToStringRemoveNullObject(byte[] data)
        {
            var Buffer = data.TakeWhile((v, index) => data.Skip(index).Any(w => w != 0x00)).ToArray();

            return (Encoding.Default.GetString(Buffer, 0, Buffer.Length - 1)).Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)[0];
        }
        static string BytesToStringConverted(byte[] bytes)
        {
           return  Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }
    }
}
