using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Serialization;

namespace PDM.Helper
{
    public static class Converters
    {
        public static string ConvertDateFormat(string date)
            => date.Replace("/", ".").Replace("-", ".");

        public static DateTime ConvertToDateTimeFromJS(string jsDate)
            => DateTime.ParseExact(jsDate.Substring(0, 24),
                              "ddd MMM d yyyy HH:mm:ss",
                              CultureInfo.InvariantCulture);

        public static string StripTags(string html)
        {
            var regEx = new Regex("<[^>]*>'");
            var text = regEx.Replace(html, string.Empty);
            text = text.Replace("&nbsp;", string.Empty);
            text = text.Replace("\n", string.Empty);
            text = text.Replace("\r", string.Empty);
            return text;
        }

        /// <summary>
        /// Used to check Whether the given date is valid date or not
        /// This pattern of data will support formats of date as follows(dd/MM/yyyy,dd.MM.yyyy,dd-MM-yyyy,MM/dd/yyyy,MM.dd.yyyy,MM-dd-yyyy,d/M/yyyy,d.M.yyyy)
        /// (d-M-yyyy,dd/MM/yyyy HH:MM:SS,dd-MM-yyyy HH:MM:SS,dd.MM.yyyy HH:MM:SS,MM.dd.yyyy HH:MM:SS,MM/dd/yyyy HH:MM:SS,MM-dd-yyyy HH:MM:SS,d/M/yyyy HH:MM:SS)
        /// (M/d/yyyy HH:MM:SS,M-d-yyyy HH:MM:SS,M.d.yyyy HH:MM:SS,dd/MM/yyyy HH:MM:SS AM (or) PM,dd-MM-yyyy HH:MM:SS AM (or) PM,dd.MM.yyyy HH:MM:SS AM (or) PM)
        /// (dd/MM/yyyy HH:MM:SS AM (or) PM,M-d-yyyy HH:MM:SS AM (or) PM,M/d/yyyy HH:MM:SS AM (or) PM,M.d.yyyy HH:MM:SS AM (or) PM)
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool IsValidDate(string date)
            => new Regex(@"^([1-9]|0[1-9]|[12][0-9]|3[01])[- /.]([1-9]|0[1-9]|[12][0-9]|3[01])[- /.]([12][0-9])*\d\d( (\d|\d\d):(\d\d)(:(\d\d))?)?( am|pm)?$", RegexOptions.IgnoreCase).IsMatch(date);

        public static string ConvertCaseString(string valToConvert, CaseNotation casesToConvert = CaseNotation.PascalCase, char cSpliter = '_')
        {
            var splittedPhrase = valToConvert.Trim().ToLower().Split(cSpliter);
            var sb = new StringBuilder();

            if (casesToConvert == CaseNotation.CamelCase)
            {
                sb.Append(splittedPhrase[0].ToLower(CultureInfo.InvariantCulture));
                splittedPhrase[0] = string.Empty;
            }

            foreach (string value in splittedPhrase)
            {
                var splittedPhraseChars = value.ToCharArray();
                if (splittedPhraseChars.Length > 0)
                    splittedPhraseChars[0] = ((new string(splittedPhraseChars[0], 1)).ToUpper(CultureInfo.InvariantCulture).ToCharArray())[0];

                sb.Append(new string(splittedPhraseChars));
            }
            return sb.ToString();
        }

        public static string RevertCaseString(string valueToRevert, CaseNotation casesToConvert = CaseNotation.PascalCase, string cSpliter = "_")
        {
            var sb = new StringBuilder();
            var i = 0;
            if (casesToConvert == CaseNotation.CamelCase)
            {
                i = 1;
                sb.Append(valueToRevert.Substring(1, 1).ToUpper(CultureInfo.InvariantCulture));
                valueToRevert = valueToRevert.Remove(1, 1);
            }
            else if (casesToConvert == CaseNotation.PascalCase)
                sb = new StringBuilder();

            foreach (char value in valueToRevert)
            {
                if (char.IsUpper(value) && i == 1)
                    sb.Append(cSpliter + value);
                else
                {
                    sb.Append(value);
                    i = 1;
                }
            }
            return sb.ToString();
        }

       
        public static bool CheckIsRtf(string check)
            => check.ToLowerInvariant().Contains("rtf") && check.ToLowerInvariant().Contains("ansi");

        public static bool CheckIsHtml(string check)
            => check != HttpUtility.HtmlEncode(check);

       
        //private static string RtfToPtf(string rtf)
        //{
        //    using (var rtfTemp = new RichTextBox { Rtf = rtf })
        //        return StripTags(rtfTemp.Text);
        //}

