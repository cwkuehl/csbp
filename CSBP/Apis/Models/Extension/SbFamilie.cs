// <copyright file="SbFamilie.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models
{
  using System.ComponentModel.DataAnnotations.Schema;
  using CSBP.Base;

  /// <summary>
  /// Entity class for table SB_Familie.
  /// </summary>
  public partial class SbFamilie : ModelBase
  {
    /// <summary>Holt oder setzt das Hochzeitsdatum.</summary>
    [NotMapped]
    public string Marriagedate { get; set; }

    /// <summary>Holt oder setzt den Hochzeitsort.</summary>
    [NotMapped]
    public string Marriageplace { get; set; }

    /// <summary>Holt oder setzt die Hochzeitsbemerkung.</summary>
    [NotMapped]
    public string Marriagememo { get; set; }

    /// <summary>Holt oder setzt den Vater.</summary>
    [NotMapped]
    public SbPerson Father { get; set; }

    /// <summary>Holt oder setzt den Mutter.</summary>
    [NotMapped]
    public SbPerson Mother { get; set; }
  }
}
