// <copyright file="CsbpBase.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Base;

/// <summary>
/// Static functions for user interface.
/// </summary>
public class CsbpBase
{
  /// <summary>
  /// Is the shortcut to be ignored for stock calculation or not.
  /// </summary>
  /// <param name="shortcut">Affected shortcut for provider.</param>
  /// <returns>Is the shortcut to be ignored for calculation or not.</returns>
  public static bool IgnoreShortcut(string shortcut)
  {
    return string.IsNullOrEmpty(shortcut) || shortcut == "0" || shortcut == "xxx";
  }

  /// <summary>
  /// Get readable stock state.
  /// </summary>
  /// <param name="state">Affected state.</param>
  /// <param name="shortcut">Affected shortcut for provider.</param>
  /// <returns>Readable stock state.</returns>
  public static string GetStockState(string state, string shortcut = null)
  {
    return state switch
    {
      "1" => IgnoreShortcut(shortcut) ? "N" : "A",
      "2" => "N",
      _ => "I",
    };
  }

  /// <summary>
  /// Liefert den Suchtext für eine Like-Suche: aus * wird % und falls kein %, wird % angehängt.
  /// </summary>
  /// <param name="s">Betroffener Suchstring.</param>
  /// <returns>Suchtext für eine Like-Suche.</returns>
  public static string GetSuche(string s)
  {
    if (string.IsNullOrEmpty(s))
      return "";
    var st = s.Replace("*", "%");
    if (!st.Contains("%"))
      st += "%";
    return st;
  }

  /// <summary>
  /// Checks if it is a filtering like expression. Empty, % and %% are not.
  /// </summary>
  /// <param name="t">Affected like expression.</param>
  /// <returns>It is a filtering like expression or not.</returns>
  public static bool IsLike(string t)
  {
    return !(string.IsNullOrEmpty(t) || t == "%" || t == "%%");
  }
}
