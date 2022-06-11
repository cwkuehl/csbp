// <copyright file="SbPerson.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models;

using System.ComponentModel.DataAnnotations.Schema;
using CSBP.Base;

/// <summary>
/// Entity class for table SB_Person.
/// </summary>
public partial class SbPerson : ModelBase
{
  /// <summary>Gets the complete ancestor name.</summary>
  ////[NotMapped]
  public string AncestorName
  {
    get
    {
      return Functions.AhnString(Uid, Geburtsname, Vorname);
    }
  }

  /// <summary>Gets or sets the birth date.</summary>
  [NotMapped]
  public string Birthdate { get; set; }

  /// <summary>Gets or sets the birth place.</summary>
  [NotMapped]
  public string Birthplace { get; set; }

  /// <summary>Gets or sets the birth memo.</summary>
  [NotMapped]
  public string Birthmemo { get; set; }

  /// <summary>Gets or sets the baptist date.</summary>
  [NotMapped]
  public string Christdate { get; set; }

  /// <summary>Gets or sets the baptist place.</summary>
  [NotMapped]
  public string Christplace { get; set; }

  /// <summary>Gets or sets the baptist memo.</summary>
  [NotMapped]
  public string Christmemo { get; set; }

  /// <summary>Gets or sets the death date.</summary>
  [NotMapped]
  public string Deathdate { get; set; }

  /// <summary>Gets or sets the death place.</summary>
  [NotMapped]
  public string Deathplace { get; set; }

  /// <summary>Gets or sets the death memo.</summary>
  [NotMapped]
  public string Deathmemo { get; set; }

  /// <summary>Gets or sets the funeral date.</summary>
  [NotMapped]
  public string Burialdate { get; set; }

  /// <summary>Gets or sets the funeral place.</summary>
  [NotMapped]
  public string Burialplace { get; set; }

  /// <summary>Gets or sets the funeral memo.</summary>
  [NotMapped]
  public string Burialmemo { get; set; }

  /// <summary>Gets or sets the father.</summary>
  [NotMapped]
  public SbPerson Father { get; set; }

  /// <summary>Gets or sets the mother.</summary>
  [NotMapped]
  public SbPerson Mother { get; set; }
}
