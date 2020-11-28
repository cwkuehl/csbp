// <copyright file="HhPeriode.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models
{
  using System.ComponentModel.DataAnnotations.Schema;
  using CSBP.Base;

  /// <summary>
  /// Entity-Klasse für Tabelle HH_Periode.
  /// </summary>
  public partial class HhPeriode : ModelBase
  {
    /// <summary>Holt den Zeitraum.</summary>
    [NotMapped]
    public string Period
    {
      get { return Functions.GetPeriod(Datum_Von, Datum_Bis); }
    }
  }
}
