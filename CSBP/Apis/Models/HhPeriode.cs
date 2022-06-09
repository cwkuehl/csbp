// <copyright file="HhPeriode.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models;

using System;
using System.ComponentModel.DataAnnotations.Schema;
using CSBP.Base;

/// <summary>
/// Entity class for table HH_Periode.
/// </summary>
[Serializable]
[Table("HH_Periode")]
public partial class HhPeriode : ModelBase
{
  /// <summary>Initializes a new instance of the <see cref="HhPeriode"/> class.</summary>
  public HhPeriode()
  {
    Functions.MachNichts();
  }

  /// <summary>Gets or sets the value of column Mandant_Nr.</summary>
  public int Mandant_Nr { get; set; }

  /// <summary>Gets or sets the value of column Nr.</summary>
  public int Nr { get; set; }

  /// <summary>Gets or sets the value of column Datum_Von.</summary>
  public DateTime Datum_Von { get; set; }

  /// <summary>Gets or sets the value of column Datum_Bis.</summary>
  public DateTime Datum_Bis { get; set; }

  /// <summary>Gets or sets the value of column Art.</summary>
  public int Art { get; set; }

  /// <summary>Gets or sets the value of column Angelegt_Von.</summary>
  public string Angelegt_Von { get; set; }

  /// <summary>Gets or sets the value of column Angelegt_Am.</summary>
  public DateTime? Angelegt_Am { get; set; }

  /// <summary>Gets or sets the value of column Geaendert_Von.</summary>
  public string Geaendert_Von { get; set; }

  /// <summary>Gets or sets the value of column Geaendert_Am.</summary>
  public DateTime? Geaendert_Am { get; set; }
}
