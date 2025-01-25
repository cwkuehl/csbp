// <copyright file="UiTools.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms;

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CSBP.Base;
using CSBP.Services.Base;
using CSBP.Services.Factory;

/// <summary>
/// Static functions for user interface interaction.
/// </summary>
public class UiTools
{
  /// <summary>Affected only session id for program instance.</summary>
  public const string SessionId = "0";

  /// <summary>
  /// Save bytes a file in the temp folder and optional open it.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="bytes">Content as bytes.</param>
  /// <param name="name">Main part of the file name.</param>
  /// <param name="daterandom">Add date and random number to file name or not.</param>
  /// <param name="ext">Affected file extension.</param>
  /// <param name="open">Open saved file or not.</param>
  public static void SaveFile(ServiceDaten daten, byte[] bytes, string name, bool daterandom = true,
      string ext = "html", bool open = true)
  {
    if (bytes == null || string.IsNullOrEmpty(name))
      return;
    var fn = Path.Combine(ParameterGui.TempPath, Functions.GetDateiname(name, daterandom, daterandom, ext));
    File.WriteAllBytes(fn, bytes);
    FactoryService.ClientService.CommitFile(daten, fn); // Put file into the undo stack.
    if (open)
      StartFile(fn);
  }

  /// <summary>
  /// Save lines a file in a folder and optional open it.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="lines">Content as lines.</param>
  /// <param name="path">Path to file can be empty or a full path with file name.</param>
  /// <param name="file">Main part of the file name.</param>
  /// <param name="daterandom">Add date and random number to file name or not.</param>
  /// <param name="ext">Affected file extension.</param>
  /// <param name="open">Open saved file or not.</param>
  public static void SaveFile(ServiceDaten daten, List<string> lines, string path, string file = null, bool daterandom = false,
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
    FactoryService.ClientService.CommitFile(daten, fn); // Put file into the undo stack.
    if (open)
      StartFile(fn);
  }

  /// <summary>
  /// Read file into list of strings.
  /// </summary>
  /// <param name="path">Affected path, maybe with file name.</param>
  /// <param name="file">Affected file name, maybe empty.</param>
  /// <returns>List of strings.</returns>
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

  /// <summary>
  /// Update undo/redo size in the tooltip text.
  /// </summary>
  /// <param name="undoAction">Affected undo button.</param>
  /// <param name="redoAction">Affected redo button.</param>
  public static void UpdateUndoRedoSize(Gtk.Button undoAction, Gtk.Button redoAction)
  {
    var c = ServiceBase.GetUndoRedoSize(SessionId);
    if (undoAction != null)
    {
      var tt = undoAction.TooltipText ?? "";
      tt = Functions.Between(tt, null, " (") ?? tt;
      tt = $"{tt} ({c.Item1})";
      undoAction.TooltipText = tt;
    }
    if (redoAction != null)
    {
      var tt = redoAction.TooltipText ?? "";
      tt = Functions.Between(tt, null, " (") ?? tt;
      tt = $"{tt} ({c.Item2})";
      redoAction.TooltipText = tt;
    }
  }
}
