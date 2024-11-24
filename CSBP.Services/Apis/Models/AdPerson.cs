// <copyright file="AdPerson.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Apis.Models;

using System;
using System.ComponentModel.DataAnnotations.Schema;
using CSBP.Services.Base;

/// <summary>
/// Entity class for table AD_Person.
/// </summary>
[Serializable]
[Table("AD_Person")]
public partial class AdPerson : ModelBase
{
  /// <summary>Initializes a new instance of the <see cref="AdPerson"/> class.</summary>
  public AdPerson()
  {
    Functions.MachNichts();
  }

  /// <summary>Gets or sets the value of column Mandant_Nr.</summary>
  public int Mandant_Nr { get; set; }

  /// <summary>Gets or sets the value of column Uid.</summary>
  public string Uid { get; set; }

  /// <summary>Gets or sets the value of column Typ.</summary>
  public int Typ { get; set; }

  /// <summary>Gets or sets the value of column Geschlecht.</summary>
  public string Geschlecht { get; set; }

  /// <summary>Gets or sets the value of column Geburt.</summary>
  public DateTime? Geburt { get; set; }

  /// <summary>Gets or sets the value of column GeburtK.</summary>
  public int GeburtK { get; set; }

  /// <summary>Gets or sets the value of column Anrede.</summary>
  public int Anrede { get; set; }

  /// <summary>Gets or sets the value of column FAnrede.</summary>
  public int FAnrede { get; set; }

  /// <summary>Gets or sets the value of column Name1.</summary>
  public string Name1 { get; set; }

  /// <summary>Gets or sets the value of column Name2.</summary>
  public string Name2 { get; set; }

  /// <summary>Gets or sets the value of column Praedikat.</summary>
  public string Praedikat { get; set; }

  /// <summary>Gets or sets the value of column Vorname.</summary>
  public string Vorname { get; set; }

  /// <summary>Gets or sets the value of column Titel.</summary>
  public string Titel { get; set; }

  /// <summary>Gets or sets the value of column Person_Status.</summary>
  public int Person_Status { get; set; }

  /// <summary>Gets or sets the value of column Angelegt_Von.</summary>
  public string Angelegt_Von { get; set; }

  /// <summary>Gets or sets the value of column Angelegt_Am.</summary>
  public DateTime? Angelegt_Am { get; set; }

  /// <summary>Gets or sets the value of column Geaendert_Von.</summary>
  public string Geaendert_Von { get; set; }

  /// <summary>Gets or sets the value of column Geaendert_Am.</summary>
  public DateTime? Geaendert_Am { get; set; }
}
