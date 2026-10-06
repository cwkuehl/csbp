// <copyright file="ServiceDaten.cs" company="cwkuehl.de">
//   Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Base;

using BlazorSpa.Base.Models;
using BlazorSpa.Base.Services;
using CSBP.Services.Undo;

/// <summary>
/// Contains all data for executing service functions.
/// </summary>
public class ServiceDaten : ServiceDatenBase
{
  /// <summary>
  /// Initializes a new instance of the <see cref="ServiceDaten"/> class.
  /// </summary>
  /// <param name="sessionId">Betroffene Sitzung.</param>
  /// <param name="mandantNr">Betroffene Mandantennummer.</param>
  /// <param name="benutzerId">Betroffener Benutzer.</param>
  /// <param name="rollen">Betroffene Rollen.</param>
  public ServiceDaten(string sessionId, int mandantNr, string benutzerId, List<string> rollen)
    : base(new UserDaten(sessionId, mandantNr, benutzerId, rollen))
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="ServiceDaten"/> class.
  /// </summary>
  /// <param name="daten">Betroffene Benutzerdaten.</param>
  public ServiceDaten(UserDaten daten)
    : base(daten)
  {
  }

  /// <summary>Gets or sets the undo list.</summary>
  public UndoList UndoList { get; set; }
}
