// <copyright file="AG400Backups.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.AG;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CSBP.Apis.Enums;
using CSBP.Base;
using CSBP.Services.Factory;
using CSBP.Services.Server;
using Gtk;
using static CSBP.Resources.M;
using static CSBP.Resources.Messages;

/// <summary>Controller for AG400Backups dialog.</summary>
public partial class AG400Backups : CsbpBin
{
  /// <summary>Status für Task.</summary>
  readonly StringBuilder Status = new();

  /// <summary>Abbruch für Task.</summary>
  readonly StringBuilder Cancel = new();

#pragma warning disable CS0649

  /// <summary>Button RefreshAction.</summary>
  [Builder.Object]
  private readonly Button refreshAction;

  /// <summary>Button EditAction.</summary>
  [Builder.Object]
  private readonly Button editAction;

  /// <summary>Label verzeichnisse0.</summary>
  [Builder.Object]
  private readonly Label verzeichnisse0;

  /// <summary>TreeView verzeichnisse.</summary>
  [Builder.Object]
  private readonly TreeView verzeichnisse;

  /// <summary>Entry mandant.</summary>
  [Builder.Object]
  private readonly Entry mandant;

#pragma warning restore CS0649

  /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
  /// <param name="p1">1. Parameter für Dialog.</param>
  /// <param name="p">Betroffener Eltern-Dialog.</param>
  /// <returns>Nicht-modalen Dialogs.</returns>
  public static AG400Backups Create(object p1 = null, CsbpBin p = null)
  {
    return new AG400Backups(GetBuilder("AG400Backups", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Konstruktor für modalen Dialog.</summary>
  /// <param name="b">Betroffener Builder.</param>
  /// <param name="h">Betroffenes Handle vom Builder.</param>
  /// <param name="d">Betroffener einbettender Dialog.</param>
  /// <param name="dt">Betroffener Dialogtyp.</param>
  /// <param name="p1">1. Parameter für Dialog.</param>
  /// <param name="p">Betroffener Eltern-Dialog.</param>
  /// <returns>Nicht-modalen Dialogs.</returns>
  public AG400Backups(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, dt, p1, p)
  {
    SetBold(verzeichnisse0);
    InitData(0);
  }

  /// <summary>Model-Daten initialisieren.</summary>
  /// <param name="step">Betroffener Schritt: 0 erstmalig, 1 aktualisieren.</param>
  protected override void InitData(int step)
  {
    var daten = ServiceDaten;
    if (step <= 0)
    {
      mandant.Text = daten.MandantNr.ToString();
      var http = HttpsServer.IsStarted();
#if DEBUG
      http = true;
#endif
      if (http)
      {
        Task.Run(() =>
        {
          HttpServer.Start(daten.BenutzerId);
        });
      }
      Task.Run(() =>
      {
        HttpsServer.Start(daten.BenutzerId);
      });
    }
    if (step <= 1)
    {
      var l = Get(FactoryService.ClientService.GetBackupEntryList(daten));
      var values = new List<string[]>();
      foreach (var e in l)
      {
        // Nr.;Ziel;V.;P.;Quellen;Geändert am;Geändert von;Angelegt am;Angelegt von
        values.Add(new string[] { e.Uid, e.Target, e.Encrypted ? "X" : "", e.Zipped ? "X" : "", e.SourcesText,
            Functions.ToString(e.Geaendert_Am, true), e.Geaendert_Von,
            Functions.ToString(e.Angelegt_Am, true), e.Angelegt_Von });
      }
      AddStringColumnsSort(verzeichnisse, AG400_verzeichnisse_columns, values);
    }
  }

  /// <summary>Aktualisierung des Eltern-Dialogs.</summary>
  protected override void UpdateParent()
  {
    refreshAction.Click();
  }

  /// <summary>Schließen des Dialogs.</summary>
  public override object Close()
  {
    HttpServer.Stop();
    HttpsServer.Stop();
    return null;
  }

  /// <summary>Handle Refresh.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnRefreshClicked(object sender, EventArgs e)
  {
    RefreshTreeView(verzeichnisse, 1);
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

  /// <summary>Handle Verzeichnisse.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnVerzeichnisseRowActivated(object sender, RowActivatedArgs e)
  {
    editAction.Activate();
  }

  /// <summary>Handle Sicherung.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnSicherungClicked(object sender, EventArgs e)
  {
    MakeBackup();
  }

  /// <summary>Handle Diffsicherung.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnDiffsicherungClicked(object sender, EventArgs e)
  {
  }

  /// <summary>Handle Ruecksicherung.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnRuecksicherungClicked(object sender, EventArgs e)
  {
    MakeBackup(true);
  }

  /// <summary>Handle Abbrechen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAbbrechenClicked(object sender, EventArgs e)
  {
    Cancel.Append("Cancel");
  }

  /// <summary>Handle Sqlsicherung.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnSqlsicherungClicked(object sender, EventArgs e)
  {
  }

  /// <summary>Handle Mandantkopieren.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMandantkopierenClicked(object sender, EventArgs e)
  {
  }

  /// <summary>Handle Mandantrepkopieren.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMandantrepkopierenClicked(object sender, EventArgs e)
  {
  }

  /// <summary>Starten des Details-Dialogs.</summary>
  /// <param name="dt">Betroffener Dialog-Typ.</param>
  void StartDialog(DialogTypeEnum dt)
  {
    var uid = GetValue<string>(verzeichnisse, dt != DialogTypeEnum.New);
    Start(typeof(AG410Backup), AG410_title, dt, uid, csbpparent: this);
  }

#pragma warning disable RECS0165 // Asynchrone Methoden sollten eine Aufgabe anstatt 'void' zurückgeben.
  private async void MakeBackup(bool restore = false)
#pragma warning restore RECS0165
  {
    try
    {
      var password = "";
      var uid = GetValue<string>(verzeichnisse);
      if (restore && !ShowYesNoQuestion(M0(AG001)))
        return;
      var be = Get(FactoryService.ClientService.GetBackupEntry(ServiceDaten, uid));
      if (be != null && be.Encrypted)
      {
        password = (string)Start(typeof(AG420Encryption), AG420_title, parameter1: uid, modal: true, csbpparent: this);
        if (string.IsNullOrEmpty(password))
          return;
      }
      ShowStatus(Status, Cancel);
      var r = await Task.Run(() =>
      {
        var r0 = FactoryService.ClientService.MakeBackup(ServiceDaten, uid, restore, password, Status, Cancel);
        return r0;
      });
      r.ThrowAllErrors();
    }
    catch (Exception ex)
    {
      Application.Invoke(delegate
      {
        ShowError(ex.Message);
      });
    }
    finally
    {
      Cancel.Append("End");
    }
  }
}
