using Authi.Common.Services;
using System;

namespace Authi.Common.Test
{
    [TestClass]
    public class BinarySerializerTests : TestsBase
    {
        public IBinarySerializer Serializer { get; } = new BinarySerializer();

        [TestMethod]
        public void StringTest()
        {
            const string input = "test";
            var bytes = Serializer.Serialize(input);
            var output = Serializer.Deserialize<string>(bytes);
            Assert.AreEqual(input, output);
        }

        [TestMethod]
        public void IntTest()
        {
            const int input = 42;
            var bytes = Serializer.Serialize(input);
            var output = Serializer.DeserializeValue<int>(bytes);
            Assert.AreEqual(input, output);
        }

        [TestMethod]
        public void BoolTest()
        {
            const bool input = true;
            var bytes = Serializer.Serialize(input);
            var output = Serializer.DeserializeValue<bool>(bytes);
            Assert.AreEqual(input, output);
        }

        [TestMethod]
        public void DoubleTest()
        {
            const double input = 123.456;
            var bytes = Serializer.Serialize(input);
            var output = Serializer.DeserializeValue<double>(bytes);
            Assert.AreEqual(input, output);
        }

        [TestMethod]
        public void GuidTest()
        {
            var input = Guid.NewGuid();
            var bytes = Serializer.Serialize(input);
            var output = Serializer.DeserializeValue<Guid>(bytes);
            Assert.AreEqual(input, output);
        }

        [TestMethod]
        public void IntArrayTest()
        {
            var input = new[] { 1, 2, 3, 4 };
            var bytes = Serializer.Serialize(input);
            var output = Serializer.Deserialize<int[]>(bytes);
            CollectionAssert.AreEqual(input, output);
        }

        [TestMethod]
        public void BoolArrayTest()
        {
            var input = new[] { true, false, true };
            var bytes = Serializer.Serialize(input);
            var output = Serializer.Deserialize<bool[]>(bytes);
            CollectionAssert.AreEqual(input, output);
        }

        [TestMethod]
        public void DoubleArrayTest()
        {
            var input = new[] { 1.1, 2.2, 3.3 };
            var bytes = Serializer.Serialize(input);
            var output = Serializer.Deserialize<double[]>(bytes);
            CollectionAssert.AreEqual(input, output);
        }

        [TestMethod]
        public void GuidArrayTest()
        {
            var input = new[] { Guid.NewGuid(), Guid.NewGuid() };
            var bytes = Serializer.Serialize(input);
            var output = Serializer.Deserialize<Guid[]>(bytes);
            CollectionAssert.AreEqual(input, output);
        }

        [TestMethod]
        public void NullStringTest()
        {
            string? input = null;
            var bytes = Serializer.Serialize(input);
            var output = Serializer.Deserialize<string>(bytes);
            Assert.IsNull(output);
        }

        [TestMethod]
        public void NullIntTest()
        {
            int? input = null;
            var bytes = Serializer.Serialize(input);
            var output = Serializer.DeserializeValue<int>(bytes);
            Assert.AreEqual(input, output);
        }

        [TestMethod]
        public void NullByteArrayTest()
        {
            byte[]? input = null;
            var bytes = Serializer.Serialize(input);
            var output = Serializer.Deserialize<byte[]>(bytes);
            Assert.IsNull(output);
        }
    }
}
