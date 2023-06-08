// <copyright file="AgDialog.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models;

using System;
using System.ComponentModel.DataAnnotations.Schema;
using CSBP.Base;

/// <summary>
/// Entity class for table AG_Dialog.
/// </summary>
[Serializable]
[Table("AG_Dialog")]
public partial class AgDialog : ModelBase
{
  /// <summary>Initializes a new instance of the <see cref="AgDialog"/> class.</summary>
  public AgDialog()
  {
    Functions.MachNichts();
  }

  /// <summary>Gets or sets the value of column Mandant_Nr.</summary>
  public int Mandant_Nr { get; set; }

  /// <summary>Gets or sets the value of column Uid.</summary>
  public string Uid { get; set; }

  /// <summary>Gets or sets the value of column Api.</summary>
  public string Api { get; set; }

  /// <summary>Gets or sets the value of column Datum.</summary>
  public DateTime Datum { get; set; }

  /// <summary>Gets or sets the value of column Nr.</summary>
  public int Nr { get; set; }

  /// <summary>Gets or sets the value of column Url.</summary>
  public string Url { get; set; }

  /// <summary>Gets or sets the value of column Frage.</summary>
  public string Frage { get; set; }

  /// <summary>Gets or sets the value of column Antwort.</summary>
  public string Antwort { get; set; }

  /// <summary>Gets or sets the value of column Angelegt_Von.</summary>
  public string Angelegt_Von { get; set; }

  /// <summary>Gets or sets the value of column Angelegt_Am.</summary>
  public DateTime? Angelegt_Am { get; set; }

  /// <summary>Gets or sets the value of column Geaendert_Von.</summary>
  public string Geaendert_Von { get; set; }

  /// <summary>Gets or sets the value of column Geaendert_Am.</summary>
  public DateTime? Geaendert_Am { get; set; }
}
