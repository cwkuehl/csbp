// <copyright file="LoginService.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services;

using System;
using System.Linq;
using CSBP.Services.Apis.Models;
using CSBP.Services.Apis.Services;
using CSBP.Services.Base;
using static CSBP.Services.Resources.M;
using static CSBP.Services.Resources.Messages;

/// <summary>
/// Implementation of login service.
/// </summary>
public class LoginService : ServiceBase, ILoginService
{
  /// <summary>
  /// Checks whether login is without password or not.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <returns>User data with roles if login is without password or null.</returns>
  public ServiceErgebnis<UserDaten> IsWithoutPassword(ServiceDaten daten)
  {
    var r = new ServiceErgebnis<UserDaten>();
    var wert = GetWithoutLogin(daten);
    if (!string.IsNullOrEmpty(wert) && !string.IsNullOrEmpty(daten.BenutzerId) && string.Compare(wert, daten.BenutzerId, true) == 0)
    {
      var benutzer = BenutzerRep.Get(daten, daten.MandantNr, daten.BenutzerId);
      if (benutzer != null)
      {
        // User exists.
        var benutzerId = benutzer.Benutzer_ID;
        var rollen = new List<string> { benutzer.Berechtigung == 2 ? UserDaten.RoleSuperadmin : benutzer.Berechtigung == 1 ? UserDaten.RoleAdmin : UserDaten.RoleUser };
        var ud = new UserDaten(daten.Daten.SessionId, daten.MandantNr, benutzerId, rollen);
        r.Ergebnis = ud;
        Log.Debug(AM003(daten.MandantNr, benutzerId));
      }
      //// if (r.ergebnis) {
      ////   initMandant(daten)
      //// }
    }
    return r;
  }

  /// <summary>
  /// Do login with all checks.
  /// </summary>
  /// <param name="daten">Affected client number and user id.</param>
  /// <param name="kennwort">Affected password.</param>
  /// <param name="speichern">Save password or not.</param>
  /// <returns>User data with roles and possibly errors.</returns>
  public ServiceErgebnis<UserDaten> Login(ServiceDaten daten, string kennwort, bool speichern)
  {
    if (daten.MandantNr < 0)
    {
      // Invalid login. Invalid client.
      throw new MessageException(AM001);
    }
    if (string.IsNullOrWhiteSpace(daten.BenutzerId))
    {
      // Invalid login. Invalid user.
      throw new MessageException(AM001);
    }

    var r = new ServiceErgebnis<UserDaten>();
    var benutzer = BenutzerRep.Get(daten, daten.MandantNr, daten.BenutzerId);
    if (benutzer != null)
    {
      // User exists.
      var benutzerId = benutzer.Benutzer_ID;
      var rollen = new List<string> { benutzer.Berechtigung == 2 ? UserDaten.RoleSuperadmin : benutzer.Berechtigung == 1 ? UserDaten.RoleAdmin : UserDaten.RoleUser };
      var ud = new UserDaten(daten.Daten.SessionId, daten.MandantNr, benutzerId, rollen);
      r.Ergebnis = ud;
      Log.Debug(AM003(daten.MandantNr, benutzerId));

      var wert = GetWithoutLogin(daten);
      if (!string.IsNullOrEmpty(wert) && string.Compare(wert, benutzerId, true) == 0)
      {
        // Login without password.
        Speichern(daten, daten.MandantNr, benutzerId, speichern);
        return r;
      }
      if (string.IsNullOrEmpty(benutzer.Passwort) && string.IsNullOrEmpty(kennwort))
      {
        // Login with empty password.
        Speichern(daten, daten.MandantNr, benutzerId, speichern);
        return r;
      }
      if (!string.IsNullOrEmpty(benutzer.Passwort) && !string.IsNullOrEmpty(kennwort))
      {
        // Login with password
        if (benutzer.Passwort == kennwort)
        {
          Speichern(daten, daten.MandantNr, benutzerId, speichern);
          return r;
        }
      }
    }

    // Saves current user as user.
    var liste = BenutzerRep.GetList(daten, daten.MandantNr);
    if (liste.Count == 1 && liste[0].Benutzer_ID == Constants.USER_ID
      && !string.Equals(daten.BenutzerId, Constants.USER_ID, StringComparison.InvariantCultureIgnoreCase))
    {
      var rollen = new List<string> { liste[0].Berechtigung == 2 ? UserDaten.RoleSuperadmin : liste[0].Berechtigung == 1 ? UserDaten.RoleAdmin : UserDaten.RoleUser };
      var ud = new UserDaten(daten.Daten.SessionId, daten.MandantNr, daten.BenutzerId, rollen);
      r.Ergebnis = ud;
      BenutzerRep.Save(daten, daten.MandantNr, daten.BenutzerId, kennwort, liste[0].Berechtigung, liste[0].Akt_Periode,
        liste[0].Person_Nr, liste[0].Geburt, liste[0].Parameter, liste[0].Angelegt_Von, liste[0].Angelegt_Am, daten.BenutzerId, daten.Jetzt);
      BenutzerRep.Delete(daten, liste[0]);
      Speichern(daten, daten.MandantNr, daten.BenutzerId, speichern);
      return r;
    }

    // Invalid login. Client, user or password are invalid.
    throw new MessageException(AM001);
  }

