// <copyright file="UiFunctions.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Base;

/// <summary>
/// Static functions for user interface.
/// </summary>
public class UiFunctions
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
}
