// <copyright file="UndoEntry.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Undo;

using CSBP.Apis.Models;
using CSBP.Base;

/// <summary>
/// Class with original and actual entry value.
/// </summary>
public class UndoEntry
{
  /// <summary>Get the original value.</summary>
  public ModelBase Original { get; private set; }

  /// <summary>Get the actual value.</summary>
  public ModelBase Actual { get; private set; }

  /// <summary>Is it a file with name and content?</summary>
  public bool IsFileData { get; private set; }

  /// <summary>Is it an insert entry?</summary>
  public bool IsInsert
  {
    get { return Original == null; }
  }

  /// <summary>Is it an update entry?</summary>
  public bool IsUpdate
  {
    get { return Original != null && Actual != null; }
  }

  /// <summary>Is it a delete entry?</summary>
  public bool IsDelete
  {
    get { return Actual == null; }
  }

  /// <summary>Create an insert entry.</summary>
  public static UndoEntry Insert(ModelBase e)
  {
    return new UndoEntry
    {
      Original = null,
      Actual = e,
      IsFileData = e is FileData,
    };
  }

  /// <summary>Create an update entry.</summary>
  public static UndoEntry Update(ModelBase original, ModelBase actual)
  {
    return new UndoEntry
    {
      Original = original,
      Actual = actual,
      IsFileData = original is FileData || actual is FileData
    };
  }

  /// <summary>Create a delete entry.</summary>
  public static UndoEntry Delete(ModelBase e)
  {
    return new UndoEntry
    {
      Original = e,
      Actual = null,
      IsFileData = e is FileData
    };
  }
}
