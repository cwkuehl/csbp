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

  /// <summary>State for task.</summary>
  private readonly StringBuilder state = new();

  /// <summary>Cancel for task.</summary>
  private readonly StringBuilder cancel = new();

  /// <summary>Initializes a new instance of the <see cref="AG400Backups"/> class.</summary>
  /// <param name="b">Affected Builder.</param>
  /// <param name="h">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="type">Affected dialog class type.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  public AG400Backups(Builder b, IntPtr h, Dialog d = null, Type type = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, type ?? typeof(AG400Backups), dt, p1, p)
  {
    SetBold(verzeichnisse0);
    InitData(0);
  }

  /// <summary>Creates non modal dialog.</summary>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  /// <returns>Created dialog.</returns>
  public static AG400Backups Create(object p1 = null, CsbpBin p = null)
  {
    return new AG400Backups(GetBuilder("AG400Backups", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Schließen des Dialogs.</summary>
  /// <returns>Affected value.</returns>
  public override object Close()
  {
    HttpServer.Stop();
    HttpsServer.Stop();
    return null;
  }

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  protected override void InitData(int step)
  {
    var daten = ServiceDaten;
    if (step <= 0)
    {
      SetText(mandant, daten.MandantNr.ToString());
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
        // No.;Target;E.;P.;Sources;Changed at;Changed by;Created at;Created by
        values.Add(new string[]
        {
          e.Uid, e.Target, e.Encrypted ? "X" : "", e.Zipped ? "X" : "", e.SourcesText,
          Functions.ToString(e.Geaendert_Am, true), e.Geaendert_Von,
          Functions.ToString(e.Angelegt_Am, true), e.Angelegt_Von,
        });
      }
      AddStringColumnsSort(verzeichnisse, AG400_verzeichnisse_columns, values);
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
    RefreshTreeView(verzeichnisse, 1);
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

  /// <summary>Handles Verzeichnisse.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnVerzeichnisseRowActivated(object sender, RowActivatedArgs e)
  {
    editAction.Activate();
  }

  /// <summary>Handles Sicherung.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnSicherungClicked(object sender, EventArgs e)
  {
    MakeBackup();
  }

  /// <summary>Handles Diffsicherung.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnDiffsicherungClicked(object sender, EventArgs e)
  {
  }

  /// <summary>Handles Ruecksicherung.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnRuecksicherungClicked(object sender, EventArgs e)
  {
    MakeBackup(true);
  }

  /// <summary>Handles Abbrechen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAbbrechenClicked(object sender, EventArgs e)
  {
    cancel.Append("cancel");
  }

  /// <summary>Handles Sqlsicherung.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnSqlsicherungClicked(object sender, EventArgs e)
  {
  }

  /// <summary>Handles Mandantkopieren.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMandantkopierenClicked(object sender, EventArgs e)
  {
  }

  /// <summary>Handles Mandantrepkopieren.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMandantrepkopierenClicked(object sender, EventArgs e)
  {
  }

  /// <summary>Starts the details dialog.</summary>
  /// <param name="dt">Affected dialog type.</param>
  private void StartDialog(DialogTypeEnum dt)
  {
    var uid = GetValue<string>(verzeichnisse, dt != DialogTypeEnum.New);
    Start(typeof(AG410Backup), AG410_title, dt, uid, csbpparent: this);
  }

#pragma warning disable RECS0165 // Asynchrone Methoden sollten eine Aufgabe anstatt 'void' zurückgeben.

  /// <summary>
  /// Does Backup.
  /// </summary>
  /// <param name="restore">Restore or not.</param>
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
      ShowStatus(state, cancel);
      var r = await Task.Run(() =>
      {
        var r0 = FactoryService.ClientService.MakeBackup(ServiceDaten, uid, restore, password, state, cancel);
        return r0;
      });
      r.ThrowAllErrors();
    }
    catch (Exception ex)
    {
      Application.Invoke((sender, e) =>
      {
        ShowError(ex.Message);
      });
    }
    finally
    {
      cancel.Append("End");
    }
  }
}
