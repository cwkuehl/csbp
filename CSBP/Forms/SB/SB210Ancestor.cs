// <copyright file="SB210Ancestor.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.SB;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using CSBP.Apis.Enums;
using CSBP.Apis.Models;
using CSBP.Apis.Services;
using CSBP.Base;
using CSBP.Services.Factory;
using Gtk;
using static CSBP.Resources.Messages;

/// <summary>Controller for SB210Ancestor dialog.</summary>
public partial class SB210Ancestor : CsbpBin
{
#pragma warning disable CS0649

  /// <summary>Entry nr.</summary>
  [Builder.Object]
  private readonly Entry nr;

  /// <summary>Entry geburtsname.</summary>
  [Builder.Object]
  private readonly Entry geburtsname;

  /// <summary>Entry vorname.</summary>
  [Builder.Object]
  private readonly Entry vorname;

  /// <summary>Entry name.</summary>
  [Builder.Object]
  private readonly Entry name;

  /// <summary>Box Biler.</summary>
  [Builder.Object]
  private readonly Box bilder;

  /// <summary>Label geschlecht0.</summary>
  [Builder.Object]
  private readonly Label geschlecht0;

  /// <summary>RadioButton geschlecht1.</summary>
  [Builder.Object]
  private readonly RadioButton geschlecht1;

  /// <summary>RadioButton geschlecht2.</summary>
  [Builder.Object]
  private readonly RadioButton geschlecht2;

  /// <summary>RadioButton geschlecht3.</summary>
  [Builder.Object]
  private readonly RadioButton geschlecht3;

  /// <summary>TextView bilddaten.</summary>
  [Builder.Object]
  private readonly TextView bilddaten;

  /// <summary>Entry geburtsdatum.</summary>
  [Builder.Object]
  private readonly Entry geburtsdatum;

  /// <summary>Entry geburtsort.</summary>
  [Builder.Object]
  private readonly Entry geburtsort;

  /// <summary>TextView geburtsbem.</summary>
  [Builder.Object]
  private readonly TextView geburtsbem;

  /// <summary>Entry taufdatum.</summary>
  [Builder.Object]
  private readonly Entry taufdatum;

  /// <summary>Entry taufort.</summary>
  [Builder.Object]
  private readonly Entry taufort;

  /// <summary>TextView taufbem.</summary>
  [Builder.Object]
  private readonly TextView taufbem;

  /// <summary>Entry todesdatum.</summary>
  [Builder.Object]
  private readonly Entry todesdatum;

  /// <summary>Entry todesort.</summary>
  [Builder.Object]
  private readonly Entry todesort;

  /// <summary>TextView todesbem.</summary>
  [Builder.Object]
  private readonly TextView todesbem;

  /// <summary>Entry begraebnisdatum.</summary>
  [Builder.Object]
  private readonly Entry begraebnisdatum;

  /// <summary>Entry begraebnisort.</summary>
  [Builder.Object]
  private readonly Entry begraebnisort;

  /// <summary>TextView begraebnisbem.</summary>
  [Builder.Object]
  private readonly TextView begraebnisbem;

  /// <summary>Entry konfession.</summary>
  [Builder.Object]
  private readonly Entry konfession;

  /// <summary>Entry titel.</summary>
  [Builder.Object]
  private readonly Entry titel;

  /// <summary>TextView bemerkung.</summary>
  [Builder.Object]
  private readonly TextView bemerkung;

  /// <summary>Entry gatte.</summary>
  [Builder.Object]
  private readonly Entry gatte;

  /// <summary>Entry gatteNr.</summary>
  [Builder.Object]
  private readonly Entry gatteNr;

  /// <summary>Entry vater.</summary>
  [Builder.Object]
  private readonly Entry vater;

  /// <summary>Entry vaterNr.</summary>
  [Builder.Object]
  private readonly Entry vaterNr;

  /// <summary>Entry mutter.</summary>
  [Builder.Object]
  private readonly Entry mutter;

  /// <summary>Entry mutterNr.</summary>
  [Builder.Object]
  private readonly Entry mutterNr;

