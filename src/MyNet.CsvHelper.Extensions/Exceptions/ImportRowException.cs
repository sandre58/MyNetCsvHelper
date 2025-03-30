// -----------------------------------------------------------------------
// <copyright file="ImportRowException.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.CsvHelper.Extensions.Exceptions;

public class ImportRowException : Exception
{
    public ImportRowException(int rowIndex, int columnIndex, string? row, string? columnHeader, object? rowValue, string? message, Exception? exception)
        : base(message, exception)
    {
        RowIndex = rowIndex;
        ColumnIndex = columnIndex;
        ColumnHeader = columnHeader;
        Row = row;
        RowValue = rowValue;
    }

    public ImportRowException()
    {
    }

    public ImportRowException(string message)
        : base(message)
    {
    }

    public ImportRowException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public int RowIndex { get; }

    public int ColumnIndex { get; }

    public string? Row { get; }

    public string? ColumnHeader { get; }

    public object? RowValue { get; }
}
