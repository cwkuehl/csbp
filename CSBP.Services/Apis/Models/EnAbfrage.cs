// <copyright file="EnAbfrage.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Apis.Models;

using System;
using System.ComponentModel.DataAnnotations.Schema;
using CSBP.Services.Base;

/// <summary>
/// Entity class for table EN_Abfrage.
/// </summary>
[Serializable]
[Table("EN_Abfrage")]
public partial class EnAbfrage : ModelBase
{
  /// <summary>Initializes a new instance of the <see cref="EnAbfrage"/> class.</summary>
  public EnAbfrage()
  {
    Functions.MachNichts();
  }

  /// <summary>Gets or sets the value of column Mandant_Nr.</summary>
  public int Mandant_Nr { get; set; }

  /// <summary>Gets or sets the value of column Uid.</summary>
  public string Uid { get; set; }

  /// <summary>Gets or sets the value of column Sortierung.</summary>
  public string Sortierung { get; set; }

  /// <summary>Gets or sets the value of column Art.</summary>
  public string Art { get; set; }

  /// <summary>Gets or sets the value of column Bezeichnung.</summary>
  public string Bezeichnung { get; set; }

  /// <summary>Gets or sets the value of column Host_Url.</summary>
  public string Host_Url { get; set; }

  /// <summary>Gets or sets the value of column Datentyp.</summary>
  public string Datentyp { get; set; }

  /// <summary>Gets or sets the value of column Schreibbarkeit.</summary>
  public string Schreibbarkeit { get; set; }

  /// <summary>Gets or sets the value of column Einheit.</summary>
  public string Einheit { get; set; }

  /// <summary>Gets or sets the value of column Param1.</summary>
  public string Param1 { get; set; }

  /// <summary>Gets or sets the value of column Param2.</summary>
  public string Param2 { get; set; }

  /// <summary>Gets or sets the value of column Param3.</summary>
  public string Param3 { get; set; }

  /// <summary>Gets or sets the value of column Param4.</summary>
  public string Param4 { get; set; }

  /// <summary>Gets or sets the value of column Param5.</summary>
  public string Param5 { get; set; }

  /// <summary>Gets or sets the value of column Status.</summary>
  public string Status { get; set; }

  /// <summary>Gets or sets the value of column Notiz.</summary>
  public string Notiz { get; set; }

  /// <summary>Gets or sets the value of column Parameter.</summary>
  public string Parameter
  {
    get { return GetExtension(); }
    set { SetExtension(value); }
  }

  /// <summary>Gets or sets the value of column Angelegt_Von.</summary>
  public string Angelegt_Von { get; set; }

  /// <summary>Gets or sets the value of column Angelegt_Am.</summary>
  public DateTime? Angelegt_Am { get; set; }

  /// <summary>Gets or sets the value of column Geaendert_Von.</summary>
  public string Geaendert_Von { get; set; }

  /// <summary>Gets or sets the value of column Geaendert_Am.</summary>
  public DateTime? Geaendert_Am { get; set; }
}
