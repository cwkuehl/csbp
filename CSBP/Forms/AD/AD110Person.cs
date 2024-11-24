// <copyright file="AD110Person.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.AD;

using System;
using CSBP.Forms.Controls;
using CSBP.Services.Apis.Enums;
using CSBP.Services.Apis.Models;
using CSBP.Services.Base;
using CSBP.Services.Factory;
using Gtk;
using static CSBP.Services.Resources.Messages;

/// <summary>Controller for AD110Person dialog.</summary>
public partial class AD110Person : CsbpBin
{
#pragma warning disable CS0649

  /// <summary>Entry nr.</summary>
  [Builder.Object]
  private readonly Entry nr;

  /// <summary>Entry sitzNr.</summary>
  [Builder.Object]
  private readonly Entry sitzNr;

  /// <summary>Entry adressNr.</summary>
  [Builder.Object]
  private readonly Entry adressNr;

  /// <summary>Entry titel.</summary>
  [Builder.Object]
  private readonly Entry titel;

  /// <summary>Entry vorname.</summary>
  [Builder.Object]
  private readonly Entry vorname;

  /// <summary>Entry praedikat.</summary>
  [Builder.Object]
  private readonly Entry praedikat;

  /// <summary>Label name10.</summary>
  [Builder.Object]
  private readonly Label name10;

  /// <summary>Entry name1.</summary>
  [Builder.Object]
  private readonly Entry name1;

  /// <summary>Entry name2.</summary>
  [Builder.Object]
  private readonly Entry name2;

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

  /// <summary>Date Geburt.</summary>
  private readonly Date geburt;

  /// <summary>Label personStatus0.</summary>
  [Builder.Object]
  private readonly Label personStatus0;

  /// <summary>RadioButton personStatus1.</summary>
  [Builder.Object]
  private readonly RadioButton personStatus1;

  /// <summary>RadioButton personStatus2.</summary>
  [Builder.Object]
  private readonly RadioButton personStatus2;

  /// <summary>Entry name.</summary>
  [Builder.Object]
  private readonly Entry name;

  /// <summary>Entry strasse.</summary>
  [Builder.Object]
  private readonly Entry strasse;

  /// <summary>Entry hausnr.</summary>
  [Builder.Object]
  private readonly Entry hausnr;

  /// <summary>Entry postfach.</summary>
  [Builder.Object]
  private readonly Entry postfach;

  /// <summary>Entry staat.</summary>
  [Builder.Object]
  private readonly Entry staat;

  /// <summary>Entry plz.</summary>
  [Builder.Object]
  private readonly Entry plz;

  /// <summary>Entry ort.</summary>
  [Builder.Object]
  private readonly Entry ort;

  /// <summary>Entry telefon.</summary>
  [Builder.Object]
  private readonly Entry telefon;

  /// <summary>Entry fax.</summary>
  [Builder.Object]
  private readonly Entry fax;

  /// <summary>Entry mobil.</summary>
  [Builder.Object]
  private readonly Entry mobil;

  /// <summary>Entry homepage.</summary>
  [Builder.Object]
  private readonly Entry homepage;

  /// <summary>Entry email.</summary>
  [Builder.Object]
  private readonly Entry email;

  /// <summary>TextView notiz.</summary>
  [Builder.Object]
  private readonly TextView notiz;

  /// <summary>RadioButton sitzStatus1.</summary>
  [Builder.Object]
  private readonly RadioButton sitzStatus1;

  /// <summary>RadioButton sitzStatus2.</summary>
  [Builder.Object]
  private readonly RadioButton sitzStatus2;

  /// <summary>Entry adresseAnzahl.</summary>
  [Builder.Object]
  private readonly Entry adresseAnzahl;

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
  private AdSitz model;

  /// <summary>Initializes a new instance of the <see cref="AD110Person"/> class.</summary>
  /// <param name="b">Affected Builder.</param>
  /// <param name="h">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="type">Affected dialog class type.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  public AD110Person(Builder b, IntPtr h, Dialog d = null, Type type = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, type ?? typeof(AD110Person), dt, p1, p)
  {
    geburt = new Date(Builder.GetObject("geburt").Handle)
    {
      IsNullable = true,
      IsWithCalendar = true,
      IsCalendarOpen = false,
    };
    geburt.DateChanged += OnGeburtDateChanged;
    geburt.Show();
    SetBold(name10);
    SetBold(geschlecht0);
    SetBold(personStatus0);
    InitData(0);
    titel.GrabFocus();
  }

