using OtpNet;

namespace Authi.App.Logic.Extensions
{
    public static class StringConverter
    {
        public static string ToBase32String(this byte[] value)
        {
            return Base32Encoding.ToString(value);
        }

        public static byte[] ToBase32Bytes(this string value)
        {
            return Base32Encoding.ToBytes(value);
        }
    }
}
