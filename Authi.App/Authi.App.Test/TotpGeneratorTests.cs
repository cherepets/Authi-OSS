using Authi.App.Logic.Services;
using Authi.Common.Services;
using Authi.Common.Test.Mocks;
using System;

namespace Authi.App.Test
{
    [TestClass]
    public class TotpGeneratorTests : AppTestsBase
    {
        [TestMethod]
        public void TotpGenerationHappyTest()
        {
            const string code = "JBSWY3DPEHPK3PXP";
            const string expected = "965095";

            ServicesMock
                .Override<IClock>(new MockClock
                {
                    UniversalTime = new DateTimeOffset(1993, 10, 23, 0, 0, 0, TimeSpan.Zero)
                });

            var totpGenerator = new TotpGenerator();

            var success = totpGenerator.TryCalculateTotp(code, out var totp);

            Assert.IsTrue(success);
            Assert.AreEqual(expected, totp);
        }

        [TestMethod]
        public void TotpGenerationInvalidCodeTest()
        {
            const string code = "@";

            ServicesMock
                .Override<IClock>(new MockClock
                {
                    UniversalTime = new DateTimeOffset(1993, 10, 23, 0, 0, 0, TimeSpan.Zero)
                });

            var totpGenerator = new TotpGenerator();

            var success = totpGenerator.TryCalculateTotp(code, out var totp);

            Assert.IsFalse(success);
            Assert.IsNull(totp);
        }
    }
}