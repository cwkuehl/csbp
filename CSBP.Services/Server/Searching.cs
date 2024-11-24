// <copyright file="Searching.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Server;

using System;
using System.Text;

/// <summary>Searches in StringBuilder or byte arrays according to Knuth Morris Pratt algorithm.
/// See https://stackoverflow.com/questions/12261344/fastest-search-method-in-stringbuilder.
/// </summary>
public static class Searching
{
  /// <summary>
  /// Checks whether haystack contains needle or not.
  /// </summary>
  /// <param name="haystack">Affected haystack string.</param>
  /// <param name="needle">Affected needle string.</param>
  /// <returns>Haystack contains needle or not.</returns>
  public static bool Contains(this StringBuilder haystack, string needle)
  {
    return haystack.IndexOf(needle) != -1;
  }

  /// <summary>
  /// Gets index of needle in haystack.
  /// </summary>
  /// <param name="haystack">Affected haystack string.</param>
  /// <param name="needle">Affected needle string.</param>
  /// <param name="startIndex">Affected start index.</param>
  /// <returns>Index or -1.</returns>
  public static int IndexOf(this StringBuilder haystack, string needle, int startIndex = 0)
  {
    if (haystack == null || needle == null || startIndex < 0)
      throw new ArgumentNullException(nameof(haystack));
    if (needle.Length == 0)
      return 0; // empty strings are everywhere!
    if (needle.Length == 1)
    {
      // can't beat just spinning through for it
      var c = needle[0];
      for (var idx = startIndex; idx != haystack.Length; ++idx)
        if (haystack[idx] == c)
          return idx;
      return -1;
    }
    int m = startIndex;
    int i = 0;
    int[] t = KMPTable(needle);
    while (m + i < haystack.Length)
    {
      if (needle[i] == haystack[m + i])
      {
        if (i == needle.Length - 1)
          return m == needle.Length ? -1 : m; // match -1 = failure to find conventional in .NET
        ++i;
      }
      else
      {
        m = m + i - t[i];
        i = t[i] > -1 ? t[i] : 0;
      }
    }
    return -1;
  }

  /// <summary>
  /// Gets index of byte needle in byte haystack.
  /// </summary>
  /// <param name="haystack">Affected haystack bytes.</param>
  /// <param name="needle">Affected needle bytes.</param>
  /// <returns>Index of -1.</returns>
  public static int IndexOf(this byte[] haystack, byte[] needle)
  {
    if (haystack == null || needle == null)
      throw new ArgumentNullException(nameof(haystack));
    if (needle.Length == 0)
      return 0; // empty strings are everywhere!
    if (needle.Length == 1)
    {
      // can't beat just spinning through for it
      var c = needle[0];
      for (var idx = 0; idx != haystack.Length; ++idx)
        if (haystack[idx] == c)
          return idx;
      return -1;
    }
    int m = 0;
    int i = 0;
    int[] t = KMPTable(needle);
    while (m + i < haystack.Length)
    {
      if (needle[i] == haystack[m + i])
      {
        if (i == needle.Length - 1)
          return m == needle.Length ? -1 : m; // match -1 = failure to find conventional in .NET
        ++i;
      }
      else
      {
        m = m + i - t[i];
        i = t[i] > -1 ? t[i] : 0;
      }
    }
    return -1;
  }

  /// <summary>
  /// Get KMP table from string.
  /// </summary>
  /// <param name="sought">Affected string.</param>
  /// <returns>KMP table.</returns>
  private static int[] KMPTable(string sought)
  {
    int[] table = new int[sought.Length];
    int pos = 2;
    int cnd = 0;
    table[0] = -1;
    table[1] = 0;
    while (pos < table.Length)
      if (sought[pos - 1] == sought[cnd])
        table[pos++] = ++cnd;
      else if (cnd > 0)
        cnd = table[cnd];
      else
        table[pos++] = 0;
    return table;
  }

  /// <summary>
  /// Get KMP table from bytes.
  /// </summary>
  /// <param name="sought">Affected bytes.</param>
  /// <returns>KMP table.</returns>
  private static int[] KMPTable(byte[] sought)
  {
    int[] table = new int[sought.Length];
    int pos = 2;
    int cnd = 0;
    table[0] = -1;
    table[1] = 0;
    while (pos < table.Length)
      if (sought[pos - 1] == sought[cnd])
        table[pos++] = ++cnd;
      else if (cnd > 0)
        cnd = table[cnd];
      else
        table[pos++] = 0;
    return table;
  }
}
