// <copyright file="Benutzer.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models;

using System;
using System.ComponentModel.DataAnnotations.Schema;
using CSBP.Base;

/// <summary>
/// Entity class for table Benutzer.
/// </summary>
[Serializable]
[Table("Benutzer")]
public partial class Benutzer : ModelBase
{
  /// <summary>Initializes a new instance of the <see cref="Benutzer"/> class.</summary>
  public Benutzer()
  {
    Functions.MachNichts();
  }

  /// <summary>Gets or sets the value of column Mandant_Nr.</summary>
  public int Mandant_Nr { get; set; }

  /// <summary>Gets or sets the value of column Benutzer_ID.</summary>
  public string Benutzer_ID { get; set; }

  /// <summary>Gets or sets the value of column Passwort.</summary>
  public string Passwort { get; set; }

  /// <summary>Gets or sets the value of column Berechtigung.</summary>
  public int Berechtigung { get; set; }

  /// <summary>Gets or sets the value of column Akt_Periode.</summary>
  public int Akt_Periode { get; set; }

  /// <summary>Gets or sets the value of column Person_Nr.</summary>
  public int Person_Nr { get; set; }

  /// <summary>Gets or sets the value of column Geburt.</summary>
  public DateTime? Geburt { get; set; }

  /// <summary>Gets or sets the value of column Angelegt_Von.</summary>
  public string Angelegt_Von { get; set; }

  /// <summary>Gets or sets the value of column Angelegt_Am.</summary>
  public DateTime? Angelegt_Am { get; set; }

  /// <summary>Gets or sets the value of column Geaendert_Von.</summary>
  public string Geaendert_Von { get; set; }

  /// <summary>Gets or sets the value of column Geaendert_Am.</summary>
  public DateTime? Geaendert_Am { get; set; }
}
