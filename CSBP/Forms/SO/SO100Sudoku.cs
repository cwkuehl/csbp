// <copyright file="SO100Sudoku.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.SO
{
  using System;
  using CSBP.Apis.Enums;
  using CSBP.Apis.Models;
  using CSBP.Apis.Services;
  using CSBP.Base;
  using CSBP.Forms.Controls;
  using CSBP.Services.Factory;
  using Gtk;
  using static CSBP.Resources.M;
  using static CSBP.Resources.Messages;

  /// <summary>Controller für SO100Sudoku Dialog.</summary>
  public partial class SO100Sudoku : CsbpBin
  {
    /// <summary>Dialog Model.</summary>
    private MaMandant Model;

#pragma warning disable 169, 649

    /// <summary>Button RefreshAction.</summary>
    [Builder.Object]
    private Button refreshAction;

    /// <summary>Button UndoAction.</summary>
    [Builder.Object]
    private Button undoAction;

    /// <summary>Button SaveAction.</summary>
    [Builder.Object]
    private Button saveAction;

    /// <summary>Button DeleteAction.</summary>
    [Builder.Object]
    private Button deleteAction;

    /// <summary>Label sudoku0.</summary>
    [Builder.Object]
    private Label sudoku0;

    /// <summary>Label anzahl.</summary>
    [Builder.Object]
    private Label anzahl;

    /// <summary>Button zug.</summary>
    [Builder.Object]
    private Button zug;

    /// <summary>Button loesen.</summary>
    [Builder.Object]
    private Button loesen;

    /// <summary>Button test.</summary>
    [Builder.Object]
    private Button test;

    /// <summary>CheckButton diagonal.</summary>
    [Builder.Object]
    private CheckButton diagonal;

    /// <summary>Label leery.</summary>
    [Builder.Object]
    private Label leery;

#pragma warning restore 169, 649

    /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public static SO100Sudoku Create(object p1 = null, CsbpBin p = null)
    {
      return new SO100Sudoku(GetBuilder("SO100Sudoku", out var handle), handle, p1: p1, p: p);
    }

    /// <summary>Konstruktor für modalen Dialog.</summary>
    /// <param name="builder">Betroffener Builder.</param>
    /// <param name="h">Betroffenes Handle vom Builder.</param>
    /// <param name="d">Betroffener einbettender Dialog.</param>
    /// <param name="dt">Betroffener Dialogtyp.</param>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public SO100Sudoku(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
        : base(b, h, d, dt, p1, p)
    {
      // SetBold(client0);
      Functions.MachNichts(Model);
      InitData(0);
    }

    /// <summary>Model-Daten initialisieren.</summary>
    /// <param name="step">Betroffener Schritt: 0 erstmalig, 1 aktualisieren.</param>
    override protected void InitData(int step)
    {
      if (step <= 0)
      {
      }
    }

    /// <summary>Behandlung von Refresh.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnRefreshClicked(object sender, EventArgs e)
    {
    }

    /// <summary>Behandlung von Undo.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnUndoClicked(object sender, EventArgs e)
    {
    }

    /// <summary>Behandlung von Save.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnSaveClicked(object sender, EventArgs e)
    {
    }

    /// <summary>Behandlung von Delete.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnDeleteClicked(object sender, EventArgs e)
    {
    }

    /// <summary>Behandlung von Zug.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnZugClicked(object sender, EventArgs e)
    {
    }

    /// <summary>Behandlung von Loesen.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnLoesenClicked(object sender, EventArgs e)
    {
    }

    /// <summary>Behandlung von Test.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnTestClicked(object sender, EventArgs e)
    {
    }
  }
}
