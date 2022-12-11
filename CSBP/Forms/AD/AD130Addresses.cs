// <copyright file="AD130Addresses.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.AD;

using System;
using System.Collections.Generic;
using CSBP.Apis.Enums;
using CSBP.Apis.Models;
using CSBP.Base;
using CSBP.Services.Factory;
using Gtk;
using static CSBP.Resources.Messages;

/// <summary>Controller for AD130Addresses dialog.</summary>
public partial class AD130Addresses : CsbpBin
{
#pragma warning disable CS0649

  /// <summary>Label adressen0.</summary>
  [Builder.Object]
  private readonly Label adressen0;

  /// <summary>TreeView adressen.</summary>
  [Builder.Object]
  private readonly TreeView adressen;

  /// <summary>Button ok.</summary>
  [Builder.Object]
  private readonly Button ok;

#pragma warning restore CS0649

  /// <summary>Initializes a new instance of the <see cref="AD130Addresses"/> class.</summary>
  /// <param name="b">Affected Builder.</param>
  /// <param name="h">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="type">Affected dialog class type.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  public AD130Addresses(Builder b, IntPtr h, Dialog d = null, Type type = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
    : base(b, h, d, type ?? typeof(AD130Addresses), dt, p1, p)
  {
    SetBold(adressen0);
    InitData(0);
    adressen.GrabFocus();
  }

  /// <summary>Creates non modal dialog.</summary>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  /// <returns>Created dialog.</returns>
  public static AD130Addresses Create(object p1 = null, CsbpBin p = null)
  {
    return new AD130Addresses(GetBuilder("AD130Addresses", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  protected override void InitData(int step)
  {
    if (step <= 1)
    {
      var l = Get(FactoryService.AddressService.GetAddressList(ServiceDaten)) ?? new List<AdAdresse>();
      var values = new List<string[]>();
      foreach (var e in l)
      {
        // No.;St.;Postal code;Town;Street;No.;Changed at;Changed by;Created at;Created by
        values.Add(new string[]
          {
            Functions.ToString(e.Uid), Functions.ToString(e.Staat), Functions.ToString(e.Plz),
            Functions.ToString(e.Ort), Functions.ToString(e.Strasse), Functions.ToString(e.HausNr),
            Functions.ToString(e.Geaendert_Am, true), e.Geaendert_Von,
            Functions.ToString(e.Angelegt_Am, true), e.Angelegt_Von,
          });
      }
      AddStringColumnsSort(adressen, AD130_adressen_columns, values);
    }
  }

  /// <summary>Handles Adressen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAdressenRowActivated(object sender, RowActivatedArgs e)
  {
    ok.Activate();
  }

  /// <summary>Handles Ok.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnOkClicked(object sender, EventArgs e)
  {
    var uid = GetValue<string>(adressen);
    Response = uid;
    CloseDialog();
  }

  /// <summary>Handles Abbrechen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAbbrechenClicked(object sender, EventArgs e)
  {
    CloseDialog();
  }
}
