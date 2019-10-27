using System;
using DataSizeUnits;
using Xunit;

namespace Tests {

    public class DataSizeTests {

        [Theory, MemberData(nameof(autoScaleData))]
        public void autoScale(long inputBytes, double expectedSize, DataSize.Unit expectedUnit, bool useBytes) {
            (double actualSize, DataSize.Unit actualUnit) = DataSize.convert(inputBytes, useBytes);
            Assert.Equal(expectedSize, actualSize, 3);
            Assert.Equal(expectedUnit, actualUnit);
        }

        public static TheoryData<long, double, DataSize.Unit, bool> autoScaleData => new TheoryData<long, double, DataSize.Unit, bool> {
            { 0, 0.0, DataSize.Unit.BYTE, true },
            { 1, 1, DataSize.Unit.BYTE, true },
            { 1023, 1023.0, DataSize.Unit.BYTE, true },
            { 1024, 1.0, DataSize.Unit.KILOBYTE, true },
            { 1536, 1.5, DataSize.Unit.KILOBYTE, true },
            { 1024 * 1024, 1, DataSize.Unit.MEGABYTE, true },
            { 1024 * 1024 * 1024, 1, DataSize.Unit.GIGABYTE, true },
            { 1024L * 1024 * 1024 * 1024, 1, DataSize.Unit.TERABYTE, true },
            { 1024L * 1024 * 1024 * 1024 * 1024, 1, DataSize.Unit.PETABYTE, true },
            { 0, 0.0, DataSize.Unit.BIT, false },
            { 1, 8, DataSize.Unit.BIT, false },
            { 1023, 8.184, DataSize.Unit.KILOBIT, false },
            { 1024, 8.192, DataSize.Unit.KILOBIT, false },
            { 1536, 12.288, DataSize.Unit.KILOBIT, false },
            { 1024 * 1024, 8.388608, DataSize.Unit.MEGABIT, false },
            { 1024 * 1024 * 1024, 8.589934592, DataSize.Unit.GIGABIT, false },
            { 1024L * 1024 * 1024 * 1024, 8.796093022208, DataSize.Unit.TERABIT, false },
            { 1024L * 1024 * 1024 * 1024 * 1024, 9.007199254740992, DataSize.Unit.PETABIT, false },
        };

        [Theory, MemberData(nameof(scaleToData))]
        public void manualScale(long inputBytes, DataSize.Unit inputDestinationScale, double expectedValue) {
            double actualValue = DataSize.convert(inputBytes, inputDestinationScale);
            Assert.Equal(expectedValue, actualValue, 3);
        }

        public static TheoryData<long, DataSize.Unit, double> scaleToData => new TheoryData<long, DataSize.Unit, double> {
            { 0, DataSize.Unit.BYTE, 0 },
            { 0, DataSize.Unit.BIT, 0 },
            { 0, DataSize.Unit.PETABYTE, 0 },
            { 0, DataSize.Unit.PETABIT, 0 },
            { 9_995_326_316_544, DataSize.Unit.BYTE, 9_995_326_316_544 },
            { 9_995_326_316_544, DataSize.Unit.KILOBYTE, 9_761_060_856 },
            { 9_995_326_316_544, DataSize.Unit.MEGABYTE, 9_532_285.9921875 },
            { 9_995_326_316_544, DataSize.Unit.GIGABYTE, 9_308.873039245605 },
            { 9_995_326_316_544, DataSize.Unit.TERABYTE, 9.090696327388287 },
            { 9_995_326_316_544, DataSize.Unit.PETABYTE, 0.0088776331322151 },
        };

        [Fact]
        public void scaleUnitToUnit() {
            double actual = DataSize.convert(150, DataSize.Unit.MEGABIT, DataSize.Unit.MEGABYTE);
            Assert.Equal(17.8813934326171875, actual, 3);
        }

        [Fact]
        public void unsupportedMagnitude() {
            Assert.Throws<ArgumentOutOfRangeException>(() => DataSize.convert(long.MaxValue, true));
        }

    }

}