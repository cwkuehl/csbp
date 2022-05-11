// <copyright file="Searching.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Server
{
  using System;
  using System.Text;

  /// <summary>Suche im StringBuilder nach dem Knuth-Morris-Pratt-Algorithmus.
  /// Siehe https://stackoverflow.com/questions/12261344/fastest-search-method-in-stringbuilder
  /// </summary>
  public static class Searching
  {
    public static bool Contains(this StringBuilder haystack, string needle)
    {
      return haystack.IndexOf(needle) != -1;
    }

    public static int IndexOf(this StringBuilder haystack, string needle, int startIndex = 0)
    {
      if (haystack == null || needle == null || startIndex < 0)
        throw new ArgumentNullException();
      if (needle.Length == 0)
        return 0; // empty strings are everywhere!
      if (needle.Length == 1) // can't beat just spinning through for it
      {
        var c = needle[0];
        for (var idx = startIndex; idx != haystack.Length; ++idx)
          if (haystack[idx] == c)
            return idx;
        return -1;
      }
      int m = startIndex;
      int i = 0;
      int[] T = KMPTable(needle);
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
          m = m + i - T[i];
          i = T[i] > -1 ? T[i] : 0;
        }
      }
      return -1;
    }

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

    public static int IndexOf(this byte[] haystack, byte[] needle)
    {
      if (haystack == null || needle == null)
        throw new ArgumentNullException();
      if (needle.Length == 0)
        return 0; // empty strings are everywhere!
      if (needle.Length == 1) // can't beat just spinning through for it
      {
        var c = needle[0];
        for (var idx = 0; idx != haystack.Length; ++idx)
          if (haystack[idx] == c)
            return idx;
        return -1;
      }
      int m = 0;
      int i = 0;
      int[] T = KMPTable(needle);
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
          m = m + i - T[i];
          i = T[i] > -1 ? T[i] : 0;
        }
      }
      return -1;
    }

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
}
