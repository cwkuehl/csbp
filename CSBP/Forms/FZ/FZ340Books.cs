// <copyright file="FZ340Books.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.FZ
{
  using System;
  using System.Collections.Generic;
  using CSBP.Apis.Enums;
  using CSBP.Apis.Models;
  using CSBP.Base;
  using CSBP.Services.Factory;
  using Gtk;
  using static CSBP.Resources.Messages;

  /// <summary>Controller für FZ340Books Dialog.</summary>
  public partial class FZ340Books : CsbpBin
  {
#pragma warning disable 169, 649

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

    /// <summary>Label buecher0.</summary>
    [Builder.Object]
    private Label buecher0;

    /// <summary>TreeView buecher.</summary>
    [Builder.Object]
    private TreeView buecher;

    /// <summary>Label titel0.</summary>
    [Builder.Object]
    private Label titel0;

    /// <summary>Entry titel.</summary>
    [Builder.Object]
    private Entry titel;

    /// <summary>Label autor0.</summary>
    [Builder.Object]
    private Label autor0;

    /// <summary>ComboBox autor.</summary>
    [Builder.Object]
    private ComboBox autor;

    /// <summary>Label serie0.</summary>
    [Builder.Object]
    private Label serie0;

    /// <summary>ComboBox serie.</summary>
    [Builder.Object]
    private ComboBox serie;

    /// <summary>Button alle.</summary>
    [Builder.Object]
    private Button alle;

#pragma warning restore 169, 649

    /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public static FZ340Books Create(object p1 = null, CsbpBin p = null)
    {
      return new FZ340Books(GetBuilder("FZ340Books", out var handle), handle, p1: p1, p: p);
    }

    /// <summary>Konstruktor für modalen Dialog.</summary>
    /// <param name="builder">Betroffener Builder.</param>
    /// <param name="h">Betroffenes Handle vom Builder.</param>
    /// <param name="d">Betroffener einbettender Dialog.</param>
    /// <param name="dt">Betroffener Dialogtyp.</param>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public FZ340Books(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
        : base(b, h, d, dt, p1, p)
    {
      ObservableEventThrottle(refreshAction, delegate { RefreshTreeView(buecher, 1); });
      // SetBold(client0);
      InitData(0);
      buecher.GrabFocus();
    }

    /// <summary>Model-Daten initialisieren.</summary>
    /// <param name="step">Betroffener Schritt: 0 erstmalig, 1 aktualisieren.</param>
    override protected void InitData(int step)
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
          // Nr.;Titel;Autor;Serie;Nr.;Seiten;Sprache;Besitz;Gelesen;Gehört;Geändert am;Geändert von;Angelegt am;Angelegt von
          values.Add(new string[] { e.Uid, e.Titel, e.AuthorCompleteName, e.SeriesName,
            Functions.ToString(e.Seriennummer), Functions.ToString(e.Seiten),
            e.Language, e.StatePossession ? "x" : "",
            Functions.ToString(e.StateRead), Functions.ToString(e.StateHeard),
            Functions.ToString(e.Geaendert_Am, true), e.Geaendert_Von,
            Functions.ToString(e.Angelegt_Am, true), e.Angelegt_Von });
        }
        AddStringColumnsSort(buecher, FZ340_buecher_columns, values);
      }
    }

    /// <summary>Aktualisierung des Eltern-Dialogs.</summary>
    override protected void UpdateParent()
    {
      refreshAction.Click();
    }

    /// <summary>Behandlung von Refresh.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnRefreshClicked(object sender, EventArgs e)
    {
      // RefreshTreeView(buecher, 1);
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

    /// <summary>Behandlung von Buecher.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnBuecherRowActivated(object sender, RowActivatedArgs e)
    {
      editAction.Activate();
    }

    /// <summary>Behandlung von Titel.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnTitelKeyReleaseEvent(object o, KeyReleaseEventArgs e)
    {
      refreshAction.Click();
    }

    /// <summary>Behandlung von Autor.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnAutorChanged(object sender, EventArgs e)
    {
      refreshAction.Click();
    }

    /// <summary>Behandlung von Serie.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnSerieChanged(object sender, EventArgs e)
    {
      refreshAction.Click();
    }

    /// <summary>Behandlung von Alle.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnAlleClicked(object sender, EventArgs e)
    {
      RefreshTreeView(buecher, 0);
    }

    /// <summary>Starten des Details-Dialogs.</summary>
    /// <param name="dt">Betroffener Dialog-Typ.</param>
    void StartDialog(DialogTypeEnum dt)
    {
      var uid = GetValue<string>(buecher, dt != DialogTypeEnum.New);
      Start(typeof(FZ350Book), FZ350_title, dt, uid, csbpparent: this);
    }
  }
}
