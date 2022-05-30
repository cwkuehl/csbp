// <copyright file="SB200Ancestors.cs" company="cwkuehl.de">
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

  /// <summary>Controller für SB200Ancestors Dialog.</summary>
  public partial class SB200Ancestors : CsbpBin
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

    /// <summary>Button PrintAction.</summary>
    [Builder.Object]
    private Button printAction;

    /// <summary>Button FloppyAction.</summary>
    [Builder.Object]
    private Button floppyAction;

    /// <summary>Label ahnen0.</summary>
    [Builder.Object]
    private Label ahnen0;

    /// <summary>TreeView ahnen.</summary>
    [Builder.Object]
    private TreeView ahnen;

    /// <summary>Label ahnenStatus.</summary>
    [Builder.Object]
    private Label ahnenStatus;

    /// <summary>Label name0.</summary>
    [Builder.Object]
    private Label name0;

    /// <summary>Entry name.</summary>
    [Builder.Object]
    private Entry name;

    /// <summary>Label vorname0.</summary>
    [Builder.Object]
    private Label vorname0;

    /// <summary>Entry vorname.</summary>
    [Builder.Object]
    private Entry vorname;

    /// <summary>CheckButton filtern.</summary>
    [Builder.Object]
    private CheckButton filtern;

    /// <summary>Button alle.</summary>
    [Builder.Object]
    private Button alle;

    /// <summary>Label springen0.</summary>
    [Builder.Object]
    private Label springen0;

    /// <summary>Button spName.</summary>
    [Builder.Object]
    private Button spName;

    /// <summary>Button spVater.</summary>
    [Builder.Object]
    private Button spVater;

    /// <summary>Button spMutter.</summary>
    [Builder.Object]
    private Button spMutter;

    /// <summary>Button spKind.</summary>
    [Builder.Object]
    private Button spKind;

    /// <summary>Button spEhegatte.</summary>
    [Builder.Object]
    private Button spEhegatte;

    /// <summary>Button spGeschwister.</summary>
    [Builder.Object]
    private Button spGeschwister;

    /// <summary>Button spFamilie.</summary>
    [Builder.Object]
    private Button spFamilie;

    /// <summary>Button spFamilienKind.</summary>
    [Builder.Object]
    private Button spFamilienKind;

