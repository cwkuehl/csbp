// <copyright file="ByteDaten.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Apis.Models;

using System;
using System.ComponentModel.DataAnnotations.Schema;
using CSBP.Services.Base;

/// <summary>
/// Entity class for table Byte_Daten.
/// </summary>
[Serializable]
[Table("Byte_Daten")]
public partial class ByteDaten : ModelBase
{
  /// <summary>Initializes a new instance of the <see cref="ByteDaten"/> class.</summary>
  public ByteDaten()
  {
    Functions.MachNichts();
  }

  /// <summary>Gets or sets the value of column Mandant_Nr.</summary>
  public int Mandant_Nr { get; set; }

  /// <summary>Gets or sets the value of column Typ.</summary>
  public string Typ { get; set; }

  /// <summary>Gets or sets the value of column Uid.</summary>
  public string Uid { get; set; }

  /// <summary>Gets or sets the value of column Lfd_Nr.</summary>
  public int Lfd_Nr { get; set; }

  /// <summary>Gets or sets the value of column Metadaten.</summary>
  public string Metadaten { get; set; }

  /// <summary>Gets or sets the value of column Bytes.</summary>
  public byte[] Bytes { get; set; }

  /// <summary>Gets or sets the value of column Angelegt_Von.</summary>
  public string Angelegt_Von { get; set; }

  /// <summary>Gets or sets the value of column Angelegt_Am.</summary>
  public DateTime? Angelegt_Am { get; set; }

  /// <summary>Gets or sets the value of column Geaendert_Von.</summary>
  public string Geaendert_Von { get; set; }

  /// <summary>Gets or sets the value of column Geaendert_Am.</summary>
  public DateTime? Geaendert_Am { get; set; }
}
