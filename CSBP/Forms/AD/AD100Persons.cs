// <copyright file="AD100Persons.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.AD;

using System;
using System.Collections.Generic;
using CSBP.Apis.Enums;
using CSBP.Apis.Models;
using CSBP.Base;
using CSBP.Services.Factory;
using Gtk;
using static CSBP.Resources.M;
using static CSBP.Resources.Messages;

/// <summary>Controller for AD100Persons dialog.</summary>
public partial class AD100Persons : CsbpBin
{
#pragma warning disable CS0649

  /// <summary>Button RefreshAction.</summary>
  [Builder.Object]
  private readonly Button refreshAction;

  /// <summary>Button EditAction.</summary>
  [Builder.Object]
  private readonly Button editAction;

  /// <summary>TreeView personen.</summary>
  [Builder.Object]
  private readonly TreeView personen;

  /// <summary>Entry suche.</summary>
  [Builder.Object]
  private readonly Entry suche;

  /// <summary>Entry name.</summary>
  [Builder.Object]
  private readonly Entry name;

  /// <summary>Entry vorname.</summary>
  [Builder.Object]
  private readonly Entry vorname;

#pragma warning restore CS0649

  /// <summary>Initializes a new instance of the <see cref="AD100Persons"/> class.</summary>
  /// <param name="b">Affected Builder.</param>
  /// <param name="h">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  public AD100Persons(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, dt, p1, p)
  {
    ObservableEventThrottle(refreshAction, (sender, e) => { RefreshTreeView(personen, 1); });
    //// SetBold(client0);
    InitData(0);
    personen.GrabFocus();
  }

  /// <summary>Creates non modal dialog.</summary>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  /// <returns>Created dialog.</returns>
  public static AD100Persons Create(object p1 = null, CsbpBin p = null)
  {
    return new AD100Persons(GetBuilder("AD100Persons", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  protected override void InitData(int step)
  {
    if (step <= 0)
    {
      EventsActive = false;
      SetText(suche, "%%");
      SetText(name, "%%");
      SetText(vorname, "%%");
      EventsActive = true;
    }
    if (step <= 1)
    {
      var l = Get(FactoryService.AddressService.GetPersonList(ServiceDaten, false, suche.Text, name.Text,
        vorname.Text)) ?? new List<AdSitz>();
#pragma warning disable CS0618
      var store = AddStringColumns(personen, AD100_personen_columns);
#pragma warning restore CS0618
      var pi = default(TreeIter);
      string uid = null;
      foreach (var e in l)
      {
        // No.;Description;Site;Changed at;Changed by;Created at;Created by
        if (uid == e.Person?.Uid)
        {
          store.AppendValues(pi, e.Uid, "", Functions.ToString(e.SiteName),
            Functions.ToString(e.ChangedAt, true), e.ChangedBy,
            Functions.ToString(e.CreatedAt, true), e.CreatedBy);
        }
        else
        {
          pi = store.AppendValues(e.Uid, Functions.ToString(e.PersonName), Functions.ToString(e.SiteName),
            Functions.ToString(e.ChangedAt, true), e.ChangedBy,
            Functions.ToString(e.CreatedAt, true), e.CreatedBy);
          uid = e.Person?.Uid;
        }
      }
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
    // RefreshTreeView(personen, 1);
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
    var r = Get(FactoryService.AddressService.GetAddressReport(ServiceDaten));
    UiTools.SaveFile(r, M0(AD012));
  }

  /// <summary>Handles Floppy.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnFloppyClicked(object sender, EventArgs e)
  {
    Start(typeof(AD200Interface), AD200_title, csbpparent: this);
  }

  /// <summary>Handles Personen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnPersonenRowActivated(object sender, RowActivatedArgs e)
  {
    editAction.Activate();
  }

  /// <summary>Handles Suche.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnSucheKeyReleaseEvent(object sender, KeyReleaseEventArgs e)
  {
    refreshAction.Click();
  }

  /// <summary>Handles Name.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnNameKeyReleaseEvent(object sender, KeyReleaseEventArgs e)
  {
    refreshAction.Click();
  }

  /// <summary>Handles Vorname.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnVornameKeyReleaseEvent(object sender, KeyReleaseEventArgs e)
  {
    refreshAction.Click();
  }

  /// <summary>Handles Alle.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAlleClicked(object sender, EventArgs e)
  {
    RefreshTreeView(personen, 0);
  }

  /// <summary>Handles Sitzneu.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnSitzneuClicked(object sender, EventArgs e)
  {
    StartDialog(DialogTypeEnum.Copy2);
  }

  /// <summary>Handles Sitzeins.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnSitzeinsClicked(object sender, EventArgs e)
  {
    var uid = GetValue<string>(personen);
    if (Get(FactoryService.AddressService.MakeSiteFirst(ServiceDaten, uid)))
      refreshAction.Click();
  }

  /// <summary>Handles Gebliste.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnGeblisteClicked(object sender, EventArgs e)
  {
    Start(typeof(AD120Birthdays), AD120_title, csbpparent: this);
  }

  /// <summary>Starts the details dialog.</summary>
  /// <param name="dt">Affected dialog type.</param>
  private void StartDialog(DialogTypeEnum dt)
  {
    var uid = GetValue<string>(personen, dt != DialogTypeEnum.New);
    Start(typeof(AD110Person), AD110_title, dt, uid, csbpparent: this);
  }
}
