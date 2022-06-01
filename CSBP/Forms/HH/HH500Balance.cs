// <copyright file="HH500Balance.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.HH
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;
  using CSBP.Apis.Enums;
  using CSBP.Apis.Models;
  using CSBP.Apis.Services;
  using CSBP.Base;
  using CSBP.Forms.Controls;
  using CSBP.Services.Factory;
  using Gtk;
  using static CSBP.Resources.Messages;

  /// <summary>Controller für HH500Balance Dialog.</summary>
  public partial class HH500Balance : CsbpBin
  {
    /// <summary>Dialog Model.</summary>
    private int mlBerechnen = 0;
    private bool sollTabelle;
    private List<HhBilanz> sollListe;
    private List<HhBilanz> habenListe;

    /// <summary>Status für Task.</summary>
    readonly StringBuilder Status = new();

    /// <summary>Abbruch für Task.</summary>
    readonly StringBuilder Cancel = new();

#pragma warning disable CS0649

    /// <summary>Label soll0.</summary>
    [Builder.Object]
    private readonly Label soll0;

    /// <summary>TreeView soll.</summary>
    [Builder.Object]
    private readonly TreeView soll;

    /// <summary>Label haben0.</summary>
    [Builder.Object]
    private readonly Label haben0;

    /// <summary>TreeView haben.</summary>
    [Builder.Object]
    private readonly TreeView haben;

    /// <summary>Label sollBetrag0.</summary>
    [Builder.Object]
    private readonly Label sollBetrag0;

    /// <summary>Label habenBetrag0.</summary>
    [Builder.Object]
    private readonly Label habenBetrag0;

    /// <summary>Label von0.</summary>
    [Builder.Object]
    private readonly Label von0;

    /// <summary>Date Von.</summary>
    //[Builder.Object]
    private readonly Date von;

    /// <summary>Label bis0.</summary>
    [Builder.Object]
    private readonly Label bis0;

    /// <summary>Date Bis.</summary>
    //[Builder.Object]
    private readonly Date bis;

#pragma warning restore CS0649

    /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public static HH500Balance Create(object p1 = null, CsbpBin p = null)
    {
      return new HH500Balance(GetBuilder("HH500Balance", out var handle), handle, p1: p1, p: p);
    }

    /// <summary>Konstruktor für modalen Dialog.</summary>
    /// <param name="builder">Betroffener Builder.</param>
    /// <param name="h">Betroffenes Handle vom Builder.</param>
    /// <param name="d">Betroffener einbettender Dialog.</param>
    /// <param name="dt">Betroffener Dialogtyp.</param>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public HH500Balance(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
        : base(b, h, d, dt, p1, p)
    {
      von = new Date(Builder.GetObject("von").Handle)
      {
        IsNullable = false,
        IsWithCalendar = true,
        IsCalendarOpen = false
      };
      von.DateChanged += OnVonDateChanged;
      von.Show();
      bis = new Date(Builder.GetObject("bis").Handle)
      {
        IsNullable = false,
        IsWithCalendar = true,
        IsCalendarOpen = false
      };
      bis.DateChanged += OnBisDateChanged;
      bis.Show();
      // SetBold(client0);
      InitData(0);
      soll.GrabFocus();
    }

    /// <summary>Model-Daten initialisieren.</summary>
    /// <param name="step">Betroffener Schritt: 0 erstmalig, 1 aktualisieren.</param>
    protected override void InitData(int step)
    {
      var daten = ServiceDaten;
      var parameter = Parameter1 as string;
      if (step <= 0)
      {
        EventsActive = false;
        var max = Get(FactoryService.BudgetService.GetMinMaxPeriod(daten));
        var dB = daten.Heute;
        dB = new DateTime(dB.Year, dB.Month, DateTime.DaysInMonth(dB.Year, dB.Month));
        if (max != null && max.Datum_Bis < dB)
          dB = max.Datum_Bis;
        var dV = new DateTime(dB.Year, 1, 1);
        if (max != null && max.Datum_Von > dV)
          dV = max.Datum_Von;
        if (Constants.KZBI_GV == parameter)
        {
          von.Value = dV;
          bis.Value = dB;
        }
        else if (Constants.KZBI_EROEFFNUNG == parameter)
        {
          von.Value = dV;
          bis.Value = dV;
        }
        else
        {
          von.Value = dB;
          bis.Value = dB;
        }
        if (Constants.KZBI_GV == parameter)
        {
          soll0.Text = HH500_soll_GV;
          haben0.Text = HH500_haben_GV;
        }
        else
        {
          soll0.Text = HH500_soll_EB;
          haben0.Text = HH500_haben_EB;
          von0.Text = HH500_von_EB;
          bis0.Visible = false;
          bis0.NoShowAll = true;
          bis.Visible = false;
          bis.NoShowAll = true;
        }
        soll0.UseUnderline = true;
        haben0.UseUnderline = true;
        von0.UseUnderline = true;
        EventsActive = true;
      }
      if (step <= 1)
      {
        DateTime v = von.ValueNn;
        DateTime b = parameter == Constants.KZBI_GV ? bis.ValueNn : von.ValueNn;
        var l = Get(FactoryService.BudgetService.GetBalanceList(daten, parameter, v, b)) ?? new List<HhBilanz>();
        sollListe = l.Where(a => a.AccountType > 0).ToList();
        habenListe = l.Where(a => a.AccountType <= 0).ToList();
        // Nr.;Bezeichnung;Betrag;Geändert am;Geändert von;Angelegt am;Angelegt von
        var svalues = new List<string[]>();
        var hvalues = new List<string[]>();
        foreach (var e in sollListe)
        {
          svalues.Add(new string[] { e.Konto_Uid, e.AccountName, Functions.ToString(e.AccountEsum, 2),
            Functions.ToString(e.Geaendert_Am, true), e.Geaendert_Von,
            Functions.ToString(e.Angelegt_Am, true), e.Angelegt_Von });
        }
        foreach (var e in habenListe)
        {
          hvalues.Add(new string[] { e.Konto_Uid, e.AccountName, Functions.ToString(e.AccountEsum, 2),
            Functions.ToString(e.Geaendert_Am, true), e.Geaendert_Von,
            Functions.ToString(e.Angelegt_Am, true), e.Angelegt_Von });
        }
        sollBetrag0.Text = Functions.ToString(sollListe.Sum(a => a.AccountEsum), 2);
        habenBetrag0.Text = Functions.ToString(habenListe.Sum(a => a.AccountEsum), 2);
        AddStringColumnsSort(soll, HH500_soll_columns, svalues);
        AddStringColumnsSort(haben, HH500_haben_columns, hvalues);
      }
    }

    /// <summary>Behandlung von Refresh.</summary>
    private void OnRefresh()
    {
      if (!EventsActive)
        return;
      try
      {
        EventsActive = false;
        var s = haben.Selection.GetSelectedRows();
        RefreshTreeView(soll, 1);
        // RefreshTreeView(haben, 1);
        foreach (var p in s)
          haben.Selection.SelectPath(p);
      }
      finally
      {
        EventsActive = true;
      }
    }

    /// <summary>Behandlung von Refresh.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnRefreshClicked(object sender, EventArgs e)
    {
      // Berechnen der Bilanz.
      ShowStatus(Status, Cancel);
      Task.Run(() =>
      {
        var keine = true;
        var weiter = 2;
        DateTime? v = null;
        ServiceErgebnis<string[]> r = null;
        try
        {
          if (mlBerechnen > 0)
          {
            v = von.ValueNn;
            mlBerechnen = 0;
          }
          while (weiter >= 2 && (r == null || r.Ok))
          {
            r = FactoryService.BudgetService.CalculateBalances(ServiceDaten, Status, Cancel, true, v);
            v = null;
            if (r.Ok)
            {
              var l = r.Ergebnis;
              if (l != null && l.Length >= 1)
              {
                weiter = Functions.ToInt32(l[0]);
                if (weiter > 0)
                  keine = false;
              }
            }
            else
              Application.Invoke(delegate
              {
                Get(r);
              });
          }
        }
        catch (Exception ex)
        {
          Functions.MachNichts(ex);
        }
        finally
        {
          if (keine)
            mlBerechnen++;
          else
            Application.Invoke(delegate
            {
              OnRefresh();
            });
        }
        return 0;
      });
    }

    /// <summary>Behandlung von Undo.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnUndoClicked(object sender, EventArgs e)
    {
      if (MainClass.Undo())
        OnRefresh();
    }

    /// <summary>Behandlung von Redo.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnRedoClicked(object sender, EventArgs e)
    {
      if (MainClass.Redo())
        OnRefresh();
    }

    /// <summary>Behandlung von Print.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnPrintClicked(object sender, EventArgs e)
    {
      var d = new Tuple<string, DateTime, DateTime>(Parameter1?.ToString(), von.ValueNn, bis.ValueNn);
      Start(typeof(HH510Interface), HH510_title, DialogTypeEnum.Without, d, csbpparent: this);
    }

    /// <summary>Behandlung von Soll.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnSollCursorChanged(object sender, EventArgs e)
    {
      if (EventsActive)
        sollTabelle = true;
    }

    /// <summary>Behandlung von Soll.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnSollRowActivated(object sender, RowActivatedArgs e)
    {
      var uid = GetValue<string>(soll, true);
      StartBookings(uid);
    }

    /// <summary>Behandlung von Haben.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnHabenCursorChanged(object sender, EventArgs e)
    {
      if (EventsActive)
        sollTabelle = false;
    }

    /// <summary>Behandlung von Haben.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnHabenRowActivated(object sender, RowActivatedArgs e)
    {
      var uid = GetValue<string>(haben, true);
      StartBookings(uid);
    }

    /// <summary>Behandlung von von.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnVonDateChanged(object sender, DateChangedEventArgs e)
    {
      OnRefresh();
    }

    /// <summary>Behandlung von bis.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnBisDateChanged(object sender, DateChangedEventArgs e)
    {
      if (bis.ValueNn < von.ValueNn)
        bis.Value = von.ValueNn;
      OnRefresh();
    }

    /// <summary>Behandlung von Oben.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnObenClicked(object sender, EventArgs e)
    {
      SwapSortings(true);
    }

    /// <summary>Behandlung von Unten.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnUntenClicked(object sender, EventArgs e)
    {
      SwapSortings(false);
    }

    /// <summary>Starten des Buchungen-Dialogs.</summary>
    /// <param name="uid">Betroffenes Konto.</param>
    void StartBookings(string uid)
    {
      var a = Get(FactoryService.BudgetService.GetAccount(ServiceDaten, uid));
      if (a != null)
      {
        var v = von.ValueNn;
        var b = bis.ValueNn;
        if (Parameter1?.ToString() == Constants.KZBI_EROEFFNUNG)
          b = v.AddYears(1).AddDays(-1);
        else if (Parameter1?.ToString() == Constants.KZBI_SCHLUSS)
        {
          b = v;
          v = b.AddDays(1 - b.DayOfYear);
        }
        var p1 = new Tuple<HhKonto, DateTime, DateTime>(a, v, b);
        MainClass.MainWindow.AppendPage(HH400Bookings.Create(p1), a.Name);
      }
    }

    /// <summary>Tauschen der Konto-Sortierungen.</summary>
    /// <param name="up">Nach oben?</param>
    void SwapSortings(bool up)
    {
      var uid = sollTabelle ? GetValue<string>(soll, false) : GetValue<string>(haben, false);
      if (string.IsNullOrEmpty(uid))
        return;
      var l = sollTabelle ? sollListe : habenListe;
      var acc = l.FirstOrDefault(a => a.Konto_Uid == uid);
      if (acc == null)
        return;
      var i = l.IndexOf(acc);
      var d = up ? -1 : 1;
      if (0 <= i && i < l.Count && 0 <= i + d && i + d < l.Count)
      {
        var uid2 = l[i + d].Konto_Uid;
        var r = FactoryService.BudgetService.SwapAccountSort(ServiceDaten, uid, uid2);
        Get(r);
        if (r.Ok)
        {
          OnRefresh();
          if (sollTabelle)
            SetText(soll, uid);
          else
            SetText(haben, uid);
        }
      }
    }
  }
}
