// <copyright file="UndoList.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Undo
{
  using System.Collections.Generic;
  using CSBP.Base;

  public class UndoList
  {
    public List<UndoEntry> List { get; private set; } = new List<UndoEntry>();

    public void Insert(ModelBase e)
    {
      List.Add(UndoEntry.Insert(e));
    }

    public void Update(ModelBase original, ModelBase actual)
    {
      List.Add(UndoEntry.Update(original, actual));
    }

    public void Delete(ModelBase e)
    {
      List.Add(UndoEntry.Delete(e));
    }
  }
}
