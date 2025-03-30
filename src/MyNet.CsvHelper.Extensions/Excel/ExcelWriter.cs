// -----------------------------------------------------------------------
// <copyright file="ExcelWriter.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ClosedXML.Excel;
using CsvHelper;
using CsvHelper.Configuration;

namespace MyNet.CsvHelper.Extensions.Excel;

/// <summary>
/// Used to write CSV files.
/// </summary>
public sealed class ExcelWriter : CsvWriter
{
    private readonly IXLWorksheet _worksheet;
    private readonly Stream _stream;
    private bool _disposed;
    private int _row = 1;
    private int _index = 1;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExcelWriter"/> class.
    /// </summary>
    public ExcelWriter(string path, string? sheetName = "Export", CultureInfo? culture = null)
        : this(File.Open(path, FileMode.OpenOrCreate, FileAccess.Write), sheetName, culture) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExcelWriter"/> class.
    /// </summary>
    public ExcelWriter(Stream stream, string? sheetName = "Export", CultureInfo? culture = null)
        : this(stream, sheetName, new CsvConfiguration(culture ?? CultureInfo.CurrentCulture)) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExcelWriter"/> class.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposed in class Dispose()")]
    private ExcelWriter(Stream stream, string? sheetName, CsvConfiguration configuration)
        : base(TextWriter.Null, configuration)
    {
        configuration.Validate();
        _worksheet = new XLWorkbook().AddWorksheet(sheetName);
        _stream = stream;
    }

    /// <inheritdoc/>
    public override void WriteField(string? field, bool shouldQuote)
    {
        field = SanitizeForInjection(field);

        WriteToCell(field);
        _index++;
    }

    /// <inheritdoc/>
    public override void NextRecord()
    {
        Flush();
        _index = 1;
        _row++;
    }

    /// <inheritdoc/>
    public override async Task NextRecordAsync()
    {
        await FlushAsync().ConfigureAwait(false);
        _index = 1;
        _row++;
    }

    /// <inheritdoc/>
    public override void Flush() => _stream.Flush();

    /// <inheritdoc/>
    public override Task FlushAsync() => _stream.FlushAsync();

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (_disposed)
        {
            return;
        }

        Flush();
        _worksheet.Workbook.SaveAs(_stream);
        _stream.Flush();

        if (disposing)
        {
            // Dispose managed state (managed objects)
            _worksheet.Workbook.Dispose();
            _stream.Dispose();
        }

        // Free unmanaged resources (unmanaged objects) and override finalizer
        // Set large fields to null
        _disposed = true;
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        await FlushAsync().ConfigureAwait(false);
        _worksheet.Workbook.SaveAs(_stream);
        await _stream.FlushAsync().ConfigureAwait(false);

        if (disposing)
        {
            // Dispose managed state (managed objects)
            _worksheet.Workbook.Dispose();
            await _stream.DisposeAsync().ConfigureAwait(false);
        }

        // Free unmanaged resources (unmanaged objects) and override finalizer
        // Set large fields to null
        _disposed = true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteToCell(string? value)
    {
        var length = value?.Length ?? 0;

        if (value == null || length == 0)
        {
            return;
        }

        _worksheet.Worksheet.AsRange().Cell(_row, _index).Value = value;
    }
}
