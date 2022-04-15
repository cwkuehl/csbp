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
  using CSBP.Services.Factory;

  public class UiTools
  {
    /// <summary>
    /// Save bytes a file in the temp folder and optional open it.
    /// </summary>
    /// <param name="bytes">Content as bytes.</param>
    /// <param name="name">Main part of the file name.</param>
    /// <param name="daterandom">Add date and random number to file name?</param>
    /// <param name="ext">Affected file extension.</param>
    /// <param name="open">Open saved file?</param>
    public static void SaveFile(byte[] bytes, string name, bool daterandom = true,
        string ext = "html", bool open = true)
    {
      if (bytes == null || string.IsNullOrEmpty(name))
        return;
      var fn = Path.Combine(Parameter.TempPath,
          Functions.GetDateiname(name, daterandom, daterandom, ext));
      File.WriteAllBytes(fn, bytes);
      FactoryService.ClientService.CommitFile(fn); // Put file into the undo stack.
      if (open)
        StartFile(fn);
    }

    /// <summary>
    /// Save lines a file in a folder and optional open it.
    /// </summary>
    /// <param name="lines">Content as lines.</param>
    /// <param name="path">Path to file can be empty or a full path with file name.</param>
    /// <param name="file">Main part of the file name.</param>
    /// <param name="daterandom">Add date and random number to file name?</param>
    /// <param name="ext">Affected file extension.</param>
    /// <param name="open">Open saved file?</param>
    public static void SaveFile(List<string> lines, string path, string file = null, bool daterandom = false,
        string ext = null, bool open = true)
    {
      if (lines == null)
        return;
      if (string.IsNullOrEmpty(path) && string.IsNullOrEmpty(file))
        return;
      if (!string.IsNullOrWhiteSpace(file) && !string.IsNullOrWhiteSpace(ext))
        file = Functions.GetDateiname(file, daterandom, daterandom, ext);
      var fn = string.IsNullOrEmpty(path) ? file : string.IsNullOrEmpty(file)
          ? path : Path.Combine(path ?? "", file);
      File.WriteAllLines(fn, lines);
      FactoryService.ClientService.CommitFile(fn); // Put file into the undo stack.
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
    /// <param name="args">Optional argument.</param>
    public static void StartFile(string fn, string args = null)
    {
      var process = new Process();
      process.StartInfo.UseShellExecute = true;
      process.StartInfo.FileName = fn;
      if (args != null)
        process.StartInfo.Arguments = args;
      process.Start();
    }
  }
}
