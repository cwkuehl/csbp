// <copyright file="Functions.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Base;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Serialization;
using static CSBP.Resources.M;
using static CSBP.Resources.Messages;

public static class Functions
{
  static readonly RandomNumberGenerator csp = RandomNumberGenerator.Create();
  private static CultureInfo CultureInfo = CultureInfo.CreateSpecificCulture("de-DE");
  private static readonly CultureInfo CultureInfoDe = CultureInfo.CreateSpecificCulture("de-DE");
  private static CultureInfo cultureInfoEn = CultureInfo.CreateSpecificCulture("en-GB");

  /// <summary>
  /// Setzen der Sprache.
  /// </summary>
  /// <param name="ci">Betroffenes CultureInfo.</param>
  public static void SetCultureInfo(CultureInfo ci)
  {
    if (ci == null)
      return;
    CultureInfo = ci;
    Thread.CurrentThread.CurrentCulture = ci;
    Thread.CurrentThread.CurrentUICulture = ci;
    // Messages.Culture = ci;
  }

  /// <summary>
  /// Is the actual language German?
  /// </summary>
  /// <value>True if actual language German.</value>
  public static bool IsDe
  {
    get { return CultureInfo.TwoLetterISOLanguageName == "de"; }
  }

  public static CultureInfo CultureInfoEn { get => cultureInfoEn; set => cultureInfoEn = value; }

  /// <summary>
  /// Funktion, die nichts macht.
  /// </summary>
  /// <param name="obj">Optionaler Parameter wird nicht verwendet.</param>
  /// <returns>Zahl 0.</returns>
  public static int MachNichts(object obj = null)
  {
    if (obj == null)
      return 0;
    return 0;
  }

  /// <summary>
  /// Wandelt einen String in einen Integer um.
  /// </summary>
  /// <returns>String als Integer.</returns>
  /// <param name="s">Zu konvertierender String.</param>
  public static int ToInt32(string s)
  {
    var d = ToDecimal(s, 0);
    if (d.HasValue && int.MinValue <= d.Value && d.Value <= int.MaxValue)
      return (int)d.Value;
    return 0;
  }

  /// <summary>
  /// Wandelt einen String in einen Long um.
  /// </summary>
  /// <returns>String als Integer.</returns>
  /// <param name="s">Zu konvertierender String.</param>
  public static long ToInt64(string s)
  {
    if (string.IsNullOrWhiteSpace(s) || !long.TryParse(s, out long l))
    {
      return 0;
    }
    return l;
  }

