﻿// <copyright file="GedcomEventEnum.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace CSBP.Apis.Enums
{
  /// <summary>Aufzählung für GEDCOM-Ereignisse.</summary>
  public sealed class GedcomEventEnum
  {
    /// <summary>Wert des Ereignisses.</summary>
    private string value;

    /// <summary>Symbol des Ereignisses.</summary>
    private string symbol;

    /// <summary>Alle Werte muss vor den einzelnen Werten initialisiert werden.</summary>
    private static readonly Dictionary<string, GedcomEventEnum> instance = new Dictionary<string, GedcomEventEnum>();

    /// <summary>Ereignis Unbekannt.</summary>
    public static readonly GedcomEventEnum UNKNOWN = new GedcomEventEnum("", "");

    /// <summary>Ereignis Geburt.</summary>
    public static readonly GedcomEventEnum BIRTH = new GedcomEventEnum("BIRT", "*");

    /// <summary>Ereignis Taufe.</summary>
    public static readonly GedcomEventEnum CHRIST = new GedcomEventEnum("CHR", "~");

    /// <summary>Ereignis Tod.</summary>
    public static readonly GedcomEventEnum DEATH = new GedcomEventEnum("DEAT", "+");

    /// <summary>Ereignis Begräbnis.</summary>
    public static readonly GedcomEventEnum BURIAL = new GedcomEventEnum("BURI", "b");

    /// <summary>Ereignis Heirat.</summary>
    public static readonly GedcomEventEnum MARRIAGE = new GedcomEventEnum("MARR", "h");

    /// <summary>Kontruktor mit Werten.</summary>
    /// <param name="v">Betroffenes GEDCOM-Kürzel.</param>
    /// <param name="s">Betroffenes Symbol.</param>
    private GedcomEventEnum(string v, string s)
    {
      value = v;
      symbol = s;
      instance.Add(v, this);
    }

    /// <summary>Liefert Ereignis als GEDCOM-Kürzel.</summary>
    /// <returns>Ereignis als GEDCOM-Kürzel.</returns>
    public override string ToString()
    {
      return value;
    }

    /// <summary>Liefert Ereignis als Symbol.</summary>
    /// <returns>Ereignis als Symbol.</returns>
    public string ToSymbol()
    {
      return symbol;
    }

    /// <summary>Konverter aus Kürzel.</summary>
    /// <param name="v">Betroffenes Kürzel.</param>
    public static explicit operator GedcomEventEnum(string v)
    {
      if (instance.TryGetValue(v, out var result))
        return result;
      return UNKNOWN;
    }
  }
}
