﻿// <copyright file="HhBuchung.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Apis.Models;

using System;
using System.ComponentModel.DataAnnotations.Schema;
using CSBP.Services.Base;

/// <summary>
/// Entity class for table HH_Buchung.
/// </summary>
public partial class HhBuchung : ModelBase
{
  /// <summary>Gets or sets the debit account name.</summary>
  [NotMapped]
  public string DebitName { get; set; }

  /// <summary>Gets or sets the debit account from date.</summary>
  [NotMapped]
  public DateTime? DebitFrom { get; set; }

  /// <summary>Gets or sets the debit account to date.</summary>
  [NotMapped]
  public DateTime? DebitTo { get; set; }

  /// <summary>Gets or sets the debit account type.</summary>
  [NotMapped]
  public string DebitType { get; set; }

  /// <summary>Gets or sets the credit account name.</summary>
  [NotMapped]
  public string CreditName { get; set; }

  /// <summary>Gets or sets the credit account from date.</summary>
  [NotMapped]
  public DateTime? CreditFrom { get; set; }

  /// <summary>Gets or sets the credit account to date.</summary>
  [NotMapped]
  public DateTime? CreditTo { get; set; }

  /// <summary>Gets or sets the credit account type.</summary>
  [NotMapped]
  public string CreditType { get; set; }
}
