// <copyright file="TbEintragOrt.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Apis.Models;

using System;
using System.ComponentModel.DataAnnotations.Schema;
using CSBP.Services.Base;

/// <summary>
/// Entity class for table TB_Eintrag_Ort.
/// </summary>
[Serializable]
[Table("TB_Eintrag_Ort")]
public partial class TbEintragOrt : ModelBase
{
  /// <summary>Initializes a new instance of the <see cref="TbEintragOrt"/> class.</summary>
  public TbEintragOrt()
  {
    Functions.MachNichts();
  }

  /// <summary>Gets or sets the value of column Mandant_Nr.</summary>
  public int Mandant_Nr { get; set; }

  /// <summary>Gets or sets the value of column Ort_Uid.</summary>
  public string Ort_Uid { get; set; }

  /// <summary>Gets or sets the value of column Datum_Von.</summary>
  public DateTime Datum_Von { get; set; }

  /// <summary>Gets or sets the value of column Datum_Bis.</summary>
  public DateTime Datum_Bis { get; set; }

  /// <summary>Gets or sets the value of column Angelegt_Von.</summary>
  public string Angelegt_Von { get; set; }

  /// <summary>Gets or sets the value of column Angelegt_Am.</summary>
  public DateTime? Angelegt_Am { get; set; }

  /// <summary>Gets or sets the value of column Geaendert_Von.</summary>
  public string Geaendert_Von { get; set; }

  /// <summary>Gets or sets the value of column Geaendert_Am.</summary>
  public DateTime? Geaendert_Am { get; set; }
}
