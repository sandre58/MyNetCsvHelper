// -----------------------------------------------------------------------
// <copyright file="EnumClassConverter.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using MyNet.Humanizer;
using MyNet.Utilities;

namespace MyNet.CsvHelper.Extensions.Converters;

public class EnumClassConverter<T> : DefaultTypeConverter
    where T : EnumClass<T>
{
    public override object ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData) => text?.DehumanizeTo<T>()!;

    public override string ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData) => value is IEnumeration e ? e.Humanize() ?? string.Empty : string.Empty;
}
