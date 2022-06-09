// <copyright file="SoKurse.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models;

using System;
using System.ComponentModel.DataAnnotations.Schema;
using CSBP.Base;

/// <summary>
/// Entity class for table SO_Kurse.
/// </summary>
[Serializable]
[Table("SO_Kurse")]
public partial class SoKurse : ModelBase
{
  /// <summary>Initializes a new instance of the <see cref="SoKurse"/> class.</summary>
  public SoKurse()
  {
    Functions.MachNichts();
  }

  /// <summary>Gets or sets the value of column Datum.</summary>
  public DateTime Datum { get; set; }

  /// <summary>Gets or sets the value of column Open.</summary>
  public decimal Open { get; set; }

  /// <summary>Gets or sets the value of column High.</summary>
  public decimal High { get; set; }

  /// <summary>Gets or sets the value of column Low.</summary>
  public decimal Low { get; set; }

  /// <summary>Gets or sets the value of column Close.</summary>
  public decimal Close { get; set; }

  /// <summary>Gets or sets the value of column Price.</summary>
  public decimal Price { get; set; }

  /// <summary>Gets or sets the value of column Bewertung.</summary>
  public string Bewertung { get; set; }

  /// <summary>Gets or sets the value of column Trend.</summary>
  public string Trend { get; set; }

  /// <summary>Gets or sets the value of column Bemerkung.</summary>
  public string Bemerkung { get; set; }
}
