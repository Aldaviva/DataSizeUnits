using System;

namespace DataSizeUnits {

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

        /// <summary>
        /// <para>Get the abbreviation for this unit.</para>
        /// <para>Byte values are always uppercase: B, KB, MB, GB, TB, PB, EB.</para>
        /// <para>Bit values are always lowercase: b, kb, mb, gb, tb, pb, eb.</para>
        /// </summary>
        /// <returns>Two letter abbreviation (one letter for bit or byte).</returns>
        public static string ToAbbreviation(this Unit unit) {
            switch (unit) {
                case Unit.Byte:
                    return "B";
                case Unit.Kilobyte:
                    return "KB";
                case Unit.Megabyte:
                    return "MB";
                case Unit.Gigabyte:
                    return "GB";
                case Unit.Terabyte:
                    return "TB";
                case Unit.Petabyte:
                    return "PB";
                case Unit.Exabyte:
                    return "EB";

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

    }

}