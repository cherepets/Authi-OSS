using Authi.Common.Extensions;
using Authi.Common.Services;
using Org.BouncyCastle.Crypto;

namespace Authi.Common.Test
{
    [TestClass]
    public class CryptoTests : TestsBase
    {
        [TestMethod]
        public void AesCanDecryptWithCorrectKeyTest()
        {
            var crypto = new Crypto();

            var key = crypto.GenerateAesKey();
            var original = "sample text";

            var encrypted = crypto.Encrypt(original.ToUtfBytes(), key);
            var decrypted = crypto.Decrypt(encrypted, key).ToUtfString();

            Assert.AreEqual(original, decrypted);
        }

        [TestMethod]
        public void AesCantDecryptWithIncorrectKeyTest()
        {
            var crypto = new Crypto();

            var key1 = crypto.GenerateAesKey();
            var key2 = crypto.GenerateAesKey();
            var original = "sample text";

            var encrypted = crypto.Encrypt(original.ToUtfBytes(), key1);

            Assert.ThrowsExactly<InvalidCipherTextException>(() =>
                crypto.Decrypt(encrypted, key2));
        }

        [TestMethod]
        public void X25519CanDecryptWithCorrectKeyTest()
        {
            var crypto = new Crypto();

            var alice = crypto.GenerateX25519KeyPair();
            var bob = crypto.GenerateX25519KeyPair();

            var original = "sample text";

            byte[] encrypted;
            string decrypted;

            // Alice to Bob
            encrypted = crypto.Encrypt(original.ToUtfBytes(), new X25519KeyPair(alice.Private, bob.Public));
            decrypted = crypto.Decrypt(encrypted, new X25519KeyPair(bob.Private, alice.Public)).ToUtfString();
            Assert.AreEqual(original, decrypted);

            // Bob to Alice
            encrypted = crypto.Encrypt(original.ToUtfBytes(), new X25519KeyPair(bob.Private, alice.Public));
            decrypted = crypto.Decrypt(encrypted, new X25519KeyPair(alice.Private, bob.Public)).ToUtfString();
            Assert.AreEqual(original, decrypted);
        }

        [TestMethod]
        public void X25519CantDecryptWithIncorrectKeyTest()
        {
            var crypto = new Crypto();

            var alice = crypto.GenerateX25519KeyPair();
            var bob = crypto.GenerateX25519KeyPair();

            var original = "sample text";

            byte[] encrypted;

            // Alice to Bob
            encrypted = crypto.Encrypt(original.ToUtfBytes(), new X25519KeyPair(alice.Private, bob.Public));
            Assert.ThrowsExactly<InvalidCipherTextException>(() =>
                 crypto.Decrypt(encrypted, new X25519KeyPair(alice.Private, alice.Public)));
            Assert.ThrowsExactly<InvalidCipherTextException>(() =>
                 crypto.Decrypt(encrypted, new X25519KeyPair(bob.Private, bob.Public)));

            // Bob to Alice
            encrypted = crypto.Encrypt(original.ToUtfBytes(), new X25519KeyPair(bob.Private, alice.Public));
            Assert.ThrowsExactly<InvalidCipherTextException>(() =>
                 crypto.Decrypt(encrypted, new X25519KeyPair(alice.Private, alice.Public)));
            Assert.ThrowsExactly<InvalidCipherTextException>(() =>
                 crypto.Decrypt(encrypted, new X25519KeyPair(bob.Private, bob.Public)));
        }
    }
}