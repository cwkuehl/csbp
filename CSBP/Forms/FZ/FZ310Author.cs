// <copyright file="FZ310Author.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.FZ
{
  using System;
  using CSBP.Apis.Enums;
  using CSBP.Apis.Models;
  using CSBP.Apis.Services;
  using CSBP.Services.Factory;
  using Gtk;
  using static CSBP.Resources.Messages;

  /// <summary>Controller für FZ310Author Dialog.</summary>
  public partial class FZ310Author : CsbpBin
  {
    /// <summary>Dialog Model.</summary>
    private FzBuchautor Model;

#pragma warning disable CS0649

    /// <summary>Entry nr.</summary>
    [Builder.Object]
    private readonly Entry nr;

    /// <summary>Label name0.</summary>
    [Builder.Object]
    private readonly Label name0;

    /// <summary>Entry name.</summary>
    [Builder.Object]
    private readonly Entry name;

    /// <summary>Entry vorname.</summary>
    [Builder.Object]
    private readonly Entry vorname;

    /// <summary>TextView notiz.</summary>
    [Builder.Object]
    private readonly TextView notiz;

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

    /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public static FZ310Author Create(object p1 = null, CsbpBin p = null)
    {
      return new FZ310Author(GetBuilder("FZ310Author", out var handle), handle, p1: p1, p: p);
    }

    /// <summary>Konstruktor für modalen Dialog.</summary>
    /// <param name="builder">Betroffener Builder.</param>
    /// <param name="h">Betroffenes Handle vom Builder.</param>
    /// <param name="d">Betroffener einbettender Dialog.</param>
    /// <param name="dt">Betroffener Dialogtyp.</param>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public FZ310Author(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
        : base(b, h, d, dt, p1, p)
    {
      SetBold(name0);
      InitData(0);
      name.GrabFocus();
    }

    /// <summary>Model-Daten initialisieren.</summary>
    /// <param name="step">Betroffener Schritt: 0 erstmalig, 1 aktualisieren.</param>
    protected override void InitData(int step)
    {
      if (step <= 0)
      {
        var neu = DialogType == DialogTypeEnum.New;
        var loeschen = DialogType == DialogTypeEnum.Delete;
        if (!neu && Parameter1 is string uid)
        {
          var k = Get(FactoryService.PrivateService.GetAuthor(ServiceDaten, uid));
          if (k == null)
          {
            Application.Invoke(delegate
            {
              dialog.Hide();
            });
            return;
          }
          Model = k;
          nr.Text = k.Uid;
          name.Text = k.Name;
          vorname.Text = k.Vorname;
          notiz.Buffer.Text = k.Notiz ?? "";
          angelegt.Text = Base.ModelBase.FormatDateOf(k.Angelegt_Am, k.Angelegt_Von);
          geaendert.Text = Base.ModelBase.FormatDateOf(k.Geaendert_Am, k.Geaendert_Von);
        }
        nr.IsEditable = false;
        name.IsEditable = !loeschen;
        vorname.IsEditable = !loeschen;
        notiz.Editable = !loeschen;
        angelegt.IsEditable = false;
        geaendert.IsEditable = false;
        if (loeschen)
          ok.Label = Forms_delete;
      }
    }

    /// <summary>Behandlung von Ok.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnOkClicked(object sender, EventArgs e)
    {
      ServiceErgebnis r = null;
      FzBuchautor author = null;
      if (DialogType == DialogTypeEnum.New || DialogType == DialogTypeEnum.Copy
          || DialogType == DialogTypeEnum.Edit)
      {
        var r1 = FactoryService.PrivateService.SaveAuthor(ServiceDaten,
          DialogType == DialogTypeEnum.Edit ? nr.Text : null, name.Text, vorname.Text, notiz.Buffer.Text);
        author = r1.Ergebnis;
        r = r1;
      }
      else if (DialogType == DialogTypeEnum.Delete)
      {
        r = FactoryService.PrivateService.DeleteAuthor(ServiceDaten, Model);
      }
      if (r != null)
      {
        Get(r);
        if (r.Ok)
        {
          UpdateParent();
          Response = author;
          dialog.Hide();
        }
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
