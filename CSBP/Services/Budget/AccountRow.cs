// <copyright file="AccountRow.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Budget
{
  using System;
  using CSBP.Base;

  /// <summary>
  /// Entity-Klasse für eine Kontozeile.
  /// </summary>
  [Serializable]
  public partial class AccountRow : ModelBase
  {
    /// <summary>Holt oder setzt die Nummer der linken Spalte.</summary>
    public string Nr { get; set; }

    /// <summary>Holt oder setzt den Namen der linken Spalte.</summary>
    public string Name { get; set; }

    /// <summary>Holt oder setzt den Betrag der linken Spalte.</summary>
    public decimal? Value { get; set; }

    /// <summary>Holt oder setzt die Nummer der rechten Spalte.</summary>
    public string Nr2 { get; set; }

    /// <summary>Holt oder setzt den Namen der rechten Spalte.</summary>
    public string Name2 { get; set; }

    /// <summary>Holt oder setzt den Betrag der rechten Spalte.</summary>
    public decimal? Value2 { get; set; }
  }
}
