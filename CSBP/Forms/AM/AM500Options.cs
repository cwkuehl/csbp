// <copyright file="AM500Options.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.AM;

using System;
using System.Collections.Generic;
using System.Linq;
using CSBP.Services.Apis.Enums;
using CSBP.Apis.Models;
using CSBP.Base;
using CSBP.Services.Factory;
using Gtk;
using static CSBP.Resources.Messages;

/// <summary>Controller for AM500Options dialog.</summary>
public partial class AM500Options : CsbpBin
{
#pragma warning disable CS0649

  /// <summary>Label einstellungen0.</summary>
  [Builder.Object]
  private readonly Label einstellungen0;

  /// <summary>TreeView einstellungen.</summary>
  [Builder.Object]
  private readonly TreeView einstellungen;

#pragma warning restore CS0649

  /// <summary>Dialog model.</summary>
  private List<MaParameter> model;
  private TreeStore store;

  /// <summary>Initializes a new instance of the <see cref="AM500Options"/> class.</summary>
  /// <param name="b">Affected Builder.</param>
  /// <param name="h">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="type">Affected dialog class type.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  public AM500Options(Builder b, IntPtr h, Dialog d = null, Type type = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, type ?? typeof(AM500Options), dt, p1, p)
  {
    SetBold(einstellungen0);
    InitData(0);
    einstellungen.GrabFocus();
  }

  /// <summary>Creates non modal dialog.</summary>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  /// <returns>Created dialog.</returns>
  public static AM500Options Create(object p1 = null, CsbpBin p = null)
  {
    return new AM500Options(GetBuilder("AM500Options", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  protected override void InitData(int step)
  {
    var daten = ServiceDaten;
    if (step <= 1)
    {
      model = Get(FactoryService.ClientService.GetOptionList(daten, daten.MandantNr,
        Parameter.Params)) ?? new List<MaParameter>();
      var values = new List<string[]>();
      foreach (var e in model)
      {
        // No.;Description;Latitude_r;Longitude_r;From;To;Changed at;Changed by;Created at;Created by
        values.Add(new string[]
        {
        Functions.ToString(e.Mandant_Nr), e.Schluessel,
        Functions.ToString(e.Wert), Functions.ToString(e.Comment), Functions.ToString(e.Default),
        Functions.ToString(e.Geaendert_Am, true), e.Geaendert_Von,
        Functions.ToString(e.Angelegt_Am, true), e.Angelegt_Von,
        });
      }
      store = AddStringColumnsSort(einstellungen, AM500_einstellungen_columns, values);
    }
  }

  /// <summary>Handles Ok.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnOkClicked(object sender, EventArgs e)
  {
    if (model == null || store == null)
      return;
    if (store.GetIterFirst(out var i))
    {
      do
      {
        var val = store.GetValue(i, 1) as string;
        var p = model.FirstOrDefault(a => a.Schluessel == val);
        if (p != null)
        {
          p.Wert = store.GetValue(i, 2) as string;
        }
      }
      while (store.IterNext(ref i));
    }
    var daten = ServiceDaten;
    if (Get(FactoryService.ClientService.SaveOptionList(daten, daten.MandantNr,
        model, Parameter.Params)))
    {
      Parameter.Save();
      MainClass.MainWindow.RefreshTitle();
      CloseDialog();
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
