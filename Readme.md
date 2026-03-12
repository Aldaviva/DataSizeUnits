💾💿 DataSizeUnits
===

[![Package Version](https://img.shields.io/nuget/v/DataSizeUnits?logo=nuget)](https://www.nuget.org/packages/DataSizeUnits/) [![NuGet Gallery Download Count](https://img.shields.io/nuget/dt/DataSizeUnits?logo=nuget&color=blue
)](https://www.nuget.org/packages/DataSizeUnits/) [![Build status](https://img.shields.io/github/actions/workflow/status/Aldaviva/DataSizeUnits/dotnetpackage.yml?branch=master&logo=github)](https://github.com/Aldaviva/DataSizeUnits/actions/workflows/dotnetpackage.yml) [![Test status](https://img.shields.io/testspace/tests/Aldaviva/Aldaviva:DataSizeUnits/master?passed_label=passing&failed_label=failing&logo=data%3Aimage%2Fsvg%2Bxml%3Bbase64%2CPHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCA4NTkgODYxIj48cGF0aCBkPSJtNTk4IDUxMy05NCA5NCAyOCAyNyA5NC05NC0yOC0yN3pNMzA2IDIyNmwtOTQgOTQgMjggMjggOTQtOTQtMjgtMjh6bS00NiAyODctMjcgMjcgOTQgOTQgMjctMjctOTQtOTR6bTI5My0yODctMjcgMjggOTQgOTQgMjctMjgtOTQtOTR6TTQzMiA4NjFjNDEuMzMgMCA3Ni44My0xNC42NyAxMDYuNS00NFM1ODMgNzUyIDU4MyA3MTBjMC00MS4zMy0xNC44My03Ni44My00NC41LTEwNi41UzQ3My4zMyA1NTkgNDMyIDU1OWMtNDIgMC03Ny42NyAxNC44My0xMDcgNDQuNXMtNDQgNjUuMTctNDQgMTA2LjVjMCA0MiAxNC42NyA3Ny42NyA0NCAxMDdzNjUgNDQgMTA3IDQ0em0wLTU1OWM0MS4zMyAwIDc2LjgzLTE0LjgzIDEwNi41LTQ0LjVTNTgzIDE5Mi4zMyA1ODMgMTUxYzAtNDItMTQuODMtNzcuNjctNDQuNS0xMDdTNDczLjMzIDAgNDMyIDBjLTQyIDAtNzcuNjcgMTQuNjctMTA3IDQ0cy00NCA2NS00NCAxMDdjMCA0MS4zMyAxNC42NyA3Ni44MyA0NCAxMDYuNVMzOTAgMzAyIDQzMiAzMDJ6bTI3NiAyODJjNDIgMCA3Ny42Ny0xNC44MyAxMDctNDQuNXM0NC02NS4xNyA0NC0xMDYuNWMwLTQyLTE0LjY3LTc3LjY3LTQ0LTEwN3MtNjUtNDQtMTA3LTQ0Yy00MS4zMyAwLTc2LjY3IDE0LjY3LTEwNiA0NHMtNDQgNjUtNDQgMTA3YzAgNDEuMzMgMTQuNjcgNzYuODMgNDQgMTA2LjVTNjY2LjY3IDU4NCA3MDggNTg0em0tNTU3IDBjNDIgMCA3Ny42Ny0xNC44MyAxMDctNDQuNXM0NC02NS4xNyA0NC0xMDYuNWMwLTQyLTE0LjY3LTc3LjY3LTQ0LTEwN3MtNjUtNDQtMTA3LTQ0Yy00MS4zMyAwLTc2LjgzIDE0LjY3LTEwNi41IDQ0UzAgMzkxIDAgNDMzYzAgNDEuMzMgMTQuODMgNzYuODMgNDQuNSAxMDYuNVMxMDkuNjcgNTg0IDE1MSA1ODR6IiBmaWxsPSIjZmZmIi8%2BPC9zdmc%2B)](https://aldaviva.testspace.com/spaces/194266) [![Coverage status](https://img.shields.io/coveralls/github/Aldaviva/DataSizeUnits?logo=coveralls)](https://coveralls.io/github/Aldaviva/DataSizeUnits?branch=master)

Convert and format data size units (bits, bytes, kilobits, kilobytes, and others).

## Installation

This is a [.NET Standard 2.0](https://dotnet.microsoft.com/platform/dotnet-standard#versions) library, and can be depended upon in projects which target at least .NET 5, .NET Core 2.0, or .NET Framework 4.6.1.

```ps1
dotnet add package DataSizeUnits
```

## Features
- **Convert** between many units of digital information, including bits, bytes, and their higher-order units (kilobits and kilobytes and the rest, up to and including quettabits and quettabytes)
    - 150 Mbit → 17.8 MByte
        ```cs
        double sizeInMegabytes = new DataSize(150, DataSizeUnit.Megabit).AsUnit(DataSizeUnit.Megabyte);
        // sizeInMegabytes == 17.8
        ```

- **Normalize** a number of bytes to an automatically-selected unit based on its magnitude
    - 2,097,152 bytes → 2 MB
        ```cs
        (double quantity, DataSizeUnit unit) normalized = new DataSize(2_097_152).AsAutomaticUnit();
        // normalized.quantity == 2.0
        // normalized.unit == DataSizeUnit.Megabyte
        ```
    - 2,097,152 bytes → 16.78 mbit
        ```cs
        normalized = new DataSize(2_097_152).AsAutomaticUnit(true); // pass true to get bits units instead of bytes
        // normalized.quantity == 16.78
        // normalized.unit == DataSizeUnit.Megabit
        ```
    - The unit will be automatically selected so the value is greater than or equal to 1 of that unit, and less than 1 of the next largest unit. For example, 2,097,152 bytes is greater than or equal to 1 MB and less than 1 GB, so it is normalized to MB.

- **Parse** data sizes
    - 1.5 MB → 1,572,864 bytes
        ```cs
        DataSize parsed = DataSize.Parse("1.5 MB");
        // parsed.Bytes == 1572864
        ```

- **Parse and format** unit names and abbreviations
    - Megabyte, MByte, mebibyte, MiB, MB, and M are all megabytes
        ```cs
        DataSizeUnit? unit = DataSizeUnit.Parse("MB");
        // unit == DataSizeUnit.Megabyte
        ```
    - Abbreviations for each unit are of the short, case-sensitive forms.
        ```cs
        string abbreviation = DataSizeUnit.Terabyte.ToAbbreviation();
        // abbreviation == "TB"
        ```
        ```cs
        string iecAbbreviation = DataSizeUnit.Terabyte.ToAbbreviation(true); // pass true for the IEC abbreviations (kibibyte, etc.)
        // iecAbbreviation == "TiB"
        ```
    - Get the unit names in JEDEC (TB) or IEC (TiB) variants.
        ```cs
        string name = DataSizeUnit.Terabyte.ToName();
        // name == "terabyte"
        ```
        ```cs
        string iecName = DataSizeUnit.Terabyte.ToName(true); // pass true for the IEC names (kibibyte, etc.)
        // iecName == "tebibyte"
        ```

- **Format** bytes as a string with different unit and precision options
    - Automatic precision from culture, automatic byte-based unit
        ```cs
        string formatted = new DataSize(1572864).ToString();
        // formatted == "1.50 MB"
        ```
    - Automatic precision, manual unit
        ```cs
         formatted = new DataSize(1572864).ToString(DataSizeUnit.Kilobyte);
        // formatted == "1,536.00 kB"
        ```
    - Manual precision, automatic unit
        ```cs
        formatted = new DataSize(1572864).ToString(1);
        // formatted == "1.5 MB"
        ```
    - Automatic precision, automatic bit-based unit
        ```cs
        formatted = new DataSize(1572864).ToString(true);
        // formatted == "12.58 mb"
        ```

- **Serialize and deserialize** to and from bits
    - JSON ([System.Text.Json](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/overview) or [Newtonsoft.Json](https://www.newtonsoft.com/json/help))
        ```json
        { "filename": "example.txt", "size": 8192 }
        ```
    - XML ([XmlSerializer](https://learn.microsoft.com/en-us/dotnet/standard/serialization/xml-and-soap-serialization))
        ```xml
        <?xml version="1.0" encoding="utf-8"?>
        <MyFile xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
            <Filename>example.txt</Filename>
            <Size bits="8192" />
        </MyFile>
        ```