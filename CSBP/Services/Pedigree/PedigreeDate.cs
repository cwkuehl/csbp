// <copyright file="PedigreeDate.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

using System;
using System.Text;
using System.Text.RegularExpressions;
using CSBP.Base;
using static CSBP.Resources.M;
using static CSBP.Resources.Messages;

namespace CSBP.Services.Pedigree
{
  /// <summary>
  /// Date class for pedigree.
  /// </summary>
  public class PedigreeDate
  {
    /// <summary>Ist das Datum leer.</summary>
    public bool Empty { get { return Tag <= 0 && Monat <= 0 && Jahr == 0; } }

    /// <summary>Day of month (1-31 or 0).</summary>
    public int Tag { get; private set; } = 0;

    /// <summary>Month (1-12 or 0).</summary>
    public int Monat { get; private set; } = 0;

    /// <summary>Year a.d. (1-99999 or 0).</summary>
    public int Jahr { get; private set; } = 0;

    /// <summary>Trenner zwischen kurzem und langem Jahr.</summary>
    private const int JAHR_KURZ = 10000;

    /// <summary>Constructor with initialization.</summary>
    public PedigreeDate(int date = 0, int month = 0, int year = 0)
    {
      Tag = Math.Max(0, date);
      Monat = Math.Max(0, month);
      Jahr = year;
    }

    /// <summary>Initialization of date.</summary>
    public void Init()
    {
      Tag = 0;
      Monat = 0;
      Jahr = 0;
    }

    /// <summary>Zuweisung der Werte eines anderen Datums.</summary>
    /// <param name="v">Betroffenes Datum.</param>
    public void Set(PedigreeDate v)
    {
      if (v == null)
        return;
      Tag = v.Tag;
      Monat = v.Monat;
      Jahr = v.Jahr;
    }

    /**
     * Parst das übergebene Datum und speichert es in den internen Daten.
     * @param parseDatum Zu parsendes Datum.
     * @return Nicht analysierter Teil-String des Datums.
     */
    public string Parse(string parseDatum)
    {
      var datum = parseDatum;
      Init();
      if (datum != null)
      {
        // alles außer Ziffern und Punkten am Ende entfernen
        var m = Regex.Match(datum, "(.+?)[^\\d\\.]*$");
        if (m.Success)
        {
          datum = m.Groups[1].Value;
          m = Regex.Match(datum, "(.*?)([\\d]+)$");
          if (m.Success)
          {
            // letzte Zahl ist Jahr
            datum = m.Groups[1].Value;
            Jahr = Functions.ToInt32(m.Groups[2].Value);
            m = Regex.Match(datum, "(.*?)([\\d]+)\\.$");
            if (m.Success)
            {
              // letzte Zahl mit Punkt ist Monat
              datum = m.Groups[1].Value;
              Monat = Functions.ToInt32(m.Groups[2].Value);
              m = Regex.Match(datum, "(.*?)([\\d]+)\\.$");
              if (m.Success)
              {
                // letzte Zahl mit Punkt ist Tag
                datum = m.Groups[1].Value;
                Tag = Functions.ToInt32(m.Groups[2].Value);
              }
            }
          }
        }
      }
      return datum;
    }

    /**
     * Liefert das interne Datum als String.
     * Falls gedcom gesetzt ist, wird folgendes Format benutzt DD MMM YYYY; Sonst DD.MM.YYYY.
     * @param gedcom true, wenn Datum in GEDCOM-Datei benötigt wird.
     * @return Datum als String.
     */
    public string Deparse(bool gedcom = false)
    {
      var datum = new StringBuilder();
      var trenner = gedcom ? " " : ".";
      if (Tag != 0)
        datum.Append($"{Tag:00}");
      if (Monat != 0)
      {
        if (datum.Length > 0)
          datum.Append(trenner);
        if (gedcom)
          datum.Append(new DateTime(2000, Monat, 1).ToString("MMM", Functions.CultureInfoEn));
        else
          datum.Append($"{Monat:00}");
      }
      if (Jahr != 0)
      {
        if (datum.Length > 0)
          datum.Append(trenner);
        if (Jahr < JAHR_KURZ)
          datum.Append($"{Jahr:0000}");
        else
          datum.Append($"{Jahr:00000}");
      }
      return datum.ToString();
    }
  }
}