using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Serialization;
using DataSizeUnits;
using Newtonsoft.Json;
using Xunit;
using Formatting = System.Xml.Formatting;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Tests;

public class SerializationTests {

    [Fact]
    public void SerializeNewtonsoftJson() {
        DataSize input  = new(1024);
        string   actual = JsonConvert.SerializeObject(input);
        Assert.Equal("""{"Quantity":1024.0,"Unit":1}""", actual);
    }

    [Fact]
    public void DeserializeNewtonsoftJson() {
        const string input  = """{"Quantity":1024.0,"Unit":1}""";
        DataSize     actual = JsonConvert.DeserializeObject<DataSize>(input);
        Assert.Equal(1024, actual.Quantity);
        Assert.Equal(Unit.Byte, actual.Unit);
    }

    [Fact]
    public void SerializeNativeJson() {
        DataSize input  = new(1024);
        string   actual = JsonSerializer.Serialize(input);
        Assert.Equal("""{"Quantity":1024,"Unit":1}""", actual);
    }

    [Fact]
    public void DeserializeNativeJson() {
        const string input  = """{"Quantity":1024.0,"Unit":1}""";
        DataSize     actual = JsonSerializer.Deserialize<DataSize>(input);
        Assert.Equal(1024, actual.Quantity);
        Assert.Equal(Unit.Byte, actual.Unit);
    }

    [Fact]
    public void SerializeNativeXml() {
        DataSize            input         = new(1024);
        XmlSerializer       xmlSerializer = new(typeof(DataSize));
        using StringWriter  stringWriter  = new();
        using XmlTextWriter xmlTextWriter = new(stringWriter) { Formatting = Formatting.None };
        xmlSerializer.Serialize(xmlTextWriter, input);
        string actual = stringWriter.ToString();

        Assert.Equal( /*lang=xml*/
            """<?xml version="1.0" encoding="utf-16"?><DataSize xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"><Quantity>1024</Quantity><Unit>Byte</Unit></DataSize>""",
            actual);
    }

    [Fact]
    public void DeserializeNativeXml() {
        const string input  = /*lang=xml*/"""<?xml version="1.0" ?><DataSize><Quantity>1024</Quantity><Unit>Byte</Unit></DataSize>""";
        DataSize     actual = (DataSize) new XmlSerializer(typeof(DataSize)).Deserialize(new StringReader(input))!;
        Assert.Equal(1024, actual.Quantity);
        Assert.Equal(Unit.Byte, actual.Unit);
    }

    [Fact]
    public void SerializeBinary() {
        DataSize           input     = new(1, Unit.Megabyte);
        IFormatter         formatter = new BinaryFormatter();
        using MemoryStream stream    = new();
        formatter.Serialize(stream, input);

        stream.Position = 0;

        DataSize deserialized = (DataSize) formatter.Deserialize(stream);
        Assert.Equal(1, deserialized.Quantity);
        Assert.Equal(Unit.Megabyte, deserialized.Unit);
    }

}