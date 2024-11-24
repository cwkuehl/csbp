// <copyright file="MaMandant.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Apis.Models;

using System;
using System.ComponentModel.DataAnnotations.Schema;
using CSBP.Services.Base;

/// <summary>
/// Entity class for table MA_Mandant.
/// </summary>
[Serializable]
[Table("MA_Mandant")]
public partial class MaMandant : ModelBase
{
  /// <summary>Initializes a new instance of the <see cref="MaMandant"/> class.</summary>
  public MaMandant()
  {
    Functions.MachNichts();
  }

  /// <summary>Gets or sets the value of column Nr.</summary>
  public int Nr { get; set; }

  /// <summary>Gets or sets the value of column Beschreibung.</summary>
  public string Beschreibung { get; set; }

  /// <summary>Gets or sets the value of column Angelegt_Von.</summary>
  public string Angelegt_Von { get; set; }

  /// <summary>Gets or sets the value of column Angelegt_Am.</summary>
  public DateTime? Angelegt_Am { get; set; }

  /// <summary>Gets or sets the value of column Geaendert_Von.</summary>
  public string Geaendert_Von { get; set; }

  /// <summary>Gets or sets the value of column Geaendert_Am.</summary>
  public DateTime? Geaendert_Am { get; set; }
}
