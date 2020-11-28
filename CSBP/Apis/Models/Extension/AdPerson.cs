// <copyright file="AdPerson.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models
{
  using System.Text;
  using CSBP.Base;

  /// <summary>
  /// Entity-Klasse für Tabelle AD_Person.
  /// </summary>
  public partial class AdPerson : ModelBase
  {
    /// <summary>Holt den Namen inkl. Vornamen.</summary>
    ////[NotMapped]
    public string Name
    {
      get
      {
        var sb = new StringBuilder();
        sb.Append(this.Person_Status == 0 ? "" : "(");
        sb.Append(Name1);
        if (!string.IsNullOrEmpty(Vorname))
          sb.Append(", ").Append(Vorname);
        sb.Append(this.Person_Status == 0 ? "" : ")");
        return sb.ToString();
      }
    }
  }
}
