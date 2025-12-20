using System;
using System.Text;
using System.Text.RegularExpressions;

namespace AgentsDataView.Common.Utilities
{
    public static class StringExtensions
    {
        public static bool HasValue(this string? value, bool ignoreWhiteSpace = true)
        {
            return ignoreWhiteSpace ? !string.IsNullOrWhiteSpace(value) : !string.IsNullOrEmpty(value);
        }

        public static int ToInt(this string? value)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return 0;
                }
                return Convert.ToInt32(value);
            }
            catch
            {
                return int.Parse(Regex.Replace(value, "[^0-9]", ""));
            }

        }

        public static decimal ToDecimal(this string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return 0;
            }
            return Convert.ToDecimal(value);
        }

        public static string ToNumeric(this int value)
        {
            return value.ToString("N0"); //"123,456"
        }

        public static string ToNumeric(this decimal value)
        {
            return value.ToString("N0");
        }

        public static string ToCurrency(this int value)
        {
            //fa-IR => current culture currency symbol => ریال
            //123456 => "123,123ریال"
            return value.ToString("C0");
        }

        public static string ToCurrency(this decimal value)
        {
            return value.ToString("C0");
        }

        public static string En2Fa(this string str)
        {
            return str.Replace("0", "۰")
                .Replace("1", "۱")
                .Replace("2", "۲")
                .Replace("3", "۳")
                .Replace("4", "۴")
                .Replace("5", "۵")
                .Replace("6", "۶")
                .Replace("7", "۷")
                .Replace("8", "۸")
                .Replace("9", "۹");
        }

        public static string Fa2En(this string str)
        {
            StringBuilder sb = new(str);
            return sb.Replace("۰", "0")
                .Replace("۱", "1")
                .Replace("۲", "2")
                .Replace("۳", "3")
                .Replace("۴", "4")
                .Replace("۵", "5")
                .Replace("۶", "6")
                .Replace("۷", "7")
                .Replace("۸", "8")
                .Replace("۹", "9")
                //iphone numeric
                .Replace("٠", "0")
                .Replace("١", "1")
                .Replace("٢", "2")
                .Replace("٣", "3")
                .Replace("٤", "4")
                .Replace("٥", "5")
                .Replace("٦", "6")
                .Replace("٧", "7")
                .Replace("٨", "8")
                .Replace("٩", "9").ToString();
        }

        public static string FixPersianChars(this string str)
        {
            StringBuilder sb = new(str);
            return sb.Replace("ﮎ", "ک")
                .Replace("ﮏ", "ک")
                .Replace("ﮐ", "ک")
                .Replace("ﮑ", "ک")
                .Replace("ك", "ک")
                .Replace("ي", "ی")
                .Replace(" ", " ")
                .Replace("‌", " ")
                .Replace("ھ", "ه").ToString();//.Replace("ئ", "ی");
        }

        public static string? CleanString(this string str)
        {
            return str.Trim().FixPersianChars().Fa2En().NullIfEmpty();
        }
        public static string CleanString2(this string str)
        {
            return str.Trim().FixPersianChars().Fa2En();
        }
        public static void CleanStrings(this string[] strings)
        {
            for (int i = 0; i < strings.Length; i++)
            {
                strings[i] = strings[i].Trim().FixPersianChars().Fa2En();
            }
        }

        public static string? NullIfEmpty(this string? str)
        {
            return str?.Length == 0 ? null : str;
        }

        public static bool IsValidMobileNumber(this string str, bool checkEmpty = false)
        {

            if (checkEmpty && !str.HasValue())
            {
                return false;
            }
            else if (str == null || str == string.Empty)
            {
                return true;
            }
            Regex regex = new Regex(@"^(?:0|98|\+98|\+980|0098|098|00980)?(9\d{9})$");
            return regex.IsMatch(str);
        }
        public static bool IsValidNumber(this string str, bool checkIfEmpty = false)
        {
            if (!checkIfEmpty && !str.HasValue())
            {
                return true;
            }
            return Regex.IsMatch(str, @"^\d+$");
        }
        public static bool StringsEqual(this string? str1, string? str2)
        {
            if (string.IsNullOrWhiteSpace(str1) && string.IsNullOrWhiteSpace(str2))
            {
                return true;
            }
            if ((!string.IsNullOrWhiteSpace(str1) && string.IsNullOrWhiteSpace(str2)) || (string.IsNullOrWhiteSpace(str1) && !string.IsNullOrWhiteSpace(str2)))
            {
                return false;
            }
            str1 = str1.CleanString();
            str2 = str2.CleanString();
            return str1.Equals(str2, StringComparison.OrdinalIgnoreCase);
        }
    }
}
