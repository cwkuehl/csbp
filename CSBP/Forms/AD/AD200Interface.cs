// <copyright file="AD200Interface.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.AD
{
  using System;
  using CSBP.Apis.Enums;
  using CSBP.Base;
  using CSBP.Services.Factory;
  using Gtk;
  using static CSBP.Resources.M;
  using static CSBP.Resources.Messages;

  /// <summary>Controller für AD200Interface Dialog.</summary>
  public partial class AD200Interface : CsbpBin
  {
    /// <summary>Label datei0.</summary>
    [Builder.Object]
    private readonly Label datei0;

    /// <summary>Entry datei.</summary>
    [Builder.Object]
    private readonly Entry datei;

    /// <summary>CheckButton loeschen.</summary>
    [Builder.Object]
    private readonly CheckButton loeschen;

    /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public static AD200Interface Create(object p1 = null, CsbpBin p = null)
    {
      return new AD200Interface(GetBuilder("AD200Interface", out var handle), handle, p1: p1, p: p);
    }

    /// <summary>Konstruktor für modalen Dialog.</summary>
    /// <param name="builder">Betroffener Builder.</param>
    /// <param name="h">Betroffenes Handle vom Builder.</param>
    /// <param name="d">Betroffener einbettender Dialog.</param>
    /// <param name="dt">Betroffener Dialogtyp.</param>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public AD200Interface(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
        : base(b, h, d, dt, p1, p)
    {
      SetBold(datei0);
      InitData(0);
      datei.GrabFocus();
    }

    /// <summary>Model-Daten initialisieren.</summary>
    /// <param name="step">Betroffener Schritt: 0 erstmalig, 1 aktualisieren.</param>
    protected override void InitData(int step)
    {
      if (step <= 0)
      {
        datei.Text = string.IsNullOrEmpty(Parameter.AD200File)
          ? System.IO.Path.Combine(Parameter.TempPath, AD200_select_file)
          : Parameter.AD200File;
      }
    }

    /// <summary>Behandlung von Dateiauswahl.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnDateiauswahlClicked(object sender, EventArgs e)
    {
      var file = SelectFile(string.IsNullOrEmpty(datei.Text) ? AD200_select_file : datei.Text, "*.csv", AD200_select_ext);
      if (!string.IsNullOrEmpty(file))
      {
        datei.Text = file;
        Parameter.AD200File = file;
      }
    }

    /// <summary>Behandlung von Export.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnExportClicked(object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty(datei.Text))
        throw new MessageException(M1012);
      var lines = Get(FactoryService.AddressService.ExportAddressList(ServiceDaten));
      UiTools.SaveFile(lines, datei.Text, open: true);
    }

    /// <summary>Behandlung von Importieren.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnImportierenClicked(object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty(datei.Text))
        throw new MessageException(M1012);
      if (!ShowYesNoQuestion(M0(AD011)))
        return;
      if (loeschen.Active)
      {
        var r = FactoryService.AddressService.ImportAddressList(ServiceDaten, null, true);
        Application.Invoke(delegate
        {
          UpdateParent();
        });
        if (!r.Ok)
        {
          Get(r);
          return;
        }
      }
      var lines = UiTools.ReadFile(datei.Text);
      var message = Get(FactoryService.AddressService.ImportAddressList(ServiceDaten, lines, false));
      Application.Invoke(delegate
      {
        UpdateParent();
      });
      if (!string.IsNullOrEmpty(message))
        ShowInfo(message);
    }

    /// <summary>Behandlung von Abbrechen.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnAbbrechenClicked(object sender, EventArgs e)
    {
      Parameter.AD200File = datei.Text;
      dialog.Hide();
    }
  }
}
