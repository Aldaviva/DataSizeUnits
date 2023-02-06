using System;
using DataSizeUnits;
using Xunit;

namespace Tests; 

/// <summary>
/// lol
/// </summary>
public class UnitTests {

    [Fact]
    public void JedecNames() {
        Assert.Equal("byte", Unit.Byte.ToName());
        Assert.Equal("kilobyte", Unit.Kilobyte.ToName());
        Assert.Equal("megabyte", Unit.Megabyte.ToName());
        Assert.Equal("gigabyte", Unit.Gigabyte.ToName());
        Assert.Equal("terabyte", Unit.Terabyte.ToName());
        Assert.Equal("petabyte", Unit.Petabyte.ToName());
        Assert.Equal("exabyte", Unit.Exabyte.ToName());
        Assert.Equal("bit", Unit.Bit.ToName());
        Assert.Equal("kilobit", Unit.Kilobit.ToName());
        Assert.Equal("megabit", Unit.Megabit.ToName());
        Assert.Equal("gigabit", Unit.Gigabit.ToName());
        Assert.Equal("terabit", Unit.Terabit.ToName());
        Assert.Equal("petabit", Unit.Petabit.ToName());
        Assert.Equal("exabit", Unit.Exabit.ToName());
    }

    [Fact]
    public void JedecAbbreviations() {
        Assert.Equal("B", Unit.Byte.ToAbbreviation());
        Assert.Equal("KB", Unit.Kilobyte.ToAbbreviation());
        Assert.Equal("MB", Unit.Megabyte.ToAbbreviation());
        Assert.Equal("GB", Unit.Gigabyte.ToAbbreviation());
        Assert.Equal("TB", Unit.Terabyte.ToAbbreviation());
        Assert.Equal("PB", Unit.Petabyte.ToAbbreviation());
        Assert.Equal("EB", Unit.Exabyte.ToAbbreviation());
        Assert.Equal("b", Unit.Bit.ToAbbreviation());
        Assert.Equal("kb", Unit.Kilobit.ToAbbreviation());
        Assert.Equal("mb", Unit.Megabit.ToAbbreviation());
        Assert.Equal("gb", Unit.Gigabit.ToAbbreviation());
        Assert.Equal("tb", Unit.Terabit.ToAbbreviation());
        Assert.Equal("pb", Unit.Petabit.ToAbbreviation());
        Assert.Equal("eb", Unit.Exabit.ToAbbreviation());
    }

    [Fact]
    public void IecNames() {
        Assert.Equal("byte", Unit.Byte.ToName(true));
        Assert.Equal("kibibyte", Unit.Kilobyte.ToName(true));
        Assert.Equal("mebibyte", Unit.Megabyte.ToName(true));
        Assert.Equal("gibibyte", Unit.Gigabyte.ToName(true));
        Assert.Equal("tebibyte", Unit.Terabyte.ToName(true));
        Assert.Equal("pebibyte", Unit.Petabyte.ToName(true));
        Assert.Equal("exbibyte", Unit.Exabyte.ToName(true));
        Assert.Equal("bit", Unit.Bit.ToName(true));
        Assert.Equal("kilobit", Unit.Kilobit.ToName(true));
        Assert.Equal("megabit", Unit.Megabit.ToName(true));
        Assert.Equal("gigabit", Unit.Gigabit.ToName(true));
        Assert.Equal("terabit", Unit.Terabit.ToName(true));
        Assert.Equal("petabit", Unit.Petabit.ToName(true));
        Assert.Equal("exabit", Unit.Exabit.ToName(true));
    }

    [Fact]
    public void IecAbbreviations() {
        Assert.Equal("B", Unit.Byte.ToAbbreviation(true));
        Assert.Equal("KiB", Unit.Kilobyte.ToAbbreviation(true));
        Assert.Equal("MiB", Unit.Megabyte.ToAbbreviation(true));
        Assert.Equal("GiB", Unit.Gigabyte.ToAbbreviation(true));
        Assert.Equal("TiB", Unit.Terabyte.ToAbbreviation(true));
        Assert.Equal("PiB", Unit.Petabyte.ToAbbreviation(true));
        Assert.Equal("EiB", Unit.Exabyte.ToAbbreviation(true));
        Assert.Equal("b", Unit.Bit.ToAbbreviation(true));
        Assert.Equal("kb", Unit.Kilobit.ToAbbreviation(true));
        Assert.Equal("mb", Unit.Megabit.ToAbbreviation(true));
        Assert.Equal("gb", Unit.Gigabit.ToAbbreviation(true));
        Assert.Equal("tb", Unit.Terabit.ToAbbreviation(true));
        Assert.Equal("pb", Unit.Petabit.ToAbbreviation(true));
        Assert.Equal("eb", Unit.Exabit.ToAbbreviation(true));
    }

    [Fact]
    public void CreateDataSizeFromUnit() {
        DataSize actual = Unit.Megabyte.Quantity(4);
        Assert.Equal(4, actual.Quantity);
        Assert.Equal(Unit.Megabyte, actual.Unit);
    }

    [Fact]
    public void IsMultipleOfBits() {
        Assert.False(Unit.Byte.IsMultipleOfBits());
        Assert.False(Unit.Kilobyte.IsMultipleOfBits());
        Assert.False(Unit.Megabyte.IsMultipleOfBits());
        Assert.False(Unit.Gigabyte.IsMultipleOfBits());
        Assert.False(Unit.Terabyte.IsMultipleOfBits());
        Assert.False(Unit.Petabyte.IsMultipleOfBits());
        Assert.False(Unit.Exabyte.IsMultipleOfBits());
        Assert.True(Unit.Bit.IsMultipleOfBits());
        Assert.True(Unit.Kilobit.IsMultipleOfBits());
        Assert.True(Unit.Megabit.IsMultipleOfBits());
        Assert.True(Unit.Gigabit.IsMultipleOfBits());
        Assert.True(Unit.Terabit.IsMultipleOfBits());
        Assert.True(Unit.Petabit.IsMultipleOfBits());
        Assert.True(Unit.Exabit.IsMultipleOfBits());

        Assert.Throws<ArgumentOutOfRangeException>(() => ((Unit) 999999).IsMultipleOfBits());
    }

}