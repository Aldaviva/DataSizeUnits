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
            string actualOutput = string.Format(new DataSizeFormatter(), formatString, inputBytes);
            Assert.Equal(expectedOutput, actualOutput);
        }

        public static TheoryData<ulong, string, string> formatData = new TheoryData<ulong, string, string> {
            { 0, "", "0 B" },
            { 0, "A", "0 B" },
            { 1024, "A", "1 KB" },
            { 1024 * 1024, "A", "1 MB" },
            { 1024 * 1024 * 1024, "A", "1 GB" },
            { 1024L * 1024 * 1024 * 1024, "A", "1 TB" },
            { 1024L * 1024 * 1024 * 1024 * 1024, "A", "1 PB" }
        };

        public static TheoryData<ulong, string, string> precisionData = new TheoryData<ulong, string, string> {
            { 9_995_326_316_544, "A", "9 TB" },
            { 9_995_326_316_544, "1", "9,995,326,316,544.0 B" },
            { 9_995_326_316_544, "A1", "9.1 TB" },
            { 9_995_326_316_544, "A2", "9.09 TB" },
            { 9_995_326_316_544, "A3", "9.091 TB" }
        };

        public static TheoryData<ulong, string, string> unitData = new TheoryData<ulong, string, string> {
            { 9_995_326_316_544, "B", "9,995,326,316,544 B" },
            { 9_995_326_316_544, "KB", "9,761,060,856 KB" },
            { 9_995_326_316_544, "MB", "9,532,286 MB" },
            { 9_995_326_316_544, "GB", "9,309 GB" },
            { 9_995_326_316_544, "TB", "9 TB" },
            { 9_995_326_316_544, "PB", "0 PB" },
            { 9_995_326_316_544, "byte", "9,995,326,316,544 B" },
            { 9_995_326_316_544, "kilobyte", "9,761,060,856 KB" },
            { 9_995_326_316_544, "megabyte", "9,532,286 MB" },
            { 9_995_326_316_544, "gigabyte", "9,309 GB" },
            { 9_995_326_316_544, "terabyte", "9 TB" },
            { 9_995_326_316_544, "petabyte", "0 PB" },
            { 9_995_326_316_544_000, "b", "79,962,610,532,352,000 b" },
            { 9_995_326_316_544_000, "kb", "79,962,610,532,352 kb" },
            { 9_995_326_316_544_000, "mb", "79,962,610,532 mb" },
            { 9_995_326_316_544_000, "gb", "79,962,611 gb" },
            { 9_995_326_316_544_000, "tb", "79,963 tb" },
            { 9_995_326_316_544_000, "pb", "80 pb" },
            { 9_995_326_316_544_000, "bit", "79,962,610,532,352,000 b" },
            { 9_995_326_316_544_000, "kilobit", "79,962,610,532,352 kb" },
            { 9_995_326_316_544_000, "megabit", "79,962,610,532 mb" },
            { 9_995_326_316_544_000, "gigabit", "79,962,611 gb" },
            { 9_995_326_316_544_000, "terabit", "79,963 tb" },
            { 9_995_326_316_544_000, "petabit", "80 pb" },
        };

        [Theory, MemberData(nameof(otherData))]
        public void handleOtherFormats(object otherInput, string expectedOutput) {
            string actualOutput = string.Format(new DataSizeFormatter(), "{0:D}", otherInput);
            Assert.Equal(expectedOutput, actualOutput);
        }

        public static TheoryData<object, string> otherData = new TheoryData<object, string> {
            { new DateTime(1988, 8, 17, 16, 30, 0), "Wednesday, August 17, 1988" },
            { new Unformattable(), "unformattable" },
            { null, "" }
        };

    }

    internal class Unformattable {

        public override string ToString() {
            return "unformattable";
        }

    }

}