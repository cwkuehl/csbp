// <copyright file="WpStand.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Apis.Models;

using System.ComponentModel.DataAnnotations.Schema;
using CSBP.Services.Base;

/// <summary>
/// Entity class for table WP_Stand.
/// </summary>
public partial class WpStand : ModelBase
{
  /// <summary>Gets or sets the stock description.</summary>
  [NotMapped]
  public string StockDescription { get; set; }
}
