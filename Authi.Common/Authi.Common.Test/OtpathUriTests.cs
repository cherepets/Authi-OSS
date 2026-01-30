using Authi.Common.Client;

namespace Authi.Common.Test
{
    [TestClass]
    public class OtpathUriTests
    {
        [TestMethod]
        public void OtpauthUriFromStringTest()
        {
            var uri = new OtpauthUri("otpauth://totp/issuer:?secret=secret");
            Assert.AreEqual("issuer", uri.Issuer);
            Assert.AreEqual("secret", uri.Secret);
        }

        [TestMethod]
        public void OtpauthUriWithIssuerFromStringTest()
        {
            var uri = new OtpauthUri("otpauth://totp/skip:?secret=secret&issuer=issuer");
            Assert.AreEqual("issuer", uri.Issuer);
            Assert.AreEqual("secret", uri.Secret);
        }

        [TestMethod]
        public void OtpauthUriFromIssuerAndSecretTest()
        {
            var uri = new OtpauthUri("issuer", "secret");
            Assert.AreEqual("otpauth://totp/issuer:?secret=secret&issuer=issuer", uri.ToString());
        }
    }
}
