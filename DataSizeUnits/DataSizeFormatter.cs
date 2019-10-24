using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace DataSizeUnits {

    internal class DataSizeFormatter: IFormatProvider, ICustomFormatter {

        public object GetFormat(Type formatType) {
            return formatType == typeof(ICustomFormatter) ? this : null;
        }

        public string Format(string format, object arg, IFormatProvider formatProvider) {
            if (!Equals(formatProvider)) {
                return null;
            }

            if (string.IsNullOrEmpty(format)) {
                format = "B";
            }

            ulong bytes;
            try {
                bytes = Convert.ToUInt64(arg);
            } catch (Exception) {
                return handleOtherFormats(format, arg);
            }

            string unitString = Regex.Match(format, @"^[a-z]+", RegexOptions.IgnoreCase).Value;
            if (string.IsNullOrEmpty(unitString)) {
                unitString = "B";
            }

            if (!int.TryParse(Regex.Match(format, @"\d+$").Value, out int precision)) {
                precision = 0;
            }

            if (unitString.ToLowerInvariant() == "a") {
                (double scaledValue, DataSize.Unit scaledUnit) = DataSize.scaleAutomatically(bytes, unitString == "A");
                return DataSizeFormatter.format(scaledValue, scaledUnit, precision);
            } else {
                try {
                    DataSize.Unit unit = DataSize.forAbbreviation(unitString);
                    double scaledValue = DataSize.scaleTo(bytes, unit);
                    return DataSizeFormatter.format(scaledValue, unit, precision);
                } catch (ArgumentOutOfRangeException) {
                    return handleOtherFormats(format, arg);
                }
            }
        }

        private static string format(double value, DataSize.Unit unit, int precision) {
            var culture = (CultureInfo) CultureInfo.CurrentCulture.Clone();
            culture.NumberFormat.NumberDecimalDigits = precision;

            return value.ToString("N", culture) + " " + DataSize.toAbbreviation(unit);
        }

        private static string handleOtherFormats(string format, object arg) {
            try {
                if (arg is IFormattable formattable) {
                    return formattable.ToString(format, CultureInfo.CurrentCulture);
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

}