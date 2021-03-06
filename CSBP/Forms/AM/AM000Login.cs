// <copyright file="AM000Login.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.AM
{
  using System;
  using CSBP.Apis.Enums;
  using CSBP.Apis.Services;
  using CSBP.Base;
  using CSBP.Services.Factory;
  using Gtk;

  /// <summary>Controller für AM000Login Dialog.</summary>
  public class AM000Login : CsbpBin
  {
#pragma warning disable 169, 649

    /// <summary>Mandant Label.</summary>
    [Builder.Object]
    private Label client0;

    /// <summary>Benutzer Label.</summary>
    [Builder.Object]
    private Label user0;

    /// <summary>Kennwort Label.</summary>
    [Builder.Object]
    private Label password0;

    /// <summary>Mandant Entry.</summary>
    [Builder.Object]
    private Entry client;

    /// <summary>Benutzer Entry.</summary>
    [Builder.Object]
    private Entry user;

    /// <summary>Kennwort Entry.</summary>
    [Builder.Object]
    private Entry password;

    /// <summary>Speichern CheckButton.</summary>
    [Builder.Object]
    private CheckButton save;

#pragma warning restore 169, 649

    /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public static AM000Login Create(object p1 = null, CsbpBin p = null)
    {
      return new AM000Login(GetBuilder("AM000Login", out var handle), handle, p1: p1, p: p);
    }

    /// <summary>Konstruktor für modalen Dialog.</summary>
    /// <param name="builder">Betroffener Builder.</param>
    /// <param name="h">Betroffenes Handle vom Builder.</param>
    /// <param name="d">Betroffener einbettender Dialog.</param>
    /// <param name="dt">Betroffener Dialogtyp.</param>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public AM000Login(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
        : base(b, h, d, dt, p1, p)
    {
      SetBold(client0);
      SetBold(user0);
      SetBold(password0);
      InitData(0);
    }

    /// <summary>Model-Daten initialisieren.</summary>
    /// <param name="step">Betroffener Schritt: 0 erstmalig, 1 aktualisieren.</param>
    override protected void InitData(int step)
    {
      if (step == 0)
      {
        client.Text = Parameter.LoginClient;
        user.Text = Parameter.LoginUser;
        if (string.IsNullOrWhiteSpace(user.Text))
          user.Text = Environment.UserName;
        if (string.IsNullOrWhiteSpace(client.Text))
          client.GrabFocus();
        else if (string.IsNullOrWhiteSpace(user.Text))
          user.GrabFocus();
        else
          password.GrabFocus();
      }
    }

    /// <summary>Behandlung von Anmelden.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnLogin(object sender, EventArgs a)
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

    /// <summary>Behandlung von Zurücksetzen.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnReset(object sender, EventArgs a)
    {
      MainClass.MainWindow.Reset();
    }

    /// <summary>Behandlung von Abbrechen.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnCancel(object sender, EventArgs a)
    {
      dialog.Hide();
    }
  }
}