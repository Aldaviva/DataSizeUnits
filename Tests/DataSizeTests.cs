namespace Tests;

public class DataSizeTests {

    [Theory] [MemberData(nameof(NormalizeData))]
    public void Normalize(long inputBytes, double expectedSize, DataSizeUnit expectedUnit, bool useBytes) {
        (double quantity, DataSizeUnit unit) actual = new DataSize(inputBytes).AsAutomaticUnit(!useBytes);

        Assert.Equal(expectedUnit, actual.unit);
        Assert.Equal(expectedSize, actual.quantity, 3);
    }

    public static TheoryData<long, double, DataSizeUnit, bool> NormalizeData => new() {
        { 0, 0.0, DataSizeUnit.Byte, true },
        { 1, 1, DataSizeUnit.Byte, true },
        { 1023, 1023.0, DataSizeUnit.Byte, true },
        { 1024, 1.0, DataSizeUnit.Kilobyte, true },
        { 1536, 1.5, DataSizeUnit.Kilobyte, true },
        { 1024 * 1024, 1, DataSizeUnit.Megabyte, true },
        { 1024 * 1024 * 1024, 1, DataSizeUnit.Gigabyte, true },
        { 1024L * 1024 * 1024 * 1024, 1, DataSizeUnit.Terabyte, true },
        { 1024L * 1024 * 1024 * 1024 * 1024, 1, DataSizeUnit.Petabyte, true },
        { 1024L * 1024 * 1024 * 1024 * 1024 * 1024, 1, DataSizeUnit.Exabyte, true },
        { 0, 0.0, DataSizeUnit.Bit, false },
        { 1, 8, DataSizeUnit.Bit, false },
        { 1023, 8.184, DataSizeUnit.Kilobit, false },
        { 1024, 8.192, DataSizeUnit.Kilobit, false },
        { 1536, 12.288, DataSizeUnit.Kilobit, false },
        { 1024 * 1024, 8.388608, DataSizeUnit.Megabit, false },
        { 1024 * 1024 * 1024, 8.589934592, DataSizeUnit.Gigabit, false },
        { 1024L * 1024 * 1024 * 1024, 8.796093022208, DataSizeUnit.Terabit, false },
        { 1024L * 1024 * 1024 * 1024 * 1024, 9.007199254740992, DataSizeUnit.Petabit, false },
        { 1024L * 1024 * 1024 * 1024 * 1024 * 1024, 9.22337203685478, DataSizeUnit.Exabit, false },
        { 125_000_000_000_000_000, 1, DataSizeUnit.Exabit, false },
        { 124_999_937_499_999_991, 999.999, DataSizeUnit.Petabit, false },
    };

    [Theory] [MemberData(nameof(ScaleToData))]
    public void ManualScale(long inputBytes, DataSizeUnit inputDestinationScale, double expectedValue) {
        double actual = new DataSize(inputBytes).AsUnit(inputDestinationScale);

        Assert.Equal(expectedValue, actual, 3);
    }

    public static TheoryData<long, DataSizeUnit, double> ScaleToData => new() {
        { 0, DataSizeUnit.Byte, 0 },
        { 0, DataSizeUnit.Bit, 0 },
        { 0, DataSizeUnit.Exabyte, 0 },
        { 0, DataSizeUnit.Exabit, 0 },
        { 9_995_326_316_544, DataSizeUnit.Byte, 9_995_326_316_544 },
        { 9_995_326_316_544, DataSizeUnit.Kilobyte, 9_761_060_856 },
        { 9_995_326_316_544, DataSizeUnit.Megabyte, 9_532_285.9921875 },
        { 9_995_326_316_544, DataSizeUnit.Gigabyte, 9_308.873039245605 },
        { 9_995_326_316_544, DataSizeUnit.Terabyte, 9.090696327388287 },
        { 9_995_326_316_544, DataSizeUnit.Petabyte, 0.0088776331322151 },
        { 9_995_326_316_544, DataSizeUnit.Exabyte, 0.0000086695636056 }
    };

