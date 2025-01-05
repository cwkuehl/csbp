// <copyright file="LinqExtensions.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Base;

using System;
using System.Linq;
using System.Linq.Expressions;

/// <summary>
/// Statische Klasse für LinQ-Extensions.
/// </summary>
public static class LinqExtensions
{
  /// <summary>
  /// Ordnet eine Query nach einem Spaltennamen.
  /// </summary>
  /// <typeparam name="TSource">Betroffener Query-Typ.</typeparam>
  /// <param name="source">Betroffene Query.</param>
  /// <param name="field">Betroffener Spaltennamen.</param>
  /// <param name="desc">Soll absteigend sortiert werden.</param>
  /// <returns>Geordnete Query.</returns>
  public static IOrderedQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> source, string field, bool desc = false)
  {
    var param = Expression.Parameter(typeof(TSource), "r");
    var exp = Expression.Property(param, field);
    var lambda = Expression.Lambda(exp, param); // r => r.AlgumaCoisa
    var type = typeof(TSource).GetProperty(field)?.PropertyType;
    if (type == null)
      throw new ArgumentException($"Property {field} in Type {typeof(TSource).FullName} nicht gefunden.");
    var name = desc ? "OrderByDescending" : "OrderBy";
    var method = typeof(Queryable).GetMethods().First(m => m.Name == name && m.GetParameters().Length == 2);
    var methodog = method.MakeGenericMethod([typeof(TSource), type]);
    return methodog.Invoke(source, [source, lambda]) as IOrderedQueryable<TSource>;
  }

  /// <summary>
  /// Ordnet eine Query nach einem weiteren Spaltennamen.
  /// </summary>
  /// <typeparam name="TSource">Betroffener Query-Typ.</typeparam>
  /// <param name="source">Betroffene Query.</param>
  /// <param name="field">Betroffener Spaltennamen.</param>
  /// <param name="desc">Soll absteigend sortiert werden.</param>
  /// <returns>Geordnete Query.</returns>
  public static IOrderedQueryable<TSource> ThenBy<TSource>(this IOrderedQueryable<TSource> source, string field, bool desc = false)
  {
    var param = Expression.Parameter(typeof(TSource), "r");
    var exp = Expression.Property(param, field);
    var lambda = Expression.Lambda<Func<TSource, string>>(exp, param); // r => r.AlgumaCoisa
    var type = typeof(TSource).GetProperty(field).PropertyType;
    var name = desc ? "ThenByDescending" : "ThenBy";
    var method = typeof(Queryable).GetMethods().First(m => m.Name == name && m.GetParameters().Length == 2);
    var methodog = method.MakeGenericMethod(new[] { typeof(TSource), type });
    return methodog.Invoke(source, [source, lambda]) as IOrderedQueryable<TSource>;
  }
}
