// <copyright file="SB500Gedcom.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.SB
{
  using System;
  using CSBP.Apis.Enums;
  using CSBP.Apis.Models;
  using CSBP.Apis.Services;
  using CSBP.Base;
  using CSBP.Forms.Controls;
  using CSBP.Services.Factory;
  using Gtk;
  using static CSBP.Resources.M;
  using static CSBP.Resources.Messages;

  /// <summary>Controller für SB500Gedcom Dialog.</summary>
  public partial class SB500Gedcom : CsbpBin
  {
#pragma warning disable 169, 649

    /// <summary>Label name0.</summary>
    [Builder.Object]
    private Label name0;

    /// <summary>Entry name.</summary>
    [Builder.Object]
    private Entry name;

    /// <summary>Label datei0.</summary>
    [Builder.Object]
    private Label datei0;

    /// <summary>Entry datei.</summary>
    [Builder.Object]
    private Entry datei;

    /// <summary>Button dateiAuswahl.</summary>
    [Builder.Object]
    private Button dateiAuswahl;

    /// <summary>Label filter0.</summary>
    [Builder.Object]
    private Label filter0;

    /// <summary>TextView filter.</summary>
    [Builder.Object]
    private TextView filter;

    /// <summary>Button export.</summary>
    [Builder.Object]
    private Button export;

    /// <summary>Button importieren.</summary>
    [Builder.Object]
    private Button importieren;

    /// <summary>Button abbrechen.</summary>
    [Builder.Object]
    private Button abbrechen;

#pragma warning restore 169, 649

    /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public static SB500Gedcom Create(object p1 = null, CsbpBin p = null)
    {
      return new SB500Gedcom(GetBuilder("SB500Gedcom", out var handle), handle, p1: p1, p: p);
    }

    /// <summary>Konstruktor für modalen Dialog.</summary>
    /// <param name="builder">Betroffener Builder.</param>
    /// <param name="h">Betroffenes Handle vom Builder.</param>
    /// <param name="d">Betroffener einbettender Dialog.</param>
    /// <param name="dt">Betroffener Dialogtyp.</param>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public SB500Gedcom(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
        : base(b, h, d, dt, p1, p)
    {
      SetBold(name0);
      InitData(0);
      name.GrabFocus();
    }

    /// <summary>Model-Daten initialisieren.</summary>
    /// <param name="step">Betroffener Schritt: 0 erstmalig, 1 aktualisieren.</param>
    override protected void InitData(int step)
    {
      if (step <= 0)
      {
        EventsActive = false;
        name.Text = Parameter.SB500Name ?? "";
        datei.Text = string.IsNullOrEmpty(Parameter.SB500File)
          ? System.IO.Path.Combine(Parameter.TempPath, SB500_select_file)
          : Parameter.SB500File;
        filter.Buffer.Text = Parameter.SB500Filter ?? "";
        EventsActive = true;
      }
    }

    /// <summary>Behandlung von Name.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnNameKeyReleaseEvent(object o, KeyReleaseEventArgs e)
    {
      if (!EventsActive)
        return;
      Parameter.SB500Name = name.Text;
    }

    /// <summary>Behandlung von Filter.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnFilterKeyReleaseEvent(object o, KeyReleaseEventArgs e)
    {
      if (!EventsActive)
        return;
      Parameter.SB500Filter = filter.Buffer.Text;
    }

    /// <summary>Behandlung von Dateiauswahl.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnDateiauswahlClicked(object sender, EventArgs e)
    {
      var file = SelectFile(string.IsNullOrEmpty(datei.Text) ? SB500_select_file : datei.Text, "*.csv", SB500_select_ext);
      if (!string.IsNullOrEmpty(file))
      {
        datei.Text = file;
        Parameter.SB500File = file;
      }
    }

    /// <summary>Behandlung von Export.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnExportClicked(object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty(datei.Text))
        throw new MessageException(M1012);
      var lines = Get(FactoryService.PedigreeService.ExportAncestorList(ServiceDaten, datei.Text, name.Text, filter.Buffer.Text));
      UiTools.SaveFile(lines, datei.Text);
    }

    /// <summary>Behandlung von Importieren.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnImportierenClicked(object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty(datei.Text))
        throw new MessageException(M1012);
      if (!ShowYesNoQuestion(M0(SB029)))
        return;
      var lines = UiTools.ReadFile(datei.Text);
      var message = Get(FactoryService.PedigreeService.ImportAncestorList(ServiceDaten, lines));
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
      dialog.Hide();
    }
  }
}