  /// <summary>ComboBox quelle.</summary>
  [Builder.Object]
  private readonly ComboBox quelle;

  /// <summary>Entry status1.</summary>
  [Builder.Object]
  private readonly Entry status1;

  /// <summary>Entry status2.</summary>
  [Builder.Object]
  private readonly Entry status2;

  /// <summary>Entry status3.</summary>
  [Builder.Object]
  private readonly Entry status3;

  /// <summary>Entry angelegt.</summary>
  [Builder.Object]
  private readonly Entry angelegt;

  /// <summary>Entry geaendert.</summary>
  [Builder.Object]
  private readonly Entry geaendert;

  /// <summary>Button ok.</summary>
  [Builder.Object]
  private readonly Button ok;

  /// <summary>Button hinzufuegen.</summary>
  [Builder.Object]
  private readonly Button hinzufuegen;

  /// <summary>List of images.</summary>
  private List<ByteDaten> imagelist = new();

#pragma warning restore CS0649

  /// <summary>Dialog model.</summary>
  private SbPerson model;

  /// <summary>Initializes a new instance of the <see cref="SB210Ancestor"/> class.</summary>
  /// <param name="b">Affected Builder.</param>
  /// <param name="h">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="type">Affected dialog class type.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  public SB210Ancestor(Builder b, IntPtr h, Dialog d = null, Type type = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, type ?? typeof(SB210Ancestor), dt, p1, p)
  {
    SetBold(geburtsname);
    SetBold(geschlecht0);
    bilddaten.DragDataReceived += OnBilderDragDataReceived;
    InitData(0);
    geburtsname.GrabFocus();
  }

