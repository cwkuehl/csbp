// <copyright file="FZ710Memo.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.FZ;

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using CSBP.Apis.Enums;
using CSBP.Apis.Models;
using CSBP.Apis.Services;
using CSBP.Base;
using CSBP.Forms.Controls;
using CSBP.Services.Factory;
using Gtk;
using static CSBP.Resources.Messages;

/// <summary>Controller for FZ710Memo dialog.</summary>
public partial class FZ710Memo : CsbpBin
{
  /// <summary>Dialog model.</summary>
  private FzNotiz Model;

#pragma warning disable CS0649

  /// <summary>Entry nr.</summary>
  [Builder.Object]
  private readonly Entry nr;

  /// <summary>Label thema0.</summary>
  [Builder.Object]
  private readonly Label thema0;

  /// <summary>Entry thema.</summary>
  [Builder.Object]
  private readonly Entry thema;

  /// <summary>Paned splitpane.</summary>
  [Builder.Object]
  private readonly Paned splitpane;

  /// <summary>TextView notiz.</summary>
  [Builder.Object]
  private readonly TextView notiz;

  /// <summary>TreeView tabelle.</summary>
  [Builder.Object]
  private readonly TreeView tabelle;

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

  /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
  /// <param name="p1">1. Parameter für Dialog.</param>
  /// <param name="p">Betroffener Eltern-Dialog.</param>
  /// <returns>Nicht-modalen Dialogs.</returns>
  public static FZ710Memo Create(object p1 = null, CsbpBin p = null)
  {
    return new FZ710Memo(GetBuilder("FZ710Memo", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Konstruktor für modalen Dialog.</summary>
  /// <param name="b">Betroffener Builder.</param>
  /// <param name="h">Betroffenes Handle vom Builder.</param>
  /// <param name="d">Betroffener einbettender Dialog.</param>
  /// <param name="dt">Betroffener Dialogtyp.</param>
  /// <param name="p1">1. Parameter für Dialog.</param>
  /// <param name="p">Betroffener Eltern-Dialog.</param>
  /// <returns>Nicht-modalen Dialogs.</returns>
  public FZ710Memo(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, dt, p1, p)
  {
    SetBold(thema0);
    InitData(0);
    if (string.IsNullOrEmpty(thema.Text))
      thema.GrabFocus();
    else
      notiz.GrabFocus();
  }

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  protected override void InitData(int step)
  {
    var daten = ServiceDaten;
    if (step <= 0)
    {
      var neu = DialogType == DialogTypeEnum.New;
      var loeschen = DialogType == DialogTypeEnum.Delete;
      if (!neu && Parameter1 is string uid)
      {
        var k = Get(FactoryService.PrivateService.GetMemo(ServiceDaten, uid));
        if (k == null)
        {
          Application.Invoke(delegate
          {
            dialog.Hide();
          });
          return;
        }
        Model = k;
        nr.Text = k.Uid;
        thema.Text = k.Thema ?? "";
        notiz.Buffer.Text = GetMemo(k.Notiz) ?? "";
        angelegt.Text = ModelBase.FormatDateOf(k.Angelegt_Am, k.Angelegt_Von);
        geaendert.Text = ModelBase.FormatDateOf(k.Geaendert_Am, k.Geaendert_Von);
      }
      else
        GetMemo(null);
      nr.IsEditable = false;
      thema.IsEditable = !loeschen;
      notiz.Editable = !loeschen;
      tabelle.Sensitive = !loeschen;
      angelegt.IsEditable = false;
      geaendert.IsEditable = false;
      if (loeschen)
        ok.Label = Forms_delete;
    }
  }

  /// <summary>Handles Tabelle.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnTabelleRowActivated(object sender, RowActivatedArgs e)
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
      var notes = SetMemo(tabelle.Model, tabelle.Data["flist"] as Formulas, notiz.Buffer.Text, splitpane.Position);
      r = FactoryService.PrivateService.SaveMemo(ServiceDaten,
          DialogType == DialogTypeEnum.Edit ? nr.Text : null, thema.Text, notes);
    }
    else if (DialogType == DialogTypeEnum.Delete)
    {
      r = FactoryService.PrivateService.DeleteMemo(ServiceDaten, Model);
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

  /// <summary>Handles Abbrechen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAbbrechenClicked(object sender, EventArgs e)
  {
    dialog.Hide();
  }

  /// <summary>
  /// Initialize Memo.
  /// </summary>
  /// <returns>Memo as XML string.</returns>
  private static string InitMemo()
  {
    var zeilen = 1;
    var spalten = 2;
    var teiler = 0;
    var doc = new XDocument(new XDeclaration("1.0", "UTF-8", null));
    var table = new XElement("tabelle", new XAttribute("spalten", $"{spalten - 2}")
      , new XAttribute("zeilen", $"{zeilen}"), new XAttribute("teiler", $"{teiler}")
      , new XAttribute("breite0", "50"), new XAttribute("hoehe0", "30")
      );
    doc.Add(table);
    using var sw = new StringWriter();
    using var tw = XmlWriter.Create(sw);
    doc.WriteTo(tw);
    tw.Flush();
    return sw.GetStringBuilder().ToString();
  }

  /// <summary>
  /// Get memo from xml.
  /// </summary>
  /// <param name="xml">Affected xml.</param>
  /// <returns>memo from xml</returns>
  private string GetMemo(string xml)
  {
    if (string.IsNullOrWhiteSpace(xml))
      xml = InitMemo();
    var doc = new XmlDocument();
    doc.Load(new StringReader(xml));
    var root = doc.DocumentElement;
    var table = root.SelectSingleNode("/tabelle");
    if (table != null)
    {
      var teiler = Functions.ToInt32(table.Attributes["teiler"]?.Value);
      if (teiler > 0)
        splitpane.Position = teiler;
      var spalten = Math.Max(Functions.ToInt32(table.Attributes["spalten"]?.Value), 1);
      var zeilen = Math.Max(Functions.ToInt32(table.Attributes["zeilen"]?.Value), 1);
      var list = new List<string[]>();
      var flist = new Formulas();
      for (var i = 0; i < zeilen; i++)
      {
        var arr = new string[spalten + 2];
        arr[0] = Functions.ToString(i + 1);
        arr[1] = $"{i + 1:000}";
        var zellen = table.SelectNodes($"zelle[@y='{i}']");
        foreach (XmlElement z in zellen)
        {
          var x = Functions.ToInt32(z.Attributes["x"]?.Value);
          var formel = z.FirstChild?.InnerText;
          arr[x + 2] = formel;
          var f = Formula.Instance(formel, x, i); // Read formula.
          if (f != null)
            flist.List.Add(f);
        }
        list.Add(arr);
      }
      AddStringColumns(tabelle, spalten, DialogType != DialogTypeEnum.Delete, list, flist);
    }
    var node = root.SelectSingleNode("//tabelle//notiz");
    if (node != null)
      return node.InnerText;
    return null;
  }

  /*
   <?xml version="1.0" encoding="UTF-8"?><tabelle breite0="42" hoehe0="30" spalten="3" teiler="44" zeilen="8">
   <zelle x="0" y="0"><formel>Monat</formel><format fett="true" /></zelle><zelle x="1" y="0"><formel>Turniername</formel><format fett="true" /></zelle><zelle x="2" y="0"><formel>Ort</formel><format fett="true" /></zelle><zelle x="0" y="1"><formel>Januar</formel></zelle><zelle x="1" y="1"><formel>Hatsu Basho</formel></zelle><zelle x="2" y="1"><formel>Tokio</formel></zelle><zelle x="0" y="2"><formel>März</formel></zelle><zelle x="1" y="2"><formel>Haru Basho</formel></zelle><zelle x="2" y="2"><formel>Osaka</formel></zelle><zelle x="0" y="3"><formel>Mai</formel></zelle><zelle x="1" y="3"><formel>Natsu Basho</formel></zelle><zelle x="2" y="3"><formel>Tokio</formel></zelle><zelle x="0" y="4"><formel>Juli</formel></zelle><zelle x="1" y="4"><formel>Juli-Basho</formel></zelle><zelle x="2" y="4"><formel>Nagoya</formel></zelle><zelle x="0" y="5"><formel>September</formel></zelle><zelle x="1" y="5"><formel>Aki Basho</formel></zelle><zelle x="2" y="5"><formel>Tokio</formel></zelle><zelle x="0" y="6"><formel>November</formel></zelle><zelle x="1" y="6"><formel>Kyushu Basho</formel></zelle><zelle x="2" y="6"><formel>Fukuoka</formel></zelle>
   <zeile hoehe="30" nr="0" /><zeile hoehe="30" nr="1" /><zeile hoehe="30" nr="2" /><zeile hoehe="30" nr="3" /><zeile hoehe="30" nr="4" /><zeile hoehe="30" nr="5" /><zeile hoehe="30" nr="6" /><zeile hoehe="30" nr="7" />
   <spalte breite="90" nr="0" /><spalte breite="102" nr="1" /><spalte breite="75" nr="2" />
   <notiz>Turniere</notiz></tabelle>
   */

  private static string SetMemo(ITreeModel model, Formulas flist, string memo, int teiler)
  {
    memo = memo.TrimNull();
    var spalten = model.NColumns;
    var zeilen = 1;
    if (model.GetIterFirst(out var it))
    {
      while (model.IterNext(ref it))
        zeilen++;
    }
    var doc = new XDocument(new XDeclaration("1.0", "UTF-8", null));
    var table = new XElement("tabelle", new XAttribute("spalten", $"{spalten - 2}")
      , new XAttribute("zeilen", $"{zeilen}"), new XAttribute("teiler", $"{teiler}")
      , new XAttribute("breite0", "50"), new XAttribute("hoehe0", "30")
      );
    doc.Add(table);
    if (model.GetIterFirst(out it))
    {
      var y = 0;
      do
      {
        for (var x = 0; x < spalten - 2; x++)
        {
          var f = flist?.Get(x + 2, y); // Save formula instead of value.
          var value = f?.formula;
          if (string.IsNullOrEmpty(value))
          {
            var v = new GLib.Value();
            model.GetValue(it, x + 2, ref v);
            value = v.Val as string;
          }
          else if (f.bold)
            value = Functions.MakeBold(value);
          if (!string.IsNullOrEmpty(value))
          {
            var e = new XElement("zelle", new XAttribute("x", $"{x}"),
                    new XAttribute("y", $"{y}"), new XElement("formel", value));
            if (y == 0)
              e.Add(new XElement("format", new XAttribute("fett", "true")));
            table.Add(e);
          }
        }
        y++;
      } while (model.IterNext(ref it));
    }
    for (var y = 0; y < zeilen; y++)
    {
      table.Add(new XElement("zeile", new XAttribute("nr", $"{y}"), new XAttribute("hoehe", "30")));
    }
    for (var x = 0; x < spalten - 2; x++)
    {
      table.Add(new XElement("spalte", new XAttribute("nr", $"{x}"), new XAttribute("breite", "50")));
    }
    table.Add(new XElement("notiz", memo ?? ""));
    using var stringWriter = new StringWriter();
    using var xmlTextWriter = XmlWriter.Create(stringWriter);
    doc.WriteTo(xmlTextWriter);
    xmlTextWriter.Flush();
    return stringWriter.GetStringBuilder().ToString();
  }
}
