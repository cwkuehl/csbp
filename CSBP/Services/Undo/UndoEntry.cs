// <copyright file="UndoEntry.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Undo
{
  using CSBP.Base;

  public class UndoEntry
  {
    public ModelBase Original { get; private set; }

    public ModelBase Actual { get; private set; }

    public bool IsInsert
    {
      get { return Original == null; }
    }

    public bool IsUpdate
    {
      get { return Original != null && Actual != null; }
    }

    public bool IsDelete
    {
      get { return Actual == null; }
    }

    public static UndoEntry Insert(ModelBase e)
    {
      return new UndoEntry
      {
        Original = null,
        Actual = e
      };
    }

    public static UndoEntry Update(ModelBase original, ModelBase actual)
    {
      return new UndoEntry
      {
        Original = original,
        Actual = actual
      };
    }

    public static UndoEntry Delete(ModelBase e)
    {
      return new UndoEntry
      {
        Original = e,
        Actual = null,
      };
    }
  }
}
