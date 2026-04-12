// <copyright file="EnAbfrage.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Apis.Models;

using System.ComponentModel.DataAnnotations.Schema;
using CSBP.Services.Base;

/// <summary>
/// Entity class for table EN_Abfrage.
/// </summary>
public partial class EnAbfrage : ModelBase
{
  /// <summary>Gets or sets the datatype without enum.</summary>
  [NotMapped]
  public string Datatype
  {
    get
    {
      return SplitDatatype.Datatype;
    }

    set
    {
      SplitDatatype = (value, Enum);
    }
  }

  /// <summary>Gets or sets the enum values.</summary>
  [NotMapped]
  public string Enum
  {
    get
    {
      return SplitDatatype.Enum;
    }

    set
    {
      SplitDatatype = (Datatype, value);
    }
  }

  /// <summary>Gets or sets the splitted datatype.</summary>
  [NotMapped]
  public (string Datatype, string Enum) SplitDatatype
  {
    get
    {
      if (string.IsNullOrWhiteSpace(Datentyp))
        return (null, null);
      var arr = (Datentyp ?? "").Split('|', 2);
      return (arr.Length > 0 ? arr[0] ?? null : null, arr.Length > 1 ? arr[1] ?? null : null);
    }

    set
    {
      Datentyp = Functions.TrimNull((value.Datatype ?? "") + Functions.Iif(string.IsNullOrEmpty(value.Enum), "", $"|{value.Enum}"));
    }
  }
}
