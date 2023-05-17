// <copyright file="Weather.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.NonService;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using CSBP.Base;
using static CSBP.Resources.M;
using static CSBP.Resources.Messages;

/// <summary>
/// Class for weather data.
/// </summary>
[Serializable]
public class Weather
{
  /// <summary>Gets or sets the time.</summary>
  public DateTime Time { get; set; }

  /// <summary>Gets or sets the air temperature in °C.</summary>
  public decimal Temp { get; set; }

  /// <summary>Gets or sets the dew point in °C.</summary>
  public decimal Dwpt { get; set; }

  /// <summary>Gets or sets the relative humidity in percent (%).</summary>
  public decimal Rhum { get; set; }

  /// <summary>Gets or sets the one hour precipitation total in mm.</summary>
  public decimal Prcp { get; set; }

  /// <summary>Gets or sets the snow depth in mm.</summary>
  public decimal Snow { get; set; }

  /// <summary>Gets or sets the wind direction in degrees (°).</summary>
  public decimal Wdir { get; set; }

  /// <summary>Gets or sets the average wind speed in km/h.</summary>
  public decimal Wspd { get; set; }

  /// <summary>Gets or sets the peak wind gust in km/h.</summary>
  public decimal Wpgt { get; set; }

  /// <summary>Gets or sets the sea-level air pressure in hPa.</summary>
  public decimal Pres { get; set; }

  /// <summary>Gets or sets the one hour sunshine total in minutes (m).</summary>
  public decimal Tsun { get; set; }

  /// <summary>Gets or sets the weather condition code.
  /// Code Weather Condition
  /// 1 Clear
  /// 2 Fair
  /// 3 Cloudy
  /// 4 Overcast
  /// 5 Fog
  /// 6 Freezing Fog
  /// 7 Light Rain
  /// 8 Rain
  /// 9 Heavy Rain
  /// 10 Freezing Rain
  /// 11 Heavy Freezing Rain
  /// 12 Sleet
  /// 13 Heavy Sleet
  /// 14 Light Snowfall
  /// 15 Snowfall
  /// 16 Heavy Snowfall
  /// 17 Rain Shower
  /// 18 Heavy Rain Shower
  /// 19 Sleet Shower
  /// 20 Heavy Sleet Shower
  /// 21 Snow Shower
  /// 22 Heavy Snow Shower
  /// 23 Lightning
  /// 24 Hail
  /// 25 Thunderstorm
  /// 26 Heavy Thunderstorm
  /// 27 Storm.</summary>
  public int Coco { get; set; }
}
