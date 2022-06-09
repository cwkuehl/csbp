// <copyright file="FzBuchautor.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models;

using System;
using System.ComponentModel.DataAnnotations.Schema;
using CSBP.Base;

/// <summary>
/// Entity class for table FZ_Buchautor.
/// </summary>
[Serializable]
[Table("FZ_Buchautor")]
public partial class FzBuchautor : ModelBase
{
  /// <summary>Initializes a new instance of the <see cref="FzBuchautor"/> class.</summary>
  public FzBuchautor()
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
