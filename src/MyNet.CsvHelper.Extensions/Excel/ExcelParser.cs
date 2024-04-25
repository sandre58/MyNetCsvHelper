// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ClosedXML.Excel;
using CsvHelper;
using CsvHelper.Configuration;

namespace MyNet.CsvHelper.Extensions.Excel
{
    public class ExcelParser : IParser
    {
        private bool _disposed;
        private readonly IXLWorksheet _worksheet;
        private readonly Stream _stream;
        private readonly int _lastRow;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelParser"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="sheetName">The sheet name</param>
        /// <param name="culture">The culture.</param>
        public ExcelParser(string path, string? sheetName = null, CultureInfo? culture = null) : this(File.Open(path, FileMode.OpenOrCreate, FileAccess.Read), sheetName, culture)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelParser"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="sheetName">The sheet name</param>
        /// <param name="culture">The culture.</param>
        /// <param name="leaveOpen"><c>true</c> to leave the <see cref="TextWriter"/> open after the <see cref="ExcelParser"/> object is disposed, otherwise <c>false</c>.</param>
        public ExcelParser(Stream stream, string? sheetName = null, CultureInfo? culture = null, bool leaveOpen = false) : this(stream, sheetName, new CsvConfiguration(culture ?? CultureInfo.InvariantCulture))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelParser"/> class.
        /// </summary>
        /// <param name="path">The stream.</param>
        /// <param name="sheetName">The sheet name</param>
        /// <param name="configuration">The configuration.</param>
        public ExcelParser(string path, CsvConfiguration configuration, string? sheetName = null) : this(File.Open(path, FileMode.OpenOrCreate, FileAccess.Read), sheetName, configuration)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelParser"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="sheetName">The sheet name</param>
        /// <param name="configuration">The configuration.</param>
        public ExcelParser(Stream stream, string? sheetName, CsvConfiguration configuration)
        {
            var workbook = new XLWorkbook(stream);

            _worksheet = string.IsNullOrEmpty(sheetName) ? workbook.Worksheet(1) : workbook.Worksheet(sheetName);

            Configuration = configuration ?? new CsvConfiguration(CultureInfo.InvariantCulture);
            _stream = stream;
            var lastRowUsed = _worksheet.LastRowUsed();
            if (lastRowUsed != null)
            {
                _lastRow = lastRowUsed.RowNumber();

                var cellsUsed = _worksheet.CellsUsed();
                Count = cellsUsed.Max(c => c.Address.ColumnNumber) -
                    cellsUsed.Min(c => c.Address.ColumnNumber) + 1;
            }

            Context = new CsvContext(this);
        }


        /// <inheritdoc/>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            _stream?.Dispose();

            // Free unmanaged resources (unmanaged objects) and override finalizer
            // Set large fields to null

            _disposed = true;
        }

        public bool Read()
        {
            if (Row > _lastRow)
            {
                return false;
            }

            Record = GetRecord();
            Row++;
            RawRow++;
            return true;
        }

        public Task<bool> ReadAsync()
        {
            if (Row > _lastRow)
            {
                return Task.FromResult(false);
            }

            Record = GetRecord();
            Row++;
            RawRow++;
            return Task.FromResult(true);
        }

        public long ByteCount => -1;
        public long CharCount => -1;
        public int Count { get; }

        public string this[int index] => Record?.ElementAtOrDefault(index) ?? string.Empty;

        public string[]? Record { get; private set; }

        public string RawRecord => string.Join(Configuration.Delimiter, Record ?? []);
        public int Row { get; private set; } = 1;
        public int RawRow { get; private set; } = 1;
        public CsvContext Context { get; }
        public IParserConfiguration Configuration { get; }

        public string Delimiter => ";";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string[]? GetRecord()
        {
            var currentRow = _worksheet.Row(Row);
            var cells = currentRow.Cells(1, Count);
            var values = cells.Select(x => x.Value.ToString() ?? string.Empty).ToArray();
            return values;
        }
    }
}