  /// <summary>Creates non modal dialog.</summary>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  /// <returns>Created dialog.</returns>
  public static SB210Ancestor Create(object p1 = null, CsbpBin p = null)
  {
    return new SB210Ancestor(GetBuilder("SB210Ancestor", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  protected override void InitData(int step)
  {
    var daten = ServiceDaten;
    if (step <= 0)
    {
      EventsActive = false;
      SetUserData(new[] { geschlecht1, geschlecht2, geschlecht3 },
        new[] { GenderEnum.MAENNLICH.ToString(), GenderEnum.WEIBLICH.ToString(), GenderEnum.NEUTRUM.ToString() });
      var sl = Get(FactoryService.PedigreeService.GetSourceList(daten));
      var rs = AddColumns(quelle, emptyentry: true);
      foreach (var p in sl)
        rs.AppendValues(p.SourceName, p.Uid);
      var neu = DialogType == DialogTypeEnum.New;
      var loeschen = DialogType == DialogTypeEnum.Delete;
      var aendern = DialogType == DialogTypeEnum.Edit;
      if (!neu && Parameter1 is string uid)
      {
        var k = Get(FactoryService.PedigreeService.GetAncestor(daten, uid));
        if (k == null)
        {
          Application.Invoke((sender, e) => { CloseDialog(); });
          return;
        }
        model = k;
        SetText(nr, k.Uid);
        SetText(geburtsname, k.Geburtsname);
        SetText(vorname, k.Vorname);
        SetText(name, k.Name);
        imagelist = Get(FactoryService.PedigreeService.GetBytes(daten, k.Uid));
        if (imagelist != null)
        {
          foreach (var b in imagelist)
          {
            AppendImage(b.Bytes, b.Metadaten);
          }
        }
        SetText(geschlecht1, k.Geschlecht);
        SetText(geburtsdatum, k.Birthdate);
        SetText(geburtsort, k.Birthplace);
        SetText(geburtsbem, k.Birthmemo);
        SetText(taufdatum, k.Christdate);
        SetText(taufort, k.Christplace);
        SetText(taufbem, k.Christmemo);
        SetText(todesdatum, k.Deathdate);
        SetText(todesort, k.Deathplace);
        SetText(todesbem, k.Deathmemo);
        SetText(begraebnisdatum, k.Burialdate);
        SetText(begraebnisort, k.Burialplace);
        SetText(begraebnisbem, k.Burialmemo);
        SetText(konfession, k.Konfession);
        SetText(titel, k.Titel);
        SetText(bemerkung, k.Bemerkung);
        var g = Get(FactoryService.PedigreeService.GetSpouseList(daten, uid)); // alle Ehegatten
        if (g != null)
        {
          var sb = new StringBuilder();
          foreach (var p in g)
          {
            Functions.Append(sb, "; ", p.AncestorName);
          }
          SetText(gatte, sb.ToString());
        }
        SetText(vater, Functions.AhnString(k.Father?.Uid, k.Father?.Geburtsname, k.Father?.Vorname));
        SetText(mutter, Functions.AhnString(k.Mother?.Uid, k.Mother?.Geburtsname, k.Mother?.Vorname));
        if (DialogType == DialogTypeEnum.Copy)
        {
          SetText(gatteNr, k.Uid);
          SetText(vaterNr, k.Father?.Uid);
          SetText(mutterNr, k.Mother?.Uid);
        }
        SetText(quelle, k.Quelle_Uid);
        SetText(status1, Functions.ToString(k.Status1));
        SetText(status2, Functions.ToString(k.Status2));
        SetText(status3, Functions.ToString(k.Status3));
        SetText(angelegt, ModelBase.FormatDateOf(k.Angelegt_Am, k.Angelegt_Von));
        SetText(geaendert, ModelBase.FormatDateOf(k.Geaendert_Am, k.Geaendert_Von));
      }
      nr.IsEditable = false;
      geburtsname.IsEditable = !loeschen;
      vorname.IsEditable = !loeschen;
      name.IsEditable = !loeschen;
      bilddaten.Sensitive = !loeschen;
      foreach (RadioButton a in geschlecht1.Group)
        a.Sensitive = !loeschen;
      geburtsdatum.IsEditable = !loeschen;
      geburtsort.IsEditable = !loeschen;
      geburtsbem.Editable = !loeschen;
      taufdatum.IsEditable = !loeschen;
      taufort.IsEditable = !loeschen;
      taufbem.Editable = !loeschen;
      todesdatum.IsEditable = !loeschen;
      todesort.IsEditable = !loeschen;
      todesbem.Editable = !loeschen;
      begraebnisdatum.IsEditable = !loeschen;
      begraebnisort.IsEditable = !loeschen;
      begraebnisbem.Editable = !loeschen;
      konfession.IsEditable = !loeschen;
      titel.IsEditable = !loeschen;
      bemerkung.Editable = !loeschen;
      gatte.IsEditable = false;
      gatteNr.IsEditable = !loeschen;
      vater.IsEditable = false;
      vaterNr.IsEditable = !loeschen;
      mutter.IsEditable = false;
      mutterNr.IsEditable = !loeschen;
      quelle.Sensitive = !loeschen;
      status1.IsEditable = !loeschen;
      status2.IsEditable = !loeschen;
      status3.IsEditable = !loeschen;
      angelegt.IsEditable = false;
      geaendert.IsEditable = false;
      if (loeschen)
        ok.Label = Forms_delete;
      hinzufuegen.Sensitive = !loeschen;
      EventsActive = true;
    }
  }

  /// <summary>Handles Bilder.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnBilderDragDataReceived(object sender, DragDataReceivedArgs e)
  {
    if (e.SelectionData.Text != null)
    {
      var arr = Regex.Split(e.SelectionData.Text, "\r\n|\r|\n");
      foreach (var file in arr)
      {
        var m = Regex.Match(file, @"^(file:\/*)(.+?)$"); // Windows bzw. Linux
        if (m.Success)
        {
          var f = m.Groups[2].Value;
          if (Functions.IsLinux())
            f = $"/{f}";
          AppendImageFile(f, file);
        }
      }
    }
  }

  /// <summary>Handles Quelle.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnQuelleChanged(object sender, EventArgs e)
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
      var list = ParseMetadata(imagelist, bilddaten.Buffer.Text);
      r = FactoryService.PedigreeService.SaveAncestor(ServiceDaten,
        DialogType == DialogTypeEnum.Edit ? nr.Text : null, name.Text, vorname.Text, geburtsname.Text,
        GetText(geschlecht1), titel.Text, konfession.Text, bemerkung.Buffer.Text, GetText(quelle),
        Functions.ToInt32(status1.Text), Functions.ToInt32(status2.Text), Functions.ToInt32(status3.Text),
        geburtsdatum.Text, geburtsort.Text, geburtsbem.Buffer.Text, null,
        taufdatum.Text, taufort.Text, taufbem.Buffer.Text, null,
        todesdatum.Text, todesort.Text, todesbem.Buffer.Text, null,
        begraebnisdatum.Text, begraebnisort.Text, begraebnisbem.Buffer.Text, null,
        gatteNr.Text, vaterNr.Text, mutterNr.Text, list);
    }
    else if (DialogType == DialogTypeEnum.Delete)
    {
      r = FactoryService.PedigreeService.DeleteAncestor(ServiceDaten, model);
    }
    if (r != null)
    {
      Get(r);
      if (r.Ok)
      {
        UpdateParent();
        CloseDialog();
      }
    }
  }

  /// <summary>Handles Hinzufuegen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnHinzufuegenClicked(object sender, EventArgs e)
  {
    var file = SelectFile(SB210_select_file, "*.png", SB210_select_ext);
    if (!string.IsNullOrEmpty(file))
    {
      AppendImageFile(file);
    }
  }

  /// <summary>Handles Abbrechen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAbbrechenClicked(object sender, EventArgs e)
  {
    CloseDialog();
  }

  /// <summary>
  /// Append image file.
  /// </summary>
  /// <param name="list">Affected image list.</param>
  /// <param name="metadata">Affected meta data for all images.</param>
  private static List<ByteDaten> ParseMetadata(List<ByteDaten> list, string metadata)
  {
    if (list == null || list.Count <= 0)
      return list;
    var xml = $"<images>{Functions.ToString(metadata)}</images>";
    var doc = new XmlDocument();
    doc.Load(new StringReader(xml));
    var root = doc.DocumentElement;
    var images = root.SelectNodes("/images/image");
    if (images.Count != list.Count)
    {
      throw new Exception("Parse error of meta data.");
      //// ShowError("Parse error of meta data.");
      //// return list;
    }
    for (var i = 0; i < list.Count; i++)
    {
      var image = images[i];
      list[i].Metadaten = image.OuterXml;
    }
    return list;
  }

  /// <summary>
  /// Append image and meta data.
  /// </summary>
  /// <param name="bytes">Image as bytes.</param>
  /// <param name="metadata">Affected meta data.</param>
  /// <param name="append">Append to imagelist.</param>
  /// <param name="dropname">Affected drop file name which has to be removed.</param>
  private void AppendImage(byte[] bytes, string metadata, bool append = false, string dropname = null)
  {
    if (bytes == null || string.IsNullOrEmpty(metadata))
      return;
    var p = new Image
    {
      Pixbuf = new Gdk.Pixbuf(bytes),
    };
    bilder.Add(p);
    if (!string.IsNullOrWhiteSpace(dropname))
      SetText(bilddaten, bilddaten.Buffer.Text.Replace(dropname, ""));
    SetText(bilddaten, Functions.Append(bilddaten.Buffer.Text, Constants.CRLF, metadata));
    if (append)
    {
      imagelist.Add(new ByteDaten
      {
        Metadaten = metadata,
        Bytes = bytes,
      });
    }
  }

  /// <summary>
  /// Append image file.
  /// </summary>
  /// <param name="file">Affected file name.</param>
  /// <param name="dropname">Affected drop file name which has to be removed.</param>
  private void AppendImageFile(string file, string dropname = null)
  {
    var bytes = File.ReadAllBytes(file);
    var date = File.GetCreationTimeUtc(file);
    var length = bytes.Length;
    var metadaten = $"<image><text>Bild</text><file>{file}</file><date>{date:yyyy-MM-dd'T'hh:mm:ss'Z'}</date><size>{length}</size></image>";
    AppendImage(bytes, metadaten, true, dropname);
    bilder.ShowAll();
  }
}
