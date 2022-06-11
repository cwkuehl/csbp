// <copyright file="WpKonfiguration.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models;

using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using CSBP.Base;

/// <summary>
/// Entity class for table WP_Konfiguration.
/// </summary>
public partial class WpKonfiguration : ModelBase
{
  /// <summary>Gets or sets the extended Description.</summary>
  [NotMapped]
  public string Description { get; set; }

  /// <summary>Gets or sets the box size.</summary>
  [NotMapped]
  public decimal? Box { get; set; }

  /// <summary>Gets or sets the scale.</summary>
  [NotMapped]
  public int Scale { get; set; }

  /// <summary>Gets or sets the reversal.</summary>
  [NotMapped]
  public int Reversal { get; set; }

  /// <summary>Gets or sets the method.</summary>
  [NotMapped]
  public int Method { get; set; }

  /// <summary>Gets or sets the duration in days.</summary>
  [NotMapped]
  public int Duration { get; set; }

  /// <summary>Gets or sets a value indicating whether the chart can be relativ.</summary>
  [NotMapped]
  public bool Relative { get; set; }

  /// <summary>
  /// Gets all extended values as string.
  /// </summary>
  /// <returns>Extended values as string.</returns>
  protected override string GetExtension()
  {
    var sb = new StringBuilder();
    sb.Append(ToString(Box)).Append(';');
    sb.Append(ToString(false)).Append(';');
    sb.Append(ToString(Reversal)).Append(';');
    sb.Append(ToString(Method)).Append(';');
    sb.Append(ToString(Duration)).Append(';');
    sb.Append(ToString(Relative)).Append(';');
    sb.Append(ToString(Scale)).Append(';');
    return sb.ToString();
  }

  /// <summary>
  /// Sets all extended values from string.
  /// </summary>
  /// <param name="value">Extended values as string.</param>
  protected override void SetExtension(string value)
  {
    var arr = (value ?? "").Split(';');
    Box = arr.Length > 0 ? ToDecimal(arr[0]) : null;
    Reversal = arr.Length > 2 ? ToInt(arr[2]) ?? 0 : 0;
    Method = arr.Length > 3 ? ToInt(arr[3]) ?? 0 : 0;
    Duration = arr.Length > 4 ? ToInt(arr[4]) ?? 0 : 0;
    Relative = arr.Length > 5 && (ToBool(arr[5]) ?? false);
    Scale = arr.Length > 6 ? ToInt(arr[6]) ?? 0 : 0;
  }
}