        /// <summary>
        /// Determines if a supplied string contains a valid numeric value.  Allows for integer and decimal values and negativity.
        /// </summary>
        /// <param name="testNumber">String to be checked</param>
        /// <returns>True if supplied string is a valid numeric value, otherwise false.</returns>
        public static bool CheckIsNumeric(object testNumber)
        {
            try
            {
                testNumber = double.Parse(testNumber.ToString());
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Determines if a supplied string contains a int.
        /// </summary>
        /// <param name="testInteger">String to be checked</param>
        /// <returns>True if supplied string is a valid long, otherwise false.</returns>
        public static bool CheckIsInt(object testInteger)
        {
            try
            {
                int result;
                return int.TryParse(testInteger.ToString(), out result);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool CheckIsEmptyString(string input)
        => string.IsNullOrWhiteSpace(input);

        public static string ZeroPadString(string input, int Length)
        {
            string returnVal;

            //if the original string already exceeds the length desired, return unchanged
            if (input.Length > Length)
            {
                returnVal = input;
            }
            else
            {
                returnVal = input;

                while (returnVal.Length < Length)
                {
                    returnVal = "0" + returnVal;
                }
            }

            return returnVal;
        }

        public static string ZeroPadString(int input, int Length)
            => ZeroPadString(input.ToString(), Length);

        /// <summary>
        /// Returns the file name portion from a file path or returns
        ///  the same string if it is not a full path
        /// </summary>
        /// <param name="path">The path that contains the file name to be returned</param>
        /// <returns>The file name portion or the entire string if it is not a path</returns>
        public static string GetFileNameFromPath(string path)
        {
            var pos = path.LastIndexOf("/", StringComparison.OrdinalIgnoreCase);
            return (pos > -1)
                && (pos + 1 < path.Length) ? path.Substring(pos + 1) : path;
        }

        /// <summary>
        /// Try to convert an object to long. If it's not valid set it to a default value
        /// </summary>
        /// <param name="input">The object to be converted</param>
        /// <param name="outputIfInvalid">Return value if the input is invalid</param>
        /// <returns>Either the converted value or the default output</returns>
        public static long ConvertLong(object input, long outputIfInvalid)
        {
            var output = outputIfInvalid;

            if (input != null && input != DBNull.Value)
            {
                try
                {
                    output = Convert.ToInt64(input);
                }
                catch (Exception)
                {
                    output = outputIfInvalid;
                }
            }

            return output;
        }

        /// <summary>
        /// Try to convert an object to long using the overloaded ConvertLong function with
        /// default return value 0
        /// </summary>
        /// <param name="input">Object to be converted</param>
        /// <returns></returns>
        public static long ConvertLong(object input)
            => ConvertLong(input, 0);

        /// <summary>
        /// Try to convert an object to string. If it's not valid set it to a default value
        /// </summary>
        /// <param name="input">The object to be converted</param>
        /// <param name="outputIfInvalid">Return value if the input is invalid</param>
        /// <returns>Either the converted value or the default output</returns>
        public static string ConvertString(object input, string outputIfInvalid)
        {
            var output = outputIfInvalid;

            if (input != null && input != DBNull.Value)
            {
                try
                {
                    output = Convert.ToString(input);
                }
                catch (Exception)
                {
                    output = outputIfInvalid;
                }
            }

            return output;
        }

        /// <summary>
        /// Try to convert an object to string using the overloaded ConvertLong function with
        /// default return value ""
        /// </summary>
        /// <param name="input">Object to be converted</param>
        /// <returns></returns>
        public static string ConvertString(object input)
            => ConvertString(input, string.Empty);

        public static DateTime ConvertDate(object input, DateTime outputIfInvalid)
        {
            var output = outputIfInvalid;
            if (input != null && input != DBNull.Value)
            {
                try
                {
                    output = Convert.ToDateTime(input);
                }
                catch (Exception)
                {
                    output = outputIfInvalid;
                }
            }

            return output;
        }

        private static DateTime DefaultDateTime = new DateTime(1, 1, 1);

        public static DateTime ConvertDate(object input)
            => ConvertDate(input, DefaultDateTime);

        /// <summary>
        /// Try to convert an object to int. If it's not valid set it to a default value
        /// </summary>
        /// <param name="input">The object to be converted</param>
        /// <param name="outputIfInvalid">Return value if the input is invalid</param>
        /// <returns>Either the converted value or the default output</returns>
        public static int ConvertInt(object input, int outputIfInvalid)
        {
            var output = outputIfInvalid;

            if (input != null && input != DBNull.Value)
            {
                try
                {
                    output = Convert.ToInt32(input);
                }
                catch (Exception)
                {
                    output = outputIfInvalid;
                }
            }

            return output;
        }

        /// <summary>
        /// Try to convert an object to int using the overloaded ConvertLong function with
        /// default return value 0
        /// </summary>
        /// <param name="input">Object to be converted</param>
        /// <returns></returns>
        public static int ConvertInt(object input)
            => ConvertInt(input, 0);

        /// <summary>
        /// Try to convert an object to int. If it's not valid set it to a default value
        /// </summary>
        /// <param name="input">The object to be converted</param>
        /// <param name="outputIfInvalid">Return value if the input is invalid</param>
        /// <param name="regionalSettings">culture</param>
        /// <returns>Either the converted value or the default output</returns>
        public static double ConvertDouble(object input, double outputIfInvalid, string regionalSettings)
        {
            var output = outputIfInvalid;

            if (input != null && input != DBNull.Value)
            {
                try
                {
                    var cultureInformation = string.IsNullOrWhiteSpace(regionalSettings) ? System.Threading.Thread.CurrentThread.CurrentCulture : new CultureInfo(regionalSettings);
                    var nFormat = cultureInformation.NumberFormat;
                    var nFormatOriginal = CultureInfo.CurrentCulture.NumberFormat;
                    output = nFormat.NumberDecimalSeparator.ToString() != nFormatOriginal.NumberDecimalSeparator.ToString() ? Convert.ToDouble(input, nFormatOriginal) : Convert.ToDouble(input);
                }
                catch (Exception)
                {
                    output = outputIfInvalid;
                }
            }

            return output;
        }

        /// <summary>
        /// Try to convert an object to int. If it's not valid set it to a default value
        /// </summary>
        /// <param name="input">The object to be converted</param>
        /// <param name="outputIfInvalid">Return value if the input is invalid</param>
        /// <param name="nFormat">Format</param>
        /// <returns>Either the converted value or the default output</returns>
        public static double ConvertDouble(object input, double outputIfInvalid, NumberFormatInfo nFormat)
        {
            var output = outputIfInvalid;

            if (input != null && input != DBNull.Value)
            {
                try
                {
                    output = Convert.ToDouble(input, nFormat);
                }
                catch (Exception)
                {
                    output = outputIfInvalid;
                }
            }

            return output;
        }

        /// <summary>
        /// Try to convert an object to Double using the overloaded ConvertLong function with
        /// default return value 0
        /// </summary>
        /// <param name="input">Object to be converted</param>
        /// <returns></returns>
        public static double ConvertDouble(object input)
            => ConvertDouble(input, 0, string.Empty);

        public static double ConvertDouble(string regionalSettings, object input) //1st param is reg. to easier find and replace in dim. app
            => ConvertDouble(input, 0, regionalSettings);

        public static double ConvertDouble(string regionalSettings, object input, double outputIfInvalid) //1st param is reg. to easier find and replace in dim. app
           => ConvertDouble(input, outputIfInvalid, regionalSettings);

        /// <summary>
        /// Try to convert an object to decimal. If it's not valid set it to a default value
        /// </summary>
        /// <param name="input">The object to be converted</param>
        /// <param name="outputIfInvalid">Return value if the input is invalid</param>
        /// <returns>Either the converted value or the default output</returns>
        public static decimal ConvertDecimal(object input, decimal outputIfInvalid)
        {
            var output = outputIfInvalid;

            if (input != null && input != DBNull.Value)
            {
                try
                {
                    output = Convert.ToDecimal(input);
                }
                catch (Exception)
                {
                    output = outputIfInvalid;
                }
            }

            return output;
        }

        /// <summary>
        /// Try to convert an object to decimal using the overloaded ConvertLong function with
        /// default return value 0
        /// </summary>
        /// <param name="input">Object to be converted</param>
        /// <returns></returns>
        public static decimal ConvertDecimal(object input) => ConvertDecimal(input, 0);

        /// <summary>
        /// Try to convert an object to bool since Convert.ToBoolean doesn't handle strings
        /// First try to convert it using builtin functions
        /// If it doesn't work then convert it to string and check against the list of true and false values
        /// </summary>
        /// <param name="input">The object to be converted</param>
        /// <param name="trueStrings">List of strings to be considered as true</param>
        /// <param name="falseStrings">List of strings to be considered as false</param>
        /// <param name="outputIfInvalid">Return value if the input is invalid</param>
        /// <returns>Either the converted value or the default output</returns>
        public static bool ConvertBool(object input, string[] trueStrings, string[] falseStrings, bool outputIfInvalid)
        {
            var output = outputIfInvalid;

            if (input != null && input != DBNull.Value)
            {
                try
                {
                    output = Convert.ToBoolean(input);
                }
                catch (Exception)
                {
                    //Convert raised error. Convert to string and check against list

                    var convertedString = ConvertString(input, string.Empty);
                    var isMatch = false;

                    //First check is input is in list of true values
                    foreach (string trueValue in trueStrings)
                    {
                        if (convertedString == trueValue)
                        {
                            output = true;
                            isMatch = true;
                        }
                    }

                    //If not found, then check against list of false values
                    if (!isMatch)
                    {
                        foreach (string falseValue in falseStrings)
                        {
                            if (convertedString == falseValue)
                            {
                                output = false;
                                isMatch = true;
                            }
                        }
                    }
                    //Finally, set output to invalid if still not found
                    if (!isMatch)
                    {
                        output = outputIfInvalid;
                    }
                }
            }

            return output;
        }

        public static bool ConvertBool(object input, string trueString, string falseString, bool outputIfInvalid)
            => ConvertBool(input, new string[] { trueString }, new string[] { falseString }, outputIfInvalid);

        public static bool ConvertBool(object input, string trueString, string falseString)
            => ConvertBool(input, new string[] { trueString }, new string[] { falseString }, false);

        /// <summary>
        /// Try to convert an object to int using the overloaded ConvertLong function with
        /// default return value false
        /// </summary>
        /// <param name="input">Object to be converted</param>
        /// <returns></returns>
        public static bool ConvertBool(object input)
        {
            string[] trueList = { "1", "Y", "y" };
            string[] falseList = { "0", "N", "n" };
            return ConvertBool(input, trueList, falseList, false);
        }

        /// <summary>
        /// Try to convert an object to string. If it's not valid set it to a default value
        /// </summary>
        /// <param name="input">The object to be converted</param>
        /// <param name="outputIfInvalid">Return value if the input is invalid</param>
        /// <returns>Either the converted value or the default output</returns>
        public static DateTime ConvertDateTime(object input, DateTime outputIfInvalid)
        {
            var output = outputIfInvalid;

            if (input != null && input != DBNull.Value)
            {
                try
                {
                    output = Convert.ToDateTime(input);
                }
                catch (Exception)
                {
                    output = outputIfInvalid;
                }
            }

            return output;
        }

        /// <summary>
        /// Try to convert an object to string using the overloaded ConvertLong function with
        /// default return value ""
        /// </summary>
        /// <param name="input">Object to be converted</param>
        /// <returns></returns>
        public static DateTime ConvertDateTime(object input)
            => ConvertDateTime(input, new DateTime(1, 1, 1));

        /// <summary>
        /// Converts a string from win1252 encoding to cyrillic encoding
        /// </summary>
        /// <param name="actual">string encoded in win1252</param>
        /// <returns>cyrillic encoded string (10007)</returns>
        public static string ConvertToRussian(string actual)
        {
            var actualBytes = Encoding.GetEncoding(1252).GetBytes(actual);
            return Encoding.GetEncoding(10007).GetString(actualBytes);
        }

        public static DateTime GetDateTime(DateTime date, string time)
        {
            if (string.IsNullOrWhiteSpace(time) || !time.Contains(':')) throw new ArgumentNullException(nameof(time), "Supply valid time in HH:mm:ss format");

            var timeParts = time.Split(':').Select(t => ConvertInt(t)).ToList();
            if (timeParts.Count == 1) timeParts.AddRange(new[] { 0, 0 });
            else if (timeParts.Count == 2) timeParts.AddRange(new[] { 0, });

            return new DateTime(date.Year, date.Month, date.Day, timeParts[0], timeParts[1], timeParts[2]);
        }

        public static string ToJsonText(this object val, bool indent)
            => JsonConvert.SerializeObject(val, indent ? Formatting.Indented : Formatting.None);

        public static string ToJsonText(this object val)
#if DEBUG
           => JsonConvert.SerializeObject(val, Formatting.Indented);

#else

           => JsonConvert.SerializeObject(val, Formatting.None);
#endif

        public static string ToXmlText(this object val)
        {
            var sbResult = new StringBuilder();
            var xmlSerializer = new XmlSerializer(val.GetType());
            var textWriter = new StringWriter(sbResult);
            xmlSerializer.Serialize(textWriter, val);
            return sbResult.ToString();
        }

        public static T ToObject<T>(this string json)
            => JsonConvert.DeserializeObject<T>(json);

        public static T ToObjectFromXml<T>(this string xml)
        {
            var ser = new XmlSerializer(typeof(T));
            using (var stringReader = new StringReader(xml))
                return (T)ser.Deserialize(stringReader);
        }

        public static IList<T> ConvertDataTableToList<T>(DataTable dt)
        {
            IList<T> data = new List<T>();
            if (dt != null && dt.Rows?.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    T item = GetItem<T>(row);
                    data.Add(item);
                }
            }
            return data;
        }
        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (var pro in temp.GetProperties())
                {
                    if (pro.Name == ConvertCaseString(column.ColumnName))
                    {
                        if (dr[column.ColumnName] != null && dr[column.ColumnName] != DBNull.Value)
                            pro.SetValue(obj, dr[column.ColumnName], null);
                        break;
                    }
                }
            }
            return obj;
        }
    }

    public enum CaseNotation
    {
        PascalCase,
        CamelCase
    }
}