// <copyright file="SB210Ancestor.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.SB
{
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

  /// <summary>Controller für SB210Ancestor Dialog.</summary>
  public partial class SB210Ancestor : CsbpBin
  {
    /// <summary>Dialog Model.</summary>
    private SbPerson Model;

#pragma warning disable 169, 649

    /// <summary>Label nr0.</summary>
    [Builder.Object]
    private Label nr0;

    /// <summary>Entry nr.</summary>
    [Builder.Object]
    private Entry nr;

    /// <summary>Label geburtsname0.</summary>
    [Builder.Object]
    private Label geburtsname0;

    /// <summary>Entry geburtsname.</summary>
    [Builder.Object]
    private Entry geburtsname;

    /// <summary>Label vorname0.</summary>
    [Builder.Object]
    private Label vorname0;

    /// <summary>Entry vorname.</summary>
    [Builder.Object]
    private Entry vorname;

    /// <summary>Label name0.</summary>
    [Builder.Object]
    private Label name0;

    /// <summary>Entry name.</summary>
    [Builder.Object]
    private Entry name;

    /// <summary>Label bildersw.</summary>
    [Builder.Object]
    private ScrolledWindow bildersw;

    /// <summary>Box Biler.</summary>
    [Builder.Object]
    private Box bilder;

    /// <summary>Label geschlecht0.</summary>
    [Builder.Object]
    private Label geschlecht0;

    /// <summary>RadioButton geschlecht1.</summary>
    [Builder.Object]
    private RadioButton geschlecht1;

    /// <summary>RadioButton geschlecht2.</summary>
    [Builder.Object]
    private RadioButton geschlecht2;

    /// <summary>RadioButton geschlecht3.</summary>
    [Builder.Object]
    private RadioButton geschlecht3;

    /// <summary>Label bilddaten0.</summary>
    [Builder.Object]
    private Label bilddaten0;

    /// <summary>TextView bilddaten.</summary>
    [Builder.Object]
    private TextView bilddaten;

    /// <summary>Label geburtsdatum0.</summary>
    [Builder.Object]
    private Label geburtsdatum0;

    /// <summary>Entry geburtsdatum.</summary>
    [Builder.Object]
    private Entry geburtsdatum;

    /// <summary>Label geburtsort0.</summary>
    [Builder.Object]
    private Label geburtsort0;

    /// <summary>Entry geburtsort.</summary>
    [Builder.Object]
    private Entry geburtsort;

    /// <summary>Label geburtsbem0.</summary>
    [Builder.Object]
    private Label geburtsbem0;

    /// <summary>TextView geburtsbem.</summary>
    [Builder.Object]
    private TextView geburtsbem;

    /// <summary>Label taufdatum0.</summary>
    [Builder.Object]
    private Label taufdatum0;

    /// <summary>Entry taufdatum.</summary>
    [Builder.Object]
    private Entry taufdatum;

    /// <summary>Label taufort0.</summary>
    [Builder.Object]
    private Label taufort0;

    /// <summary>Entry taufort.</summary>
    [Builder.Object]
    private Entry taufort;

    /// <summary>Label taufbem0.</summary>
    [Builder.Object]
    private Label taufbem0;

    /// <summary>TextView taufbem.</summary>
    [Builder.Object]
    private TextView taufbem;

    /// <summary>Label todesdatum0.</summary>
    [Builder.Object]
    private Label todesdatum0;

    /// <summary>Entry todesdatum.</summary>
    [Builder.Object]
    private Entry todesdatum;

    /// <summary>Label todesort0.</summary>
    [Builder.Object]
    private Label todesort0;

    /// <summary>Entry todesort.</summary>
    [Builder.Object]
    private Entry todesort;

    /// <summary>Label todesbem0.</summary>
    [Builder.Object]
    private Label todesbem0;

    /// <summary>TextView todesbem.</summary>
    [Builder.Object]
    private TextView todesbem;

    /// <summary>Label begraebnisdatum0.</summary>
    [Builder.Object]
    private Label begraebnisdatum0;

    /// <summary>Entry begraebnisdatum.</summary>
    [Builder.Object]
    private Entry begraebnisdatum;

    /// <summary>Label begraebnisort0.</summary>
    [Builder.Object]
    private Label begraebnisort0;

    /// <summary>Entry begraebnisort.</summary>
    [Builder.Object]
    private Entry begraebnisort;

    /// <summary>Label begraebnisbem0.</summary>
    [Builder.Object]
    private Label begraebnisbem0;

    /// <summary>TextView begraebnisbem.</summary>
    [Builder.Object]
    private TextView begraebnisbem;

    /// <summary>Label konfession0.</summary>
    [Builder.Object]
    private Label konfession0;

    /// <summary>Entry konfession.</summary>
    [Builder.Object]
    private Entry konfession;

    /// <summary>Label titel0.</summary>
    [Builder.Object]
    private Label titel0;

    /// <summary>Entry titel.</summary>
    [Builder.Object]
    private Entry titel;

    /// <summary>Label bemerkung0.</summary>
    [Builder.Object]
    private Label bemerkung0;

    /// <summary>TextView bemerkung.</summary>
    [Builder.Object]
    private TextView bemerkung;

    /// <summary>Label gatte0.</summary>
    [Builder.Object]
    private Label gatte0;

    /// <summary>Entry gatte.</summary>
    [Builder.Object]
    private Entry gatte;

    /// <summary>Label gatteNr0.</summary>
    [Builder.Object]
    private Label gatteNr0;

    /// <summary>Entry gatteNr.</summary>
    [Builder.Object]
    private Entry gatteNr;

    /// <summary>Label vater0.</summary>
    [Builder.Object]
    private Label vater0;

    /// <summary>Entry vater.</summary>
    [Builder.Object]
    private Entry vater;

    /// <summary>Label vaterNr0.</summary>
    [Builder.Object]
    private Label vaterNr0;

    /// <summary>Entry vaterNr.</summary>
    [Builder.Object]
    private Entry vaterNr;

    /// <summary>Label mutter0.</summary>
    [Builder.Object]
    private Label mutter0;

    /// <summary>Entry mutter.</summary>
    [Builder.Object]
    private Entry mutter;

    /// <summary>Label mutterNr0.</summary>
    [Builder.Object]
    private Label mutterNr0;

    /// <summary>Entry mutterNr.</summary>
    [Builder.Object]
    private Entry mutterNr;

    /// <summary>Label quelle0.</summary>
    [Builder.Object]
    private Label quelle0;

    /// <summary>ComboBox quelle.</summary>
    [Builder.Object]
    private ComboBox quelle;

    /// <summary>Label status10.</summary>
    [Builder.Object]
    private Label status10;

    /// <summary>Entry status1.</summary>
    [Builder.Object]
    private Entry status1;

    /// <summary>Label status20.</summary>
    [Builder.Object]
    private Label status20;

    /// <summary>Entry status2.</summary>
    [Builder.Object]
    private Entry status2;

    /// <summary>Label status30.</summary>
    [Builder.Object]
    private Label status30;

    /// <summary>Entry status3.</summary>
    [Builder.Object]
    private Entry status3;

    /// <summary>Label angelegt0.</summary>
    [Builder.Object]
    private Label angelegt0;

    /// <summary>Entry angelegt.</summary>
    [Builder.Object]
    private Entry angelegt;

    /// <summary>Label geaendert0.</summary>
    [Builder.Object]
    private Label geaendert0;

    /// <summary>Entry geaendert.</summary>
    [Builder.Object]
    private Entry geaendert;

    /// <summary>Button ok.</summary>
    [Builder.Object]
    private Button ok;

    /// <summary>Button hinzufuegen.</summary>
    [Builder.Object]
    private Button hinzufuegen;

    /// <summary>Button abbrechen.</summary>
    [Builder.Object]
    private Button abbrechen;

    /// <summary>List of images.</summary>
    private List<ByteDaten> imagelist = new List<ByteDaten>();

#pragma warning restore 169, 649

    /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public static SB210Ancestor Create(object p1 = null, CsbpBin p = null)
    {
      return new SB210Ancestor(GetBuilder("SB210Ancestor", out var handle), handle, p1: p1, p: p);
    }

    /// <summary>Konstruktor für modalen Dialog.</summary>
    /// <param name="builder">Betroffener Builder.</param>
    /// <param name="h">Betroffenes Handle vom Builder.</param>
    /// <param name="d">Betroffener einbettender Dialog.</param>
    /// <param name="dt">Betroffener Dialogtyp.</param>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public SB210Ancestor(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
        : base(b, h, d, dt, p1, p)
    {
      SetBold(geburtsname);
      SetBold(geschlecht0);
      bilddaten.DragDataReceived += OnBilderDragDataReceived;
      InitData(0);
      geburtsname.GrabFocus();
    }

    /// <summary>Model-Daten initialisieren.</summary>
    /// <param name="step">Betroffener Schritt: 0 erstmalig, 1 aktualisieren.</param>
    override protected void InitData(int step)
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
        var uid = Parameter1 as string;
        if (!neu && uid != null)
        {
          var k = Get(FactoryService.PedigreeService.GetAncestor(daten, uid));
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
          geburtsname.Text = k.Geburtsname ?? "";
          vorname.Text = k.Vorname ?? "";
          name.Text = k.Name ?? "";
          imagelist = Get(FactoryService.PedigreeService.GetBytes(daten, k.Uid));
          if (imagelist != null)
          {
            foreach (var b in imagelist)
            {
              AppendImage(b.Bytes, b.Metadaten);
            }
          }
          SetText(geschlecht1, k.Geschlecht);
          geburtsdatum.Text = k.Birthdate ?? "";
          geburtsort.Text = k.Birthplace ?? "";
          geburtsbem.Buffer.Text = k.Birthmemo ?? "";
          taufdatum.Text = k.Christdate ?? "";
          taufort.Text = k.Christplace ?? "";
          taufbem.Buffer.Text = k.Christmemo ?? "";
          todesdatum.Text = k.Deathdate ?? "";
          todesort.Text = k.Deathplace ?? "";
          todesbem.Buffer.Text = k.Deathmemo ?? "";
          begraebnisdatum.Text = k.Burialdate ?? "";
          begraebnisort.Text = k.Burialplace ?? "";
          begraebnisbem.Buffer.Text = k.Burialmemo ?? "";
          konfession.Text = k.Konfession ?? "";
          titel.Text = k.Titel ?? "";
          bemerkung.Buffer.Text = k.Bemerkung ?? "";
          var g = Get(FactoryService.PedigreeService.GetSpouseList(daten, uid)); // alle Ehegatten
          if (g != null)
          {
            var sb = new StringBuilder();
            foreach (var p in g)
            {
              Functions.Append(sb, "; ", p.AncestorName);
            }
            gatte.Text = sb.ToString();
          }
          vater.Text = Functions.AhnString(k.Father?.Uid, k.Father?.Geburtsname, k.Father?.Vorname);
          mutter.Text = Functions.AhnString(k.Mother?.Uid, k.Mother?.Geburtsname, k.Mother?.Vorname);
          if (DialogType == DialogTypeEnum.Copy)
          {
            gatteNr.Text = k.Uid ?? "";
            vaterNr.Text = k.Father?.Uid ?? "";
            mutterNr.Text = k.Mother?.Uid ?? "";
          }
          SetText(quelle, k.Quelle_Uid);
          status1.Text = Functions.ToString(k.Status1);
          status2.Text = Functions.ToString(k.Status2);
          status3.Text = Functions.ToString(k.Status3);
          angelegt.Text = k.FormatDateOf(k.Angelegt_Am, k.Angelegt_Von);
          geaendert.Text = k.FormatDateOf(k.Geaendert_Am, k.Geaendert_Von);
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

    /// <summary>Behandlung von Bilder.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnBilderDragDataReceived(object o, DragDataReceivedArgs e)
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

    /// <summary>Behandlung von Quelle.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnQuelleChanged(object sender, EventArgs e)
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
        r = FactoryService.PedigreeService.DeleteAncestor(ServiceDaten, Model);
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
      var file = SelectFile(SB210_select_file, "*.png", SB210_select_ext);
      if (!string.IsNullOrEmpty(file))
      {
        AppendImageFile(file);
      }
    }

    /// <summary>Behandlung von Abbrechen.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnAbbrechenClicked(object sender, EventArgs e)
    {
      dialog.Hide();
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
        Pixbuf = new Gdk.Pixbuf(bytes)
      };
      bilder.Add(p);
      if (!string.IsNullOrWhiteSpace(dropname))
        bilddaten.Buffer.Text = bilddaten.Buffer.Text.Replace(dropname, "");
      bilddaten.Buffer.Text = Functions.Append(bilddaten.Buffer.Text, Constants.CRLF, metadata);
      if (append)
      {
        imagelist.Add(new ByteDaten
        {
          Metadaten = metadata,
          Bytes = bytes
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

    /// <summary>
    /// Append image file.
    /// </summary>
    /// <param name="list">Affected image list.</param>
    /// <param name="metadata">Affected meta data for all images.</param>
    private List<ByteDaten> ParseMetadata(List<ByteDaten> list, string metadata)
    {
      if (list == null || list.Count <= 0)
        return list;
      var xml = $"<images>{metadata ?? ""}</images>";
      var doc = new XmlDocument();
      doc.Load(new StringReader(xml));
      var root = doc.DocumentElement;
      var images = root.SelectNodes("/images/image");
      if (images.Count != list.Count)
      {
        throw new Exception("Parse error of meta data.");
        // ShowError("Parse error of meta data.");
        // return list;
      }
      for (var i = 0; i < list.Count; i++)
      {
        var image = images[i];
        list[i].Metadaten = image.OuterXml;
      }
      // var sb = new StringBuilder();
      // String[] array = s.split("(" + Pattern.quote("</image>" + Constant.CRLF + "<image>") + "|"
      //         + Pattern.quote("</image>" + Constant.LF + "<image>") + ")");
      // for (int i = 0; array != null && i < array.length && i < byteliste.size(); i++)
      // {
      //   sb.setLength(0);
      //   if (!array[i].startsWith("<image>"))
      //   {
      //     sb.append("<image>");
      //   }
      //   sb.append(array[i]);
      //   if (!array[i].endsWith("</image>"))
      //   {
      //     sb.append("</image>");
      //   }
      //   ByteDaten bd = byteliste.get(i);
      //   bd.setMetadaten(sb.toString());
      // }
      return list;
    }
  }
}
