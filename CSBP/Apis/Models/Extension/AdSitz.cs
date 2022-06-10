// <copyright file="AdSitz.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models;

using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using CSBP.Base;

/// <summary>
/// Entity class for table AD_Sitz.
/// </summary>
public partial class AdSitz : ModelBase
{
  /// <summary>Gets last name and first name.</summary>
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

  /// <summary>Gets site name.</summary>
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

  /// <summary>Gets or sets the person.</summary>
  [NotMapped]
  public AdPerson Person { get; set; }

  /// <summary>Gets or sets the address.</summary>
  [NotMapped]
  public AdAdresse Address { get; set; }

  /// <summary>Gets the user who created.</summary>
  [NotMapped]
  public string CreatedBy
  {
    get
    {
      return new[]
      {
        new { by = Angelegt_Von, at = Angelegt_Am },
        new { by = Person?.Angelegt_Von, at = Person?.Angelegt_Am },
        new { by = Address?.Angelegt_Von, at = Address?.Angelegt_Am },
      }
      .Where(a => !string.IsNullOrEmpty(a.by) && a.at.HasValue)
      .OrderBy(a => a.at).FirstOrDefault()?.by;
    }
  }

  /// <summary>Gets the creation time.</summary>
  [NotMapped]
  public DateTime? CreatedAt
  {
    get
    {
      return new[]
      {
        new { by = Angelegt_Von, at = Angelegt_Am },
        new { by = Person?.Angelegt_Von, at = Person?.Angelegt_Am },
        new { by = Address?.Angelegt_Von, at = Address?.Angelegt_Am },
      }
      .Where(a => !string.IsNullOrEmpty(a.by) && a.at.HasValue)
      .OrderBy(a => a.at).FirstOrDefault()?.at;
    }
  }

  /// <summary>Gets the user who changed the last time.</summary>
  [NotMapped]
  public string ChangedBy
  {
    get
    {
      return new[]
      {
        new { by = Angelegt_Von, at = Angelegt_Am },
        new { by = Person?.Angelegt_Von, at = Person?.Angelegt_Am },
        new { by = Address?.Angelegt_Von, at = Address?.Angelegt_Am },
      }
      .Where(a => !string.IsNullOrEmpty(a.by) && a.at.HasValue)
      .OrderByDescending(a => a.at).FirstOrDefault()?.by;
    }
  }

  /// <summary>Gets the time of last change.</summary>
  [NotMapped]
  public DateTime? ChangedAt
  {
    get
    {
      return new[]
      {
        new { by = Angelegt_Von, at = Angelegt_Am },
        new { by = Person?.Angelegt_Von, at = Person?.Angelegt_Am },
        new { by = Address?.Angelegt_Von, at = Address?.Angelegt_Am },
      }
      .Where(a => !string.IsNullOrEmpty(a.by) && a.at.HasValue)
      .OrderByDescending(a => a.at).FirstOrDefault()?.at;
    }
  }
}
