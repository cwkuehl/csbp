// <copyright file="WP210Stock.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.WP
{
  using System;
  using System.Linq;
  using CSBP.Apis.Enums;
  using CSBP.Apis.Models;
  using CSBP.Apis.Services;
  using CSBP.Base;
  using CSBP.Services.Factory;
  using Gtk;
  using static CSBP.Resources.Messages;

  /// <summary>Controller für WP210Stock Dialog.</summary>
  public partial class WP210Stock : CsbpBin
  {
    /// <summary>Dialog Model.</summary>
    private WpWertpapier Model;

    /// <summary>Zuletzt kopierte ID.</summary>
    public static string lastcopyuid = null;

#pragma warning disable 169, 649

    /// <summary>Label nr0.</summary>
    [Builder.Object]
    private Label nr0;

    /// <summary>Entry nr.</summary>
    [Builder.Object]
    private Entry nr;

    /// <summary>Label bezeichnung0.</summary>
    [Builder.Object]
    private Label bezeichnung0;

    /// <summary>Entry bezeichnung.</summary>
    [Builder.Object]
    private Entry bezeichnung;

    /// <summary>Label provider0.</summary>
    [Builder.Object]
    private Label provider0;

    /// <summary>ComboBox provider.</summary>
    [Builder.Object]
    private ComboBox provider;

    /// <summary>Label kuerzel0.</summary>
    [Builder.Object]
    private Label kuerzel0;

    /// <summary>Entry kuerzel.</summary>
    [Builder.Object]
    private Entry kuerzel;

    /// <summary>Label status0.</summary>
    [Builder.Object]
    private Label status0;

    /// <summary>ComboBox status.</summary>
    [Builder.Object]
    private ComboBox status;

    /// <summary>Label aktKurs0.</summary>
    [Builder.Object]
    private Label aktKurs0;

    /// <summary>Entry aktKurs.</summary>
    [Builder.Object]
    private Entry aktKurs;

    /// <summary>Label stopKurs0.</summary>
    [Builder.Object]
    private Label stopKurs0;

    /// <summary>Entry stopKurs.</summary>
    [Builder.Object]
    private Entry stopKurs;

    /// <summary>Label signalKurs10.</summary>
    [Builder.Object]
    private Label signalKurs10;

    /// <summary>Entry signalKurs1.</summary>
    [Builder.Object]
    private Entry signalKurs1;

    /// <summary>Label muster0.</summary>
    [Builder.Object]
    private Label muster0;

    /// <summary>Entry muster.</summary>
    [Builder.Object]
    private Entry muster;

    /// <summary>Label typ0.</summary>
    [Builder.Object]
    private Label typ0;

    /// <summary>Entry typ.</summary>
    [Builder.Object]
    private Entry typ;

    /// <summary>Label waehrung0.</summary>
    [Builder.Object]
    private Label waehrung0;

    /// <summary>Entry waehrung.</summary>
    [Builder.Object]
    private Entry waehrung;

    /// <summary>Label sortierung0.</summary>
    [Builder.Object]
    private Label sortierung0;

    /// <summary>Entry sortierung.</summary>
    [Builder.Object]
    private Entry sortierung;

    /// <summary>Label relation0.</summary>
    [Builder.Object]
    private Label relation0;

    /// <summary>ComboBox relation.</summary>
    [Builder.Object]
    private ComboBox relation;

    /// <summary>Label notiz0.</summary>
    [Builder.Object]
    private Label notiz0;

    /// <summary>TextView notiz.</summary>
    [Builder.Object]
    private TextView notiz;

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

    /// <summary>CheckButton anlage.</summary>
    [Builder.Object]
    private CheckButton anlage;

    /// <summary>Button ok.</summary>
    [Builder.Object]
    private Button ok;

    /// <summary>Button abbrechen.</summary>
    [Builder.Object]
    private Button abbrechen;

#pragma warning restore 169, 649

    /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public static WP210Stock Create(object p1 = null, CsbpBin p = null)
    {
      return new WP210Stock(GetBuilder("WP210Stock", out var handle), handle, p1: p1, p: p);
    }

    /// <summary>Konstruktor für modalen Dialog.</summary>
    /// <param name="builder">Betroffener Builder.</param>
    /// <param name="h">Betroffenes Handle vom Builder.</param>
    /// <param name="d">Betroffener einbettender Dialog.</param>
    /// <param name="dt">Betroffener Dialogtyp.</param>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public WP210Stock(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
        : base(b, h, d, dt, p1, p)
    {
      SetBold(bezeichnung0);
      SetBold(provider0);
      SetBold(kuerzel0);
      SetBold(status0);
      InitData(0);
      bezeichnung.GrabFocus();
    }

    /// <summary>Model-Daten initialisieren.</summary>
    /// <param name="step">Betroffener Schritt: 0 erstmalig, 1 aktualisieren.</param>
    override protected void InitData(int step)
    {
      if (step <= 0)
      {
        var pl = Get(FactoryService.StockService.GetProviderList(ServiceDaten));
        AddColumns(provider, pl);
        AddColumns(status, Get(FactoryService.StockService.GetStateList(ServiceDaten)));
        if (pl != null && pl.Any())
          SetText(provider, pl.First().Schluessel);
        SetText(status, "1");
        var neu = DialogType == DialogTypeEnum.New;
        var loeschen = DialogType == DialogTypeEnum.Delete;
        var copy = DialogType == DialogTypeEnum.Copy;
        var uid = Parameter1 as string;
        if (!neu && uid != null)
        {
          var k = Get(FactoryService.StockService.GetStock(ServiceDaten, uid));
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
          bezeichnung.Text = k.Bezeichnung;
          SetText(provider, k.Datenquelle);
          kuerzel.Text = k.Kuerzel;
          SetText(status, k.Status);
          aktKurs.Text = Functions.ToString(k.CurrentPrice);
          stopKurs.Text = Functions.ToString(k.StopPrice);
          signalKurs1.Text = Functions.ToString(k.SignalPrice1);
          muster.Text = k.Pattern;
          typ.Text = k.Type;
          waehrung.Text = k.Currency;
          sortierung.Text = k.Sorting;
          SetText(relation, k.Relation_Uid);
          notiz.Buffer.Text = k.Notiz ?? "";
          angelegt.Text = ModelBase.FormatDateOf(k.Angelegt_Am, k.Angelegt_Von);
          geaendert.Text = ModelBase.FormatDateOf(k.Geaendert_Am, k.Geaendert_Von);
        }
        if (neu || copy)
          anlage.Active = true;
        nr.IsEditable = false;
        bezeichnung.IsEditable = !loeschen;
        provider.Sensitive = !loeschen;
        kuerzel.IsEditable = !loeschen;
        status.Sensitive = !loeschen;
        aktKurs.IsEditable = false;
        stopKurs.IsEditable = false;
        signalKurs1.IsEditable = !loeschen;
        muster.IsEditable = false;
        typ.IsEditable = !loeschen;
        waehrung.IsEditable = !loeschen;
        sortierung.IsEditable = !loeschen;
        relation.Sensitive = !loeschen;
        notiz.Editable = !loeschen;
        angelegt.IsEditable = false;
        geaendert.IsEditable = false;
        anlage.Sensitive = !loeschen;
        if (loeschen)
          ok.Label = Forms_delete;
        var rl = Get(FactoryService.StockService.GetStockList(ServiceDaten, true, null, null, copy ? null : Model?.Uid));
        var rs = AddColumns(relation);
        foreach (var p in rl)
          rs.AppendValues(p.Bezeichnung, p.Uid);
        if (!neu && Model != null)
          SetText(relation, Model.Relation_Uid);
      }
    }

    /// <summary>Behandlung von Provider.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnProviderChanged(object sender, EventArgs e)
    {
    }

    /// <summary>Behandlung von Status.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnStatusChanged(object sender, EventArgs e)
    {
    }

    /// <summary>Behandlung von Relation.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnRelationChanged(object sender, EventArgs e)
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
        var rb = FactoryService.StockService.SaveStock(ServiceDaten,
          DialogType == DialogTypeEnum.Edit ? nr.Text : null, bezeichnung.Text, kuerzel.Text,
          Functions.ToDecimal(signalKurs1.Text, 4), sortierung.Text,
          GetText(provider), GetText(status), GetText(relation), notiz.Buffer.Text,
          typ.Text, waehrung.Text, anlage.Active);
        r = rb;
        if (rb.Ok && rb.Ergebnis != null && DialogType == DialogTypeEnum.Copy)
          lastcopyuid = rb.Ergebnis.Uid;
      }
      else if (DialogType == DialogTypeEnum.Delete)
      {
        r = FactoryService.StockService.DeleteStock(ServiceDaten, Model);
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

    /// <summary>Behandlung von Abbrechen.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnAbbrechenClicked(object sender, EventArgs e)
    {
      dialog.Hide();
    }
  }
}
