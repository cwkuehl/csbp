// <copyright file="FzBuchautor.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models;

using CSBP.Base;

/// <summary>
/// Entity class for table FZ_Buchautor.
/// </summary>
public partial class FzBuchautor : ModelBase
{
  /// <summary>Gets the complet name.</summary>
  ////[NotMapped]
  public string CompleteName
  {
    get
    {
      if (string.IsNullOrWhiteSpace(Name))
        return Vorname;
      if (string.IsNullOrWhiteSpace(Vorname))
        return Name;
      return $"{Vorname} {Name}";
    }
  }
}
