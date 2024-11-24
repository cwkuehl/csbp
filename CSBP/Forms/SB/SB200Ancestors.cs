// <copyright file="SB200Ancestors.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.SB;

using System;
using System.Collections.Generic;
using CSBP.Services.Apis.Enums;
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

  /// <summary>Initializes a new instance of the <see cref="SB200Ancestors"/> class.</summary>
  /// <param name="b">Affected Builder.</param>
  /// <param name="h">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="type">Affected dialog class type.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  public SB200Ancestors(Builder b, IntPtr h, Dialog d = null, Type type = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, type ?? typeof(SB200Ancestors), dt, p1, p)
  {
    ObservableEventThrottle(refreshAction, (sender, e) => { RefreshTreeView(ahnen, 1); });
    //// SetBold(client0);
    InitData(0);
    ahnen.GrabFocus();
  }

  /// <summary>Creates non modal dialog.</summary>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  /// <returns>Created dialog.</returns>
  public static SB200Ancestors Create(object p1 = null, CsbpBin p = null)
  {
    return new SB200Ancestors(GetBuilder("SB200Ancestors", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>
  /// Sets selected ancestor.
  /// </summary>
  /// <param name="uid">Affected uid.</param>
  public void OnAncestor(string uid)
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
      SetText(name, "%%");
      SetText(vorname, "%%");
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
        // No.;Maiden name;First names;Surname;G.;Born;Dead;Changed at;Changed by;Created at;Created by
        values.Add(new string[]
        {
          e.Uid, e.Geburtsname, e.Vorname, e.Name, e.Geschlecht, e.Birthdate, e.Deathdate,
          Functions.ToString(e.Geaendert_Am, true), e.Geaendert_Von,
          Functions.ToString(e.Angelegt_Am, true), e.Angelegt_Von,
        });
        anz++;
        if (string.IsNullOrEmpty(e.Birthdate))
          anzg++;
      }
      AddStringColumnsSort(ahnen, SB200_ahnen_columns, values);
      SetText(ahnenStatus, SB028(anz, anzg));
    }
  }

  /// <summary>Updates parent dialog.</summary>
  protected override void UpdateParent()
  {
    refreshAction.Click();
  }

  /// <summary>Handles Refresh.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnRefreshClicked(object sender, EventArgs e)
  {
    // RefreshTreeView(ahnen, 1);
  }

  /// <summary>Handles Undo.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnUndoClicked(object sender, EventArgs e)
  {
    if (MainClass.Undo())
      refreshAction.Click();
  }

  /// <summary>Handles Redo.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnRedoClicked(object sender, EventArgs e)
  {
    if (MainClass.Redo())
      refreshAction.Click();
  }

  /// <summary>Handles New.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnNewClicked(object sender, EventArgs e)
  {
    StartDialog(DialogTypeEnum.New);
  }

  /// <summary>Handles Copy.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnCopyClicked(object sender, EventArgs e)
  {
    StartDialog(DialogTypeEnum.Copy);
  }

  /// <summary>Handles Edit.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnEditClicked(object sender, EventArgs e)
  {
    StartDialog(DialogTypeEnum.Edit);
  }

  /// <summary>Handles Delete.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnDeleteClicked(object sender, EventArgs e)
  {
    StartDialog(DialogTypeEnum.Delete);
  }

  /// <summary>Handles Print.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnPrintClicked(object sender, EventArgs e)
  {
    Start(typeof(SB220Print), SB220_title, csbpparent: this);
  }

  /// <summary>Handles Floppy.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnFloppyClicked(object sender, EventArgs e)
  {
    Start(typeof(SB500Gedcom), SB500_title, csbpparent: this);
  }

  /// <summary>Handles Ahnen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAhnenRowActivated(object sender, RowActivatedArgs e)
  {
    editAction.Activate();
  }

  /// <summary>Handles Name.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnNameKeyReleaseEvent(object sender, KeyReleaseEventArgs e)
  {
    if (!EventsActive || !filtern.Active)
      return;
    refreshAction.Click();
  }

  /// <summary>Handles Vorname.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnVornameKeyReleaseEvent(object sender, KeyReleaseEventArgs e)
  {
    if (!EventsActive || !filtern.Active)
      return;
    refreshAction.Click();
  }

  /// <summary>Handles Filtern.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnFiltern(object sender, EventArgs e)
  {
    if (!EventsActive)
      return;
    refreshAction.Click();
  }

  /// <summary>Handles Alle.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAlleClicked(object sender, EventArgs e)
  {
    RefreshTreeView(ahnen, 0);
    name.GrabFocus();
  }

  /// <summary>Handles Spname.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnSpnameClicked(object sender, EventArgs e)
  {
    var uid = GetValue<string>(ahnen, false);
    var r = Get(FactoryService.PedigreeService.GetNextAncestorName(ServiceDaten, uid, name.Text, vorname.Text));
    if (!string.IsNullOrEmpty(r))
      SetText(ahnen, r);
  }

  /// <summary>Handles Spvater.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnSpvaterClicked(object sender, EventArgs e)
  {
    var r = GetAncestor();
    if (r != null && r.Father != null)
      SetText(ahnen, r.Father.Uid);
  }

  /// <summary>Handles Spmutter.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnSpmutterClicked(object sender, EventArgs e)
  {
    var r = GetAncestor();
    if (r != null && r.Mother != null)
      SetText(ahnen, r.Mother.Uid);
  }

  /// <summary>Handles Spkind.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnSpkindClicked(object sender, EventArgs e)
  {
    var uid = GetValue<string>(ahnen, false);
    var r = Get(FactoryService.PedigreeService.GetFirstChild(ServiceDaten, uid));
    if (!string.IsNullOrEmpty(r))
      SetText(ahnen, r);
  }

  /// <summary>Handles Spehegatte.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnSpehegatteClicked(object sender, EventArgs e)
  {
    var uid = GetValue<string>(ahnen, false);
    var r = Get(FactoryService.PedigreeService.GetNextSpouse(ServiceDaten, uid));
    if (r != null)
      SetText(ahnen, r.Uid);
  }

  /// <summary>Handles Spgeschwister.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnSpgeschwisterClicked(object sender, EventArgs e)
  {
    var uid = GetValue<string>(ahnen, false);
    var r = Get(FactoryService.PedigreeService.GetNextSibling(ServiceDaten, uid));
    if (r != null)
      SetText(ahnen, r.Uid);
  }

  /// <summary>Handles Spfamilie.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnSpfamilieClicked(object sender, EventArgs e)
  {
    var uid = GetValue<string>(ahnen, false);
    if (!string.IsNullOrEmpty(uid))
    {
      var f = Focus<SB300Families>(SB300_title);
      f?.OnSpElternFamilie(uid);
    }
  }

  /// <summary>Handles Spfamilienkind.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnSpfamilienkindClicked(object sender, EventArgs e)
  {
    var uid = GetValue<string>(ahnen, false);
    if (!string.IsNullOrEmpty(uid))
    {
      var f = Focus<SB300Families>(SB300_title);
      f?.OnSpFamilienKind(uid);
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
