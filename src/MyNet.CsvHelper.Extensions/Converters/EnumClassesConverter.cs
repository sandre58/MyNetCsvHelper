// -----------------------------------------------------------------------
// <copyright file="EnumClassesConverter.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using MyNet.Humanizer;
using MyNet.Utilities;

namespace MyNet.CsvHelper.Extensions.Converters;

public class EnumClassesConverter<T> : DefaultTypeConverter
    where T : EnumClass<T>
{
    public override object ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData) => text?.Split(',').Select(x => x.DehumanizeTo<T>()) ?? [];

    public override string ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData) => value is IEnumerable<T> e ? string.Join(",", e) : string.Empty;
}
