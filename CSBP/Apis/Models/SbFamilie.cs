// <copyright file="SbFamilie.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models;

using System;
using System.ComponentModel.DataAnnotations.Schema;
using CSBP.Base;

/// <summary>
/// Entity class for table SB_Familie.
/// </summary>
[Serializable]
[Table("SB_Familie")]
public partial class SbFamilie : ModelBase
{
  /// <summary>Initializes a new instance of the <see cref="SbFamilie"/> class.</summary>
  public SbFamilie()
  {
    Functions.MachNichts();
  }

  /// <summary>Gets or sets the value of column Mandant_Nr.</summary>
  public int Mandant_Nr { get; set; }

  /// <summary>Gets or sets the value of column Uid.</summary>
  public string Uid { get; set; }

  /// <summary>Gets or sets the value of column Mann_Uid.</summary>
  public string Mann_Uid { get; set; }

  /// <summary>Gets or sets the value of column Frau_Uid.</summary>
  public string Frau_Uid { get; set; }

  /// <summary>Gets or sets the value of column Status1.</summary>
  public int Status1 { get; set; }

  /// <summary>Gets or sets the value of column Status2.</summary>
  public int Status2 { get; set; }

  /// <summary>Gets or sets the value of column Status3.</summary>
  public int Status3 { get; set; }

  /// <summary>Gets or sets the value of column Angelegt_Von.</summary>
  public string Angelegt_Von { get; set; }

  /// <summary>Gets or sets the value of column Angelegt_Am.</summary>
  public DateTime? Angelegt_Am { get; set; }

  /// <summary>Gets or sets the value of column Geaendert_Von.</summary>
  public string Geaendert_Von { get; set; }

  /// <summary>Gets or sets the value of column Geaendert_Am.</summary>
  public DateTime? Geaendert_Am { get; set; }
}
