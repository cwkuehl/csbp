// <copyright file="FzFahrrad.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models
{
  using CSBP.Apis.Enums;
  using CSBP.Base;
  using CSBP.Resources;

  /// <summary>
  /// Entity class for table FZ_Fahrrad.
  /// </summary>
  public partial class FzFahrrad : ModelBase
  {
    /// <summary>Holt den Wert der Typ-Bezeichnung.</summary>
    ////[NotMapped]
    public string TypBezeichnung => this.Typ == (int)BikeTypeEnum.Tour
      ? Messages.Enum_bike_tour : Messages.Enum_bike_weekly;

    /// <summary>Holt einen Wert, der angibt, ob der Typ wöchentlich ist.</summary>
    ////[NotMapped]
    public bool IsWeekly => this.Typ != (int)BikeTypeEnum.Tour;
  }
}
