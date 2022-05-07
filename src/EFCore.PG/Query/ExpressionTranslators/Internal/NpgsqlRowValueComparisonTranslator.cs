// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using Npgsql.EntityFrameworkCore.PostgreSQL.Internal;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Expressions.Internal;

namespace Npgsql.EntityFrameworkCore.PostgreSQL.Query.ExpressionTranslators.Internal;

public class NpgsqlRowValueComparisonTranslator : IMethodCallTranslator
{
    private readonly NpgsqlSqlExpressionFactory _sqlExpressionFactory;

    private static readonly MethodInfo GreaterThan =
        typeof(NpgsqlDbFunctionsExtensions).GetRuntimeMethod(
            nameof(NpgsqlDbFunctionsExtensions.GreaterThan),
            new[] { typeof(DbFunctions), typeof(ITuple), typeof(ITuple) })!;

    private static readonly MethodInfo LessThan =
        typeof(NpgsqlDbFunctionsExtensions).GetMethods()
            .Single(m => m.Name == nameof(NpgsqlDbFunctionsExtensions.LessThan));

    private static readonly MethodInfo GreaterThanOrEqual =
        typeof(NpgsqlDbFunctionsExtensions).GetMethods()
            .Single(m => m.Name == nameof(NpgsqlDbFunctionsExtensions.GreaterThanOrEqual));

    private static readonly MethodInfo LessThanOrEqual =
        typeof(NpgsqlDbFunctionsExtensions).GetMethods()
            .Single(m => m.Name == nameof(NpgsqlDbFunctionsExtensions.LessThanOrEqual));

    private static readonly Dictionary<MethodInfo, ExpressionType> Methods = new()
    {
        { GreaterThan, ExpressionType.GreaterThan },
        { LessThan, ExpressionType.LessThan },
        { GreaterThanOrEqual, ExpressionType.GreaterThanOrEqual },
        { LessThanOrEqual, ExpressionType.LessThanOrEqual }
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="NpgsqlRowValueComparisonTranslator"/> class.
    /// </summary>
    public NpgsqlRowValueComparisonTranslator(NpgsqlSqlExpressionFactory sqlExpressionFactory)
        => _sqlExpressionFactory = sqlExpressionFactory;

    /// <inheritdoc />
    public virtual SqlExpression? Translate(
        SqlExpression? instance,
        MethodInfo method,
        IReadOnlyList<SqlExpression> arguments,
        IDiagnosticsLogger<DbLoggerCategory.Query> logger)
    {
        if (method.DeclaringType != typeof(NpgsqlDbFunctionsExtensions)
            || !Methods.TryGetValue(method, out var expressionType)
            || arguments[1] is not PostgresRowValueExpression rowValue1
            || arguments[2] is not PostgresRowValueExpression rowValue2)
        {
            return null;
        }

        if (rowValue1.Values.Count != rowValue2.Values.Count)
        {
            throw new ArgumentException(NpgsqlStrings.RowValueMethodRequiresTwoArraysOfSameLength(method.Name));
        }

        return _sqlExpressionFactory.MakeBinary(expressionType, rowValue1, rowValue2, typeMapping: null);
    }
}