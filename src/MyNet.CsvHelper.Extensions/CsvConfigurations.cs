// -----------------------------------------------------------------------
// <copyright file="CsvConfigurations.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CsvHelper.Configuration;
using MyNet.CsvHelper.Extensions.Exceptions;
using MyNet.Utilities.Logging;

namespace MyNet.CsvHelper.Extensions;

public static class CsvConfigurations
{
    public static CsvConfiguration Default
        => new(CultureInfo.CurrentCulture)
        {
            PrepareHeaderForMatch = args => args.Header,

            HeaderValidated = args =>
            {
                foreach (var invalidHeader in args.InvalidHeaders)
                    LogManager.Warning($"Header with name '{string.Join("' or '", invalidHeader.Names)}'[{invalidHeader.Index}] was not found.");
            },

            MissingFieldFound = args =>
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

        configuration.HeaderValidated = args =>
        {
            if (args.InvalidHeaders.Length != 0)
                exceptions.Add(new ColumnsMissingException([.. args.InvalidHeaders.SelectMany(x => x.Names)]));
        };

        configuration.ReadingExceptionOccurred = args =>
        {
            var columnIndex = args.Exception.Context?.Reader?.CurrentIndex;
            var rowIndex = args.Exception.Context?.Parser?.RawRow - 1;
            var columnHeader = args.Exception.Context?.Reader?.HeaderRecord![columnIndex ?? 0];
            var row = args.Exception.Context?.Parser?.RawRecord;
            var rowValue = args.Exception.Context?.Parser?[columnIndex ?? 0];
            var exception = args.Exception.InnerException ?? args.Exception;
            exceptions.Add(new ImportRowException(rowIndex ?? 0, columnIndex ?? 0, row, columnHeader, rowValue, exception.Message, exception));
            return false;
        };

        return configuration;
    }
}
