namespace DataSizeUnits;

/// <summary>
/// <para>Orders of magnitude of data, from bit and byte to exabit and exabyte.</para>
/// <para>Kilobits and other *bits units are multiples of 1000 of the next smaller unit. For example, a megabit is 1,000,000 bits (1000 * 1000).</para>
/// <para>Kilobytes and other *bytes units are multiples of 1024 of the next smaller unit. For example, a megabyte is 1,048,576 bytes (1024 * 1024).</para>
/// </summary>
public enum Unit {

    /// <summary>
    /// 1 bit
    /// </summary>
    Bit,

    /// <summary>
    /// 8 bits
    /// </summary>
    Byte,

    /// <summary>
    /// 1000 bits
    /// </summary>
    Kilobit,

    /// <summary>
    /// 1024 bytes
    /// </summary>
    Kilobyte,

    /// <summary>
    /// 1000 kilobits, or 1,000,000 bits
    /// </summary>
    Megabit,

    /// <summary>
    /// 1024 kilobytes, or 1,048,576 bytes
    /// </summary>
    Megabyte,

    /// <summary>
    /// 1000 megabits, or 1,000,000,000 bits
    /// </summary>
    Gigabit,

    /// <summary>
    /// 1024 megabytes, or 1,073,741,824 bytes
    /// </summary>
    Gigabyte,

    /// <summary>
    /// 1000 gigabits, or 1,000,000,000,000 bits
    /// </summary>
    Terabit,

    /// <summary>
    /// 1024 gigabytes, or 1,099,511,627,776 bytes
    /// </summary>
    Terabyte,

    /// <summary>
    /// 1000 terabits, or 1,000,000,000,000,000 bits
    /// </summary>
    Petabit,

    /// <summary>
    /// 1024 terabytes, or 1,125,899,906,842,624 bytes
    /// </summary>
    Petabyte,

    /// <summary>
    /// 1000 petabits, or 1,000,000,000,000,000,000 bits
    /// </summary>
    Exabit,

    /// <summary>
    /// 1024 petabytes, or 1,152,921,504,606,846,976 bytes
    /// </summary>
    Exabyte

}

/// <summary>
/// Methods on instances of the <see cref="Unit"/> enum.
/// </summary>
public static class UnitExtensions {

    /// <summary>Get the short version of this unit's name (1-3 characters), such as <c>MB</c>.</summary>
    /// <param name="unit">the unit of data size</param>
    /// <param name="iec"><c>true</c> to return the IEC abbreviation (KiB, MiB, etc.), or <c>false</c> (the default) to return
    /// the JEDEC abbreviation (KB, MB, etc.)</param>
    /// <returns>The abbreviation for this unit.</returns>
    /// <exception cref="ArgumentOutOfRangeException">if <paramref name="unit"/> was force-cast from a fake value</exception>
    public static string ToAbbreviation(this Unit unit, bool iec = false) => unit switch {
        Unit.Byte     => "B",
        Unit.Kilobyte => iec ? "KiB" : "KB",
        Unit.Megabyte => iec ? "MiB" : "MB",
        Unit.Gigabyte => iec ? "GiB" : "GB",
        Unit.Terabyte => iec ? "TiB" : "TB",
        Unit.Petabyte => iec ? "PiB" : "PB",
        Unit.Exabyte  => iec ? "EiB" : "EB",
        Unit.Bit      => "b",
        Unit.Kilobit  => "kb",
        Unit.Megabit  => "mb",
        Unit.Gigabit  => "gb",
        Unit.Terabit  => "tb",
        Unit.Petabit  => "pb",
        Unit.Exabit   => "eb",
        _             => throw new ArgumentOutOfRangeException(nameof(unit), unit, null)
    };

    /// <summary>
    /// Get the long version of this unit's name, such as <c>megabyte</c>.
    /// </summary>
    /// <param name="unit">the unit of data size</param>
    /// <param name="iec"><c>true</c> to return the IEC name (kibibyte, mebibyte, etc.), or <c>false</c> (the default) to return
    /// the JEDEC name (kilobyte, megabyte, etc.)</param>
    /// <exception cref="ArgumentOutOfRangeException">illegal value of <paramref name="unit"/> from force casting</exception>
    /// <returns>The name of this unit.</returns>
    public static string ToName(this Unit unit, bool iec = false) => unit switch {
        Unit.Byte     => "byte",
        Unit.Kilobyte => iec ? "kibibyte" : "kilobyte",
        Unit.Megabyte => iec ? "mebibyte" : "megabyte",
        Unit.Gigabyte => iec ? "gibibyte" : "gigabyte",
        Unit.Terabyte => iec ? "tebibyte" : "terabyte",
        Unit.Petabyte => iec ? "pebibyte" : "petabyte",
        Unit.Exabyte  => iec ? "exbibyte" : "exabyte",
        Unit.Bit      => "bit",
        Unit.Kilobit  => "kilobit",
        Unit.Megabit  => "megabit",
        Unit.Gigabit  => "gigabit",
        Unit.Terabit  => "terabit",
        Unit.Petabit  => "petabit",
        Unit.Exabit   => "exabit",
        _             => throw new ArgumentOutOfRangeException(nameof(unit), unit, null)
    };

    /// <summary>
    /// Determine whether a data size <see cref="Unit"/> is based on bits or bytes.
    /// </summary>
    /// <param name="unit">the unit of data size</param>
    /// <returns><c>true</c> if <paramref name="unit"/> is based on bits, or <c>false</c> if it is based on bytes</returns>
    /// <exception cref="ArgumentOutOfRangeException">illegal value of <paramref name="unit"/> from force casting</exception>
    public static bool IsMultipleOfBits(this Unit unit) => unit switch {
        Unit.Byte
            or Unit.Kilobyte
            or Unit.Megabyte
            or Unit.Gigabyte
            or Unit.Terabyte
            or Unit.Petabyte
            or Unit.Exabyte => false,
        Unit.Bit
            or Unit.Kilobit
            or Unit.Megabit
            or Unit.Gigabit
            or Unit.Terabit
            or Unit.Petabit
            or Unit.Exabit => true,
        _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, null)
    };

    /// <summary>
    /// Construct an instance of <see cref="DataSize"/> based on this <see cref="Unit"/> and the given <paramref name="quantity"/> of that unit.
    /// </summary>
    /// <param name="unit">the unit of data size</param>
    /// <param name="quantity">how many of <paramref name="unit"/> to represent</param>
    /// <returns>a <see cref="DataSize"/> instance with the given <paramref name="unit"/> and <paramref name="quantity"/></returns>
    public static DataSize Quantity(this Unit unit, double quantity) {
        return new DataSize(quantity, unit);
    }

}