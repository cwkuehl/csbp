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

/// <summary>
/// General useful functions.
/// </summary>
public static partial class Functions
{
#pragma warning disable SA1310

  /// <summary>Initial parser state.</summary>
  private const int Z_ANFANG = 0;

  /// <summary>String with ".</summary>
  private const int Z_ZK_ANFANG = 10;

  /// <summary>End of string or first of a double ".</summary>
  private const int Z_ZK_ENDE = 20;

  /// <summary>String without ".</summary>
  private const int Z_ZEICHENKETTE = 50;

  /// <summary>Beginning of end of line.</summary>
  private const int Z_ENDE_ANFANG = 90;

  /// <summary>End of end of line.</summary>
  private const int Z_ENDE_ENDE = 95;

#pragma warning restore SA1310

  /// <summary>Epoch start at 1970-01-01.</summary>
  private static readonly DateTime EpochStart = new(1970, 1, 1);

  /// <summary>Instance of random number generator.</summary>
  private static readonly RandomNumberGenerator Csp = RandomNumberGenerator.Create();

  /// <summary>German culture info.</summary>
  private static readonly CultureInfo CultureInfoDeRo = CultureInfo.CreateSpecificCulture("de-DE");

  /// <summary>English culture info.</summary>
  private static readonly CultureInfo CultureInfoEnRo = CultureInfo.CreateSpecificCulture("en-GB");

  /// <summary>Current culture info.</summary>
  private static CultureInfo cultureInfoCuSt = CultureInfo.CreateSpecificCulture("de-DE");

  /// <summary>Gets the current culture info.</summary>
  public static CultureInfo CultureInfoCu => cultureInfoCuSt;

  /// <summary>Gets the German culture info.</summary>
  public static CultureInfo CultureInfoDe => CultureInfoDeRo;

  /// <summary>Gets the English culture info.</summary>
  public static CultureInfo CultureInfoEn => CultureInfoEnRo;

  /// <summary>
  /// Gets a value indicating whether the actual language is German.
  /// </summary>
  /// <value>True if actual language German.</value>
  public static bool IsDe
  {
    get { return CultureInfoCu.TwoLetterISOLanguageName == "de"; }
  }

  /// <summary>
  /// Sets the current culture info.
  /// </summary>
  /// <param name="ci">Affected culture info.</param>
  public static void SetCultureInfo(CultureInfo ci)
  {
    if (ci == null)
      return;
    cultureInfoCuSt = ci;
    Thread.CurrentThread.CurrentCulture = ci;
    Thread.CurrentThread.CurrentUICulture = ci;
    //// Messages.Culture = ci;
  }

  /// <summary>
  /// Function does nothing.
  /// </summary>
  /// <param name="obj">Optional parameter is not used.</param>
  /// <returns>Number 0.</returns>
  public static int MachNichts(object obj = null)
  {
    if (obj == null)
      return 0;
    return 0;
  }

  /// <summary>
  /// Converts string to integer.
  /// </summary>
  /// <returns>Converted value.</returns>
  /// <param name="s">Affected string.</param>
  public static int ToInt32(string s)
  {
    var d = ToDecimal(s, 0);
    if (d.HasValue && d.Value >= int.MinValue && d.Value <= int.MaxValue)
      return (int)d.Value;
    return 0;
  }

  /// <summary>
  /// Converts string to long.
  /// </summary>
  /// <returns>Converted value.</returns>
  /// <param name="s">Affected string.</param>
  public static long ToInt64(string s)
  {
    if (string.IsNullOrWhiteSpace(s) || !long.TryParse(s, out var l))
    {
      return 0;
    }
    return l;
  }

