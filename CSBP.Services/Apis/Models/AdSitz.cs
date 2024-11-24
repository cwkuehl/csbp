// <copyright file="AdSitz.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Apis.Models;

using System;
using System.ComponentModel.DataAnnotations.Schema;
using CSBP.Services.Base;

/// <summary>
/// Entity class for table AD_Sitz.
/// </summary>
[Serializable]
[Table("AD_Sitz")]
public partial class AdSitz : ModelBase
{
  /// <summary>Initializes a new instance of the <see cref="AdSitz"/> class.</summary>
  public AdSitz()
  {
    Functions.MachNichts();
  }

  /// <summary>Gets or sets the value of column Mandant_Nr.</summary>
  public int Mandant_Nr { get; set; }

  /// <summary>Gets or sets the value of column Person_Uid.</summary>
  public string Person_Uid { get; set; }

  /// <summary>Gets or sets the value of column Reihenfolge.</summary>
  public int Reihenfolge { get; set; }

  /// <summary>Gets or sets the value of column Uid.</summary>
  public string Uid { get; set; }

  /// <summary>Gets or sets the value of column Typ.</summary>
  public int Typ { get; set; }

  /// <summary>Gets or sets the value of column Name.</summary>
  public string Name { get; set; }

  /// <summary>Gets or sets the value of column Adresse_Uid.</summary>
  public string Adresse_Uid { get; set; }

  /// <summary>Gets or sets the value of column Telefon.</summary>
  public string Telefon { get; set; }

  /// <summary>Gets or sets the value of column Fax.</summary>
  public string Fax { get; set; }

  /// <summary>Gets or sets the value of column Mobil.</summary>
  public string Mobil { get; set; }

  /// <summary>Gets or sets the value of column Email.</summary>
  public string Email { get; set; }

  /// <summary>Gets or sets the value of column Homepage.</summary>
  public string Homepage { get; set; }

  /// <summary>Gets or sets the value of column Postfach.</summary>
  public string Postfach { get; set; }

  /// <summary>Gets or sets the value of column Bemerkung.</summary>
  public string Bemerkung { get; set; }

  /// <summary>Gets or sets the value of column Sitz_Status.</summary>
  public int Sitz_Status { get; set; }

  /// <summary>Gets or sets the value of column Angelegt_Von.</summary>
  public string Angelegt_Von { get; set; }

  /// <summary>Gets or sets the value of column Angelegt_Am.</summary>
  public DateTime? Angelegt_Am { get; set; }

  /// <summary>Gets or sets the value of column Geaendert_Von.</summary>
  public string Geaendert_Von { get; set; }

  /// <summary>Gets or sets the value of column Geaendert_Am.</summary>
  public DateTime? Geaendert_Am { get; set; }
}
