// <copyright file="AG420Encryption.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.AG;

using System;
using CSBP.Services.Apis.Enums;
using CSBP.Services.Factory;
using Gtk;
using static CSBP.Services.Resources.M;
using static CSBP.Services.Resources.Messages;

/// <summary>Controller for AG420Encryption dialog.</summary>
public partial class AG420Encryption : CsbpBin
{
#pragma warning disable CS0649

  /// <summary>Entry target.</summary>
  [Builder.Object]
  private readonly Entry target;

  /// <summary>Label password0.</summary>
  [Builder.Object]
  private readonly Label password0;

  /// <summary>Entry password.</summary>
  [Builder.Object]
  private readonly Entry password;

#pragma warning restore CS0649

  /// <summary>Initializes a new instance of the <see cref="AG420Encryption"/> class.</summary>
  /// <param name="b">Affected Builder.</param>
  /// <param name="h">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="type">Affected dialog class type.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  public AG420Encryption(Builder b, IntPtr h, Dialog d = null, Type type = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, type ?? typeof(AG420Encryption), dt, p1, p)
  {
    SetBold(password0);
    InitData(0);
    password.GrabFocus();
  }

  /// <summary>Creates non modal dialog.</summary>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  /// <returns>Created dialog.</returns>
  public static AG410Backup Create(object p1 = null, CsbpBin p = null)
  {
    return new AG410Backup(GetBuilder("AG420Encryption", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  protected override void InitData(int step)
  {
    if (step <= 0)
    {
      if (Parameter1 is string uid)
      {
        var k = Get(FactoryService.ClientService.GetBackupEntry(ServiceDaten, uid));
        SetText(target, k?.Target);
      }
      target.IsEditable = false;
      password.IsEditable = true;
    }
  }

  /// <summary>Handles Ok.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnOkClicked(object sender, EventArgs e)
  {
    if (string.IsNullOrEmpty(password.Text))
    {
      ShowError(M0(AG002));
      return;
    }
    Response = password.Text;
    CloseDialog();
  }

  /// <summary>Handles Abbrechen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAbbrechenClicked(object sender, EventArgs e)
  {
    Response = null;
    CloseDialog();
  }
}
