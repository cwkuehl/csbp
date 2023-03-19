// <copyright file="TbOrt.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models;

using System;
using System.ComponentModel.DataAnnotations.Schema;
using CSBP.Base;

/// <summary>
/// Entity class for table TB_Ort.
/// </summary>
[Serializable]
[Table("TB_Ort")]
public partial class TbOrt : ModelBase
{
  /// <summary>Initializes a new instance of the <see cref="TbOrt"/> class.</summary>
  public TbOrt()
  {
    Functions.MachNichts();
  }

  /// <summary>Gets or sets the value of column Mandant_Nr.</summary>
  public int Mandant_Nr { get; set; }

  /// <summary>Gets or sets the value of column Uid.</summary>
  public string Uid { get; set; }

  /// <summary>Gets or sets the value of column Bezeichnung.</summary>
  public string Bezeichnung { get; set; }

  /// <summary>Gets or sets the value of column Breite.</summary>
  public decimal Breite { get; set; }

  /// <summary>Gets or sets the value of column Laenge.</summary>
  public decimal Laenge { get; set; }

  /// <summary>Gets or sets the value of column Hoehe.</summary>
  public decimal Hoehe { get; set; }

  /// <summary>Gets or sets the value of column Zeitzone.</summary>
  public string Zeitzone { get; set; }

  /// <summary>Gets or sets the value of column Notiz.</summary>
  public string Notiz { get; set; }

  /// <summary>Gets or sets the value of column Angelegt_Von.</summary>
  public string Angelegt_Von { get; set; }

  /// <summary>Gets or sets the value of column Angelegt_Am.</summary>
  public DateTime? Angelegt_Am { get; set; }

  /// <summary>Gets or sets the value of column Geaendert_Von.</summary>
  public string Geaendert_Von { get; set; }

  /// <summary>Gets or sets the value of column Geaendert_Am.</summary>
  public DateTime? Geaendert_Am { get; set; }
}
