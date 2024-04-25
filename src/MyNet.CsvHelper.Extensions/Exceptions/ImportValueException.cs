// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;

namespace MyNet.CsvHelper.Extensions.Exceptions
{
    public class ImportValueException : Exception
    {
        public int RowIndex { get; }

        public string? ColumnHeader { get; }

        public object? RowValue { get; }

        public ImportValueException(int rowIndex, string? columnHeader, object? rowValue, string? message, Exception? exception)
            : base(message, exception)
        {
            RowIndex = rowIndex;
            ColumnHeader = columnHeader;
            RowValue = rowValue;
        }
    }
}
