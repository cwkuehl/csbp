// <copyright file="AD100Persons.cs" company="cwkuehl.de">
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
  using static CSBP.Resources.M;
  using static CSBP.Resources.Messages;

  /// <summary>Controller für AD100Persons Dialog.</summary>
  public partial class AD100Persons : CsbpBin
  {
    /// <summary>Button RefreshAction.</summary>
    [Builder.Object]
    private readonly Button refreshAction;

    /// <summary>Button EditAction.</summary>
    [Builder.Object]
    private readonly Button editAction;

    /// <summary>TreeView personen.</summary>
    [Builder.Object]
    private readonly TreeView personen;

    /// <summary>Entry suche.</summary>
    [Builder.Object]
    private readonly Entry suche;

    /// <summary>Entry name.</summary>
    [Builder.Object]
    private readonly Entry name;

    /// <summary>Entry vorname.</summary>
    [Builder.Object]
    private readonly Entry vorname;

    /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public static AD100Persons Create(object p1 = null, CsbpBin p = null)
    {
      return new AD100Persons(GetBuilder("AD100Persons", out var handle), handle, p1: p1, p: p);
    }

    /// <summary>Konstruktor für modalen Dialog.</summary>
    /// <param name="builder">Betroffener Builder.</param>
    /// <param name="h">Betroffenes Handle vom Builder.</param>
    /// <param name="d">Betroffener einbettender Dialog.</param>
    /// <param name="dt">Betroffener Dialogtyp.</param>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public AD100Persons(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
        : base(b, h, d, dt, p1, p)
    {
      ObservableEventThrottle(refreshAction, delegate { RefreshTreeView(personen, 1); });
      // SetBold(client0);
      InitData(0);
      personen.GrabFocus();
    }

    /// <summary>Model-Daten initialisieren.</summary>
    /// <param name="step">Betroffener Schritt: 0 erstmalig, 1 aktualisieren.</param>
    protected override void InitData(int step)
    {
      if (step <= 0)
      {
        EventsActive = false;
        suche.Text = "%%";
        name.Text = "%%";
        vorname.Text = "%%";
        EventsActive = true;
      }
      if (step <= 1)
      {
        var l = Get(FactoryService.AddressService.GetPersonList(ServiceDaten, false, suche.Text, name.Text,
          vorname.Text)) ?? new List<AdSitz>();
#pragma warning disable 618
        var store = AddStringColumns(personen, AD100_personen_columns);
#pragma warning restore 618
        var pi = new TreeIter();
        string uid = null;
        foreach (var e in l)
        {
          // Nr.;Bezeichnung;Sitz;Geändert am;Geändert von;Angelegt am;Angelegt von
          if (uid == e.Person?.Uid)
          {
            store.AppendValues(pi, e.Uid, "", e.SiteName,
              Functions.ToString(e.ChangedAt, true), e.ChangedBy,
              Functions.ToString(e.CreatedAt, true), e.CreatedBy);
          }
          else
          {
            pi = store.AppendValues(e.Uid, e.PersonName, e.SiteName,
              Functions.ToString(e.ChangedAt, true), e.ChangedBy,
              Functions.ToString(e.CreatedAt, true), e.CreatedBy);
            uid = e.Person?.Uid;
          }
        }
      }
    }

    /// <summary>Aktualisierung des Eltern-Dialogs.</summary>
    protected override void UpdateParent()
    {
      refreshAction.Click();
    }

    /// <summary>Behandlung von Refresh.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnRefreshClicked(object sender, EventArgs e)
    {
      // RefreshTreeView(personen, 1);
    }

    /// <summary>Behandlung von Undo.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnUndoClicked(object sender, EventArgs e)
    {
      if (MainClass.Undo())
        refreshAction.Click();
    }

    /// <summary>Behandlung von Redo.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnRedoClicked(object sender, EventArgs e)
    {
      if (MainClass.Redo())
        refreshAction.Click();
    }

    /// <summary>Behandlung von New.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnNewClicked(object sender, EventArgs e)
    {
      StartDialog(DialogTypeEnum.New);
    }

    /// <summary>Behandlung von Copy.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnCopyClicked(object sender, EventArgs e)
    {
      StartDialog(DialogTypeEnum.Copy);
    }

    /// <summary>Behandlung von Edit.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnEditClicked(object sender, EventArgs e)
    {
      StartDialog(DialogTypeEnum.Edit);
    }

    /// <summary>Behandlung von Delete.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnDeleteClicked(object sender, EventArgs e)
    {
      StartDialog(DialogTypeEnum.Delete);
    }

    /// <summary>Behandlung von Print.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnPrintClicked(object sender, EventArgs e)
    {
      var r = Get(FactoryService.AddressService.GetAddressReport(ServiceDaten));
      UiTools.SaveFile(r, M0(AD012));
    }

    /// <summary>Behandlung von Floppy.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnFloppyClicked(object sender, EventArgs e)
    {
      Start(typeof(AD200Interface), AD200_title, csbpparent: this);
    }

    /// <summary>Behandlung von Personen.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnPersonenRowActivated(object sender, RowActivatedArgs e)
    {
      editAction.Activate();
    }

    /// <summary>Behandlung von Suche.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnSucheKeyReleaseEvent(object sender, KeyReleaseEventArgs e)
    {
      refreshAction.Click();
    }

    /// <summary>Behandlung von Name.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnNameKeyReleaseEvent(object sender, KeyReleaseEventArgs e)
    {
      refreshAction.Click();
    }

    /// <summary>Behandlung von Vorname.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnVornameKeyReleaseEvent(object sender, KeyReleaseEventArgs e)
    {
      refreshAction.Click();
    }

    /// <summary>Behandlung von Alle.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnAlleClicked(object sender, EventArgs e)
    {
      RefreshTreeView(personen, 0);
    }

    /// <summary>Behandlung von Sitzneu.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnSitzneuClicked(object sender, EventArgs e)
    {
      StartDialog(DialogTypeEnum.Copy2);
    }

    /// <summary>Behandlung von Sitzeins.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnSitzeinsClicked(object sender, EventArgs e)
    {
      var uid = GetValue<string>(personen);
      if (Get(FactoryService.AddressService.MakeSiteFirst(ServiceDaten, uid)))
        refreshAction.Click();
    }

    /// <summary>Behandlung von Gebliste.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnGeblisteClicked(object sender, EventArgs e)
    {
      Start(typeof(AD120Birthdays), AD120_title, csbpparent: this);
    }

    /// <summary>Starten des Details-Dialogs.</summary>
    /// <param name="dt">Betroffener Dialog-Typ.</param>
    void StartDialog(DialogTypeEnum dt)
    {
      var uid = GetValue<string>(personen, dt != DialogTypeEnum.New);
      Start(typeof(AD110Person), AD110_title, dt, uid, csbpparent: this);
    }
  }
}
