// <copyright file="AD130Addresses.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.AD
{
  using System;
  using System.Collections.Generic;
  using CSBP.Apis.Enums;
  using CSBP.Apis.Models;
  using CSBP.Base;
  using CSBP.Services.Factory;
  using Gtk;
  using static CSBP.Resources.Messages;

  /// <summary>Controller für AD130Addresses Dialog.</summary>
  public partial class AD130Addresses : CsbpBin
  {
    /// <summary>Label adressen0.</summary>
    [Builder.Object]
    private readonly Label adressen0;

    /// <summary>TreeView adressen.</summary>
    [Builder.Object]
    private readonly TreeView adressen;

    /// <summary>Button ok.</summary>
    [Builder.Object]
    private readonly Button ok;

    /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public static AD130Addresses Create(object p1 = null, CsbpBin p = null)
    {
      return new AD130Addresses(GetBuilder("AD130Addresses", out var handle), handle, p1: p1, p: p);
    }

    /// <summary>Konstruktor für modalen Dialog.</summary>
    /// <param name="builder">Betroffener Builder.</param>
    /// <param name="h">Betroffenes Handle vom Builder.</param>
    /// <param name="d">Betroffener einbettender Dialog.</param>
    /// <param name="dt">Betroffener Dialogtyp.</param>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public AD130Addresses(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
        : base(b, h, d, dt, p1, p)
    {
      SetBold(adressen0);
      InitData(0);
      adressen.GrabFocus();
    }

    /// <summary>Model-Daten initialisieren.</summary>
    /// <param name="step">Betroffener Schritt: 0 erstmalig, 1 aktualisieren.</param>
    protected override void InitData(int step)
    {
      if (step <= 1)
      {
        var l = Get(FactoryService.AddressService.GetAddressList(ServiceDaten)) ?? new List<AdAdresse>();
        var values = new List<string[]>();
        foreach (var e in l)
        {
          // Nr.;St.;PLZ;Ort;Straße;Haus-Nr.;Geändert am;Geändert von;Angelegt am;Angelegt von
          values.Add(new string[] { e.Uid, e.Staat, e.Plz, e.Ort, e.Strasse, e.HausNr,
            Functions.ToString(e.Geaendert_Am, true), e.Geaendert_Von,
            Functions.ToString(e.Angelegt_Am, true), e.Angelegt_Von });
        }
        AddStringColumnsSort(adressen, AD130_adressen_columns, values);
      }
    }

    /// <summary>Behandlung von Adressen.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnAdressenRowActivated(object sender, RowActivatedArgs e)
    {
      ok.Activate();
    }

    /// <summary>Behandlung von Ok.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnOkClicked(object sender, EventArgs e)
    {
      var uid = GetValue<string>(adressen);
      Response = uid;
      dialog.Hide();
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
