// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using CsvHelper.Configuration;
using MyNet.Utilities;

namespace MyNet.CsvHelper.Extensions
{
    public class DynamicClassMap<T> : ClassMap<T>
    {
        private int _index = -1;
        private readonly bool _mapIndex;

        public DynamicClassMap() { }

        public DynamicClassMap(IEnumerable<ColumnMapping<T, object?>> columns, bool mapIndex = true, bool displayTraduction = true)
        {
            _mapIndex = mapIndex;
            foreach (var item in columns)
            {
                var map = Map(item.Expression).Name(displayTraduction ? (item.ToString() ?? string.Empty) : item.ResourceKey);
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
                    Type mapType;
                    var property = member as PropertyInfo;
                    var field = member as FieldInfo;
                    mapType = property != null
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
}
