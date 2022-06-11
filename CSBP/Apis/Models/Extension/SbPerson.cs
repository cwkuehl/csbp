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
  /// <summary>Holt den komplette Ahnennamen.</summary>
  ////[NotMapped]
  public string AncestorName
  {
    get
    {
      return Functions.AhnString(Uid, Geburtsname, Vorname);
    }
  }

  /// <summary>Holt oder setzt das Geburtsdatum.</summary>
  [NotMapped]
  public string Birthdate { get; set; }

  /// <summary>Holt oder setzt den Geburtsort.</summary>
  [NotMapped]
  public string Birthplace { get; set; }

  /// <summary>Holt oder setzt die Geburtsbemerkung.</summary>
  [NotMapped]
  public string Birthmemo { get; set; }

  /// <summary>Holt oder setzt das Taufdatum.</summary>
  [NotMapped]
  public string Christdate { get; set; }

  /// <summary>Holt oder setzt den Taufort.</summary>
  [NotMapped]
  public string Christplace { get; set; }

  /// <summary>Holt oder setzt die Taufbemerkung.</summary>
  [NotMapped]
  public string Christmemo { get; set; }

  /// <summary>Holt oder setzt das Todesdatum.</summary>
  [NotMapped]
  public string Deathdate { get; set; }

  /// <summary>Holt oder setzt den Todesort.</summary>
  [NotMapped]
  public string Deathplace { get; set; }

  /// <summary>Holt oder setzt die Todesbemerkung.</summary>
  [NotMapped]
  public string Deathmemo { get; set; }

  /// <summary>Holt oder setzt das Begräbnisdatum.</summary>
  [NotMapped]
  public string Burialdate { get; set; }

  /// <summary>Holt oder setzt den Begräbnisort.</summary>
  [NotMapped]
  public string Burialplace { get; set; }

  /// <summary>Holt oder setzt die Begräbnisbemerkung.</summary>
  [NotMapped]
  public string Burialmemo { get; set; }

  /// <summary>Holt oder setzt den Vater.</summary>
  [NotMapped]
  public SbPerson Father { get; set; }

  // /// <summary>Holt oder setzt die Uid des Vaters.</summary>
  // [NotMapped]
  // public string Father_Uid { get; set; }

  // /// <summary>Holt oder setzt den Geburtsnamen des Vaters.</summary>
  // [NotMapped]
  // public string Father_Birthname { get; set; }

  // /// <summary>Holt oder setzt die Vornamen des Vaters.</summary>
  // [NotMapped]
  // public string Father_Firstname { get; set; }

  /// <summary>Holt oder setzt den Mutter.</summary>
  [NotMapped]
  public SbPerson Mother { get; set; }

  // /// <summary>Holt oder setzt die Uid der Mutter.</summary>
  // [NotMapped]
  // public string Mother_Uid { get; set; }

  // /// <summary>Holt oder setzt den Geburtsnamen der Mutter.</summary>
  // [NotMapped]
  // public string Mother_Birthname { get; set; }

  // /// <summary>Holt oder setzt die Vornamen der Mutter.</summary>
  // [NotMapped]
  // public string Mother_Firstname { get; set; }

  // /// <summary>Holt oder setzt einen Wert, der angibt, ob das Buch im eigenem Besitz ist.</summary>
  // [NotMapped]
  // public bool StatePossession { get; set; }
}
