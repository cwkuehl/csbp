// <copyright file="PermissionEnum.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Apis.Enums;

/// <summary>Enum for user permissions.</summary>
public enum PermissionEnum
{
  /// <summary>No permission.</summary>
  Without = -1,

  /// <summary>Normal user permission.</summary>
  User = 0,

  /// <summary>Admin permission for client.</summary>
  Admin = 1,

  /// <summary>Admin permission for all clients.</summary>
  All = 2,
}
