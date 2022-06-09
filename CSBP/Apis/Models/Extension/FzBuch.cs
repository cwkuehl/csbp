// <copyright file="FzBuch.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models
{
  using System;
  using System.ComponentModel.DataAnnotations.Schema;
  using CSBP.Base;
  using CSBP.Resources;

  /// <summary>
  /// Entity class for table FZ_Buch.
  /// </summary>
  public partial class FzBuch : ModelBase
  {
    /// <summary>Holt die Sprache.</summary>
    ////[NotMapped]
    public string Language => this.Sprache_Nr == 1 ? Messages.Enum_language_german
      : this.Sprache_Nr == 2 ? Messages.Enum_language_english
      : this.Sprache_Nr == 3 ? Messages.Enum_language_french
      : Messages.Enum_language_other;

    /// <summary>Holt oder setzt den Autorennamen.</summary>
    [NotMapped]
    public string AuthorName { get; set; }

    /// <summary>Holt oder setzt den Autorenvornamen.</summary>
    [NotMapped]
    public string AuthorFirstName { get; set; }

    /// <summary>Holt den Wert des kompletten Autorennamens.</summary>
    ////[NotMapped]
    public string AuthorCompleteName
    {
      get
      {
        if (string.IsNullOrWhiteSpace(AuthorName))
          return AuthorFirstName;
        if (string.IsNullOrWhiteSpace(AuthorFirstName))
          return AuthorName;
        return $"{AuthorFirstName} {AuthorName}";
      }
    }

    /// <summary>Holt oder setzt den Seriennamen.</summary>
    [NotMapped]
    public string SeriesName { get; set; }

    /// <summary>Holt oder setzt einen Wert, der angibt, ob das Buch im eigenem Besitz ist.</summary>
    [NotMapped]
    public bool StatePossession { get; set; }

    /// <summary>Holt oder setzt das Lesedatum.</summary>
    [NotMapped]
    public DateTime? StateRead { get; set; }

    /// <summary>Holt oder setzt das Hördatum.</summary>
    [NotMapped]
    public DateTime? StateHeard { get; set; }
  }
}
