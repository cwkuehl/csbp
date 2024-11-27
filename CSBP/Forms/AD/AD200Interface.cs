// <copyright file="AD200Interface.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.AD;

using System;
using CSBP.Base;
using CSBP.Services.Apis.Enums;
using CSBP.Services.Base;
using CSBP.Services.Factory;
using Gtk;
using static CSBP.Services.Resources.M;
using static CSBP.Services.Resources.Messages;

/// <summary>Controller for AD200Interface dialog.</summary>
public partial class AD200Interface : CsbpBin
{
#pragma warning disable CS0649

  /// <summary>Label datei0.</summary>
  [Builder.Object]
  private readonly Label datei0;

  /// <summary>Entry datei.</summary>
  [Builder.Object]
  private readonly Entry datei;

  /// <summary>CheckButton loeschen.</summary>
  [Builder.Object]
  private readonly CheckButton loeschen;

#pragma warning restore CS0649

  /// <summary>Initializes a new instance of the <see cref="AD200Interface"/> class.</summary>
  /// <param name="b">Affected Builder.</param>
  /// <param name="h">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="type">Affected dialog class type.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  public AD200Interface(Builder b, IntPtr h, Dialog d = null, Type type = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
    : base(b, h, d, type ?? typeof(AD200Interface), dt, p1, p)
  {
    SetBold(datei0);
    InitData(0);
    datei.GrabFocus();
  }

  /// <summary>Creates non modal dialog.</summary>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  /// <returns>Created dialog.</returns>
  public static AD200Interface Create(object p1 = null, CsbpBin p = null)
  {
    return new AD200Interface(GetBuilder("AD200Interface", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  protected override void InitData(int step)
  {
    if (step <= 0)
    {
      SetText(datei, string.IsNullOrEmpty(ParameterGui.AD200File) ? System.IO.Path.Combine(ParameterGui.TempPath, AD200_select_file) : ParameterGui.AD200File);
    }
  }

  /// <summary>Handles Dateiauswahl.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnDateiauswahlClicked(object sender, EventArgs e)
  {
    var file = SelectFile(string.IsNullOrEmpty(datei.Text) ? AD200_select_file : datei.Text, "*.csv", AD200_select_ext);
    if (!string.IsNullOrEmpty(file))
    {
      SetText(datei, file);
      ParameterGui.AD200File = file;
    }
  }

  /// <summary>Handles Export.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnExportClicked(object sender, EventArgs e)
  {
    if (string.IsNullOrEmpty(datei.Text))
      throw new MessageException(M1012);
    var lines = Get(FactoryService.AddressService.ExportAddressList(ServiceDaten));
    UiTools.SaveFile(lines, datei.Text, open: true);
  }

  /// <summary>Handles Importieren.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnImportierenClicked(object sender, EventArgs e)
  {
    if (string.IsNullOrEmpty(datei.Text))
      throw new MessageException(M1012);
    if (!ShowYesNoQuestion(M0(AD011)))
      return;
    if (loeschen.Active)
    {
      var r = FactoryService.AddressService.ImportAddressList(ServiceDaten, null, true);
      Application.Invoke((sender1, e1) => { UpdateParent(); });
      if (!r.Ok)
      {
        Get(r);
        return;
      }
    }
    var lines = UiTools.ReadFile(datei.Text);
    var message = Get(FactoryService.AddressService.ImportAddressList(ServiceDaten, lines, false));
    Application.Invoke((sender1, e1) => { UpdateParent(); });
    if (!string.IsNullOrEmpty(message))
      ShowInfo(message);
  }

  /// <summary>Handles Abbrechen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAbbrechenClicked(object sender, EventArgs e)
  {
    ParameterGui.AD200File = datei.Text;
    CloseDialog();
  }
}
