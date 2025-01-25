// <copyright file="UserDaten.cs" company="cwkuehl.de">
//   Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Base;

/// <summary>
/// Contains user data after login.
/// </summary>
public class UserDaten
{
  /// <summary>Name des User-Rolle.</summary>
  public const string RoleUser = "User";

  /// <summary>Name des Admin-Rolle.</summary>
  public const string RoleAdmin = "Admin";

  /// <summary>Name des Superadmin-Rolle.</summary>
  public const string RoleSuperadmin = "Superadmin";

  /// <summary>
  /// Initializes a new instance of the <see cref="UserDaten"/> class.
  /// </summary>
  /// <param name="sessionId">Affected session id.</param>
  /// <param name="mandantNr">Affected client number.</param>
  /// <param name="benutzerId">Affected user id.</param>
  /// <param name="rollen">Affected user roles.</param>
  public UserDaten(string sessionId, int mandantNr, string benutzerId, List<string> rollen)
  {
    SessionId = sessionId;
    MandantNr = mandantNr;
    BenutzerId = benutzerId;
    if (rollen != null)
      Rollen.AddRange(rollen);
  }

  /// <summary>Gets the session id.</summary>
  public string SessionId { get; }

  /// <summary>Gets the client number.</summary>
  public int MandantNr { get; }

  /// <summary>Gets the user id.</summary>
  public string BenutzerId { get; }

  /// <summary>Gets the user roles.</summary>
  public List<string> Rollen { get; } = [];

  /// <summary>Gets a new session id.</summary>
  /// <returns>New session id.</returns>
  public static string GetNewSessionId()
  {
    return Guid.NewGuid().ToString();
  }
}
