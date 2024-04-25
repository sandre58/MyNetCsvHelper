// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using MyNet.Humanizer;
using MyNet.Utilities;

namespace MyNet.CsvHelper.Extensions.Converters
{
    public class EnumerationConverter<T> : DefaultTypeConverter
        where T : Enumeration<T>
    {
        public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData) => text?.DehumanizeTo<T>()!;

        public override string? ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData) => value is IEnumeration e ? e.Humanize() ?? string.Empty : string.Empty;
    }
}
