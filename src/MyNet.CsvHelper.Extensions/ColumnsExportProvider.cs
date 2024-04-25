// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MyNet.Utilities;

namespace MyNet.CsvHelper.Extensions
{
    public class ColumnsExportProvider<TColumn>
    {
        private readonly IEnumerable<ColumnMapping<TColumn, object?>> _defaultColumns;
        private readonly string _columnsOrder;
        private readonly string _selectedColumns;

        public ColumnsExportProvider(IEnumerable<ColumnMapping<TColumn, object?>> defaultColumns, string columnsOrder, string selectedColumns)
        {
            _defaultColumns = defaultColumns;
            _columnsOrder = columnsOrder;
            _selectedColumns = selectedColumns;
        }

        public IDictionary<ColumnMapping<TColumn, object?>, bool> ProvideColumns()
        {
            var columns = _defaultColumns.ToObservableCollection();

            if (!string.IsNullOrEmpty(_columnsOrder))
                SortColumns(columns, _columnsOrder.Split(";"));

            var columnNames = !string.IsNullOrEmpty(_selectedColumns) ? _selectedColumns.Split(";") : null;
            return columns.ToDictionary(x => x, x => columnNames?.Contains(x.ResourceKey) ?? true);
        }

        private static void SortColumns(ObservableCollection<ColumnMapping<TColumn, object?>> columns, IEnumerable<string> namesOrder)
        {
            var newIndex = 0;
            foreach (var item in namesOrder)
            {
                var currentIndex = columns.Select(x => x.ResourceKey).ToList().IndexOf(item);

                if (currentIndex > -1)
                {
                    columns.Move(currentIndex, newIndex);
                    newIndex++;
                }
            }
        }
    }
}
