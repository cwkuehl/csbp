// <copyright file="Benutzer.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Apis.Models;

using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using CSBP.Services.Base;
using CSBP.Services.Resources;

/// <summary>
/// Entity class for table Benutzer.
/// </summary>
public partial class Benutzer : ModelBase
{
  /// <summary>Gets the permission as string.</summary>
  ////[NotMapped]
  public string Permission => this.Berechtigung == 2 ? Messages.Enum_permission_all
    : this.Berechtigung == 1 ? Messages.Enum_permission_admin
    : this.Berechtigung == 0 ? Messages.Enum_permission_user
    : Messages.Enum_permission_no;

  /// <summary>Gets or sets the form data.</summary>
  [NotMapped]
  public string FormData { get; set; }

  /// <summary>
  /// Gets all extended values as string.
  /// </summary>
  /// <returns>Extended values as string.</returns>
  protected override string GetExtension()
  {
    var sb = new StringBuilder();
    sb.Append(ToString(FormData)).Append(';');
    return sb.ToString();
  }

  /// <summary>
  /// Sets all extended values from string.
  /// </summary>
  /// <param name="v">Extended values as string.</param>
  protected override void SetExtension(string v)
  {
    var arr = (v ?? "").Split(';');
    FormData = arr.Length > 0 ? arr[0] ?? "" : null;
  }
}
