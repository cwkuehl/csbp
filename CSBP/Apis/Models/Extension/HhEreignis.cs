// <copyright file="HhEreignis.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models
{
  using System;
  using System.ComponentModel.DataAnnotations.Schema;
  using CSBP.Base;

  /// <summary>
  /// Entity-Klasse für Tabelle HH_Ereignis.
  /// </summary>
  public partial class HhEreignis : ModelBase
  {
    /// <summary>Holt oder setzt den Sollkontonamen.</summary>
    [NotMapped]
    public string DebitName { get; set; }

    /// <summary>Holt oder setzt das Ab-Datum des Sollkontos.</summary>
    [NotMapped]
    public DateTime? DebitFrom { get; set; }

    /// <summary>Holt oder setzt das Bis-Datum des Sollkontos.</summary>
    [NotMapped]
    public DateTime? DebitTo { get; set; }

    /// <summary>Holt oder setzt den Habenkontonamen.</summary>
    [NotMapped]
    public string CreditName { get; set; }

    /// <summary>Holt oder setzt das Ab-Datum des Habenkontos.</summary>
    [NotMapped]
    public DateTime? CreditFrom { get; set; }

    /// <summary>Holt oder setzt das Bis-Datum des Habenkontos.</summary>
    [NotMapped]
    public DateTime? CreditTo { get; set; }
  }
}
