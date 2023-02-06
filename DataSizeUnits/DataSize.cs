using System;
using System.Globalization;
using System.Text.Json.Serialization;

namespace DataSizeUnits;

/// <summary>
/// <para>An amount of digital data. Create instances using the constructor or struct declarations.</para>
/// <para><c>var kilobyte = new DataSize(1024);</c></para>
/// <para><c>var kilobyte = new DataSize(1, Unit.Kilobyte);</c></para>
/// </summary>
[Serializable]
public struct DataSize: IComparable<DataSize>, IFormattable {

    [JsonInclude]
    public double Quantity;

    [JsonInclude]
    public Unit Unit;

    private double AsBits => Quantity * CountBitsInUnit(Unit);

    private static readonly DataSizeFormatter Formatter = DataSizeFormatter.Instance;

    /// <summary>
    /// <para>Create a new instance with the given quantity of the given unit of data.</para>
    /// <para><c>var kilobyte = new DataSize(1, Unit.Kilobyte);</c></para>
    /// </summary>
    /// <param name="quantity">How much of the given data <c>unit</c> to represent.</param>
    /// <param name="unit">The unit of measure of the given <c>quantity</c> of data.</param>
    public DataSize(double quantity, Unit unit) {
        Quantity = quantity;
        Unit     = unit;
    }

    /// <summary>
    /// <para>Create a new instance with the given quantity of bytes.</para>
    /// <para><c>var fileSize = new DataSize(new FileInfo(fileName).Length);</c></para>
    /// </summary>
    /// <param name="bytes">How many bytes to represent.</param>
    public DataSize(long bytes): this(bytes, Unit.Byte) { }

    /// <summary>
    /// <para>Convert the data size to the automatically-chosen best fit unit. This will be the largest unit that represents
    /// the data size as a number greater than or equal to one.</para>
    /// <para><c>new DataSize(1024).Normalize().ToString();</c> → <c>1.00 KB</c></para>
    /// </summary>
    /// <param name="useBitsInsteadOfBytes"><c>true</c> to choose a multiple of bits (bits, kilobits, megabits, etc.), or <c>false</c> (the default) to choose a multiple of bytes (bytes, kilobytes, megabytes, etc.).</param>
    /// <returns>A new instance with the normalized quantity and unit. The original instance is unchanged.</returns>
    public DataSize Normalize(bool useBitsInsteadOfBytes = false) {
        double inputBytes       = ConvertToUnit(Unit.Byte).Quantity;
        int    orderOfMagnitude = (int) Math.Max(0, Math.Floor(Math.Log(Math.Abs(inputBytes), useBitsInsteadOfBytes ? 1000 : 1024)));
        Unit   outputUnit       = ForMagnitude(orderOfMagnitude, useBitsInsteadOfBytes);
        return ConvertToUnit(outputUnit);
    }

    /// <summary>
    /// <para>Convert the data size to the given unit.</para>
    /// <para><c>new DataSize(1024).ConvertToUnit(Unit.Kilobyte).ToString();</c> → <c>1.00 KB</c></para>
    /// </summary>
    /// <param name="destinationUnit">The data size unit that the resulting instance should use.</param>
    /// <returns>A new instance with the converted quantity and unit. The original instance is unchanged.</returns>
    public DataSize ConvertToUnit(Unit destinationUnit) {
        return new DataSize(AsBits / CountBitsInUnit(destinationUnit), destinationUnit);
    }

