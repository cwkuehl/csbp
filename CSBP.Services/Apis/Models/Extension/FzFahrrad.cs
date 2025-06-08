// <copyright file="FzFahrrad.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Apis.Models;

using System.ComponentModel.DataAnnotations.Schema;
using CSBP.Services.Apis.Enums;
using CSBP.Services.Base;
using CSBP.Services.Resources;

/// <summary>
/// Entity class for table FZ_Fahrrad.
/// </summary>
public partial class FzFahrrad : ModelBase
{
  /// <summary>Gets the type description.</summary>
  public string TypBezeichnung => this.Typ == (int)BikeTypeEnum.Tour ? Messages.Enum_bike_tour : Messages.Enum_bike_weekly;

  // /// <summary>Gets or sets the value of column Typ with string.</summary>
  // [NotMapped]
  // public string TypString
  // {
  //   get { return this.Typ == (int)BikeTypeEnum.Tour ? Messages.Enum_bike_tour : Messages.Enum_bike_weekly; }
  //   set { this.Typ = value == Messages.Enum_bike_tour ? (int)BikeTypeEnum.Tour : (int)BikeTypeEnum.Weekly; }
  // }

  /// <summary>Gets a value indicating whether the type is weekly.</summary>
  ////[NotMapped]
  public bool IsWeekly => this.Typ != (int)BikeTypeEnum.Tour;

  /// <summary>
  /// Gets type from type description.
  /// </summary>
  /// <param name="desc">Affected Description.</param>
  /// <returns>Affected Type as int.</returns>
  public static int GetTyp(string desc) => desc == Messages.Enum_bike_tour ? (int)BikeTypeEnum.Tour : (int)BikeTypeEnum.Weekly;
}
