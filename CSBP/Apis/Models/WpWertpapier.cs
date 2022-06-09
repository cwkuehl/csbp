// <copyright file="WpWertpapier.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models;

using System;
using System.ComponentModel.DataAnnotations.Schema;
using CSBP.Base;

/// <summary>
/// Entity class for table WP_Wertpapier.
/// </summary>
[Serializable]
[Table("WP_Wertpapier")]
public partial class WpWertpapier : ModelBase
{
  /// <summary>Initializes a new instance of the <see cref="WpWertpapier"/> class.</summary>
  public WpWertpapier()
  {
    Functions.MachNichts();
  }

  /// <summary>Gets or sets the value of column Mandant_Nr.</summary>
  public int Mandant_Nr { get; set; }

  /// <summary>Gets or sets the value of column Uid.</summary>
  public string Uid { get; set; }

  /// <summary>Gets or sets the value of column Bezeichnung.</summary>
  public string Bezeichnung { get; set; }

  /// <summary>Gets or sets the value of column Kuerzel.</summary>
  public string Kuerzel { get; set; }

  /// <summary>Gets or sets the value of column Parameter.</summary>
  public string Parameter
  {
    get { return GetExtension(); }
    set { SetExtension(value); }
  }

  /// <summary>Gets or sets the value of column Datenquelle.</summary>
  public string Datenquelle { get; set; }

  /// <summary>Gets or sets the value of column Status.</summary>
  public string Status { get; set; }

  /// <summary>Gets or sets the value of column Relation_Uid.</summary>
  public string Relation_Uid { get; set; }

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
