// <copyright file="MaParameter.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models
{
  using System.ComponentModel.DataAnnotations.Schema;
  using CSBP.Base;

  /// <summary>
  /// Entity class for table MA_Parameter.
  /// </summary>
  public partial class MaParameter : ModelBase
  {
    /// <summary>Holt oder setzt den Kommentar.</summary>
    [NotMapped]
    public string Comment { get; set; }

    /// <summary>Holt oder setzt den Standardwert.</summary>
    [NotMapped]
    public string Default { get; set; }

    /// <summary>Holt oder setzt einen Wert, der angibt, ob der Parameter nicht in der Datenbank gespeichert wird.</summary>
    [NotMapped]
    public bool NotDatabase { get; set; }
  }
}
