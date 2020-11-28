// <copyright file="PnfDate.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

using System;

namespace CSBP.Services.Pnf
{
  /// <summary>Diese Klasse regelt das Schreiben des Monatzeichens in das Chart.</summary>
  public class PnfDate
  {
    public PnfDate()
    {
    }

    /// <summary>Aktueller Kurs-Zeitpunkt.</summary>
    private DateTime? datum = null;

    /// <summary>Letztes Datum der Säule zur Bestimmung des Monatsanfangs.</summary>
    private DateTime? datumm = null;

    /// <summary>Aktuell zu schreibender Monat.</summary>
    private int monat = 0;

    public DateTime? getDatum()
    {
      return datum;
    }

    public void setDatum(DateTime? datum)
    {
      this.datum = datum;
      monat = 0;
      if (!datum.HasValue)
        return;
      var d = datum.Value;
      if (!datumm.HasValue || (datumm.Value.Year * 100 + datumm.Value.Month < d.Year * 100 + d.Month))
      {
        monat = d.Month;
      }
    }

    public char getNeuerMonat(char c)
    {
      if (monat > 0)
      {
        if (monat < 10)
        {
          return (char)((int)'0' + monat);
        }
        return (char)((int)'A' + monat - 10);
      }
      return c;
    }

    public void setMonatVerwendet()
    {
      if (datum.HasValue)
      {
        datumm = datum;
      }
      monat = 0;
    }
  }
}
