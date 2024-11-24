// <copyright file="ModelBase.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Base;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Reflection;
using static CSBP.Services.Resources.M;

/// <summary>Base class for model entity.</summary>
[Serializable]
public class ModelBase
{
  /// <summary>Conversion culture info.</summary>
  private static readonly CultureInfo ConvertCultureInfo = CultureInfo.CreateSpecificCulture("de-DE");

  /// <summary>
  /// Initializes a new instance of the <see cref="ModelBase"/> class.
  /// </summary>
  public ModelBase()
  {
    Functions.MachNichts();
  }

  /// <summary>
  /// Gets the attributed table name or null.
  /// </summary>
  public string TableName
  {
    get
    {
      var att = GetType().GetCustomAttributes(false)
          .FirstOrDefault(a => a is TableAttribute) as TableAttribute;
      return att?.Name;
    }
  }

  /// <summary>
  /// Gets all columns properties.
  /// </summary>
  [NotMapped]
  [System.Xml.Serialization.XmlIgnore]
  public List<PropertyInfo> ColumnProperties
  {
    get
    {
      var props = GetType().GetProperties().Where(a => a.CanWrite
          && a.GetCustomAttribute<NotMappedAttribute>() == null);
      return props.ToList();
    }
  }

  /// <summary>Gets or sets the extension values as string.</summary>
  private string Extension { get; set; }

  /// <summary>
  /// Formats string from date and user id.
  /// </summary>
  /// <param name="date">Affected date.</param>
  /// <param name="of">Affected user id.</param>
  /// <returns>string from date and user id.</returns>
  public static string FormatDateOf(DateTime? date, string of)
  {
    if (!date.HasValue)
      return "";
    return M1011(date.Value, of, true);
  }

  /// <summary>
  /// Sets columns Angelegt_Von and Angelegt_Am.
  /// </summary>
  /// <param name="now">Actual date and time.</param>
  /// <param name="userid">Affected user id.</param>
  public void MachAngelegt(DateTime? now, string userid)
  {
    var am = GetType().GetProperty("Angelegt_Am");
    var von = GetType().GetProperty("Angelegt_Von");
    if (am == null || von == null)
      return;
    am.GetSetMethod().Invoke(this, new object[] { now });
    von.GetSetMethod().Invoke(this, new object[] { userid });
  }

  /// <summary>
  /// Sets columns Geaendert_Von and Geaendert_Am, if the entry in the columns Angelegt_Am or Geaendert_Am lasts AEND_ZEIT ticks.
  /// </summary>
  /// <param name="now">Actual date and time.</param>
  /// <param name="userid">Affected user id.</param>
  public void MachGeaendert(DateTime? now, string userid)
  {
    var am = GetType().GetProperty("Geaendert_Am");
    var von = GetType().GetProperty("Geaendert_Von");
    if (am == null || von == null)
      return;
    var datum = am.GetGetMethod().Invoke(this, null) as DateTime? ?? GetType().GetProperty("Angelegt_Am")?.GetGetMethod().Invoke(this, null) as DateTime?;
    if (datum == null || now == null || (now.Value - datum.Value).TotalMilliseconds > Constants.AEND_ZEIT)
    {
      am.GetSetMethod().Invoke(this, new object[] { now });
      von.GetSetMethod().Invoke(this, new object[] { userid });
    }
  }

  /// <summary>Converts string bei removing semicola.</summary>
  /// <param name="s">Affected string.</param>
  /// <returns>String without semicola.</returns>
  protected static string ToString(string s)
  {
    if (string.IsNullOrEmpty(s))
      return "";
    return s.Replace(";", "");
  }

  /// <summary>Converts int to string.</summary>
  /// <param name="d">Affected integer.</param>
  /// <returns>Converted string.</returns>
  protected static string ToString(int? d)
  {
    if (d.HasValue)
      return d.Value.ToString(ConvertCultureInfo);
    return "";
  }

  /// <summary>Converts decimal to string.</summary>
  /// <param name="d">Affected decimal.</param>
  /// <param name="digits">Number of digits.</param>
  /// <returns>Converted string.</returns>
  protected static string ToString(decimal? d, int digits = -1)
  {
    if (d.HasValue)
    {
      if (digits < 0)
        return d.Value.ToString(ConvertCultureInfo);
      return Math.Round(d.Value, digits, MidpointRounding.AwayFromZero).ToString(ConvertCultureInfo);
    }
    return "";
  }

  /// <summary>Converts datetime to string.</summary>
  /// <param name="d">Affected datetime.</param>
  /// <returns>Converted string.</returns>
  protected static string ToString(DateTime? d)
  {
    if (d.HasValue)
      return d.Value.ToString(ConvertCultureInfo);
    return "";
  }

  /// <summary>Converts bool to string.</summary>
  /// <param name="d">Affected bool.</param>
  /// <returns>Converted string.</returns>
  protected static string ToString(bool? d)
  {
    if (d.HasValue)
      return d.Value.ToString(ConvertCultureInfo);
    return "";
  }

  /// <summary>Converts string to int.</summary>
  /// <param name="s">Affected string.</param>
  /// <returns>Converted integer or null.</returns>
  protected static int? ToInt(string s)
  {
    if (!string.IsNullOrWhiteSpace(s) && int.TryParse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands,
        ConvertCultureInfo, out var d))
      return d;
    return null;
  }

  /// <summary>Converts string to decimal.</summary>
  /// <param name="s">Affected string.</param>
  /// <returns>Converted decimal or null.</returns>
  protected static decimal? ToDecimal(string s)
  {
    if (!string.IsNullOrWhiteSpace(s) && decimal.TryParse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands,
        ConvertCultureInfo, out var d))
      return d;
    return null;
  }

  /// <summary>Converts string to datetime.</summary>
  /// <param name="s">Affected string.</param>
  /// <returns>Converted datetime or null.</returns>
  protected static DateTime? ToDateTime(string s)
  {
    if (!string.IsNullOrWhiteSpace(s) && DateTime.TryParse(s, ConvertCultureInfo, DateTimeStyles.AllowWhiteSpaces, out var d))
      return d;
    return null;
  }

  /// <summary>Converts string to bool.</summary>
  /// <param name="s">Affected string.</param>
  /// <returns>Converted bool or null.</returns>
  protected static bool? ToBool(string s)
  {
    if (!string.IsNullOrWhiteSpace(s) && bool.TryParse(s, out var d))
      return d;
    return null;
  }

  /// <summary>
  /// Gets all extended values as string.
  /// </summary>
  /// <returns>Extended values as string.</returns>
  protected virtual string GetExtension()
  {
    return Extension;
  }

  /// <summary>
  /// Sets all extended values from string.
  /// </summary>
  /// <param name="value">Extended values as string.</param>
  protected virtual void SetExtension(string value)
  {
    Extension = value;
  }
}
