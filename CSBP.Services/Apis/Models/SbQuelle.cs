// <copyright file="SbQuelle.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Apis.Models;

using System;
using System.ComponentModel.DataAnnotations.Schema;
using CSBP.Services.Base;

/// <summary>
/// Entity class for table SB_Quelle.
/// </summary>
[Serializable]
[Table("SB_Quelle")]
public partial class SbQuelle : ModelBase
{
  /// <summary>Initializes a new instance of the <see cref="SbQuelle"/> class.</summary>
  public SbQuelle()
  {
    Functions.MachNichts();
  }

  /// <summary>Gets or sets the value of column Mandant_Nr.</summary>
  public int Mandant_Nr { get; set; }

  /// <summary>Gets or sets the value of column Uid.</summary>
  public string Uid { get; set; }

  /// <summary>Gets or sets the value of column Beschreibung.</summary>
  public string Beschreibung { get; set; }

  /// <summary>Gets or sets the value of column Zitat.</summary>
  public string Zitat { get; set; }

  /// <summary>Gets or sets the value of column Bemerkung.</summary>
  public string Bemerkung { get; set; }

  /// <summary>Gets or sets the value of column Autor.</summary>
  public string Autor { get; set; }

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
