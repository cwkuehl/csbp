// <copyright file="ModelBase.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Base
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Globalization;
  using System.Linq;
  using System.Reflection;
  using static CSBP.Resources.M;

  /// <summary>Base class for model entity.</summary>
  [Serializable]
  public class ModelBase
  {
    private string Extension { get; set; }

    public ModelBase()
    {
      Functions.MachNichts();
    }

    /// <summary>
    /// Eintragungen in Spalten Angelegt_Von und Angelegt_Am.
    /// </summary>
    /// <param name="jetzt">Aktuelle Zeit, die in Spalte Angelegt_Am eingetragen werden soll.</param>
    /// <param name="benutzer">Benutzer-ID, die in Spalte Angelegt_Von eingetragen werden soll.</param>
    public void MachAngelegt(DateTime? jetzt, string benutzer)
    {
      var am = GetType().GetProperty("Angelegt_Am");
      var von = GetType().GetProperty("Angelegt_Von");
      if (am == null || von == null)
        return;
      am.GetSetMethod().Invoke(this, new object[] { jetzt });
      von.GetSetMethod().Invoke(this, new object[] { benutzer });
    }

    /// <summary>
    /// Eintragungen in Spalten Geaendert_Von und Geaendert_Am,
    /// wenn der letzte Eintrag in den Spalten Angelegt_Am oder Geaendert_Am schon AEND_ZEIT Ticks her ist.
    /// </summary>
    /// <param name="jetzt">Aktuelle Zeit, die in Spalte Geaendert_Am eingetragen werden soll.</param>
    /// <param name="benutzer">Benutzer-ID, die in Spalte Geaendert_Von eingetragen werden soll.</param>
    public void MachGeaendert(DateTime? jetzt, string benutzer)
    {
      var am = GetType().GetProperty("Geaendert_Am");
      var von = GetType().GetProperty("Geaendert_Von");
      if (am == null || von == null)
        return;
      var datum = am.GetGetMethod().Invoke(this, null) as DateTime?;
      if (datum == null)
      {
        datum = GetType().GetProperty("Angelegt_Am")?.GetGetMethod().Invoke(this, null) as DateTime?;
      }
      if (datum == null || jetzt == null || (jetzt.Value - datum.Value).TotalMilliseconds > Constants.AEND_ZEIT)
      {
        am.GetSetMethod().Invoke(this, new object[] { jetzt });
        von.GetSetMethod().Invoke(this, new object[] { benutzer });
      }
    }

    public string FormatDateOf(DateTime? date, string of)
    {
      if (!date.HasValue)
        return "";
      return M1011(date.Value, of, true);
    }

    public string TableName
    {
      get
      {
        var att = GetType().GetCustomAttributes(false)
            .FirstOrDefault(a => a is TableAttribute) as TableAttribute;
        return att?.Name;
      }
    }

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

    private static readonly CultureInfo ConvertCultureInfo = CultureInfo.CreateSpecificCulture("de-DE");

    ///<summary>Entfernt evtl. vorhandene Semikola.</summary>
    protected string ToString(string s)
    {
      if (string.IsNullOrEmpty(s))
        return "";
      return s.Replace(";", "");
    }

    protected string ToString(int? d)
    {
      if (d.HasValue)
        return d.Value.ToString(ConvertCultureInfo);
      return "";
    }

    protected string ToString(decimal? d, int digits = -1)
    {
      if (d.HasValue)
      {
        if (digits < 0)
          return d.Value.ToString(ConvertCultureInfo);
        return Math.Round(d.Value, digits, MidpointRounding.AwayFromZero).ToString(ConvertCultureInfo);
      }
      return "";
    }

    protected string ToString(DateTime? d)
    {
      if (d.HasValue)
        return d.Value.ToString(ConvertCultureInfo);
      return "";
    }

    protected string ToString(bool? d)
    {
      if (d.HasValue)
        return d.Value.ToString(ConvertCultureInfo);
      return "";
    }

    protected int? ToInt(string s)
    {
      if (!string.IsNullOrWhiteSpace(s) && int.TryParse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands,
          ConvertCultureInfo, out var d))
        return d;
      return null;
    }

    protected decimal? ToDecimal(string s)
    {
      if (!string.IsNullOrWhiteSpace(s) && decimal.TryParse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands,
          ConvertCultureInfo, out var d))
        return d;
      return null;
    }

    protected DateTime? ToDateTime(string s)
    {
      if (!string.IsNullOrWhiteSpace(s) && DateTime.TryParse(s, ConvertCultureInfo, DateTimeStyles.AllowWhiteSpaces, out var d))
        return d;
      return null;
    }

    protected bool? ToBool(string s)
    {
      if (!string.IsNullOrWhiteSpace(s) && bool.TryParse(s, out var d))
        return d;
      return null;
    }

    protected virtual string GetExtension()
    {
      return Extension;
    }

    protected virtual void SetExtension(string value)
    {
      Extension = value;
    }
  }
}
