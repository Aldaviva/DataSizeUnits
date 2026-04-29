using DataSizeUnits.Serialization;
using Newtonsoft.Json;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using Formatting = System.Xml.Formatting;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Tests;

public class SerializationTests {

    private static readonly JsonSerializerSettings NewtonsoftJsonSettings = new() { Converters = { new DataSizeNewtonsoftJsonConverter() } };

    [Fact]
    public void SerializeNewtonsoftJson() {
        DataSize input  = new(1024);
        string   actual = JsonConvert.SerializeObject(input, NewtonsoftJsonSettings);
        Assert.Equal("""8192""", actual);
    }

    [Fact]
    public void DeserializeNewtonsoftJson() {
        const string input  = """8192""";
        DataSize     actual = JsonConvert.DeserializeObject<DataSize>(input, NewtonsoftJsonSettings);
        Assert.Equal(1024, actual.Bytes);
    }

    [Fact]
    public void SerializeNativeJson() {
        DataSize input  = new(1024);
        string   actual = JsonSerializer.Serialize(input);
        Assert.Equal("8192", actual);
    }

    [Fact]
    public void DeserializeNativeJson() {
        const string input  = "8192";
        DataSize     actual = JsonSerializer.Deserialize<DataSize>(input);
        Assert.Equal(1024, actual.Bytes);
    }

    [Fact]
    public void DeserializeOldNativeJson() {
        const string input  = """{"Quantity":517713092,"Unit":"Byte"}""";
        DataSize     actual = JsonSerializer.Deserialize<DataSize>(input);
        Assert.Equal(517713092, actual.Bytes);
    }

    [Fact]
    public void DeserializeOldNativeJsonObject() {
        const string input  = """{ "Name": "foo.txt", "Size": { "Quantity": 517713092, "Unit": "Byte" }}""";
        MyFile       actual = JsonSerializer.Deserialize<MyFile>(input)!;
        Assert.Equal(517713092, actual.Size.Bytes);
    }

    [Fact]
    public void SerializeNativeXml() {
        MyFile              input         = new() { Name = "myFile.txt", Size = new DataSize(1024) };
        XmlSerializer       xmlSerializer = new(typeof(MyFile));
        using StringWriter  stringWriter  = new();
        using XmlTextWriter xmlTextWriter = new(stringWriter) { Formatting = Formatting.None };
        xmlSerializer.Serialize(xmlTextWriter, input);
        string actual = stringWriter.ToString();

        // XML serialization produces different attribute order in .NET Framework vs .NET Core, which is benign
        string expected;
#if NETFRAMEWORK
        expected = /*lang=xml*/
            """<?xml version="1.0" encoding="utf-16"?><MyFile xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"><Name>myFile.txt</Name><Size bits="8192" /></MyFile>""";
#else
        expected = /*lang=xml*/
            """<?xml version="1.0" encoding="utf-16"?><MyFile xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"><Name>myFile.txt</Name><Size bits="8192" /></MyFile>""";
#endif
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void DeserializeNativeXml() {
        const string input  = /*lang=xml*/"""<?xml version="1.0" ?><MyFile><Name>myFile.txt</Name><Size bits="8192" /></MyFile>""";
        MyFile       actual = (MyFile) new XmlSerializer(typeof(MyFile)).Deserialize(new StringReader(input))!;
        Assert.Equal(1024, actual.Size.Bytes);
        Assert.Equal("myFile.txt", actual.Name);
    }

    /*
     * ❌ DANGER ❌
     * Adding a no-arg constructor will cause XmlSerializer to generate invalid bytecode.
     * To allow users and JSON deserializers to instantiate this struct with no arguments, give a constructor with arity > 0 a default argument value instead.
     * https://github.com/dotnet/runtime/issues/99613, allegedly fixed in .NET 11
     */
    [Fact]
    public void XmlSerializerMustNotCrashInNetRuntime10OrEarlier() {
        Assert.Null(typeof(DataSize).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null));
    }

    [XmlRoot]
    public class MyFile {

        public string? Name { get; set; }
        public DataSize Size { get; set; }

    }

}