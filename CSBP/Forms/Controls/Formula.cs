// <copyright file="Formula.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.Controls;

using System.Text.RegularExpressions;
using CSBP.Base;

/// <summary>
///
/// </summary>
public class Formula
{
  /// <summary>Number of column the formula is in.</summary>
  public int column { get; set; }

  /// <summary>Number of row the formula is in.</summary>
  public int row { get; set; }

  /// <summary>Formula as string.</summary>
  public string formula { get; private set; }

  /// <summary>Affected function.</summary>
  public string function { get; private set; }

  /// <summary>Value as string.</summary>
  public string value { get; set; }

  /// <summary>Is formula bold?</summary>
  private bool bold;

  /// <summary>Regex for formula today.</summary>
  private static Regex RxToday = new Regex(@"^=(today)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

  /// <summary>Regex for formula today.</summary>
  private static Regex RxNow = new Regex(@"^=(now)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

  /// <summary>
  /// Private constructor.
  /// </summary>
  /// <param name="formula">Affected formula.</param>
  /// <param name="c">Affected column.</param>
  /// <param name="r">Affected row.</param>
  /// <param name="function">Affected function.</param>
  /// <param name="bold">Is formula bold?</param>
  private Formula(string formula, int c, int r, string function, bool bold)
  {
    this.formula = formula;
    this.column = c;
    this.row = r;
    this.function = function;
    this.bold = bold;
  }

  /// <summary>
  /// Create a Formula instance, if it is a formula.
  /// </summary>
  /// <param name="formula">Possible formula.</param>
  /// <param name="c">Affected column.</param>
  /// <param name="r">Affected row.</param>
  /// <returns>Formula instance or null.</returns>
  public static Formula Instance(string formula, int c, int r)
  {
    if (string.IsNullOrWhiteSpace(formula))
      return null;
    var bold = false;
    if (Functions.IsBold(formula))
    {
      bold = true;
      formula = Functions.MakeBold(formula, true);
    }
    var m = RxNow.Match(formula);
    if (m.Success)
    {
      return new Formula(formula, c, r, "now", bold);
    }
    m = RxToday.Match(formula);
    if (m.Success)
    {
      return new Formula(formula, c, r, "today", bold);
    }
    return null;
  }
}
