using System;
using System.Net;
using System.Web;

namespace Authi.Common.Client
{
    public class OtpauthUri
    {
        public string Issuer { get; }
        public string Secret { get; }

        public OtpauthUri(string uriString)
        {
            var uri = new Uri(uriString);
            var queryParams = HttpUtility.ParseQueryString(uri.Query);

            var local = uri.LocalPath.Trim('/', ':');

            Issuer = queryParams["issuer"]?.ToString() ?? local ?? string.Empty;
            Secret = queryParams["secret"]?.ToString() ?? string.Empty;
        }

        public OtpauthUri(string issuer, string secret)
        {
            Issuer = issuer;
            Secret = secret;
        }

        public override string ToString()
        {
            var issuer = WebUtility.UrlEncode(Issuer);
            var secret = WebUtility.UrlEncode(Secret);
            return $"otpauth://totp/{issuer}:?secret={secret}&issuer={issuer}";
        }
    }
}
