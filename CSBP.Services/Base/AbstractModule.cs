// <copyright file="AbstractModule.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Base;

using System;
using System.Collections.Generic;

/// <summary>
/// Abstract module for injector.
/// </summary>
public abstract class AbstractModule
{
  /// <summary>
  /// Gets the dictionary of interface type and implementation type.
  /// </summary>
  public IDictionary<Type, Type> Bindings { get; } = new Dictionary<Type, Type>();

  /// <summary>
  /// Defines the bindings.
  /// </summary>
  public virtual void Configure()
  {
    // Override
  }

  /// <summary>
  /// Defines implementation type for interface type.
  /// </summary>
  /// <typeparam name="TIf">Interface type.</typeparam>
  /// <typeparam name="TImp">Implementation type.</typeparam>
  protected void Bind<TIf, TImp>()
    where TIf : class
    where TImp : TIf
  {
    Bindings[typeof(TIf)] = typeof(TImp);
  }
}
