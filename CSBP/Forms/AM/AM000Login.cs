// <copyright file="AM000Login.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.AM;

using System;
using CSBP.Apis.Enums;
using CSBP.Apis.Services;
using CSBP.Base;
using CSBP.Services.Factory;
using Gtk;

/// <summary>Controller for AM000Login dialog.</summary>
public class AM000Login : CsbpBin
{
#pragma warning disable CS0649

  /// <summary>Label client0.</summary>
  [Builder.Object]
  private readonly Label client0;

  /// <summary>Entry client.</summary>
  [Builder.Object]
  private readonly Entry client;

  /// <summary>Label user0.</summary>
  [Builder.Object]
  private readonly Label user0;

  /// <summary>Entry user.</summary>
  [Builder.Object]
  private readonly Entry user;

  /// <summary>Label password0.</summary>
  [Builder.Object]
  private readonly Label password0;

  /// <summary>Entry password.</summary>
  [Builder.Object]
  private readonly Entry password;

  /// <summary>CheckButton save.</summary>
  [Builder.Object]
  private readonly CheckButton save;

#pragma warning restore CS0649

  /// <summary>Initializes a new instance of the <see cref="AM000Login"/> class.</summary>
  /// <param name="b">Affected Builder.</param>
  /// <param name="h">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  public AM000Login(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
    : base(b, h, d, dt, p1, p)
  {
    SetBold(client0);
    SetBold(user0);
    SetBold(password0);
    InitData(0);
  }

  /// <summary>Creates non modal dialog.</summary>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  /// <returns>Created dialog.</returns>
  public static AM000Login Create(object p1 = null, CsbpBin p = null)
  {
    return new AM000Login(GetBuilder("AM000Login", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  protected override void InitData(int step)
  {
    if (step == 0)
    {
      SetText(client, Parameter.LoginClient);
      SetText(user, Parameter.LoginUser);
      if (string.IsNullOrWhiteSpace(user.Text))
        SetText(user, Environment.UserName);
      if (string.IsNullOrWhiteSpace(client.Text))
        client.GrabFocus();
      else if (string.IsNullOrWhiteSpace(user.Text))
        user.GrabFocus();
      else
        password.GrabFocus();
    }
  }

  /// <summary>Handles login.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnLogin(object sender, EventArgs e)
  {
    var id = user.Text;
    var daten = new ServiceDaten(Functions.ToInt32(client.Text), id);
    var r = FactoryService.LoginService.Login(daten, password.Text, save.Active);
    Get(r);
    if (r.Ok)
    {
      id = r.Ergebnis;
      daten = new ServiceDaten(daten.MandantNr, id);
      dialog.Hide();
      Response = ResponseType.Ok;
      MainClass.Login(daten);
    }
    Parameter.LoginClient = client.Text;
    Parameter.LoginUser = id;
    Parameter.Save();
  }

  /// <summary>Handles reset.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnReset(object sender, EventArgs e)
  {
    MainClass.MainWindow.Reset();
  }

  /// <summary>Handles cancel.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnCancel(object sender, EventArgs e)
  {
    dialog.Hide();
  }
}
