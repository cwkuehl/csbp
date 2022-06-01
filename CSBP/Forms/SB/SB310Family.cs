// <copyright file="SB310Family.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.SB
{
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

  /// <summary>Controller für SB310Family Dialog.</summary>
  public partial class SB310Family : CsbpBin
  {
    /// <summary>Dialog Model.</summary>
    private SbFamilie Model;

    /// <summary>Liste der Kinder.</summary>
    private List<SbPerson> ChildList = new();

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

    /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public static SB310Family Create(object p1 = null, CsbpBin p = null)
    {
      return new SB310Family(GetBuilder("SB310Family", out var handle), handle, p1: p1, p: p);
    }

    /// <summary>Konstruktor für modalen Dialog.</summary>
    /// <param name="builder">Betroffener Builder.</param>
    /// <param name="h">Betroffenes Handle vom Builder.</param>
    /// <param name="d">Betroffener einbettender Dialog.</param>
    /// <param name="dt">Betroffener Dialogtyp.</param>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public SB310Family(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
        : base(b, h, d, dt, p1, p)
    {
      // SetBold(client0);
      InitData(0);
      vater.GrabFocus();
    }

    /// <summary>Model-Daten initialisieren.</summary>
    /// <param name="step">Betroffener Schritt: 0 erstmalig, 1 aktualisieren.</param>
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
            Application.Invoke(delegate
            {
              dialog.Hide();
            });
            return;
          }
          Model = k;
          nr.Text = k.Uid ?? "";
          SetText(vater, k.Mann_Uid);
          SetText(mutter, k.Frau_Uid);
          heiratsdatum.Text = k.Marriagedate ?? "";
          heiratsort.Text = k.Marriageplace ?? "";
          heiratsbem.Buffer.Text = k.Marriagememo ?? "";
          var chl = Get(FactoryService.PedigreeService.GetChildList(daten, k.Uid)) ?? new List<SbPerson>();
          ChildList.AddRange(chl);
          InitChildren();
          angelegt.Text = ModelBase.FormatDateOf(k.Angelegt_Am, k.Angelegt_Von);
          geaendert.Text = ModelBase.FormatDateOf(k.Geaendert_Am, k.Geaendert_Von);
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

    private void InitChildren()
    {
      var values = new List<string[]>();
      foreach (var e in ChildList)
      {
        // Nr.;Geburtsname;Vorname;Name;G.;Geändert am;Geändert von;Angelegt am;Angelegt von
        values.Add(new string[] { e.Uid, e.Geburtsname, e.Vorname, e.Name, e.Geschlecht,
          Functions.ToString(e.Geaendert_Am, true), e.Geaendert_Von,
          Functions.ToString(e.Angelegt_Am, true), e.Angelegt_Von });
      }
      AddStringColumnsSort(kinder, SB310_kinder_columns, values);
    }

    /// <summary>Behandlung von Vater.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnVaterChanged(object sender, EventArgs e)
    {
    }

    /// <summary>Behandlung von Mutter.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnMutterChanged(object sender, EventArgs e)
    {
    }

    /// <summary>Behandlung von Kinder.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnKinderRowActivated(object sender, RowActivatedArgs e)
    {
    }

    /// <summary>Behandlung von Kind.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnKindChanged(object sender, EventArgs e)
    {
    }

    /// <summary>Behandlung von Ok.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnOkClicked(object sender, EventArgs e)
    {
      ServiceErgebnis r = null;
      if (DialogType == DialogTypeEnum.New || DialogType == DialogTypeEnum.Copy
          || DialogType == DialogTypeEnum.Edit)
      {
        r = FactoryService.PedigreeService.SaveFamily(ServiceDaten,
          DialogType == DialogTypeEnum.Edit ? nr.Text : null, GetText(vater), GetText(mutter), heiratsdatum.Text,
          heiratsort.Text, heiratsbem.Buffer.Text, null, ChildList.Select(a => a.Uid).ToList());
      }
      else if (DialogType == DialogTypeEnum.Delete)
      {
        r = FactoryService.PedigreeService.DeleteFamily(ServiceDaten, Model);
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

    /// <summary>Behandlung von Hinzufuegen.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnHinzufuegenClicked(object sender, EventArgs e)
    {
      var uid = GetText(kind);
      if (string.IsNullOrEmpty(uid) || ChildList.Any(a => a.Uid == uid))
        return;
      var k = Get(FactoryService.PedigreeService.GetAncestor(ServiceDaten, uid));
      if (k != null)
      {
        ChildList.Add(k);
        InitChildren();
      }
    }

    /// <summary>Behandlung von Entfernen.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnEntfernenClicked(object sender, EventArgs e)
    {
      var uid = GetText(kinder);
      if (string.IsNullOrEmpty(uid))
        return;
      ChildList = ChildList.Where(a => a.Uid != uid).ToList();
      InitChildren();
    }

    /// <summary>Behandlung von Abbrechen.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnAbbrechenClicked(object sender, EventArgs e)
    {
      dialog.Hide();
    }
  }
}
