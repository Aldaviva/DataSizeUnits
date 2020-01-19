using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using DataSizeUnits;
using Newtonsoft.Json;
using Xunit;

namespace Tests {

    public class SerializationTests {

        [Fact]
        public void SerializeJson() {
            var input = new DataSize(1024);
            string actual = JsonConvert.SerializeObject(input);
            Assert.Equal(@"{""Quantity"":1024.0,""Unit"":1}", actual);
        }

        [Fact]
        public void DeserializeJson() {
            const string input = @"{""Quantity"":1024.0,""Unit"":1}";
            var actual = JsonConvert.DeserializeObject<DataSize>(input);
            Assert.Equal(1024, actual.Quantity);
            Assert.Equal(Unit.Byte, actual.Unit);
        }

        [Fact]
        public void SerializeBinary() {
            var input = new DataSize(1, Unit.Megabyte);
            IFormatter formatter = new BinaryFormatter();
            using var stream = new MemoryStream();
            formatter.Serialize(stream, input);

            stream.Position = 0;

            var deserialized = (DataSize) formatter.Deserialize(stream);
            Assert.Equal(1, deserialized.Quantity);
            Assert.Equal(Unit.Megabyte, deserialized.Unit);
        }

    }

}