using System;
using System.Text.RegularExpressions;

namespace DataSizeUnits;

/// <summary>
/// <para>An <see cref="IFormatProvider"/> for formatting bytes with unit conversion, numeric precision, and unit abbreviation. The input value is always a <see cref="long"/> of bytes.</para>
/// <para>You can also use the equivalent <see cref="DataSize.ToString(int,bool,IFormatProvider)"/> if you prefer to call a method on an existing <see cref="DataSize"/> instance.</para>
/// <para>The format string (e.g. <c>A0</c>) controls the unit and precision of the output string, and it is case-sensitive to distinguish between bit and byte units. It consists of a left (<c>A</c>) and right (<c>0</c>) side, both of which are optional.</para>
/// <para>The left side of the format string is the desired data size unit, such as <c>MB</c> for megabytes. It can be any unit abbreviation or name, from <c>B</c> to <c>Petabyte</c> and <c>b</c> to <c>petabit</c>. You can also supply <c>A</c> to have the formatter automatically normalize the data size to the best fit unit of bytes, or <c>a</c> to normalize to the best fit unit of bits. If this is omitted, it will default to <c>A</c>.</para>
/// <para>The right side of the format is the numeric precision of the output. It can be any non-negative integer, representing the number of digits after the decimal point. You can supply <c>0</c> to output an integer without any decimal point. Precision reduction uses the same rounding rules as other numeric formatting operations in .NET. If this is omitted, it defaults to the current culture's numeric precision, which is 2 for en-US.</para>
/// <para>In the below examples, the data size is 1,048,576 bytes.</para>
/// <para>Example: <code>string.format(new DataSizeFormatter(), "{0:A0}", 1_048_576); // 1 MB</code></para>
/// <list type="table">
/// <listheader><term>Format string</term><description>Example output - description</description></listheader>
/// <item><term><c>A</c></term><description>1.00 MB - convert to bytes and automatically normalize</description></item>
/// <item><term><c>A0</c></term><description>1 MB - convert to bytes and automatically normalize with 0 precision</description></item>
/// <item><term><c>a</c></term><description>8.39 mb - convert to bits and automatically normalize</description></item>
/// <item><term><c>B0</c></term><description>1,048,576 B - convert to bytes with 0 precision</description></item>
/// <item><term><c>b0</c></term><description>8,388,608 b - convert to bits with 0 precision</description></item>
/// <item><term><c>KB</c></term><description>1024.00 KB - convert to kilobytes</description></item>
/// <item><term><c>kb1</c></term><description>8388.6 kb - convert to kilobits with 1 digit of precision</description></item>
/// <item><term><c>MB</c></term><description>1.00 MB - convert to megabytes</description></item>
/// <item><term><c>mb</c></term><description>8.39 mb - convert to megabits</description></item>
/// </list>
/// </summary>
/// <example>
/// var fileSize = 1474560;
/// string.format(new DataSizeFormatter(), "{0:A1}", fileSize); // 1.4 MB
/// string.format(new DataSizeFormatter(), "{0:A}", fileSize); // 1.41 MB (default culture precision)
/// string.format(new DataSizeFormatter(), "{0:KB0}", fileSize); // 1,440 KB
/// </example>
/// <seealso cref="DataSize.ToString(int,bool,IFormatProvider)" />
/// <seealso cref="DataSize.ToString(int,Unit)" />
/// <seealso cref="DataSize.ToString(string,IFormatProvider)" />
public class DataSizeFormatter: IFormatProvider, ICustomFormatter {

    private const string DefaultFormat = "A";

    private readonly bool _handleTypesBesidesDataSize;

    public static readonly DataSizeFormatter Instance = new();

    /// <summary>
    /// Create an <see cref="IFormatProvider"/> that can format <see cref="DataSize"/> values in strings.
    /// </summary>
    /// <param name="handleTypesBesidesDataSize"><see langword="true" /> (default) to make this formatter try to convert any argument value that is not a <see cref="DataSize"/> (e.g. an <see langword="int" />) into a <see langword="long" /> of bytes and format it, or <see langword="false" /> to allow other formatters to handle it instead.</param>
    public DataSizeFormatter(bool handleTypesBesidesDataSize = true) {
        _handleTypesBesidesDataSize = handleTypesBesidesDataSize;
    }

    public object? GetFormat(Type? formatType) {
        return formatType == typeof(ICustomFormatter) ? this : null;
    }

    /// <exception cref="FormatException"></exception>
    public string Format(string? format, object? arg, IFormatProvider formatProvider) {
        if (string.IsNullOrEmpty(format)) {
            format = DefaultFormat;
        }

        DataSize dataSize;
        if (arg is DataSize size) {
            dataSize = size;
        } else if (_handleTypesBesidesDataSize) {
            dataSize.Unit = Unit.Byte;
            try {
                long bytes = Convert.ToInt64(arg);
                dataSize.Quantity = bytes;
            } catch (Exception) {
                return HandleOtherFormats(format, arg);
            }
        } else {
            return HandleOtherFormats(format, arg);
        }

        string unitString = Regex.Match(format, @"^[a-z]+", RegexOptions.IgnoreCase).Value;
        if (string.IsNullOrEmpty(unitString)) {
            unitString = DefaultFormat;
        }

        if (!int.TryParse(Regex.Match(format, @"\d+$").Value, out int precision)) {
            precision = -1;
        }

        if (unitString.ToLowerInvariant() == "a") {
            return dataSize.Normalize(unitString == "a").ToString(precision);
        } else {
            try {
                return dataSize.ConvertToUnit(DataSize.ParseUnit(unitString)).ToString(precision, false, formatProvider);
            } catch (ArgumentOutOfRangeException) {
                return HandleOtherFormats(format, arg);
            }
        }
    }

    /// <exception cref="FormatException"></exception>
    private static string HandleOtherFormats(string? format, object? arg) {
        try {
            if (arg is IFormattable formattable) {
                return formattable.ToString(format, null);
            } else if (arg != null) {
                return arg.ToString();
            } else {
                return string.Empty;
            }
        } catch (FormatException e) {
            throw new FormatException($"The format of '{format}' is invalid.", e);
        }
    }

}