// <copyright file="GenderEnum.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace CSBP.Apis.Enums
{
  /// <summary>Aufzählung für Geschlechter.</summary>
  public sealed class GenderEnum
  {
    /// <summary>Wert des Geschlechts.</summary>
    private readonly string value;

    /// <summary>Alle Geschlechter muss vor den einzelnen Werten initialisiert werden.</summary>
    private static readonly Dictionary<string, GenderEnum> instance = new();

    /// <summary>Geschlecht Neutrum.</summary>
    public static readonly GenderEnum NEUTRUM = new("N");

    /// <summary>Geschlecht Mann.</summary>
    public static readonly GenderEnum MANN = new("M");

    /// <summary>Geschlecht Frau.</summary>
    public static readonly GenderEnum FRAU = new("F");

    /// <summary>Geschlecht männlich.</summary>
    public static readonly GenderEnum MAENNLICH = new("m");

    /// <summary>Geschlecht weiblich.</summary>
    public static readonly GenderEnum WEIBLICH = new("w");

    /// <summary>Kontruktor mit Kürzel.</summary>
    /// <param name="v">Betroffenes Kürzel.</param>
    private GenderEnum(string v)
    {
      value = v;
      instance.Add(v, this);
    }

    /// <summary>Liefert Geschlecht als Kürzel.</summary>
    /// <returns>Geschlecht als Kürzel.</returns>
    public override string ToString()
    {
      return value;
    }

    /// <summary>Konverter aus Kürzel.</summary>
    /// <param name="v">Betroffenes Kürzel.</param>
    public static explicit operator GenderEnum(string v)
    {
      if (instance.TryGetValue(v, out var result))
        return result;
      return NEUTRUM;
    }
  }
}
