// <copyright file="PnfColumn.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Pnf;

using System;
using System.Text;
using CSBP.Services.Base;

/// <summary>
/// Function for PnF column with date.
/// </summary>
public class PnfColumn
{
  /// <summary>First date of column.</summary>
  private readonly DateTime? date = null;

  /// <summary>Content of column with X or O.</summary>
  private readonly StringBuilder sb = new();

  /// <summary>Content of column with X or O or month letter (1-C).</summary>
  private readonly StringBuilder sbm = new();

  /// <summary>Minimal price in column.</summary>
  private decimal? min = null;

  /// <summary>Maximal price in column.</summary>
  private decimal? max = null;

  /// <summary>
  /// Initializes a new instance of the <see cref="PnfColumn"/> class.
  /// </summary>
  /// <param name="min">Minimal price.</param>
  /// <param name="max">Maximal price.</param>
  /// <param name="boxtype">Affected box type.</param>
  /// <param name="anzahl">Number of boxes.</param>
  /// <param name="date">First date.</param>
  public PnfColumn(decimal min, decimal max, int boxtype, int anzahl, PnfDate date)
  {
    this.min = min;
    this.max = max;
    Boxtype = boxtype;
    this.date = date.Date;
    for (int i = 0; i < anzahl; i++)
    {
      Draw(date);
    }
  }

  /// <summary>
  /// Gets a value indicating whether it is a O or X column.
  /// </summary>
  public bool IsO
  {
    get
    {
      if (sb.Length <= 0)
      {
        return Boxtype == 2;
      }
      char eins = sb[0];
      return eins == 'O';
    }
  }

  /// <summary>
  /// Gets the column size.
  /// </summary>
  public int Size
  {
    get { return sb.Length; }
  }

  /// <summary>
  /// Gets the column as string.
  /// </summary>
  public string String
  {
    get { return sb.ToString(); }
  }

  /// <summary>
  /// Gets columns as character array.
  /// </summary>
  public char[] Chars
  {
    get
    {
      if (sbm.Length <= 0)
      {
        return Array.Empty<char>();
      }
      var array = sbm.ToString().ToCharArray();
      if (Boxtype == 2)
      {
        Array.Reverse(array);
      }
      return array;
    }
  }

  /// <summary>
  /// Gets minimal price.
  /// </summary>
  public decimal Min
  {
    get { return min ?? decimal.MinValue; }
  }

  /// <summary>
  /// Gets maximal price.
  /// </summary>
  public decimal Max
  {
    get { return max ?? decimal.MaxValue; }
  }

  /// <summary>Gets or sets box type: 0 unknown, 1 up X, 2 down O.</summary>
  public int Boxtype { get; set; } = 0;

  /// <summary>
  /// Gets or sets vertical position of column.
  /// </summary>
  public int Ypos { get; set; } = 0;

  /// <summary>
  /// Gets vertical position and Size.
  /// </summary>
  public int Ytop
  {
    get { return Ypos + Size; }
  }

  /// <summary>
  /// Gets first date of column.
  /// </summary>
  public DateTime? Date
  {
    get { return date; }
  }

  /// <summary>
  /// Sets a new minimum. If the price is lower than the current minimum, a O is drawn.
  /// </summary>
  /// <param name="min">Possibly new minimal price.</param>
  /// <param name="date">Possibly date for minimum.</param>
  public void SetMin(decimal min, PnfDate date)
  {
    if (Functions.CompDouble4(this.min, min) > 0)
    {
      this.min = min;
      Draw(date);
    }
  }

  /// <summary>
  /// Appends X or O or month letter to column.
  /// </summary>
  /// <param name="date">Affected date.</param>
  public void Draw(PnfDate date)
  {
    char c = Boxtype == 1 ? 'X' : 'O';
    char cm = date.GetNewMonth(c);
    sb.Append(c);
    sbm.Append(cm);
    date.SetUsedMonth();
  }

  /// <summary>
  /// Sets a new maximum. If the price is higher than the current maximum, a X is drawn.
  /// </summary>
  /// <param name="max">Possibly new maximal price.</param>
  /// <param name="date">Possibly date for maximum.</param>
  public void SetMax(decimal max, PnfDate date)
  {
    if (Functions.CompDouble4(this.max, max) < 0)
    {
      this.max = max;
      Draw(date);
    }
  }
}
