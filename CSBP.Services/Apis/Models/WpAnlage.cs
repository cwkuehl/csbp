// <copyright file="WpAnlage.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Apis.Models;

using System;
using System.ComponentModel.DataAnnotations.Schema;
using CSBP.Services.Base;

/// <summary>
/// Entity class for table WP_Anlage.
/// </summary>
[Serializable]
[Table("WP_Anlage")]
public partial class WpAnlage : ModelBase
{
  /// <summary>Initializes a new instance of the <see cref="WpAnlage"/> class.</summary>
  public WpAnlage()
  {
    Functions.MachNichts();
  }

  /// <summary>Gets or sets the value of column Mandant_Nr.</summary>
  public int Mandant_Nr { get; set; }

  /// <summary>Gets or sets the value of column Uid.</summary>
  public string Uid { get; set; }

  /// <summary>Gets or sets the value of column Wertpapier_Uid.</summary>
  public string Wertpapier_Uid { get; set; }

  /// <summary>Gets or sets the value of column Bezeichnung.</summary>
  public string Bezeichnung { get; set; }

  /// <summary>Gets or sets the value of column Parameter.</summary>
  public string Parameter
  {
    get { return GetExtension(); }
    set { SetExtension(value); }
  }

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