#pragma warning restore CS0649

    /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public static SB200Ancestors Create(object p1 = null, CsbpBin p = null)
    {
      return new SB200Ancestors(GetBuilder("SB200Ancestors", out var handle), handle, p1: p1, p: p);
    }

    /// <summary>Konstruktor für modalen Dialog.</summary>
    /// <param name="builder">Betroffener Builder.</param>
    /// <param name="h">Betroffenes Handle vom Builder.</param>
    /// <param name="d">Betroffener einbettender Dialog.</param>
    /// <param name="dt">Betroffener Dialogtyp.</param>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public SB200Ancestors(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
        : base(b, h, d, dt, p1, p)
    {
      ObservableEventThrottle(refreshAction, delegate { RefreshTreeView(ahnen, 1); });
      // SetBold(client0);
      InitData(0);
      ahnen.GrabFocus();
    }

    /**
     * Event für SpAhn.
     */
    public void OnSpAhn(string uid)
    {
      SetText(ahnen, uid);
    }

    /// <summary>Model-Daten initialisieren.</summary>
    /// <param name="step">Betroffener Schritt: 0 erstmalig, 1 aktualisieren.</param>
    protected override void InitData(int step)
    {
      var daten = ServiceDaten;
      if (step <= 0)
      {
        EventsActive = false;
        name.Text = "%%";
        vorname.Text = "%%";
        filtern.Active = false;
        EventsActive = true;
      }
      if (step <= 1)
      {
        var l = Get(FactoryService.PedigreeService.GetAncestorList(daten, filtern.Active ? name.Text : null,
          filtern.Active ? vorname.Text : null)) ?? new List<SbPerson>();
        var anz = 0;
        var anzg = 0;
        var values = new List<string[]>();
        foreach (var e in l)
        {
          // Nr.;Geburtsname;Vornamen;Name;G.;Geboren;Gestorben;Geändert am;Geändert von;Angelegt am;Angelegt von
          values.Add(new string[] { e.Uid, e.Geburtsname, e.Vorname, e.Name, e.Geschlecht, e.Birthdate, e.Deathdate,
            Functions.ToString(e.Geaendert_Am, true), e.Geaendert_Von,
            Functions.ToString(e.Angelegt_Am, true), e.Angelegt_Von });
          anz++;
          if (string.IsNullOrEmpty(e.Birthdate))
            anzg++;
        }
        AddStringColumnsSort(ahnen, SB200_ahnen_columns, values);
        ahnenStatus.Text = SB028(anz, anzg);
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
      // RefreshTreeView(ahnen, 1);
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

    /// <summary>Behandlung von Print.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnPrintClicked(object sender, EventArgs e)
    {
      Start(typeof(SB220Print), SB220_title, csbpparent: this);
    }

    /// <summary>Behandlung von Floppy.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnFloppyClicked(object sender, EventArgs e)
    {
      Start(typeof(SB500Gedcom), SB500_title, csbpparent: this);
    }

    /// <summary>Behandlung von Ahnen.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnAhnenRowActivated(object sender, RowActivatedArgs e)
    {
      editAction.Activate();
    }

    /// <summary>Behandlung von Name.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnNameKeyReleaseEvent(object o, KeyReleaseEventArgs e)
    {
      if (!EventsActive || !filtern.Active)
        return;
      refreshAction.Click();
    }

    /// <summary>Behandlung von Vorname.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnVornameKeyReleaseEvent(object o, KeyReleaseEventArgs e)
    {
      if (!EventsActive || !filtern.Active)
        return;
      refreshAction.Click();
    }

    /// <summary>Behandlung von Filtern.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnFiltern(object o, EventArgs e)
    {
      if (!EventsActive)
        return;
      refreshAction.Click();
    }

    /// <summary>Behandlung von Alle.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnAlleClicked(object sender, EventArgs e)
    {
      RefreshTreeView(ahnen, 0);
    }

    /// <summary>Behandlung von Spname.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnSpnameClicked(object sender, EventArgs e)
    {
      var uid = GetValue<string>(ahnen, false);
      var r = Get(FactoryService.PedigreeService.GetNextAncestorName(ServiceDaten, uid, name.Text, vorname.Text));
      if (!string.IsNullOrEmpty(r))
        SetText(ahnen, r);
    }

    /// <summary>Behandlung von Spvater.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnSpvaterClicked(object sender, EventArgs e)
    {
      var r = GetAncestor();
      if (r != null && r.Father != null)
        SetText(ahnen, r.Father.Uid);
    }

    /// <summary>Behandlung von Spmutter.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnSpmutterClicked(object sender, EventArgs e)
    {
      var r = GetAncestor();
      if (r != null && r.Mother != null)
        SetText(ahnen, r.Mother.Uid);
    }

    /// <summary>Behandlung von Spkind.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnSpkindClicked(object sender, EventArgs e)
    {
      var uid = GetValue<string>(ahnen, false);
      var r = Get(FactoryService.PedigreeService.GetFirstChild(ServiceDaten, uid));
      if (!string.IsNullOrEmpty(r))
        SetText(ahnen, r);
    }

    /// <summary>Behandlung von Spehegatte.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnSpehegatteClicked(object sender, EventArgs e)
    {
      var uid = GetValue<string>(ahnen, false);
      var r = Get(FactoryService.PedigreeService.GetNextSpouse(ServiceDaten, uid));
      if (r != null)
        SetText(ahnen, r.Uid);
    }

    /// <summary>Behandlung von Spgeschwister.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnSpgeschwisterClicked(object sender, EventArgs e)
    {
      var uid = GetValue<string>(ahnen, false);
      var r = Get(FactoryService.PedigreeService.GetNextSibling(ServiceDaten, uid));
      if (r != null)
        SetText(ahnen, r.Uid);
    }

    /// <summary>Behandlung von Spfamilie.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnSpfamilieClicked(object sender, EventArgs e)
    {
      var uid = GetValue<string>(ahnen, false);
      if (!string.IsNullOrEmpty(uid))
      {
        var f = Focus<SB300Families>(SB300_title);
        if (f != null)
          f.OnSpElternFamilie(uid);
      }
    }

    /// <summary>Behandlung von Spfamilienkind.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnSpfamilienkindClicked(object sender, EventArgs e)
    {
      var uid = GetValue<string>(ahnen, false);
      if (!string.IsNullOrEmpty(uid))
      {
        var f = Focus<SB300Families>(SB300_title);
        if (f != null)
          f.OnSpFamilienKind(uid);
      }
    }

    /// <summary>Starten des Details-Dialogs.</summary>
    /// <param name="dt">Betroffener Dialog-Typ.</param>
    void StartDialog(DialogTypeEnum dt)
    {
      var uid = GetValue<string>(ahnen, dt != DialogTypeEnum.New);
      Start(typeof(SB210Ancestor), SB210_title, dt, uid, csbpparent: this);
    }

    /// <summary>Liefert ausgewählten Ahnen.</summary>
    /// <returns>Ausgewählten Ahnen.</returns>
    private SbPerson GetAncestor()
    {
      var uid = GetValue<string>(ahnen);
      var r = Get(FactoryService.PedigreeService.GetAncestor(ServiceDaten, uid));
      return r;
    }
  }
}
