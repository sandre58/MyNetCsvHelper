// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;

namespace MyNet.CsvHelper.Extensions.Exceptions
{
    public class ImportRowException : Exception
    {
        public int RowIndex { get; }

        public int ColumnIndex { get; }

        public string? Row { get; }

        public string? ColumnHeader { get; }

        public object? RowValue { get; }

        public ImportRowException(int rowIndex, int columnIndex, string? row, string? columnHeader, object? rowValue, string? message, Exception? exception)
            : base(message, exception)
        {
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
            ColumnHeader = columnHeader;
            Row = row;
            RowValue = rowValue;
        }
    }
}
