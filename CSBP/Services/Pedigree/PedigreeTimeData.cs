// <copyright file="PedigreeTimeData.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Pedigree;

using System;
using System.Text.RegularExpressions;
using CSBP.Apis.Models;
using CSBP.Base;
using static CSBP.Resources.M;

/// <summary>
/// Time data class for pedigree.
/// </summary>
public class PedigreeTimeData
{
  /// <summary>Regex for month format 1.</summary>
  private static readonly Regex Month1 = new("((\\d) +)(JAN|FEB|MAR|APR|MAY|JUN|JUL|AUG|SEP|OCT|NOV|DEC)( +(\\d))", RegexOptions.Compiled);

  /// <summary>Regex for month format 2.</summary>
  private static readonly Regex Month2 = new("(JAN|FEB|MAR|APR|MAY|JUN|JUL|AUG|SEP|OCT|NOV|DEC)( +(\\d))", RegexOptions.Compiled);

  /// <summary>Array of month names.</summary>
  private static readonly string[] Months = new[] { "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" };

  /// <summary>
  /// Initializes a new instance of the <see cref="PedigreeTimeData"/> class.
  /// </summary>
  /// <param name="d1">Affected first date.</param>
  /// <param name="d2">Affected second date.</param>
  /// <param name="datetype">Affected date type relation.</param>
  public PedigreeTimeData(PedigreeDate d1 = null, PedigreeDate d2 = null, string datetype = null)
  {
    Date1 = d1 ?? new PedigreeDate();
    Date2 = d2 ?? new PedigreeDate();
    DateType = datetype ?? "";
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="PedigreeTimeData"/> class.
  /// </summary>
  /// <param name="e">Affected event.</param>
  public PedigreeTimeData(SbEreignis e)
  {
    Date1 = e == null ? new PedigreeDate() : new PedigreeDate(e.Tag1, e.Monat1, e.Jahr1);
    Date2 = e == null ? new PedigreeDate() : new PedigreeDate(e.Tag2, e.Monat2, e.Jahr2);
    DateType = e?.Datum_Typ ?? "";
  }

  /// <summary>Gets first Date.</summary>
  public PedigreeDate Date1 { get; private set; }

  /// <summary>Gets second Date.</summary>
  public PedigreeDate Date2 { get; private set; }

  /// <summary>Gets date type relation: EXAC, ABT, BET, BEF, AFT or OR.</summary>
  public string DateType { get; private set; }

  /// <summary>Initialization of the dates.</summary>
  public void Init()
  {
    Date1.Init();
    Date2.Init();
    DateType = "";
  }

  /// <summary>
  /// Formats internal dates as string.
  /// </summary>
  /// <param name="gedcom">Formats for GEDCOM file or not.</param>
  /// <returns>Formatted string.</returns>
  public string Deparse(bool gedcom = false)
  {
    var dat1 = Date1.Deparse(gedcom);
    var dat2 = Date2.Deparse(gedcom);
    string datum;
    if (gedcom)
    {
      if (DateType == "EXAC")
        datum = Date1.Deparse(gedcom);
      else if (DateType == "ABT" || DateType == "AFT" || DateType == "BEF")
        datum = $"{DateType} {dat1}";
      else if (DateType == "BET")
        datum = $"BET {dat1} AND {dat2}";
      else
        datum = Functions.Append(dat1, " OR ", dat2);
    }
    else
    {
      if (DateType == "EXAC")
        datum = dat1;
      else if (DateType == "ABT")
        datum = SB031(dat1);
      else if (DateType == "AFT")
        datum = SB032(dat1);
      else if (DateType == "BEF")
        datum = SB033(dat1);
      else if (DateType == "BET")
        datum = SB034(dat1, dat2);
      else if (DateType == "OR")
        datum = SB035(dat1, dat2);
      else
        datum = Functions.Append(dat1, ", ", dat2);
    }
    return datum;
  }

  /// <summary>
  /// Parses string dates.
  /// </summary>
  /// <param name="datumString">Affected string with dates.</param>
  /// <param name="gedcom">Parses for GEDCOM file or not.</param>
  /// <returns>Are there unparsed parts of the string or not.</returns>
  public bool Parse(string datumString, bool gedcom = false)
  {
    var mitText = false;
    var hinten = "";
    var mitte = "";
    //// var vorn = "";
    var datum = datumString;
    if (gedcom)
    {
      // [JAN|FEB|MAR|APR|MAY|JUN|JUL|AUG|SEP|OCT|NOV|DEC] ersetzen durch Monatsnummer
      var m = Month1.Match(datum);
      while (m.Success)
      {
        datum = datum.Replace(m.Groups[1].Value + m.Groups[3].Value + m.Groups[4].Value,
          $"{m.Groups[2].Value}.{GetMonat(m.Groups[3].Value)}.{m.Groups[5].Value}");
        m = m.NextMatch();
      }
      m = Month2.Match(datum);
      while (m.Success)
      {
        datum = datum.Replace(m.Groups[1].Value + m.Groups[2].Value, $"{GetMonat(m.Groups[1].Value)}.{m.Groups[3].Value}");
        m = m.NextMatch();
      }
    }
    Init();
    if (!string.IsNullOrEmpty(datum))
    {
      mitText = Regex.Match(datum, "[^\\d\\.]+").Success;
      var m = Regex.Match(datum, "^(.*?)([^\\d\\.]*)$");
      datum = m.Groups[1].Value;
      hinten = m.Groups[2].Value;
      datum = Date2.Parse(datum);
      DateType = "EXAC";
      m = Regex.Match(datum, "^(.*?)([^\\d\\.]*)$");
      //// datum = m.Groups[1].Value;
      mitte = m.Groups[2].Value;
      //// vorn = Date1.Parse(datum);
    }
    var b1 = !Date1.Empty;
    var b2 = !Date2.Empty;
    //// vorn = vorn.Replace(" ", ""); // Leerzeichen entfernen
    mitte = mitte.Replace(" ", "").ToLower(); // Leerzeichen entfernen
    hinten = hinten.Replace(" ", ""); // Leerzeichen entfernen
    if (b1 == b2)
    {
      if (b1)
      {
        if (Regex.Match(mitte, "\\/|\\-|bis|and").Success)
          DateType = "BET"; // zwischen zwei Zeitpunkten
        else if (Regex.Match(mitte, "oder|or").Success)
          DateType = "OR"; // zwei Zeitpunkte
        else
          mitText = true; // printf "unbekannte Mitte: '$mitte' bei '$kopie'\n";
      }
      else
        DateType = "";
    }
    else
    {
      if (mitte.Length > 0)
      {
        if (Regex.Match(mitte, "um|about|abt").Success)
          DateType = "ABT"; // ungefähres Datum
        else if (Regex.Match(mitte, "nach|frühestens|after|aft").Success)
          DateType = "AFT"; // nach einem Zeitpunkt
        else if (Regex.Match(mitte, "vor|before|bef").Success)
          DateType = "BEF"; // vor einem Zeitpunkt
        else
          mitText = true; // printf "unbekanntes Vorne: '$mitte' bei '$kopie'\n";
      }
      if (hinten.Length > 0)
      {
        if (Regex.Match(hinten, "\\?").Success)
          DateType = "ABT"; // ungefähres Datum
        else
          mitText = true; // printf "unbekanntes Hinten: '$hinten' bei '$kopie'\n";
      }
      if (b2)
      {
        Date1.Set(Date2);
        Date2.Init();
      }
    }
    return mitText;
  }

  /// <summary>
  /// Gets month as integer, beginning with 1 for January.
  /// </summary>
  /// <param name="monat">Affected month.</param>
  /// <returns>Month as integer.</returns>
  private static int GetMonat(string monat)
  {
    var m = Array.IndexOf(Months, monat);
    if (m >= 0)
      return m + 1;
    return -1;
  }
}
