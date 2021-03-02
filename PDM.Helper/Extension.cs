using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PDM.Helper
{
    public static class Extension
    {
        static Extension()
        {

        }
        private static string[] TrueValues = { "true", "y", "yes", "1" };

        public static List<T> ToList<T>(this DataTable dt) where T : class, new()
        {
            if (dt == null || dt.Rows.Count == 0)
                return new List<T>();
            else
                return AsEnumerable<T>(dt).ToList();
        }
        public static IEnumerable<T> AsEnumerable<T>(DataTable dt) where T : class, new()
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                var properties = typeof(T).GetProperties();
                var fieldName = string.Empty;
                object value = null;
                return dt.Rows.Cast<DataRow>().
                        Select(dr =>
                        {
                            var item = new T();

                            foreach (PropertyInfo pi in properties)
                            {
                                if (pi.CanWrite)
                                {
                                    try
                                    {
                                        fieldName = pi.Name.Trim().ToLower();
                                        if (dt.Columns.Cast<DataColumn>().Any(col => col.ColumnName.Trim().ToLower() == fieldName))
                                        {
                                            value = dr[fieldName];
                                            if (value != DBNull.Value && value != null)
                                            {
                                                object setValue = null;
                                                try
                                                {
                                                    if (pi.PropertyType.IsGenericType)
                                                        setValue = Convert.ChangeType(value, pi.PropertyType.GetGenericArguments()[0]);
                                                    else
                                                        setValue = Convert.ChangeType(value, pi.PropertyType);

                                                }
                                                catch
                                                {
                                                    if (pi.PropertyType == typeof(bool) || (pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericArguments()[0] == typeof(bool)))
                                                        setValue = TrueValues.Contains(value.ToString().ToLower().Trim());
                                                    else if (pi.PropertyType == typeof(char) || (pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericArguments()[0] == typeof(char)))
                                                        setValue = Convert.ToString(value).Length > 0 ? Convert.ToString(value)[0] : char.MinValue;
                                                }

                                                pi.SetValue(item, setValue, null);
                                            }
                                        }

                                    }
                                    catch
                                    {

                                    }
                                }


                            }
                            return item;

                        }).Where(t => t != null);
            }
            return null;
        }
    }
}
