using System;
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

        [Theory, MemberData(nameof(EqualityData))]
        public void Equality(double quantity1, Unit unit1, double quantity2, Unit unit2) {
            var dataSize1 = new DataSize(quantity1, unit1);
            var dataSize2 = new DataSize(quantity2, unit2);

            Assert.True(dataSize1.Equals(dataSize2));
            Assert.True(dataSize1.Equals((object) dataSize2));
            Assert.True(dataSize1 == dataSize2);
            Assert.True(dataSize1 >= dataSize2);
            Assert.True(dataSize1 <= dataSize2);
            Assert.False(dataSize1 != dataSize2);
            Assert.False(dataSize1 < dataSize2);
            Assert.False(dataSize1 > dataSize2);
            Assert.Equal(dataSize1.GetHashCode(), dataSize2.GetHashCode());
            Assert.Equal(0, dataSize1.CompareTo(dataSize2));
        }

        public static TheoryData<double, Unit, double, Unit> EqualityData => new TheoryData<double, Unit, double, Unit> {
            { 1, Unit.Megabyte, 1, Unit.Megabyte },
            { 1, Unit.Megabyte, 1024, Unit.Kilobyte },
            { 1, Unit.Megabyte, 1048576, Unit.Byte },
            { 1, Unit.Megabyte, 8388608, Unit.Bit }
        };

        [Theory, MemberData(nameof(InequalityData))]
        public void Inequality(double quantity1, Unit unit1, double quantity2, Unit unit2, bool isInput2BiggerThanInput1) {
            var dataSize1 = new DataSize(quantity1, unit1);
            var dataSize2 = new DataSize(quantity2, unit2);

            Assert.False(dataSize1.Equals(dataSize2));
            Assert.False(dataSize1 == dataSize2);
            Assert.True(dataSize1 != dataSize2);
            Assert.NotEqual(dataSize1.GetHashCode(), dataSize2.GetHashCode());
            int comparison = dataSize1.CompareTo(dataSize2);
            if (isInput2BiggerThanInput1) {
                Assert.True(comparison < 0);
                Assert.True(dataSize1 < dataSize2);
                Assert.True(dataSize1 <= dataSize2);
            } else {
                Assert.True(comparison > 0);
                Assert.True(dataSize1 > dataSize2);
                Assert.True(dataSize1 >= dataSize2);
            }
        }

        public static TheoryData<double, Unit, double, Unit, bool> InequalityData => new TheoryData<double, Unit, double, Unit, bool> {
            { 1, Unit.Kilobyte, 1, Unit.Megabyte, true },
            { 1.4, Unit.Megabyte, 1.44, Unit.Megabyte, true },
            { 1, Unit.Gigabyte, 1000, Unit.Megabyte, false },
            { 1, Unit.Megabyte, 1, Unit.Megabit, false }
        };

        [Fact]
        public void Addition() {
            DataSize actual = new DataSize(1, Unit.Megabyte) + new DataSize(2, Unit.Megabyte);
            Assert.Equal(3, actual.Quantity);
            Assert.Equal(Unit.Megabyte, actual.Unit);
        }

        [Fact]
        public void Subtraction() {
            DataSize actual = new DataSize(3, Unit.Megabyte) - new DataSize(2048, Unit.Kilobyte);
            Assert.Equal(1, actual.Quantity);
            Assert.Equal(Unit.Megabyte, actual.Unit);
        }

        [Fact]
        public void Multiplication() {
            DataSize actual = new DataSize(1, Unit.Megabyte) * 3;
            Assert.Equal(3, actual.Quantity);
            Assert.Equal(Unit.Megabyte, actual.Unit);
        }

        [Fact]
        public void Division() {
            DataSize actual = new DataSize(6, Unit.Megabyte) / 3;
            Assert.Equal(2, actual.Quantity);
            Assert.Equal(Unit.Megabyte, actual.Unit);
        }

        [Fact]
        public void DivisionByZero() {
            Assert.Throws<DivideByZeroException>(() => new DataSize(1) / 0);
        }

        [Fact]
        public void ToBytes() {
            long actual = new DataSize(1, Unit.Kilobyte);
            Assert.Equal(1024, actual);
        }

        [Fact]
        public void FromBytes() {
            DataSize actual = 1024;
            Assert.Equal(1024, actual.Quantity);
            Assert.Equal(Unit.Byte, actual.Unit);
        }
    }

}