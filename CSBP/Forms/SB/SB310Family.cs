// <copyright file="SB310Family.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.SB;

using System;
using System.Collections.Generic;
using System.Linq;
using CSBP.Apis.Enums;
using CSBP.Apis.Models;
using CSBP.Apis.Services;
using CSBP.Base;
using CSBP.Services.Factory;
using Gtk;
using static CSBP.Resources.Messages;

/// <summary>Controller for SB310Family dialog.</summary>
public partial class SB310Family : CsbpBin
{
#pragma warning disable CS0649

  /// <summary>Entry nr.</summary>
  [Builder.Object]
  private readonly Entry nr;

  /// <summary>ComboBox vater.</summary>
  [Builder.Object]
  private readonly ComboBox vater;

  /// <summary>ComboBox mutter.</summary>
  [Builder.Object]
  private readonly ComboBox mutter;

  /// <summary>Entry heiratsdatum.</summary>
  [Builder.Object]
  private readonly Entry heiratsdatum;

  /// <summary>Entry heiratsort.</summary>
  [Builder.Object]
  private readonly Entry heiratsort;

  /// <summary>TextView heiratsbem.</summary>
  [Builder.Object]
  private readonly TextView heiratsbem;

  /// <summary>Entry angelegt.</summary>
  [Builder.Object]
  private readonly Entry angelegt;

  /// <summary>Entry geaendert.</summary>
  [Builder.Object]
  private readonly Entry geaendert;

  /// <summary>TreeView kinder.</summary>
  [Builder.Object]
  private readonly TreeView kinder;

  /// <summary>ComboBox kind.</summary>
  [Builder.Object]
  private readonly ComboBox kind;

  /// <summary>Button ok.</summary>
  [Builder.Object]
  private readonly Button ok;

  /// <summary>Button hinzufuegen.</summary>
  [Builder.Object]
  private readonly Button hinzufuegen;

  /// <summary>Button entfernen.</summary>
  [Builder.Object]
  private readonly Button entfernen;

#pragma warning restore CS0649

  /// <summary>Dialog model.</summary>
  private SbFamilie model;

  /// <summary>List of children.</summary>
  private List<SbPerson> childList = new();

  /// <summary>Initializes a new instance of the <see cref="SB310Family"/> class.</summary>
  /// <param name="b">Affected Builder.</param>
  /// <param name="h">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  public SB310Family(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, dt, p1, p)
  {
    // SetBold(client0);
    InitData(0);
    vater.GrabFocus();
  }

