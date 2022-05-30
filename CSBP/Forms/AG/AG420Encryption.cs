// <copyright file="AG420Encryption.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.AG
{
  using System;
  using CSBP.Apis.Enums;
  using CSBP.Apis.Models.Extension;
  using CSBP.Services.Factory;
  using Gtk;
  using static CSBP.Resources.M;
  using static CSBP.Resources.Messages;

  /// <summary>Controller für AG420Encryption Dialog.</summary>
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

    /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public static AG410Backup Create(object p1 = null, CsbpBin p = null)
    {
      return new AG410Backup(GetBuilder("AG420Encryption", out var handle), handle, p1: p1, p: p);
    }

    /// <summary>Konstruktor für modalen Dialog.</summary>
    /// <param name="builder">Betroffener Builder.</param>
    /// <param name="h">Betroffenes Handle vom Builder.</param>
    /// <param name="d">Betroffener einbettender Dialog.</param>
    /// <param name="dt">Betroffener Dialogtyp.</param>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public AG420Encryption(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
        : base(b, h, d, dt, p1, p)
    {
      SetBold(password0);
      InitData(0);
      password.GrabFocus();
    }

    /// <summary>Model-Daten initialisieren.</summary>
    /// <param name="step">Betroffener Schritt: 0 erstmalig, 1 aktualisieren.</param>
    protected override void InitData(int step)
    {
      if (step <= 0)
      {
        if (Parameter1 is string uid)
        {
          var k = Get(FactoryService.ClientService.GetBackupEntry(ServiceDaten, uid));
          target.Text = k?.Target ?? "";
        }
        target.IsEditable = false;
        password.IsEditable = true;
      }
    }

    /// <summary>Behandlung von Ok.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnOkClicked(object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty(password.Text))
      {
        ShowError(M0(AG002));
        return;
      }
      Response = password.Text;
      dialog.Hide();
    }

    /// <summary>Behandlung von Abbrechen.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnAbbrechenClicked(object sender, EventArgs e)
    {
      Response = null;
      dialog.Hide();
    }
  }
}
