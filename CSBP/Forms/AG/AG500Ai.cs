// <copyright file="AG500Ai.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.AG;

using System;
using System.Collections.Generic;
using System.Linq;
using CSBP.Services.Apis.Enums;
using CSBP.Services.Apis.Models;
using CSBP.Services.Base;
using CSBP.Services.Factory;
using CSBP.Services.NonService;
using Gtk;
using static CSBP.Services.Resources.Messages;

/// <summary>Controller for AG500Ai dialog.</summary>
public partial class AG500Ai : CsbpBin
{
#pragma warning disable CS0649

  /// <summary>Button RefreshAction.</summary>
  [Builder.Object]
  private readonly Button refreshAction;

  /// <summary>Label systemprompt0.</summary>
  [Builder.Object]
  private readonly Label systemprompt0;

  /// <summary>TextView systemprompt.</summary>
  [Builder.Object]
  private readonly TextView systemprompt;

  /// <summary>Label prompt0.</summary>
  [Builder.Object]
  private readonly Label prompt0;

  /// <summary>TextView prompt.</summary>
  [Builder.Object]
  private readonly TextView prompt;

  /// <summary>TreeView dialogs.</summary>
  [Builder.Object]
  private readonly TreeView dialogs;

  /// <summary>Entry search.</summary>
  [Builder.Object]
  private readonly Entry search;

  /// <summary>Entry maxtokens.</summary>
  [Builder.Object]
  private readonly Entry maxtokens;

  /// <summary>ComboBox model.</summary>
  [Builder.Object]
  private readonly ComboBox model;

  /// <summary>TextView response.</summary>
  [Builder.Object]
  private readonly TextView response;

  /// <summary>Entry tokens.</summary>
  [Builder.Object]
  private readonly Entry tokens;

  /// <summary>CheckButton continues.</summary>
  [Builder.Object]
  private readonly CheckButton continues;

  /// <summary>Seledted dialog entry.</summary>
  private AgDialog selected;

#pragma warning restore CS0649

  /// <summary>Initializes a new instance of the <see cref="AG500Ai"/> class.</summary>
  /// <param name="b">Affected Builder.</param>
  /// <param name="h">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="type">Affected dialog class type.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  public AG500Ai(Builder b, IntPtr h, Dialog d = null, Type type = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, type ?? typeof(AG500Ai), dt, p1, p)
  {
    ObservableEventThrottle(refreshAction, (sender, e) => { RefreshTreeView(dialogs, 1); });
    SetBold(systemprompt0);
    SetBold(prompt0);
    InitData(0);
    prompt.GrabFocus();
  }

