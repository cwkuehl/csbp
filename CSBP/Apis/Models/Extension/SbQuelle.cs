﻿// <copyright file="SbQuelle.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models
{
  using CSBP.Base;

  /// <summary>
  /// Entity-Klasse für Tabelle SB_Quelle.
  /// </summary>
  public partial class SbQuelle : ModelBase
  {
    /// <summary>Holt die Quellenbeschreibung.</summary>
    ////[NotMapped]
    public string SourceName
    {
      get
      {
        return Functions.Append(Beschreibung, ", ", Autor);
      }
    }
  }
}
