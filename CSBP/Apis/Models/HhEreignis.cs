// <copyright file="HhEreignis.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models;

using System;
using System.ComponentModel.DataAnnotations.Schema;
using CSBP.Base;

/// <summary>
/// Entity class for table HH_Ereignis.
/// </summary>
[Serializable]
[Table("HH_Ereignis")]
public partial class HhEreignis : ModelBase
{
  /// <summary>Initializes a new instance of the <see cref="HhEreignis"/> class.</summary>
  public HhEreignis()
  {
    Functions.MachNichts();
  }

  /// <summary>Gets or sets the value of column Mandant_Nr.</summary>
  public int Mandant_Nr { get; set; }

  /// <summary>Gets or sets the value of column Uid.</summary>
  public string Uid { get; set; }

  /// <summary>Gets or sets the value of column Kz.</summary>
  public string Kz { get; set; }

  /// <summary>Gets or sets the value of column Soll_Konto_Uid.</summary>
  public string Soll_Konto_Uid { get; set; }

  /// <summary>Gets or sets the value of column Haben_Konto_Uid.</summary>
  public string Haben_Konto_Uid { get; set; }

  /// <summary>Gets or sets the value of column Bezeichnung.</summary>
  public string Bezeichnung { get; set; }

  /// <summary>Gets or sets the value of column EText.</summary>
  public string EText { get; set; }

  /// <summary>Gets or sets the value of column Angelegt_Von.</summary>
  public string Angelegt_Von { get; set; }

  /// <summary>Gets or sets the value of column Angelegt_Am.</summary>
  public DateTime? Angelegt_Am { get; set; }

  /// <summary>Gets or sets the value of column Geaendert_Von.</summary>
  public string Geaendert_Von { get; set; }

  /// <summary>Gets or sets the value of column Geaendert_Am.</summary>
  public DateTime? Geaendert_Am { get; set; }
}
