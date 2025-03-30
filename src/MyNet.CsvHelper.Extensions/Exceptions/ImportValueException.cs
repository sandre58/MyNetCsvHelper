// -----------------------------------------------------------------------
// <copyright file="ImportValueException.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.CsvHelper.Extensions.Exceptions;

public class ImportValueException : Exception
{
    public ImportValueException(int rowIndex, string? columnHeader, object? rowValue, string? message, Exception? exception)
        : base(message, exception)
    {
        RowIndex = rowIndex;
        ColumnHeader = columnHeader;
        RowValue = rowValue;
    }

    public ImportValueException()
    {
    }

    public ImportValueException(string message)
        : base(message)
    {
    }

    public ImportValueException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public int RowIndex { get; }

    public string? ColumnHeader { get; }

    public object? RowValue { get; }
}
