// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq.Expressions;
using CsvHelper.TypeConversion;
using MyNet.Utilities.Extensions;

namespace MyNet.CsvHelper.Extensions
{
    public class ColumnMapping<T, TMember>
    {
        public string ResourceKey { get; }

        public Expression<Func<T, TMember>> Expression { get; }

        public ITypeConverter? TypeConverter { get; private set; }

        public ColumnMapping(Expression<Func<T, TMember>> expression, string resourceKey, ITypeConverter? typeConverter = null)
        {
            Expression = expression;
            ResourceKey = resourceKey;
            TypeConverter = typeConverter;
        }

        public override string? ToString() => ResourceKey.Translate();
    }
}
