// <copyright file="ILoginService.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Apis.Services;

using CSBP.Services.Base;

/// <summary>
/// Interface for login service.
/// </summary>
public interface ILoginService
{
  /// <summary>
  /// Checks if the login without password.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <returns>User data with roles if login is without password or null.</returns>
  ServiceErgebnis<UserDaten> IsWithoutPassword(ServiceDaten daten);

  /// <summary>
  /// Do login with all checks.
  /// </summary>
  /// <param name="daten">Affected client number and user id.</param>
  /// <param name="kennwort">Affected password.</param>
  /// <param name="save">Save password or not.</param>
  /// <returns>User data with roles and possibly errors.</returns>
  ServiceErgebnis<UserDaten> Login(ServiceDaten daten, string kennwort, bool save);

  /// <summary>
  /// Changes the password of an user.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="client">Affected client id.</param>
  /// <param name="id">Affected user id.</param>
  /// <param name="passwordold">Old password.</param>
  /// <param name="passwordnew">New password.</param>
  /// <param name="save">Save password or not.</param>
  /// <returns>Possibly errors.</returns>
  ServiceErgebnis ChangePassword(ServiceDaten daten, int client, string id, string passwordold,
      string passwordnew, bool save);

  /// <summary>
  /// Does the logout.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <returns>Possibly errors.</returns>
  ServiceErgebnis Logout(ServiceDaten daten);

  /// <summary>
  /// Undo last transaction.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <returns>Is anything changed or not.</returns>
  ServiceErgebnis<bool> Undo(ServiceDaten daten);

  /// <summary>
  /// Redo last transaction.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <returns>Is anything changed or not.</returns>
  ServiceErgebnis<bool> Redo(ServiceDaten daten);

  /// <summary>Test function.</summary>
  /// <param name="daten">Service data for database access.</param>
  /// <returns>Possibly errors.</returns>
  ServiceErgebnis Test(ServiceDaten daten);
}
