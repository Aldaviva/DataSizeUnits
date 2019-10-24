using System;

namespace DataSizeUnits {

    public static class DataSize {

        public static readonly IFormatProvider FORMATTER = new DataSizeFormatter();

        public enum Unit {

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

        public static ulong toBits(Unit source) {
            switch (source) {
                case Unit.BYTE:
                    return 8L;
                case Unit.KILOBYTE:
                    return 8L * 1024;
                case Unit.MEGABYTE:
                    return 8L * 1024 * 1024;
                case Unit.GIGABYTE:
                    return 8L * 1024 * 1024 * 1024;
                case Unit.TERABYTE:
                    return 8L * 1024 * 1024 * 1024 * 1024;
                case Unit.PETABYTE:
                    return 8L * 1024 * 1024 * 1024 * 1024 * 1024;
                case Unit.BIT:
                    return 1;
                case Unit.KILOBIT:
                    return 1000L;
                case Unit.MEGABIT:
                    return 1000L * 1000;
                case Unit.GIGABIT:
                    return 1000L * 1000 * 1000;
                case Unit.TERABIT:
                    return 1000L * 1000 * 1000 * 1000;
                case Unit.PETABIT:
                    return 1000L * 1000 * 1000 * 1000 * 1000;
                default:
                    throw new ArgumentOutOfRangeException(nameof(source), source, null);
            }
        }

        public static string toAbbreviation(Unit source) {
            switch (source) {
                case Unit.BYTE:
                    return "B";
                case Unit.KILOBYTE:
                    return "KB";
                case Unit.MEGABYTE:
                    return "MB";
                case Unit.GIGABYTE:
                    return "GB";
                case Unit.TERABYTE:
                    return "TB";
                case Unit.PETABYTE:
                    return "PB";
                case Unit.BIT:
                    return "b";
                case Unit.KILOBIT:
                    return "kb";
                case Unit.MEGABIT:
                    return "mb";
                case Unit.GIGABIT:
                    return "gb";
                case Unit.TERABIT:
                    return "tb";
                case Unit.PETABIT:
                    return "pb";
                default:
                    throw new ArgumentOutOfRangeException(nameof(source), source, null);
            }
        }

        public static Unit forMagnitude(int orderOfMagnitude, bool useBytes) {
            switch (orderOfMagnitude) {
                case 0:
                    return useBytes ? Unit.BYTE : Unit.BIT;
                case 1:
                    return useBytes ? Unit.KILOBYTE : Unit.KILOBIT;
                case 2:
                    return useBytes ? Unit.MEGABYTE : Unit.MEGABIT;
                case 3:
                    return useBytes ? Unit.GIGABYTE : Unit.GIGABIT;
                case 4:
                    return useBytes ? Unit.TERABYTE : Unit.TERABIT;
                case 5:
                    return useBytes ? Unit.PETABYTE : Unit.PETABIT;
                default:
                    throw new ArgumentOutOfRangeException(
                        $"Supported orders of magnitude are between 0 (bit/byte) and 5 (petabit/petabyte) inclusive. {orderOfMagnitude} is outside the range [0,5].");
            }
        }

        public static Unit forAbbreviation(string abbreviation) {
            switch (abbreviation.ToLowerInvariant()) {
                case "byte":
                    return Unit.BYTE;
                case "bit":
                    return Unit.BIT;
                case "kilobyte":
                case "kbyte":
                case "kibibit":
                case "kib":
                case "kibit":
                    return Unit.KILOBYTE;
                case "kilobit":
                case "kbit":
                    return Unit.KILOBIT;
                case "megabyte":
                case "mbyte":
                case "mebibit":
                case "mib":
                case "mibit":
                    return Unit.MEGABYTE;
                case "megabit":
                case "mbit":
                    return Unit.MEGABIT;
                case "gigabyte":
                case "gbyte":
                case "gibibit":
                case "gib":
                case "gibit":
                    return Unit.GIGABYTE;
                case "gigabit":
                case "gbit":
                    return Unit.GIGABIT;
                case "terabyte":
                case "tbyte":
                case "tebibit":
                case "tib":
                case "tibit":
                    return Unit.TERABYTE;
                case "terabit":
                case "tbit":
                    return Unit.TERABIT;
                case "petabyte":
                case "pbyte":
                case "pebibit":
                case "pib":
                case "pibit":
                    return Unit.PETABYTE;
                case "petabit":
                case "pbit":
                    return Unit.PETABIT;
                default:
                    //not found in case-insensitive switch, continuing to case-sensitive switch
                    break;
            }

            switch (abbreviation) {
                case "B":
                    return Unit.BYTE;
                case "b":
                    return Unit.BIT;
                case "kB":
                case "KB":
                    return Unit.KILOBYTE;
                case "kb":
                case "Kb":
                    return Unit.KILOBIT;
                case "MB":
                    return Unit.MEGABYTE;
                case "mb":
                case "Mb":
                    return Unit.MEGABIT;
                case "GB":
                    return Unit.GIGABYTE;
                case "Gb":
                case "gb":
                    return Unit.GIGABIT;
                case "TB":
                    return Unit.TERABYTE;
                case "Tb":
                case "tb":
                    return Unit.TERABIT;
                case "PB":
                    return Unit.PETABYTE;
                case "Pb":
                case "pb":
                    return Unit.PETABIT;
                default:
                    throw new ArgumentOutOfRangeException("Unrecognized abbreviation for data size unit " + abbreviation);
            }
        }

        public static double scaleTo(ulong inputBytes, Unit destinationScale) {
            return inputBytes * 8.0 / toBits(destinationScale);
        }

        public static (double value, Unit unit) scaleAutomatically(ulong inputBytes, bool toBytesNotBits) {
            int orderOfMagnitude = (int) Math.Max(0, Math.Floor(Math.Log(inputBytes, toBytesNotBits ? 1024 : 1000)));
            Unit unit = forMagnitude(orderOfMagnitude, toBytesNotBits);
            double scaledValue = scaleTo(inputBytes, unit);
            return (scaledValue, unit);
        }

    }

}