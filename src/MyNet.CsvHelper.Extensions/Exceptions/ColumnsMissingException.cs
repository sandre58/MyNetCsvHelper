// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace MyNet.CsvHelper.Extensions.Exceptions
{
    public class ColumnsMissingException : Exception
    {
        public IEnumerable<string> ColumnsMissing { get; }

        public ColumnsMissingException(IEnumerable<string> columnsMissing) => ColumnsMissing = columnsMissing;

        public ColumnsMissingException(IEnumerable<string> columnsMissing, string? message, Exception? exception)
            : base(message, exception) => ColumnsMissing = columnsMissing;
    }
}