  /// <summary>Creates non modal dialog.</summary>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  /// <returns>Created dialog.</returns>
  public static SB310Family Create(object p1 = null, CsbpBin p = null)
  {
    return new SB310Family(GetBuilder("SB310Family", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  protected override void InitData(int step)
  {
    var daten = ServiceDaten;
    if (step <= 0)
    {
      EventsActive = false;
      var l = Get(FactoryService.PedigreeService.GetAncestorList(daten)) ?? new List<SbPerson>();
      var fl = new List<MaParameter>();
      var ml = new List<MaParameter>();
      foreach (var p in l)
      {
        if (p.Geschlecht == GenderEnum.WEIBLICH.ToString())
          ml.Add(new MaParameter { Schluessel = p.Uid, Wert = p.AncestorName });
        else
          fl.Add(new MaParameter { Schluessel = p.Uid, Wert = p.AncestorName });
      }
      AddColumns(vater, fl, true);
      AddColumns(mutter, ml, true);
      var neu = DialogType == DialogTypeEnum.New;
      var loeschen = DialogType == DialogTypeEnum.Delete;
      var aendern = DialogType == DialogTypeEnum.Edit;
      if (!neu && Parameter1 is string uid)
      {
        var k = Get(FactoryService.PedigreeService.GetFamily(daten, uid));
        if (k == null)
        {
          Application.Invoke((sender, e) => { dialog.Hide(); });
          return;
        }
        model = k;
        SetText(nr, k.Uid);
        SetText(vater, k.Mann_Uid);
        SetText(mutter, k.Frau_Uid);
        SetText(heiratsdatum, k.Marriagedate);
        SetText(heiratsort, k.Marriageplace);
        SetText(heiratsbem, k.Marriagememo);
        var chl = Get(FactoryService.PedigreeService.GetChildList(daten, k.Uid)) ?? new List<SbPerson>();
        childList.AddRange(chl);
        InitChildren();
        SetText(angelegt, ModelBase.FormatDateOf(k.Angelegt_Am, k.Angelegt_Von));
        SetText(geaendert, ModelBase.FormatDateOf(k.Geaendert_Am, k.Geaendert_Von));
      }
      nr.IsEditable = false;
      vater.Sensitive = !loeschen;
      mutter.Sensitive = !loeschen;
      heiratsdatum.IsEditable = !loeschen;
      heiratsort.IsEditable = !loeschen;
      heiratsbem.Editable = !loeschen;
      kind.Sensitive = !loeschen;
      angelegt.IsEditable = false;
      geaendert.IsEditable = false;
      if (loeschen)
        ok.Label = Forms_delete;
      hinzufuegen.Sensitive = !loeschen;
      entfernen.Sensitive = !loeschen;
      var cl = new List<MaParameter>();
      var v = GetText(vater);
      var m = GetText(mutter);
      foreach (var p in l)
      {
        if (p.Father == null && p.Mother == null && p.Uid != v && p.Uid != m)
          cl.Add(new MaParameter { Schluessel = p.Uid, Wert = p.AncestorName });
      }
      AddColumns(kind, cl, true);
      EventsActive = true;
    }
  }

  /// <summary>Handles Vater.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnVaterChanged(object sender, EventArgs e)
  {
  }

  /// <summary>Handles Mutter.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMutterChanged(object sender, EventArgs e)
  {
  }

  /// <summary>Handles Kinder.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnKinderRowActivated(object sender, RowActivatedArgs e)
  {
  }

  /// <summary>Handles Kind.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnKindChanged(object sender, EventArgs e)
  {
  }

  /// <summary>Handles Ok.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnOkClicked(object sender, EventArgs e)
  {
    ServiceErgebnis r = null;
    if (DialogType == DialogTypeEnum.New || DialogType == DialogTypeEnum.Copy
        || DialogType == DialogTypeEnum.Edit)
    {
      r = FactoryService.PedigreeService.SaveFamily(ServiceDaten,
        DialogType == DialogTypeEnum.Edit ? nr.Text : null, GetText(vater), GetText(mutter), heiratsdatum.Text,
        heiratsort.Text, heiratsbem.Buffer.Text, null, childList.Select(a => a.Uid).ToList());
    }
    else if (DialogType == DialogTypeEnum.Delete)
    {
      r = FactoryService.PedigreeService.DeleteFamily(ServiceDaten, model);
    }
    if (r != null)
    {
      Get(r);
      if (r.Ok)
      {
        UpdateParent();
        dialog.Hide();
      }
    }
  }

  /// <summary>Handles Hinzufuegen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnHinzufuegenClicked(object sender, EventArgs e)
  {
    var uid = GetText(kind);
    if (string.IsNullOrEmpty(uid) || childList.Any(a => a.Uid == uid))
      return;
    var k = Get(FactoryService.PedigreeService.GetAncestor(ServiceDaten, uid));
    if (k != null)
    {
      childList.Add(k);
      InitChildren();
    }
  }

  /// <summary>Handles Entfernen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnEntfernenClicked(object sender, EventArgs e)
  {
    var uid = GetText(kinder);
    if (string.IsNullOrEmpty(uid))
      return;
    childList = childList.Where(a => a.Uid != uid).ToList();
    InitChildren();
  }

  /// <summary>Handles Abbrechen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAbbrechenClicked(object sender, EventArgs e)
  {
    dialog.Hide();
  }

  /// <summary>
  /// Initialises children.
  /// </summary>
  private void InitChildren()
  {
    var values = new List<string[]>();
    foreach (var e in childList)
    {
      // No.;Maiden name;First names;Surname;G.;Changed at;Changed by;Created at;Created by
      values.Add(new string[]
      {
        e.Uid, e.Geburtsname, e.Vorname, e.Name, e.Geschlecht,
        Functions.ToString(e.Geaendert_Am, true), e.Geaendert_Von,
        Functions.ToString(e.Angelegt_Am, true), e.Angelegt_Von,
      });
    }
    AddStringColumnsSort(kinder, SB310_kinder_columns, values);
  }
}
