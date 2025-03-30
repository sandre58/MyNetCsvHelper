// -----------------------------------------------------------------------
// <copyright file="ColumnMapping.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using CsvHelper.TypeConversion;
using MyNet.Utilities;

namespace MyNet.CsvHelper.Extensions;

public class ColumnMapping<T, TMember>(Expression<Func<T, TMember>> expression, string resourceKey, ITypeConverter? typeConverter = null)
{
    public string ResourceKey { get; } = resourceKey;

    public Expression<Func<T, TMember>> Expression { get; } = expression;

    public ITypeConverter? TypeConverter { get; private set; } = typeConverter;

    public override string ToString() => ResourceKey.Translate();
}
