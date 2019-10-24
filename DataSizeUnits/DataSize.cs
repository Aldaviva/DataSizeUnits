using System;

namespace DataSizeUnits {

    public enum DataSize {

        BYTE,
        KILOBYTE,
        MEGABYTE,
        GIGABYTE,
        TERABYTE,
        PETABYTE,
        BIT,
        KILOBIT,
        MEGABIT,
        GIGABIT,
        TERABIT,
        PETABIT

    }

    public static class DataSizeMethods {

        public static ulong asBits(this DataSize source) {
            switch (source) {
                case DataSize.BYTE:
                    return 8L;
                case DataSize.KILOBYTE:
                    return 8L * 1024;
                case DataSize.MEGABYTE:
                    return 8L * 1024 * 1024;
                case DataSize.GIGABYTE:
                    return 8L * 1024 * 1024 * 1024;
                case DataSize.TERABYTE:
                    return 8L * 1024 * 1024 * 1024 * 1024;
                case DataSize.PETABYTE:
                    return 8L * 1024 * 1024 * 1024 * 1024 * 1024;
                case DataSize.BIT:
                    return 1;
                case DataSize.KILOBIT:
                    return 1000L;
                case DataSize.MEGABIT:
                    return 1000L * 1000;
                case DataSize.GIGABIT:
                    return 1000L * 1000 * 1000;
                case DataSize.TERABIT:
                    return 1000L * 1000 * 1000 * 1000;
                case DataSize.PETABIT:
                    return 1000L * 1000 * 1000 * 1000 * 1000;
                default:
                    throw new ArgumentOutOfRangeException(nameof(source), source, null);
            }
        }

        public static string toAbbreviation(this DataSize source) {
            switch (source) {
                case DataSize.BYTE:
                    return "B";
                case DataSize.KILOBYTE:
                    return "KB";
                case DataSize.MEGABYTE:
                    return "MB";
                case DataSize.GIGABYTE:
                    return "GB";
                case DataSize.TERABYTE:
                    return "TB";
                case DataSize.PETABYTE:
                    return "PB";
                case DataSize.BIT:
                    return "b";
                case DataSize.KILOBIT:
                    return "kb";
                case DataSize.MEGABIT:
                    return "mb";
                case DataSize.GIGABIT:
                    return "gb";
                case DataSize.TERABIT:
                    return "tb";
                case DataSize.PETABIT:
                    return "pb";
                default:
                    throw new ArgumentOutOfRangeException(nameof(source), source, null);
            }
        }

        public static DataSize forMagnitude(int orderOfMagnitude, bool useBytes) {
            switch (orderOfMagnitude) {
                case 0:
                    return useBytes ? DataSize.BYTE : DataSize.BIT;
                case 1:
                    return useBytes ? DataSize.KILOBYTE : DataSize.KILOBIT;
                case 2:
                    return useBytes ? DataSize.MEGABYTE : DataSize.MEGABIT;
                case 3:
                    return useBytes ? DataSize.GIGABYTE : DataSize.GIGABIT;
                case 4:
                    return useBytes ? DataSize.TERABYTE : DataSize.TERABIT;
                case 5:
                    return useBytes ? DataSize.PETABYTE : DataSize.PETABIT;
                default:
                    throw new ArgumentOutOfRangeException(
                        $"Supported orders of magnitude are between 0 (bit/byte) and 5 (petabit/petabyte) inclusive. {orderOfMagnitude} is outside the range [0,5].");
            }
        }

        public static DataSize forAbbreviation(string abbreviation) {
            switch (abbreviation.ToLowerInvariant()) {
                case "byte":
                    return DataSize.BYTE;
                case "bit":
                    return DataSize.BIT;
                case "kilobyte":
                case "kbyte":
                case "kibibit":
                case "kib":
                case "kibit":
                    return DataSize.KILOBYTE;
                case "kilobit":
                case "kbit":
                    return DataSize.KILOBIT;
                case "megabyte":
                case "mbyte":
                case "mebibit":
                case "mib":
                case "mibit":
                    return DataSize.MEGABYTE;
                case "megabit":
                case "mbit":
                    return DataSize.MEGABIT;
                case "gigabyte":
                case "gbyte":
                case "gibibit":
                case "gib":
                case "gibit":
                    return DataSize.GIGABYTE;
                case "gigabit":
                case "gbit":
                    return DataSize.GIGABIT;
                case "terabyte":
                case "tbyte":
                case "tebibit":
                case "tib":
                case "tibit":
                    return DataSize.TERABYTE;
                case "terabit":
                case "tbit":
                    return DataSize.TERABIT;
                case "petabyte":
                case "pbyte":
                case "pebibit":
                case "pib":
                case "pibit":
                    return DataSize.PETABYTE;
                case "petabit":
                case "pbit":
                    return DataSize.PETABIT;
                default:
                    //not found in case-insensitive switch, continuing to case-sensitive switch
                    break;
            }

            switch (abbreviation) {
                case "B":
                    return DataSize.BYTE;
                case "b":
                    return DataSize.BIT;
                case "kB":
                case "KB":
                    return DataSize.KILOBYTE;
                case "kb":
                case "Kb":
                    return DataSize.KILOBIT;
                case "MB":
                    return DataSize.MEGABYTE;
                case "mb":
                case "Mb":
                    return DataSize.MEGABIT;
                case "GB":
                    return DataSize.GIGABYTE;
                case "Gb":
                case "gb":
                    return DataSize.GIGABIT;
                case "TB":
                    return DataSize.TERABYTE;
                case "Tb":
                case "tb":
                    return DataSize.TERABIT;
                case "PB":
                    return DataSize.PETABYTE;
                case "Pb":
                case "pb":
                    return DataSize.PETABIT;
                default:
                    throw new ArgumentOutOfRangeException("Unrecognized abbreviation for data size unit " + abbreviation);
            }
        }

        public static double scaleTo(ulong inputBytes, DataSize destinationScale) {
            return inputBytes * 8.0 / destinationScale.asBits();
        }

        public static (double value, DataSize unit) scaleAutomatically(ulong inputBytes, bool toBytesNotBits) {
            int orderOfMagnitude = (int) Math.Max(0, Math.Floor(Math.Log(inputBytes, toBytesNotBits ? 1024 : 1000)));
            DataSize unit = forMagnitude(orderOfMagnitude, toBytesNotBits);
            double scaledValue = scaleTo(inputBytes, unit);
            return (scaledValue, unit);
        }

    }

}