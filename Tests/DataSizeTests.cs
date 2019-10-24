using DataSizeUnits;
using Xunit;
using static DataSizeUnits.DataSizeMethods;

namespace Tests {

    public class DataSizeTests {

        [Theory, MemberData(nameof(autoScaleData))]
        public void autoScale(ulong inputBytes, double expectedSize, DataSize expectedUnit, bool useBytes) {
            (double actualSize, DataSize actualUnit) = scaleAutomatically(inputBytes, useBytes);
            Assert.Equal(expectedSize, actualSize, 3);
            Assert.Equal(expectedUnit, actualUnit);
        }

        public static TheoryData<ulong, double, DataSize, bool> autoScaleData => new TheoryData<ulong, double, DataSize, bool> {
            { 0, 0.0, DataSize.BYTE, true },
            { 1, 1, DataSize.BYTE, true },
            { 1023, 1023.0, DataSize.BYTE, true },
            { 1024, 1.0, DataSize.KILOBYTE, true },
            { 1536, 1.5, DataSize.KILOBYTE, true },
            { 1024 * 1024, 1, DataSize.MEGABYTE, true },
            { 1024 * 1024 * 1024, 1, DataSize.GIGABYTE, true },
            { 1024L * 1024 * 1024 * 1024, 1, DataSize.TERABYTE, true },
            { 1024L * 1024 * 1024 * 1024 * 1024, 1, DataSize.PETABYTE, true },
            { 0, 0.0, DataSize.BIT, false },
            { 1, 8, DataSize.BIT, false },
            { 1023, 8.184, DataSize.KILOBIT, false },
            { 1024, 8.192, DataSize.KILOBIT, false },
            { 1536, 12.288, DataSize.KILOBIT, false },
            { 1024 * 1024, 8.388608, DataSize.MEGABIT, false },
            { 1024 * 1024 * 1024, 8.589934592, DataSize.GIGABIT, false },
            { 1024L * 1024 * 1024 * 1024, 8.796093022208, DataSize.TERABIT, false },
            { 1024L * 1024 * 1024 * 1024 * 1024, 9.007199254740992, DataSize.PETABIT, false },
        };

        [Theory, MemberData(nameof(scaleToData))]
        public void manualScale(ulong inputBytes, DataSize inputDestinationScale, double expectedValue) {
            double actualValue = DataSizeMethods.scaleTo(inputBytes, inputDestinationScale);
            Assert.Equal(expectedValue, actualValue, 3);
        }

        public static TheoryData<ulong, DataSize, double> scaleToData => new TheoryData<ulong, DataSize, double> {
            { 0, DataSize.BYTE, 0 },
            { 0, DataSize.BIT, 0 },
            { 0, DataSize.PETABYTE, 0 },
            { 0, DataSize.PETABIT, 0 },
            { 9_995_326_316_544, DataSize.BYTE, 9_995_326_316_544 },
            { 9_995_326_316_544, DataSize.KILOBYTE, 9_761_060_856 },
            { 9_995_326_316_544, DataSize.MEGABYTE, 9_532_285.9921875 },
            { 9_995_326_316_544, DataSize.GIGABYTE, 9_308.873039245605 },
            { 9_995_326_316_544, DataSize.TERABYTE, 9.090696327388287 },
            { 9_995_326_316_544, DataSize.PETABYTE, 0.0088776331322151 },
        };

    }

}