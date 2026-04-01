namespace Tests;

/// <summary>
/// lol
/// </summary>
public class UnitTests {

    [Fact]
    public void JedecNames() {
        Assert.Equal("byte", DataSizeUnit.Byte.ToName());
        Assert.Equal("kilobyte", DataSizeUnit.Kilobyte.ToName());
        Assert.Equal("megabyte", DataSizeUnit.Megabyte.ToName());
        Assert.Equal("gigabyte", DataSizeUnit.Gigabyte.ToName());
        Assert.Equal("terabyte", DataSizeUnit.Terabyte.ToName());
        Assert.Equal("petabyte", DataSizeUnit.Petabyte.ToName());
        Assert.Equal("exabyte", DataSizeUnit.Exabyte.ToName());
        Assert.Equal("bit", DataSizeUnit.Bit.ToName());
        Assert.Equal("kilobit", DataSizeUnit.Kilobit.ToName());
        Assert.Equal("megabit", DataSizeUnit.Megabit.ToName());
        Assert.Equal("gigabit", DataSizeUnit.Gigabit.ToName());
        Assert.Equal("terabit", DataSizeUnit.Terabit.ToName());
        Assert.Equal("petabit", DataSizeUnit.Petabit.ToName());
        Assert.Equal("exabit", DataSizeUnit.Exabit.ToName());
    }

    [Fact]
    public void JedecAbbreviations() {
        Assert.Equal("B", DataSizeUnit.Byte.ToAbbreviation());
        Assert.Equal("kB", DataSizeUnit.Kilobyte.ToAbbreviation());
        Assert.Equal("MB", DataSizeUnit.Megabyte.ToAbbreviation());
        Assert.Equal("GB", DataSizeUnit.Gigabyte.ToAbbreviation());
        Assert.Equal("TB", DataSizeUnit.Terabyte.ToAbbreviation());
        Assert.Equal("PB", DataSizeUnit.Petabyte.ToAbbreviation());
        Assert.Equal("EB", DataSizeUnit.Exabyte.ToAbbreviation());
        Assert.Equal("b", DataSizeUnit.Bit.ToAbbreviation());
        Assert.Equal("kb", DataSizeUnit.Kilobit.ToAbbreviation());
        Assert.Equal("mb", DataSizeUnit.Megabit.ToAbbreviation());
        Assert.Equal("gb", DataSizeUnit.Gigabit.ToAbbreviation());
        Assert.Equal("tb", DataSizeUnit.Terabit.ToAbbreviation());
        Assert.Equal("pb", DataSizeUnit.Petabit.ToAbbreviation());
        Assert.Equal("eb", DataSizeUnit.Exabit.ToAbbreviation());
    }

    [Fact]
    public void IecNames() {
        Assert.Equal("byte", DataSizeUnit.Byte.ToName(true));
        Assert.Equal("kibibyte", DataSizeUnit.Kilobyte.ToName(true));
        Assert.Equal("mebibyte", DataSizeUnit.Megabyte.ToName(true));
        Assert.Equal("gibibyte", DataSizeUnit.Gigabyte.ToName(true));
        Assert.Equal("tebibyte", DataSizeUnit.Terabyte.ToName(true));
        Assert.Equal("pebibyte", DataSizeUnit.Petabyte.ToName(true));
        Assert.Equal("exbibyte", DataSizeUnit.Exabyte.ToName(true));
        Assert.Equal("bit", DataSizeUnit.Bit.ToName(true));
        Assert.Equal("kilobit", DataSizeUnit.Kilobit.ToName(true));
        Assert.Equal("megabit", DataSizeUnit.Megabit.ToName(true));
        Assert.Equal("gigabit", DataSizeUnit.Gigabit.ToName(true));
        Assert.Equal("terabit", DataSizeUnit.Terabit.ToName(true));
        Assert.Equal("petabit", DataSizeUnit.Petabit.ToName(true));
        Assert.Equal("exabit", DataSizeUnit.Exabit.ToName(true));
    }

    [Fact]
    public void IecAbbreviations() {
        Assert.Equal("B", DataSizeUnit.Byte.ToAbbreviation(true));
        Assert.Equal("KiB", DataSizeUnit.Kilobyte.ToAbbreviation(true));
        Assert.Equal("MiB", DataSizeUnit.Megabyte.ToAbbreviation(true));
        Assert.Equal("GiB", DataSizeUnit.Gigabyte.ToAbbreviation(true));
        Assert.Equal("TiB", DataSizeUnit.Terabyte.ToAbbreviation(true));
        Assert.Equal("PiB", DataSizeUnit.Petabyte.ToAbbreviation(true));
        Assert.Equal("EiB", DataSizeUnit.Exabyte.ToAbbreviation(true));
        Assert.Equal("b", DataSizeUnit.Bit.ToAbbreviation(true));
        Assert.Equal("kb", DataSizeUnit.Kilobit.ToAbbreviation(true));
        Assert.Equal("mb", DataSizeUnit.Megabit.ToAbbreviation(true));
        Assert.Equal("gb", DataSizeUnit.Gigabit.ToAbbreviation(true));
        Assert.Equal("tb", DataSizeUnit.Terabit.ToAbbreviation(true));
        Assert.Equal("pb", DataSizeUnit.Petabit.ToAbbreviation(true));
        Assert.Equal("eb", DataSizeUnit.Exabit.ToAbbreviation(true));
    }

    [Fact]
    public void IsMultipleOfBits() {
        Assert.False(DataSizeUnit.Byte.IsMultipleOfBits);
        Assert.False(DataSizeUnit.Kilobyte.IsMultipleOfBits);
        Assert.False(DataSizeUnit.Megabyte.IsMultipleOfBits);
        Assert.False(DataSizeUnit.Gigabyte.IsMultipleOfBits);
        Assert.False(DataSizeUnit.Terabyte.IsMultipleOfBits);
        Assert.False(DataSizeUnit.Petabyte.IsMultipleOfBits);
        Assert.False(DataSizeUnit.Exabyte.IsMultipleOfBits);
        Assert.True(DataSizeUnit.Bit.IsMultipleOfBits);
        Assert.True(DataSizeUnit.Kilobit.IsMultipleOfBits);
        Assert.True(DataSizeUnit.Megabit.IsMultipleOfBits);
        Assert.True(DataSizeUnit.Gigabit.IsMultipleOfBits);
        Assert.True(DataSizeUnit.Terabit.IsMultipleOfBits);
        Assert.True(DataSizeUnit.Petabit.IsMultipleOfBits);
        Assert.True(DataSizeUnit.Exabit.IsMultipleOfBits);

        Assert.Throws<ArgumentOutOfRangeException>(() => ((DataSizeUnit) 999999).IsMultipleOfBits);
    }

}