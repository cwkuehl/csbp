// <copyright file="SB200Ancestors.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.SB;

using System;
using System.Collections.Generic;
using CSBP.Apis.Enums;
using CSBP.Apis.Models;
using CSBP.Base;
using CSBP.Services.Factory;
using Gtk;
using static CSBP.Resources.M;
using static CSBP.Resources.Messages;

/// <summary>Controller for SB200Ancestors dialog.</summary>
public partial class SB200Ancestors : CsbpBin
{
#pragma warning disable CS0649

  /// <summary>Button RefreshAction.</summary>
  [Builder.Object]
  private readonly Button refreshAction;

  /// <summary>Button EditAction.</summary>
  [Builder.Object]
  private readonly Button editAction;

  /// <summary>TreeView ahnen.</summary>
  [Builder.Object]
  private readonly TreeView ahnen;

  /// <summary>Label ahnenStatus.</summary>
  [Builder.Object]
  private readonly Label ahnenStatus;

  /// <summary>Entry name.</summary>
  [Builder.Object]
  private readonly Entry name;

  /// <summary>Entry vorname.</summary>
  [Builder.Object]
  private readonly Entry vorname;

  /// <summary>CheckButton filtern.</summary>
  [Builder.Object]
  private readonly CheckButton filtern;

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
  /// <param name="b">Betroffener Builder.</param>
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

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
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

  /// <summary>Updates parent dialog.</summary>
  protected override void UpdateParent()
  {
    refreshAction.Click();
  }

  /// <summary>Handle Refresh.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnRefreshClicked(object sender, EventArgs e)
  {
    // RefreshTreeView(ahnen, 1);
  }

  /// <summary>Handle Undo.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnUndoClicked(object sender, EventArgs e)
  {
    if (MainClass.Undo())
      refreshAction.Click();
  }

  /// <summary>Handle Redo.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnRedoClicked(object sender, EventArgs e)
  {
    if (MainClass.Redo())
      refreshAction.Click();
  }

  /// <summary>Handle New.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnNewClicked(object sender, EventArgs e)
  {
    StartDialog(DialogTypeEnum.New);
  }

  /// <summary>Handle Copy.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnCopyClicked(object sender, EventArgs e)
  {
    StartDialog(DialogTypeEnum.Copy);
  }

  /// <summary>Handle Edit.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnEditClicked(object sender, EventArgs e)
  {
    StartDialog(DialogTypeEnum.Edit);
  }

  /// <summary>Handle Delete.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnDeleteClicked(object sender, EventArgs e)
  {
    StartDialog(DialogTypeEnum.Delete);
  }

  /// <summary>Handle Print.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnPrintClicked(object sender, EventArgs e)
  {
    Start(typeof(SB220Print), SB220_title, csbpparent: this);
  }

  /// <summary>Handle Floppy.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnFloppyClicked(object sender, EventArgs e)
  {
    Start(typeof(SB500Gedcom), SB500_title, csbpparent: this);
  }

  /// <summary>Handle Ahnen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAhnenRowActivated(object sender, RowActivatedArgs e)
  {
    editAction.Activate();
  }

  /// <summary>Handle Name.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnNameKeyReleaseEvent(object o, KeyReleaseEventArgs e)
  {
    if (!EventsActive || !filtern.Active)
      return;
    refreshAction.Click();
  }

  /// <summary>Handle Vorname.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnVornameKeyReleaseEvent(object o, KeyReleaseEventArgs e)
  {
    if (!EventsActive || !filtern.Active)
      return;
    refreshAction.Click();
  }

  /// <summary>Handle Filtern.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnFiltern(object o, EventArgs e)
  {
    if (!EventsActive)
      return;
    refreshAction.Click();
  }

  /// <summary>Handle Alle.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAlleClicked(object sender, EventArgs e)
  {
    RefreshTreeView(ahnen, 0);
  }

  /// <summary>Handle Spname.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnSpnameClicked(object sender, EventArgs e)
  {
    var uid = GetValue<string>(ahnen, false);
    var r = Get(FactoryService.PedigreeService.GetNextAncestorName(ServiceDaten, uid, name.Text, vorname.Text));
    if (!string.IsNullOrEmpty(r))
      SetText(ahnen, r);
  }

  /// <summary>Handle Spvater.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnSpvaterClicked(object sender, EventArgs e)
  {
    var r = GetAncestor();
    if (r != null && r.Father != null)
      SetText(ahnen, r.Father.Uid);
  }

  /// <summary>Handle Spmutter.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnSpmutterClicked(object sender, EventArgs e)
  {
    var r = GetAncestor();
    if (r != null && r.Mother != null)
      SetText(ahnen, r.Mother.Uid);
  }

  /// <summary>Handle Spkind.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnSpkindClicked(object sender, EventArgs e)
  {
    var uid = GetValue<string>(ahnen, false);
    var r = Get(FactoryService.PedigreeService.GetFirstChild(ServiceDaten, uid));
    if (!string.IsNullOrEmpty(r))
      SetText(ahnen, r);
  }

  /// <summary>Handle Spehegatte.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnSpehegatteClicked(object sender, EventArgs e)
  {
    var uid = GetValue<string>(ahnen, false);
    var r = Get(FactoryService.PedigreeService.GetNextSpouse(ServiceDaten, uid));
    if (r != null)
      SetText(ahnen, r.Uid);
  }

  /// <summary>Handle Spgeschwister.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnSpgeschwisterClicked(object sender, EventArgs e)
  {
    var uid = GetValue<string>(ahnen, false);
    var r = Get(FactoryService.PedigreeService.GetNextSibling(ServiceDaten, uid));
    if (r != null)
      SetText(ahnen, r.Uid);
  }

  /// <summary>Handle Spfamilie.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
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

  /// <summary>Handle Spfamilienkind.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
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

  /// <summary>Starts the details dialog.</summary>
  /// <param name="dt">Affected dialog type.</param>
  private void StartDialog(DialogTypeEnum dt)
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
