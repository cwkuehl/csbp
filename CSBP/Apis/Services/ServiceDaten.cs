// <copyright file="ServiceDaten.cs" company="cwkuehl.de">
//   Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Services;

using System;
using CSBP.Services.Undo;

/// <summary>
/// Contains all data for executing service functions.
/// </summary>
public class ServiceDaten
{
  /// <summary>
  /// Initializes a new instance of the <see cref="ServiceDaten"/> class.
  /// </summary>
  /// <param name="mandantNr">Betroffene Mandantennummer.</param>
  /// <param name="benutzerId">Betroffener Benutzer.</param>
  public ServiceDaten(int mandantNr, string benutzerId)
  {
    MandantNr = mandantNr;
    BenutzerId = benutzerId;
    Heute = DateTime.Today;
    Jetzt = DateTime.Now;
    Jetzt = Jetzt.AddTicks(-Jetzt.Ticks % 10000000); // nur sekundengenau
  }

  /// <summary>Gets the client number.</summary>
  public int MandantNr { get; }

  /// <summary>Gets the user id.</summary>
  public string BenutzerId { get; }

  /// <summary>Gets the actual date.</summary>
  public DateTime Heute { get; }

  /// <summary>Gets the actual time.</summary>
  public DateTime Jetzt { get; }

  /// <summary>Gets or sets the internal transaction. Do not change.</summary>
  public object Tx { get; set; }

  /// <summary>Gets or sets the internal depth.</summary>
  public int Tiefe { get; set; }

  /// <summary>Gets or sets the database context.</summary>
  public object Context { get; set; }

  /// <summary>Gets or sets the undo list.</summary>
  public UndoList UndoList { get; set; }
}
