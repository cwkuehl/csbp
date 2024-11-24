// <copyright file="TbEintragOrt.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Apis.Models;

using System.ComponentModel.DataAnnotations.Schema;
using CSBP.Services.Base;

/// <summary>
/// Entity class for table TB_Eintrag_Ort.
/// </summary>
public partial class TbEintragOrt : ModelBase
{
  /// <summary>Gets or sets the Description.</summary>
  [NotMapped]
  public string Description { get; set; }

  /// <summary>Gets or sets the Latitude.</summary>
  [NotMapped]
  public decimal Latitude { get; set; }

  /// <summary>Gets or sets the Longitude.</summary>
  [NotMapped]
  public decimal Longitude { get; set; }

  /// <summary>Gets or sets the Height.</summary>
  [NotMapped]
  public decimal Height { get; set; }

  /// <summary>Gets or sets the memo.</summary>
  [NotMapped]
  public string Memo { get; set; }

  /// <summary>
  /// Gets hash of values.
  /// </summary>
  /// <returns>Hash of values.</returns>
  public string Hash()
  {
    return $"{Ort_Uid ?? ""}#{Datum_Von}#{Datum_Bis}";
  }
}