  /// <summary>
  /// Converts string to decimal.
  /// </summary>
  /// <returns>Converted value.</returns>
  /// <param name="s">Affected string.</param>
  /// <param name="digits">Number of digits to round.</param>
  /// <param name="english">Parse with English culture or not.</param>
  public static decimal? ToDecimal(string s, int digits = -1, bool english = false)
  {
    if (!string.IsNullOrWhiteSpace(s) && decimal.TryParse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands,
        english ? CultureInfoEn : CultureInfoCu, out var d))
    {
      if (digits >= 0)
        d = Math.Round(d, digits, MidpointRounding.AwayFromZero);
      return d;
    }
    return null;
  }

  /// <summary>
  /// Converts German string to decimal.
  /// </summary>
  /// <returns>Converted value.</returns>
  /// <param name="s">Affected string.</param>
  public static decimal? ToDecimalDe(string s)
  {
    if (!string.IsNullOrWhiteSpace(s) && decimal.TryParse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands,
        CultureInfoDe, out var d))
      return d;
    return null;
  }

  /// <summary>
  /// Converts current culture string to decimal.
  /// </summary>
  /// <param name="s">Affected string.</param>
  /// <param name="digits">Number of digits to round.</param>
  /// <returns>Converted value.</returns>
  public static decimal? ToDecimalCi(string s, int digits = -1)
  {
    if (!string.IsNullOrWhiteSpace(s) && decimal.TryParse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfoCu, out var d))
    {
      if (digits >= 0)
        d = Math.Round(d, digits, MidpointRounding.AwayFromZero);
      return d;
    }
    return null;
  }

  /// <summary>
  /// Converts string to nullable bool.
  /// </summary>
  /// <param name="s">Affected string.</param>
  /// <returns>Converted value.</returns>
  public static bool? ToBool(string s)
  {
    if (!string.IsNullOrWhiteSpace(s) && bool.TryParse(s, out var d))
      return d;
    return null;
  }

  /// <summary>
  /// Converts first character to upper, the to lower case.
  /// </summary>
  /// <param name="s">Affected string.</param>
  /// <returns>Converted string.</returns>
  public static string ToFirstUpper(this string s)
  {
    if (string.IsNullOrEmpty(s))
      return string.Empty;
    return char.ToUpper(s[0]) + s[1..].ToLower();
  }

  /// <summary>
  /// Optionally trims a string and returns null if it is emptly.
  /// </summary>
  /// <param name="s">Affected string.</param>
  /// <param name="trim">Trim value or not.</param>
  /// <returns>Converted string.</returns>
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
  /// Returns a left substring of a string with max. l characters and never null.
  /// </summary>
  /// <param name="s">Affected string.</param>
  /// <param name="l">Max. string length.</param>
  /// <returns>Left substring.</returns>
  public static string Left(this string s, int l = 1)
  {
    if (string.IsNullOrEmpty(s) || l <= 0)
    {
      return "";
    }
    return s[..Math.Min(l, s.Length)];
  }

  /// <summary>Appends conditionally to a StringBuilder. The string obj2 is always appended.
  /// The filler strings are append before and after the obj2, if both StringBuilder and obj2 are not empty.
  /// <b>Examples</b>:
  /// sb = "Name"; Append(sb, ", ", "Vorname", ""); => sb = "Name, Vorname";
  /// sb = "Name"; Append(sb, ", ", "", ""); => sb = "Name";
  /// sb = "Name"; Append(sb, " (", "Titel", ")"); => sb = "Name (Vorname)";
  /// ...
  /// </summary>
  /// <param name="sb">Affected StringBuilder.</param>
  /// <param name="filler1">First filler.</param>
  /// <param name="obj">String which is always appended.</param>
  /// <param name="filler2">Second filler.</param>
  /// <returns>Same StringBuilder for fluent API.</returns>
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

  /// <summary>
  /// Builds a string from two string objects and a filler. The filler is inserted between only if both string objects are not empty.
  /// </summary>
  /// <param name="obj1">First string object.</param>
  /// <param name="filler">Filler string.</param>
  /// <param name="obj2">Second string object.</param>
  /// <returns>Concatenated string.</returns>
  public static string Append(string obj1, string filler, string obj2)
  {
    var s1 = (obj1 ?? "").TrimEnd();
    var s2 = (obj2 ?? "").TrimEnd();
    if (s1.Length > 0 && s2.Length > 0)
    {
      if (!string.IsNullOrEmpty(filler))
        s1 += filler;
    }
    s1 += s2;
    return s1;
  }

  /// <summary>
  /// Converts a table name into camel case with first letter in upper case.
  /// Underscores are removed, and this pieces also start with a upper case letter.
  /// </summary>
  /// <param name="t">Affected table name.</param>
  /// <returns>Converted table name.</returns>
  public static string TabName(string t)
  {
    var arr = t.Split('_');
    return string.Join(string.Empty, arr.Select(a => a.ToFirstUpper()));
  }

  /// <summary>
  /// Checks if it is a filtering like expression. Empty, % and %% are not.
  /// </summary>
  /// <param name="t">Affected like expression.</param>
  /// <returns>It is a filtering like expression or not.</returns>
  public static bool IsLike(string t)
  {
    return !(string.IsNullOrEmpty(t) || t == "%" || t == "%%");
  }

  /// <summary>
  /// Compares two strings:
  /// 0, if s1 = s2 and
  /// 1, if s1 != s2.
  /// There is not difference between null and empty.
  /// </summary>
  /// <param name="s1">First string.</param>
  /// <param name="s2">Second string.</param>
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
  /// Returns a new guid as string.
  /// </summary>
  /// <returns>New guid as string.</returns>
  public static string GetUid()
  {
    var uid = Guid.NewGuid().ToString();
    return uid;
  }

  /// <summary>
  /// Returns file name optionally with date and random number.
  /// </summary>
  /// <param name="name">Name am Anfang.</param>
  /// <param name="datum">With current date or not.</param>
  /// <param name="zufall">With rondom number or not.</param>
  /// <param name="endung">Dateiendung ohne Punkt.</param>
  /// <returns>File name.</returns>
  public static string GetDateiname(string name, bool datum, bool zufall, string endung)
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

  /// <summary>Returns a never null string.</summary>
  /// <param name="s">Affected string.</param>
  /// <returns>Not null string.</returns>
  public static string ToString(string s)
  {
    if (string.IsNullOrEmpty(s))
      return "";
    return s;
  }

  /// <summary>
  /// Converts integer to string in current culture.
  /// </summary>
  /// <param name="i">Affected value.</param>
  /// <returns>Converted value.</returns>
  public static string ToString(int? i)
  {
    if (!i.HasValue)
      return string.Empty;
    return i.Value.ToString(CultureInfoCu);
  }

  /// <summary>
  /// Converts nullable bool to string.
  /// </summary>
  /// <param name="i">Affected value.</param>
  /// <returns>Converted value.</returns>
  public static string ToString(bool? i)
  {
    if (!i.HasValue)
      return string.Empty;
    return i.Value ? "true" : "false";
  }

  /// <summary>
  /// Converts nullable decimal to string.
  /// </summary>
  /// <param name="d">Affected value.</param>
  /// <param name="digits">Number of digits to print.</param>
  /// <param name="ci">Affected culture info.</param>
  /// <returns>Converted value.</returns>
  public static string ToString(decimal? d, int digits = -1, CultureInfo ci = null)
  {
    if (!d.HasValue)
      return string.Empty;
    return d.Value.ToString(digits < 0 ? "N" : $"N{digits}", ci ?? CultureInfoCu);
  }

  /// <summary>
  /// Converts nullable DateTime to string in format yyyy-MM-dd, yyyy-MM-dd HH:mm:ss or yyyy-MM-dd HH:mm:ss.fffffff.
  /// </summary>
  /// <param name="d">Affected value.</param>
  /// <param name="time">Formats with time or not.</param>
  /// <param name="milli">Formats with milliseconds or not.</param>
  /// <returns>Converted value.</returns>
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
  /// Converts nullable DateTime to string in format yyyy-MM-ddTHH:mm:ss.
  /// </summary>
  /// <param name="d">Affected value.</param>
  /// <param name="nullstring">Null for no value.</param>
  /// <returns>Converted value.</returns>
  public static string ToStringT(DateTime? d, bool nullstring = false)
  {
    if (!d.HasValue)
      return nullstring ? null : string.Empty;
    return d.Value.ToString("yyyy-MM-dd'T'HH:mm:ss");
  }

  /// <summary>
  /// Converts nullable DateTime to string in format dd.MM.yyyy or dd.MM.yyyy HH:mm:ss.
  /// </summary>
  /// <param name="d">Affected value.</param>
  /// <param name="time">Formats with time or not.</param>
  /// <returns>Converted value.</returns>
  public static string ToStringDe(DateTime? d, bool time = false)
  {
    if (!d.HasValue)
      return string.Empty;
    if (time)
      return d.Value.ToString("dd.MM.yyyy HH:mm:ss");
    return d.Value.ToString("dd.MM.yyyy");
  }

  /// <summary>
  /// Returns weekday as string.
  /// </summary>
  /// <param name="d">Affected date.</param>
  /// <returns>Weekday as string.</returns>
  public static string ToStringWd(DateTime? d)
  {
    if (!d.HasValue)
      return string.Empty;
    return d.Value.ToString("dddd");
  }

  /// <summary>
  /// Corrects UTC time to local.
  /// </summary>
  /// <param name="d">Affected DateTime.</param>
  /// <returns>DateTime in local time zone.</returns>
  public static DateTime? ToDateTimeLocal(DateTime? d)
  {
    if (!d.HasValue)
      return null;
    return TimeZoneInfo.ConvertTimeFromUtc(d.Value, TimeZoneInfo.Local);
  }

  /// <summary>
  /// Corrects locale time to UTC.
  /// </summary>
  /// <param name="d">Affected DateTime.</param>
  /// <returns>DateTime in UTC.</returns>
  public static DateTime? ToDateTimeUtc(DateTime? d)
  {
    if (!d.HasValue)
      return null;
    return TimeZoneInfo.ConvertTimeFromUtc(d.Value, TimeZoneInfo.Utc);
  }

  /// <summary>
  /// Parses string to nullable DateTime, with following formats:
  /// yyyy-MM-dd HH:mm:ss, dd.MM.yyyy HH:mm:ss, yyyy-MM-d and d.M.yyyy.
  /// </summary>
  /// <param name="s">Affected string.</param>
  /// <returns>Converted value.</returns>
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

  /// <summary>
  /// Parses string to nullable DateTime with format d.MM.yyyy.
  /// </summary>
  /// <param name="s">Affected string.</param>
  /// <returns>Converted value.</returns>
  public static DateTime? ToDateTimeDe(string s)
  {
    if (string.IsNullOrWhiteSpace(s)
        || !DateTime.TryParseExact(s, "d.MM.yyyy", null, DateTimeStyles.None, out var d))
      return null;
    return d;
  }

  /// <summary>
  /// Calculates DateTime as seconds after 1970-01-01.
  /// </summary>
  /// <param name="s">Betroffene Anzahl Sekunden.</param>
  /// <returns>Calculated DateTime.</returns>
  public static DateTime ToDateTime(long s)
  {
    var d = EpochStart.AddSeconds(s);
    return d;
  }

  /// <summary>
  /// Calculates seconds after 1970-01-01, Unix time.
  /// </summary>
  /// <param name="d">Betroffenes Datum.</param>
  /// <returns>Seconds after 1970-01-01.</returns>
  public static long ToEpochSecond(DateTime d)
  {
    var diff = d - EpochStart;
    return (long)diff.TotalSeconds;
  }

  /// <summary>
  /// Calculares next or previous date while it is not a workday i.e. Monday through Friday.
  /// </summary>
  /// <param name="d">Affected date.</param>
  /// <param name="after">The next or previous day.</param>
  /// <returns>Workday as DateTime.</returns>
  public static DateTime Workday(DateTime d, bool after = false)
  {
    var d0 = d;
    while (d0.DayOfWeek == DayOfWeek.Saturday || d0.DayOfWeek == DayOfWeek.Sunday)
      d0 = d0.AddDays(after ? 1 : -1);
    return d0;
  }

  /// <summary>
  /// Calculates the next Sunday.
  /// </summary>
  /// <param name="d">Affected date.</param>
  /// <returns>Next Sunday.</returns>
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

  /// <summary>Calculate the shortest possible period string between two dates.</summary>
  /// <param name="von">First date.</param>
  /// <param name="bis">Second date.</param>
  /// <param name="months">Should the number of months be added or not.</param>
  /// <returns>Period as string.</returns>
  public static string GetPeriod(DateTime? von, DateTime? bis, bool months = false)
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
      if (months)
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

  /// <summary>Calculates the number of months between two dates.</summary>
  /// <param name="von">First date.</param>
  /// <param name="bis">Second date.</param>
  /// <returns>Number of months between two dates.</returns>
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
    var m = (12 * von.Value.Year) + von.Value.Month;
    var b = bis.Value.AddDays(1);
    m -= (12 * b.Year) + b.Month;
    if (m < 0)
    {
      m = -m;
    }
    return m;
  }

  /// <summary>
  /// Gets the next random number between two values.
  /// </summary>
  /// <param name="minValue">Minimal value.</param>
  /// <param name="maxExclusiveValue">Exclusive maximal value.</param>
  /// <returns>Random number between two values.</returns>
  public static int NextRandom(int minValue, int maxExclusiveValue)
  {
    if (minValue >= maxExclusiveValue)
      throw new ArgumentOutOfRangeException(nameof(minValue)); // "minValue must be lower than maxExclusiveValue");

    var diff = (long)maxExclusiveValue - minValue;
    var upperBound = uint.MaxValue / diff * diff;

    uint ui;
    do
    {
      ui = GetRandomUInt();
    }
    while (ui >= upperBound);
    return (int)(minValue + (ui % diff));
  }

  /// <summary>
  /// Serializes an object to bytes.
  /// </summary>
  /// <param name="obj">Affected object.</param>
  /// <typeparam name="T">Affected object type.</typeparam>
  /// <returns>Serialized bytes.</returns>
  public static byte[] Serialize<T>(T obj)
  {
    using var memStream = new MemoryStream();
    var binSerializer = new XmlSerializer(typeof(T));
    binSerializer.Serialize(memStream, obj);
    return memStream.ToArray();
  }

  /// <summary>
  /// Deserializes bytes to an object.
  /// </summary>
  /// <param name="serialized">Serialized bytes.</param>
  /// <typeparam name="T">Affected type.</typeparam>
  /// <returns>Deserialized object.</returns>
  public static T Deserialize<T>(byte[] serialized)
  {
    T obj = default;
    using (var memStream = new MemoryStream(serialized))
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
  /// <param name="s">String to cut.</param>
  /// <param name="begin">String to compare at the beginning.</param>
  /// <returns>Possibly cut string.</returns>
  public static string CutStart(string s, string begin)
  {
    if (!string.IsNullOrEmpty(s) && !string.IsNullOrEmpty(begin)
            && s.StartsWith(begin, StringComparison.CurrentCultureIgnoreCase))
      return s[begin.Length..];
    return s;
  }

  /// <summary>
  /// Cuts a string if it is too long.
  /// </summary>
  /// <param name="s">String to cut.</param>
  /// <param name="length">Maximum length.</param>
  /// <returns>possibly cut string.</returns>
  public static string Cut(string s, int length)
  {
    if (!string.IsNullOrEmpty(s) && s.Length > length)
      return s[..length];
    return s;
  }

  /// <summary>Encodes a list of string to an csv string.</summary>
  /// <param name="felder">List of strings.</param>
  /// <returns>Encodes csv string.</returns>
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
      return csv.ToString();
    }
    return null;
  }

  /// <summary>Decodes a csv string to a list of strings.</summary>
  /// <param name="csv">Affected csv string.</param>
  /// <param name="trenner1">First field delimiter e.g. ;.</param>
  /// <param name="trenner2">Second field delimiter e.g. ,.</param>
  /// <returns>List of strings.</returns>
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
    var anf = '"';
    var cr = '\r';
    var lf = '\n';
    var ende = false;
    var feld = new StringBuilder();
    do
    {
      zeichen = GetChar(csv, i);
      switch (zustand)
      {
        case Z_ANFANG:
          //// Initial state
          if (zeichen == 0)
          {
            i--;
            zustand = Z_ENDE_ENDE; // End of end of line
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
            zustand = Z_ENDE_ANFANG; // Beginning of end of line
          }
          else
          {
            zustand = Z_ZEICHENKETTE; // normal string without "
            i--;
          }
          break;
        case Z_ZK_ANFANG:
          // String with "
          if (zeichen == 0)
          {
            // Zeichenkette nicht zu Ende: Parse-Error
            // i--
            zustand = Z_ENDE_ENDE; // End of end of line
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
          //// default:
      }
      if (!ende)
      {
        i++;
        if (i > csv.Length)
        {
          throw new MessageException(M1020(csv));
        }
      }
    }
    while (!ende);
    return felder;
  }

  /// <summary>
  /// Rounds a decimal number by 2 digits.
  /// </summary>
  /// <param name="d">Affected decimal.</param>
  /// <returns>Rounded decimal.</returns>
  public static decimal? Round(decimal? d)
  {
    if (d.HasValue)
    {
      return Math.Round(d.Value, 2, MidpointRounding.AwayFromZero);
    }
    return null;
  }

  /// <summary>
  /// Rounds a decimal number by 4 digits.
  /// </summary>
  /// <param name="d">Affected decimal.</param>
  /// <returns>Rounded decimal.</returns>
  public static decimal? Round4(decimal? d)
  {
    if (d.HasValue)
    {
      return Math.Round(d.Value, 4, MidpointRounding.AwayFromZero);
    }
    return null;
  }

  /// <summary>Compares two decimals by 2 digits: -1 if d1 &lt; d2; 0 if d1 = d2 and 1 if d1 &gt; d2.</summary>
  /// <param name="d1">First decimal.</param>
  /// <param name="d2">Second decimal.</param>
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

  /// <summary>Compares two decimals by 4 digits: -1 if d1 &lt; d2; 0 if d1 = d2 and 1 if d1 &gt; d2.</summary>
  /// <param name="d1">First decimal.</param>
  /// <param name="d2">Second decimal.</param>
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

  /// <summary>
  /// Calculates Euro price to DM.
  /// </summary>
  /// <param name="euro">Euro price.</param>
  /// <returns>DM price.</returns>
  public static decimal KonvDM(decimal euro)
  {
    return Round(euro * Constants.EUROFAKTOR) ?? 0;
  }

  /// <summary>
  /// Calculates DM price to Euro.
  /// </summary>
  /// <param name="dm">DM price.</param>
  /// <returns>Euro price.</returns>
  public static decimal KonvEURO(decimal dm)
  {
    return Round(dm / Constants.EUROFAKTOR) ?? 0;
  }

  /// <summary>
  /// Concatenates an ancestor string.
  /// </summary>
  /// <param name="uid">Affected ancestor id.</param>
  /// <param name="bn">Affected birth name.</param>
  /// <param name="fn">Affected first name.</param>
  /// <param name="boldname">Should the name be bold or not.</param>
  /// <param name="xref">Affected cross reference.</param>
  /// <returns>Ancestor string.</returns>
  public static string AhnString(string uid, string bn, string fn, bool boldname = false, bool xref = false)
  {
    if (string.IsNullOrEmpty(uid))
      return "";
    var sb = new StringBuilder();
    if (boldname)
      sb.Append("<b>");
    if (!string.IsNullOrEmpty(bn))
    {
      sb.Append(bn);
      if (!string.IsNullOrEmpty(fn))
        sb.Append(", ");
    }
    if (!string.IsNullOrEmpty(fn))
      sb.Append(fn);
    if (boldname)
      sb.Append(" </b>");
    sb.Append(" (");
    sb.Append(xref ? ToXref(uid) : uid);
    sb.Append(')');
    return sb.ToString();
  }

  /// <summary>
  /// Converts uid to cross reference.
  /// </summary>
  /// <param name="uid">Affected uid.</param>
  /// <returns>GEDCOM cross reference.</returns>
  public static string ToXref(string uid)
  {
    if (string.IsNullOrEmpty(uid))
      return null;
    return uid.Replace(':', ';');
  }

  /// <summary>
  /// Converts cross reference to uid.
  /// </summary>
  /// <param name="xref">Affected cross reference.</param>
  /// <returns>Affected uid.</returns>
  public static string ToUid(string xref)
  {
    if (string.IsNullOrEmpty(xref))
      return null;
    return xref.Replace(';', ':');
  }

  /// <summary>Splits string into lines.</summary>
  /// <param name="s">Affected String.</param>
  /// <param name="split">Should be splitted or not.</param>
  /// <returns>List of lines.</returns>
  public static List<string> SplitLines(string s, bool split = true)
  {
    if (string.IsNullOrEmpty(s))
      return new List<string>();
    if (split)
      return SplitLinesRegex().Split(s).ToList();
    return new List<string> { s };
  }

  /// <summary>
  /// Compares two integers by operator &lt; &lt;= = &gt;= or &gt;.
  /// </summary>
  /// <param name="i1">First integer.</param>
  /// <param name="op">Compare operator &lt; &lt;= = &gt;= or &gt;.</param>
  /// <param name="i2">Second inteter.</param>
  /// <returns>Comparison is true or not.</returns>
  public static bool VergleicheInt(int i1, string op, int i2)
  {
    var rc = false;
    if (op == null || op == "")
      rc = true;
    else if (op == "=")
      rc = i1 == i2;
    else if (op == "<=")
      rc = i1 <= i2;
    else if (op == "<")
      rc = i1 < i2;
    else if (op == ">=")
      rc = i1 >= i2;
    else if (op == ">")
      rc = i1 > i2;
    return rc;
  }

  /// <summary>Converts coordinate string to decimal tuple.</summary>
  /// <param name="s">Affected string.</param>
  /// <returns>Coordinates in decimal tuple.</returns>
  public static Tuple<decimal, decimal, decimal> ToCoordinates(string s)
  {
    if (string.IsNullOrEmpty(s))
      return null;
    var m = CoordinatesRegex().Match(s);
    if (m.Success)
    {
      return new Tuple<decimal, decimal, decimal>(ToDecimal(m.Groups[1].Value, -1, true) ?? 0,
        ToDecimal(m.Groups[3].Value, -1, true) ?? 0, ToDecimal(m.Groups[6].Value, -1, true) ?? 0);
    }
    return null;
  }

  /// <summary>
  /// Checks if there is a &lt; b &gt; tag around the string or not.
  /// </summary>
  /// <param name="s">Affected string.</param>
  /// <returns>Is there a &lt; b &gt; tag around the string or not.</returns>
  public static bool IsBold(string s)
  {
    return s != null && s.StartsWith("<b>") && s.EndsWith("</b>");
  }

  /// <summary>
  /// Puts  &lt; b &gt; tag around the string, if there is none. Or remove the &lt; b &gt; tag.
  /// </summary>
  /// <param name="s">Affected string.</param>
  /// <param name="unbold">Remove the  &lt; b &gt; tag or not.</param>
  /// <returns>String with or &lt; b &gt; tag around.</returns>
  public static string MakeBold(string s, bool unbold = false)
  {
    s ??= "";
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
  /// Checks if the program runs with Linux or not.
  /// </summary>
  /// <returns>Does the program run with Linux or not.</returns>
  public static bool IsLinux()
  {
    var linux = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux);
    return linux;
  }

  /// <summary>
  /// Does the string only contain characters of codepage Windows-1252 or not.
  /// </summary>
  /// <param name="value">Affected string.</param>
  /// <param name="crlftab">Is carriage return, line feed or tab allowed or not.</param>
  /// <param name="all">Are all characters incl. control characters (0-31) allowed or not.</param>
  /// <returns>Only characters of codepage Windows-1252 or not.</returns>
  public static bool IsWindows1252(string value, bool crlftab = false, bool all = false)
  {
    if (string.IsNullOrEmpty(value))
      return true;
    if (all)
    {
      var m = Windows1252Regex().Match(value);
      return m.Success;
    }
    if (crlftab)
    {
      var m = Windows1252CrLfTabRegex().Match(value);
      return m.Success;
    }
    if (value.Contains('\n'))
      return false; // \n is problematic in Regex.
    var match = Windows1252OhneRegex().Match(value);
    return match.Success;
  }

  /// <summary>
  /// Filter all allowed characters of codepage Windows-1252, other characters are replaced.
  /// </summary>
  /// <param name="value">Affected string.</param>
  /// <param name="crlftab">Is carriage return, line feed or tab allowed or not.</param>
  /// <param name="all">Are all characters incl. control characters (0-31) allowed or not.</param>
  /// <param name="replace">Replacement string for not allowed characters.</param>
  /// <returns>String containing only allowed characters of codepage Windows-1252 or not.</returns>
  public static string FilterWindows1252(string value, bool crlftab = false, bool all = false, string replace = " ")
  {
    if (value == null)
      return null;
    if (string.IsNullOrEmpty(value))
      return "";
    if (IsWindows1252(value, crlftab, all))
      return value;
    replace ??= " ";
    var sb = new StringBuilder();
    foreach (var c in value)
    {
      var s = char.ToString(c);
      //// ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
      if (IsWindows1252(s, crlftab, all))
        sb.Append(s);
      else
        sb.Append(replace);
    }
    return sb.ToString();
  }

  /// <summary>Regex for splitting lines.</summary>
  [GeneratedRegex("\r\n|\r|\n")]
  public static partial Regex SplitLinesRegex();

  /// <summary>Regex for file uri.</summary>
  [GeneratedRegex("^(file:\\/*)(.+?)$")]
  public static partial Regex FileRegex();

  /// <summary>
  /// Gets a random integer.
  /// </summary>
  /// <returns>Random integer.</returns>
  private static uint GetRandomUInt()
  {
    var randomBytes = GenerateRandomBytes(sizeof(uint));
    return BitConverter.ToUInt32(randomBytes, 0);
  }

  /// <summary>
  /// Gets random bytes.
  /// </summary>
  /// <param name="bytesNumber">Number of bytes.</param>
  /// <returns>Random bytes.</returns>
  private static byte[] GenerateRandomBytes(int bytesNumber)
  {
    var buffer = new byte[bytesNumber];
    Csp.GetBytes(buffer);
    return buffer;
  }

  /// <summary>Gets the ith character of a string or 0 if the string is shorter than i characters.</summary>
  /// <param name="str">Affected string.</param>
  /// <param name="i">Zero based index.</param>
  /// <returns>The ith character or 0.</returns>
  private static char GetChar(string str, int i)
  {
    if (str != null && i < str.Length)
    {
      return str[i];
    }
    return (char)0;
  }

  /// <summary>Regular expression for coordinates.</summary>
  [GeneratedRegex("^(-?\\d+(\\.\\d+)),\\s*(-?\\d+(\\.\\d+))(,\\s*(-?\\d+(\\.\\d+))z?)?$", RegexOptions.Compiled)]
  private static partial Regex CoordinatesRegex();

  /// <summary>All Windows-1252 characters.</summary>
  [GeneratedRegex("^(\n[\\u0000-\\u007F]|\n\\u20AC| # 80 Euro\n\\u201A| # 82\n\\u0192| # 83\n\\u201E| # 84\n\\u2026| # 85\n\\u2020| # 86\n\\u2021| # 87\n\\u02C6| # 88\n\\u2030| # 89\n\\u0160| # 8A\n\\u2039| # 8B\n\\u0152| # 8C\n\\u017D| # 8E\n\\u2018| # 91\n\\u2019| # 92\n\\u201C| # 93\n\\u201D| # 94\n\\u2022| # 95\n\\u2013| # 96\n\\u2014| # 97\n\\u02DC| # 98\n\\u2122| # 99\n\\u0161| # 9A\n\\u203A| # 9B\n\\u0153| # 9C\n\\u017E| # 9E\n\\u0178| # 9F\n[\\u00A0-\\u00FF]|\n)*$", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace)]
  private static partial Regex Windows1252Regex();

  /// <summary>All allowed Windows-1252 characters without control characters but with CR, LF and Tab.</summary>
  [GeneratedRegex("^(\n\\u0009| # Tab\n\\u000A| # LF\n\\u000D| # CR\n[\\u0020-\\u007F]|\n\\u20AC| # 80 Euro\n\\u201A| # 82\n\\u0192| # 83\n\\u201E| # 84\n\\u2026| # 85\n\\u2020| # 86\n\\u2021| # 87\n\\u02C6| # 88\n\\u2030| # 89\n\\u0160| # 8A\n\\u2039| # 8B\n\\u0152| # 8C\n\\u017D| # 8E\n\\u2018| # 91\n\\u2019| # 92\n\\u201C| # 93\n\\u201D| # 94\n\\u2022| # 95\n\\u2013| # 96\n\\u2014| # 97\n\\u02DC| # 98\n\\u2122| # 99\n\\u0161| # 9A\n\\u203A| # 9B\n\\u0153| # 9C\n\\u017E| # 9E\n\\u0178| # 9F\n[\\u00A0-\\u00FF]|\n)*$", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace)]
  private static partial Regex Windows1252CrLfTabRegex();

  /// <summary>All allowed Windows-1252 characters without control characters.</summary>
  [GeneratedRegex("^(\n[\\u0020-\\u007F]|\n\\u20AC| # 80 Euro\n\\u201A| # 82\n\\u0192| # 83\n\\u201E| # 84\n\\u2026| # 85\n\\u2020| # 86\n\\u2021| # 87\n\\u02C6| # 88\n\\u2030| # 89\n\\u0160| # 8A\n\\u2039| # 8B\n\\u0152| # 8C\n\\u017D| # 8E\n\\u2018| # 91\n\\u2019| # 92\n\\u201C| # 93\n\\u201D| # 94\n\\u2022| # 95\n\\u2013| # 96\n\\u2014| # 97\n\\u02DC| # 98\n\\u2122| # 99\n\\u0161| # 9A\n\\u203A| # 9B\n\\u0153| # 9C\n\\u017E| # 9E\n\\u0178| # 9F\n[\\u00A0-\\u00FF]|\n)*$", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace)]
  private static partial Regex Windows1252OhneRegex();
}
