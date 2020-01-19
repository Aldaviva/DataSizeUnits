using DataSizeUnits;
using Xunit;

namespace Tests {

    public class DataSizeTests {

        [Theory, MemberData(nameof(AutoScaleData))]
        public void AutoScale(long inputBytes, double expectedSize, Unit expectedUnit, bool useBytes) {
            DataSize actual = new DataSize(inputBytes).Normalize(!useBytes);
            Assert.Equal(expectedSize, actual.Quantity, 3);
            Assert.Equal(expectedUnit, actual.Unit);
        }

        public static TheoryData<long, double, Unit, bool> AutoScaleData => new TheoryData<long, double, Unit, bool> {
            { 0, 0.0, Unit.Byte, true },
            { 1, 1, Unit.Byte, true },
            { 1023, 1023.0, Unit.Byte, true },
            { 1024, 1.0, Unit.Kilobyte, true },
            { 1536, 1.5, Unit.Kilobyte, true },
            { 1024 * 1024, 1, Unit.Megabyte, true },
            { 1024 * 1024 * 1024, 1, Unit.Gigabyte, true },
            { 1024L * 1024 * 1024 * 1024, 1, Unit.Terabyte, true },
            { 1024L * 1024 * 1024 * 1024 * 1024, 1, Unit.Petabyte, true },
            { 1024L * 1024 * 1024 * 1024 * 1024 * 1024, 1, Unit.Exabyte, true },
            { 0, 0.0, Unit.Bit, false },
            { 1, 8, Unit.Bit, false },
            { 1023, 8.184, Unit.Kilobit, false },
            { 1024, 8.192, Unit.Kilobit, false },
            { 1536, 12.288, Unit.Kilobit, false },
            { 1024 * 1024, 8.388608, Unit.Megabit, false },
            { 1024 * 1024 * 1024, 8.589934592, Unit.Gigabit, false },
            { 1024L * 1024 * 1024 * 1024, 8.796093022208, Unit.Terabit, false },
            { 1024L * 1024 * 1024 * 1024 * 1024, 9.007199254740992, Unit.Petabit, false },
            { 1024L * 1024 * 1024 * 1024 * 1024 * 1024, 9.22337203685478, Unit.Exabit, false },
        };

        [Theory, MemberData(nameof(ScaleToData))]
        public void ManualScale(long inputBytes, Unit inputDestinationScale, double expectedValue) {
            DataSize actual = new DataSize(inputBytes).ConvertToUnit(inputDestinationScale);
            Assert.Equal(expectedValue, actual.Quantity, 3);
            Assert.Equal(inputDestinationScale, actual.Unit);
        }

        public static TheoryData<long, Unit, double> ScaleToData => new TheoryData<long, Unit, double> {
            { 0, Unit.Byte, 0 },
            { 0, Unit.Bit, 0 },
            { 0, Unit.Exabyte, 0 },
            { 0, Unit.Exabit, 0 },
            { 9_995_326_316_544, Unit.Byte, 9_995_326_316_544 },
            { 9_995_326_316_544, Unit.Kilobyte, 9_761_060_856 },
            { 9_995_326_316_544, Unit.Megabyte, 9_532_285.9921875 },
            { 9_995_326_316_544, Unit.Gigabyte, 9_308.873039245605 },
            { 9_995_326_316_544, Unit.Terabyte, 9.090696327388287 },
            { 9_995_326_316_544, Unit.Petabyte, 0.0088776331322151 },
            { 9_995_326_316_544, Unit.Exabyte, 0.0000086695636056 }
        };

        [Fact]
        public void ScaleUnitToUnit() {
            DataSize actual = new DataSize(150, Unit.Megabit).ConvertToUnit(Unit.Megabyte);
            Assert.Equal(17.8813934326171875, actual.Quantity, 3);
            Assert.Equal(Unit.Megabyte, actual.Unit);
        }

    }

}