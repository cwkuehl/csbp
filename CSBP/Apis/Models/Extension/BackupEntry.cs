// <copyright file="BackupEntry.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models.Extension;

using System;
using System.Collections.Generic;
using CSBP.Base;

/// <summary>
/// Backup entry.
/// </summary>
[Serializable]
public class BackupEntry : ModelBase
{
  /// <summary>Gets or sets the uid.</summary>
  public string Uid { get; set; }

  /// <summary>Gets or sets the source directories.</summary>
  public string[] Sources { get; set; }

  /// <summary>Gets or sets the target directory.</summary>
  public string Target { get; set; }

  /// <summary>Gets or sets a value indicating whether to encrypt or not.</summary>
  public bool Encrypted { get; set; }

  /// <summary>Gets or sets a value indicating whether to zip or not.</summary>
  public bool Zipped { get; set; }

  /// <summary>Gets or sets the value of column Angelegt_Von.</summary>
  public string Angelegt_Von { get; set; }

  /// <summary>Gets or sets the value of column Angelegt_Am.</summary>
  public DateTime? Angelegt_Am { get; set; }

  /// <summary>Gets or sets the value of column Geaendert_Von.</summary>
  public string Geaendert_Von { get; set; }

  /// <summary>Gets or sets the value of column Geaendert_Am.</summary>
  public DateTime? Geaendert_Am { get; set; }

  /// <summary>Gets the source as a string.</summary>
  public string SourcesText
  {
    get
    {
      return Sources == null ? string.Empty : string.Join(";", Sources);
    }
  }

  /// <summary>
  /// Splits the sources.
  /// </summary>
  /// <returns>The sources.</returns>
  /// <param name="sources">Splitted sources.</param>
  public static string[] SplitSources(string sources)
  {
    if (string.IsNullOrWhiteSpace(sources))
      return Array.Empty<string>();
    return sources.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
  }

  /// <summary>
  /// Gets list with backup entries.
  /// </summary>
  /// <returns>List with backup entries.</returns>
  public static List<BackupEntry> GetBackupEntryList()
  {
    var v = Parameter.GetValue("Backups") ?? "";
    var bytes = Convert.FromBase64String(v);
    List<BackupEntry> l = null;
    if (bytes.Length > 0)
      try
      {
        l = Functions.Deserialize<List<BackupEntry>>(bytes);
      }
      catch (Exception)
      {
        Functions.MachNichts();
      }
    if (l == null)
      l = new List<BackupEntry>();
    return l;
  }

  /// <summary>
  /// Saves list with backup entries.
  /// </summary>
  /// <param name="l">List with backup entries.</param>
  public static void SaveBackupEntryList(List<BackupEntry> l)
  {
    var v = string.Empty;
    if (l != null && l.Count > 0)
    {
      var bytes = Functions.Serialize(l);
      v = Convert.ToBase64String(bytes);
    }
    Parameter.SetValue("Backups", v);
  }
}
