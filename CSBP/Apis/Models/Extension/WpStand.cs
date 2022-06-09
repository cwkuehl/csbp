// <copyright file="WpStand.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models
{
  using System.ComponentModel.DataAnnotations.Schema;
  using CSBP.Base;

  /// <summary>
  /// Entity class for table WP_Stand.
  /// </summary>
  public partial class WpStand : ModelBase
  {
    /// <summary>Holt oder setzt die Wertpapier-Bezeichnung.</summary>
    [NotMapped]
    public string StockDescription { get; set; }
  }
}
