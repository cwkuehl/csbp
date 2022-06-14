// <copyright file="SB500Gedcom.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.SB;

using System;
using CSBP.Apis.Enums;
using CSBP.Base;
using CSBP.Services.Factory;
using Gtk;
using static CSBP.Resources.M;
using static CSBP.Resources.Messages;

/// <summary>Controller for SB500Gedcom dialog.</summary>
public partial class SB500Gedcom : CsbpBin
{
#pragma warning disable CS0649

  /// <summary>Label name0.</summary>
  [Builder.Object]
  private readonly Label name0;

  /// <summary>Entry name.</summary>
  [Builder.Object]
  private readonly Entry name;

  /// <summary>Entry datei.</summary>
  [Builder.Object]
  private readonly Entry datei;

  /// <summary>TextView filter.</summary>
  [Builder.Object]
  private readonly TextView filter;

#pragma warning restore CS0649

  /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
  /// <param name="p1">1. Parameter für Dialog.</param>
  /// <param name="p">Betroffener Eltern-Dialog.</param>
  /// <returns>Nicht-modalen Dialogs.</returns>
  public static SB500Gedcom Create(object p1 = null, CsbpBin p = null)
  {
    return new SB500Gedcom(GetBuilder("SB500Gedcom", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Konstruktor für modalen Dialog.</summary>
  /// <param name="b">Betroffener Builder.</param>
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

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  protected override void InitData(int step)
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

  /// <summary>Handles Name.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnNameKeyReleaseEvent(object sender, KeyReleaseEventArgs e)
  {
    if (!EventsActive)
      return;
    Parameter.SB500Name = name.Text;
  }

  /// <summary>Handles Filter.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnFilterKeyReleaseEvent(object sender, KeyReleaseEventArgs e)
  {
    if (!EventsActive)
      return;
    Parameter.SB500Filter = filter.Buffer.Text;
  }

  /// <summary>Handles Dateiauswahl.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnDateiauswahlClicked(object sender, EventArgs e)
  {
    var file = SelectFile(string.IsNullOrEmpty(datei.Text) ? SB500_select_file : datei.Text, "*.csv", SB500_select_ext);
    if (!string.IsNullOrEmpty(file))
    {
      datei.Text = file;
      Parameter.SB500File = file;
    }
  }

  /// <summary>Handles Export.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnExportClicked(object sender, EventArgs e)
  {
    if (string.IsNullOrEmpty(datei.Text))
      throw new MessageException(M1012);
    var lines = Get(FactoryService.PedigreeService.ExportAncestorList(ServiceDaten, datei.Text, name.Text, filter.Buffer.Text));
    UiTools.SaveFile(lines, datei.Text);
  }

  /// <summary>Handles Importieren.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
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

  /// <summary>Handles Abbrechen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAbbrechenClicked(object sender, EventArgs e)
  {
    dialog.Hide();
  }
}
