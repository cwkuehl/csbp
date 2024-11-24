// <copyright file="Benutzer.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Apis.Models;

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
}
