namespace Tests;

public class ReadmeSnippets {

    [Fact]
    public void Convert() {
        double sizeInMegabytes = new DataSize(150, DataSizeUnit.Megabit).AsUnit(DataSizeUnit.Megabyte);
        // sizeInMegabytes == 17.8
        Assert.Equal(17.8, sizeInMegabytes, 0.1);
    }

    [Fact]
    public void Normalize() {
        (double quantity, DataSizeUnit unit) normalized = new DataSize(2_097_152).AsAutomaticUnit();
        // normalized.quantity == 2.0
        // normalized.unit == DataSizeUnit.Megabyte
        Assert.Equal(2, normalized.quantity);
        Assert.Equal(DataSizeUnit.Megabyte, normalized.unit);

        normalized = new DataSize(2_097_152).AsAutomaticUnit(true); // pass true to get bits units instead of bytes
        // normalized.quantity == 16.78
        // normalized.unit == DataSizeUnit.Megabit
        Assert.Equal(16.78, normalized.quantity, 0.01);
        Assert.Equal(DataSizeUnit.Megabit, normalized.unit);
    }

    [Fact]
    public void Units() {
        DataSizeUnit? unit = DataSizeUnit.Parse("MB");
        // unit == DataSizeUnit.Megabyte
        Assert.Equal(DataSizeUnit.Megabyte, unit);

        string abbreviation = DataSizeUnit.Terabyte.ToAbbreviation();
        // abbreviation == "TB"
        Assert.Equal("TB", abbreviation);

        string iecAbbreviation = DataSizeUnit.Terabyte.ToAbbreviation(true); // pass true for the IEC abbreviations (kibibyte, etc.)
        // iecAbbreviation == "TiB"
        Assert.Equal("TiB", iecAbbreviation);

        string name = DataSizeUnit.Terabyte.ToName();
        // name == "terabyte"
        Assert.Equal("terabyte", name);

        string iecName = DataSizeUnit.Terabyte.ToName(true); // pass true for the IEC names (kibibyte, etc.)
        // iecName == "tebibyte"
        Assert.Equal("tebibyte", iecName);
    }

    [Fact]
    public void Parsing() {
        DataSize parsed = DataSize.Parse("1.5 MB");
        // parsed.Bytes == 1572864
        Assert.Equal(1572864, parsed.Bytes);
    }

    [Fact]
    public void Format() {
        string formatted = new DataSize(1572864).ToString(); // automatic precision (from culture), in specified unit
        // formatted == "1.50 MB"
        Assert.Equal("1.50 MB", formatted);

        formatted = new DataSize(1572864).ToString(DataSizeUnit.Kilobyte); // automatic precision (from culture), in specified unit
        // formatted == "1,536.00 kB"
        Assert.Equal("1,536.00 kB", formatted);

        formatted = new DataSize(1572864).ToString(1); // precision 1, in automatic byte-based units
        // formatted == "1.5 MB"
        Assert.Equal("1.5 MB", formatted);

        formatted = new DataSize(1572864).ToString(true); // automatic precision (from culture), in automatic bit-based units
        // formatted == "12.58 mb"
        Assert.Equal("12.58 mb", formatted);
    }

}