    /// <summary>
    /// <para>Get a data size unit from its string name or abbreviation.</para>
    /// <para>Supports units of bits and bytes, including the SI units like kibibytes, as well as all their abbreviations.</para>
    /// <para>Some abbreviations are case-insensitive, such as <c>megabyte</c>, but others are case-sensitive, like <c>mb</c> and <c>MB</c> because one means megabits and the other means megabytes.</para>
    /// <para>For example, all the inputs that will be parsed as <c>Unit.Megabyte</c> are <c>M</c>, <c>MB</c>, <c>megabyte</c>, <c>mbyte</c>, <c>mib</c>, and <c>mebibyte</c> (the first two are case-sensitive).</para>
    /// <para>Usage: <c>Unit megabyte = DataSize.ParseUnit("megabyte");</c></para>
    /// </summary>
    /// <param name="unitNameOrAbbreviation">The name (e.g. <c>kilobyte</c>) or abbreviation (e.g. <c>kB</c>) of a data size unit.</param>
    /// <returns>The <see cref="Unit"/> value that represents the matched data size unit.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The given name does not match any known units or their abbreviations.</exception>
    public static Unit ParseUnit(string unitNameOrAbbreviation) {
        switch (unitNameOrAbbreviation.ToLowerInvariant()) {
            case "byte":
                return Unit.Byte;
            case "kilobyte":
            case "kbyte":
            case "kib":
            case "kibibyte":
                return Unit.Kilobyte;
            case "megabyte":
            case "mbyte":
            case "mib":
            case "mebibyte":
                return Unit.Megabyte;
            case "gigabyte":
            case "gbyte":
            case "gib":
            case "gibibyte":
                return Unit.Gigabyte;
            case "terabyte":
            case "tbyte":
            case "tib":
            case "tebibyte":
                return Unit.Terabyte;
            case "petabyte":
            case "pbyte":
            case "pib":
            case "pebibyte":
                return Unit.Petabyte;
            case "exabyte":
            case "ebyte":
            case "eib":
            case "exbibyte":
                return Unit.Exabyte;

            case "bit":
                return Unit.Bit;
            case "kilobit":
            case "kbit":
                return Unit.Kilobit;
            case "megabit":
            case "mbit":
                return Unit.Megabit;
            case "gigabit":
            case "gbit":
                return Unit.Gigabit;
            case "terabit":
            case "tbit":
                return Unit.Terabit;
            case "petabit":
            case "pbit":
                return Unit.Petabit;
            case "exabit":
            case "ebit":
                return Unit.Exabit;

            default:
                //not found in case-insensitive switch, continuing to case-sensitive switch below
                break;
        }

        switch (unitNameOrAbbreviation) {
            case "B":
                return Unit.Byte;
            case "kB":
            case "KB":
            case "K":
                return Unit.Kilobyte;
            case "MB":
            case "M":
                return Unit.Megabyte;
            case "GB":
            case "G":
                return Unit.Gigabyte;
            case "TB":
            case "T":
                return Unit.Terabyte;
            case "PB":
            case "P":
                return Unit.Petabyte;
            case "EB":
            case "E":
                return Unit.Exabyte;

            case "b":
                return Unit.Bit;
            case "kb":
            case "Kb":
            case "k":
                return Unit.Kilobit;
            case "mb":
            case "Mb":
            case "m":
                return Unit.Megabit;
            case "Gb":
            case "gb":
            case "g":
                return Unit.Gigabit;
            case "Tb":
            case "tb":
            case "t":
                return Unit.Terabit;
            case "Pb":
            case "pb":
            case "p":
                return Unit.Petabit;
            case "Eb":
            case "eb":
            case "e":
                return Unit.Exabit;

            default:
                throw new ArgumentOutOfRangeException("Unrecognized abbreviation for data size unit " + unitNameOrAbbreviation);
        }
    }

    private static ulong CountBitsInUnit(Unit sourceUnit) {
        switch (sourceUnit) {
            case Unit.Byte:
                return 8;
            case Unit.Kilobyte:
                return (ulong) 8 << 10;
            case Unit.Megabyte:
                return (ulong) 8 << 20;
            case Unit.Gigabyte:
                return (ulong) 8 << 30;
            case Unit.Terabyte:
                return (ulong) 8 << 40;
            case Unit.Petabyte:
                return (ulong) 8 << 50;
            case Unit.Exabyte:
                return (ulong) 8 << 60;

            case Unit.Bit:
                return 1;
            case Unit.Kilobit:
                return 1000L;
            case Unit.Megabit:
                return 1000L * 1000;
            case Unit.Gigabit:
                return 1000L * 1000 * 1000;
            case Unit.Terabit:
                return 1000L * 1000 * 1000 * 1000;
            case Unit.Petabit:
                return 1000L * 1000 * 1000 * 1000 * 1000;
            case Unit.Exabit:
                return 1000L * 1000 * 1000 * 1000 * 1000 * 1000;

            default:
                throw new ArgumentOutOfRangeException(nameof(sourceUnit), sourceUnit, null);
        }
    }

    private static Unit ForMagnitude(int orderOfMagnitude, bool useBitsInsteadOfBytes) {
        switch (orderOfMagnitude) {
            case 0:
                return useBitsInsteadOfBytes ? Unit.Bit : Unit.Byte;
            case 1:
                return useBitsInsteadOfBytes ? Unit.Kilobit : Unit.Kilobyte;
            case 2:
                return useBitsInsteadOfBytes ? Unit.Megabit : Unit.Megabyte;
            case 3:
                return useBitsInsteadOfBytes ? Unit.Gigabit : Unit.Gigabyte;
            case 4:
                return useBitsInsteadOfBytes ? Unit.Terabit : Unit.Terabyte;
            case 5:
                return useBitsInsteadOfBytes ? Unit.Petabit : Unit.Petabyte;
            default:
                return useBitsInsteadOfBytes ? Unit.Exabit : Unit.Exabyte;
        }
    }

