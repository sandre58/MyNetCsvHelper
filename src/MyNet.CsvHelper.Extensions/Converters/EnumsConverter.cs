// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using MyNet.Humanizer;

namespace MyNet.CsvHelper.Extensions.Converters
{
    public class EnumsConverter<T> : DefaultTypeConverter
        where T : struct, Enum
    {
        public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData) => text?.Split(',').Select(x => x.DehumanizeTo<T>(OnNoMatch.ReturnsDefault)) ?? [];

        public override string? ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData) => value is IEnumerable<T> e ? string.Join(",", e) : string.Empty;
    }
}
