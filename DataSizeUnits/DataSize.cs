using System;
using System.Globalization;

namespace DataSizeUnits;

/// <summary>
/// <para>An amount of digital data. Create instances using the constructors.</para>
/// <para><c>var kilobyte = new DataSize(1024);</c></para>
/// <para><c>var kilobyte = new DataSize(1, Unit.Kilobyte);</c></para>
/// </summary>
/// <param name="quantity">How much of the given data <paramref name="unit"/> to represent</param>
/// <param name="unit">The unit of measure of the given <paramref name="quantity"/> of data</param>
[Serializable]
public readonly struct DataSize(double quantity, Unit unit): IComparable<DataSize>, IEquatable<DataSize>, IFormattable {

    private static readonly DataSizeFormatter Formatter = DataSizeFormatter.Instance;

    private double AsBits => Quantity * CountBitsInUnit(Unit);

    /// <summary>How much of the given data <see cref="Unit"/> to represent</summary>
    public double Quantity { get; init; } = quantity;

    /// <summary>The unit of measure of the given <see cref="Quantity"/> of data</summary>
    public Unit Unit { get; init; } = unit;

    /// <summary>
    /// Create a new instance that represents 0 bytes.
    /// </summary>
    public DataSize(): this(0, Unit.Byte) { }

    /// <summary>
    /// <para>Create a new instance with the given quantity of bytes.</para>
    /// <para><c>var fileSize = new DataSize(new FileInfo(fileName).Length);</c></para>
    /// </summary>
    /// <param name="bytes">How many bytes to represent.</param>
    public DataSize(long bytes): this(bytes, Unit.Byte) { }

    /// <inheritdoc cref="DataSize(long)" />
    public DataSize(ulong bytes): this(bytes, Unit.Byte) { }

    /// <summary>
    /// <para>Convert the data size to the automatically-chosen best fit unit. This will be the largest unit that represents
    /// the data size as a number greater than or equal to one.</para>
    /// <para><c>new DataSize(1024).Normalize().ToString();</c> → <c>1.00 KB</c></para>
    /// </summary>
    /// <param name="useBitsInsteadOfBytes"><c>true</c> to choose a multiple of bits (bits, kilobits, megabits, etc.), or <c>false</c> (the default) to choose a multiple of bytes (bytes, kilobytes, megabytes, etc.).</param>
    /// <returns>A new instance with the normalized quantity and unit. The original instance is unchanged.</returns>
    public DataSize Normalize(bool useBitsInsteadOfBytes = false) {
        double inputBitsOrBytes = ConvertToUnit(useBitsInsteadOfBytes ? Unit.Bit : Unit.Byte).Quantity;
        int    orderOfMagnitude = (int) Math.Max(0, Math.Floor(Math.Log(Math.Abs(inputBitsOrBytes), useBitsInsteadOfBytes ? 1000 : 1024)));
        Unit   outputUnit       = ForMagnitude(orderOfMagnitude, useBitsInsteadOfBytes);
        return ConvertToUnit(outputUnit);
    }

    /// <summary>
    /// <para>Convert the data size to the given unit.</para>
    /// <para><c>new DataSize(1024).ConvertToUnit(Unit.Kilobyte).ToString();</c> → <c>1.00 KB</c></para>
    /// </summary>
    /// <param name="destinationUnit">The data size unit that the resulting instance should use.</param>
    /// <returns>A new instance with the converted quantity and unit. The original instance is unchanged.</returns>
    public DataSize ConvertToUnit(Unit destinationUnit) => new(AsBits / CountBitsInUnit(destinationUnit), destinationUnit);

    /// <summary>
    /// <para>Get a data size unit from its string name or abbreviation.</para>
    /// <para>Supports units of bits and bytes, including the JEDEC units like kilobytes and IEC units like kibibytes, as well as all their abbreviations.</para>
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

        return unitNameOrAbbreviation switch {
            "B"                 => Unit.Byte,
            "kB" or "KB" or "K" => Unit.Kilobyte,
            "MB" or "M"         => Unit.Megabyte,
            "GB" or "G"         => Unit.Gigabyte,
            "TB" or "T"         => Unit.Terabyte,
            "PB" or "P"         => Unit.Petabyte,
            "EB" or "E"         => Unit.Exabyte,
            "b"                 => Unit.Bit,
            "kb" or "Kb" or "k" => Unit.Kilobit,
            "mb" or "Mb" or "m" => Unit.Megabit,
            "Gb" or "gb" or "g" => Unit.Gigabit,
            "Tb" or "tb" or "t" => Unit.Terabit,
            "Pb" or "pb" or "p" => Unit.Petabit,
            "Eb" or "eb" or "e" => Unit.Exabit,
            _                   => throw new ArgumentOutOfRangeException("Unrecognized abbreviation for data size unit " + unitNameOrAbbreviation)
        };
    }

    /// <exception cref="ArgumentOutOfRangeException">illegal value of <paramref name="sourceUnit"/> from force casting</exception>
    private static ulong CountBitsInUnit(Unit sourceUnit) => sourceUnit switch {
        Unit.Byte     => 8,
        Unit.Kilobyte => 8192UL,
        Unit.Megabyte => 8388608UL,
        Unit.Gigabyte => 8589934592UL,
        Unit.Terabyte => 8796093022208UL,
        Unit.Petabyte => 9007199254740992UL,
        Unit.Exabyte  => 9223372036854775808UL,
        Unit.Bit      => 1,
        Unit.Kilobit  => 1000L,
        Unit.Megabit  => 1000000UL,
        Unit.Gigabit  => 1000000000UL,
        Unit.Terabit  => 1000000000000UL,
        Unit.Petabit  => 1000000000000000UL,
        Unit.Exabit   => 1000000000000000000UL,
        _             => throw new ArgumentOutOfRangeException(nameof(sourceUnit), sourceUnit, null)
    };

    private static Unit ForMagnitude(int orderOfMagnitude, bool useBitsInsteadOfBytes) => orderOfMagnitude switch {
        0 => useBitsInsteadOfBytes ? Unit.Bit : Unit.Byte,
        1 => useBitsInsteadOfBytes ? Unit.Kilobit : Unit.Kilobyte,
        2 => useBitsInsteadOfBytes ? Unit.Megabit : Unit.Megabyte,
        3 => useBitsInsteadOfBytes ? Unit.Gigabit : Unit.Gigabyte,
        4 => useBitsInsteadOfBytes ? Unit.Terabit : Unit.Terabyte,
        5 => useBitsInsteadOfBytes ? Unit.Petabit : Unit.Petabyte,
        _ => useBitsInsteadOfBytes ? Unit.Exabit : Unit.Exabyte
    };

    /// <summary>
    /// <para>Format as a string. The quantity is formatted as a number using the current culture's numeric formatting information,
    /// such as thousands separators and precision. The unit's short abbreviation is appended after a space.</para>
    /// <para><c>new DataSize(1536).ConvertToUnit(Unit.Kilobyte).ToString();</c> → <c>1.50 KB</c></para>
    /// </summary>
    /// <returns>String with the formatted data quantity and unit abbreviation, separated by a space.</returns>
    public override string ToString() => $"{Quantity:N} {Unit.ToAbbreviation()}";

    /// <summary>
    /// <para>Format the quantity and unit abbreviation as a string. The quantity is formatted as a number using the current culture's numeric formatting information,
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
    /// <returns>String with the formatted data quantity and unit JEDEC (MB) abbreviation, separated by a space.</returns>
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

    /// <summary>
    /// Format as a string with a template and format provider to override <see cref="DataSizeFormatter"/>
    /// </summary>
    /// <param name="template">A format string like <c>A2</c></param>
    /// <param name="formatProvider">A format provider to use when serializing numbers, or <c>null</c> to use the default <see cref="DataSizeFormatter"/></param>
    /// <returns>This instance formatted in the given template with the given <see cref="IFormatProvider"/></returns>
    /// <exception cref="FormatException">if <paramref name="formatProvider"/> throws a <see cref="FormatException"/></exception>
    public string ToString(string? template, IFormatProvider? formatProvider = null) => Formatter.Format(template, this, formatProvider ?? Formatter);

    /// <summary>
    /// Format as a string with the quantity at the given precision and unit
    /// </summary>
    /// <param name="precision">Number of digits after the decimal place</param>
    /// <param name="unit">The unit of measure to convert the quantity to before formatting</param>
    /// <returns>The quantity of data in the given unit, rounded to the specified precision, followed by a space and the unit's JEDEC (MB) abbreviation</returns>
    public string ToString(int precision, Unit unit) => ConvertToUnit(unit).ToString(precision);

    /// <summary>
    /// Compare data size equality
    /// </summary>
    /// <param name="other">Another amount of data</param>
    /// <returns><c>true</c> if this instance and <paramref name="other"/> represent the same amount of data, or <c>false</c> if they represent different amounts</returns>
    public bool Equals(DataSize other) => AsBits.Equals(other.AsBits);

    /// <inheritdoc cref="Equals(DataSizeUnits.DataSize)" />
    public override bool Equals(object? other) => other is DataSize other2 && Equals(other2);

    /// <inheritdoc cref="Equals(DataSizeUnits.DataSize)" />
    public static bool operator ==(DataSize self, DataSize other) => self.Equals(other);

    /// <inheritdoc cref="Equals(DataSizeUnits.DataSize)" />
    public static bool operator !=(DataSize self, DataSize other) => !self.Equals(other);

    /// <inheritdoc cref="Double.GetHashCode"/>
    public override int GetHashCode() => AsBits.GetHashCode();

    /// <summary>
    /// Compares two <see cref="DataSize"/> instances
    /// </summary>
    /// <param name="other">Another amount of data</param>
    /// <returns>A number &lt; 0 if this instance is smaller than <paramref name="other"/>, <c>0</c> if they are equal, or a number &gt; 0 if this instance is larger than <paramref name="other"/></returns>
    public int CompareTo(DataSize other) => AsBits.CompareTo(other.AsBits);

    /// <summary>
    /// Compare two amounts of data
    /// </summary>
    /// <param name="a">An amount of data</param>
    /// <param name="b">Another amount of data</param>
    /// <returns><c>true</c> if <paramref name="a"/> represents less data than <paramref name="b"/>, or <c>false</c> if <paramref name="a"/> represents either more than or the same amount of data as <paramref name="b"/></returns>
    public static bool operator <(DataSize a, DataSize b) => a.AsBits < b.AsBits;

    /// <summary>
    /// Compare two amounts of data
    /// </summary>
    /// <param name="a">An amount of data</param>
    /// <param name="b">Another amount of data</param>
    /// <returns><c>true</c> if <paramref name="a"/> represents more data than <paramref name="b"/>, or <c>false</c> if <paramref name="a"/> represents either less than or the same amount of data as <paramref name="b"/></returns>
    public static bool operator >(DataSize a, DataSize b) => a.AsBits > b.AsBits;

    /// <summary>
    /// Compare two amounts of data
    /// </summary>
    /// <param name="a">An amount of data</param>
    /// <param name="b">Another amount of data</param>
    /// <returns><c>true</c> if <paramref name="a"/> represents either less than or the same amount of data as <paramref name="b"/>, or <c>false</c> if <paramref name="a"/> represents more data than <paramref name="b"/></returns>
    public static bool operator <=(DataSize a, DataSize b) => a.AsBits <= b.AsBits;

    /// <summary>
    /// Compare two amounts of data
    /// </summary>
    /// <param name="a">An amount of data</param>
    /// <param name="b">Another amount of data</param>
    /// <returns><c>true</c> if <paramref name="a"/> represents either more than or the same amount of data as <paramref name="b"/>, or <c>false</c> if <paramref name="a"/> represents less data than <paramref name="b"/></returns>
    public static bool operator >=(DataSize a, DataSize b) => a.AsBits >= b.AsBits;

    /// <summary>
    /// Adds amounts of data together
    /// </summary>
    /// <param name="a">An amount of data</param>
    /// <param name="b">Another amount of data</param>
    /// <returns>The sum of the data sizes of <paramref name="a"/> and <paramref name="b"/>, in the units of <paramref name="a"/></returns>
    public static DataSize operator +(DataSize a, DataSize b) => a with { Quantity = a.Quantity + b.ConvertToUnit(a.Unit).Quantity };

    /// <summary>
    /// Subtracts amounts of data
    /// </summary>
    /// <param name="a">An amount of data</param>
    /// <param name="b">Another amount of data</param>
    /// <returns>The difference of the data sizes of <paramref name="a"/> and <paramref name="b"/>, in the units of <paramref name="a"/></returns>
    public static DataSize operator -(DataSize a, DataSize b) => a with { Quantity = a.Quantity - b.ConvertToUnit(a.Unit).Quantity };

    /// <summary>
    /// Multiplies amounts of data together
    /// </summary>
    /// <param name="a">An amount of data</param>
    /// <param name="b">Another amount of data</param>
    /// <returns>The product of the data sizes of <paramref name="a"/> and <paramref name="b"/>, in the units of <paramref name="a"/></returns>
    public static DataSize operator *(DataSize a, double b) => a with { Quantity = a.Quantity * b };

    /// <summary>
    /// Divides an amount of data
    /// </summary>
    /// <param name="a">An amount of data as the numerator</param>
    /// <param name="b">Denominator</param>
    /// <returns>The quotient of the data size <paramref name="a"/> divided by <paramref name="b"/>, in the units of <paramref name="a"/></returns>
    /// <exception cref="DivideByZeroException">if <paramref name="b"/> is <c>0</c></exception>
    public static DataSize operator /(DataSize a, double b) {
        if (!b.Equals(0)) {
            return a with { Quantity = a.Quantity / b };
        } else {
            throw new DivideByZeroException($"Cannot divide {a} by zero");
        }
    }

    /// <summary>
    /// Divides an amount of data
    /// </summary>
    /// <param name="a">An amount of data as the numerator</param>
    /// <param name="b">An amount of data as the denominator</param>
    /// <returns>The quotient of the data size <paramref name="a"/> divided by <paramref name="b"/>, in the units of <paramref name="a"/></returns>
    /// <exception cref="DivideByZeroException">if <paramref name="b"/> is <c>0</c></exception>
    public static double operator /(DataSize a, DataSize b) {
        if (!b.Quantity.Equals(0)) {
            return a.AsBits / b.AsBits;
        } else {
            throw new DivideByZeroException($"Cannot divide {a} by zero");
        }
    }

    /// <summary>
    /// Implicitly cast a <see cref="DataSize"/> instance to a 64-bit integer number of bytes.
    /// </summary>
    /// <param name="dataSize">An amount of data</param>
    /// <returns>The number of bytes in <paramref name="dataSize"/></returns>
    public static implicit operator long(DataSize dataSize) => (long) dataSize.AsBits / 8;

    /// <summary>
    /// Implicitly cast a 64-bit integer to a <see cref="DataSize"/> that represents the original value's number of bytes.
    /// </summary>
    /// <param name="bytes">A 64-bit integer</param>
    /// <returns>A <see cref="DataSize"/> instance with the <see cref="Quantity"/>set to <paramref name="bytes"/> number of bytes, and the <see cref="Unit"/> set to <see cref="DataSizeUnits.Unit.Byte"/></returns>
    public static implicit operator DataSize(long bytes) => new(bytes);

}