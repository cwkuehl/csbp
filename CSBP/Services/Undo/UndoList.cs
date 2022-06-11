// <copyright file="UndoList.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Undo;

using System.Collections.Generic;
using CSBP.Base;

/// <summary>
/// List of undo entry.
/// </summary>
public class UndoList
{
  /// <summary>Gets list of undo entries.</summary>
  public List<UndoEntry> List { get; private set; } = new List<UndoEntry>();

  /// <summary>
  /// Add an insert entry.
  /// </summary>
  /// <param name="e">Affected model base.</param>
  public void Insert(ModelBase e)
  {
    List.Add(UndoEntry.Insert(e));
  }

  /// <summary>
  /// Add an update entry.
  /// </summary>
  /// <param name="original">Original value.</param>
  /// <param name="actual">Actual value.</param>
  public void Update(ModelBase original, ModelBase actual)
  {
    List.Add(UndoEntry.Update(original, actual));
  }

  /// <summary>
  /// Add a delete entry.
  /// </summary>
  /// <param name="e">Affected model base.</param>
  public void Delete(ModelBase e)
  {
    List.Add(UndoEntry.Delete(e));
  }
}
