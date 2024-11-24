// <copyright file="FzBuch.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Apis.Models;

using System;
using System.ComponentModel.DataAnnotations.Schema;
using CSBP.Services.Base;

/// <summary>
/// Entity class for table FZ_Buch.
/// </summary>
[Serializable]
[Table("FZ_Buch")]
public partial class FzBuch : ModelBase
{
  /// <summary>Initializes a new instance of the <see cref="FzBuch"/> class.</summary>
  public FzBuch()
  {
    Functions.MachNichts();
  }

  /// <summary>Gets or sets the value of column Mandant_Nr.</summary>
  public int Mandant_Nr { get; set; }

  /// <summary>Gets or sets the value of column Uid.</summary>
  public string Uid { get; set; }

  /// <summary>Gets or sets the value of column Autor_Uid.</summary>
  public string Autor_Uid { get; set; }

  /// <summary>Gets or sets the value of column Serie_Uid.</summary>
  public string Serie_Uid { get; set; }

  /// <summary>Gets or sets the value of column Seriennummer.</summary>
  public int Seriennummer { get; set; }

  /// <summary>Gets or sets the value of column Titel.</summary>
  public string Titel { get; set; }

  /// <summary>Gets or sets the value of column Untertitel.</summary>
  public string Untertitel { get; set; }

  /// <summary>Gets or sets the value of column Seiten.</summary>
  public int Seiten { get; set; }

  /// <summary>Gets or sets the value of column Sprache_Nr.</summary>
  public int Sprache_Nr { get; set; }

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