    [Fact]
    public void ScaleUnitToUnit() {
        double actual = new DataSize(150, DataSizeUnit.Megabit).AsUnit(DataSizeUnit.Megabyte);

        Assert.Equal(17.8813934326171875, actual, 3);
    }

    [Theory] [MemberData(nameof(EqualityData))]
    public void Equality(ulong quantity1, DataSizeUnit unit1, ulong quantity2, DataSizeUnit unit2) {
        DataSize dataSize1 = new(quantity1, unit1);
        DataSize dataSize2 = new(quantity2, unit2);

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

    public static TheoryData<ulong, DataSizeUnit, ulong, DataSizeUnit> EqualityData => new() {
        { 1, DataSizeUnit.Megabyte, 1, DataSizeUnit.Megabyte },
        { 1, DataSizeUnit.Megabyte, 1024, DataSizeUnit.Kilobyte },
        { 1, DataSizeUnit.Megabyte, 1048576, DataSizeUnit.Byte },
        { 1, DataSizeUnit.Megabyte, 8388608, DataSizeUnit.Bit }
    };

    [Theory] [MemberData(nameof(InequalityData))]
    public void Inequality(DataSize dataSize1, DataSize dataSize2, bool isInput2BiggerThanInput1) {
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

    public static TheoryData<DataSize, DataSize, bool> InequalityData => new() {
        { new DataSize(1, DataSizeUnit.Kilobyte), new DataSize(1, DataSizeUnit.Megabyte), true },
        { new DataSize(1.4, DataSizeUnit.Megabyte), new DataSize(1.44, DataSizeUnit.Megabyte), true },
        { new DataSize(1, DataSizeUnit.Gigabyte), new DataSize(1000, DataSizeUnit.Megabyte), false },
        { new DataSize(1, DataSizeUnit.Megabyte), new DataSize(1, DataSizeUnit.Megabit), false }
    };

    [Fact]
    public void Addition() {
        DataSize actual = new DataSize(1, DataSizeUnit.Megabyte) + new DataSize(2, DataSizeUnit.Megabyte);
        Assert.Equal(3 * 1024 * 1024, actual.Bytes);
    }

    [Fact]
    public void Subtraction() {
        DataSize actual = new DataSize(3, DataSizeUnit.Megabyte) - new DataSize(2048, DataSizeUnit.Kilobyte);
        Assert.Equal(1024 * 1024, actual.Bytes);
    }

    [Fact]
    public void Multiplication() {
        DataSize actual = new DataSize(1, DataSizeUnit.Megabyte) * 3;
        Assert.Equal(3 * 1024 * 1024, actual.Bytes);
    }

    [Fact]
    public void DivisionByDouble() {
        DataSize actual = new DataSize(6, DataSizeUnit.Megabyte) / 3;
        Assert.Equal(2 * 1024 * 1024, actual.Bytes);
    }

    [Fact]
    public void DivisionByDatasize() {
        double actual = new DataSize(6, DataSizeUnit.Megabyte) / new DataSize(3, DataSizeUnit.Megabyte);
        Assert.Equal(2, actual);
    }

    [Fact]
    public void DivisionByZero() {
        Assert.Throws<DivideByZeroException>(() => new DataSize(1) / 0);
        Assert.Throws<DivideByZeroException>(() => new DataSize(1) / new DataSize(0));
    }

    [Fact]
    public void ToBytes() {
        long actual = (long) new DataSize(1, DataSizeUnit.Kilobyte);
        Assert.Equal(1024, actual);
    }

    [Fact]
    public void FromBytes() {
        DataSize actual = 1024;
        Assert.Equal(1024, actual.Bytes);
    }

    [Fact]
    public void FromUnsignedLong() {
        DataSize actual = new(1024ul);
        Assert.Equal(1024, actual.Bytes);
    }

    [Fact]
    public void ParameterlessConstructor() {
        DataSize actual = new();
        Assert.Equal(0, actual.Bytes);
    }

}