    /// <summary>
    /// <para>Format as a string. The quantity is formatted as a number using the current culture's numeric formatting information,
    /// such as thousands separators and precision. The unit's short abbreviation is appended after a space.</para>
    /// <para><c>new DataSize(1536).ConvertToUnit(Unit.Kilobyte).ToString();</c> → <c>1.50 KB</c></para>
    /// </summary>
    /// <returns>String with the formatted data quantity and unit abbreviation, separated by a space.</returns>
    public override string ToString() {
        return $"{Quantity:N} {Unit.ToAbbreviation()}";
    }

    /// <summary>
    /// <para>Format as a string. The quantity is formatted as a number using the current culture's numeric formatting information,
    /// such as thousands separators. The number of digits after the decimal place is specified as the <c>precision</c> parameter,
    /// overriding the culture's default numeric precision.</para>
    /// </summary>
    /// <param name="precision">Number of digits after the decimal place to use when formatting the quantity as a number. The
    /// default for en-US is 2. To use the default for the current culture, pass the value <c>-1</c>, or call
    /// <see cref="ToString()"/>.</param>
    /// <param name="normalize"><c>true</c> to first normalize this instance to an automatically-chosen unit before converting it
    /// to a string, or <c>false</c> (the default) to use the original unit this instance was defined with.</param>
    /// <param name="formatProvider">The culture to use for formatting strings. The number of decimal digits will be overridden by
    /// <paramref name="precision"/> if it is 0 or greater. Defaults to current culture.</param>
    /// <returns>String with the formatted data quantity and unit abbreviation, separated by a space.</returns>
    public string ToString(int precision, bool normalize = false, IFormatProvider? formatProvider = null) {
        formatProvider ??= CultureInfo.CurrentCulture;
        if (precision >= 0) {
            NumberFormatInfo numberFormatInfo = (NumberFormatInfo) (formatProvider.GetFormat(typeof(NumberFormatInfo)) as NumberFormatInfo ?? NumberFormatInfo.CurrentInfo).Clone();
            numberFormatInfo.NumberDecimalDigits = precision;
            formatProvider                       = numberFormatInfo;
        }

        if (normalize) {
            return Normalize(Unit.IsMultipleOfBits()).ToString(precision, false, formatProvider);
        } else {
            return Quantity.ToString("N", formatProvider) + " " + Unit.ToAbbreviation();
        }
    }

    public string ToString(string? format, IFormatProvider? formatProvider = null) {
        return Formatter.Format(format, this, formatProvider ?? Formatter);
    }

    public string ToString(int precision, Unit unit) {
        return ConvertToUnit(unit).ToString(precision);
    }

    public bool Equals(DataSize other) {
        return AsBits.Equals(other.AsBits);
    }

    public override bool Equals(object? obj) {
        return obj is DataSize other && Equals(other);
    }

    public override int GetHashCode() {
        return AsBits.GetHashCode();
    }

    public int CompareTo(DataSize other) {
        return AsBits.CompareTo(other.AsBits);
    }

    public static bool operator <(DataSize a, DataSize b) => a.AsBits < b.AsBits;

    public static bool operator >(DataSize a, DataSize b) => a.AsBits > b.AsBits;

    public static bool operator <=(DataSize a, DataSize b) => a.AsBits <= b.AsBits;

    public static bool operator >=(DataSize a, DataSize b) => a.AsBits >= b.AsBits;

    public static DataSize operator +(DataSize a, DataSize b) {
        return new DataSize(a.Quantity + b.ConvertToUnit(a.Unit).Quantity, a.Unit);
    }

    public static DataSize operator -(DataSize a, DataSize b) {
        return new DataSize(a.Quantity - b.ConvertToUnit(a.Unit).Quantity, a.Unit);
    }

    public static DataSize operator *(DataSize a, double b) {
        return new DataSize(a.Quantity * b, a.Unit);
    }

    /// <exception cref="DivideByZeroException"></exception>
    public static DataSize operator /(DataSize a, double b) {
        if (!b.Equals(0)) {
            return new DataSize(a.Quantity / b, a.Unit);
        } else {
            throw new DivideByZeroException();
        }
    }

    /// <exception cref="DivideByZeroException"></exception>
    public static double operator /(DataSize a, DataSize b) {
        if (!b.Quantity.Equals(0)) {
            return a.AsBits / b.AsBits;
        } else {
            throw new DivideByZeroException();
        }
    }

    public static bool operator ==(DataSize a, DataSize b) => a.Equals(b);

    public static bool operator !=(DataSize a, DataSize b) => !a.Equals(b);

    public static implicit operator long(DataSize dataSize) => (long) dataSize.AsBits / 8;

    public static implicit operator DataSize(long bytes) => new(bytes);

}