using System;
using System.Text;

namespace Authi.Common.Extensions
{
    public static class StringConverter
    {
        public static string ToUtfString(this byte[] value)
        {
            return Encoding.UTF8.GetString(value);
        }

        public static byte[] ToUtfBytes(this string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }

        public static string ToBase64String(this byte[] value)
        {
            return Convert.ToBase64String(value);
        }

        public static byte[] ToBase64Bytes(this string value)
        {
            return Convert.FromBase64String(value);
        }
    }
}
