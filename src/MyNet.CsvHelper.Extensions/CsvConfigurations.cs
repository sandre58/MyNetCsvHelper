// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CsvHelper.Configuration;
using MyNet.CsvHelper.Extensions.Exceptions;
using MyNet.Utilities.Logging;

namespace MyNet.CsvHelper.Extensions
{
    public static class CsvConfigurations
    {
        public static CsvConfiguration Default
            => new(CultureInfo.CurrentCulture)
            {
                PrepareHeaderForMatch = (args) => args.Header,

                HeaderValidated = (args) =>
                {
                    foreach (var invalidHeader in args.InvalidHeaders)
                        LogManager.Warning($"Header with name '{string.Join("' or '", invalidHeader.Names)}'[{invalidHeader.Index}] was not found.");
                },

                MissingFieldFound = (args) =>
                {
                    // Get by index.
                    if (args.HeaderNames == null || args.HeaderNames.Length == 0)
                    {
                        LogManager.Warning($"Field at index '{args.Index}' does not exist.");
                        return;
                    }

                    // Get by name.
                    var indexText = args.Index > 0 ? $" at field index '{args.Index}'" : string.Empty;

                    if (args.HeaderNames?.Length == 1)
                    {
                        LogManager.Warning($"Field with name '{args.HeaderNames[0]}'{indexText} does not exist.");
                        return;
                    }

                    LogManager.Warning($"Field containing names '{string.Join("' or '", args.HeaderNames ?? [])}'{indexText} does not exist.");
                }
            };

        public static CsvConfiguration GetConfigurationWithNoThrowException(ICollection<Exception> exceptions)
        {
            var configuration = Default;

            configuration.HeaderValidated = (args) =>
            {
                if (args.InvalidHeaders is not null && args.InvalidHeaders.Length != 0)
                    exceptions.Add(new ColumnsMissingException(args.InvalidHeaders.SelectMany(x => x.Names).ToArray()));
            };

            configuration.ReadingExceptionOccurred = args =>
            {
                var columnIndex = args.Exception.Context.Reader.CurrentIndex;
                var rowIndex = args.Exception.Context.Parser.RawRow - 1;
                var columnHeader = args.Exception.Context.Reader.HeaderRecord![columnIndex];
                var row = args.Exception.Context.Parser.RawRecord;
                var rowValue = args.Exception.Context.Parser[columnIndex];
                var exception = args.Exception.InnerException ?? args.Exception;
                exceptions.Add(new ImportRowException(rowIndex, columnIndex, row, columnHeader, rowValue, exception?.Message, exception));
                return false;
            };

            return configuration;
        }
    }
}
