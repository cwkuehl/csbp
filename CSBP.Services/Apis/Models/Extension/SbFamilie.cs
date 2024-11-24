// <copyright file="SbFamilie.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Apis.Models;

using System.ComponentModel.DataAnnotations.Schema;
using CSBP.Services.Base;

/// <summary>
/// Entity class for table SB_Familie.
/// </summary>
public partial class SbFamilie : ModelBase
{
  /// <summary>Gets or sets the marriage date.</summary>
  [NotMapped]
  public string Marriagedate { get; set; }

  /// <summary>Gets or sets the marriage place.</summary>
  [NotMapped]
  public string Marriageplace { get; set; }

  /// <summary>Gets or sets the marriage memo.</summary>
  [NotMapped]
  public string Marriagememo { get; set; }

  /// <summary>Gets or sets the father.</summary>
  [NotMapped]
  public SbPerson Father { get; set; }

  /// <summary>Gets or sets the mother.</summary>
  [NotMapped]
  public SbPerson Mother { get; set; }
}
