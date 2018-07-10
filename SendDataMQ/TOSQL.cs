using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendDataMQ
{
    class TOSQL
    {
        public static string EmptyNull(object Obj)
        {
            try
            {
                return Obj.ToString().Trim();
            }
            catch
            { }
            return "";
        }

        public static int SQLToInt(object Obj)
        {
            int i = 0;
            try
            {
                i = Convert.ToInt32(Obj);
            }
            catch
            { }
            return i;
        }
        public static float SQLToFloat(string Obj)
        {
            float i = 0;
            try
            {
                i = float.Parse(Obj);
            }
            catch
            { }
            return i;
        }
        public static decimal SQLToDecimal(object Obj)
        {
            decimal i = 0;
            try
            {
                i = Convert.ToDecimal(Obj);
            }
            catch
            { }
            return i;
        }
        public static double SQLToDouble(object Obj)
        {
            double i = 0;
            try
            {
                i = Convert.ToDouble(Obj);
            }
            catch
            { }
            return i;
        }
        public static DateTime SQLToDateTime(object Obj)
        {
            try
            {
                return Convert.ToDateTime(Obj);
            }
            catch (Exception x)
            {

            }
            return DateTime.MinValue;
        }
        public static DateTime SQLToDateRic(object Obj)
        {
            try
            {
                return Convert.ToDateTime(Obj);
            }
            catch (Exception x)
            {
                return DateTime.Now;
            }

        }
        public static bool SQLBoolean(object Obj)
        {
            try
            {
                return Convert.ToBoolean(Obj);
            }
            catch (Exception x)
            {
                return false;
            }
        }
        public static bool IsNumberic(object Obj)
        {
            bool b = false;
            try
            {
                double d = Convert.ToDouble(Obj);
                b = true;
            }
            catch
            { }
            return b;
        }

        public static bool IsDate(object Obj)
        {
            bool b = false;
            try
            {
                DateTime d = Convert.ToDateTime(Obj);
                b = true;
            }
            catch
            { }
            return b;
        }

        public static string SQLString(object Obj)
        {
            string s = EmptyNull(Obj);
            if (s == "") return " NULL ";
            return " '" + s.Replace("'", "''") + "' ";
        }

        public static string SQLUString(object Obj)
        {
            string s = EmptyNull(Obj);
            if (s == "") return " NULL ";
            return " N'" + s.Replace("'", "''") + "' ";
        }

        public static string SQLLikeString(object Obj)
        {
            string s = EmptyNull(Obj);
            if (s == "") return " NULL ";
            return " N'%" + s.Replace("'", "''") + "%' ";
        }

        public static string SQLNumeric(object Obj)
        {
            if (IsNumberic(Obj))
            {
                return EmptyNull(Obj).Replace(",", "");
            }
            return " NULL ";
        }

        public static string SQLBit(object Obj)
        {
            string s = EmptyNull(Obj).ToUpper();
            if (s == "TRUE" || s == "1" || s == "YES") return " 1 ";
            return " 0 ";
        }

        public static string SQLBitNull(object Obj)
        {
            try
            {
                string s = EmptyNull(Obj.ToString().ToUpper());
                if (s == "TRUE" || s == "1" || s == "YES")
                    return " 1 ";
            }
            catch { }
            return " 0 ";
        }

        public static string SQLDate(DateTime? Obj)
        {
            if (IsDate(Obj))
            {
                DateTime obj = Convert.ToDateTime(Obj);
                string s = " '" + obj.Year.ToString() + "/" + obj.Month.ToString() + "/" + obj.Day.ToString() + " ";
                s += obj.Hour.ToString() + ":" + obj.Minute.ToString() + ":" + obj.Second.ToString() + "' ";

                return s;
            }
            return " NULL ";
        }

        public static string SQLDateNonHour(DateTime Obj)
        {
            if (IsDate(Obj))
            {
                Obj = Convert.ToDateTime(Obj);
                return " '" + Obj.Year.ToString() + "/" + Obj.Month.ToString() + "/" + Obj.Day.ToString() + "' ";
            }
            return " NULL ";
        }
    }
}
