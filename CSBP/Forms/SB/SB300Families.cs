// <copyright file="SB300Families.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.SB
{
  using System;
  using System.Collections.Generic;
  using CSBP.Apis.Enums;
  using CSBP.Apis.Models;
  using CSBP.Base;
  using CSBP.Services.Factory;
  using Gtk;
  using static CSBP.Resources.M;
  using static CSBP.Resources.Messages;

  /// <summary>Controller für SB300Families Dialog.</summary>
  public partial class SB300Families : CsbpBin
  {
#pragma warning disable CS0649

    /// <summary>Button RefreshAction.</summary>
    [Builder.Object]
    private Button refreshAction;

    /// <summary>Button UndoAction.</summary>
    [Builder.Object]
    private Button undoAction;

    /// <summary>Button RedoAction.</summary>
    [Builder.Object]
    private Button redoAction;

    /// <summary>Button NewAction.</summary>
    [Builder.Object]
    private Button newAction;

    /// <summary>Button CopyAction.</summary>
    [Builder.Object]
    private Button copyAction;

    /// <summary>Button EditAction.</summary>
    [Builder.Object]
    private Button editAction;

    /// <summary>Button DeleteAction.</summary>
    [Builder.Object]
    private Button deleteAction;

    /// <summary>Label familien0.</summary>
    [Builder.Object]
    private Label familien0;

    /// <summary>TreeView familien.</summary>
    [Builder.Object]
    private TreeView familien;

    /// <summary>Label springen0.</summary>
    [Builder.Object]
    private Label springen0;

    /// <summary>Button spVater.</summary>
    [Builder.Object]
    private Button spVater;

    /// <summary>Button spMutter.</summary>
    [Builder.Object]
    private Button spMutter;

    /// <summary>Button spKind.</summary>
    [Builder.Object]
    private Button spKind;

#pragma warning restore CS0649

    /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public static SB300Families Create(object p1 = null, CsbpBin p = null)
    {
      return new SB300Families(GetBuilder("SB300Families", out var handle), handle, p1: p1, p: p);
    }

    /// <summary>Konstruktor für modalen Dialog.</summary>
    /// <param name="builder">Betroffener Builder.</param>
    /// <param name="h">Betroffenes Handle vom Builder.</param>
    /// <param name="d">Betroffener einbettender Dialog.</param>
    /// <param name="dt">Betroffener Dialogtyp.</param>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public SB300Families(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
        : base(b, h, d, dt, p1, p)
    {
      // SetBold(client0);
      InitData(0);
      familien.GrabFocus();
    }

    /**
     * Event für SpElternFamilie.
     */
    public void OnSpElternFamilie(string uid)
    {
      if (!string.IsNullOrEmpty(uid))
      {
        var r = Get(FactoryService.PedigreeService.GetFamily(ServiceDaten, null, uid));
        if (r == null)
          MainClass.MainWindow.SetError(M0(SB027));
        else
        {
          familien.GrabFocus();
          SetText(familien, r.Uid);
        }
      }
    }

    /**
     * Event für SpFamilienKind.
     */
    public void OnSpFamilienKind(string uid)
    {
      if (!string.IsNullOrEmpty(uid))
      {
        var r = Get(FactoryService.PedigreeService.GetFamily(ServiceDaten, null, null, uid));
        if (r == null)
          MainClass.MainWindow.SetError(M0(SB027));
        else
        {
          familien.GrabFocus();
          SetText(familien, r.Uid);
        }
      }
    }

    /// <summary>Model-Daten initialisieren.</summary>
    /// <param name="step">Betroffener Schritt: 0 erstmalig, 1 aktualisieren.</param>
    protected override void InitData(int step)
    {
      var daten = ServiceDaten;
      if (step <= 1)
      {
        var l = Get(FactoryService.PedigreeService.GetFamilyList(daten)) ?? new List<SbFamilie>();
        var values = new List<string[]>();
        foreach (var e in l)
        {
          // Nr.;Vater;Mutter;Geändert am;Geändert von;Angelegt am;Angelegt von
          values.Add(new string[] { e.Uid, e.Father?.AncestorName ?? "", e.Mother?.AncestorName ?? "",
            Functions.ToString(e.Geaendert_Am, true), e.Geaendert_Von,
            Functions.ToString(e.Angelegt_Am, true), e.Angelegt_Von });
        }
        AddStringColumnsSort(familien, SB300_familien_columns, values);
      }
    }

    /// <summary>Aktualisierung des Eltern-Dialogs.</summary>
    protected override void UpdateParent()
    {
      refreshAction.Click();
    }

    /// <summary>Behandlung von Refresh.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnRefreshClicked(object sender, EventArgs e)
    {
      RefreshTreeView(familien, 1);
    }

    /// <summary>Behandlung von Undo.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnUndoClicked(object sender, EventArgs e)
    {
      if (MainClass.Undo())
        refreshAction.Click();
    }

    /// <summary>Behandlung von Redo.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnRedoClicked(object sender, EventArgs e)
    {
      if (MainClass.Redo())
        refreshAction.Click();
    }

    /// <summary>Behandlung von New.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnNewClicked(object sender, EventArgs e)
    {
      StartDialog(DialogTypeEnum.New);
    }

    /// <summary>Behandlung von Copy.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnCopyClicked(object sender, EventArgs e)
    {
      StartDialog(DialogTypeEnum.Copy);
    }

    /// <summary>Behandlung von Edit.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnEditClicked(object sender, EventArgs e)
    {
      StartDialog(DialogTypeEnum.Edit);
    }

    /// <summary>Behandlung von Delete.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnDeleteClicked(object sender, EventArgs e)
    {
      StartDialog(DialogTypeEnum.Delete);
    }

    /// <summary>Behandlung von Familien.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnFamilienRowActivated(object sender, RowActivatedArgs e)
    {
      editAction.Activate();
    }

    /// <summary>Behandlung von Spvater.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnSpvaterClicked(object sender, EventArgs e)
    {
      var uid = GetValue<string>(familien, false);
      if (!string.IsNullOrEmpty(uid))
      {
        var f = Get(FactoryService.PedigreeService.GetFamily(ServiceDaten, uid));
        if (f != null)
        {
          var dlg = Focus<SB200Ancestors>(SB200_title);
          if (dlg != null)
            dlg.OnSpAhn(f.Mann_Uid);
        }
      }
    }

    /// <summary>Behandlung von Spmutter.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnSpmutterClicked(object sender, EventArgs e)
    {
      var uid = GetValue<string>(familien, false);
      if (!string.IsNullOrEmpty(uid))
      {
        var f = Get(FactoryService.PedigreeService.GetFamily(ServiceDaten, uid));
        if (f != null)
        {
          var dlg = Focus<SB200Ancestors>(SB200_title);
          if (dlg != null)
            dlg.OnSpAhn(f.Frau_Uid);
        }
      }
    }

    /// <summary>Behandlung von Spkind.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnSpkindClicked(object sender, EventArgs e)
    {
      var uid = GetValue<string>(familien, false);
      if (!string.IsNullOrEmpty(uid))
      {
        var cuid = Get(FactoryService.PedigreeService.GetFirstChild(ServiceDaten, null, uid));
        if (!string.IsNullOrEmpty(cuid))
        {
          var dlg = Focus<SB200Ancestors>(SB200_title);
          if (dlg != null)
            dlg.OnSpAhn(cuid);
        }
      }
    }

    /// <summary>Starten des Details-Dialogs.</summary>
    /// <param name="dt">Betroffener Dialog-Typ.</param>
    void StartDialog(DialogTypeEnum dt)
    {
      var uid = GetValue<string>(familien, dt != DialogTypeEnum.New);
      Start(typeof(SB310Family), SB310_title, dt, uid, csbpparent: this);
    }
  }
}
