// <copyright file="FzBuchstatus.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Apis.Models;

using System;
using System.ComponentModel.DataAnnotations.Schema;
using CSBP.Services.Base;

/// <summary>
/// Entity class for table FZ_Buchstatus.
/// </summary>
[Serializable]
[Table("FZ_Buchstatus")]
public partial class FzBuchstatus : ModelBase
{
  /// <summary>Initializes a new instance of the <see cref="FzBuchstatus"/> class.</summary>
  public FzBuchstatus()
  {
    Functions.MachNichts();
  }

  /// <summary>Gets or sets the value of column Mandant_Nr.</summary>
  public int Mandant_Nr { get; set; }

  /// <summary>Gets or sets the value of column Buch_Uid.</summary>
  public string Buch_Uid { get; set; }

  /// <summary>Gets or sets a value indicating whether column Ist_Besitz is true.</summary>
  public bool Ist_Besitz { get; set; }

  /// <summary>Gets or sets the value of column Lesedatum.</summary>
  public DateTime? Lesedatum { get; set; }

  /// <summary>Gets or sets the value of column Hoerdatum.</summary>
  public DateTime? Hoerdatum { get; set; }

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