  /// <summary>Creates non modal dialog.</summary>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  /// <returns>Created dialog.</returns>
  public static AD110Person Create(object p1 = null, CsbpBin p = null)
  {
    return new AD110Person(GetBuilder("AD110Person", out var handle), handle, p1: p1, p: p);
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
        var k = Get(FactoryService.AddressService.GetSite(ServiceDaten, uid));
        if (k == null)
        {
          Application.Invoke((sender, e) =>
          {
            CloseDialog();
          });
          return;
        }
        model = k;
        SetText(nr, k.Person?.Uid);
        SetText(sitzNr, k.Uid);
        SetText(adressNr, k.Address?.Uid);
        SetText(titel, k.Person?.Titel);
        SetText(vorname, k.Person?.Vorname);
        SetText(praedikat, k.Person?.Praedikat);
        SetText(name1, k.Person?.Name1);
        SetText(name2, k.Person?.Name2);
        geschlecht1.Active = k.Person?.Geschlecht != "M" && k.Person?.Geschlecht != "F";
        geschlecht2.Active = k.Person?.Geschlecht == "M";
        geschlecht3.Active = k.Person?.Geschlecht == "F";
        geburt.Value = k.Person?.Geburt;
        personStatus1.Active = k.Person?.Person_Status == 0;
        personStatus2.Active = k.Person?.Person_Status != 0;
        SetText(name, k.Name);
        SetText(postfach, k.Postfach);
        SetText(telefon, k.Telefon);
        SetText(fax, k.Fax);
        SetText(mobil, k.Mobil);
        SetText(homepage, k.Homepage);
        SetText(email, k.Email);
        SetText(notiz, k.Bemerkung);
        sitzStatus1.Active = k.Sitz_Status == 0;
        sitzStatus2.Active = k.Sitz_Status != 0;
        SetAddress(k.Address);
        SetText(angelegt, ModelBase.FormatDateOf(k.CreatedAt, k.CreatedBy));
        SetText(geaendert, ModelBase.FormatDateOf(k.ChangedAt, k.CreatedBy));
      }
      if (DialogType == DialogTypeEnum.Copy2)
      {
        SetText(sitzNr, "");
        SetText(adressNr, "");
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

  /// <summary>Handles geburt.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnGeburtDateChanged(object sender, DateChangedEventArgs e)
  {
  }

  /// <summary>Handles Ok.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
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
      r = FactoryService.AddressService.DeleteSite(ServiceDaten, model);
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

  /// <summary>Handles Adressedupl.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAdresseduplClicked(object sender, EventArgs e)
  {
    SetText(adressNr, "");
    SetText(adresseAnzahl, "0");
  }

  /// <summary>Handles Adressewechseln.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAdressewechselnClicked(object sender, EventArgs e)
  {
    if (Start(typeof(AD130Addresses), AD130_title, csbpparent: this, modal: true) is string auid)
    {
      SetAddress(Get(FactoryService.AddressService.GetAddress(ServiceDaten, auid)));
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
  /// Sets a new address.
  /// </summary>
  /// <param name="ad">New address.</param>
  private void SetAddress(AdAdresse ad)
  {
    var diff = 0;
    if (Functions.CompString(ad?.Uid, adressNr.Text) != 0)
    {
      // If the number is changing, the new number will be used once more.
      diff = 1;
    }
    SetText(adressNr, ad?.Uid);
    SetText(strasse, ad?.Strasse);
    SetText(hausnr, ad?.HausNr);
    SetText(staat, ad?.Staat);
    SetText(plz, ad?.Plz);
    SetText(ort, ad?.Ort);
    SetText(adresseAnzahl, Functions.ToString(diff + Get(FactoryService.AddressService.GetAddressCount(ServiceDaten, ad?.Uid))));
  }
}
