// <copyright file="SbQuelle.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Apis.Models;

using CSBP.Services.Base;

/// <summary>
/// Entity class for table SB_Quelle.
/// </summary>
public partial class SbQuelle : ModelBase
{
  /// <summary>Gets the source description.</summary>
  ////[NotMapped]
  public string SourceName
  {
    get
    {
      return Functions.Append(Beschreibung, ", ", Autor);
    }
  }
}
