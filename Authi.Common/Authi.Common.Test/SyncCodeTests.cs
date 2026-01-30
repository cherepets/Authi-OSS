using Authi.Common.Client;
using Authi.Common.Services;
using System;

namespace Authi.Common.Test
{
    [TestClass]
    public class SyncCodeTests
    {
        [TestMethod]
        public void SyncCodeHappyTest()
        {
            var crypto = new Crypto();

            var syncCode = new SyncCode
            {
                SyncId = Guid.NewGuid(),
                DataKey = crypto.GenerateAesKey(),
                OneTimeKey = crypto.GenerateAesKey()
            };
            Assert.AreEqual(36, syncCode.SyncId.ToString().Length);
            Assert.AreEqual(44, syncCode.DataKey.ToString().Length);
            Assert.AreEqual(44, syncCode.OneTimeKey.ToString().Length);

            var serialized = syncCode.Serialize();
            Assert.HasCount(80, serialized);

            var deserialized = SyncCode.Deserialize(serialized);
            Assert.AreEqual(syncCode.SyncId.ToString(), deserialized.SyncId.ToString());
            Assert.AreEqual(syncCode.DataKey.ToString(), deserialized.DataKey.ToString());
            Assert.AreEqual(syncCode.OneTimeKey.ToString(), deserialized.OneTimeKey.ToString());
        }
    }
}
