// <copyright file="AG500Ai.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.AG;

using System;
using System.Collections.Generic;
using System.Linq;
using CSBP.Apis.Enums;
using CSBP.Apis.Models;
using CSBP.Base;
using CSBP.Forms.TB;
using CSBP.Services.Factory;
using CSBP.Services.NonService;
using Gtk;
using static CSBP.Resources.M;
using static CSBP.Resources.Messages;

/// <summary>Controller for AG500Ai dialog.</summary>
public partial class AG500Ai : CsbpBin
{
#pragma warning disable CS0649

  /// <summary>Label prompt0.</summary>
  [Builder.Object]
  private readonly Label prompt0;

  /// <summary>TextView prompt.</summary>
  [Builder.Object]
  private readonly TextView prompt;

  /// <summary>TreeView dialogs.</summary>
  [Builder.Object]
  private readonly TreeView dialogs;

  /// <summary>ComboBox model.</summary>
  [Builder.Object]
  private readonly ComboBox model;

  /// <summary>Entry tokens.</summary>
  [Builder.Object]
  private readonly Entry tokens;

  /// <summary>TextView response.</summary>
  [Builder.Object]
  private readonly TextView response;

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
    SetBold(prompt0);
    InitData(0);
    prompt.GrabFocus();
  }

  /// <summary>Gets or sets the diary entry for copying.</summary>
  private string Copy { get; set; } = string.Empty;

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
      InitLists();
      SetText(prompt, Functions.IsDe ? "Sag dies ist ein Test!" : "Say this is a test!");
      SetText(tokens, "50");
      SetText(model, AiData.Gpt35);
      response.Editable = false;
      EventsActive = true;
    }
    if (step <= 1)
    {
      var l = Get(FactoryService.ClientService.GetDialogList(ServiceDaten)) ?? new List<AgDialog>();
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
    InitData(1);
  }

  /// <summary>Handles Copy.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnCopyClicked(object sender, EventArgs e)
  {
    Copy = prompt.Buffer.Text;
  }

  /// <summary>Handles Paste.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnPasteClicked(object sender, EventArgs e)
  {
    SetText(prompt, Copy);
  }

  /// <summary>Handles Undo.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnUndoClicked(object sender, EventArgs e)
  {
    if (MainClass.Undo())
    {
      InitData(1);
    }
  }

  /// <summary>Handles Redo.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnRedoClicked(object sender, EventArgs e)
  {
    if (MainClass.Redo())
    {
      InitData(1);
    }
  }

  /// <summary>Handles Delete.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnDeleteClicked(object sender, EventArgs e)
  {
    var uid = GetText(dialogs);
    if (Get(FactoryService.ClientService.DeleteDialog(ServiceDaten, uid)))
      InitData(1);
  }

  /// <summary>Handles Weather.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnWeatherClicked(object sender, EventArgs e)
  {
    // TODO Weather.
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
    if (d != null && d.Data != null)
    {
      SetText(prompt, d.Data.Prompt);
      SetText(tokens, Functions.ToString(d.Data.MaxTokens));
      SetText(model, d.Data.Model);
      SetText(response, d.Data.Messages.FirstOrDefault());
    }
  }

  /// <summary>Handles Execute.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnExecuteClicked(object sender, EventArgs e)
  {
    var maxtokens = Functions.ToInt32(tokens.Text);
    var r = FactoryService.ClientService.AskChatGpt(ServiceDaten, prompt.Buffer.Text, null, maxtokens);
    Get(r);
    var data = r.Ergebnis;
    if (r.Ok && data != null)
    {
      if (data.Messages.Any())
        SetText(response, r.Ergebnis.Messages.FirstOrDefault());
      InitData(1);
    }
  }

  /// <summary>Initialises the lists.</summary>
  private void InitLists()
  {
    var daten = ServiceDaten;
    var uid = GetText(model);
    AddColumns(model, Get(FactoryService.ClientService.GetAiModelList(daten)));
    SetText(model, uid);
  }
}
