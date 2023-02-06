using System;

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

public static class UnitExtensions {

    /// <summary>Get the short version of this unit's name (1-3 characters), such as <c>MB</c>.</summary>
    /// <param name="unit"></param>
    /// <param name="iec"><c>true</c> to return the IEC abbreviation (KiB, MiB, etc.), or <c>false</c> (the default) to return
    /// the JEDEC abbreviation (KB, MB, etc.)</param>
    /// <returns>The abbreviation for this unit.</returns>
    public static string ToAbbreviation(this Unit unit, bool iec = false) {
        switch (unit) {
            case Unit.Byte:
                return "B";
            case Unit.Kilobyte:
                return iec ? "KiB" : "KB";
            case Unit.Megabyte:
                return iec ? "MiB" : "MB";
            case Unit.Gigabyte:
                return iec ? "GiB" : "GB";
            case Unit.Terabyte:
                return iec ? "TiB" : "TB";
            case Unit.Petabyte:
                return iec ? "PiB" : "PB";
            case Unit.Exabyte:
                return iec ? "EiB" : "EB";

            case Unit.Bit:
                return "b";
            case Unit.Kilobit:
                return "kb";
            case Unit.Megabit:
                return "mb";
            case Unit.Gigabit:
                return "gb";
            case Unit.Terabit:
                return "tb";
            case Unit.Petabit:
                return "pb";
            case Unit.Exabit:
                return "eb";

            default:
                throw new ArgumentOutOfRangeException(nameof(unit), unit, null);
        }
    }

    /// <summary>
    /// Get the long version of this unit's name, such as <c>megabyte</c>.
    /// </summary>
    /// <param name="unit"></param>
    /// <param name="iec"><c>true</c> to return the IEC name (kibibyte, mebibyte, etc.), or <c>false</c> (the default) to return
    /// the JEDEC name (kilobyte, megabyte, etc.)</param>
    /// <returns>The name of this unit.</returns>
    public static string ToName(this Unit unit, bool iec = false) {
        switch (unit) {
            case Unit.Byte:
                return "byte";
            case Unit.Kilobyte:
                return iec ? "kibibyte" : "kilobyte";
            case Unit.Megabyte:
                return iec ? "mebibyte" : "megabyte";
            case Unit.Gigabyte:
                return iec ? "gibibyte" : "gigabyte";
            case Unit.Terabyte:
                return iec ? "tebibyte" : "terabyte";
            case Unit.Petabyte:
                return iec ? "pebibyte" : "petabyte";
            case Unit.Exabyte:
                return iec ? "exbibyte" : "exabyte";

            case Unit.Bit:
                return "bit";
            case Unit.Kilobit:
                return "kilobit";
            case Unit.Megabit:
                return "megabit";
            case Unit.Gigabit:
                return "gigabit";
            case Unit.Terabit:
                return "terabit";
            case Unit.Petabit:
                return "petabit";
            case Unit.Exabit:
                return "exabit";

            default:
                throw new ArgumentOutOfRangeException(nameof(unit), unit, null);
        }
    }

    public static bool IsMultipleOfBits(this Unit unit) {
        switch (unit) {
            case Unit.Byte:
            case Unit.Kilobyte:
            case Unit.Megabyte:
            case Unit.Gigabyte:
            case Unit.Terabyte:
            case Unit.Petabyte:
            case Unit.Exabyte:
                return false;

            case Unit.Bit:
            case Unit.Kilobit:
            case Unit.Megabit:
            case Unit.Gigabit:
            case Unit.Terabit:
            case Unit.Petabit:
            case Unit.Exabit:
                return true;

            default:
                throw new ArgumentOutOfRangeException(nameof(unit), unit, null);
        }
    }

    public static DataSize Quantity(this Unit unit, double quantity) {
        return new DataSize(quantity, unit);
    }

}