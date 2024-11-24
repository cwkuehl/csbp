// <copyright file="TbEintrag.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Apis.Models;

using System;
using System.ComponentModel.DataAnnotations.Schema;
using CSBP.Services.Base;

/// <summary>
/// Entity class for table TB_Eintrag.
/// </summary>
[Serializable]
[Table("TB_Eintrag")]
public partial class TbEintrag : ModelBase
{
  /// <summary>Initializes a new instance of the <see cref="TbEintrag"/> class.</summary>
  public TbEintrag()
  {
    Functions.MachNichts();
  }

  /// <summary>Gets or sets the value of column Mandant_Nr.</summary>
  public int Mandant_Nr { get; set; }

  /// <summary>Gets or sets the value of column Datum.</summary>
  public DateTime Datum { get; set; }

  /// <summary>Gets or sets the value of column Eintrag.</summary>
  public string Eintrag { get; set; }

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
