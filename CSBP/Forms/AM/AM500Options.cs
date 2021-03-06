// <copyright file="AM500Options.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.AM
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using CSBP.Apis.Enums;
  using CSBP.Apis.Models;
  using CSBP.Base;
  using CSBP.Services.Factory;
  using Gtk;
  using static CSBP.Resources.Messages;

  /// <summary>Controller für AM500Options Dialog.</summary>
  public partial class AM500Options : CsbpBin
  {
    /// <summary>Dialog Model.</summary>
    List<MaParameter> Model;
    TreeStore Store;

#pragma warning disable 169, 649

    /// <summary>Label einstellungen0.</summary>
    [Builder.Object]
    private Label einstellungen0;

    /// <summary>TreeView einstellungen.</summary>
    [Builder.Object]
    private TreeView einstellungen;

    /// <summary>Button ok.</summary>
    [Builder.Object]
    private Button ok;

    /// <summary>Button abbrechen.</summary>
    [Builder.Object]
    private Button abbrechen;

#pragma warning restore 169, 649

    /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public static AM500Options Create(object p1 = null, CsbpBin p = null)
    {
      return new AM500Options(GetBuilder("AM500Options", out var handle), handle, p1: p1, p: p);
    }

    /// <summary>Konstruktor für modalen Dialog.</summary>
    /// <param name="builder">Betroffener Builder.</param>
    /// <param name="h">Betroffenes Handle vom Builder.</param>
    /// <param name="d">Betroffener einbettender Dialog.</param>
    /// <param name="dt">Betroffener Dialogtyp.</param>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public AM500Options(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
        : base(b, h, d, dt, p1, p)
    {
      SetBold(einstellungen0);
      InitData(0);
      einstellungen.GrabFocus();
    }

    /// <summary>Model-Daten initialisieren.</summary>
    /// <param name="step">Betroffener Schritt: 0 erstmalig, 1 aktualisieren.</param>
    override protected void InitData(int step)
    {
      var daten = ServiceDaten;
      if (step <= 1)
      {
        Model = Get(FactoryService.ClientService.GetOptionList(daten, daten.MandantNr,
          Parameter.Params)) ?? new List<MaParameter>();
#pragma warning disable 618
        Store = AddStringColumns(einstellungen, AM500_einstellungen_columns);
#pragma warning restore 618
        foreach (var e in Model)
        {
          // Mandant;Schlüssel;Wert;Kommentar;Standard;Geändert am;Geändert von;Angelegt am;Angelegt von
          Store.AppendValues(Functions.ToString(e.Mandant_Nr), e.Schluessel,
            e.Wert ?? "", e.Comment ?? "", e.Default ?? "",
            Functions.ToString(e.Geaendert_Am, true), e.Geaendert_Von,
            Functions.ToString(e.Angelegt_Am, true), e.Angelegt_Von);
        }
      }
    }

    /// <summary>Behandlung von Ok.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnOkClicked(object sender, EventArgs e)
    {
      if (Model == null || Store == null)
        return;
      if (Store.GetIterFirst(out var i))
      {
        do
        {
          var val = Store.GetValue(i, 1) as string;
          var p = Model.FirstOrDefault(a => a.Schluessel == val);
          if (p != null)
          {
            p.Wert = Store.GetValue(i, 2) as string;
          }
        } while (Store.IterNext(ref i));
      }
      var daten = ServiceDaten;
      if (Get(FactoryService.ClientService.SaveOptionList(daten, daten.MandantNr,
          Model, Parameter.Params)))
      {
        Parameter.Save();
        MainClass.MainWindow.RefreshTitle();
        dialog.Hide();
      }
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
