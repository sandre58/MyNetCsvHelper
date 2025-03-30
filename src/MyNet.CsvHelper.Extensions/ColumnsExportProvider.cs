// -----------------------------------------------------------------------
// <copyright file="ColumnsExportProvider.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MyNet.Utilities;

namespace MyNet.CsvHelper.Extensions;

public class ColumnsExportProvider<TColumn>(IEnumerable<ColumnMapping<TColumn, object?>> defaultColumns, string columnsOrder, string selectedColumns)
{
    public IDictionary<ColumnMapping<TColumn, object?>, bool> ProvideColumns()
    {
        var columns = defaultColumns.ToObservableCollection();

        if (!string.IsNullOrEmpty(columnsOrder))
            SortColumns(columns, columnsOrder.Split(";"));

        var columnNames = !string.IsNullOrEmpty(selectedColumns) ? selectedColumns.Split(";") : null;
        return columns.ToDictionary(x => x, x => columnNames?.Contains(x.ResourceKey) ?? true);
    }

    private static void SortColumns(ObservableCollection<ColumnMapping<TColumn, object?>> columns, IEnumerable<string> namesOrder)
    {
        var newIndex = 0;
        foreach (var item in namesOrder)
        {
            var currentIndex = columns.Select(x => x.ResourceKey).ToList().IndexOf(item);

            if (currentIndex <= -1)
                continue;
            columns.Move(currentIndex, newIndex);
            newIndex++;
        }
    }
}
