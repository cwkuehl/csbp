// <copyright file="AccountRow.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Budget;

using System;
using CSBP.Services.Base;

/// <summary>
/// Entity class for an account row.
/// </summary>
[Serializable]
public partial class AccountRow : ModelBase
{
  /// <summary>Gets or sets number in left column.</summary>
  public string Nr { get; set; }

  /// <summary>Gets or sets name in left column.</summary>
  public string Name { get; set; }

  /// <summary>Gets or sets amount in left column.</summary>
  public decimal? Value { get; set; }

  /// <summary>Gets or sets number in right column.</summary>
  public string Nr2 { get; set; }

  /// <summary>Gets or sets name in right column.</summary>
  public string Name2 { get; set; }

  /// <summary>Gets or sets amount in right column.</summary>
  public decimal? Value2 { get; set; }
}
