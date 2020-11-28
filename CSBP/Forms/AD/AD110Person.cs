// <copyright file="AD110Person.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.AD
{
  using System;
  using CSBP.Apis.Enums;
  using CSBP.Apis.Models;
  using CSBP.Apis.Services;
  using CSBP.Base;
  using CSBP.Forms.Controls;
  using CSBP.Services.Factory;
  using Gtk;
  using static CSBP.Resources.Messages;

  /// <summary>Controller für AD110Person Dialog.</summary>
  public partial class AD110Person : CsbpBin
  {
    /// <summary>Dialog Model.</summary>
    private AdSitz Model;

#pragma warning disable 169, 649

    /// <summary>Label nr0.</summary>
    [Builder.Object]
    private Label nr0;

    /// <summary>Entry nr.</summary>
    [Builder.Object]
    private Entry nr;

    /// <summary>Entry sitzNr.</summary>
    [Builder.Object]
    private Entry sitzNr;

    /// <summary>Entry adressNr.</summary>
    [Builder.Object]
    private Entry adressNr;

    /// <summary>Label titel0.</summary>
    [Builder.Object]
    private Label titel0;

    /// <summary>Entry titel.</summary>
    [Builder.Object]
    private Entry titel;

    /// <summary>Label vorname0.</summary>
    [Builder.Object]
    private Label vorname0;

    /// <summary>Entry vorname.</summary>
    [Builder.Object]
    private Entry vorname;

    /// <summary>Label praedikat0.</summary>
    [Builder.Object]
    private Label praedikat0;

    /// <summary>Entry praedikat.</summary>
    [Builder.Object]
    private Entry praedikat;

    /// <summary>Label name10.</summary>
    [Builder.Object]
    private Label name10;

    /// <summary>Entry name1.</summary>
    [Builder.Object]
    private Entry name1;

    /// <summary>Label name20.</summary>
    [Builder.Object]
    private Label name20;

    /// <summary>Entry name2.</summary>
    [Builder.Object]
    private Entry name2;

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

    /// <summary>Label geburt0.</summary>
    [Builder.Object]
    private Label geburt0;

    /// <summary>Date Geburt.</summary>
    //[Builder.Object]
    private Date geburt;

    /// <summary>Label personStatus0.</summary>
    [Builder.Object]
    private Label personStatus0;

    /// <summary>RadioButton personStatus1.</summary>
    [Builder.Object]
    private RadioButton personStatus1;

    /// <summary>RadioButton personStatus2.</summary>
    [Builder.Object]
    private RadioButton personStatus2;

    /// <summary>Label name0.</summary>
    [Builder.Object]
    private Label name0;

    /// <summary>Entry name.</summary>
    [Builder.Object]
    private Entry name;

    /// <summary>Label strasse0.</summary>
    [Builder.Object]
    private Label strasse0;

    /// <summary>Entry strasse.</summary>
    [Builder.Object]
    private Entry strasse;

    /// <summary>Label hausnr0.</summary>
    [Builder.Object]
    private Label hausnr0;

    /// <summary>Entry hausnr.</summary>
    [Builder.Object]
    private Entry hausnr;

    /// <summary>Label postfach0.</summary>
    [Builder.Object]
    private Label postfach0;

    /// <summary>Entry postfach.</summary>
    [Builder.Object]
    private Entry postfach;

    /// <summary>Label staat0.</summary>
    [Builder.Object]
    private Label staat0;

    /// <summary>Entry staat.</summary>
    [Builder.Object]
    private Entry staat;

    /// <summary>Label plz0.</summary>
    [Builder.Object]
    private Label plz0;

    /// <summary>Entry plz.</summary>
    [Builder.Object]
    private Entry plz;

    /// <summary>Label ort0.</summary>
    [Builder.Object]
    private Label ort0;

    /// <summary>Entry ort.</summary>
    [Builder.Object]
    private Entry ort;

    /// <summary>Label telefon0.</summary>
    [Builder.Object]
    private Label telefon0;

    /// <summary>Entry telefon.</summary>
    [Builder.Object]
    private Entry telefon;

    /// <summary>Label fax0.</summary>
    [Builder.Object]
    private Label fax0;

    /// <summary>Entry fax.</summary>
    [Builder.Object]
    private Entry fax;

    /// <summary>Label mobil0.</summary>
    [Builder.Object]
    private Label mobil0;

    /// <summary>Entry mobil.</summary>
    [Builder.Object]
    private Entry mobil;

    /// <summary>Label homepage0.</summary>
    [Builder.Object]
    private Label homepage0;

    /// <summary>Entry homepage.</summary>
    [Builder.Object]
    private Entry homepage;

    /// <summary>Label email0.</summary>
    [Builder.Object]
    private Label email0;

    /// <summary>Entry email.</summary>
    [Builder.Object]
    private Entry email;

    /// <summary>Label notiz0.</summary>
    [Builder.Object]
    private Label notiz0;

    /// <summary>TextView notiz.</summary>
    [Builder.Object]
    private TextView notiz;

    /// <summary>Label sitzStatus0.</summary>
    [Builder.Object]
    private Label sitzStatus0;

    /// <summary>RadioButton sitzStatus1.</summary>
    [Builder.Object]
    private RadioButton sitzStatus1;

    /// <summary>RadioButton sitzStatus2.</summary>
    [Builder.Object]
    private RadioButton sitzStatus2;

    /// <summary>Label adresseAnzahl0.</summary>
    [Builder.Object]
    private Label adresseAnzahl0;

    /// <summary>Entry adresseAnzahl.</summary>
    [Builder.Object]
    private Entry adresseAnzahl;

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

    /// <summary>Button adresseDupl.</summary>
    [Builder.Object]
    private Button adresseDupl;

    /// <summary>Button adresseWechseln.</summary>
    [Builder.Object]
    private Button adresseWechseln;

    /// <summary>Button abbrechen.</summary>
    [Builder.Object]
    private Button abbrechen;

#pragma warning restore 169, 649

    /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public static AD110Person Create(object p1 = null, CsbpBin p = null)
    {
      return new AD110Person(GetBuilder("AD110Person", out var handle), handle, p1: p1, p: p);
    }

    /// <summary>Konstruktor für modalen Dialog.</summary>
    /// <param name="builder">Betroffener Builder.</param>
    /// <param name="h">Betroffenes Handle vom Builder.</param>
    /// <param name="d">Betroffener einbettender Dialog.</param>
    /// <param name="dt">Betroffener Dialogtyp.</param>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public AD110Person(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
        : base(b, h, d, dt, p1, p)
    {
      geburt = new Date(Builder.GetObject("geburt").Handle)
      {
        IsNullable = false,
        IsWithCalendar = true,
        IsCalendarOpen = false
      };
      geburt.DateChanged += OnGeburtDateChanged;
      geburt.Show();
      SetBold(name10);
      SetBold(geschlecht0);
      SetBold(personStatus0);
      InitData(0);
      titel.GrabFocus();
    }

    /// <summary>Model-Daten initialisieren.</summary>
    /// <param name="step">Betroffener Schritt: 0 erstmalig, 1 aktualisieren.</param>
    override protected void InitData(int step)
    {
      if (step <= 0)
      {
        var neu = DialogType == DialogTypeEnum.New;
        var loeschen = DialogType == DialogTypeEnum.Delete;
        var uid = Parameter1 as string;
        if (!neu && uid != null)
        {
          var k = Get(FactoryService.AddressService.GetSite(ServiceDaten, uid));
          if (k == null)
          {
            Application.Invoke(delegate
            {
              dialog.Hide();
            });
            return;
          }
          Model = k;
          nr.Text = k.Person?.Uid ?? "";
          sitzNr.Text = k.Uid ?? "";
          adressNr.Text = k.Address?.Uid ?? "";
          titel.Text = k.Person?.Titel ?? "";
          vorname.Text = k.Person?.Vorname ?? "";
          praedikat.Text = k.Person?.Praedikat ?? "";
          name1.Text = k.Person?.Name1 ?? "";
          name2.Text = k.Person?.Name2 ?? "";
          geschlecht1.Active = k.Person?.Geschlecht != "M" && k.Person?.Geschlecht != "F";
          geschlecht2.Active = k.Person?.Geschlecht == "M";
          geschlecht3.Active = k.Person?.Geschlecht == "F";
          geburt.Value = k.Person?.Geburt;
          personStatus1.Active = k.Person?.Person_Status == 0;
          personStatus2.Active = k.Person?.Person_Status != 0;
          name.Text = k.Name;
          postfach.Text = k.Postfach ?? "";
          telefon.Text = k.Telefon ?? "";
          fax.Text = k.Fax ?? "";
          mobil.Text = k.Mobil ?? "";
          homepage.Text = k.Homepage ?? "";
          email.Text = k.Email ?? "";
          notiz.Buffer.Text = k.Bemerkung ?? "";
          sitzStatus1.Active = k.Sitz_Status == 0;
          sitzStatus2.Active = k.Sitz_Status != 0;
          SetAddress(k.Address);
          angelegt.Text = k.FormatDateOf(k.CreatedAt, k.CreatedBy);
          geaendert.Text = k.FormatDateOf(k.ChangedAt, k.CreatedBy);
        }
        if (DialogType == DialogTypeEnum.Copy2)
        {
          sitzNr.Text = "";
          adressNr.Text = "";
        }
        nr.IsEditable = false;
        sitzNr.IsEditable = false;
        adressNr.IsEditable = false;
        titel.IsEditable = !loeschen;
        vorname.IsEditable = !loeschen;
        praedikat.IsEditable = !loeschen;
        name1.IsEditable = !loeschen;
        name2.IsEditable = !loeschen;
        foreach (RadioButton a in geschlecht1.Group)
          a.Sensitive = !loeschen;
        geburt.Sensitive = !loeschen;
        foreach (RadioButton a in personStatus1.Group)
          a.Sensitive = !loeschen;
        name.IsEditable = !loeschen;
        strasse.IsEditable = !loeschen;
        hausnr.IsEditable = !loeschen;
        postfach.IsEditable = !loeschen;
        staat.IsEditable = !loeschen;
        plz.IsEditable = !loeschen;
        ort.IsEditable = !loeschen;
        telefon.IsEditable = !loeschen;
        fax.IsEditable = !loeschen;
        mobil.IsEditable = !loeschen;
        homepage.IsEditable = !loeschen;
        email.IsEditable = !loeschen;
        notiz.Editable = !loeschen;
        foreach (RadioButton a in sitzStatus1.Group)
          a.Sensitive = !loeschen;
        adresseAnzahl.IsEditable = false;
        angelegt.IsEditable = false;
        geaendert.IsEditable = false;
        if (loeschen)
          ok.Label = Forms_delete;
      }
    }

    /// <summary>Behandlung von geburt.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnGeburtDateChanged(object sender, DateChangedEventArgs e)
    {
    }

    /// <summary>Behandlung von Ok.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnOkClicked(object sender, EventArgs e)
    {
      ServiceErgebnis r = null;
      if (DialogType == DialogTypeEnum.New || DialogType == DialogTypeEnum.Copy
          || DialogType == DialogTypeEnum.Copy2 || DialogType == DialogTypeEnum.Edit)
      {
        var newuid = DialogType == DialogTypeEnum.New || DialogType == DialogTypeEnum.Copy;
        var uid = newuid ? null : nr.Text;
        var suid = newuid ? null : sitzNr.Text;
        var auid = newuid ? null : adressNr.Text;
        r = FactoryService.AddressService.SaveSite(ServiceDaten, uid,
            geschlecht2.Active ? "M" : geschlecht3.Active ? "F" : "N", geburt.Value, name1.Text,
            name2.Text, praedikat.Text, vorname.Text, titel.Text, personStatus1.Active ? 0 : 1,
            suid, name.Text, telefon.Text, fax.Text, mobil.Text, email.Text, homepage.Text,
            postfach.Text, notiz.Buffer.Text, sitzStatus1.Active ? 0 : 1, auid, staat.Text,
            plz.Text, ort.Text, strasse.Text, hausnr.Text);
      }
      else if (DialogType == DialogTypeEnum.Delete)
      {
        r = FactoryService.AddressService.DeleteSite(ServiceDaten, Model);
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

    /// <summary>Behandlung von Adressedupl.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnAdresseduplClicked(object sender, EventArgs e)
    {
      adressNr.Text = "";
      adresseAnzahl.Text = "0";
    }

    /// <summary>Behandlung von Adressewechseln.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnAdressewechselnClicked(object sender, EventArgs e)
    {
      var auid = Start(typeof(AD130Addresses), AD130_title, csbpparent: this, modal: true) as string;
      if (auid != null)
      {
        SetAddress(Get(FactoryService.AddressService.GetAddress(ServiceDaten, auid)));
      }
    }

    /// <summary>Behandlung von Abbrechen.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnAbbrechenClicked(object sender, EventArgs e)
    {
      dialog.Hide();
    }

    private void SetAddress(AdAdresse ad)
    {
      var diff = 0;
      if (Functions.CompString(ad?.Uid, adressNr.Text) != 0)
      {
        // Falls sich Nummer ändert, wird neue Nummer einmal mehr benutzt.
        diff = 1;
      }
      strasse.Text = ad?.Strasse ?? "";
      hausnr.Text = ad?.HausNr ?? "";
      staat.Text = ad?.Staat ?? "";
      plz.Text = ad?.Plz ?? "";
      ort.Text = ad?.Ort ?? "";
      adresseAnzahl.Text = Functions.ToString(diff
          + Get(FactoryService.AddressService.GetAddressCount(ServiceDaten, ad?.Uid)));
    }
  }
}
