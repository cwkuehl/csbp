// <copyright file="AbstractModule.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace CSBP.Services.Base
{
    public abstract class AbstractModule
    {
        public virtual void Configure()
        {
            // Override
        }

        public IDictionary<Type, Type> Bindings { get; } = new Dictionary<Type, Type>();

        protected void Bind<T, U>() where T : class where U : T {
            Bindings[typeof(T)] = typeof(U);
        }
    }
}
