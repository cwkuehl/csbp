// <copyright file="TbEintrag.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models
{
  using System.Collections.Generic;
  using System.ComponentModel.DataAnnotations.Schema;
  using CSBP.Base;

  /// <summary>
  /// Entity-Klasse für Tabelle TB_Eintrag.
  /// </summary>
  public partial class TbEintrag : ModelBase
  {
    /// <summary>Holt oder setzt die Positionen.</summary>
    [NotMapped]
    public List<TbEintragOrt> Positions { get; set; }
  }
}
