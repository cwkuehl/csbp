// <copyright file="HhBilanz.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models
{
  using System;
  using System.ComponentModel.DataAnnotations.Schema;
  using CSBP.Base;

  /// <summary>
  /// Entity-Klasse für Tabelle HH_Bilanz.
  /// </summary>
  public partial class HhBilanz : ModelBase
  {
    /// <summary>Holt oder setzt den Kontonamen.</summary>
    [NotMapped]
    public string AccountName { get; set; }

    /// <summary>Holt oder setzt die Sortierung des Kontos.</summary>
    [NotMapped]
    public string AccountSort { get; set; }

    /// <summary>Holt oder setzt die Art des Kontos.</summary>
    [NotMapped]
    public int AccountType { get; set; }

    /// <summary>Holt oder setzt die Summe des Kontos.</summary>
    [NotMapped]
    public decimal AccountSum { get; set; }

    /// <summary>Holt oder setzt die EUR-Summe des Kontos.</summary>
    [NotMapped]
    public decimal AccountEsum { get; set; }
  }
}
