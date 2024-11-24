// <copyright file="FZ310Author.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.FZ;

using System;
using CSBP.Services.Apis.Enums;
using CSBP.Services.Apis.Models;
using CSBP.Services.Base;
using CSBP.Services.Factory;
using Gtk;
using static CSBP.Services.Resources.Messages;

/// <summary>Controller for FZ310Author dialog.</summary>
public partial class FZ310Author : CsbpBin
{
#pragma warning disable CS0649

  /// <summary>Entry nr.</summary>
  [Builder.Object]
  private readonly Entry nr;

  /// <summary>Label name0.</summary>
  [Builder.Object]
  private readonly Label name0;

  /// <summary>Entry name.</summary>
  [Builder.Object]
  private readonly Entry name;

  /// <summary>Entry vorname.</summary>
  [Builder.Object]
  private readonly Entry vorname;

  /// <summary>TextView notiz.</summary>
  [Builder.Object]
  private readonly TextView notiz;

  /// <summary>Entry angelegt.</summary>
  [Builder.Object]
  private readonly Entry angelegt;

  /// <summary>Entry geaendert.</summary>
  [Builder.Object]
  private readonly Entry geaendert;

  /// <summary>Button ok.</summary>
  [Builder.Object]
  private readonly Button ok;

#pragma warning restore CS0649

  /// <summary>Dialog model.</summary>
  private FzBuchautor model;

  /// <summary>Initializes a new instance of the <see cref="FZ310Author"/> class.</summary>
  /// <param name="b">Affected Builder.</param>
  /// <param name="h">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="type">Affected dialog class type.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  public FZ310Author(Builder b, IntPtr h, Dialog d = null, Type type = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, type ?? typeof(FZ310Author), dt, p1, p)
  {
    SetBold(name0);
    InitData(0);
    vorname.GrabFocus();
  }

  /// <summary>Creates non modal dialog.</summary>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  /// <returns>Created dialog.</returns>
  public static FZ310Author Create(object p1 = null, CsbpBin p = null)
  {
    return new FZ310Author(GetBuilder("FZ310Author", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  protected override void InitData(int step)
  {
    if (step <= 0)
    {
      var neu = DialogType == DialogTypeEnum.New;
      var loeschen = DialogType == DialogTypeEnum.Delete;
      if (!neu && Parameter1 is string uid)
      {
        var k = Get(FactoryService.PrivateService.GetAuthor(ServiceDaten, uid));
        if (k == null)
        {
          Application.Invoke((sender, e) => { CloseDialog(); });
          return;
        }
        model = k;
        SetText(nr, k.Uid);
        SetText(name, k.Name);
        SetText(vorname, k.Vorname);
        SetText(notiz, k.Notiz);
        SetText(angelegt, ModelBase.FormatDateOf(k.Angelegt_Am, k.Angelegt_Von));
        SetText(geaendert, ModelBase.FormatDateOf(k.Geaendert_Am, k.Geaendert_Von));
      }
      nr.IsEditable = false;
      name.IsEditable = !loeschen;
      vorname.IsEditable = !loeschen;
      notiz.Editable = !loeschen;
      angelegt.IsEditable = false;
      geaendert.IsEditable = false;
      if (loeschen)
        ok.Label = Forms_delete;
    }
  }

  /// <summary>Handles Ok.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnOkClicked(object sender, EventArgs e)
  {
    ServiceErgebnis r = null;
    FzBuchautor author = null;
    if (DialogType == DialogTypeEnum.New || DialogType == DialogTypeEnum.Copy
        || DialogType == DialogTypeEnum.Edit)
    {
      var r1 = FactoryService.PrivateService.SaveAuthor(ServiceDaten,
        DialogType == DialogTypeEnum.Edit ? nr.Text : null, name.Text, vorname.Text, notiz.Buffer.Text);
      author = r1.Ergebnis;
      r = r1;
    }
    else if (DialogType == DialogTypeEnum.Delete)
    {
      r = FactoryService.PrivateService.DeleteAuthor(ServiceDaten, model);
    }
    if (r != null)
    {
      Get(r);
      if (r.Ok)
      {
        UpdateParent();
        Response = author;
        CloseDialog();
      }
    }
  }

  /// <summary>Handles Abbrechen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAbbrechenClicked(object sender, EventArgs e)
  {
    CloseDialog();
  }
}
