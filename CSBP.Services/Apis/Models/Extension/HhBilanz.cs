// <copyright file="HhBilanz.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Apis.Models;

using System.ComponentModel.DataAnnotations.Schema;
using CSBP.Services.Base;

/// <summary>
/// Entity class for table HH_Bilanz.
/// </summary>
public partial class HhBilanz : ModelBase
{
  /// <summary>Gets or sets the account name.</summary>
  [NotMapped]
  public string AccountName { get; set; }

  /// <summary>Gets or sets the sorting id.</summary>
  [NotMapped]
  public string AccountSort { get; set; }

  /// <summary>Gets or sets the account type.</summary>
  [NotMapped]
  public int AccountType { get; set; }

  /// <summary>Gets or sets the initial account sum.</summary>
  [NotMapped]
  public decimal AccountSum { get; set; }

  /// <summary>Gets or sets the initial account euro sum.</summary>
  [NotMapped]
  public decimal AccountEsum { get; set; }
}
