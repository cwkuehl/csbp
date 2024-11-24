// <copyright file="GedcomEventEnum.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Apis.Enums;

using System.Collections.Generic;

/// <summary>Event type of GEDCOM.</summary>
public sealed class GedcomEventEnum
{
  /// <summary>String value of event.</summary>
  private readonly string value;

  /// <summary>Symbol of event.</summary>
  private readonly string symbol;

#pragma warning disable SA1204, SA1202

  /// <summary>All values must be initialized before the single values.</summary>
  private static readonly Dictionary<string, GedcomEventEnum> Instance = new();

  /// <summary>Ereignis Unbekannt.</summary>
  public static readonly GedcomEventEnum UNKNOWN = new("", "");

  /// <summary>Ereignis Geburt.</summary>
  public static readonly GedcomEventEnum BIRTH = new("BIRT", "*");

  /// <summary>Ereignis Taufe.</summary>
  public static readonly GedcomEventEnum CHRIST = new("CHR", "~");

  /// <summary>Ereignis Tod.</summary>
  public static readonly GedcomEventEnum DEATH = new("DEAT", "+");

  /// <summary>Ereignis Begräbnis.</summary>
  public static readonly GedcomEventEnum BURIAL = new("BURI", "b");

  /// <summary>Ereignis Heirat.</summary>
  public static readonly GedcomEventEnum MARRIAGE = new("MARR", "h");

#pragma warning restore SA1204, SA1202

  /// <summary>Initializes a new instance of the <see cref="GedcomEventEnum"/> class.</summary>
  /// <param name="v">Affected GEDCOM shortcut.</param>
  /// <param name="s">Affected symbol.</param>
  private GedcomEventEnum(string v, string s)
  {
    value = v;
    symbol = s;
    Instance.Add(v, this);
  }

  /// <summary>Conversion from shortcut.</summary>
  /// <param name="v">Affected shortcut.</param>
  public static explicit operator GedcomEventEnum(string v) => Instance.TryGetValue(v, out var result) ? result : UNKNOWN;

  /// <summary>Gets GEDCOM shortcut.</summary>
  /// <returns>Affected GEDCOM shortcut.</returns>
  public override string ToString()
  {
    return value;
  }

  /// <summary>Gets symbol.</summary>
  /// <returns>Affected symbol.</returns>
  public string ToSymbol()
  {
    return symbol;
  }
}
