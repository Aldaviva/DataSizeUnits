using System;
using DataSizeUnits;
using Xunit;

namespace Tests {

    public class DataSizeFormatterTests {

        [Theory]
        [MemberData(nameof(formatData))]
        [MemberData(nameof(precisionData))]
        [MemberData(nameof(unitData))]
        public void format(ulong inputBytes, string formatSyntax, string expectedOutput) {
            string formatString = "{0:" + formatSyntax + "}";
            string actualOutput = string.Format(DataSize.FORMATTER, formatString, inputBytes);
            Assert.Equal(expectedOutput, actualOutput);
        }

        public static TheoryData<ulong, string, string> formatData = new TheoryData<ulong, string, string> {
            { 0, "", "0.00 B" },
            { 0, "A", "0.00 B" },
            { 1024, "A", "1.00 KB" },
            { 1024 * 1024, "A", "1.00 MB" },
            { 1024 * 1024 * 1024, "A", "1.00 GB" },
            { 1024L * 1024 * 1024 * 1024, "A", "1.00 TB" },
            { 1024L * 1024 * 1024 * 1024 * 1024, "A", "1.00 PB" }
        };

        public static TheoryData<ulong, string, string> precisionData = new TheoryData<ulong, string, string> {
            { 9_995_326_316_544, "A", "9.09 TB" },
            { 9_995_326_316_544, "A0", "9 TB" },
            { 9_995_326_316_544, "1", "9,995,326,316,544.0 B" },
            { 9_995_326_316_544, "A1", "9.1 TB" },
            { 9_995_326_316_544, "A2", "9.09 TB" },
            { 9_995_326_316_544, "A3", "9.091 TB" }
        };

        public static TheoryData<ulong, string, string> unitData = new TheoryData<ulong, string, string> {
            { 9_995_326_316_544, "B0", "9,995,326,316,544 B" },
            { 9_995_326_316_544, "K0", "9,761,060,856 KB" },
            { 9_995_326_316_544, "KB0", "9,761,060,856 KB" },
            { 9_995_326_316_544, "MB0", "9,532,286 MB" },
            { 9_995_326_316_544, "GB0", "9,309 GB" },
            { 9_995_326_316_544, "TB0", "9 TB" },
            { 9_995_326_316_544, "PB0", "0 PB" },
            { 9_995_326_316_544, "byte0", "9,995,326,316,544 B" },
            { 9_995_326_316_544, "kilobyte0", "9,761,060,856 KB" },
            { 9_995_326_316_544, "megabyte0", "9,532,286 MB" },
            { 9_995_326_316_544, "gigabyte0", "9,309 GB" },
            { 9_995_326_316_544, "terabyte0", "9 TB" },
            { 9_995_326_316_544, "petabyte0", "0 PB" },
            { 9_995_326_316_544_000, "b0", "79,962,610,532,352,000 b" },
            { 9_995_326_316_544_000, "kb0", "79,962,610,532,352 kb" },
            { 9_995_326_316_544_000, "k0", "79,962,610,532,352 kb" },
            { 9_995_326_316_544_000, "mb0", "79,962,610,532 mb" },
            { 9_995_326_316_544_000, "gb0", "79,962,611 gb" },
            { 9_995_326_316_544_000, "tb0", "79,963 tb" },
            { 9_995_326_316_544_000, "pb0", "80 pb" },
            { 9_995_326_316_544_000, "bit0", "79,962,610,532,352,000 b" },
            { 9_995_326_316_544_000, "kilobit0", "79,962,610,532,352 kb" },
            { 9_995_326_316_544_000, "megabit0", "79,962,610,532 mb" },
            { 9_995_326_316_544_000, "gigabit0", "79,962,611 gb" },
            { 9_995_326_316_544_000, "terabit0", "79,963 tb" },
            { 9_995_326_316_544_000, "petabit0", "80 pb" },
        };

        [Theory, MemberData(nameof(otherData))]
        public void handleOtherFormats(object otherInput, string expectedOutput) {
            string actualOutput = string.Format(DataSize.FORMATTER, "{0:D}", otherInput);
            Assert.Equal(expectedOutput, actualOutput);
        }

        public static TheoryData<object, string> otherData = new TheoryData<object, string> {
            { new DateTime(1988, 8, 17, 16, 30, 0), "Wednesday, August 17, 1988" },
            { new Unformattable(), "unformattable" },
            { null, "" }
        };

        [Fact]
        public void handleFormatException() {
            Assert.Throws<FormatException>(() => string.Format(DataSize.FORMATTER, "{0:MB1}", new Unstringable()));
        }

        [Fact]
        public void wrongUsage() {
            string actual = ((ICustomFormatter) DataSize.FORMATTER).Format("{0:A}", 0, null);
            Assert.Null(actual);
        }

        // I can't fucking believe the C# compiler is stupid enough to allow this.
        [Fact]
        public void madeUpEnumValue() {
            const DataSize.Unit madeUpEnumValue = (DataSize.Unit) 9999;
            Assert.Throws<ArgumentOutOfRangeException>(() => DataSize.convert(1, madeUpEnumValue));
            Assert.Throws<ArgumentOutOfRangeException>(() => DataSize.toAbbreviation(madeUpEnumValue));
        }

        [Fact]
        public void negativeNumbers() {
            string actual = string.Format(DataSize.FORMATTER, "{0:K0}", -1024);
            Assert.Equal("-1 KB", actual);
        }

    }

    internal class Unformattable {

        public override string ToString() {
            return "unformattable";
        }

    }

    internal class Unstringable {

        public override string ToString() {
            throw new FormatException();
        }

    }

}