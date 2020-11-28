// <copyright file="AdSitz.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models
{
  using System;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Linq;
  using System.Text;
  using CSBP.Base;

  /// <summary>
  /// Entity-Klasse für Tabelle AD_Sitz.
  /// </summary>
  public partial class AdSitz : ModelBase
  {
    /// <summary>Holt den Namen inkl. Vornamen.</summary>
    ////[NotMapped]
    public string PersonName
    {
      get
      {
        var sb = new StringBuilder();
        sb.Append(Person?.Person_Status == 0 ? "" : "(");
        sb.Append(Person?.Name1);
        if (!string.IsNullOrEmpty(Person?.Vorname))
          sb.Append(", ").Append(Person?.Vorname);
        sb.Append(Person?.Person_Status == 0 ? "" : ")");
        return sb.ToString();
      }
    }

    /// <summary>Holt den Sitz-Namen inkl. Ort.</summary>
    ////[NotMapped]
    public string SiteName
    {
      get
      {
        var sb = new StringBuilder();
        sb.Append(Sitz_Status == 0 ? "" : "(");
        sb.Append(Name);
        if (!string.IsNullOrEmpty(Address?.Ort))
          sb.Append(", ").Append(Address?.Ort);
        sb.Append(Sitz_Status == 0 ? "" : ")");
        return sb.ToString();
      }
    }

    /// <summary>Holt oder setzt die Person.</summary>
    [NotMapped]
    public AdPerson Person { get; set; }

    /// <summary>Holt oder setzt die Adresse.</summary>
    [NotMapped]
    public AdAdresse Address { get; set; }

    /// <summary>Holt die Benutzer-ID der ersten Erfassung.</summary>
    [NotMapped]
    public string CreatedBy
    {
      get
      {
        return new[] {
          new { by = Angelegt_Von, at = Angelegt_Am },
          new { by = Person?.Angelegt_Von, at = Person?.Angelegt_Am },
          new { by = Address?.Angelegt_Von, at = Address?.Angelegt_Am }
        }.Where(a => !string.IsNullOrEmpty(a.by) && a.at.HasValue)
        .OrderBy(a => a.at).FirstOrDefault()?.by;
      }
    }

    /// <summary>Holt den Zeitpunkt der ersten Erfassung.</summary>
    [NotMapped]
    public DateTime? CreatedAt
    {
      get
      {
        return new[] {
          new { by = Angelegt_Von, at = Angelegt_Am },
          new { by = Person?.Angelegt_Von, at = Person?.Angelegt_Am },
          new { by = Address?.Angelegt_Von, at = Address?.Angelegt_Am }
        }.Where(a => !string.IsNullOrEmpty(a.by) && a.at.HasValue)
        .OrderBy(a => a.at).FirstOrDefault()?.at;
      }
    }

    /// <summary>Holt die Benutzer-ID der letzten Änderung.</summary>
    [NotMapped]
    public string ChangedBy
    {
      get
      {
        return new[] {
          new { by = Angelegt_Von, at = Angelegt_Am },
          new { by = Person?.Angelegt_Von, at = Person?.Angelegt_Am },
          new { by = Address?.Angelegt_Von, at = Address?.Angelegt_Am }
        }.Where(a => !string.IsNullOrEmpty(a.by) && a.at.HasValue)
        .OrderByDescending(a => a.at).FirstOrDefault()?.by;
      }
    }

    /// <summary>Holt den Zeitpunkt der letzten Änderung.</summary>
    [NotMapped]
    public DateTime? ChangedAt
    {
      get
      {
        return new[] {
          new { by = Angelegt_Von, at = Angelegt_Am },
          new { by = Person?.Angelegt_Von, at = Person?.Angelegt_Am },
          new { by = Address?.Angelegt_Von, at = Address?.Angelegt_Am }
        }.Where(a => !string.IsNullOrEmpty(a.by) && a.at.HasValue)
        .OrderByDescending(a => a.at).FirstOrDefault()?.at;
      }
    }

  }
}
