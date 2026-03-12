using System.ComponentModel;
using System.Numerics;

#pragma warning disable CA2208 // defective heuristic, was never updated for extension blocks in C# 14

namespace DataSizeUnits;

/// <summary>
/// <para>Orders of magnitude of data, from bit and byte to exabit and exabyte.</para>
/// <para>Kilobits and other *bits units are multiples of 1000 of the next smaller unit. For example, a megabit is 1,000,000 bits (1000 * 1000).</para>
/// <para>Kilobytes and other *bytes units are multiples of 1024 of the next smaller unit. For example, a megabyte is 1,048,576 bytes (1024 * 1024).</para>
/// </summary>
public enum DataSizeUnit {

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
    Exabyte,

    /// <summary>
    /// 1000 exabits, or 1,000,000,000,000,000,000,000 bits
    /// </summary>
    Zettabit,

    /// <summary>
    /// 1024 exabytes, or 1,180,591,620,717,411,303,424 bytes
    /// </summary>
    Zettabyte,

    /// <summary>
    /// 1000 zettabits, or 1,000,000,000,000,000,000,000,000 bits
    /// </summary>
    Yottabit,

    /// <summary>
    /// 1024 zettabytes, or 1,208,925,819,614,629,174,706,176 bytes
    /// </summary>
    Yottabyte,

    /// <summary>
    /// 1000 yottabits, or 1,000,000,000,000,000,000,000,000,000 bits
    /// </summary>
    Ronnabit,

    /// <summary>
    /// 1024 yottabytes, or 1,237,940,039,285,380,274,899,124,224 bytes
    /// </summary>
    Ronnabyte,

    /// <summary>
    /// 1000 ronnabits, or 1,000,000,000,000,000,000,000,000,000,000 bits
    /// </summary>
    Quettabit,

    /// <summary>
    /// 1024 ronnabytes, or 1,267,650,600,228,229,401,496,703,205,376 bytes
    /// </summary>
    Quettabyte

}

