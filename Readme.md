# DataSizeUnits
Deal with data size units in C# (bytes, kilobytes, megabytes, and others).

## Features
- **Convert a number of bytes to different units** (kilobytes, megabytes, gigabytes, terabytes, petabytes)
	- 2,097,152 bytes to kilobytes → 2048 KB
        ```cs
        double scaled = DataSize.convert(2_097_152, Unit.KILOBYTE);
        // scaled == 2048
        ```
- **Convert a number of bytes to an automatically-selected unit** based on its size
 	- 2,097,152 bytes → 2 MB
        ```cs
        (double scaled, Unit unit) = DataSize.convert(2_097_152);
        // scaled == 2
        // unit == Unit.MEGABYTE
        ```
    - 2,097,152 bytes → 16.78 mbit
        ```cs
        (double scaled, Unit unit) = DataSize.convert(2_097_152, false);
        // scaled == 16.78
        // unit == Unit.MEGABIT
        ```
 	- The unit will be automatically selected so the value is greater than or equal to 1 of that unit, and less than 1 of the next largest unit. For example, 2,097,152 bytes is greater than or equal to 1 MB and less than 1 GB, so it is converted to MB.
- **Convert between any units**, including any combination of bits, bytes, and their higher-order units (kilobits and kilobytes and the rest, up to and including petabits and petabytes)
	- 150 Mbit → 17.8 MByte
        ```cs
        double scaled = DataSize.convert(150, Unit.MEGABIT, Unit.MEGABYTE);
        // scaled == 17.8
        ```
- **Interpret** different suffices
	- Megabyte, MByte, mebibit, mib, mibit, MB, and M are all megabytes
        ```cs
        Unit unit = DataSize.forAbbreviation("mebibit");
        // unit == Unit.MEGABYTE
        ```
	- Default suffices for each unit are of the short, case-sensitive forms.
        ```cs
        string abbreviation = DataSize.toAbbreviation(Unit.TERABYTE);
        // abbreviation == "TB"
        ```
- **Format** bytes as a string with different unit and precision options
	- `{1536:KB1}` → `1.5 KB`
        ```cs
        string formatted = string.Format(DataSize.FORMATTER, "Size: {0:KB1}", 1536);
        // formatted == "Size: 1.5 KB"
        ```
    - The format specifier (`KB1` above) is made up of two optional parts, the destination unit (`KB`) and the precision (`1`).
    - The destination unit is the data size unit to which you want the input bytes to be converted. You can pass any data size unit abbreviation. Case matters for ambiguous units, like `kB`/`KB`/`K` for kilobytes and `kb`/`k`/`Kb` for kilobits. Unambiguous units like `kilobyte`/`kbyte`/`kibibit`/`kib`/`kibit` can be provided in any case. You can also specify `A` to automatically select the unit of bytes and higher magnitudes, and likewise `a` for bits, which is the default behavior if you omit the destination unit.
    - The precision is the number of digits after the decimal place. If you omit this, it will use the default number format value for the culture of the current thread, for example 2. Set this to `0` if you want integers only.

## Install
[This package is available in the NuGet Gallery.](https://www.nuget.org/packages/DataSizeUnits/)

This is a [.NET Standard 2.0](https://dotnet.microsoft.com/platform/dotnet-standard#versions) library, and can be depended upon in projects with .NET Framework 4.6.1, .NET Core 2.0, or later.

|Tool|Installation|
|---|---|
|Visual Studio NuGet Package Manager|Search for `DataSizeUnits`|
|Visual Studio Package Manager Console|`Install-Package DataSizeUnits`|
|.NET Core SDK CLI|`dotnet add package DataSizeUnits`|