  /// <summary>Creates non modal dialog.</summary>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  /// <returns>Created dialog.</returns>
  public static AG500Ai Create(object p1 = null, CsbpBin p = null)
  {
    return new AG500Ai(GetBuilder("AG500Ai", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  protected override void InitData(int step)
  {
    if (step <= 0)
    {
      EventsActive = false;
      SetText(search, "%%");
      InitLists();
      SetText(model, AiData.LocalLlama3);
      OnNewClicked(null, null);
      response.Editable = false;
      tokens.IsEditable = false;
      EventsActive = true;
    }
    if (step <= 1)
    {
      var l = Get(FactoryService.ClientService.GetDialogList(ServiceDaten, search: search.Text)) ?? new List<AgDialog>();
      var values = new List<string[]>();
      foreach (var e in l)
      {
        // No.;Date;Prompt;Changed at;Changed by;Created at;Created by
        values.Add(new string[]
        {
          e.Uid, Functions.ToString(e.Datum), e.Data.Prompt,
          Functions.ToString(e.Geaendert_Am, true), e.Geaendert_Von,
          Functions.ToString(e.Angelegt_Am, true), e.Angelegt_Von,
        });
      }
      AddStringColumnsSort(dialogs, AG500_dialogs_columns, values);
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
    // RefreshTreeView(dialogs, 1);
  }

  /// <summary>Handles Undo.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnUndoClicked(object sender, EventArgs e)
  {
    if (MainClass.Undo())
    {
      refreshAction.Click();
    }
  }

  /// <summary>Handles Redo.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnRedoClicked(object sender, EventArgs e)
  {
    if (MainClass.Redo())
    {
      refreshAction.Click();
    }
  }

  /// <summary>Handles New.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnNewClicked(object sender, EventArgs e)
  {
    selected = null;
    SetText(dialogs, null);
    var spde = "Du bist einer der besten Programmierer. Du gibst präzise und korrekte Antworten. Du entwickelst Konzepte Schritt für Schritt.";
    var sp = "You are a first class programmer. Let's think step by step.";
    SetText(systemprompt, Functions.IsDe ? spde : sp);
    SetText(prompt, Functions.IsDe ? "Sag dies ist ein Test!" : "Say this is a test!");
    SetText(maxtokens, "1000");
    SetText(response, null);
    SetText(tokens, null);
    var ea = EventsActive;
    EventsActive = false;
    continues.Active = false;
    EventsActive = ea;
    refreshAction.Click();
  }

  /// <summary>Handles Delete.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnDeleteClicked(object sender, EventArgs e)
  {
    var uid = GetText(dialogs);
    if (Get(FactoryService.ClientService.DeleteDialog(ServiceDaten, uid)))
    {
      if (selected?.Uid == uid)
        selected = null;
      refreshAction.Click();
    }
  }

  /// <summary>Handles Dialogs.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnDialogsCursorChanged(object sender, EventArgs e)
  {
  }

  /// <summary>Handles Dialogs.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnDialogsRowActivated(object sender, RowActivatedArgs e)
  {
    var uid = Functions.ToString(GetText(dialogs));
    var l = Get(FactoryService.ClientService.GetDialogList(ServiceDaten, null, uid)) ?? new List<AgDialog>();
    var d = l.FirstOrDefault();
    if (d?.Data != null)
    {
      selected = d;
      SetData(d.Data);
    }
  }

  /// <summary>Handles Search.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnSearchKeyReleaseEvent(object sender, KeyReleaseEventArgs e)
  {
    refreshAction.Click();
  }

  /// <summary>Handles continue.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnContinue(object sender, EventArgs e)
  {
    if (!EventsActive)
      return;
    var ea = EventsActive;
    try
    {
      EventsActive = false;
      var r = FactoryService.ClientService.ContinueDialog(ServiceDaten, selected, continues.Active, systemprompt.Buffer.Text, prompt.Buffer.Text, response.Buffer.Text);
      Get(r);
      var data = r.Ergebnis;
      if (r.Ok && data != null)
      {
        SetText(prompt, data.Prompt);
        SetText(response, data.GetDialogHistory);
        continues.Active = data.ContinueDialog;
      }
    }
    finally
    {
      EventsActive = ea;
    }
  }

  /// <summary>Handles Execute.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnExecuteClicked(object sender, EventArgs e)
  {
    var model0 = GetText(model);
    var maxt = Functions.ToInt32(maxtokens.Text);
    var r = FactoryService.ClientService.AskChatGpt(ServiceDaten, systemprompt.Buffer.Text, prompt.Buffer.Text, model0, maxt,
      dialog: continues.Active ? selected : null);
    Get(r);
    var data = r.Ergebnis;
    if (r.Ok && data != null)
    {
      // SetData(data, true);
      refreshAction.Click();
      SetText(dialogs, data.DialogUid);
      Application.Invoke((sender, e) =>
      {
        OnDialogsRowActivated(null, null);
      });
    }
  }

  /// <summary>Handles All.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAllClicked(object sender, EventArgs e)
  {
    SetText(search, "%%");
    refreshAction.Click();
    search.GrabFocus();
  }

  /// <summary>Initialises the lists.</summary>
  private void InitLists()
  {
    var daten = ServiceDaten;
    var uid = GetText(model);
    AddColumns(model, Get(FactoryService.ClientService.GetAiModelList(daten)));
    SetText(model, uid);
  }

  /// <summary>Sets dialogs data.</summary>
  /// <param name="data">Affected data.</param>
  /// <param name="responly">True, if only response should be set.</param>
  private void SetData(AiData data, bool responly = false)
  {
    if (data == null)
      return;
    try
    {
      EventsActive = false;
      if (!responly)
      {
        SetText(systemprompt, data.SystemPrompt);
        SetText(prompt, data.Prompt);
        //// SetText(maxtokens, Functions.ToString(data.MaxTokens));
        SetText(model, data.Model);
      }
      SetText(response, data.GetDialogHistory);
      SetText(tokens, @$"{data.PromptTokens}+{data.CompletionTokens}={data.PromptTokens + data.CompletionTokens} ({data.FinishReasons.FirstOrDefault()})");
      continues.Active = data.ContinueDialog;
    }
    finally
    {
      EventsActive = true;
    }
    if (data.ContinueDialog)
      SetText(prompt, null);
  }
}
