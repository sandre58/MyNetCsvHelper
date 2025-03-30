// -----------------------------------------------------------------------
// <copyright file="ColumnsMissingException.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace MyNet.CsvHelper.Extensions.Exceptions;

public class ColumnsMissingException : Exception
{
    public ColumnsMissingException(IEnumerable<string> columnsMissing) => ColumnsMissing = columnsMissing;

    public ColumnsMissingException(IEnumerable<string> columnsMissing, string? message, Exception? exception)
        : base(message, exception) => ColumnsMissing = columnsMissing;

    public ColumnsMissingException()
    {
    }

    public ColumnsMissingException(string message)
        : base(message)
    {
    }

    public ColumnsMissingException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public IEnumerable<string> ColumnsMissing { get; } = [];
}
