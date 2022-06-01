// <copyright file="Injector.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Base
{
  using System;
  using System.Collections.Generic;
  using CSBP.Resources;

  public class Injector
  {
    private readonly AbstractModule module;

    private readonly IDictionary<Type, Tuple<Type, object, object>> instances = new Dictionary<Type, Tuple<Type, object, object>>();

    private Injector(AbstractModule module)
    {
      this.module = module;
    }

    internal static Injector Create(AbstractModule module)
    {
      module.Configure();
      var injector = new Injector(module);

      // Alles Instanzen erzeugen
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

      // Referenzen setzen
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

    public T GetInstance<T>() where T : class
    {
      var type = typeof(T);
      var value = default(T);
      if (type != null)
      {
        instances.TryGetValue(type, out var obj);
        value = obj.Item3 as T;
      }
      if (value == null)
      {
        throw new Exception(string.Format(Messages.M1057, type?.Name));
      }
      return value;
    }
  }
}
