// <copyright file="FzFahrrad.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models;

using CSBP.Apis.Enums;
using CSBP.Base;
using CSBP.Resources;

/// <summary>
/// Entity class for table FZ_Fahrrad.
/// </summary>
public partial class FzFahrrad : ModelBase
{
  /// <summary>Gets the type description.</summary>
  ////[NotMapped]
  public string TypBezeichnung => this.Typ == (int)BikeTypeEnum.Tour
    ? Messages.Enum_bike_tour : Messages.Enum_bike_weekly;

  /// <summary>Gets a value indicating whether the type is weekly.</summary>
  ////[NotMapped]
  public bool IsWeekly => this.Typ != (int)BikeTypeEnum.Tour;
}
