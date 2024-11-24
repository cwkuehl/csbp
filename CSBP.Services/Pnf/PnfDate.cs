// <copyright file="PnfDate.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Pnf;

using System;

/// <summary>Month letter for chart.</summary>
public class PnfDate
{
  /// <summary>Curent date of price.</summary>
  private DateTime? date = null;

  /// <summary>Last date of column for determinating the beginning of the month.</summary>
  private DateTime? datumm = null;

  /// <summary>Month number to write.</summary>
  private int monat = 0;

  /// <summary>
  /// Initializes a new instance of the <see cref="PnfDate"/> class.
  /// </summary>
  public PnfDate()
  {
  }

  /// <summary>Gets or sets current date of price.</summary>
  public DateTime? Date
  {
    get
    {
      return date;
    }

    set
    {
      date = value;
      monat = 0;
      if (!date.HasValue)
        return;
      var d = date.Value;
      if (!datumm.HasValue || ((datumm.Value.Year * 100) + datumm.Value.Month < (d.Year * 100) + d.Month))
      {
        monat = d.Month;
      }
    }
  }

  /// <summary>
  /// Gets letter for new month.
  /// </summary>
  /// <param name="c">Default value if month is 0.</param>
  /// <returns>New month letter or c.</returns>
  public char GetNewMonth(char c)
  {
    if (monat > 0)
    {
      if (monat < 10)
      {
        return (char)((int)'0' + monat);
      }
      return (char)((int)'A' + monat - 10);
    }
    return c;
  }

  /// <summary>
  /// Sets the used month.
  /// </summary>
  public void SetUsedMonth()
  {
    if (date.HasValue)
    {
      datumm = date;
    }
    monat = 0;
  }
}
