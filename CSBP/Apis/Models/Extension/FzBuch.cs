// <copyright file="FzBuch.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models;

using System;
using System.ComponentModel.DataAnnotations.Schema;
using CSBP.Base;
using CSBP.Resources;

/// <summary>
/// Entity class for table FZ_Buch.
/// </summary>
public partial class FzBuch : ModelBase
{
  /// <summary>Gets the language.</summary>
  ////[NotMapped]
  public string Language => this.Sprache_Nr == 1 ? Messages.Enum_language_german
    : this.Sprache_Nr == 2 ? Messages.Enum_language_english
    : this.Sprache_Nr == 3 ? Messages.Enum_language_french
    : Messages.Enum_language_other;

  /// <summary>Gets or sets the author's name.</summary>
  [NotMapped]
  public string AuthorName { get; set; }

  /// <summary>Gets or sets the author's first name.</summary>
  [NotMapped]
  public string AuthorFirstName { get; set; }

  /// <summary>Gets the author's complete name.</summary>
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

  /// <summary>Gets or sets the series name.</summary>
  [NotMapped]
  public string SeriesName { get; set; }

  /// <summary>Gets or sets a value indicating whether the book is owned.</summary>
  [NotMapped]
  public bool StatePossession { get; set; }

  /// <summary>Gets or sets the read date.</summary>
  [NotMapped]
  public DateTime? StateRead { get; set; }

  /// <summary>Gets or sets the heard date.</summary>
  [NotMapped]
  public DateTime? StateHeard { get; set; }
}
