using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendDataMQ
{
    public class Commond
    {
        #region DataView
        public static object GetFieldValue(DataRowView row, string FieldName)
        {
            return (row.Row.Table.Columns.Contains(FieldName) ? row[FieldName] : null);
        }
        public static string GetStringFieldValue(DataRowView row, string FieldName)
        {
            return (row.Row.Table.Columns.Contains(FieldName) && row[FieldName] != DBNull.Value ? Convert.ToString(row[FieldName]) : "");
        }
        public static int GetIntFieldValue(DataRowView row, string FieldName)
        {
            return (row.Row.Table.Columns.Contains(FieldName) && row[FieldName] != DBNull.Value ? Convert.ToInt32(row[FieldName]) : 0);
        }
        public static float GetFloatFieldValue(DataRowView row, string FieldName)
        {
            return (row.Row.Table.Columns.Contains(FieldName) && row[FieldName] != DBNull.Value ? Convert.ToSingle(row[FieldName]) : 0);
        }
        public static double GetDoubleFieldValue(DataRowView row, string FieldName)
        {
            return (row.Row.Table.Columns.Contains(FieldName) && row[FieldName] != DBNull.Value ? Convert.ToDouble(row[FieldName]) : 0);
        }
        public static bool GetBooleanFieldValue(DataRowView row, string FieldName)
        {
            return (row.Row.Table.Columns.Contains(FieldName) && row[FieldName] != DBNull.Value ? Convert.ToBoolean(row[FieldName]) : false);
        }
        public static DateTime GetDateTimeFieldValue(DataRowView row, string FieldName)
        {
            return (row.Row.Table.Columns.Contains(FieldName) && row[FieldName] != DBNull.Value ? Convert.ToDateTime(row[FieldName]) : DateTime.MinValue);
        }

        public static object GetFieldValue(DataRow row, string FieldName)
        {
            return (row.Table.Columns.Contains(FieldName) ? row[FieldName] : null);
        }
        public static string GetStringFieldValue(DataRow row, string FieldName)
        {
            return (row.Table.Columns.Contains(FieldName) && row[FieldName] != DBNull.Value ? Convert.ToString(row[FieldName]) : "");
        }
        public static int GetIntFieldValue(DataRow row, string FieldName)
        {
            return (row.Table.Columns.Contains(FieldName) && row[FieldName] != DBNull.Value ? Convert.ToInt32(row[FieldName]) : 0);
        }
        public static bool GetBooleanFieldValue(DataRow row, string FieldName)
        {
            return (row.Table.Columns.Contains(FieldName) && row[FieldName] != DBNull.Value ? Convert.ToBoolean(row[FieldName]) : false);
        }
        public static DateTime GetDateTimeFieldValue(DataRow row, string FieldName)
        {
            return (row.Table.Columns.Contains(FieldName) && row[FieldName] != DBNull.Value ? Convert.ToDateTime(row[FieldName]) : DateTime.MinValue);
        }
        #endregion
    }
}
