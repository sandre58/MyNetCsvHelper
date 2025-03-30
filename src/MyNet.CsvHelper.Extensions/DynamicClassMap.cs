// -----------------------------------------------------------------------
// <copyright file="DynamicClassMap.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using CsvHelper.Configuration;
using MyNet.Utilities;

namespace MyNet.CsvHelper.Extensions;

public class DynamicClassMap<T> : ClassMap<T>
{
    private readonly bool _mapIndex;
    private int _index = -1;

    public DynamicClassMap() { }

    public DynamicClassMap(IEnumerable<ColumnMapping<T, object?>> columns, bool mapIndex = true, bool displayTraduction = true)
    {
        _mapIndex = mapIndex;
        foreach (var item in columns)
        {
            var map = Map(item.Expression).Name(displayTraduction ? item.ToString() : item.ResourceKey);
            if (item.TypeConverter is not null)
                map.IfNotNull(x => x.TypeConverter(item.TypeConverter));
        }
    }

    public MemberMap Map(Expression<Func<T, object?>> expression, bool useExistingMap = true)
    {
        _index++;

        var stack = expression.GetMembers();
        if (stack.Count == 0)
        {
            throw new InvalidOperationException("No members were found in expression '{expression}'.");
        }

        ClassMap currentClassMap = this;
        MemberInfo member;

        if (stack.Count > 1)
        {
            // We need to add a reference map for every sub member.
            while (stack.Count > 1)
            {
                member = stack.Pop();
                var property = member as PropertyInfo;
                var field = member as FieldInfo;
                var mapType = property != null
                    ? typeof(DefaultClassMap<>).MakeGenericType(property.PropertyType)
                    : field != null
                        ? typeof(DefaultClassMap<>).MakeGenericType(field.FieldType)
                        : throw new InvalidOperationException("The given expression was not a property or a field.");

                var referenceMap = currentClassMap.References(mapType, member);
                currentClassMap = referenceMap.Data.Mapping;
            }
        }

        // Add the member map to the last reference map.
        member = stack.Pop();

        var result = currentClassMap.Map(typeof(T), member, useExistingMap);

        if (_mapIndex) result = result.Index(_index);
        return result;
    }
}
