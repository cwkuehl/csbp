// <copyright file="AdAdresse.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Apis.Models;

using System;
using System.ComponentModel.DataAnnotations.Schema;
using CSBP.Services.Base;

/// <summary>
/// Entity class for table AD_Adresse.
/// </summary>
[Serializable]
[Table("AD_Adresse")]
public partial class AdAdresse : ModelBase
{
  /// <summary>Initializes a new instance of the <see cref="AdAdresse"/> class.</summary>
  public AdAdresse()
  {
    Functions.MachNichts();
  }

  /// <summary>Gets or sets the value of column Mandant_Nr.</summary>
  public int Mandant_Nr { get; set; }

  /// <summary>Gets or sets the value of column Uid.</summary>
  public string Uid { get; set; }

  /// <summary>Gets or sets the value of column Staat.</summary>
  public string Staat { get; set; }

  /// <summary>Gets or sets the value of column Plz.</summary>
  public string Plz { get; set; }

  /// <summary>Gets or sets the value of column Ort.</summary>
  public string Ort { get; set; }

  /// <summary>Gets or sets the value of column Strasse.</summary>
  public string Strasse { get; set; }

  /// <summary>Gets or sets the value of column HausNr.</summary>
  public string HausNr { get; set; }

  /// <summary>Gets or sets the value of column Angelegt_Von.</summary>
  public string Angelegt_Von { get; set; }

  /// <summary>Gets or sets the value of column Angelegt_Am.</summary>
  public DateTime? Angelegt_Am { get; set; }

  /// <summary>Gets or sets the value of column Geaendert_Von.</summary>
  public string Geaendert_Von { get; set; }

  /// <summary>Gets or sets the value of column Geaendert_Am.</summary>
  public DateTime? Geaendert_Am { get; set; }
}