  /// <summary>
  /// Changes the password of an user.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="client">Affected client id.</param>
  /// <param name="id">Affected user id.</param>
  /// <param name="passwordold">Old password.</param>
  /// <param name="passwordnew">New password.</param>
  /// <param name="save">Save password or not.</param>
  /// <returns>Possibly errors.</returns>
  public ServiceErgebnis ChangePassword(ServiceDaten daten, int client, string id, string passwordold,
    string passwordnew, bool save)
  {
    var r = new ServiceErgebnis();
    if (client < 0)
    {
      // Invalid login. Invalid client.
      r.Errors.Add(Message.New(AM001));
    }
    else if (string.IsNullOrEmpty(id))
    {
      // Invalid login. Invalid user.
      r.Errors.Add(Message.New(AM001));
    }
    if (string.IsNullOrEmpty(passwordnew))
    {
      // The new password must not be empty.
      r.Errors.Add(Message.New(AM002));
    }
    if (!r.Ok)
      return r;
    var benutzer = BenutzerRep.Get(daten, client, id) ?? throw new MessageException(AM001);
    if (Functions.CompString(benutzer.Passwort, passwordold) != 0)
    {
      // Invalid login. Old password wrong.
      throw new MessageException(AM001);
    }
    benutzer.Passwort = passwordnew;
    BenutzerRep.Update(daten, benutzer);
    Speichern(daten, client, id, save);
    return r;
  }

  /// <summary>
  /// Does the logout.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <returns>Possibly errors.</returns>
  public ServiceErgebnis Logout(ServiceDaten daten)
  {
    BenutzerRep.Get(daten, 0, null);
    return new ServiceErgebnis();
  }

  /// <summary>
  /// Undo last transaction.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <returns>Is anything changed or not.</returns>
  public ServiceErgebnis<bool> Undo(ServiceDaten daten)
  {
    var c = Undo0(daten);
    return new ServiceErgebnis<bool>(c);
  }

  /// <summary>
  /// Redoes last transaction.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <returns>Is anything changed or not.</returns>
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
    var b = BenutzerRep.Save(daten, daten.MandantNr, id, null, 0, 0, 0, null, null);
    b = BenutzerRep.GetList(daten, daten.MandantNr).FirstOrDefault(a => a.Benutzer_ID == id);
    if (b == null)
      throw new Exception("Datensatz nicht eingetragen.");
    SaveChanges(daten);
    b = BenutzerRep.GetList(daten, daten.MandantNr).FirstOrDefault(a => a.Benutzer_ID == id);
    if (b == null)
      throw new Exception("Datensatz immer noch nicht eingetragen.");
    BenutzerRep.Delete(daten, b);
    //// b = BenutzerRep.GetList(daten, daten.MandantNr).FirstOrDefault(a => a.Benutzer_ID == id);
    //// if (b != null)
    ////    throw new Exception("Datensatz nicht gelöscht.");
    SaveChanges(daten);
    b = BenutzerRep.GetList(daten, daten.MandantNr).FirstOrDefault(a => a.Benutzer_ID == id);
    if (b != null)
      throw new Exception("Datensatz immer noch nicht gelöscht.");
    b = BenutzerRep.Save(daten, daten.MandantNr, id, null, 0, 0, 0, null, null);
    //// b = BenutzerRep.GetList(daten, daten.MandantNr).FirstOrDefault(a => a.Benutzer_ID == id);
    //// if (b == null)
    ////    throw new Exception("Datensatz nicht wieder eingetragen.");
    SaveChanges(daten);
    b = BenutzerRep.GetList(daten, daten.MandantNr).FirstOrDefault(a => a.Benutzer_ID == id);
    if (b == null)
      throw new Exception("Datensatz immer noch nicht wieder eingetragen.");
    return r;
  }

  /// <summary>
  /// Gets saved user id which does not need to input a password.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <returns>Saved user id which does not need to input a password.</returns>
  private static string GetWithoutLogin(ServiceDaten daten)
  {
    return MaParameterRep.Get(daten, daten.MandantNr, Constants.EINST_MA_OHNE_ANMELDUNG)?.Wert;
  }

  /// <summary>
  /// Saves user id without login und inits database.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="client">Affected client number.</param>
  /// <param name="userid">Affected user id.</param>
  /// <param name="save">Saves or removes user id.</param>
  private static void Speichern(ServiceDaten daten, int client, string userid, bool save)
  {
    var maEinstellung = MaParameterRep.Get(daten, client, Constants.EINST_MA_OHNE_ANMELDUNG);
    if (maEinstellung == null)
    {
      maEinstellung = new MaParameter
      {
        Mandant_Nr = client,
        Schluessel = Constants.EINST_MA_OHNE_ANMELDUNG,
        Wert = "",
      };
      MaParameterRep.Insert(daten, maEinstellung);
    }
    var wert = maEinstellung.Wert;
    if (!save && !string.IsNullOrEmpty(wert) && string.Compare(wert, userid, true) == 0)
    {
      maEinstellung.Wert = "";
      //// System.out.println("Ist  " + maEinstellung.toBuffer(null))
      MaParameterRep.Update(daten, maEinstellung);
    }
    if (save && (string.IsNullOrEmpty(wert) || string.Compare(wert, userid, true) != 0))
    {
      maEinstellung.Wert = userid;
      MaParameterRep.Update(daten, maEinstellung);
    }
    InitMandant(daten);
  }

  /// <summary>
  /// Initializes or optimizes database.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
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
}
