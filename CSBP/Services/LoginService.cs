// <copyright file="LoginService.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services
{
  using System;
  using System.Linq;
  using CSBP.Apis.Models;
  using CSBP.Apis.Services;
  using CSBP.Base;
  using CSBP.Services.Base;
  using static CSBP.Resources.M;
  using static CSBP.Resources.Messages;

  public class LoginService : ServiceBase, ILoginService
  {
    /// <summary>
    /// Is login wihtout password?
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <returns>Is login wihtout password?</returns>
    public ServiceErgebnis<bool> IsWithoutPassword(ServiceDaten daten)
    {
      var r = new ServiceErgebnis<bool>(false);
      var wert = getOhneAnmelden(daten);
      if (!string.IsNullOrEmpty(wert) && !string.IsNullOrEmpty(daten.BenutzerId))
      {
        r.Ergebnis = string.Compare(wert, daten.BenutzerId, true) == 0;
        // if (r.ergebnis) {
        // 	initMandant(daten)
        // }
      }
      return r;
    }

    /// <summary>
    /// Do login with all checks.
    /// </summary>
    /// <param name="daten">Affected client number and user id.</param>
    /// <param name="kennwort">Affected password.</param>
    /// <param name="speichern">Save password?</param>
    /// <returns>Possibly errors.</returns>
    public ServiceErgebnis<string> Login(ServiceDaten daten, string kennwort, bool speichern)
    {
      if (daten.MandantNr < 0)
      {
        // Die Anmeldedaten sind ungültig. Mandant ungültig.
        throw new MessageException(AM001);
      }
      if (string.IsNullOrWhiteSpace(daten.BenutzerId))
      {
        // Die Anmeldedaten sind ungültig. Benutzer ungültig.
        throw new MessageException(AM001);
      }

      var r = new ServiceErgebnis<string>();
      var benutzer = BenutzerRep.Get(daten, daten.MandantNr, daten.BenutzerId);
      if (benutzer != null)
      {
        // Benutzer vorhanden.
        var benutzerId = benutzer.Benutzer_ID;
        r.Ergebnis = benutzerId;
        Log.Debug(AM003(daten.MandantNr, benutzerId));

        var wert = getOhneAnmelden(daten);
        if (!string.IsNullOrEmpty(wert) && string.Compare(wert, benutzerId, true) == 0)
        {
          // Anmeldung ohne Kennwort
          Speichern(daten, daten.MandantNr, benutzerId, speichern);
          return r;
        }
        if (string.IsNullOrEmpty(benutzer.Passwort) && string.IsNullOrEmpty(kennwort))
        {
          // Anmeldung mit leerem Kennwort
          Speichern(daten, daten.MandantNr, benutzerId, speichern);
          return r;
        }
        if (!string.IsNullOrEmpty(benutzer.Passwort) && !string.IsNullOrEmpty(kennwort))
        {
          // Anmeldung mit Kennwort
          if (benutzer.Passwort == kennwort)
          {
            Speichern(daten, daten.MandantNr, benutzerId, speichern);
            return r;
          }
        }
      }

      // Anzumeldenden Benutzer als Benutzer eintragen
      var liste = BenutzerRep.GetList(daten, daten.MandantNr);
      if (liste.Count == 1 && Constants.USER_ID == liste[0].Benutzer_ID
        && !string.Equals(daten.BenutzerId, Constants.USER_ID, StringComparison.InvariantCultureIgnoreCase))
      {
        r.Ergebnis = daten.BenutzerId;
        BenutzerRep.Save(daten, daten.MandantNr, daten.BenutzerId, kennwort, liste[0].Berechtigung, liste[0].Akt_Periode,
          liste[0].Person_Nr, liste[0].Geburt, liste[0].Angelegt_Von, liste[0].Angelegt_Am, daten.BenutzerId, daten.Jetzt);
        BenutzerRep.Delete(daten, liste[0]);
        Speichern(daten, daten.MandantNr, daten.BenutzerId, speichern);
        return r;
      }

      // Die Anmeldedaten sind ungültig. Mandant, Benutzer oder Kennwort ungültig.
      throw new MessageException(AM001);
    }

    public ServiceErgebnis ChangePassword(ServiceDaten daten, int client, string id, string passwordold,
        string passwordnew, bool speichern)
    {
      var r = new ServiceErgebnis();
      if (client < 0)
      {
        // Die Anmeldedaten sind ungültig. Mandant ungültig.
        r.Errors.Add(Message.New(AM001));
      }
      else if (string.IsNullOrEmpty(id))
      {
        // Die Anmeldedaten sind ungültig. Benutzer ungültig.
        r.Errors.Add(Message.New(AM001));
      }
      if (string.IsNullOrEmpty(passwordnew))
      {
        // Das neue Kennwort darf nicht leer sein.
        r.Errors.Add(Message.New(AM002));
      }
      if (!r.Ok)
        return r;
      var benutzer = BenutzerRep.Get(daten, client, id);
      if (benutzer == null)
      {
        // Die Anmeldedaten sind ungültig. Der Benutzer wurde nicht gefunden.
        throw new MessageException(AM001);
      }
      if (Functions.CompString(benutzer.Passwort, passwordold) != 0)
      {
        // Die Anmeldedaten sind ungültig. Altes Kennwort ist falsch.
        throw new MessageException(AM001);
      }
      benutzer.Passwort = passwordnew;
      BenutzerRep.Update(daten, benutzer);
      Speichern(daten, client, id, speichern);
      return r;
    }

    public ServiceErgebnis Logout(ServiceDaten daten)
    {
      BenutzerRep.Get(daten, 0, null);
      return new ServiceErgebnis();
    }

    private static string getOhneAnmelden(ServiceDaten daten)
    {
      return MaParameterRep.Get(daten, daten.MandantNr, Constants.EINST_MA_OHNE_ANMELDUNG)?.Wert;
    }

    private static void Speichern(ServiceDaten daten, int mandantNr, string benutzerId, bool speichern)
    {
      var maEinstellung = MaParameterRep.Get(daten, mandantNr, Constants.EINST_MA_OHNE_ANMELDUNG);
      if (maEinstellung == null)
      {
        maEinstellung = new MaParameter
        {
          Mandant_Nr = mandantNr,
          Schluessel = Constants.EINST_MA_OHNE_ANMELDUNG,
          Wert = ""
        };
        MaParameterRep.Insert(daten, maEinstellung);
      }
      var wert = maEinstellung.Wert;
      if (!speichern && !string.IsNullOrEmpty(wert) && string.Compare(wert, benutzerId, true) == 0)
      {
        maEinstellung.Wert = "";
        // System.out.println("Ist  " + maEinstellung.toBuffer(null))
        MaParameterRep.Update(daten, maEinstellung);
      }
      if (speichern && (string.IsNullOrEmpty(wert) || string.Compare(wert, benutzerId, true) != 0))
      {
        maEinstellung.Wert = benutzerId;
        MaParameterRep.Update(daten, maEinstellung);
      }
      InitMandant(daten);
    }

    private static void InitMandant(ServiceDaten daten)
    {
      var e = MaParameterRep.Get(daten, daten.MandantNr, Constants.EINST_MA_REPLIKATION_UID);
      if (e == null || string.IsNullOrEmpty(e.Wert))
      {
        MaParameterRep.Save(daten, daten.MandantNr, Constants.EINST_MA_REPLIKATION_UID,
            Guid.NewGuid().ToString());
      }
      e = MaParameterRep.Get(daten, daten.MandantNr, "AG_BACKUPS");
      if (e != null)
        MaParameterRep.Delete(daten, e);
      e = MaParameterRep.Get(daten, daten.MandantNr, "ANWENDUNGS_TITEL");
      if (e != null)
        MaParameterRep.Delete(daten, e);
    }

    /// <summary>
    /// Undo last transaction.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <returns>Is anything changed?</returns>
    public ServiceErgebnis<bool> Undo(ServiceDaten daten)
    {
      var c = Undo0(daten);
      return new ServiceErgebnis<bool>(c);
    }

    /// <summary>
    /// Redo last transaction.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <returns>Is anything changed?</returns>
    public ServiceErgebnis<bool> Redo(ServiceDaten daten)
    {
      var c = Redo0(daten);
      return new ServiceErgebnis<bool>(c);
    }

    /// <summary>Test function.</summary>
    /// <param name="daten">Service data for database access.</param>
    /// <returns>Possibly errors.</returns>
    public ServiceErgebnis Test(ServiceDaten daten)
    {
      var r = new ServiceErgebnis();
      var id = "Test";
      var b = BenutzerRep.Save(daten, daten.MandantNr, id, null, 0, 0, 0, null);
      b = BenutzerRep.GetList(daten, daten.MandantNr).FirstOrDefault(a => a.Benutzer_ID == id);
      if (b == null)
        throw new Exception("Datensatz nicht eingetragen.");
      SaveChanges(daten);
      b = BenutzerRep.GetList(daten, daten.MandantNr).FirstOrDefault(a => a.Benutzer_ID == id);
      if (b == null)
        throw new Exception("Datensatz immer noch nicht eingetragen.");
      BenutzerRep.Delete(daten, b);
      //b = BenutzerRep.GetList(daten, daten.MandantNr).FirstOrDefault(a => a.Benutzer_ID == id);
      //if (b != null)
      //    throw new Exception("Datensatz nicht gelöscht.");
      SaveChanges(daten);
      b = BenutzerRep.GetList(daten, daten.MandantNr).FirstOrDefault(a => a.Benutzer_ID == id);
      if (b != null)
        throw new Exception("Datensatz immer noch nicht gelöscht.");
      b = BenutzerRep.Save(daten, daten.MandantNr, id, null, 0, 0, 0, null);
      //b = BenutzerRep.GetList(daten, daten.MandantNr).FirstOrDefault(a => a.Benutzer_ID == id);
      //if (b == null)
      //    throw new Exception("Datensatz nicht wieder eingetragen.");
      SaveChanges(daten);
      b = BenutzerRep.GetList(daten, daten.MandantNr).FirstOrDefault(a => a.Benutzer_ID == id);
      if (b == null)
        throw new Exception("Datensatz immer noch nicht wieder eingetragen.");
      return r;
    }
  }
}