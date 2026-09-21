using DataSizeUnits.Serialization;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace DataSizeUnits;

/// <summary>
/// <para>Represents an amount of digital data. Create instances using the constructors or <c>(Try)Parse</c> static methods.</para>
/// <code>var kilobyte = new DataSize(1024);
/// kilobyte = new DataSize(1, DataSizeUnit.Kilobyte);</code>
/// <para>Values are represented internally as bits using infinite-precision signed <see cref="BigInteger"/>s.</para>
/// </summary>
[Serializable]
[JsonConverter(typeof(DataSizeJsonConverter))]
public readonly partial struct DataSize: IXmlSerializable
#if NET7_0_OR_GREATER
    , INumber<DataSize>, ISignedNumber<DataSize>
#else
    , IComparable<DataSize>, IEquatable<DataSize>, IFormattable
#endif
{

    static DataSize() {
        try {
            DataSizeNewtonsoftJsonConverterRegistrar.Register();
        } catch (FileNotFoundException) {
            // Newtonsoft.Json is not on the assembly load path, so skip registering our JsonConverter with it
        }
    }

    /// <summary>
    /// The total number of bits represented by this value.
    /// </summary>
    [XmlIgnore]
    public BigInteger Bits { get; }

    /// <summary>
    /// <para>The total number of bytes represented by this value.</para>
    /// <para>If there is a partial byte because <see cref="Bits"/> is not an integer multiple of 8, this is rounded toward zero to the closest integer byte.</para>
    /// </summary>
    [JsonIgnore] [XmlIgnore]
    public BigInteger Bytes => Bits >> 3;

    /// <summary>
    /// Create a new value that represents the given quantity of bits.
    /// </summary>
    /// <param name="bits">How many bits to represent.</param>
    public DataSize(BigInteger bits) {
        Bits = bits;
    }

    /*
     * ❌ DANGER ❌
     * Adding a no-arg constructor will cause XmlSerializer to generate invalid bytecode.
     * To allow users and JSON deserializers to instantiate this struct with no arguments, give a constructor with arity > 0 a default argument value instead (in this case, DataSize(long)).
     * https://github.com/dotnet/runtime/issues/99613, allegedly fixed in .NET 11
     */
    /*
    /// <summary>
    /// Create a new value that represents 0 bytes.
    /// </summary>
    public DataSize() {
        Bits = BigInteger.Zero;
    }
    */

    /// <summary>
    /// <para>Create a new value that represents the given quantity of bytes.</para>
    /// <code>var fileSize = new DataSize(new FileInfo(fileName).Length);</code>
    /// </summary>
    /// <param name="bytes">How many bytes to represent, or 0 if omitted.</param>
    public DataSize(long bytes = 0): this((BigInteger) bytes << 3) {}

    /// <inheritdoc cref="DataSize(long)" />
    public DataSize(ulong bytes): this((BigInteger) bytes << 3) {}

    /// <summary>
    /// <para>Create a new value that represents the given quantity and unit of data.</para>
    /// <code>var size = new DataSize(2, DataSizeUnit.Megabyte);</code>
    /// </summary>
    /// <param name="quantity">How much <paramref name="unit"/> to represent.</param>
    /// <param name="unit">A unit of data size.</param>
    public DataSize(long quantity, DataSizeUnit unit = DataSizeUnit.Byte): this(quantity * unit.AsBits) {}

    /// <inheritdoc cref="DataSize(long,DataSizeUnit)" />
    public DataSize(ulong quantity, DataSizeUnit unit = DataSizeUnit.Byte): this(quantity * unit.AsBits) {}

    /// <inheritdoc cref="DataSize(long,DataSizeUnit)" />
    public DataSize(BigInteger quantity, DataSizeUnit unit = DataSizeUnit.Byte): this(quantity * unit.AsBits) {}

    /// <inheritdoc cref="DataSize(long,DataSizeUnit)" />
    public DataSize(double quantity, DataSizeUnit unit): this(DoubleToBigInteger(quantity, unit.AsBits)) {}

    /// This is very stupid
    private static BigInteger DoubleToBigInteger(double quantity, BigInteger coefficient) {
        string     doubleAsString    = quantity.ToString("F17", CultureInfo.InvariantCulture).TrimEnd('0');
        int        decimalPointIndex = doubleAsString.LastIndexOf('.');
        int        extraMagnitude    = decimalPointIndex == -1 ? 0 : doubleAsString.Length - decimalPointIndex - 1;
        BigInteger numerator         = BigInteger.Parse(doubleAsString.Replace(".", string.Empty));
        BigInteger denominator       = BigInteger.Pow(10, extraMagnitude);
        return numerator * coefficient / denominator;
    }

    /*
     * https://www.reflectionit.nl/blog/2022/implement-ixmlserializable-in-a-readonly-struct
     */
    void IXmlSerializable.ReadXml(XmlReader reader) {
        BigInteger bits = reader.MoveToAttribute("bits") ? BigInteger.Parse(reader.Value, NumberStyles.Integer, CultureInfo.InvariantCulture) : BigInteger.Zero;
        reader.Skip();
        Unsafe.AsRef(in this) = new DataSize(bits);
    }

    void IXmlSerializable.WriteXml(XmlWriter writer) {
        writer.WriteAttributeString("bits", Bits.ToString(CultureInfo.InvariantCulture));
    }

    XmlSchema? IXmlSerializable.GetSchema() => null;

    /// <summary>
    /// <para>Convert this value to the given <paramref name="unit"/>.</para>
    /// <para>This is a floating-point conversion, best with a <paramref name="unit"/> that is LARGER than what this <see cref="DataSize"/> was created with (e.g. kB → MB), thereby decreasing the magnitude of the resulting quantity. However, be aware that this incurs a loss of precision due to the limitations of <see cref="double"/>. When converting to a smaller unit or otherwise expecting an integral result, consider using <see cref="AsUnitExact"/> for infinite integer precision.</para>
    /// <code>double _512kbytesInMbytes = new DataSizeUnit(512, DataSizeUnits.Kilobyte).AsUnit(DataSizeUnits.Megabyte);
    /// // _512kbytesInMbytes == 0.5</code>
    /// </summary>
    /// <param name="unit">The unit of data to which you want to convert this value's number of bits.</param>
    /// <returns>The converted quantity in the <paramref name="unit"/> specified.</returns>
    public double AsUnit(DataSizeUnit unit) => unit switch {
        DataSizeUnit.Bit  => (double) Bits,
        DataSizeUnit.Byte => (double) Bytes,
        _                 => (double) Bits / (double) unit.AsBits
    };

    /// <summary>
    /// <para>Convert this value to the given <paramref name="unit"/>.</para>
    /// <para>This is an integer conversion, best with a <paramref name="unit"/> that is SMALLER than what this <see cref="DataSize"/> was created with (e.g. MB → kB), thereby increasing the magnitude of the resulting quantity. However, be aware that this cannot represent floating-point results and truncates to whole numbers of <paramref name="unit"/>. When converting to a larger unit or otherwise expecting a floating-point result, consider using <see cref="AsUnit"/> for fractional precision.</para>
    /// <code>BigInteger oneMbyteInKbytes = new DataSizeUnit(1, DataSizeUnit.Megabyte).AsUnitExact(DataSizeUnit.Kilobyte);
    /// // oneMbyteInKbytes == 1024</code>
    /// </summary>
    /// <param name="unit">The unit of data to which you want to convert this value's number of bits.</param>
    /// <returns>The converted quantity in the <paramref name="unit"/> specified.</returns>
    public BigInteger AsUnitExact(DataSizeUnit unit) => unit switch {
        DataSizeUnit.Bit  => Bits,
        DataSizeUnit.Byte => Bytes,
        _                 => Bits / unit.AsBits
    };

    /// <summary>
    /// <para>Converts this value to an automatically ranged unit.</para>
    /// <para>This normalizes the value to make it greater than or equal to 1 of the chosen unit, and less than 1 of the next highest unit.</para>
    /// <para>For example, given 524,288,000 bytes, this will automatically range to megabytes, because 1 MB ≤ 500 MB &lt; 1 GB:</para>
    /// <code>var normalized = new DataSizeUnit(524_288_000).AsAutomaticUnit();
    /// // normalized.quantity == 500.0
    /// // normalized.unit == DataSizeUnit.Megabytes</code>
    /// </summary>
    /// <param name="useBitsInsteadOfBytes"><c>true</c> to choose a unit based on bits, or <c>false</c> to choose a unit based on bytes</param>
    /// <returns></returns>
    public (double quantity, DataSizeUnit unit) AsAutomaticUnit(bool useBitsInsteadOfBytes = false) {
        if (Bits.Equals(BigInteger.Zero)) {
            return (0, useBitsInsteadOfBytes ? DataSizeUnit.Bit : DataSizeUnit.Byte);
        }

        int orderOfMagnitude;
        if (useBitsInsteadOfBytes) {
            // BigInteger.Log10 introduces double imprecision
            orderOfMagnitude = (BigInteger.Abs(Bits).ToString("R").Length - 1) / 3;
        } else {
#if NET7_0_OR_GREATER
            orderOfMagnitude = (int) (BigInteger.Log2(BigInteger.Abs(Bits) >> 3) / 10);
#else
            // BigInteger.Log introduces double imprecision
            byte[] bytes = (BigInteger.Abs(Bits) >> 3).ToByteArray();
            byte   mostSignificantByte = Enumerable.Reverse(bytes).SkipWhile(b => b == 0).First();
            orderOfMagnitude = ((bytes.Length - 1) * 8 + mostSignificantByte switch {
                >= 0x80 => 8,
                >= 0x40 => 7,
                >= 0x20 => 6,
                >= 0x10 => 5,
                >= 0x08 => 4,
                >= 0x04 => 3,
                >= 0x02 => 2,
                >= 0x01 => 1,
                >= 0x00 => 0
            } - 1) / 10;
#endif
        }

        DataSizeUnit unit = DataSizeUnit.ForMagnitude(orderOfMagnitude, useBitsInsteadOfBytes);
        return (quantity: AsUnit(unit), unit);
    }

}