// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ClosedXML.Excel;
using CsvHelper;
using CsvHelper.Configuration;

namespace MyNet.CsvHelper.Extensions.Excel
{
    /// <summary>
	/// Used to write CSV files.
	/// </summary>
	public sealed class ExcelWriter : CsvWriter
    {
        private bool _disposed;
        private int _row = 1;
        private int _index = 1;
        private readonly IXLWorksheet _worksheet;
        private readonly Stream _stream;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelWriter"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="sheetName">The sheet name</param>
        /// <param name="culture"></param>
        public ExcelWriter(string path, string? sheetName = "Export", CultureInfo? culture = null) : this(File.Open(path, FileMode.OpenOrCreate, FileAccess.Write), sheetName, culture) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelWriter"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="sheetName">The sheet name</param>
        /// <param name="culture">The culture.</param>
        public ExcelWriter(Stream stream, string? sheetName = "Export", CultureInfo? culture = null) : this(stream, sheetName, new CsvConfiguration(culture ?? CultureInfo.CurrentCulture)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelWriter"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="sheetName">The sheet name</param>
        /// <param name="configuration">The configuration.</param>
        private ExcelWriter(Stream stream, string? sheetName, CsvConfiguration configuration) : base(TextWriter.Null, configuration)
        {
            configuration.Validate();
            _worksheet = new XLWorkbook().AddWorksheet(sheetName);
            _stream = stream;
        }


        /// <inheritdoc/>
        public override void WriteField(string field, bool shouldQuote)
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
            await FlushAsync();
            _index = 1;
            _row++;
        }

        /// <inheritdoc/>
        public override void Flush() => _stream.Flush();

        /// <inheritdoc/>
        public override Task FlushAsync() => _stream.FlushAsync();


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteToCell(string value)
        {
            var length = value?.Length ?? 0;

            if (value == null || length == 0)
            {
                return;
            }

            _worksheet.Worksheet.AsRange().Cell(_row, _index).Value = value;
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
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
    }
}
