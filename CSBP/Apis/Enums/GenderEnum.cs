// <copyright file="GenderEnum.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Enums;

using System.Collections.Generic;

/// <summary>Enum for gender.</summary>
public sealed class GenderEnum
{
  /// <summary>Value of gender as shortcut.</summary>
  private readonly string value;

#pragma warning disable SA1204, SA1202
  /// <summary>All values must be initialized before the single values.</summary>
  private static readonly Dictionary<string, GenderEnum> Instance = new();

  /// <summary>Gender neutrum.</summary>
  public static readonly GenderEnum NEUTRUM = new("N");

  /// <summary>Gender male.</summary>
  public static readonly GenderEnum MANN = new("M");

  /// <summary>Gender female.</summary>
  public static readonly GenderEnum FRAU = new("F");

  /// <summary>Gender male 2.</summary>
  public static readonly GenderEnum MAENNLICH = new("m");

  /// <summary>Gender female 2.</summary>
  public static readonly GenderEnum WEIBLICH = new("w");
#pragma warning restore SA1204, SA1202

  /// <summary>Initializes a new instance of the <see cref="GenderEnum"/> class.</summary>
  /// <param name="v">Affected shortcut.</param>
  private GenderEnum(string v)
  {
    value = v;
    Instance.Add(v, this);
  }

  /// <summary>Conversion from shortcut.</summary>
  /// <param name="v">Affected shortcut.</param>
  public static explicit operator GenderEnum(string v) => Instance.TryGetValue(v, out var result) ? result : NEUTRUM;

  /// <summary>Gets shortcut.</summary>
  /// <returns>Affected shortcut.</returns>
  public override string ToString()
  {
    return value;
  }
}
