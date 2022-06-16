// <copyright file="FZ340Books.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.FZ;

using System;
using System.Collections.Generic;
using CSBP.Apis.Enums;
using CSBP.Apis.Models;
using CSBP.Base;
using CSBP.Services.Factory;
using Gtk;
using static CSBP.Resources.Messages;

/// <summary>Controller for FZ340Books dialog.</summary>
public partial class FZ340Books : CsbpBin
{
#pragma warning disable CS0649

  /// <summary>Button RefreshAction.</summary>
  [Builder.Object]
  private readonly Button refreshAction;

  /// <summary>Button EditAction.</summary>
  [Builder.Object]
  private readonly Button editAction;

  /// <summary>TreeView buecher.</summary>
  [Builder.Object]
  private readonly TreeView buecher;

  /// <summary>Entry titel.</summary>
  [Builder.Object]
  private readonly Entry titel;

  /// <summary>ComboBox autor.</summary>
  [Builder.Object]
  private readonly ComboBox autor;

  /// <summary>ComboBox serie.</summary>
  [Builder.Object]
  private readonly ComboBox serie;

#pragma warning restore CS0649

  /// <summary>Initializes a new instance of the <see cref="FZ340Books"/> class.</summary>
  /// <param name="b">Affected Builder.</param>
  /// <param name="h">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  public FZ340Books(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, dt, p1, p)
  {
    ObservableEventThrottle(refreshAction, (sender, e) => { RefreshTreeView(buecher, 1); });
    //// SetBold(client0);
    InitData(0);
    buecher.GrabFocus();
  }

  /// <summary>Creates non modal dialog.</summary>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  /// <returns>Created dialog.</returns>
  public static FZ340Books Create(object p1 = null, CsbpBin p = null)
  {
    return new FZ340Books(GetBuilder("FZ340Books", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  protected override void InitData(int step)
  {
    var daten = ServiceDaten;
    if (step <= 0)
    {
      titel.Text = "%%";
      var al = Get(FactoryService.PrivateService.GetAuthorList(daten));
      var rs = AddColumns(autor, emptyentry: true);
      foreach (var p in al)
        rs.AppendValues(p.CompleteName, p.Uid);
      var sl = Get(FactoryService.PrivateService.GetSeriesList(daten));
      rs = AddColumns(serie, emptyentry: true);
      foreach (var p in sl)
        rs.AppendValues(p.Name, p.Uid);
      SetText(autor, null);
      SetText(serie, null);
    }
    if (step <= 1)
    {
      var l = Get(FactoryService.PrivateService.GetBookList(daten, GetText(autor), GetText(serie),
        null, titel.Text)) ?? new List<FzBuch>();
      var values = new List<string[]>();
      foreach (var e in l)
      {
        // No.;Title;Author;Series;No._r;Pages_r;Language;Pos.;Read;Heard;Changed at;Changed by;Created at;Created by
        values.Add(new string[]
        {
          e.Uid, e.Titel, e.AuthorCompleteName, e.SeriesName,
          Functions.ToString(e.Seriennummer), Functions.ToString(e.Seiten),
          e.Language, e.StatePossession ? "x" : "",
          Functions.ToString(e.StateRead), Functions.ToString(e.StateHeard),
          Functions.ToString(e.Geaendert_Am, true), e.Geaendert_Von,
          Functions.ToString(e.Angelegt_Am, true), e.Angelegt_Von,
        });
      }
      AddStringColumnsSort(buecher, FZ340_buecher_columns, values);
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
    // RefreshTreeView(buecher, 1);
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

  /// <summary>Handles Buecher.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnBuecherRowActivated(object sender, RowActivatedArgs e)
  {
    editAction.Activate();
  }

  /// <summary>Handles Titel.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnTitelKeyReleaseEvent(object sender, KeyReleaseEventArgs e)
  {
    refreshAction.Click();
  }

  /// <summary>Handles Autor.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAutorChanged(object sender, EventArgs e)
  {
    refreshAction.Click();
  }

  /// <summary>Handles Serie.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnSerieChanged(object sender, EventArgs e)
  {
    refreshAction.Click();
  }

  /// <summary>Handles Alle.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAlleClicked(object sender, EventArgs e)
  {
    RefreshTreeView(buecher, 0);
  }

  /// <summary>Starts the details dialog.</summary>
  /// <param name="dt">Affected dialog type.</param>
  private void StartDialog(DialogTypeEnum dt)
  {
    var uid = GetValue<string>(buecher, dt != DialogTypeEnum.New);
    Start(typeof(FZ350Book), FZ350_title, dt, uid, csbpparent: this);
  }
}
