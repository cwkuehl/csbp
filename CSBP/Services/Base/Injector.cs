// <copyright file="Injector.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Base;

using System;
using System.Collections.Generic;
using CSBP.Services.Resources;

/// <summary>
/// Creats instances and injects dependant instances.
/// </summary>
public class Injector
{
  /// <summary>Defining module.</summary>
  private readonly AbstractModule module;

  /// <summary>Dictionary for instances.</summary>
  private readonly IDictionary<Type, Tuple<Type, object, object>> instances = new Dictionary<Type, Tuple<Type, object, object>>();

  /// <summary>
  /// Initializes a new instance of the <see cref="Injector"/> class.
  /// </summary>
  /// <param name="module">Affected module.</param>
  private Injector(AbstractModule module)
  {
    this.module = module;
  }

  /// <summary>
  /// Gets instance of interface type.
  /// </summary>
  /// <typeparam name="TIf">Affected interface type.</typeparam>
  /// <returns>Instance of interface type.</returns>
  public TIf GetInstance<TIf>()
    where TIf : class
  {
    var type = typeof(TIf);
    var value = default(TIf);
    if (type != null)
    {
      instances.TryGetValue(type, out var obj);
      value = obj?.Item3 as TIf;
    }
    if (value == null)
    {
      throw new Exception(string.Format(Messages.M1057, type?.Name));
    }
    return value;
  }

  /// <summary>
  /// Creates an injector.
  /// </summary>
  /// <param name="module">Affected module.</param>
  /// <returns>Created injector.</returns>
  internal static Injector Create(AbstractModule module)
  {
    module.Configure();
    var injector = new Injector(module);

    // Create all instances.
    foreach (var b in injector.module.Bindings)
    {
      var instance = Activator.CreateInstance(b.Value);
      var i0 = instance;
      if (b.Value.Name.EndsWith("Service", StringComparison.Ordinal))
      {
        var tptype = typeof(TransactionProxy<>).MakeGenericType(new[] { b.Key });
        var m = tptype.GetMethod("Create");
        instance = m.Invoke(null, new[] { i0 });
      }
      injector.instances.Add(b.Key, new Tuple<Type, object, object>(b.Value, i0, instance));
    }

    // Sets references
    foreach (var i in injector.instances)
    {
      foreach (var p in i.Value.Item1.GetProperties())
      {
        if (injector.instances.ContainsKey(p.PropertyType))
        {
          _ = injector.instances[p.PropertyType].Item2;
          var setter = p.GetSetMethod();
          setter.Invoke(i.Value.Item2, new[] { injector.instances[p.PropertyType].Item3 });
        }
      }
    }
    return injector;
  }
}
