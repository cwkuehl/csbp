// <copyright file="SbPerson.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Apis.Models;

using System;
using System.ComponentModel.DataAnnotations.Schema;
using CSBP.Services.Base;

/// <summary>
/// Entity class for table SB_Person.
/// </summary>
[Serializable]
[Table("SB_Person")]
public partial class SbPerson : ModelBase
{
  /// <summary>Initializes a new instance of the <see cref="SbPerson"/> class.</summary>
  public SbPerson()
  {
    Functions.MachNichts();
  }

  /// <summary>Gets or sets the value of column Mandant_Nr.</summary>
  public int Mandant_Nr { get; set; }

  /// <summary>Gets or sets the value of column Uid.</summary>
  public string Uid { get; set; }

  /// <summary>Gets or sets the value of column Name.</summary>
  public string Name { get; set; }

  /// <summary>Gets or sets the value of column Vorname.</summary>
  public string Vorname { get; set; }

  /// <summary>Gets or sets the value of column Geburtsname.</summary>
  public string Geburtsname { get; set; }

  /// <summary>Gets or sets the value of column Geschlecht.</summary>
  public string Geschlecht { get; set; }

  /// <summary>Gets or sets the value of column Titel.</summary>
  public string Titel { get; set; }

  /// <summary>Gets or sets the value of column Konfession.</summary>
  public string Konfession { get; set; }

  /// <summary>Gets or sets the value of column Bemerkung.</summary>
  public string Bemerkung { get; set; }

  /// <summary>Gets or sets the value of column Quelle_Uid.</summary>
  public string Quelle_Uid { get; set; }

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
