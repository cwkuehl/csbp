// <copyright file="ServiceDaten.cs" company="cwkuehl.de">
//   Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Services
{
  using System;
  using CSBP.Services.Undo;

  /// <summary>
  /// Diese Klasse enthält alle Daten zum Ausführen einer Service-Funktion.
  /// </summary>
  public class ServiceDaten
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="ServiceDaten" /> Klasse.
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

    /// <summary>
    /// Holt die Mandantennummer.
    /// </summary>
    public int MandantNr { get; }

    /// <summary>
    /// Holt den Benutzer.
    /// </summary>
    public string BenutzerId { get; }

    /// <summary>
    /// Holt das aktuelle Datum.
    /// </summary>
    public DateTime Heute { get; }

    /// <summary>
    /// Holt den aktuellen Zeitpunkt.
    /// </summary>
    public DateTime Jetzt { get; }

    /// <summary>
    /// Holt oder setzt die interne Transaktion und darf nicht verändert werden.
    /// </summary>
    public object Tx { get; set; }

    /// <summary>
    /// Holt oder setzt die interne Schachteltiefe.
    /// </summary>
    public int Tiefe { get; set; }

    /// <summary>
    /// Holt oder setzt den Datenbank-Kontext.
    /// </summary>
    public object Context { get; set; }

    /// <summary>
    /// Holt oder setzt die Undo-Liste.
    /// </summary>
    public UndoList UndoList { get; set; }
  }
}
