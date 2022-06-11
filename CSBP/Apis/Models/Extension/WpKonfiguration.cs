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
  /// <summary>Holt oder setzt die erweiterte Bezeichnung.</summary>
  [NotMapped]
  public string Description { get; set; }

  /// <summary>Holt oder setzt die Boxgröße.</summary>
  [NotMapped]
  public decimal? Box { get; set; }

  /// <summary>Holt oder setzt die Skala.</summary>
  [NotMapped]
  public int Scale { get; set; }

  /// <summary>Holt oder setzt die Umkehr.</summary>
  [NotMapped]
  public int Reversal { get; set; }

  /// <summary>Holt oder setzt die Methode.</summary>
  [NotMapped]
  public int Method { get; set; }

  /// <summary>Holt oder setzt die Dauer in Tagen.</summary>
  [NotMapped]
  public int Duration { get; set; }

  /// <summary>Holt oder setzt einen Wert, der angibt, ob es relativ ist.</summary>
  [NotMapped]
  public bool Relative { get; set; }

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
