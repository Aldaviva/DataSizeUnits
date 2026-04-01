using System.Globalization;
using System.Numerics;
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

#if NET8_0_OR_GREATER
using System.Buffers;
#endif

#if !NET7_0_OR_GREATER
// ReSharper disable InheritdocInvalidUsage - docs come from types introduced in .NET 7
#endif

#pragma warning disable CS1573 // Broken heuristic, not aware of inheritdoc

namespace DataSizeUnits;

public readonly partial struct DataSize {

    private static readonly char[] Whitespace = ['\t', '\n', '\v', '\f', '\r', '\x20'];

#if NET8_0_OR_GREATER
    private static readonly SearchValues<char> WhitespaceSearchValues = SearchValues.Create(Whitespace);
#endif

    #region Constants

    /// <inheritdoc />
    public static DataSize Zero => new(BigInteger.Zero);

    #endregion

    #region Formatting

    /// <summary>
    /// <para>Format as a string. The quantity is normalized and formatted as a number using the current culture's numeric formatting information, such as thousands separators and precision. The byte-based auto-ranged unit's short abbreviation is appended after a space.</para>
    /// <para><c>new DataSize(1536).ToString()</c> → <c>1.50 kB</c></para>
    /// </summary>
    /// <returns>String with the formatted data quantity and unit abbreviation, separated by a space.</returns>
    public override string ToString() => ToString();

    /// <summary>
    /// <para>Format as a string. The quantity is normalized and formatted as a number. The auto-ranged unit's short abbreviation is appended after a space.</para>
    /// <para><c>new DataSize(1536).ToString(true)</c> → <c>12.29 kb</c></para>
    /// </summary>
    /// <param name="useBitsInsteadOfBytes"><c>true</c> to range the value to a unit based on bits, or <c>false</c> to use a unit based on bytes.</param>
    /// <param name="formatProvider">Localization settings, such as number precision and thousands separators.</param>
    /// <returns>String with the formatted data quantity and unit abbreviation, separated by a space.</returns>
    public string ToString(bool useBitsInsteadOfBytes = false, IFormatProvider? formatProvider = null) {
        (double quantity, DataSizeUnit unit) = AsAutomaticUnit(useBitsInsteadOfBytes);
        return string.Format(formatProvider, "{0:N} {1}", quantity, unit.ToAbbreviation());
    }

    /// <summary>
    /// <para>Format as a string. The quantity is normalized and formatted as a number using the current culture's numeric formatting information, such as thousands separators. The auto-ranged unit's short abbreviation is appended after a space.</para>
    /// <para><c>new DataSize(1536).ToString(1)</c> → <c>1.5 kB</c></para>
    /// </summary>
    /// <param name="precision">Number of digits after the decimal place to show when rendering the quantity.</param>
    /// <param name="useBitsInsteadOfBytes"><c>true</c> to range the value to a unit based on bits, or <c>false</c> to use a unit based on bytes.</param>
    /// <param name="formatProvider">Localization settings, such as thousands separators.</param>
    /// <returns>String with the formatted data quantity and unit abbreviation, separated by a space.</returns>
    public string ToString(int precision, bool useBitsInsteadOfBytes = false, IFormatProvider? formatProvider = null) =>
        ToString(useBitsInsteadOfBytes, WithPrecision(precision, formatProvider));

    /// <summary>
    /// <para>Format as a string. The quantity is converted and formatted as a number using the current culture's numeric formatting information, such as thousands separators. The specified unit's short abbreviation is appended after a space.</para>
    /// <para><c>new DataSize(1536).ToString(1, DataSizeUnits.Kilobyte)</c> → <c>1.5 kB</c></para>
    /// </summary>
    /// <param name="precision">Number of digits after the decimal place to show when rendering the quantity.</param>
    /// <param name="unit">The unit to which this value should be converted for rendering.</param>
    /// <param name="formatProvider">Localization settings, such as thousands separators.</param>
    /// <returns>String with the formatted data quantity and unit abbreviation, separated by a space.</returns>
    public string ToString(int precision, DataSizeUnit unit, IFormatProvider? formatProvider = null) =>
        ToString(unit, WithPrecision(precision, formatProvider));

    /// <summary>
    /// <para>Format as a string. The quantity is converted and formatted as a number using the current culture's numeric formatting information, such as thousands separators and precision. The specified unit's short abbreviation is appended after a space.</para>
    /// <para><c>new DataSize(1536).ToString(DataSizeUnits.Kilobyte)</c> → <c>1.50 kB</c></para>
    /// </summary>
    /// <param name="unit">The unit to which this value should be converted for rendering.</param>
    /// <param name="formatProvider">Localization settings, such as number precision and thousands separators.</param>
    /// <returns>String with the formatted data quantity and unit abbreviation, separated by a space.</returns>
    public string ToString(DataSizeUnit unit, IFormatProvider? formatProvider = null) =>
        string.Format(formatProvider, "{0:N} {1}", AsUnit(unit), unit.ToAbbreviation());

    private static IFormatProvider WithPrecision(int precision, IFormatProvider? formatProvider) {
        formatProvider ??= CultureInfo.CurrentCulture;
        if (precision >= 0) {
            NumberFormatInfo numberFormatInfo = (NumberFormatInfo) (formatProvider.GetFormat(typeof(NumberFormatInfo)) as NumberFormatInfo ?? NumberFormatInfo.CurrentInfo).Clone();
            numberFormatInfo.NumberDecimalDigits = precision;
            formatProvider                       = numberFormatInfo;
        }
        return formatProvider;
    }

    /// <inheritdoc />
    public string ToString(string? format, IFormatProvider? formatProvider) {
        (double quantity, DataSizeUnit unit) = AsAutomaticUnit();
        return quantity.ToString(format ?? "N", formatProvider ?? CultureInfo.CurrentCulture) + ' ' + unit.ToAbbreviation();
    }

    #endregion

    #region Math operators

    /// <summary>
    /// Adds amounts of data together
    /// </summary>
    /// <param name="a">An amount of data</param>
    /// <param name="b">Another amount of data</param>
    /// <returns>The sum of the data sizes of <paramref name="a"/> and <paramref name="b"/></returns>
    public static DataSize operator +(DataSize a, DataSize b) => new(a.Bits + b.Bits);

    /// <summary>
    /// Adds amounts of data together
    /// </summary>
    /// <param name="dataSize">An amount of data</param>
    /// <param name="bytes">Number of bytes</param>
    /// <returns>The sum of the data sizes of <paramref name="dataSize"/> and <paramref name="bytes"/></returns>
    public static DataSize operator +(DataSize dataSize, BigInteger bytes) => new(dataSize.Bits + (bytes << 3));

    /// <inheritdoc cref="op_Addition(DataSizeUnits.DataSize,System.Numerics.BigInteger)" />
    public static DataSize operator +(DataSize dataSize, ulong bytes) => dataSize + ((BigInteger) bytes << 3);

    /// <inheritdoc cref="op_Addition(DataSizeUnits.DataSize,System.Numerics.BigInteger)" />
    public static DataSize operator +(DataSize dataSize, long bytes) => dataSize + ((BigInteger) bytes << 3);

    /// <inheritdoc cref="op_Addition(DataSizeUnits.DataSize,System.Numerics.BigInteger)" />
    public static DataSize operator +(BigInteger bytes, DataSize dataSize) => new(dataSize.Bits + (bytes << 3));

    /// <inheritdoc cref="op_Addition(DataSizeUnits.DataSize,System.Numerics.BigInteger)" />
    public static DataSize operator +(ulong bytes, DataSize dataSize) => dataSize + ((BigInteger) bytes << 3);

    /// <inheritdoc cref="op_Addition(DataSizeUnits.DataSize,System.Numerics.BigInteger)" />
    public static DataSize operator +(long bytes, DataSize dataSize) => dataSize + ((BigInteger) bytes << 3);

    /// <summary>
    /// Subtracts amounts of data
    /// </summary>
    /// <param name="a">An amount of data</param>
    /// <param name="b">Another amount of data</param>
    /// <returns>The difference of the data sizes of <paramref name="a"/> and <paramref name="b"/></returns>
    public static DataSize operator -(DataSize a, DataSize b) => new(a.Bits - b.Bits);

    /// <summary>
    /// Subtracts amounts of data
    /// </summary>
    /// <param name="dataSize">An amount of data</param>
    /// <param name="bytes">Number of bytes</param>
    /// <returns>The difference of the data sizes of <paramref name="dataSize"/> and <paramref name="bytes"/></returns>
    public static DataSize operator -(DataSize dataSize, BigInteger bytes) => new(dataSize.Bits - (bytes << 3));

    /// <inheritdoc cref="op_Subtraction(DataSizeUnits.DataSize,System.Numerics.BigInteger)" />
    public static DataSize operator -(DataSize dataSize, ulong bytes) => dataSize - ((BigInteger) bytes << 3);

    /// <inheritdoc cref="op_Subtraction(DataSizeUnits.DataSize,System.Numerics.BigInteger)" />
    public static DataSize operator -(DataSize dataSize, long bytes) => dataSize - ((BigInteger) bytes << 3);

    /// <summary>
    /// Subtracts amounts of data
    /// </summary>
    /// <param name="bytes">Number of bytes</param>
    /// <param name="dataSize">An amount of data</param>
    /// <returns>The difference of the data sizes of <paramref name="bytes"/> and <paramref name="dataSize"/></returns>
    public static DataSize operator -(BigInteger bytes, DataSize dataSize) => new(dataSize.Bits - (bytes << 3));

    /// <summary>
    /// Multiplies amounts of data together
    /// </summary>
    /// <param name="dataSize">An amount of data</param>
    /// <param name="coefficient">Another amount of data</param>
    /// <returns>The product of the data sizes of <paramref name="dataSize"/> and <paramref name="coefficient"/></returns>
    public static DataSize operator *(DataSize dataSize, BigInteger coefficient) => new(dataSize.Bits * coefficient);

    /// <inheritdoc cref="op_Multiply(DataSizeUnits.DataSize,System.Numerics.BigInteger)" />
    public static DataSize operator *(DataSize dataSize, long coefficient) => new(dataSize.Bits * coefficient);

    /// <inheritdoc cref="op_Multiply(DataSizeUnits.DataSize,System.Numerics.BigInteger)" />
    public static DataSize operator *(DataSize dataSize, ulong coefficient) => new(dataSize.Bits * coefficient);

    /// <inheritdoc cref="op_Multiply(DataSizeUnits.DataSize,System.Numerics.BigInteger)" />
    public static DataSize operator *(BigInteger coefficient, DataSize dataSize) => new(dataSize.Bits * coefficient);

    /// <inheritdoc cref="op_Multiply(DataSizeUnits.DataSize,System.Numerics.BigInteger)" />
    public static DataSize operator *(ulong coefficient, DataSize dataSize) => new(dataSize.Bits * coefficient);

    /// <inheritdoc cref="op_Multiply(DataSizeUnits.DataSize,System.Numerics.BigInteger)" />
    public static DataSize operator *(long coefficient, DataSize dataSize) => new(dataSize.Bits * coefficient);

    /// <summary>
    /// Divides an amount of data
    /// </summary>
    /// <param name="numerator">An amount of data as the numerator</param>
    /// <param name="denominator">Denominator</param>
    /// <returns>The quotient of the data size <paramref name="numerator"/> divided by <paramref name="denominator"/></returns>
    /// <exception cref="DivideByZeroException">if <paramref name="denominator"/> is <c>0</c></exception>
    public static DataSize operator /(DataSize numerator, BigInteger denominator) {
        if (!denominator.Equals(BigInteger.Zero)) {
            return new DataSize(numerator.Bits / denominator);
        } else {
            throw new DivideByZeroException($"Cannot divide {numerator} by zero");
        }
    }

    /// <summary>
    /// Divides an amount of data
    /// </summary>
    /// <param name="numerator">An amount of data as the numerator</param>
    /// <param name="denominator">An amount of data as the denominator</param>
    /// <returns>The quotient of the data size <paramref name="numerator"/> divided by <paramref name="denominator"/></returns>
    /// <exception cref="DivideByZeroException">if <paramref name="denominator"/> is <c>0</c></exception>
    public static double operator /(DataSize numerator, DataSize denominator) {
        if (!denominator.Bits.Equals(BigInteger.Zero)) {
            return (double) numerator.Bits / (double) denominator.Bits;
        } else {
            throw new DivideByZeroException($"Cannot divide {numerator} by zero");
        }
    }

    /// <inheritdoc cref="op_Division(DataSizeUnits.DataSize,System.Numerics.BigInteger)" />
    public static DataSize operator /(DataSize numerator, ulong denominator) => numerator / (BigInteger) denominator;

    /// <inheritdoc cref="op_Division(DataSizeUnits.DataSize,System.Numerics.BigInteger)" />
    public static DataSize operator /(DataSize numerator, long denominator) => numerator / (BigInteger) denominator;

    /// <inheritdoc cref="op_Division(DataSizeUnits.DataSize,System.Numerics.BigInteger)" />
    public static DataSize operator /(DataSize numerator, double denominator) {
        if (denominator == 0) {
            throw new DivideByZeroException($"Cannot divide {numerator} by zero");
        } else if (numerator.Bits >= ulong.MinValue && numerator.Bits <= ulong.MaxValue) {
            return new DataSize((BigInteger) Math.Round((ulong) numerator.Bits / denominator));
        } else if (numerator.Bits >= long.MinValue && numerator.Bits <= long.MaxValue) {
            return new DataSize((BigInteger) Math.Round((long) numerator.Bits / denominator));
        } else {
            return new DataSize((BigInteger) Math.Round((double) numerator.Bits / denominator));
        }
    }

    /// <inheritdoc />
    public static DataSize operator --(DataSize value) {
        BigInteger bits = value.Bits;
        return new DataSize(--bits);
    }

    /// <inheritdoc />
    public static DataSize operator ++(DataSize value) {
        BigInteger bits = value.Bits;
        return new DataSize(++bits);
    }

    /// <inheritdoc />
    public static DataSize operator %(DataSize left, DataSize right) => new(left.Bits % right.Bits);

    /// <inheritdoc />
    public static DataSize operator *(DataSize left, DataSize right) => new(left.Bits * right.Bits);

    /// <inheritdoc />
    public static DataSize operator -(DataSize value) => new(-value.Bits);

    /// <inheritdoc />
    public static DataSize operator +(DataSize value) => new(+value.Bits);

    /// <inheritdoc />
    public static DataSize Abs(DataSize value) => new(BigInteger.Abs(value.Bits));

    #endregion

    #region Equality and comparisons

    /// <summary>
    /// Compare data size equality
    /// </summary>
    /// <param name="other">Another amount of data</param>
    /// <returns><c>true</c> if this instance and <paramref name="other"/> represent the same amount of data, or <c>false</c> if they represent different amounts</returns>
    public bool Equals(DataSize other) => Bits.Equals(other.Bits);

    /// <inheritdoc cref="Equals(DataSizeUnits.DataSize)" />
    public override bool Equals(object? other) => other is DataSize other2 && Equals(other2);

    /// <inheritdoc cref="Equals(DataSizeUnits.DataSize)" />
    public static bool operator ==(DataSize a, DataSize b) => a.Equals(b);

    /// <inheritdoc cref="Equals(DataSizeUnits.DataSize)" />
    public static bool operator !=(DataSize a, DataSize b) => !a.Equals(b);

    /// <inheritdoc cref="Double.GetHashCode"/>
    public override int GetHashCode() => Bits.GetHashCode();

    /// <summary>
    /// Compares two <see cref="DataSize"/> instances
    /// </summary>
    /// <param name="other">Another amount of data</param>
    /// <returns>A number &lt; 0 if this instance is smaller than <paramref name="other"/>, <c>0</c> if they are equal, or a number &gt; 0 if this instance is larger than <paramref name="other"/></returns>
    public int CompareTo(DataSize other) => Bits.CompareTo(other.Bits);

    /// <inheritdoc cref="IComparable.CompareTo" />
    public int CompareTo(object? other) => other switch {
        null            => 1,
        DataSize other2 => CompareTo(other2),
        _               => throw new ArgumentException($"Object must be of type {nameof(DataSize)}")
    };

    /// <summary>
    /// Compare two amounts of data
    /// </summary>
    /// <param name="a">An amount of data</param>
    /// <param name="b">Another amount of data</param>
    /// <returns><c>true</c> if <paramref name="a"/> represents less data than <paramref name="b"/>, or <c>false</c> if <paramref name="a"/> represents either more than or the same amount of data as <paramref name="b"/></returns>
    public static bool operator <(DataSize a, DataSize b) => a.Bits < b.Bits;

    /// <summary>
    /// Compare two amounts of data
    /// </summary>
    /// <param name="a">An amount of data</param>
    /// <param name="b">Another amount of data</param>
    /// <returns><c>true</c> if <paramref name="a"/> represents more data than <paramref name="b"/>, or <c>false</c> if <paramref name="a"/> represents either less than or the same amount of data as <paramref name="b"/></returns>
    public static bool operator >(DataSize a, DataSize b) => a.Bits > b.Bits;

    /// <summary>
    /// Compare two amounts of data
    /// </summary>
    /// <param name="a">An amount of data</param>
    /// <param name="b">Another amount of data</param>
    /// <returns><c>true</c> if <paramref name="a"/> represents either less than or the same amount of data as <paramref name="b"/>, or <c>false</c> if <paramref name="a"/> represents more data than <paramref name="b"/></returns>
    public static bool operator <=(DataSize a, DataSize b) => a.Bits <= b.Bits;

    /// <summary>
    /// Compare two amounts of data
    /// </summary>
    /// <param name="a">An amount of data</param>
    /// <param name="b">Another amount of data</param>
    /// <returns><c>true</c> if <paramref name="a"/> represents either more than or the same amount of data as <paramref name="b"/>, or <c>false</c> if <paramref name="a"/> represents less data than <paramref name="b"/></returns>
    public static bool operator >=(DataSize a, DataSize b) => a.Bits >= b.Bits;

    /// <summary>
    /// Byte equality check.
    /// </summary>
    /// <param name="dataSize">Data size.</param>
    /// <param name="bytes">Number of bytes.</param>
    /// <returns><c>true</c> if <paramref name="dataSize"/> and <paramref name="bytes"/> represent the same number of bytes, or <c>false</c> otherwise.</returns>
    public static bool operator ==(DataSize dataSize, long bytes) => dataSize.Bytes == bytes;

    /// <summary>
    /// Byte inequality check.
    /// </summary>
    /// <param name="dataSize">Data size.</param>
    /// <param name="bytes">Number of bytes.</param>
    /// <returns><c>false</c> if <paramref name="dataSize"/> and <paramref name="bytes"/> represent the same number of bytes, or <c>true</c> otherwise.</returns>
    public static bool operator !=(DataSize dataSize, long bytes) => dataSize.Bytes != bytes;

    /// <summary>
    /// Byte comparison.
    /// </summary>
    /// <param name="dataSize">Data size.</param>
    /// <param name="bytes">Number of bytes.</param>
    /// <returns><c>true</c> if <paramref name="dataSize"/> represents fewer bytes than <paramref name="bytes"/>, or <c>false</c> otherwise.</returns>
    public static bool operator <(DataSize dataSize, long bytes) => dataSize.Bytes < bytes;

    /// <summary>
    /// Byte comparison.
    /// </summary>
    /// <param name="dataSize">Data size.</param>
    /// <param name="bytes">Number of bytes.</param>
    /// <returns><c>true</c> if <paramref name="dataSize"/> represents more bytes than <paramref name="bytes"/>, or <c>false</c> otherwise.</returns>
    public static bool operator >(DataSize dataSize, long bytes) => dataSize.Bytes > bytes;

    /// <summary>
    /// Byte comparison.
    /// </summary>
    /// <param name="dataSize">Data size.</param>
    /// <param name="bytes">Number of bytes.</param>
    /// <returns><c>true</c> if <paramref name="dataSize"/> represents the same or fewer bytes than <paramref name="bytes"/>, or <c>false</c> otherwise.</returns>
    public static bool operator <=(DataSize dataSize, long bytes) => dataSize.Bytes <= bytes;

    /// <summary>
    /// Byte comparison.
    /// </summary>
    /// <param name="dataSize">Data size.</param>
    /// <param name="bytes">Number of bytes.</param>
    /// <returns><c>true</c> if <paramref name="dataSize"/> represents the same or more bytes than <paramref name="bytes"/>, or <c>false</c> otherwise.</returns>
    public static bool operator >=(DataSize dataSize, long bytes) => dataSize.Bytes >= bytes;

    /// <summary>
    /// Byte equality check.
    /// </summary>
    /// <param name="bytes">Number of bytes.</param>
    /// <param name="dataSize">Data size.</param>
    /// <returns><c>true</c> if <paramref name="bytes"/> and <paramref name="dataSize"/> represent the same number of bytes, or <c>false</c> otherwise.</returns>
    public static bool operator ==(long bytes, DataSize dataSize) => bytes == dataSize.Bytes;

    /// <summary>
    /// Byte inequality check.
    /// </summary>
    /// <param name="bytes">Number of bytes.</param>
    /// <param name="dataSize">Data size.</param>
    /// <returns><c>false</c> if <paramref name="dataSize"/> and <paramref name="bytes"/> represent the same number of bytes, or <c>true</c> otherwise.</returns>
    public static bool operator !=(long bytes, DataSize dataSize) => bytes != dataSize.Bytes;

    /// <summary>
    /// Byte comparison.
    /// </summary>
    /// <param name="bytes">Number of bytes.</param>
    /// <param name="dataSize">Data size.</param>
    /// <returns><c>true</c> if <paramref name="bytes"/> represents fewer bytes than <paramref name="dataSize"/>, or <c>false</c> otherwise.</returns>
    public static bool operator <(long bytes, DataSize dataSize) => bytes < dataSize.Bytes;

    /// <summary>
    /// Byte comparison.
    /// </summary>
    /// <param name="bytes">Number of bytes.</param>
    /// <param name="dataSize">Data size.</param>
    /// <returns><c>true</c> if <paramref name="bytes"/> represents more bytes than <paramref name="dataSize"/>, or <c>false</c> otherwise.</returns>
    public static bool operator >(long bytes, DataSize dataSize) => bytes > dataSize.Bytes;

    /// <summary>
    /// Byte comparison.
    /// </summary>
    /// <param name="bytes">Number of bytes.</param>
    /// <param name="dataSize">Data size.</param>
    /// <returns><c>true</c> if <paramref name="bytes"/> represents the same or fewer bytes than <paramref name="dataSize"/>, or <c>false</c> otherwise.</returns>
    public static bool operator <=(long bytes, DataSize dataSize) => bytes <= dataSize.Bytes;

    /// <summary>
    /// Byte comparison.
    /// </summary>
    /// <param name="bytes">Number of bytes.</param>
    /// <param name="dataSize">Data size.</param>
    /// <returns><c>true</c> if <paramref name="bytes"/> represents the same or more bytes than <paramref name="dataSize"/>, or <c>false</c> otherwise.</returns>
    public static bool operator >=(long bytes, DataSize dataSize) => bytes >= dataSize.Bytes;

    /// <inheritdoc cref="op_Equality(DataSizeUnits.DataSize,long)" />
    public static bool operator ==(DataSize dataSize, ulong bytes) => dataSize.Bytes == bytes;

    /// <inheritdoc cref="op_Inequality(DataSizeUnits.DataSize,long)" />
    public static bool operator !=(DataSize dataSize, ulong bytes) => dataSize.Bytes != bytes;

    /// <inheritdoc cref="op_LessThan(DataSizeUnits.DataSize,long)" />
    public static bool operator <(DataSize dataSize, ulong bytes) => dataSize.Bytes < bytes;

    /// <inheritdoc cref="op_GreaterThan(DataSizeUnits.DataSize,long)" />
    public static bool operator >(DataSize dataSize, ulong bytes) => dataSize.Bytes > bytes;

    /// <inheritdoc cref="op_LessThanOrEqual(DataSizeUnits.DataSize,long)" />
    public static bool operator <=(DataSize dataSize, ulong bytes) => dataSize.Bytes <= bytes;

    /// <inheritdoc cref="op_GreaterThanOrEqual(DataSizeUnits.DataSize,long)" />
    public static bool operator >=(DataSize dataSize, ulong bytes) => dataSize.Bytes >= bytes;

    /// <inheritdoc cref="op_Equality(long,DataSizeUnits.DataSize)" />
    public static bool operator ==(ulong bytes, DataSize dataSize) => bytes == dataSize.Bytes;

    /// <inheritdoc cref="op_Inequality(long,DataSizeUnits.DataSize)" />
    public static bool operator !=(ulong bytes, DataSize dataSize) => bytes != dataSize.Bytes;

    /// <inheritdoc cref="op_LessThan(long,DataSizeUnits.DataSize)" />
    public static bool operator <(ulong bytes, DataSize dataSize) => bytes < dataSize.Bytes;

    /// <inheritdoc cref="op_GreaterThan(long,DataSizeUnits.DataSize)" />
    public static bool operator >(ulong bytes, DataSize dataSize) => bytes > dataSize.Bytes;

    /// <inheritdoc cref="op_LessThanOrEqual(long,DataSizeUnits.DataSize)" />
    public static bool operator <=(ulong bytes, DataSize dataSize) => bytes <= dataSize.Bytes;

    /// <inheritdoc cref="op_GreaterThanOrEqual(long,DataSizeUnits.DataSize)" />
    public static bool operator >=(ulong bytes, DataSize dataSize) => bytes >= dataSize.Bytes;

    /// <summary>
    /// Bit equality check.
    /// </summary>
    /// <param name="dataSize">Data size.</param>
    /// <param name="bits">Number of bits.</param>
    /// <returns><c>true</c> if <paramref name="dataSize"/> and <paramref name="bits"/> represent the same number of bits, or <c>false</c> otherwise.</returns>
    public static bool operator ==(DataSize dataSize, BigInteger bits) => dataSize.Bits == bits;

    /// <summary>
    /// Bit inequality check.
    /// </summary>
    /// <param name="dataSize">Data size.</param>
    /// <param name="bits">Number of bits.</param>
    /// <returns><c>false</c> if <paramref name="dataSize"/> and <paramref name="bits"/> represent the same number of bits, or <c>true</c> otherwise.</returns>
    public static bool operator !=(DataSize dataSize, BigInteger bits) => dataSize.Bits != bits;

    /// <summary>
    /// Bit comparison.
    /// </summary>
    /// <param name="dataSize">Data size.</param>
    /// <param name="bits">Number of bits.</param>
    /// <returns><c>true</c> if <paramref name="dataSize"/> represents fewer bits than <paramref name="bits"/>, or <c>false</c> otherwise.</returns>
    public static bool operator <(DataSize dataSize, BigInteger bits) => dataSize.Bits < bits;

    /// <summary>
    /// Bit comparison.
    /// </summary>
    /// <param name="dataSize">Data size.</param>
    /// <param name="bits">Number of bits.</param>
    /// <returns><c>true</c> if <paramref name="dataSize"/> represents more bits than <paramref name="bits"/>, or <c>false</c> otherwise.</returns>
    public static bool operator >(DataSize dataSize, BigInteger bits) => dataSize.Bits > bits;

    /// <summary>
    /// Bit comparison.
    /// </summary>
    /// <param name="dataSize">Data size.</param>
    /// <param name="bits">Number of bits.</param>
    /// <returns><c>true</c> if <paramref name="dataSize"/> represents the same or fewer bits than <paramref name="bits"/>, or <c>false</c> otherwise.</returns>
    public static bool operator <=(DataSize dataSize, BigInteger bits) => dataSize.Bits <= bits;

    /// <summary>
    /// Bit comparison.
    /// </summary>
    /// <param name="dataSize">Data size.</param>
    /// <param name="bits">Number of bits.</param>
    /// <returns><c>true</c> if <paramref name="dataSize"/> represents the same or more bits than <paramref name="bits"/>, or <c>false</c> otherwise.</returns>
    public static bool operator >=(DataSize dataSize, BigInteger bits) => dataSize.Bits >= bits;

    /// <summary>
    /// Bit equality check.
    /// </summary>
    /// <param name="bits">Number of bits.</param>
    /// <param name="dataSize">Data size.</param>
    /// <returns><c>true</c> if <paramref name="bits"/> and <paramref name="dataSize"/> represent the same number of bits, or <c>false</c> otherwise.</returns>
    public static bool operator ==(BigInteger bits, DataSize dataSize) => bits == dataSize.Bits;

    /// <summary>
    /// Bit inequality check.
    /// </summary>
    /// <param name="bits">Number of bits.</param>
    /// <param name="dataSize">Data size.</param>
    /// <returns><c>false</c> if <paramref name="dataSize"/> and <paramref name="bits"/> represent the same number of bits, or <c>true</c> otherwise.</returns>
    public static bool operator !=(BigInteger bits, DataSize dataSize) => bits != dataSize.Bits;

    /// <summary>
    /// Bit comparison.
    /// </summary>
    /// <param name="bits">Number of bits.</param>
    /// <param name="dataSize">Data size.</param>
    /// <returns><c>true</c> if <paramref name="bits"/> represents fewer bits than <paramref name="dataSize"/>, or <c>false</c> otherwise.</returns>
    public static bool operator <(BigInteger bits, DataSize dataSize) => bits < dataSize.Bits;

    /// <summary>
    /// Bit comparison.
    /// </summary>
    /// <param name="bits">Number of bits.</param>
    /// <param name="dataSize">Data size.</param>
    /// <returns><c>true</c> if <paramref name="bits"/> represents more bits than <paramref name="dataSize"/>, or <c>false</c> otherwise.</returns>
    public static bool operator >(BigInteger bits, DataSize dataSize) => bits > dataSize.Bits;

    /// <summary>
    /// Bit comparison.
    /// </summary>
    /// <param name="bits">Number of bits.</param>
    /// <param name="dataSize">Data size.</param>
    /// <returns><c>true</c> if <paramref name="bits"/> represents the same or fewer bits than <paramref name="dataSize"/>, or <c>false</c> otherwise.</returns>
    public static bool operator <=(BigInteger bits, DataSize dataSize) => bits <= dataSize.Bits;

    /// <summary>
    /// Bit comparison.
    /// </summary>
    /// <param name="bits">Number of bits.</param>
    /// <param name="dataSize">Data size.</param>
    /// <returns><c>true</c> if <paramref name="bits"/> represents the same or more bits than <paramref name="dataSize"/>, or <c>false</c> otherwise.</returns>
    public static bool operator >=(BigInteger bits, DataSize dataSize) => bits >= dataSize.Bits;

    #endregion

    #region Casting

    /// <summary>
    /// Explicitly cast a <see cref="DataSize"/> instance to a <see cref="long"/> number of bytes.
    /// </summary>
    /// <param name="dataSize">An amount of data.</param>
    /// <returns>The number of bytes in <paramref name="dataSize"/>.</returns>
    public static explicit operator long(DataSize dataSize) => (long) (dataSize.Bits / 8);

    /// <summary>
    /// Explicitly cast a <see cref="DataSize"/> instance to a <see cref="ulong"/> number of bytes.
    /// </summary>
    /// <param name="dataSize">An amount of data.</param>
    /// <returns>The number of bytes in <paramref name="dataSize"/>.</returns>
    public static explicit operator ulong(DataSize dataSize) => (ulong) (dataSize.Bits / 8);

    /// <summary>
    /// Explicitly cast a <see cref="long"/> to a <see cref="DataSize"/> that represents the original value's number of bytes.
    /// </summary>
    /// <param name="bytes">Number of bytes to be represented.</param>
    /// <returns>A <see cref="DataSize"/> value that represents <paramref name="bytes"/> number of bytes.</returns>
    public static implicit operator DataSize(long bytes) => new(bytes);

    /// <summary>
    /// Explicitly cast an <see cref="int"/> to a <see cref="DataSize"/> that represents the original value's number of bytes.
    /// </summary>
    /// <param name="bytes">Number of bytes to be represented.</param>
    /// <returns>A <see cref="DataSize"/> value that represents <paramref name="bytes"/> number of bytes.</returns>
    public static implicit operator DataSize(int bytes) => new(bytes);

    /// <summary>
    /// Explicitly cast a <see cref="uint"/> to a <see cref="DataSize"/> that represents the original value's number of bytes.
    /// </summary>
    /// <param name="bytes">Number of bytes to be represented.</param>
    /// <returns>A <see cref="DataSize"/> value that represents <paramref name="bytes"/> number of bytes.</returns>
    public static implicit operator DataSize(uint bytes) => new(bytes);

    /// <summary>
    /// Explicitly cast a <see cref="ulong"/> to a <see cref="DataSize"/> that represents the original value's number of bytes.
    /// </summary>
    /// <param name="bytes">Number of bytes to be represented.</param>
    /// <returns>A <see cref="DataSize"/> value that represents <paramref name="bytes"/> number of bytes.</returns>
    public static implicit operator DataSize(ulong bytes) => new(bytes);

    /// <summary>
    /// Explicitly cast a <see cref="short"/> to a <see cref="DataSize"/> that represents the original value's number of bytes.
    /// </summary>
    /// <param name="bytes">Number of bytes to be represented.</param>
    /// <returns>A <see cref="DataSize"/> value that represents <paramref name="bytes"/> number of bytes.</returns>
    public static implicit operator DataSize(short bytes) => new(bytes);

    /// <summary>
    /// Explicitly cast a <see cref="ushort"/> to a <see cref="DataSize"/> that represents the original value's number of bytes.
    /// </summary>
    /// <param name="bytes">Number of bytes to be represented.</param>
    /// <returns>A <see cref="DataSize"/> value that represents <paramref name="bytes"/> number of bytes.</returns>
    public static implicit operator DataSize(ushort bytes) => new(bytes);

    /// <summary>
    /// Explicitly cast a <see cref="byte"/> to a <see cref="DataSize"/> that represents the original value's number of bytes.
    /// </summary>
    /// <param name="bytes">Number of bytes to be represented.</param>
    /// <returns>A <see cref="DataSize"/> value that represents <paramref name="bytes"/> number of bytes.</returns>
    public static implicit operator DataSize(byte bytes) => new(bytes);

    /// <summary>
    /// Explicitly cast a <see cref="byte"/> to a <see cref="DataSize"/> that represents the original value's number of bytes.
    /// </summary>
    /// <param name="bytes">Number of bytes to be represented.</param>
    /// <returns>A <see cref="DataSize"/> value that represents <paramref name="bytes"/> number of bytes.</returns>
    public static implicit operator DataSize(sbyte bytes) => new(bytes);

    #endregion

    #region Parsing

    /// <summary>
    /// <para>Parses a <see cref="DataSize"/> from a string that consists of a numeric quantity and a unit.</para>
    /// </summary>
    /// <param name="s">String of the form <c>123.456 MB</c>. The number can be floating-point, integral, positive, or negative. If the unit is missing, bytes are assumed.</param>
    /// <param name="formatProvider">Localization settings.</param>
    /// <returns>The parsed <see cref="DataSize"/> value if the parsing succeeded.</returns>
    /// <exception cref="FormatException"><paramref name="s"/> failed to parse as a quantity and unit of data.</exception>
    public static DataSize Parse(string s, IFormatProvider? formatProvider = null) {
        if (TryParse(s, formatProvider, out DataSize dataSize)) {
            return dataSize;
        } else {
            throw new FormatException($"Could not parse \"{s}\" to a {nameof(DataSize)}, it was not in the format \"512 kB\"");
        }
    }

    /// <summary>
    /// <para>Parses a <see cref="DataSize"/> from a string that consists of a numeric quantity and a unit.</para>
    /// </summary>
    /// <param name="s">String of the form <c>123.456 MB</c>. The number can be floating-point, integral, positive, or negative. If the unit is missing, bytes are assumed.</param>
    /// <param name="formatProvider">Localization settings.</param>
    /// <param name="result">The parsed <see cref="DataSize"/> value if the parsing succeeded, or <c>new DataSize()</c> if it failed.</param>
    /// <returns><c>true</c> if the parsing succeeded, or <c>false</c> if it failed.</returns>
    public static bool TryParse(
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        [NotNullWhen(true)]
#endif
        string? s, IFormatProvider? formatProvider, out DataSize result) {
        return TryParse(s, NumberStyles.Integer, formatProvider, out result);
    }

    /// <inheritdoc cref="Parse(string,IFormatProvider)" />
    /// <param name="style">Number style settings.</param>
    public static DataSize Parse(string s, NumberStyles style, IFormatProvider? formatProvider = null) {
        if (TryParse(s, style, formatProvider, out DataSize dataSize)) {
            return dataSize;
        } else {
            throw new FormatException($"Could not parse \"{s}\" to a {nameof(DataSize)}, it was not in the format \"512 kB\"");
        }
    }

#if NET8_0_OR_GREATER
    /// <inheritdoc cref="Parse(string,IFormatProvider)" />
    public static DataSize Parse(ReadOnlySpan<char> s, IFormatProvider? formatProvider = null) {
        if (TryParse(s, formatProvider, out DataSize dataSize)) {
            return dataSize;
        } else {
            throw new FormatException($"Could not parse \"{s}\" to a {nameof(DataSize)}, it was not in the format \"512 kB\"");
        }
    }

    /// <inheritdoc cref="TryParse(string?,System.IFormatProvider?,out DataSizeUnits.DataSize)" />
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? formatProvider, out DataSize result) {
        return TryParse(s, NumberStyles.Integer, formatProvider, out result);
    }

    /// <inheritdoc cref="Parse(string,IFormatProvider)" />
    /// <param name="style">Number style settings.</param>
    public static DataSize Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? formatProvider) {
        if (TryParse(s, style, formatProvider, out DataSize dataSize)) {
            return dataSize;
        } else {
            throw new FormatException($"Could not parse \"{s}\" to a {nameof(DataSize)}, it was not in the format \"512 kB\"");
        }
    }

    /// <inheritdoc cref="TryParse(string?,System.IFormatProvider?,out DataSizeUnits.DataSize)" />
    /// <param name="style">Number style settings.</param>
    public static bool TryParse(
        [NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? formatProvider, out DataSize result) {
        if (s is not null) {
            return TryParse(s.AsSpan(), style, formatProvider, out result);
        } else {
            result = new DataSize(0);
            return false;
        }
    }

    /// <inheritdoc cref="TryParse(string?,System.IFormatProvider?,out DataSizeUnits.DataSize)" />
    /// <param name="style">Number style settings.</param>
    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? formatProvider, out DataSize result) {
        result = new DataSize(0);
        int                whitespaceStart = s.IndexOfAny(WhitespaceSearchValues);
        ReadOnlySpan<char> left            = (whitespaceStart == -1 ? s : s[..whitespaceStart]).Trim();

        BigInteger? integerBits  = null;
        double      floatingBits = 0;
        if (left.Contains('.')) {
            if (!double.TryParse(left, formatProvider, out floatingBits)) return false;
        } else {
            if (!BigInteger.TryParse(left, style, formatProvider, out BigInteger bits)) return false;
            integerBits = bits;
        }

        DataSizeUnit unit = (whitespaceStart != -1 ? DataSizeUnit.Parse(s[whitespaceStart..].Trim().ToString()) : null) ?? DataSizeUnit.Byte;

        result = integerBits is null ? new DataSize(floatingBits, unit) : new DataSize(integerBits.Value, unit);
        return true;
    }

#else
    /// <inheritdoc cref="TryParse(string?,System.IFormatProvider?,out DataSizeUnits.DataSize)" />
    /// <param name="style">Number style settings.</param>
    public static bool TryParse(string? s, NumberStyles style, IFormatProvider? formatProvider, out DataSize result) {
        result = new DataSize(0);
        if (s is null) return false;

        int    whitespaceStart = s.IndexOfAny(Whitespace);
        string left = (whitespaceStart == -1 ? s : s.Substring(0, whitespaceStart)).Trim();

        BigInteger? integerBits = null;
        double      floatingBits = 0;
        if (left.Contains('.')) {
            if (!double.TryParse(left, NumberStyles.Float | NumberStyles.AllowThousands, formatProvider, out floatingBits)) return false;
        } else {
            if (!BigInteger.TryParse(left, style, formatProvider, out BigInteger bits)) return false;
            integerBits = bits;
        }

        DataSizeUnit unit = (whitespaceStart != -1 ? DataSizeUnit.Parse(s.Substring(whitespaceStart).Trim()) : null) ?? DataSizeUnit.Byte;

        result = integerBits is null ? new DataSize(floatingBits, unit) : new DataSize(integerBits.Value, unit);
        return true;
    }

#endif

    #endregion

}