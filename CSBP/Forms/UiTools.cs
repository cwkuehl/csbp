// <copyright file="UiTools.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms
{
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.IO;
  using System.Linq;
  using CSBP.Base;

  public class UiTools
  {
    public static void SaveFile(byte[] bytes, string name, bool daterandom = true,
        string ext = "html", bool open = true)
    {
      if (bytes == null || string.IsNullOrEmpty(name))
        return;
      var fn = Path.Combine(Parameter.TempPath,
          Functions.GetDateiname(name, daterandom, daterandom, ext));
      File.WriteAllBytes(fn, bytes);
      if (open)
        StartFile(fn);
    }

    public static void SaveFile(List<string> lines, string path, string file = null, bool open = false)
    {
      if (lines == null)
        return;
      if (string.IsNullOrEmpty(path) && string.IsNullOrEmpty(file))
        return;
      var fn = string.IsNullOrEmpty(path) ? file : string.IsNullOrEmpty(file)
          ? path : Path.Combine(path ?? "", file);
      File.WriteAllLines(fn, lines);
      if (open)
        StartFile(fn);
    }

    public static List<string> ReadFile(string path, string file = null)
    {
      if (string.IsNullOrEmpty(path) && string.IsNullOrEmpty(file))
        return null;
      var fn = string.IsNullOrEmpty(path) ? file : string.IsNullOrEmpty(file)
          ? path : Path.Combine(path ?? "", file);
      var lines = File.ReadLines(fn);
      return lines.ToList();
    }

    /// <summary>
    /// Opens a file with default application or browser.
    /// </summary>
    /// <param name="fn">Affected file incl. path or URL.</param>
    public static void StartFile(string fn)
    {
      var process = new Process();
      process.StartInfo.UseShellExecute = true;
      process.StartInfo.FileName = fn;
      process.Start();
    }
  }
}