/// <summary>
/// Methods on the <see cref="DataSizeUnit"/> enum.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public static class UnitExtensions {

    private static readonly BigInteger Exabyte = new(9_223_372_036_854_775_808UL);
    private static readonly BigInteger Exabit  = new(1_000_000_000_000_000_000UL);

    /// <param name="unit">the unit of data size</param>
    extension(DataSizeUnit unit) {

        /// <summary>Get the short version of this unit's name (1-3 characters), such as <c>MB</c>.</summary>
        /// <param name="iec"><c>true</c> to return the IEC abbreviation (KiB, MiB, etc.), or <c>false</c> (the default) to return
        /// the JEDEC abbreviation (KB, MB, etc.)</param>
        /// <returns>The abbreviation for this unit.</returns>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="unit"/> was force-cast from a fake value</exception>
        public string ToAbbreviation(bool iec = false) => unit switch {
            DataSizeUnit.Byte       => "B",
            DataSizeUnit.Kilobyte   => iec ? "KiB" : "kB",
            DataSizeUnit.Megabyte   => iec ? "MiB" : "MB",
            DataSizeUnit.Gigabyte   => iec ? "GiB" : "GB",
            DataSizeUnit.Terabyte   => iec ? "TiB" : "TB",
            DataSizeUnit.Petabyte   => iec ? "PiB" : "PB",
            DataSizeUnit.Exabyte    => iec ? "EiB" : "EB",
            DataSizeUnit.Zettabyte  => iec ? "ZiB" : "ZB",
            DataSizeUnit.Yottabyte  => iec ? "YiB" : "YB",
            DataSizeUnit.Ronnabyte  => iec ? "RiB" : "RB",
            DataSizeUnit.Quettabyte => iec ? "QiB" : "QB",
            DataSizeUnit.Bit        => "b",
            DataSizeUnit.Kilobit    => "kb",
            DataSizeUnit.Megabit    => "mb",
            DataSizeUnit.Gigabit    => "gb",
            DataSizeUnit.Terabit    => "tb",
            DataSizeUnit.Petabit    => "pb",
            DataSizeUnit.Exabit     => "eb",
            DataSizeUnit.Zettabit   => "zb",
            DataSizeUnit.Yottabit   => "yb",
            DataSizeUnit.Ronnabit   => "rb",
            DataSizeUnit.Quettabit  => "qb",
            _                       => throw new ArgumentOutOfRangeException(nameof(unit), unit, null)
        };

        /// <summary>
        /// Get the long version of this unit's name, such as <c>megabyte</c>.
        /// </summary>
        /// <param name="iec"><c>true</c> to return the IEC name (kibibyte, mebibyte, etc.), or <c>false</c> (the default) to return
        /// the JEDEC name (kilobyte, megabyte, etc.)</param>
        /// <exception cref="ArgumentOutOfRangeException">illegal value of <paramref name="unit"/> from force casting</exception>
        /// <returns>The name of this unit.</returns>
        public string ToName(bool iec = false) => unit switch {
            DataSizeUnit.Byte       => "byte",
            DataSizeUnit.Kilobyte   => iec ? "kibibyte" : "kilobyte",
            DataSizeUnit.Megabyte   => iec ? "mebibyte" : "megabyte",
            DataSizeUnit.Gigabyte   => iec ? "gibibyte" : "gigabyte",
            DataSizeUnit.Terabyte   => iec ? "tebibyte" : "terabyte",
            DataSizeUnit.Petabyte   => iec ? "pebibyte" : "petabyte",
            DataSizeUnit.Exabyte    => iec ? "exbibyte" : "exabyte",
            DataSizeUnit.Zettabyte  => iec ? "zebibyte" : "zettabyte",
            DataSizeUnit.Yottabyte  => iec ? "yobibyte" : "yottabyte",
            DataSizeUnit.Ronnabyte  => iec ? "robibyte" : "ronnabyte",
            DataSizeUnit.Quettabyte => iec ? "quebibyte" : "quettabyte",
            DataSizeUnit.Bit        => "bit",
            DataSizeUnit.Kilobit    => "kilobit",
            DataSizeUnit.Megabit    => "megabit",
            DataSizeUnit.Gigabit    => "gigabit",
            DataSizeUnit.Terabit    => "terabit",
            DataSizeUnit.Petabit    => "petabit",
            DataSizeUnit.Exabit     => "exabit",
            DataSizeUnit.Zettabit   => "zettabit",
            DataSizeUnit.Yottabit   => "yottabit",
            DataSizeUnit.Ronnabit   => "ronnabit",
            DataSizeUnit.Quettabit  => "quettabit",
            _                       => throw new ArgumentOutOfRangeException(nameof(unit), unit, null)
        };

        /// <summary>
        /// Determine whether a data size <see cref="DataSizeUnit"/> is based on bits or bytes.
        /// </summary>
        /// <returns><c>true</c> if <paramref name="unit"/> is based on bits, or <c>false</c> if it is based on bytes</returns>
        /// <exception cref="ArgumentOutOfRangeException">illegal value of <paramref name="unit"/> from force casting</exception>
        public bool IsMultipleOfBits => unit switch {
            DataSizeUnit.Byte
                or DataSizeUnit.Kilobyte
                or DataSizeUnit.Megabyte
                or DataSizeUnit.Gigabyte
                or DataSizeUnit.Terabyte
                or DataSizeUnit.Petabyte
                or DataSizeUnit.Exabyte
                or DataSizeUnit.Zettabyte
                or DataSizeUnit.Yottabyte
                or DataSizeUnit.Ronnabyte
                or DataSizeUnit.Quettabyte => false,
            DataSizeUnit.Bit
                or DataSizeUnit.Kilobit
                or DataSizeUnit.Megabit
                or DataSizeUnit.Gigabit
                or DataSizeUnit.Terabit
                or DataSizeUnit.Petabit
                or DataSizeUnit.Exabit
                or DataSizeUnit.Zettabit
                or DataSizeUnit.Yottabit
                or DataSizeUnit.Ronnabit
                or DataSizeUnit.Quettabit => true,
            _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, null)
        };

        /// <summary>
        /// <para>Get a data size unit from its string name or abbreviation.</para>
        /// <para>Supports units of bits and bytes, including the JEDEC units like kilobytes and IEC units like kibibytes, as well as all their abbreviations.</para>
        /// <para>Some abbreviations are case-insensitive, such as <c>megabyte</c>, but others are case-sensitive, like <c>mb</c> and <c>MB</c> because one means megabits and the other means megabytes.</para>
        /// <para>For example, all the inputs that will be parsed as <c>Unit.Megabyte</c> are <c>M</c>, <c>MB</c>, <c>megabyte</c>, <c>mbyte</c>, <c>mib</c>, and <c>mebibyte</c> (the first two are case-sensitive).</para>
        /// <para>Usage: <c>Unit megabyte = DataSize.ParseUnit("megabyte");</c></para>
        /// </summary>
        /// <param name="unitNameOrAbbreviation">The name (e.g. <c>kilobyte</c>) or abbreviation (e.g. <c>kB</c>) of a data size unit.</param>
        /// <returns>The <see cref="DataSizeUnit"/> value that represents the matched data size unit.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The given name does not match any known units or their abbreviations.</exception>
        public static DataSizeUnit? Parse(string unitNameOrAbbreviation) {
            unitNameOrAbbreviation = unitNameOrAbbreviation.Trim();
            switch (unitNameOrAbbreviation.ToLowerInvariant()) {
                case "byte":
                    return DataSizeUnit.Byte;
                case "kilobyte":
                case "kbyte":
                case "kib":
                case "kibibyte":
                    return DataSizeUnit.Kilobyte;
                case "megabyte":
                case "mbyte":
                case "mib":
                case "mebibyte":
                    return DataSizeUnit.Megabyte;
                case "gigabyte":
                case "gbyte":
                case "gib":
                case "gibibyte":
                    return DataSizeUnit.Gigabyte;
                case "terabyte":
                case "tbyte":
                case "tib":
                case "tebibyte":
                    return DataSizeUnit.Terabyte;
                case "petabyte":
                case "pbyte":
                case "pib":
                case "pebibyte":
                    return DataSizeUnit.Petabyte;
                case "exabyte":
                case "ebyte":
                case "eib":
                case "exbibyte":
                    return DataSizeUnit.Exabyte;
                case "zettabyte":
                case "zbyte":
                case "zib":
                case "zebibyte":
                    return DataSizeUnit.Zettabyte;
                case "yottabyte":
                case "ybyte":
                case "yib":
                case "yobibyte":
                    return DataSizeUnit.Yottabyte;
                case "ronnabyte":
                case "rbyte":
                case "rib":
                case "robibyte":
                    return DataSizeUnit.Ronnabyte;
                case "quettabyte":
                case "qbyte":
                case "qib":
                case "quebibyte":
                    return DataSizeUnit.Quettabyte;

                case "bit":
                    return DataSizeUnit.Bit;
                case "kilobit":
                case "kbit":
                    return DataSizeUnit.Kilobit;
                case "megabit":
                case "mbit":
                    return DataSizeUnit.Megabit;
                case "gigabit":
                case "gbit":
                    return DataSizeUnit.Gigabit;
                case "terabit":
                case "tbit":
                    return DataSizeUnit.Terabit;
                case "petabit":
                case "pbit":
                    return DataSizeUnit.Petabit;
                case "exabit":
                case "ebit":
                    return DataSizeUnit.Exabit;
                case "zettabit":
                case "zbit":
                    return DataSizeUnit.Zettabit;
                case "yottabit":
                case "ybit":
                    return DataSizeUnit.Yottabit;
                case "ronnabit":
                case "rbit":
                    return DataSizeUnit.Ronnabit;
                case "quettabit":
                case "qbit":
                    return DataSizeUnit.Quettabit;

                default:
                    //not found in case-insensitive switch, continuing to case-sensitive switch below
                    break;
            }

            return unitNameOrAbbreviation switch {
                "B"                 => DataSizeUnit.Byte,
                "kB" or "KB" or "K" => DataSizeUnit.Kilobyte,
                "MB" or "M"         => DataSizeUnit.Megabyte,
                "GB" or "G"         => DataSizeUnit.Gigabyte,
                "TB" or "T"         => DataSizeUnit.Terabyte,
                "PB" or "P"         => DataSizeUnit.Petabyte,
                "EB" or "E"         => DataSizeUnit.Exabyte,
                "ZB" or "Z"         => DataSizeUnit.Zettabyte,
                "YB" or "Y"         => DataSizeUnit.Yottabyte,
                "RB" or "R"         => DataSizeUnit.Ronnabyte,
                "QB" or "Q"         => DataSizeUnit.Quettabyte,
                "b"                 => DataSizeUnit.Bit,
                "kb" or "Kb" or "k" => DataSizeUnit.Kilobit,
                "mb" or "Mb" or "m" => DataSizeUnit.Megabit,
                "Gb" or "gb" or "g" => DataSizeUnit.Gigabit,
                "Tb" or "tb" or "t" => DataSizeUnit.Terabit,
                "Pb" or "pb" or "p" => DataSizeUnit.Petabit,
                "Eb" or "eb" or "e" => DataSizeUnit.Exabit,
                "Zb" or "zb" or "z" => DataSizeUnit.Zettabit,
                "Yb" or "yb" or "y" => DataSizeUnit.Yottabit,
                "Rb" or "rb" or "r" => DataSizeUnit.Ronnabit,
                "Qb" or "qb" or "q" => DataSizeUnit.Quettabit,
                _                   => throw new ArgumentOutOfRangeException("Unrecognized abbreviation for data size unit " + unitNameOrAbbreviation)
            };
        }

        /// <exception cref="ArgumentOutOfRangeException">illegal value of <paramref name="unit"/> from force casting</exception>
        public BigInteger AsBits => unit switch {
            DataSizeUnit.Byte       => 8,
            DataSizeUnit.Kilobyte   => 8192UL,
            DataSizeUnit.Megabyte   => 8388608UL,
            DataSizeUnit.Gigabyte   => 8589934592UL,
            DataSizeUnit.Terabyte   => 8796093022208UL,
            DataSizeUnit.Petabyte   => 9007199254740992UL,
            DataSizeUnit.Exabyte    => Exabyte,
            DataSizeUnit.Zettabyte  => Exabyte * 1024UL,
            DataSizeUnit.Yottabyte  => Exabyte * 1048576UL,
            DataSizeUnit.Ronnabyte  => Exabyte * 1073741824UL,
            DataSizeUnit.Quettabyte => Exabyte * 1099511627776UL,
            DataSizeUnit.Bit        => 1,
            DataSizeUnit.Kilobit    => 1000L,
            DataSizeUnit.Megabit    => 1000000UL,
            DataSizeUnit.Gigabit    => 1000000000UL,
            DataSizeUnit.Terabit    => 1000000000000UL,
            DataSizeUnit.Petabit    => 1000000000000000UL,
            DataSizeUnit.Exabit     => Exabit,
            DataSizeUnit.Zettabit   => Exabit * 1000UL,
            DataSizeUnit.Yottabit   => Exabit * 1000000UL,
            DataSizeUnit.Ronnabit   => Exabit * 1000000000UL,
            DataSizeUnit.Quettabit  => Exabit * 1000000000000UL,
            _                       => throw new ArgumentOutOfRangeException(nameof(unit), unit, null)
        };

        internal static DataSizeUnit ForMagnitude(int orderOfMagnitude, bool useBitsInsteadOfBytes) => orderOfMagnitude switch {
            0 => useBitsInsteadOfBytes ? DataSizeUnit.Bit : DataSizeUnit.Byte,
            1 => useBitsInsteadOfBytes ? DataSizeUnit.Kilobit : DataSizeUnit.Kilobyte,
            2 => useBitsInsteadOfBytes ? DataSizeUnit.Megabit : DataSizeUnit.Megabyte,
            3 => useBitsInsteadOfBytes ? DataSizeUnit.Gigabit : DataSizeUnit.Gigabyte,
            4 => useBitsInsteadOfBytes ? DataSizeUnit.Terabit : DataSizeUnit.Terabyte,
            5 => useBitsInsteadOfBytes ? DataSizeUnit.Petabit : DataSizeUnit.Petabyte,
            6 => useBitsInsteadOfBytes ? DataSizeUnit.Exabit : DataSizeUnit.Exabyte,
            7 => useBitsInsteadOfBytes ? DataSizeUnit.Zettabit : DataSizeUnit.Zettabyte,
            8 => useBitsInsteadOfBytes ? DataSizeUnit.Yottabit : DataSizeUnit.Yottabyte,
            9 => useBitsInsteadOfBytes ? DataSizeUnit.Ronnabit : DataSizeUnit.Ronnabyte,
            _ => useBitsInsteadOfBytes ? DataSizeUnit.Quettabit : DataSizeUnit.Quettabyte
        };

    }

}