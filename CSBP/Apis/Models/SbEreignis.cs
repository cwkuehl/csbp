// <copyright file="SbEreignis.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models;

using System;
using System.ComponentModel.DataAnnotations.Schema;
using CSBP.Base;

/// <summary>
/// Entity class for table SB_Ereignis.
/// </summary>
[Serializable]
[Table("SB_Ereignis")]
public partial class SbEreignis : ModelBase
{
  /// <summary>Initializes a new instance of the <see cref="SbEreignis"/> class.</summary>
  public SbEreignis()
  {
    Functions.MachNichts();
  }

  /// <summary>Gets or sets the value of column Mandant_Nr.</summary>
  public int Mandant_Nr { get; set; }

  /// <summary>Gets or sets the value of column Person_Uid.</summary>
  public string Person_Uid { get; set; }

  /// <summary>Gets or sets the value of column Familie_Uid.</summary>
  public string Familie_Uid { get; set; }

  /// <summary>Gets or sets the value of column Typ.</summary>
  public string Typ { get; set; }

  /// <summary>Gets or sets the value of column Tag1.</summary>
  public int Tag1 { get; set; }

  /// <summary>Gets or sets the value of column Monat1.</summary>
  public int Monat1 { get; set; }

  /// <summary>Gets or sets the value of column Jahr1.</summary>
  public int Jahr1 { get; set; }

  /// <summary>Gets or sets the value of column Tag2.</summary>
  public int Tag2 { get; set; }

  /// <summary>Gets or sets the value of column Monat2.</summary>
  public int Monat2 { get; set; }

  /// <summary>Gets or sets the value of column Jahr2.</summary>
  public int Jahr2 { get; set; }

  /// <summary>Gets or sets the value of column Datum_Typ.</summary>
  public string Datum_Typ { get; set; }

  /// <summary>Gets or sets the value of column Ort.</summary>
  public string Ort { get; set; }

  /// <summary>Gets or sets the value of column Bemerkung.</summary>
  public string Bemerkung { get; set; }

  /// <summary>Gets or sets the value of column Quelle_Uid.</summary>
  public string Quelle_Uid { get; set; }

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
