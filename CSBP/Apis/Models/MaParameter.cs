// <copyright file="MaParameter.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models;

using System;
using System.ComponentModel.DataAnnotations.Schema;
using CSBP.Base;

/// <summary>
/// Entity class for table MA_Parameter.
/// </summary>
[Serializable]
[Table("MA_Parameter")]
public partial class MaParameter : ModelBase
{
  /// <summary>Initializes a new instance of the <see cref="MaParameter"/> class.</summary>
  public MaParameter()
  {
    Functions.MachNichts();
  }

  /// <summary>Gets or sets the value of column Mandant_Nr.</summary>
  public int Mandant_Nr { get; set; }

  /// <summary>Gets or sets the value of column Schluessel.</summary>
  public string Schluessel { get; set; }

  /// <summary>Gets or sets the value of column Wert.</summary>
  public string Wert { get; set; }

  /// <summary>Gets or sets the value of column Angelegt_Von.</summary>
  public string Angelegt_Von { get; set; }

  /// <summary>Gets or sets the value of column Angelegt_Am.</summary>
  public DateTime? Angelegt_Am { get; set; }

  /// <summary>Gets or sets the value of column Geaendert_Von.</summary>
  public string Geaendert_Von { get; set; }

  /// <summary>Gets or sets the value of column Geaendert_Am.</summary>
  public DateTime? Geaendert_Am { get; set; }

  /// <summary>Gets or sets the value of column Replikation_Uid.</summary>
  public string Replikation_Uid { get; set; }
}
