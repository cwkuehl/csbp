// <copyright file="ILoginService.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Services
{
  public interface ILoginService
  {
    /// <summary>
    /// Is login wihtout password.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <returns>Is anything changed?</returns>
    ServiceErgebnis<bool> IsWithoutPassword(ServiceDaten daten);

    ServiceErgebnis Login(ServiceDaten daten, string kennwort, bool speichern);
    ServiceErgebnis ChangePassword(ServiceDaten daten, int client, string id, string passwordold,
        string passwordnew, bool speichern);
    ServiceErgebnis Logout(ServiceDaten daten);

    /// <summary>
    /// Undo last transaction.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <returns>Is anything changed?</returns>
    ServiceErgebnis<bool> Undo(ServiceDaten daten);

    /// <summary>
    /// Redo last transaction.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <returns>Is anything changed?</returns>
    ServiceErgebnis<bool> Redo(ServiceDaten daten);

    /// <summary>Test function.</summary>
    /// <param name="daten">Service data for database access.</param>
    /// <returns>Possibly errors.</returns>
    ServiceErgebnis Test(ServiceDaten daten);
  }
}