  /// <summary>
  /// Wandelt einen String in einen Decimal um.
  /// </summary>
  /// <returns>String als Decimal.</returns>
  /// <param name="s">Zu konvertierender String.</param>
  /// <param name="digits">Number of digits to round.</param>
  /// <param name="english">Parse with English culture.</param>
  public static decimal? ToDecimal(string s, int digits = -1, bool english = false)
  {
    if (!string.IsNullOrWhiteSpace(s) && decimal.TryParse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands,
        english ? CultureInfoEn : CultureInfo, out var d))
    {
      if (digits >= 0)
        d = Math.Round(d, digits, MidpointRounding.AwayFromZero);
      return d;
    }
    return null;
  }

  /// <summary>
  /// Wandelt einen deutschen String in einen Decimal um.
  /// </summary>
  /// <returns>String als Decimal.</returns>
  /// <param name="s">Zu konvertierender String.</param>
  public static decimal? ToDecimalDe(string s)
  {
    if (!string.IsNullOrWhiteSpace(s) && decimal.TryParse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands,
        CultureInfoDe, out var d))
      return d;
    return null;
  }

  /// <summary>
  /// Wandelt einen String in einen Decimal um, je nach CultureInfo.
  /// </summary>
  /// <returns>String als Decimal.</returns>
  /// <param name="s">Zu konvertierender String.</param>
  /// <param name="digits">Number of digits to round.</param>
  /// <param name="english">Parse with English culture.</param>
  public static decimal? ToDecimalCi(string s, int digits = -1)
  {
    if (!string.IsNullOrWhiteSpace(s) && decimal.TryParse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo, out var d))
    {
      if (digits >= 0)
        d = Math.Round(d, digits, MidpointRounding.AwayFromZero);
      return d;
    }
    return null;
  }

  /// <summary>
  /// Wandelt String in nullable bool um.
  /// </summary>
  /// <param name="s">Betroffener String</param>
  /// <returns>String als nullabel bool.</returns>
  public static bool? ToBool(string s)
  {
    if (!string.IsNullOrWhiteSpace(s) && bool.TryParse(s, out var d))
      return d;
    return null;
  }

  /// <summary>
  /// Wandelt den ersten Buchenstaben in Großbuchstaben um, der Rest wird klein.
  /// </summary>
  /// <returns>Umgewandelter String.</returns>
  /// <param name="s">Betroffer String.</param>
  public static string ToFirstUpper(this string s)
  {
    if (string.IsNullOrEmpty(s))
    {
      return string.Empty;
    }
    return char.ToUpper(s[0]) + s[1..].ToLower();
  }

  /// <summary>
  /// Trimmt einen String und gibt null zurück, falls er leer ist.
  /// </summary>
  /// <returns>Getrimmter String oder null.</returns>
  /// <param name="s">Betroffer String.</param>
  /// <param name="trim">Soll getrimmt werden?</param>
  public static string TrimNull(this string s, bool trim = true)
  {
    if (trim)
      s = s?.Trim();
    if (string.IsNullOrEmpty(s))
    {
      return null;
    }
    return s;
  }

  /// <summary>
  /// Liefert den linken Teil eines String, maximal der Länge l, niemals null.
  /// </summary>
  /// <returns>Getrimmter String oder null.</returns>
  /// <param name="s">Betroffer String.</param>
  /// <param name="l">Maximale Stringlänge.</param>
  public static string Left(this string s, int l = 1)
  {
    if (string.IsNullOrEmpty(s) || l <= 0)
    {
      return "";
    }
    return s[..Math.Min(l, s.Length)];
  }

  /// <summary>Zusammenbau eines Strings. Das Objekt wird immer zu dem StringBuffer hinzugefügt. Die Füll-Strings werden vor und
  ///  hinter dem Objekt eingefügt, wenn StringBuffer und Objekt nicht leere Strings darstellen.
  /// <b>Beispiele</b>:
  /// strB = "Name"; anhaengen(strB, ", ", "Vorname", "");
  /// liefert: strB = "Name, Vorname";
  /// strB = "Name"; anhaengen(strB, ", ", "", "");
  /// liefert: strB = "Name";
  /// strB = "Name"; anhaengen(strB, " (", "Titel", ")");
  /// liefert: strB = "Name (Vorname)";</summary>
  /// <param name="sb">Betroffener StringBuilder.</param>
  /// <param name="filler1">Erster Füll-String.</param>
  /// <param name="obj">String, der immer angehängt.</param>
  /// <param name="filler2">Zweiter Füll-String.</param>
  /// <returns>Gleichen StringBuilder für Fluent API.</returns>
  public static StringBuilder Append(this StringBuilder sb, string filler1, string obj, string filler2 = null)
  {
    if (sb != null)
    {
      var s = ToString(obj);
      if (sb.Length > 0 && s.Length > 0)
      {
        if (!string.IsNullOrEmpty(filler1))
        {
          sb.Append(filler1);
        }
        sb.Append(s);
        if (!string.IsNullOrEmpty(filler2))
        {
          sb.Append(filler2);
        }
      }
      else
      {
        sb.Append(s);
      }
    }
    return sb;
  }

  /**
   * Zusammenbau eines String aus zwei Objekten. Nur wenn beide Objekte nicht leere Strings darstellen, wird der
   * Füll-String dazwischen eingefügt.
   * <p>
   * @param objB Erstes Objekt.
   * @param filler Erster Füll-String.
   * @param obj Zweites Objekt.
   * @return Zusammengesetzter String.
   */
  public static string Append(string objB, string filler, string obj)
  {
    var strObjB = (objB ?? "").TrimEnd();
    var strObj = (obj ?? "").TrimEnd();
    if (strObjB.Length > 0 && strObj.Length > 0)
    {
      if (!string.IsNullOrEmpty(filler))
        strObjB += filler;
    }
    strObjB += strObj;
    return strObjB;
  }

  /// <summary>
  /// Wandelt einen Tabellenname in Camelcase mit erstem Großbuchstaben.
  /// Unterstriche werden entfernt, diese Teile beginnen wieder groß.
  /// </summary>
  /// <param name="t">Betroffener Tabellenname.</param>
  /// <returns>Berechneter Tabellenname.</returns>
  public static string TabName(string t)
  {
    var arr = t.Split('_');
    return string.Join(string.Empty, arr.Select(a => ToFirstUpper(a)));
  }

  /// <summary>
  /// Muss eine like-Operation gemacht werden: Nicht leer und nicht % oder %%.
  /// </summary>
  /// <param name="t">Betroffener Suchstring.</param>
  /// <returns>Muss eine like-Operation gemacht werden?</returns>
  public static bool IsLike(string t)
  {
    return !(string.IsNullOrEmpty(t) || t == "%" || t == "%%");
  }

  /// <summary>
  /// Vergleich zweier Strings liefert:
  /// 0, falls s1 = s2 und
  /// 1, falls s1 != s2.
  /// Dabei wird nicht zwischen leerem String und null unterschieden.
  /// </summary>
  /// <param name="s1">Erster String-Wert.</param>
  /// <param name="s2">Zweiter String-Wert.</param>
  /// <returns>0 oder +1.</returns>
  public static int CompString(string s1, string s2)
  {
    if (string.IsNullOrEmpty(s1) != string.IsNullOrEmpty(s2))
    {
      return 1;
    }
    if (!string.IsNullOrEmpty(s1) && string.Compare(s1, s2) != 0)
    {
      return 1;
    }
    return 0;
  }

  /// <summary>
  /// Liefert neue Guid als String.
  /// </summary>
  /// <returns>Guid als String.</returns>
  public static string GetUid()
  {
    var uid = Guid.NewGuid().ToString();
    return uid;
  }

  /// <summary>
  /// Liefert einen Dateinamen mit aktuellem Datum und Zufallszahl.
  /// </summary>
  /// <returns>Zusammengesetzter Dateiname.</returns>
  /// <param name="name">Name am Anfang.</param>
  /// <param name="datum">Soll das aktuelle Datum eingefügt werden?</param>
  /// <param name="zufall">Soll eine Zufallszahl eingefügt werden?</param>
  /// <param name="endung">Dateiendung ohne Punkt.</param>
  public static String GetDateiname(string name, bool datum, bool zufall, string endung)
  {
    var sb = new StringBuilder();
    if (!string.IsNullOrEmpty(name))
    {
      sb.Append(name);
    }
    if (datum)
    {
      sb.Append('_').Append(DateTime.Today.ToString("yyyyMMdd"));
    }
    if (zufall)
    {
      sb.Append('_').Append(NextRandom(1000, 10000));
    }
    if (!string.IsNullOrEmpty(endung))
    {
      sb.Append('.').Append(endung);
    }
    return sb.ToString();
  }

  ///<summary>Entfernt evtl. vorhandene Semikola.</summary>
  public static string ToString(string s)
  {
    if (string.IsNullOrEmpty(s))
      return "";
    return s;
  }

  /// <summary>
  /// Liefert Integer als String.
  /// </summary>
  /// <param name="i">Betroffener Wert.</param>
  /// <returns>Integer als String.</returns>
  public static string ToString(int? i)
  {
    if (!i.HasValue)
      return string.Empty;
    return i.Value.ToString(CultureInfo);
  }

  /// <summary>
  /// Liefert Bool als String.
  /// </summary>
  /// <param name="i">Betroffener Wert.</param>
  /// <returns>Bool als String.</returns>
  public static string ToString(bool? i)
  {
    if (!i.HasValue)
      return string.Empty;
    return i.Value ? "true" : "false";
  }

  /// <summary>
  /// Liefert Decimal als String.
  /// </summary>
  /// <param name="d">Betroffener Wert.</param>
  /// <param name="digits">Number of digits to print.</param>
  /// <param name="ci">Affected culture info.</param>
  /// <returns>Decimal als String.</returns>
  public static string ToString(decimal? d, int digits = -1, CultureInfo ci = null)
  {
    if (!d.HasValue)
      return string.Empty;
    return d.Value.ToString(digits < 0 ? "N" : $"N{digits}", ci ?? CultureInfo);
  }

  /// <summary>
  /// Liefert Datum im Format yyyy-MM-dd als String.
  /// </summary>
  /// <param name="d">Betroffenes Datum.</param>
  /// <param name="time">Soll die Uhrzeit angehängt werden.</param>
  /// <param name="milli">Sollen die Millisekunden auch angehängt werden.</param>
  /// <returns>Datum im Format yyyy-MM-dd als String.</returns>
  public static string ToString(DateTime? d, bool time = false, bool milli = false)
  {
    if (!d.HasValue)
      return string.Empty;
    if (time)
    {
      if (milli)
        return d.Value.ToString("yyyy-MM-dd HH:mm:ss.fffffff");
      return d.Value.ToString("yyyy-MM-dd HH:mm:ss");
    }
    return d.Value.ToString("yyyy-MM-dd");
  }

  /// <summary>
  /// Liefert Datum im Format dd.MM.yyyy als String.
  /// </summary>
  /// <param name="d">Betroffenes Datum.</param>
  /// <param name="time">Soll die Uhrzeit angehängt werden.</param>
  /// <returns>Datum im Format dd.MM.yyyy als String.</returns>
  public static string ToStringDe(DateTime? d, bool time = false)
  {
    if (!d.HasValue)
      return string.Empty;
    if (time)
      return d.Value.ToString("dd.MM.yyyy HH:mm:ss");
    return d.Value.ToString("dd.MM.yyyy");
  }

  /// <summary>
  /// Liefert Wochentag als String.
  /// </summary>
  /// <param name="d">Betroffenes Datum</param>
  /// <returns>Wochentag als String.</returns>
  public static string ToStringWd(DateTime? d)
  {
    if (!d.HasValue)
      return string.Empty;
    return d.Value.ToString("dddd");
  }

  /// <summary>
  /// Korrigiert Datum in die locale Zeitzone.
  /// </summary>
  /// <param name="d">Betroffenes Datum.</param>
  /// <returns>Datum in der locale Zeitzone.</returns>
  public static DateTime? ToDateTimeLocal(DateTime? d)
  {
    if (!d.HasValue)
      return null;
    return TimeZoneInfo.ConvertTimeFromUtc(d.Value, TimeZoneInfo.Local);
  }

  /// <summary>
  /// Korrigiert locales Datum in UTC.
  /// </summary>
  /// <param name="d">Betroffenes Datum.</param>
  /// <returns>Datum in UTC.</returns>
  public static DateTime? ToDateTimeUtc(DateTime? d)
  {
    if (!d.HasValue)
      return null;
    return TimeZoneInfo.ConvertTimeFromUtc(d.Value, TimeZoneInfo.Utc);
  }

  /// <summary>
  /// Liefert String im Format yyyy-MM-dd HH:mm:ss als DateTime.
  /// </summary>
  /// <param name="s">Betroffener String</param>
  /// <returns>String im Format yyyy-MM-dd als DateTime.</returns>
  public static DateTime? ToDateTime(string s)
  {
    if (string.IsNullOrWhiteSpace(s))
      return null;
    if (s.EndsWith(".0"))
      s = s[..^2];
    if (!DateTime.TryParseExact(s, "yyyy-MM-d HH:mm:ss", null, DateTimeStyles.None, out var d))
      if (!DateTime.TryParseExact(s, "dd.MM.yyyy HH:mm:ss", null, DateTimeStyles.None, out d))
        if (!DateTime.TryParseExact(s, "yyyy-MM-d", null, DateTimeStyles.None, out d))
          if (!DateTime.TryParseExact(s, "d.M.yyyy", null, DateTimeStyles.None, out d))
            return null;
    return d;
  }

  private static readonly DateTime EpochStart = new(1970, 1, 1);

  /// <summary>
  /// Liefert Epochen-Sekunden nach dem 1.1.1970 als DateTime.
  /// </summary>
  /// <param name="s">Betroffene Anzahl Sekunden.</param>
  /// <returns>Epochen-Sekunden nach dem 1.1.1970 als DateTime.</returns>
  public static DateTime ToDateTime(long s)
  {
    var d = EpochStart.AddSeconds(s);
    return d;
  }

  /// <summary>
  /// Liefert die Epochen-Sekunden nach dem 1.1.1970 als long.
  /// </summary>
  /// <param name="d">Betroffenes Datum.</param>
  /// <returns>Epochen-Sekunden nach dem 1.1.1970 als DateTime.</returns>
  public static long ToEpochSecond(DateTime d)
  {
    var diff = d - EpochStart;
    return (long)diff.TotalSeconds;
  }

  /// <summary>
  /// Liefert String im Format d.M.yyyy als DateTime.
  /// </summary>
  /// <param name="s">Betroffener String</param>
  /// <returns>String im Format d.M.yyyy als DateTime.</returns>
  public static DateTime? ToDateTimeDe(string s)
  {
    if (string.IsNullOrWhiteSpace(s)
        || !DateTime.TryParseExact(s, "d.MM.yyyy", null, DateTimeStyles.None, out var d))
      return null;
    return d;
  }

  /// <summary>
  /// Liefert letzten Werktag (nicht Samstag und Sonntag), der am Datum oder davor oder danach liegt.
  /// </summary>
  /// <param name="d">Betroffenes Datum.</param>
  /// <param name="danach">Soll der nächste Werktag danach oder davor gesucht werden.</param>
  /// <returns>Werktag als Datum.</returns>
  public static DateTime Workday(DateTime d, bool danach = false)
  {
    var d0 = d;
    while (d0.DayOfWeek == DayOfWeek.Saturday || d0.DayOfWeek == DayOfWeek.Sunday)
      d0 = d0.AddDays(danach ? 1 : -1);
    return d0;
  }

  /**
   * Liefert nächsten Sonntag, der am Datum oder danach liegt.
   * @param d Betroffenes Datum.
   * @return Datums-String.
   */
  public static DateTime Sunday(DateTime? d = null)
  {
    var d0 = d;
    if (!d0.HasValue)
      d0 = DateTime.Today;
    while (d0.Value.DayOfWeek != DayOfWeek.Sunday)
    {
      d0 = d0.Value.AddDays(1);
    }
    return d0.Value;
  }

  /// <summary>Liefert einen möglichst kurzen String mit Zeitraum.</summary>
  /// <returns>Zeitraum als String.</returns>
  /// <param name="von">Betroffenes Anfangsdatum</param>
  /// <param name="bis">Betroffenes Enddatum.</param>
  /// <param name="monate">Soll die Anzahl der Monate in Klammern angefügt werden?</param>
  public static string GetPeriod(DateTime? von, DateTime? bis, bool monate = false)
  {
    var sb = new StringBuilder();
    if (von.HasValue && bis.HasValue)
    {
      if (von.Value.Day != 1 || bis.Value.Day != DateTime.DaysInMonth(bis.Value.Year, bis.Value.Month))
      {
        if (von.Value.Year == bis.Value.Year)
        {
          sb.Append(von.Value.ToString("yyyy-MM-dd-"));
          sb.Append(bis.Value.ToString("MM-dd"));
        }
        else
        {
          sb.Append(von.Value.ToString("yyyy-MM-dd-"));
          sb.Append(bis.Value.ToString("yyyy-MM-dd"));
        }
      }
      else if (von.Value.Year == bis.Value.Year)
      {
        if (von.Value.Month == bis.Value.Month)
        {
          sb.Append(von.Value.ToString("MMMM yyyy"));
        }
        else if (von.Value.Month == 1 && bis.Value.Month == 12)
        {
          sb.Append(von.Value.ToString("yyyy"));
        }
        else
        {
          sb.Append(von.Value.ToString("MMMM-"));
          sb.Append(bis.Value.ToString("MMMM yyyy"));
        }
      }
      else
      {
        sb.Append(von.Value.ToString("MMMM yyyy-"));
        sb.Append(bis.Value.ToString("MMMM yyyy"));
      }
      if (monate)
      {
        var mon = MonthDifference(von, bis);
        sb.Append(" (").Append(mon).Append(' ');
        sb.Append(M0(mon == 1 ? M2096 : M2097)).Append(')');
      }
    }
    else if (von.HasValue)
    {
      sb.Append(von.Value.ToString("yyyy-MM-dd"));
    }
    return sb.ToString();
  }

  /// <summary>Liefert die Anzahl der Monate zwischen zwei Datumswerten.</summary>
  /// <returns>Anzahl der Monate zwischen zwei Datumswerten.</returns>
  /// <param name="von">1. Datum.</param>
  /// <param name="bis">2. Datum.</param>
  public static int MonthDifference(DateTime? von, DateTime? bis)
  {
    if (von.HasValue != bis.HasValue)
    {
      return -1;
    }
    if (!von.HasValue || von == bis)
    {
      return 0;
    }
    var m = 12 * von.Value.Year + von.Value.Month;
    var b = bis.Value.AddDays(1);
    m -= 12 * b.Year + b.Month;
    if (m < 0)
    {
      m = -m;
    }
    return m;
  }

  public static int NextRandom(int minValue, int maxExclusiveValue)
  {
    if (minValue >= maxExclusiveValue)
      throw new ArgumentOutOfRangeException(nameof(minValue)); // "minValue must be lower than maxExclusiveValue");

    long diff = (long)maxExclusiveValue - minValue;
    long upperBound = uint.MaxValue / diff * diff;

    uint ui;
    do
    {
      ui = GetRandomUInt();
    } while (ui >= upperBound);
    return (int)(minValue + (ui % diff));
  }

  static uint GetRandomUInt()
  {
    var randomBytes = GenerateRandomBytes(sizeof(uint));
    return BitConverter.ToUInt32(randomBytes, 0);
  }

  static byte[] GenerateRandomBytes(int bytesNumber)
  {
    byte[] buffer = new byte[bytesNumber];
    csp.GetBytes(buffer);
    return buffer;
  }

  public static byte[] Serialize<T>(T obj)
  {
    using var memStream = new MemoryStream();
    var binSerializer = new XmlSerializer(typeof(T));
    binSerializer.Serialize(memStream, obj);
    return memStream.ToArray();
  }

  public static T Deserialize<T>(byte[] serializedObj)
  {
    T obj = default;
    using (var memStream = new MemoryStream(serializedObj))
    {
      // var binSerializer = new BinaryFormatter();
      var binSerializer = new XmlSerializer(typeof(T));
      obj = (T)binSerializer.Deserialize(memStream);
    }
    return obj;
  }

  /// <summary>
  /// Cuts a string if it starts with an other string.
  /// </summary>
  /// <returns>The string.</returns>
  /// <param name="s">String to cut.</param>
  /// <param name="begin">String to compare at the beginning.</param>
  public static string CutStart(string s, string begin)
  {
    if (!string.IsNullOrEmpty(s) && !string.IsNullOrEmpty(begin)
            && s.StartsWith(begin, StringComparison.CurrentCultureIgnoreCase))
      return s[begin.Length..];
    return s;
  }

  /// <summary>
  /// Cuts a string if it is to long.
  /// </summary>
  /// <returns>The possibly cut string.</returns>
  /// <param name="s">String to cut.</param>
  /// <param name="length">Maximum length.</param>
  public static string Cut(string s, int length)
  {
    if (!string.IsNullOrEmpty(s) && s.Length > length)
      return s[..length];
    return s;
  }

  /** Anfangszustand. */
  const int Z_ANFANG = 0;
  /** Zeichenkette mit ". */
  const int Z_ZK_ANFANG = 10;
  /** Zeichenketten-Ende oder erstes doppeltes ". */
  const int Z_ZK_ENDE = 20;
  /** Zeichenketten ohne ". */
  const int Z_ZEICHENKETTE = 50;
  /** Zeilenende-Anfang. */
  const int Z_ENDE_ANFANG = 90;
  /** Zeilenende-Ende. */
  const int Z_ENDE_ENDE = 95;

  /// <summary>Liefert das i-te Zeichen eines Strings oder 0, wenn String keine i Zeichen hat.</summary>
  /// <param name="str">String darf nicht null sein.</param>
  /// <param name="i">Nummer des Zeichens beginnend mit 0.</param>
  /// <returns>Liefert das i-te Zeichen eines Strings oder 0, wenn String keine i Zeichen hat.</returns>
  private static char GetChar(string str, int i)
  {
    if (str != null && i < str.Length)
    {
      return str[i];
    }
    return (char)0;
  }

  /// <summary>Kodierung eines Vektor von Strings in einen String, die eine Zeile von komma-separierten Feldern mit Zeilenende.</summary>
  /// <param name="felder">Liste von Strings.</param>
  /// <returns>Zeile von komma-separierten Feldern mit Zeilenende.</returns>
  public static string EncodeCSV(List<string> felder)
  {
    if (felder != null && felder.Count > 0)
    {
      var csv = new StringBuilder();
      foreach (var f in felder)
      {
        if (csv.Length > 0)
        {
          csv.Append(';');
        }
        csv.Append('"');
        if (f == null)
        {
          csv.Append("null");
        }
        else
        {
          csv.Append(f.Replace("\"", "\"\""));
        }
        csv.Append('"');
      }
      // csv.Append(Constants.CRLF);
      return csv.ToString();
    }
    return null;
  }

  /// <summary>Dekodierung einer CSV-Datei-Zeile als String in einen Vektor von Strings.</summary>
  /// <param name="csv">CSV-Datei-Zeile als String.</param>
  /// <param name="trenner1">1. Feldtrenner, z.B. ;.</param>
  /// <param name="trenner2">2. Feldtrenner, z.B. ,.</param>
  /// <returns>Vektor von Strings.</returns>
  public static List<string> DecodeCSV(string csv, char trenner1 = ';', char trenner2 = ',')
  {
    if (string.IsNullOrEmpty(csv))
    {
      return null;
    }

    var felder = new List<string>();
    var zustand = Z_ANFANG;
    var i = 0;
    char zeichen;
    char anf = '"';
    char cr = '\r';
    char lf = '\n';
    var ende = false;
    var feld = new StringBuilder();
    do
    {
      zeichen = GetChar(csv, i);
      switch (zustand)
      {
        case Z_ANFANG: // Anfangszustand
          if (zeichen == 0)
          {
            i--;
            zustand = Z_ENDE_ENDE; // Zeilenende-Ende
          }
          else if (zeichen == anf)
          {
            zustand = Z_ZK_ANFANG;
          }
          else if (zeichen == trenner1 || zeichen == trenner2)
          {
            felder.Add(feld.ToString());
            feld.Length = 0;
          }
          else if (zeichen == cr || zeichen == lf)
          {
            zustand = Z_ENDE_ANFANG; // Zeilenende-Anfang
          }
          else
          {
            zustand = Z_ZEICHENKETTE; // normale Zeichenkette ohne "
            i--;
          }
          break;
        case Z_ZK_ANFANG: // Zeichenkette mit "
          if (zeichen == 0)
          {
            // Zeichenkette nicht zu Ende: Parse-Error
            // i--
            zustand = Z_ENDE_ENDE; // Zeilenende-Ende
          }
          else if (zeichen == anf)
          {
            zustand = Z_ZK_ENDE;
          }
          else
          {
            feld.Append(zeichen);
          }
          break;
        case Z_ZK_ENDE: // Zeichenketten-Ende oder erstes doppeltes "
          if (zeichen == 0)
          {
            i--;
            zustand = Z_ENDE_ENDE; // Zeilenende-Ende
          }
          else if (zeichen == anf)
          {
            feld.Append(zeichen);
            zustand = Z_ZK_ANFANG;
          }
          else if (zeichen == trenner1 || zeichen == trenner2)
          {
            zustand = Z_ANFANG;
            felder.Add(feld.ToString());
            feld.Clear();
          }
          else if (zeichen == cr || zeichen == lf)
          {
            i--;
            zustand = Z_ENDE_ANFANG; // Zeilenende-Anfang
          }
          else
          {
            throw new MessageException(M1019(i, csv));
          }
          break;
        case Z_ZEICHENKETTE: // Zeichenketten ohne "
          if (zeichen == 0)
          {
            i--;
            zustand = Z_ENDE_ENDE; // Zeilenende-Ende
          }
          else if (zeichen == trenner1 || zeichen == trenner2)
          {
            zustand = Z_ANFANG;
            felder.Add(feld.ToString());
            feld.Clear();
          }
          else
          {
            feld.Append(zeichen);
          }
          break;
        case Z_ENDE_ANFANG: // Zeilenende-Anfang
          if (!(zeichen == cr || zeichen == lf))
          {
            i--;
            zustand = Z_ENDE_ENDE;
          }
          break;
        case Z_ENDE_ENDE: // Zeilenende-Ende
          ende = true;
          felder.Add(feld.ToString());
          feld.Clear();
          break;
          // default:
          // machNichts
      }
      if (!ende)
      {
        i++;
        if (i > csv.Length)
        {
          throw new MessageException(M1020(csv));
        }
      }
    } while (!ende);
    return felder;
  }

  public static decimal? Round(decimal? d)
  {
    if (d.HasValue)
    {
      return Math.Round(d.Value, 2, MidpointRounding.AwayFromZero);
    }
    return null;
  }

  public static decimal? Round4(decimal? d)
  {
    if (d.HasValue)
    {
      return Math.Round(d.Value, 4, MidpointRounding.AwayFromZero);
    }
    return null;
  }

  /// <summary>Vergleich zweier Werte auf 2 Nachkommastellen liefert
  /// -1, falls d1 &lt; d2
  /// 0, falls d1 = d2 und
  /// 1, falls d1 &gt; d2.</summary>
  /// <param name="d1">Erster Wert.</param>
  /// <param name="d2">Zweiter Wert.</param>
  /// <returns>-1, 0 oder 1.</returns>
  public static int CompDouble(decimal? d1, decimal? d2)
  {
    if (!d1.HasValue)
      return d2.HasValue ? -1 : 0;
    if (!d2.HasValue)
      return 1;
    if (d1 < d2 - 0.005m)
      return -1;
    if (d1 > d2 + 0.005m)
      return 1;
    return 0;
  }

  /// <summary>Vergleich zweier Werte auf 4 Nachkommastellen liefert
  /// -1, falls d1 &lt; d2
  /// 0, falls d1 = d2 und
  /// 1, falls d1 &gt; d2.</summary>
  /// <param name="d1">Erster Wert.</param>
  /// <param name="d2">Zweiter Wert.</param>
  /// <returns>-1, 0 oder 1.</returns>
  public static int CompDouble4(decimal? d1, decimal? d2)
  {
    if (!d1.HasValue)
      return d2.HasValue ? -1 : 0;
    if (!d2.HasValue)
      return 1;
    if (d1 < d2 - 0.00005m)
      return -1;
    if (d1 > d2 + 0.00005m)
      return 1;
    return 0;
  }

  /**
   * Umrechnung eines Euro-Betrages in DM.
   * @param euro Betrag in Euro.
   * @return Konvertierter Betrag in DM.
   */
  public static decimal KonvDM(decimal euro)
  {
    return Round(euro * Constants.EUROFAKTOR) ?? 0;
  }

  /**
   * Umrechnung eines DM-Betrages in Euro.
   * @param dm Betrag in DM.
   * @return Konvertierter Betrag in Euro.
   */
  public static decimal KonvEURO(decimal dm)
  {
    return Round(dm / Constants.EUROFAKTOR) ?? 0;
  }

  /**
   * Zusammensetzen eines Ahnennamens aus Ahnen-Nummer, Geburtsnamen und Vornamen.
   * @param uid Ahnen-Nummer.
   * @param strG Geburtsname.
   * @param strV Vorname.
   * @param nameFett Soll der Name fett geschrieben werden?
   * @param xref Soll der Name fett geschrieben werden?
   * @return Zusammengesetzter Ahnenname.
   */
  public static string AhnString(string uid, string strG, string strV, bool nameFett = false, bool xref = false)
  {
    if (string.IsNullOrEmpty(uid))
      return "";
    var sb = new StringBuilder();
    if (nameFett)
      sb.Append("<b>");
    if (!string.IsNullOrEmpty(strG))
    {
      sb.Append(strG);
      if (!string.IsNullOrEmpty(strV))
        sb.Append(", ");
    }
    if (!string.IsNullOrEmpty(strV))
      sb.Append(strV);
    if (nameFett)
      sb.Append(" </b>");
    sb.Append(" (");
    sb.Append(xref ? ToXref(uid) : uid);
    sb.Append(')');
    return sb.ToString();
  }

  /**
   * Liefert XREF aus Uid. Dabei wird Doppelpunkt durch Semikolon ersetzt.
   * @param uid Uid aus Programm.
   * @return xref.
   */
  public static string ToXref(string uid)
  {
    if (string.IsNullOrEmpty(uid))
      return null;
    return uid.Replace(':', ';');
  }

  /**
   * Liefert Uid aus XREF. Dabei wird Semikolon durch Doppelpunkt ersetzt.
   * @param xref XREF aus Gedcom-Schnittstelle.
   * @return Uid.
   */
  public static string ToUid(string xref)
  {
    if (string.IsNullOrEmpty(xref))
      return null;
    return xref.Replace(';', ':');
  }

  /// <summary>String in Zeilen spalten.</summary>
  /// <param name="s">Betroffener String.</param>
  /// <param name="split">Soll gespalten werden.</param>
  /// <returns>Liste von Zeilen.</returns>
  public static List<string> SplitLines(string s, bool split = true)
  {
    if (string.IsNullOrEmpty(s))
      return new List<string>();
    if (split)
      return Regex.Split(s, "\r\n|\r|\n").ToList();
    return new List<string> { s };
  }

  /**
   * Vergleicht zwei int-Werte mit Hilfe eines Operators.
   * @param variable Linker int-Wert bei Vergleich.
   * @param op String mit <, <=, =, > oder >=; null entspricht =.
   * @param wert Rechter int-Wert bei Vergleich.
   * @return True, wenn Vergleich stimmt; sonst false.
   */
  public static bool VergleicheInt(int variable, string op, int wert)
  {
    var rc = false;
    if (op == null || op == "")
      rc = true;
    else if (op == "=")
      rc = variable == wert;
    else if (op == "<=")
      rc = variable <= wert;
    else if (op == "<")
      rc = variable < wert;
    else if (op == ">=")
      rc = variable >= wert;
    else if (op == ">")
      rc = variable > wert;
    return rc;
  }

  //private static Regex RxCoordinates = new Regex(@"^(-?\d+(\.\d+)?),\s*(-?\d+(\.\d+)?)(,\s*(-?\d+(\.\d+)?)z?)?$", RegexOptions.Compiled);
  private static readonly Regex RxCoordinates = new(@"^(-?\d+(\.\d+)),\s*(-?\d+(\.\d+))(,\s*(-?\d+(\.\d+))z?)?$", RegexOptions.Compiled);

  /// <summary>String in Koordinaten in Dezimalform.</summary>
  /// <param name="s">Betroffener String.</param>
  /// <returns>Koordinaten in Dezimalform.</returns>
  public static Tuple<decimal, decimal, decimal> ToCoordinates(string s)
  {
    if (string.IsNullOrEmpty(s))
      return null;
    var m = RxCoordinates.Match(s);
    if (m.Success)
    {
      return new Tuple<decimal, decimal, decimal>(ToDecimal(m.Groups[1].Value, -1, true) ?? 0,
        ToDecimal(m.Groups[3].Value, -1, true) ?? 0, ToDecimal(m.Groups[6].Value, -1, true) ?? 0);
    }
    return null;
  }

  /// <summary>
  /// Is there a <b> tag around the string?
  /// </summary>
  /// <param name="s">Affected string</param>
  /// <returns>Is there a <b> tag around the string?</returns>
  public static bool IsBold(string s)
  {
    return s != null && s.StartsWith("<b>") && s.EndsWith("</b>");
  }

  /// <summary>
  /// Put <b> tag around the string, if there is none. Or remove the <b> tag.
  /// </summary>
  /// <param name="s">Affected string.</param>
  /// <param name="unbold">Remove the <b> tag?</param>
  /// <returns>String with or  <b> around.</returns>
  public static string MakeBold(string s, bool unbold = false)
  {
    if (s == null)
      s = "";
    if (unbold)
    {
      if (IsBold(s))
        return s[3..^4];
      return s;
    }
    if (IsBold(s))
      return s;
    return $"<b>{s}</b>";
  }

  /// <summary>
  /// Läuft das Programm unter Linux?
  /// </summary>
  /// <returns>Läuft das Programm unter Linux?</returns>
  public static bool IsLinux()
  {
    var linux = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux);
    return linux;
  }

  /// <summary>
  /// Alle Windows-1252-Zeichen.
  /// </summary>
  private static readonly Regex RxWindow1252 = new(@"^(
[\u0000-\u007F]|
\u20AC| # 80 Euro
\u201A| # 82
\u0192| # 83
\u201E| # 84
\u2026| # 85
\u2020| # 86
\u2021| # 87
\u02C6| # 88
\u2030| # 89
\u0160| # 8A
\u2039| # 8B
\u0152| # 8C
\u017D| # 8E
\u2018| # 91
\u2019| # 92
\u201C| # 93
\u201D| # 94
\u2022| # 95
\u2013| # 96
\u2014| # 97
\u02DC| # 98
\u2122| # 99
\u0161| # 9A
\u203A| # 9B
\u0153| # 9C
\u017E| # 9E
\u0178| # 9F
[\u00A0-\u00FF]|
)*$", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

  /// <summary>
  /// Alle erlaubten Windows-1252-Zeichen für IRM@-Strings, ohne Steuerzeichen, mit CR, LF und Tab.
  /// </summary>
  private static readonly Regex RxWindow1252CrLfTab = new(@"^(
\u0009| # Tab
\u000A| # LF
\u000D| # CR
[\u0020-\u007F]|
\u20AC| # 80 Euro
\u201A| # 82
\u0192| # 83
\u201E| # 84
\u2026| # 85
\u2020| # 86
\u2021| # 87
\u02C6| # 88
\u2030| # 89
\u0160| # 8A
\u2039| # 8B
\u0152| # 8C
\u017D| # 8E
\u2018| # 91
\u2019| # 92
\u201C| # 93
\u201D| # 94
\u2022| # 95
\u2013| # 96
\u2014| # 97
\u02DC| # 98
\u2122| # 99
\u0161| # 9A
\u203A| # 9B
\u0153| # 9C
\u017E| # 9E
\u0178| # 9F
[\u00A0-\u00FF]|
)*$", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

  /// <summary>
  /// Alle erlaubten Windows-1252-Zeichen für IRM@-Strings ohne Steuerzeichen.
  /// </summary>
  private static readonly Regex RxWindow1252Ohne = new(@"^(
[\u0020-\u007F]|
\u20AC| # 80 Euro
\u201A| # 82
\u0192| # 83
\u201E| # 84
\u2026| # 85
\u2020| # 86
\u2021| # 87
\u02C6| # 88
\u2030| # 89
\u0160| # 8A
\u2039| # 8B
\u0152| # 8C
\u017D| # 8E
\u2018| # 91
\u2019| # 92
\u201C| # 93
\u201D| # 94
\u2022| # 95
\u2013| # 96
\u2014| # 97
\u02DC| # 98
\u2122| # 99
\u0161| # 9A
\u203A| # 9B
\u0153| # 9C
\u017E| # 9E
\u0178| # 9F
[\u00A0-\u00FF]|
)*$", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

  /// <summary>
  /// Does the string only contain characters of codepage Windows-1252?
  /// </summary>
  /// <param name="value">Affected string.</param>
  /// <param name="crlftab">Is carriage return, line feed or tab allowed?</param>
  /// <param name="all">Are all characters incl. control characters (0-31) allowed?</param>
  /// <returns>Only characters of codepage Windows-1252?</returns>
  public static bool IsWindows1252(string value, bool crlftab = false, bool all = false)
  {
    if (string.IsNullOrEmpty(value))
      return true;
    if (all)
    {
      var m = RxWindow1252.Match(value);
      return m.Success;
    }
    if (crlftab)
    {
      var m = RxWindow1252CrLfTab.Match(value);
      return m.Success;
    }
    if (value.Contains('\n'))
      return false; // \n is problematic in Regex.
    var match = RxWindow1252Ohne.Match(value);
    return match.Success;
  }

  /// <summary>
  /// Filter all allowed characters of codepage Windows-1252, other characters are replaced.
  /// </summary>
  /// <param name="value">Affected string.</param>
  /// <param name="crlftab">Is carriage return, line feed or tab allowed?</param>
  /// <param name="all">Are all characters incl. control characters (0-31) allowed?</param>
  /// <param name="replace">Replacement string for not allowed characters.</param>
  /// <returns>String containing only allowed characters of codepage Windows-1252?</returns>
  public static string FilterWindows1252(string value, bool crlftab = false, bool alle = false, string replace = " ")
  {
    if (value == null)
      return null;
    if (string.IsNullOrEmpty(value))
      return "";
    if (IsWindows1252(value, crlftab, alle))
      return value;
    replace ??= " ";
    var sb = new StringBuilder();
    foreach (char c in value)
    {
      var s = char.ToString(c);
      //// ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
      if (IsWindows1252(s, crlftab, alle))
        sb.Append(s);
      else
        sb.Append(replace);
    }
    return sb.ToString();
  }
}
