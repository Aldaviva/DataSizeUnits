DataSizeUnits
===

[![Build status](https://img.shields.io/github/actions/workflow/status/Aldaviva/DataSizeUnits/dotnetpackage.yml?branch=master&logo=github)](https://github.com/Aldaviva/DataSizeUnits/actions/workflows/dotnetpackage.yml) [![Test status](https://img.shields.io/testspace/tests/Aldaviva/Aldaviva:DataSizeUnits/master?passed_label=passing&failed_label=failing&logo=data%3Aimage%2Fsvg%2Bxml%3Bbase64%2CPHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCA4NTkgODYxIj48cGF0aCBkPSJtNTk4IDUxMy05NCA5NCAyOCAyNyA5NC05NC0yOC0yN3pNMzA2IDIyNmwtOTQgOTQgMjggMjggOTQtOTQtMjgtMjh6bS00NiAyODctMjcgMjcgOTQgOTQgMjctMjctOTQtOTR6bTI5My0yODctMjcgMjggOTQgOTQgMjctMjgtOTQtOTR6TTQzMiA4NjFjNDEuMzMgMCA3Ni44My0xNC42NyAxMDYuNS00NFM1ODMgNzUyIDU4MyA3MTBjMC00MS4zMy0xNC44My03Ni44My00NC41LTEwNi41UzQ3My4zMyA1NTkgNDMyIDU1OWMtNDIgMC03Ny42NyAxNC44My0xMDcgNDQuNXMtNDQgNjUuMTctNDQgMTA2LjVjMCA0MiAxNC42NyA3Ny42NyA0NCAxMDdzNjUgNDQgMTA3IDQ0em0wLTU1OWM0MS4zMyAwIDc2LjgzLTE0LjgzIDEwNi41LTQ0LjVTNTgzIDE5Mi4zMyA1ODMgMTUxYzAtNDItMTQuODMtNzcuNjctNDQuNS0xMDdTNDczLjMzIDAgNDMyIDBjLTQyIDAtNzcuNjcgMTQuNjctMTA3IDQ0cy00NCA2NS00NCAxMDdjMCA0MS4zMyAxNC42NyA3Ni44MyA0NCAxMDYuNVMzOTAgMzAyIDQzMiAzMDJ6bTI3NiAyODJjNDIgMCA3Ny42Ny0xNC44MyAxMDctNDQuNXM0NC02NS4xNyA0NC0xMDYuNWMwLTQyLTE0LjY3LTc3LjY3LTQ0LTEwN3MtNjUtNDQtMTA3LTQ0Yy00MS4zMyAwLTc2LjY3IDE0LjY3LTEwNiA0NHMtNDQgNjUtNDQgMTA3YzAgNDEuMzMgMTQuNjcgNzYuODMgNDQgMTA2LjVTNjY2LjY3IDU4NCA3MDggNTg0em0tNTU3IDBjNDIgMCA3Ny42Ny0xNC44MyAxMDctNDQuNXM0NC02NS4xNyA0NC0xMDYuNWMwLTQyLTE0LjY3LTc3LjY3LTQ0LTEwN3MtNjUtNDQtMTA3LTQ0Yy00MS4zMyAwLTc2LjgzIDE0LjY3LTEwNi41IDQ0UzAgMzkxIDAgNDMzYzAgNDEuMzMgMTQuODMgNzYuODMgNDQuNSAxMDYuNVMxMDkuNjcgNTg0IDE1MSA1ODR6IiBmaWxsPSIjZmZmIi8%2BPC9zdmc%2B)](https://aldaviva.testspace.com/spaces/194266) [![Coverage status](https://img.shields.io/coveralls/github/Aldaviva/DataSizeUnits?logo=coveralls)](https://coveralls.io/github/Aldaviva/DataSizeUnits?branch=master)

Convert and format data size units in .NET (bits, bytes, kilobits, kilobytes, and others).

## Features
- **Convert** between many units of digital information, including bits, bytes, and their higher-order units (kilobits and kilobytes and the rest, up to and including exabits and exabytes)
	- 150 Mbit → 17.8 MByte
        ```cs
        DataSize sizeInMegabytes = new DataSize(150, Unit.Megabit).ConvertToUnit(Unit.Megabyte);
        // sizeInMegabytes.Quantity == 17.8
        ```

- **Normalize** a number of bytes to an automatically-selected unit based on its magnitude
 	- 2,097,152 bytes → 2 MB
        ```cs
        DataSize normalized = new DataSize(2_097_152).Normalize();
        // normalized.Quantity == 2.0
        // normalized.Unit == Unit.Megabyte
        ```
 	- 2,097,152 bytes → 16.78 mbit
        ```cs
        DataSize normalized = new DataSize(2_097_152).Normalize(true);
        // normalized.Quantity == 16.78
        // normalized.Unit == Unit.Megabit
        ```
 	- The unit will be automatically selected so the value is greater than or equal to 1 of that unit, and less than 1 of the next largest unit. For example, 2,097,152 bytes is greater than or equal to 1 MB and less than 1 GB, so it is normalized to MB.

- **Parse and format** unit names and abbreviations
	- Megabyte, MByte, mebibyte, MiB, MB, and M are all megabytes
        ```cs
        Unit unit = DataSize.ParseUnit("MB");
        // unit == Unit.Megabyte
        ```
	- Abbreviations for each unit are of the short, case-sensitive forms.
        ```cs
        string abbreviation = Unit.Terabyte.ToAbbreviation();
        // abbreviation == "TB"
        ```
        ```cs
        string iecAbbreviation = Unit.Terabyte.ToAbbreviation(true);
        // iecAbbreviation == "TiB";
        ```
    - Get the unit names in JEDEC (`MB`) or IEC (`MiB`) variants.
        ```cs
        string name = Unit.Terabyte.ToName();
        // name == "terabyte";
        ```
        ```cs
        string iecName = Unit.Terabyte.ToName(true);
        // iecName == "tebibyte";
        ```

- **Format** bytes as a string with different unit and precision options
	- 1,536 bytes to kilobytes, 1 digit after the decimal point → `1.5 KB`
        ```cs
        string formatted = new DataSize(1536).ConvertToUnit(Unit.Kilobyte).ToString(1);
        // formatted == "1.5 KB"
        ```
        ```cs
        string formatted = string.Format(new DataSizeFormatter(), "Size: {0:KB1}", 1536);
        // formatted == "Size: 1.5 KB"
        ```
        ```cs
        string formatted = string.Format(new DataSizeFormatter(), "Size: {0:A1}", 1536);
        // formatted == "Size: 1.5 KB"
        ```
    - The format specifier (like `KB1` above) is made up of two optional parts, the destination unit (`KB`) and the precision (`1`).
    - The destination unit (`KB`) is the data size unit to which you want the input bytes to be converted. You can also specify **`A`** to automatically normalize the unit of bytes and higher magnitudes, which is the default behavior if you omit the destination unit, and **`a`** normalizes to bits. Case matters for ambiguous units, like `kB`/`KB`/`K` for kilobytes and `kb`/`k`/`Kb` for kilobits. Unambiguous units like `kilobyte`/`kbyte`/`kibibyte`/`kib` can be provided in any case. 
    - The precision (`1`) is the number of digits after the decimal place. If you omit this, it will use the default number value for the culture of the current thread, for example 2. Set this to `0` if you want integers only.

## Install
[This package is available in NuGet Gallery.](https://www.nuget.org/packages/DataSizeUnits/)

This is a [.NET Standard 2.0](https://dotnet.microsoft.com/platform/dotnet-standard#versions) library, and can be depended upon in projects with .NET Framework 4.6.1, .NET Core 2.0, or later.

|Tool|Installation|
|---|---|
|Visual Studio NuGet Package Manager|Search for `DataSizeUnits`|
|Visual Studio Package Manager Console|`Install-Package DataSizeUnits`|
|.NET Core SDK CLI|`dotnet add package DataSizeUnits`|
