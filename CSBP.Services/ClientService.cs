// <copyright file="ClientService.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services;

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using CSBP.Services.Apis.Enums;
using CSBP.Services.Apis.Models;
using CSBP.Services.Apis.Models.Extension;
using CSBP.Services.Apis.Services;
using CSBP.Services.Base;
using CSBP.Services.Base.Csv;
using CSBP.Services.Client;
using CSBP.Services.NonService;
using CSBP.Services.Reports;
using CSBP.Services.Repositories.Base;
using CSBP.Services.Undo;
using static CSBP.Services.Resources.M;
using static CSBP.Services.Resources.Messages;

/// <summary>
/// Implementation of client service.
/// </summary>
public partial class ClientService : ServiceBase, IClientService
{
  /// <summary>Sets budget service.</summary>
  public IBudgetService BudgetService { private get; set; }

  /// <summary>Sets private service.</summary>
  public IPrivateService PrivateService { private get; set; }

  /// <summary>
  /// Initializes the database.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <returns>Possibly errors.</returns>
  public ServiceErgebnis InitDb(ServiceDaten daten)
  {
    var p = MaParameterRep.Get(daten, 0, Constants.EINST_DB_VERSION);
    if (p == null)
    {
      p = new MaParameter
      {
        Mandant_Nr = 0,
        Schluessel = Constants.EINST_DB_VERSION,
        Wert = "55",
      };
      MaParameterRep.Insert(daten, p);
      SaveChanges(daten);
    }
    var weiter = true;
    var version = Functions.ToInt32(p.Wert);
    var versionalt = version;
    while (weiter)
    {
      if (version <= 53)
      {
        var dba = new DbAlter(DatabaseTypeEnum.SqLite);
        var mout = new List<string>();
        var tab = "WP_Wertpapier";
        dba.AddTab0();
        dba.AddTab1("Mandant_Nr", "D_INTEGER", false);
        dba.AddTab1("Uid", "D_REPL_ID", false);
        dba.AddTab1("Bezeichnung", "D_STRING_50", false);
        dba.AddTab1("Kuerzel", "D_STRING_20", false); // vorher 10
        dba.AddTab1("Parameter", "D_MEMO", true);
        dba.AddTab1("Datenquelle", "D_STRING_35", false);
        dba.AddTab1("Status", "D_STRING_10", false);
        dba.AddTab1("Relation_Uid", "D_REPL_ID", true);
        dba.AddTab1("Notiz", "D_MEMO", true);
        dba.AddTab1("Angelegt_Von", "D_STRING_20", true);
        dba.AddTab1("Angelegt_Am", "D_DATETIME", true);
        dba.AddTab1("Geaendert_Von", "D_STRING_20", true);
        dba.AddTab1("Geaendert_Am", "D_DATETIME", true);
        dba.AddTab2(mout, tab, "Mandant_Nr, Uid", "Mandant_Nr, Uid");
        MaMandantRep.Execute(daten, mout);
        version = 54;
      }
      else if (version <= 54)
      {
        var mout = new List<string>
        {
          "update sb_person set quelle_uid=null where quelle_uid='0'",
        };
        MaMandantRep.Execute(daten, mout);
        version = 55;
      }
      else if (version <= 55)
      {
        var mp = MaParameterRep.Get(daten, 0, Constants.EINST_DATENBANK);
        if (mp == null)
        {
          mp = new MaParameter
          {
            Mandant_Nr = 0,
            Schluessel = Constants.EINST_DATENBANK,
            Wert = "SQLITE",
          };
          MaParameterRep.Insert(daten, mp);
        }
        else
        {
          mp.Wert = "SQLITE";
          MaParameterRep.Update(daten, mp);
        }
        mp = MaParameterRep.Get(daten, 0, Constants.EINST_DB_INIT);
        if (mp == null)
        {
          mp = new MaParameter
          {
            Mandant_Nr = 0,
            Schluessel = Constants.EINST_DB_INIT,
            Wert = "0",
          };
          MaParameterRep.Insert(daten, mp);
        }
        else
        {
          mp.Wert = "0";
          MaParameterRep.Update(daten, mp);
        }
        var dba = new DbAlter(DatabaseTypeEnum.SqLite);
        var mout = new List<string>();
        var tab = "TB_Ort";
        dba.CreateTab0();
        dba.CreateTab1("Mandant_Nr", "D_INTEGER", false);
        dba.CreateTab1("Uid", "D_REPL_ID", false);
        dba.CreateTab1("Bezeichnung", "D_STRING_50", true);
        dba.CreateTab1("Breite", "D_GELDBETRAG", false);
        dba.CreateTab1("Laenge", "D_GELDBETRAG", false);
        dba.CreateTab1("Hoehe", "D_GELDBETRAG", false);
        dba.CreateTab1("Notiz", "D_MEMO", true);
        dba.CreateTab1("Angelegt_Von", "D_STRING_20", true);
        dba.CreateTab1("Angelegt_Am", "D_DATETIME", true);
        dba.CreateTab1("Geaendert_Von", "D_STRING_20", true);
        dba.CreateTab1("Geaendert_Am", "D_DATETIME", true);
        dba.CreateTab2(mout, tab, "Mandant_Nr, Uid");
        tab = "TB_Eintrag_Ort";
        dba.CreateTab0();
        dba.CreateTab1("Mandant_Nr", "D_INTEGER", false);
        dba.CreateTab1("Ort_Uid", "D_REPL_ID", false);
        dba.CreateTab1("Datum_Von", "D_DATE", false);
        dba.CreateTab1("Datum_Bis", "D_DATE", false);
        dba.CreateTab1("Angelegt_Von", "D_STRING_20", true);
        dba.CreateTab1("Angelegt_Am", "D_DATETIME", true);
        dba.CreateTab1("Geaendert_Von", "D_STRING_20", true);
        dba.CreateTab1("Geaendert_Am", "D_DATETIME", true);
        dba.CreateTab2(mout, tab, "Mandant_Nr, Ort_Uid, Datum_Von, Datum_Bis");

        // mout.Add("INSERT INTO TB_Ort(Mandant_Nr,Uid,Breite,Laenge,Hoehe,Notiz,Angelegt_Von,Angelegt_Am) VALUES(1,'2',3,4,5,'6','ich','2020-08-15');");
        // dba.AddTab0();
        // dba.AddTab1("Mandant_Nr", "D_INTEGER", false);
        // dba.AddTab1("Uid", "D_REPL_ID", false);
        // dba.AddTab1("Bezeichnung", "D_STRING_50", true);
        // dba.AddTab1("Breite", "D_GELDBETRAG", false);
        // dba.AddTab1("Laenge", "D_GELDBETRAG", false);
        // dba.AddTab1("Hoehe", "D_GELDBETRAG", false);
        // dba.AddTab1("Notiz", "D_MEMO", true);
        // dba.AddTab1a("Nix", "D_GELDBETRAG", false, "27");
        // dba.AddTab1("Angelegt_Von", "D_STRING_20", true);
        // dba.AddTab1("Angelegt_Am", "D_DATETIME", true);
        // dba.AddTab1("Geaendert_Von", "D_STRING_20", true);
        // dba.AddTab1("Geaendert_Am", "D_DATETIME", true);
        // dba.AddTab2(mout, tab, "Mandant_Nr, Uid", "Mandant_Nr, Uid");
        mout.Add($"DELETE FROM MA_PARAMETER WHERE MANDANT_NR=0 AND NOT SCHLUESSEL IN ('{Constants.EINST_DATENBANK}','{Constants.EINST_DB_INIT}','{Constants.EINST_DB_VERSION}');");
        MaMandantRep.Execute(daten, mout);
        version = 56;
      }
      else if (version <= 56)
      {
        var dba = new DbAlter(DatabaseTypeEnum.SqLite);
        var mout = new List<string>();
        var tab = "FZ_Buch";
        dba.AddTab0();
        dba.AddTab1("Mandant_Nr", "D_INTEGER", false);
        dba.AddTab1("Uid", "D_REPL_ID", false);
        dba.AddTab1("Autor_Uid", "D_REPL_ID", false);
        dba.AddTab1("Serie_Uid", "D_REPL_ID", false);
        dba.AddTab1("Seriennummer", "D_INTEGER", false);
        dba.AddTab1("Titel", "D_STRING_255", false); // vorher 100
        dba.AddTab1a("Untertitel", "D_STRING_255", true, "NULL");
        dba.AddTab1("Seiten", "D_INTEGER", false);
        dba.AddTab1("Sprache_Nr", "D_INTEGER", false);
        dba.AddTab1a("Notiz", "D_MEMO", true, "NULL");
        dba.AddTab1("Angelegt_Von", "D_STRING_20", true);
        dba.AddTab1("Angelegt_Am", "D_DATETIME", true);
        dba.AddTab1("Geaendert_Von", "D_STRING_20", true);
        dba.AddTab1("Geaendert_Am", "D_DATETIME", true);
        dba.AddTab2(mout, tab, "Mandant_Nr, Uid", "Mandant_Nr, Uid");
        tab = "FZ_Buchautor";
        dba.AddTab0();
        dba.AddTab1("Mandant_Nr", "D_INTEGER", false);
        dba.AddTab1("Uid", "D_REPL_ID", false);
        dba.AddTab1("Name", "D_STRING_255", false); // vorher 50
        dba.AddTab1("Vorname", "D_STRING_255", true); // vorher 50
        dba.AddTab1a("Notiz", "D_MEMO", true, "NULL");
        dba.AddTab1("Angelegt_Von", "D_STRING_20", true);
        dba.AddTab1("Angelegt_Am", "D_DATETIME", true);
        dba.AddTab1("Geaendert_Von", "D_STRING_20", true);
        dba.AddTab1("Geaendert_Am", "D_DATETIME", true);
        dba.AddTab2(mout, tab, "Mandant_Nr, Uid", "Mandant_Nr, Uid");
        tab = "FZ_Buchserie";
        dba.AddTab0();
        dba.AddTab1("Mandant_Nr", "D_INTEGER", false);
        dba.AddTab1("Uid", "D_REPL_ID", false);
        dba.AddTab1("Name", "D_STRING_255", false);
        dba.AddTab1a("Notiz", "D_MEMO", true, "NULL");
        dba.AddTab1("Angelegt_Von", "D_STRING_20", true);
        dba.AddTab1("Angelegt_Am", "D_DATETIME", true);
        dba.AddTab1("Geaendert_Von", "D_STRING_20", true);
        dba.AddTab1("Geaendert_Am", "D_DATETIME", true);
        dba.AddTab2(mout, tab, "Mandant_Nr, Uid", "Mandant_Nr, Uid");
        MaMandantRep.Execute(daten, mout);
        version = 57;
      }
      else if (version <= 57)
      {
        var dba = new DbAlter(DatabaseTypeEnum.SqLite);
        var mout = new List<string>();
        var tab = "TB_Ort";
        dba.AddTab0();
        dba.AddTab1("Mandant_Nr", "D_INTEGER", false);
        dba.AddTab1("Uid", "D_REPL_ID", false);
        dba.AddTab1("Bezeichnung", "D_STRING_50", true);
        dba.AddTab1("Breite", "D_GELDBETRAG", false);
        dba.AddTab1("Laenge", "D_GELDBETRAG", false);
        dba.AddTab1("Hoehe", "D_GELDBETRAG", false);
        dba.AddTab1a("Zeitzone", "D_STRING_50", true, "NULL");
        dba.AddTab1("Notiz", "D_MEMO", true);
        dba.AddTab1("Angelegt_Von", "D_STRING_20", true);
        dba.AddTab1("Angelegt_Am", "D_DATETIME", true);
        dba.AddTab1("Geaendert_Von", "D_STRING_20", true);
        dba.AddTab1("Geaendert_Am", "D_DATETIME", true);
        dba.AddTab2(mout, tab, "Mandant_Nr, Uid", "Mandant_Nr, Uid");
        MaMandantRep.Execute(daten, mout);
        version = 58;
      }
      else if (version <= 58)
      {
        var dba = new DbAlter(DatabaseTypeEnum.SqLite);
        var mout = new List<string>();
        var tab = "TB_Wetter";
        dba.CreateTab0();
        dba.CreateTab1("Mandant_Nr", "D_INTEGER", false);
        dba.CreateTab1("Datum", "D_DATE", false);
        dba.CreateTab1("Ort_Uid", "D_REPL_ID", false);
        dba.CreateTab1("Api", "D_STRING_10", false);
        dba.CreateTab1("Werte", "D_MEMO", true);
        dba.CreateTab1("Angelegt_Von", "D_STRING_20", true);
        dba.CreateTab1("Angelegt_Am", "D_DATETIME", true);
        dba.CreateTab1("Geaendert_Von", "D_STRING_20", true);
        dba.CreateTab1("Geaendert_Am", "D_DATETIME", true);
        dba.CreateTab2(mout, tab, "Mandant_Nr, Datum, Ort_Uid, Api");
        MaMandantRep.Execute(daten, mout);
        version = 59;
      }
      else if (version <= 59)
      {
        var dba = new DbAlter(DatabaseTypeEnum.SqLite);
        var mout = new List<string>();
        var tab = "AG_Dialog";
        dba.CreateTab0();
        dba.CreateTab1("Mandant_Nr", "D_INTEGER", false);
        dba.CreateTab1("Uid", "D_REPL_ID", false);
        dba.CreateTab1("Api", "D_STRING_10", false);
        dba.CreateTab1("Datum", "D_DATE", false);
        dba.CreateTab1("Nr", "D_INTEGER", false);
        dba.CreateTab1("Url", "D_STRING_255", true);
        dba.CreateTab1("Frage", "D_MEMO", true);
        dba.CreateTab1("Antwort", "D_MEMO", true);
        dba.CreateTab1("Angelegt_Von", "D_STRING_20", true);
        dba.CreateTab1("Angelegt_Am", "D_DATETIME", true);
        dba.CreateTab1("Geaendert_Von", "D_STRING_20", true);
        dba.CreateTab1("Geaendert_Am", "D_DATETIME", true);
        dba.CreateTab2(mout, tab, "Mandant_Nr, Uid");
        dba.CreateTab3(mout, tab, "SK_AG_Dialog", true, "Mandant_Nr, Api, Datum, Nr, Uid");
        MaMandantRep.Execute(daten, mout);
        version = 60;
      }
      else if (version <= 60)
      {
        var dba = new DbAlter(DatabaseTypeEnum.SqLite);
        var mout = new List<string>();
        var tab = "Benutzer";
        dba.AddTab0();
        dba.AddTab1("Mandant_Nr", "D_INTEGER", false);
        dba.AddTab1("Benutzer_ID", "D_STRING_20", false);
        dba.AddTab1("Passwort", "D_STRING_50", true);
        dba.AddTab1("Berechtigung", "D_INTEGER", false);
        dba.AddTab1("Akt_Periode", "D_INTEGER", false);
        dba.AddTab1("Person_Nr", "D_INTEGER", false);
        dba.AddTab1("Geburt", "D_DATE", true);
        dba.AddTab1a("Parameter", "D_MEMO", true, "NULL");
        dba.AddTab1("Angelegt_Von", "D_STRING_20", true);
        dba.AddTab1("Angelegt_Am", "D_DATETIME", true);
        dba.AddTab1("Geaendert_Von", "D_STRING_20", true);
        dba.AddTab1("Geaendert_Am", "D_DATETIME", true);
        dba.AddTab2(mout, tab, "Mandant_Nr, Benutzer_ID", "Mandant_Nr, Benutzer_ID");
        MaMandantRep.Execute(daten, mout);
        version = 61;
      }
      if (versionalt < version)
      {
        p.Wert = Functions.ToString(version);
        MaParameterRep.Update(daten, p);
        versionalt = version;
      }
      else
        weiter = false;
    }
    return new ServiceErgebnis();
  }

  /// <summary>
  /// Returns a CSV file with all data of a form.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="page">Affected page, e.g. "AG100".</param>
  /// <param name="rm">Affected read model for filtering and sorting.</param>
  /// <returns>CSV file as string.</returns>
  public ServiceErgebnis<string> GetCsvString(ServiceDaten daten, string page, TableReadModel rm)
  {
    var r = new ServiceErgebnis<string>();
    if (!(page == "AG100") || rm == null)
      return r;
    rm.NoPaging = true;
    var cs = new CsvWriter();
    if (page == "AG100")
    {
      var l = MaMandantRep.GetList(daten, rm);
      cs.AddCsvLine(["Nr", "Beschreibung", "Angelegt_Am", "Angelegt_Von", "Geaendert_Am", "Geaendert_Von"]);
      foreach (var o in l)
      {
        cs.AddCsvLine([Functions.ToString(o.Nr), o.Beschreibung, Functions.ToString(o.Angelegt_Am), o.Angelegt_Von, Functions.ToString(o.Geaendert_Am), o.Geaendert_Von]);
      }
    }
    return new ServiceErgebnis<string>(cs.GetContent());
  }

  /// <summary>
  /// Gets list with clients.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="rm">Affected read model for filtering and sorting.</param>
  /// <returns>List with clients.</returns>
  public ServiceErgebnis<List<MaMandant>> GetClientList(ServiceDaten daten, TableReadModel rm)
  {
    var l = MaMandantRep.GetList(daten, rm);
    //// var user = BenutzerRep.GetList(daten, null, -1, null).FirstOrDefault();
    //// var per = user == null ? (int)PermissionEnum.Without : user.Berechtigung;
    //// if (per <= (int)PermissionEnum.Admin)
    ////   l = l.Where(a => a.Nr == daten.MandantNr).ToList();
    return new ServiceErgebnis<List<MaMandant>>(l);
  }

  /// <summary>
  /// Gets client by number.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="nr">Affected client number.</param>
  /// <returns>Affected client or null.</returns>
  public ServiceErgebnis<MaMandant> GetClient(ServiceDaten daten, int nr)
  {
    var e = MaMandantRep.Get(daten, nr);
    return new ServiceErgebnis<MaMandant>(e);
  }

  /// <summary>
  /// Saves a client.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="nr">Affected client number.</param>
  /// <param name="desc">Affected description.</param>
  /// <returns>Created or changed entity.</returns>
  public ServiceErgebnis<MaMandant> SaveClient(ServiceDaten daten, int nr, string desc)
  {
    if (string.IsNullOrWhiteSpace(desc))
    {
      throw new MessageException(AM008);
    }
    if (nr <= 0)
    {
      var last = MaMandantRep.GetList(daten).LastOrDefault();
      nr = last == null ? 1 : last.Nr + 1;
      if (BenutzerRep.Get(daten, nr, Constants.USER_ID) == null)
      {
        var b = new Benutzer
        {
          Mandant_Nr = nr,
          Benutzer_ID = Constants.USER_ID,
          Passwort = null,
          Berechtigung = (int)PermissionEnum.Admin,
          Akt_Periode = 0,
        };
        BenutzerRep.Insert(daten, b);
      }
    }
    var e = MaMandantRep.Save(daten, nr, desc);
    return new ServiceErgebnis<MaMandant>(e);
  }

  /// <summary>
  /// Deletes a client.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Affected entity.</param>
  /// <returns>Possibly errors.</returns>
  public ServiceErgebnis DeleteClient(ServiceDaten daten, MaMandant e)
  {
    if (e != null && e.Nr == daten.MandantNr)
    {
      // Der aktuelle Mandant kann nicht gelöscht werden.
      throw new MessageException(AM004);
    }

    // Delete client in all tables.
    foreach (var t in GetAllTables())
    {
      if (!(t.Name == "Benutzer" || t.Name == "MA_Mandant"))
      {
        MaMandantRep.Execute(daten, $"DELETE FROM {t.Name} WHERE {t.ClientNumber}={e.Nr}");
      }
    }
    var blist = BenutzerRep.GetList(daten, e.Nr);
    foreach (var b in blist)
    {
      BenutzerRep.Delete(daten, b);
    }
    MaMandantRep.Delete(daten, e);
    return new ServiceErgebnis();
  }

  /// <summary>
  /// Gets a list of users.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <returns>List of users.</returns>
  public ServiceErgebnis<List<Benutzer>> GetUserList(ServiceDaten daten)
  {
    var b = GetBerechtigung(daten, daten.MandantNr, daten.BenutzerId);
    var id = b >= 1 ? null : b == 0 ? daten.BenutzerId : "###";
    var liste = BenutzerRep.GetList(daten, null, 0, id);
    var r = new ServiceErgebnis<List<Benutzer>>(liste);
    return r;
  }

  /// <summary>
  /// Gets user by number.
  /// </summary>
  /// <returns>The user.</returns>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="nr">Affected user number.</param>
  public ServiceErgebnis<Benutzer> GetUser(ServiceDaten daten, int nr)
  {
    var e = BenutzerRep.GetList(daten, null, nr, null).FirstOrDefault();
    return new ServiceErgebnis<Benutzer>(e);
  }

  /// <summary>
  /// Saves an user.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="nr">Affected user number.</param>
  /// <param name="id">Affected user ID.</param>
  /// <param name="password">Affected password.</param>
  /// <param name="permission">Affected permission.</param>
  /// <param name="birthday">Affected birthday.</param>
  /// <returns>Created or changed entity.</returns>
  public ServiceErgebnis<Benutzer> SaveUser(ServiceDaten daten, int nr, string id, string password,
      int permission, DateTime? birthday)
  {
    if (string.IsNullOrWhiteSpace(id))
    {
      throw new MessageException(AM009);
    }
    if (GetBerechtigung(daten, daten.MandantNr, daten.BenutzerId) < permission)
    {
      throw new MessageException(AM010);
    }
    var e = BenutzerRep.Get(daten, daten.MandantNr, id);
    var enr = nr;
    var liste = BenutzerRep.GetList(daten, null, 0, id, enr);
    if (liste.Count > 0)
    {
      throw new MessageException(AM011);
    }
    if (enr <= 0)
    {
      enr = 1;
      liste = BenutzerRep.GetList(daten, null, 0, null);
      foreach (var b in liste)
      {
        if (b.Person_Nr >= enr)
        {
          enr = b.Person_Nr + 1;
        }
      }
    }
    e = BenutzerRep.Save(daten, daten.MandantNr, id, password, permission,
        e == null ? 0 : e.Akt_Periode, enr, birthday);
    return new ServiceErgebnis<Benutzer>(e);
  }

  /// <summary>
  /// Deletes an user.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Affected entity.</param>
  /// <returns>Possibly errors.</returns>
  public ServiceErgebnis DeleteUser(ServiceDaten daten, Benutzer e)
  {
    if (daten.MandantNr == e.Mandant_Nr && daten.BenutzerId == e.Benutzer_ID)
    {
      throw new MessageException(AM012);
    }
    BenutzerRep.Delete(daten, e);
    return new ServiceErgebnis();
  }

  /// <summary>
  /// Gets a list of options.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="client">Affected client number.</param>
  /// <param name="parameter">Affected parameter list.</param>
  /// <returns>List of options.</returns>
  public ServiceErgebnis<List<MaParameter>> GetOptionList(ServiceDaten daten, int client,
      Dictionary<string, Parameter> parameter)
  {
    var l = MaParameterRep.GetList(daten, client);
    l = l.Where(a => !a.Schluessel.StartsWith("HP_", StringComparison.Ordinal)
        && !a.Schluessel.StartsWith("MENU_", StringComparison.Ordinal)
        && !a.Schluessel.StartsWith("MO_", StringComparison.Ordinal)
        && !a.Schluessel.StartsWith("SO_", StringComparison.Ordinal)
        && !a.Schluessel.StartsWith("VM_", StringComparison.Ordinal)
        && !a.Schluessel.StartsWith("AG_SMTP_SERVER", StringComparison.Ordinal)
        ).ToList();
    foreach (var p in parameter)
    {
      var mp = l.FirstOrDefault(a => a.Schluessel == p.Key);
      if (mp == null)
      {
        mp = new MaParameter
        {
          Mandant_Nr = client,
          Schluessel = p.Key,
          Wert = p.Value.GetValue(),
          Comment = p.Value.Comment,
          Default = p.Value.Default,
          NotDatabase = true,
        };
        l.Add(mp);
      }
      else
      {
        if (p.Value.Database)
          p.Value.SetValue(mp.Wert);
        mp.Wert = p.Value.GetValue();
        mp.Comment = p.Value.Comment;
        mp.Default = p.Value.Default;
      }
    }
    l = l.OrderBy(a => a.Schluessel).ToList();
    var r = new ServiceErgebnis<List<MaParameter>>(l);
    return r;
  }

  /// <summary>
  /// Saves an option.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="client">Affected client number.</param>
  /// <param name="p">Affected parameter.</param>
  /// <param name="value">New value.</param>
  /// <returns>Possibly errors.</returns>
  public ServiceErgebnis SaveOption(ServiceDaten daten, int client, Parameter p, string value)
  {
    var r = new ServiceErgebnis();
    if (client <= 0 || p == null)
    {
      throw new ArgumentException(null, nameof(client));
    }
    p.SetValue(value);
    if (p.Database)
    {
      MaParameterRep.Save(daten, client, p.Key, p.GetValue());
    }
    return r;
  }

  /// <summary>
  /// Saves a list of options.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="client">Affected client number.</param>
  /// <param name="olist">Affected option list.</param>
  /// <param name="plist">Affected parameter list.</param>
  /// <returns>Possibly errors.</returns>
  public ServiceErgebnis SaveOptionList(ServiceDaten daten, int client, List<MaParameter> olist, Dictionary<string, Parameter> plist)
  {
    foreach (var mp in olist)
    {
      var save = false;
      //// mp.Wert = mp.Wert ?? "";
      if (plist.TryGetValue(mp.Schluessel, out var p))
      {
        p.SetValue(mp.Wert);
        if (p.Database)
          save = true;
      }
      else if (!mp.NotDatabase)
      {
        save = true;
      }
      if (save)
      {
        MaParameterRep.Save(daten, mp.Mandant_Nr, mp.Schluessel, mp.Wert,
            mp.Angelegt_Von, mp.Geaendert_Am, mp.Geaendert_Von, mp.Geaendert_Am);
      }
    }
    return new ServiceErgebnis();
  }

  /// <summary>
  /// Gets list with backup entries.
  /// </summary>
  /// <returns>List with backup entries.</returns>
  /// <param name="daten">Service data for database access.</param>
  public ServiceErgebnis<List<BackupEntry>> GetBackupEntryList(ServiceDaten daten)
  {
    var l = BackupEntry.GetBackupEntryList();
    return new ServiceErgebnis<List<BackupEntry>>(l);
  }

  /// <summary>
  /// Gets a backup entry.
  /// </summary>
  /// <returns>Backup entry or null.</returns>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Affected ID.</param>
  public ServiceErgebnis<BackupEntry> GetBackupEntry(ServiceDaten daten, string uid)
  {
    var e = GetBackupEntryIntern(daten, uid);
    return new ServiceErgebnis<BackupEntry>(e);
  }

  /// <summary>
  /// Saves backup entry.
  /// </summary>
  /// <returns>List with backup entries.</returns>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Affected ID.</param>
  /// <param name="target">Affected target.</param>
  /// <param name="sources">Affected sources.</param>
  /// <param name="encrypted">Should the target copy be encrypted or not.</param>
  /// <param name="zipped">Should the target copy be zipped or not.</param>
  public ServiceErgebnis<BackupEntry> SaveBackupEntry(ServiceDaten daten, string uid,
      string target, string[] sources, bool encrypted, bool zipped)
  {
    var r = new ServiceErgebnis<BackupEntry>();
    if (string.IsNullOrEmpty(target))
      r.Errors.Add(Message.New(M2023));
    else if (Directory.Exists(target))
      target = Path.GetFullPath(target + Path.DirectorySeparatorChar);
    else
      r.Errors.Add(Message.New(M1038(target)));
    if (sources == null || sources.Length <= 0 || string.IsNullOrEmpty(sources[0]))
      r.Errors.Add(Message.New(M2024));
    else
    {
      for (var i = 0; i < sources.Length; i++)
        if (Directory.Exists(sources[i]))
          sources[i] = Path.GetFullPath(sources[i] + Path.DirectorySeparatorChar);
        else
          r.Errors.Add(Message.New(M1038(sources[i])));
    }
    if (!r.Ok)
      return r;
    BackupEntry a = null;
    var l = BackupEntry.GetBackupEntryList();
    if (!string.IsNullOrEmpty(uid) && l != null)
      a = l.FirstOrDefault(x => x.Uid == uid);
    var e = a ?? new BackupEntry();
    e.Uid = string.IsNullOrEmpty(uid) ? Functions.GetUid() : uid;
    e.Target = target;
    e.Sources = sources;
    e.Encrypted = encrypted;
    e.Zipped = zipped;
    if (a == null)
    {
      e.MachAngelegt(daten.Jetzt, daten.BenutzerId);
      l.Add(e);
    }
    else
    {
      e.MachGeaendert(daten.Jetzt, daten.BenutzerId);
    }
    BackupEntry.SaveBackupEntryList(l);
    r.Ergebnis = e;
    return r;
  }

  /// <summary>
  /// Deletes a backup entry.
  /// </summary>
  /// <returns>Possibly errors.</returns>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Affected entity.</param>
  public ServiceErgebnis DeleteBackupEntry(ServiceDaten daten, BackupEntry e)
  {
    if (e != null)
    {
      var l = BackupEntry.GetBackupEntryList();
      if (!string.IsNullOrEmpty(e.Uid) && l != null)
      {
        var a = l.FirstOrDefault(x => x.Uid == e.Uid);
        if (a != null)
        {
          l.Remove(a);
          BackupEntry.SaveBackupEntryList(l);
        }
      }
    }
    return new ServiceErgebnis();
  }

  /// <summary>
  /// Makes a backup.
  /// </summary>
  /// <returns>Possibly errors.</returns>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Affected ID of backup entry.</param>
  /// <param name="restore">Reverse direction of copying or not.</param>
  /// <param name="password">Password for encryption.</param>
  /// <param name="state">State of backup is always updated.</param>
  /// <param name="cancel">Cancel backup if not empty.</param>
  public ServiceErgebnis MakeBackup(ServiceDaten daten, string uid, bool restore,
      string password, StringBuilder state, StringBuilder cancel)
  {
    if (state == null || cancel == null)
      throw new ArgumentException(null, nameof(state));
    var e = GetBackupEntryIntern(daten, uid) ?? throw new MessageException(M1013);
    state.Clear().Append(M0(M1031));
    var blist = new List<BackupFile>();
    foreach (var source in e.Sources)
    {
      PrepareBackup(daten, source, e.Target, restore, e.Encrypted, e.Zipped, blist, state, cancel);
    }

    // Sortieren
    blist = blist.OrderBy(a => a.Type).ThenByDescending(a => a.Path).ToList();
    var i = 0;
    var l = blist.Count;
    while (i < l && cancel.Length <= 0)
    {
      var b = blist[i];
      i++;
      switch (b.Type)
      {
        case BackupType.CreateFolder:
          state.Clear().Append($"({i}/{l}) Verzeichnis {b.Path} anlegen.");
          Directory.CreateDirectory(b.Path);
          break;
        case BackupType.DeleteFolder:
          state.Clear().Append($"({i}/{l}) Verzeichnis {b.Path} löschen.");
          Directory.Delete(b.Path);
          break;
        case BackupType.CopyFile:
          state.Clear().Append($"({i}/{l}) Datei {b.Path} kopieren.");
          if (e.Encrypted)
          {
            if (restore)
              FileDecrypt(b.Path, b.Path2, password);
            else
              FileEncrypt(b.Path, b.Path2, password);
          }
          else
            File.Copy(b.Path, b.Path2, true);
          File.SetLastWriteTimeUtc(b.Path2, b.Modified);
          break;
        case BackupType.DeleteFile:
          state.Clear().Append($"({i}/{l}) Datei {b.Path} löschen.");
          File.Delete(b.Path);
          break;
        case BackupType.ZipFolder:
          state.Clear().Append($"({i}/{l}) Verzeichnis {b.Path} packen.");
          if (restore)
          {
            if (e.Encrypted)
            {
              var aes = b.Path2 + ".aes";
              FileDecrypt(aes, b.Path2, password);
            }
            Directory.Delete(b.Path, true);
            ZipFile.ExtractToDirectory(b.Path2, b.Path);
            if (e.Encrypted)
            {
              var aes = b.Path2 + ".aes";
              File.Delete(b.Path2);
            }
            Directory.SetLastWriteTimeUtc(b.Path, b.Modified);
          }
          else
          {
            File.Delete(b.Path2);
            ZipFile.CreateFromDirectory(b.Path, b.Path2);
            if (e.Encrypted)
            {
              var aes = b.Path2 + ".aes";
              File.Delete(aes);
              FileEncrypt(b.Path2, aes, password);
              File.Delete(b.Path2);
              File.SetLastWriteTimeUtc(aes, b.Modified);
            }
            else
              File.SetLastWriteTimeUtc(b.Path2, b.Modified);
          }
          break;
      }
    }
    i = 0;
    while (i < l && cancel.Length <= 0)
    {
      var b = blist[i];
      switch (b.Type)
      {
        case BackupType.CreateFolder:
        case BackupType.ModifyFolder:
          Directory.SetLastWriteTimeUtc(b.Path, b.Modified);
          break;
      }
      i++;
    }
    state.Clear().Append($"Ende nach dem Abgleich von {blist.Count} Verzeichnissen/Dateien.");
    return new ServiceErgebnis();
  }

  /// <summary>
  /// Replicates a table.
  /// </summary>
  /// <returns>Possibly errors.</returns>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="table">Affected table.</param>
  /// <param name="mode">Affected mode.</param>
  /// <param name="json">Affected data as json string.</param>
  public ServiceErgebnis<string> ReplicateTable(ServiceDaten daten, string table, string mode, string json)
  {
    var r = new ServiceErgebnis<string>();
    var m = PreadRegex().Match(mode);
    var days = Math.Max(1, m.Success ? Functions.ToInt32(m.Groups[3].Value) : 1);
    var ja = new List<Dictionary<string, string>>();
    if (string.IsNullOrEmpty(mode))
      r.Errors.Add(new Message("Modus fehlt.", true));
    else if (table == "TB_Eintrag")
    {
      // "{\"TB_Eintrag\":[{\"datum\":\"2023-05-12\",\"eintrag\":\"Bbbccc\",\"replid\":\"0ab93c7aa65d-4cb8-86b5-628be744342c\",\"angelegtAm\":\"2023-05-12T15:04:27.000Z\",\"angelegtVon\":\"wolfgang\",\"geaendertAm\":\"2023-05-12T15:04:45.000Z\",\"geaendertVon\":\"wolfgang\"},{\"datum\":\"2023-05-11\",\"eintrag\":\"Aaa\",\"replid\":\"94b48bd702e7-4f03-8a66-f50bfeb83edb\",\"angelegtAm\":\"2023-05-12T15:04:36.000Z\",\"angelegtVon\":\"wolfgang\"}]}"
      using var doc = JsonDocument.Parse(json ?? "");
      var root = doc.RootElement;
      var arr = root.GetProperty(table).EnumerateArray();
      while (arr.MoveNext())
      {
        var a = arr.Current;
        var e = new TbEintrag
        {
          Mandant_Nr = daten.MandantNr,
          Datum = GetDateTime(a, "datum", false) ?? DateTime.MinValue, // 2020-03-27
          Eintrag = GetString(a, "eintrag"),
          Angelegt_Am = GetDateTime(a, "angelegtAm"), // 2020-03-27T16:39:20Z
          Angelegt_Von = GetString(a, "angelegtVon"),
          Geaendert_Am = GetDateTime(a, "geaendertAm"),
          Geaendert_Von = GetString(a, "geaendertVon"),
        };
        ReplicateDiary(daten, e);
      }
      var l = TbEintragRep.GetList(daten, daten.MandantNr, daten.Heute, days);
      foreach (var e in l)
      {
        var j = new Dictionary<string, string>
        {
          { "datum", Functions.ToString(e.Datum) },
          { "eintrag", e.Eintrag },
          { "replid", "server" },
          { "angelegtAm", Functions.ToStringT(e.Angelegt_Am, true) },
          { "angelegtVon", e.Angelegt_Von },
          { "geaendertAm", Functions.ToStringT(e.Geaendert_Am, true) },
          { "geaendertVon", e.Geaendert_Von },
        };
        ja.Add(j);
      }
    }
    else if (table == "FZ_Notiz")
    {
      var l = FzNotizRep.GetList(daten, daten.MandantNr);
      foreach (var e in l)
      {
        var j = new Dictionary<string, string>
        {
          { "uid", e.Uid },
          { "thema", e.Thema },
          { "notiz", e.Notiz },
          { "replid", "server" },
          { "angelegtAm", Functions.ToStringT(e.Angelegt_Am, true) },
          { "angelegtVon", e.Angelegt_Von },
          { "geaendertAm", Functions.ToStringT(e.Geaendert_Am, true) },
          { "geaendertVon", e.Geaendert_Von },
        };
        ja.Add(j);
      }
    }
    else if (table == "HH_Buchung")
    {
      // "{\"HH_Buchung\":[{\"uid\":\"5e1b51c08631-401f-a369-11768799ccda\",\"sollValuta\":\"2023-05-12T00:00:00.000Z\",\"habenValuta\":\"2023-05-12T00:00:00.000Z\",\"kz\":\"A\",\"betrag\":2066.22,\"ebetrag\":1056.44,\"sollKontoUid\":\"5f0d190d:13e2884456a:-7f4e\",\"habenKontoUid\":\"5f0d190d:13e2884456a:-7f4b\",\"btext\":\"Wertpapierverkauf ING Spotify Technology SA\",\"belegNr\":\"xxx\",\"belegDatum\":\"2023-05-12T00:00:00.000Z\",\"replid\":\"80c7bba64830-4e5d-851c-64508cb87459\",\"angelegtAm\":\"2023-05-13T20:14:16.000Z\",\"angelegtVon\":\"wolfgang\",\"geaendertAm\":null,\"geaendertVon\":null}]}"
      var today = DateTime.Today;
      using var doc = JsonDocument.Parse(json ?? "");
      var root = doc.RootElement;
      var arr = root.GetProperty(table).EnumerateArray();
      while (arr.MoveNext())
      {
        var a = arr.Current;
        var e = new HhBuchung
        {
          Mandant_Nr = daten.MandantNr,
          Uid = GetString(a, "uid"),
          Soll_Valuta = GetDateTime(a, "sollValuta", false) ?? today,
          Haben_Valuta = GetDateTime(a, "habenValuta", false) ?? today,
          Kz = GetString(a, "kz"),
          Betrag = GetDecimal(a, "betrag") ?? 0,
          EBetrag = GetDecimal(a, "ebetrag") ?? 0,
          Soll_Konto_Uid = GetString(a, "sollKontoUid"),
          Haben_Konto_Uid = GetString(a, "habenKontoUid"),
          BText = GetString(a, "btext"),
          Beleg_Nr = GetString(a, "belegNr"),
          Beleg_Datum = GetDateTime(a, "belegDatum", false) ?? today,
          Angelegt_Am = GetDateTime(a, "angelegtAm"), // 2020-03-27T16:39:20Z
          Angelegt_Von = GetString(a, "angelegtVon"),
          Geaendert_Am = GetDateTime(a, "geaendertAm"),
          Geaendert_Von = GetString(a, "geaendertVon"),
        };
        var alt = HhBuchungRep.Get(daten, daten.MandantNr, e.Uid);
        var save = alt == null;
        if (!save && !(e.Soll_Valuta == alt.Soll_Valuta && Functions.CompString(e.Kz, alt.Kz) == 0 && e.Betrag == alt.Betrag
          && e.EBetrag == alt.EBetrag && e.Soll_Konto_Uid == alt.Soll_Konto_Uid
          && e.Haben_Konto_Uid == alt.Haben_Konto_Uid && Functions.CompString(e.BText, alt.BText) == 0
          && Functions.CompString(e.Beleg_Nr, alt.Beleg_Nr) == 0 && e.Beleg_Datum == alt.Beleg_Datum))
        {
          var date = e.Angelegt_Am ?? DateTime.MinValue;
          if (e.Geaendert_Am.HasValue && e.Geaendert_Am.Value > date)
            date = e.Geaendert_Am.Value;
          var datealt = alt.Angelegt_Am ?? DateTime.MinValue;
          if (alt.Geaendert_Am.HasValue && alt.Geaendert_Am.Value > datealt)
            datealt = alt.Geaendert_Am.Value;
          //// Wenn datealt < date, Eintrag überschreiben, sonst Eintrag lassen
          if (datealt < date)
            save = true; // Overwrite
        }
        if (save)
        {
          BudgetService.SaveBookingIntern(daten, e.Uid, e.Soll_Valuta, e.Kz,
            e.Betrag, e.EBetrag, e.Soll_Konto_Uid, e.Haben_Konto_Uid, e.BText, e.Beleg_Nr, e.Beleg_Datum,
            e.Angelegt_Von, e.Angelegt_Am, e.Geaendert_Von, e.Geaendert_Am);
        }
      }
      var l = HhBuchungRep.GetList(daten, null, null, from: daten.Heute.AddDays(-days));
      foreach (var e in l)
      {
        var j = new Dictionary<string, string>
        {
          { "uid", e.Uid },
          { "sollValuta", Functions.ToString(e.Soll_Valuta) },
          { "habenValuta", Functions.ToString(e.Haben_Valuta) },
          { "kz", e.Kz },
          { "betrag", Functions.ToString(e.Betrag, 2, Functions.CultureInfoEn) },
          { "ebetrag", Functions.ToString(e.EBetrag, 2, Functions.CultureInfoEn) },
          { "sollKontoUid", e.Soll_Konto_Uid },
          { "habenKontoUid", e.Haben_Konto_Uid },
          { "btext", e.BText },
          { "belegNr", e.Beleg_Nr },
          { "belegDatum", Functions.ToString(e.Beleg_Datum) },
          { "replid", "server" },
          { "angelegtAm", Functions.ToStringT(e.Angelegt_Am, true) },
          { "angelegtVon", e.Angelegt_Von },
          { "geaendertAm", Functions.ToStringT(e.Geaendert_Am, true) },
          { "geaendertVon", e.Geaendert_Von },
        };
        ja.Add(j);
      }
    }
    else if (table == "HH_Ereignis")
    {
      var l = HhEreignisRep.GetList(daten, daten.MandantNr);
      foreach (var e in l)
      {
        var j = new Dictionary<string, string>
        {
          { "uid", e.Uid },
          { "kz", e.Kz },
          { "sollKontoUid", e.Soll_Konto_Uid },
          { "habenKontoUid", e.Haben_Konto_Uid },
          { "bezeichnung", e.Bezeichnung },
          { "etext", e.EText },
          { "replid", "server" },
          { "angelegtAm", Functions.ToStringT(e.Angelegt_Am, true) },
          { "angelegtVon", e.Angelegt_Von },
          { "geaendertAm", Functions.ToStringT(e.Geaendert_Am, true) },
          { "geaendertVon", e.Geaendert_Von },
        };
        ja.Add(j);
      }
    }
    else if (table == "HH_Konto")
    {
      var l = HhKontoRep.GetList(daten, -1, -1, dle: daten.Heute.AddDays(-days));
      foreach (var e in l)
      {
        var j = new Dictionary<string, string>
        {
          { "uid", e.Uid },
          { "sortierung", e.Sortierung },
          { "art", e.Art },
          { "kz", e.Kz },
          { "name", e.Name },
          { "gueltigVon", Functions.ToString(e.Gueltig_Von) },
          { "gueltigBis", Functions.ToString(e.Gueltig_Bis) },
          { "periodeVon", Functions.ToString(e.Periode_Von) },
          { "periodeBis", Functions.ToString(e.Periode_Bis) },
          { "betrag", Functions.ToString(e.Betrag, 2, Functions.CultureInfoEn) },
          { "ebetrag", Functions.ToString(e.EBetrag, 2, Functions.CultureInfoEn) },
          { "replid", "server" },
          { "angelegtAm", Functions.ToStringT(e.Angelegt_Am, true) },
          { "angelegtVon", e.Angelegt_Von },
          { "geaendertAm", Functions.ToStringT(e.Geaendert_Am, true) },
          { "geaendertVon", e.Geaendert_Von },
        };
        ja.Add(j);
      }
    }
    else if (table == "FZ_Fahrrad")
    {
      var l = FzFahrradRep.GetList(daten, daten.MandantNr);
      foreach (var e in l)
      {
        var j = new Dictionary<string, string>
        {
          { "uid", e.Uid },
          { "bezeichnung", e.Bezeichnung },
          { "typ", Functions.ToString(e.Typ) },
          { "replid", "server" },
          { "angelegtAm", Functions.ToStringT(e.Angelegt_Am, true) },
          { "angelegtVon", e.Angelegt_Von },
          { "geaendertAm", Functions.ToStringT(e.Geaendert_Am, true) },
          { "geaendertVon", e.Geaendert_Von },
        };
        ja.Add(j);
      }
    }
    else if (table == "FZ_Fahrradstand")
    {
      // "{\"FZ_Fahrradstand\":[{\"fahrradUid\":\"7c185036:13e081c0a7f:-8000-\",\"datum\":\"2023-05-14\",\"nr\":0,\"zaehlerKm\":0,\"periodeKm\":123,\"periodeSchnitt\":0,\"beschreibung\":\"\",\"replid\":\"fbc2bdc04099-4795-ac19-4446950b8b52\",\"angelegtAm\":\"2023-05-08T15:43:28.000Z\",\"angelegtVon\":\"wolfgang\",\"geaendertAm\":\"2023-05-13T21:04:18.000Z\",\"geaendertVon\":\"wolfgang\"}]}"
      var today = DateTime.Today;
      using var doc = JsonDocument.Parse(json ?? "");
      var root = doc.RootElement;
      var arr = root.GetProperty(table).EnumerateArray();
      while (arr.MoveNext())
      {
        var a = arr.Current;
        var e = new FzFahrradstand
        {
          Mandant_Nr = daten.MandantNr,
          Fahrrad_Uid = GetString(a, "fahrradUid"),
          Datum = GetDateTime(a, "datum", false) ?? today, // 2020-03-27
          Nr = (int)(GetDecimal(a, "nr") ?? 0),
          Zaehler_km = GetDecimal(a, "zaehlerKm") ?? 0,
          Periode_km = GetDecimal(a, "periodeKm") ?? 0,
          Periode_Schnitt = GetDecimal(a, "periodeSchnitt") ?? 0,
          Beschreibung = GetString(a, "beschreibung"),
          Angelegt_Am = GetDateTime(a, "angelegtAm"), // 2020-03-27T16:39:20Z
          Angelegt_Von = GetString(a, "angelegtVon"),
          Geaendert_Am = GetDateTime(a, "geaendertAm"),
          Geaendert_Von = GetString(a, "geaendertVon"),
        };
        var alt = FzFahrradstandRep.Get(daten, daten.MandantNr, e.Fahrrad_Uid, e.Datum, e.Nr);
        var save = alt == null;
        if (!save && !(e.Zaehler_km == alt.Zaehler_km && e.Periode_km == alt.Periode_km
          && e.Periode_Schnitt == alt.Periode_Schnitt && Functions.CompString(e.Beschreibung, alt.Beschreibung) == 0))
        {
          var date = e.Angelegt_Am ?? DateTime.MinValue;
          if (e.Geaendert_Am.HasValue && e.Geaendert_Am.Value > date)
            date = e.Geaendert_Am.Value;
          var datealt = alt.Angelegt_Am ?? DateTime.MinValue;
          if (alt.Geaendert_Am.HasValue && alt.Geaendert_Am.Value > datealt)
            datealt = alt.Geaendert_Am.Value;
          //// Wenn datealt < date, Eintrag überschreiben, sonst Eintrag lassen
          if (datealt < date)
            save = true; // Overwrite
        }
        if (save)
        {
          var r1 = PrivateService.SaveMileage(daten, e.Fahrrad_Uid, e.Datum, e.Nr,
            e.Zaehler_km, e.Periode_km, e.Periode_Schnitt, e.Beschreibung,
            e.Angelegt_Von, e.Angelegt_Am, e.Geaendert_Von, e.Geaendert_Am);
          r1.ThrowAllErrors();
        }
      }
      SaveChanges(daten);
      var l = FzFahrradstandRep.GetList(daten, null, datege: daten.Heute.AddDays(-days));
      foreach (var e in l)
      {
        var j = new Dictionary<string, string>
        {
          { "fahrradUid", e.Fahrrad_Uid },
          { "datum", Functions.ToString(e.Datum) },
          { "nr", Functions.ToString(e.Nr, 0, Functions.CultureInfoEn) },
          { "zaehlerKm", Functions.ToString(e.Zaehler_km, 2, Functions.CultureInfoEn) },
          { "periodeKm", Functions.ToString(e.Periode_km, 2, Functions.CultureInfoEn) },
          { "periodeSchnitt", Functions.ToString(e.Periode_Schnitt, 2, Functions.CultureInfoEn) },
          { "beschreibung", e.Beschreibung },
          { "replid", "server" },
          { "angelegtAm", Functions.ToStringT(e.Angelegt_Am, true) },
          { "angelegtVon", e.Angelegt_Von },
          { "geaendertAm", Functions.ToStringT(e.Geaendert_Am, true) },
          { "geaendertVon", e.Geaendert_Von },
        };
        ja.Add(j);
      }
    }
    else
      r.Errors.Add(new Message("Falsche Tabelle {0}", true, table));
    r.Ergebnis = JsonSerializer.Serialize(ja, new JsonSerializerOptions { WriteIndented = true });
    return r;
  }

  /// <summary>
  /// Gets an table report as html document in bytes.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="name">Affected table name.</param>
  /// <param name="lines">Affected table data.</param>
  /// <returns>An table report as html document in bytes.</returns>
  public ServiceErgebnis<byte[]> GetTableReport(ServiceDaten daten, string name, List<List<string>> lines)
  {
    var r = new ServiceErgebnis<byte[]>();
    var ueberschrift = M1001(name, daten.Jetzt);
    var rp = new TableReport
    {
      Caption = ueberschrift,
      Lines = lines,
    };
    r.Ergebnis = rp.Generate();
    return r;
  }

  /// <summary>
  /// Commit a new file to the undo stack.
  /// </summary>
  /// <returns>Possibly errors.</returns>
  /// <param name="name">Affected file name with path.</param>
  public ServiceErgebnis CommitFile(string name)
  {
    var r = new ServiceErgebnis();
    if (string.IsNullOrWhiteSpace(name))
      throw new MessageException(M1012);
    var bytes = File.ReadAllBytes(name);
    var e = new FileData
    {
      Name = name,
      Bytes = bytes,
    };
    var ul = new UndoList();
    ul.Insert(e);
    Commit(ul);
    return r;
  }

  /// <summary>
  /// Gets a list of AI models.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <returns>List of AI models.</returns>
  public ServiceErgebnis<List<MaParameter>> GetAiModelList(ServiceDaten daten)
  {
    Functions.MachNichts(daten);
    var l = AiData.GetAiList.Select(x => new MaParameter { Schluessel = x.Item1, Wert = x.Item2 }).ToList();
    var r = new ServiceErgebnis<List<MaParameter>>(l);
    return r;
  }

  /// <summary>
  /// Gets response from ChatGPT.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="systemprompt">Affected input system prompt string.</param>
  /// <param name="prompt">Affected input prompt string.</param>
  /// <param name="model">Affected AI model.</param>
  /// <param name="maxtokens">Affected maximal number of tokens.</param>
  /// <param name="temperature">Affected temperature between 0 and 1.</param>
  /// <param name="dialog">Affected dialog to be continued.</param>
  /// <returns>AI data with response from ChatGPT.</returns>
  public ServiceErgebnis<AiData> AskChatGpt(ServiceDaten daten, string systemprompt, string prompt, string model = AiData.Gpt35, int maxtokens = 50, decimal temperature = 0.7M, AgDialog dialog = null)
  {
    var r = new ServiceErgebnis<AiData>();
    if (string.IsNullOrEmpty(prompt))
    {
      // throw new MessageException(AG004); // Debugger stürzt ab bei zu wenig Speicher (8GB), keine Fehlermeldung erscheint.
      r.Errors.Add(Message.New(AG004));
    }
    if (string.IsNullOrEmpty(model))
      model = AiData.Gpt35;
    if (maxtokens <= 16)
      maxtokens = 16;
    if (!r.Ok)
      return r;
    AiData aidata;
    if (dialog == null)
      aidata = new AiData();
    else
    {
      aidata = AiData.ParseRequestResponse(dialog.Frage, dialog.Antwort);
      aidata.ContinueDialog = true;
    }
    aidata.Model = model;
    aidata.MaxTokens = maxtokens;
    aidata.Temperature = temperature;
    aidata.SystemPrompt = systemprompt;
    aidata.Prompt = prompt;
    var d = RequestAiServer(daten, aidata);
    if (dialog == null)
    {
      d.Uid = Functions.GetUid();
      var dlist = AgDialogRep.GetList(daten, d.Api, null, null, d.Datum, true);
      d.Nr = (dlist?.FirstOrDefault()?.Nr ?? 0) + 1; // Starting with 1.
      AgDialogRep.Insert(daten, d);
    }
    else
    {
      d.Uid = dialog.Uid;
      d.Nr = dialog.Nr;
      AgDialogRep.Update(daten, d);
    }
    aidata.DialogUid = d.Uid;
    r.Ergebnis = aidata;
    return r;
  }

  /// <summary>
  /// Gets list with dialog entries.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="api">Affected api string.</param>
  /// <param name="uid">Affected uid.</param>
  /// <param name="search">Affected search string.</param>
  /// <returns>List with dialog entries.</returns>
  public ServiceErgebnis<List<AgDialog>> GetDialogList(ServiceDaten daten, string api = null, string uid = null, string search = null)
  {
    // if (string.IsNullOrEmpty(api))
    //  api = "OPENAI";
    var r = new ServiceErgebnis<List<AgDialog>>(AgDialogRep.GetList(daten, api, uid, search));
    foreach (var d in r.Ergebnis)
    {
      d.Data = AiData.ParseRequestResponse(d.Frage, d.Antwort);
    }
    return r;
  }

  /// <summary>
  /// Deletes an dialog.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Affected uid.</param>
  /// <returns>Possibly errors.</returns>
  public ServiceErgebnis DeleteDialog(ServiceDaten daten, string uid)
  {
    var e = AgDialogRep.Get(daten, daten.MandantNr, uid) ?? throw new MessageException(M1013);
    AgDialogRep.Delete(daten, e);
    return new ServiceErgebnis();
  }

  /// <summary>
  /// Continues a dialog.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="dialog">Affected dialog entry or null.</param>
  /// <param name="continues">True if dialog continues.</param>
  /// <param name="systemprompt">Affected input system prompt string.</param>
  /// <param name="prompt">Affected input prompt string.</param>
  /// <param name="response">Affected AI response.</param>
  /// <returns>AI data with response as dialog.</returns>
  public ServiceErgebnis<AiData> ContinueDialog(ServiceDaten daten, AgDialog dialog, bool continues, string systemprompt, string prompt, string response)
  {
    var r = new ServiceErgebnis<AiData>();
    var ai = new AiData();
    if (dialog == null || string.IsNullOrEmpty(response))
      return r;
    AiData.ParseRequestResponse(dialog.Frage, dialog.Antwort, ai);
    ai.ContinueDialog = ai.ContinueDialog || continues;
    ai.Prompt = ai.ContinueDialog ? null : ai.Prompt;
    r.Ergebnis = ai;
    return r;
  }

  /// <summary>Request to AI Server depending on model.</summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="data">Affected input data.</param>
  private static AgDialog RequestAiServer(ServiceDaten daten, AiData data)
  {
    var api = "LOCAL";
    string url;
    var timeout = 30000;
    object jcontent;
    switch (data.Model)
    {
      case AiData.Gpt4:
      case AiData.Gpt35:
      {
        url = @$"https://api.openai.com/v1/chat/completions";
        var mdic = new List<Dictionary<string, string>>();
        if (!string.IsNullOrEmpty(data.SystemPrompt))
          mdic.Add(new Dictionary<string, string> { { "role", "system" }, { "content", data.SystemPrompt } });
        if (data.AssistantPrompts.Any())
        {
          var i = 0;
          foreach (var ap in data.AssistantPrompts)
          {
            mdic.Add(new Dictionary<string, string> { { "role", i % 2 == 0 ? "user" : "assistant" }, { "content", ap } });
            i++;
          }
        }
        mdic.Add(new Dictionary<string, string> { { "role", "user" }, { "content", data.Prompt } });
        jcontent = new
        {
          model = data.Model,
          messages = mdic,
          temperature = data.Temperature,
          max_tokens = data.MaxTokens,
        };
        break;
      }
      case AiData.Gpt35instruct:
      {
        url = @$"https://api.openai.com/v1/completions";
        jcontent = new
        {
          model = data.Model,
          prompt = data.Prompt,
          temperature = data.Temperature,
          max_tokens = data.MaxTokens,
        };
        break;
      }
      case AiData.Dalle2:
      {
        url = @$"https://api.openai.com/v1/images/generations";
        jcontent = new
        {
          prompt = data.Prompt,
          n = 1,
          size = "256x256", // 512x512 1024x1024
          response_format = "url", // b64_json
          quality = "standard",
        };
        break;
      }
      case AiData.LocalLlama3:
      case AiData.LocalLlama3Max:
      case AiData.LocalStarcoder27B:
      {
        url = @$"http://localhost:11434/api/chat";
        timeout = 3000000;
        var mdic = new List<Dictionary<string, string>>
        {
          new() { { "role", "system" }, { "content", data.SystemPrompt } },
        };
        if (data.AssistantPrompts.Any())
        {
          var i = 0;
          foreach (var ap in data.AssistantPrompts)
          {
            mdic.Add(new Dictionary<string, string> { { "role", i % 2 == 0 ? "user" : "assistant" }, { "content", ap } });
            i++;
          }
        }
        mdic.Add(new Dictionary<string, string> { { "role", "user" }, { "content", data.Prompt } });
        jcontent = new
        {
          model = data.Model,
          messages = mdic,
          stream = false,
          options = new
          {
            temperature = data.Temperature,
          },
        };
        break;
      }
      case AiData.LocalLlava7B:
      {
        url = @$"http://localhost:11434/api/generate";
        timeout = 3000000;
        ////var images = new string[data.Images.Count];
        data.Images.Add("/home/wolfgang/Bilder/danova3.png");
        var images = data.Images.Select(i => Convert.ToBase64String(File.ReadAllBytes(i))).ToArray();
        jcontent = new
        {
          model = data.Model,
          prompt = data.Prompt,
          stream = false,
          images = images,
          options = new
          {
            temperature = data.Temperature,
          },
        };
        break;
      }
      default:
        throw new MessageException($"Model {data.Model} not supported.");
    }
    var httpsclient = HttpClientFactory.CreateClient(timeout: timeout);
    if (url.Contains("api.openai.com"))
    {
      api = "OPENAI";
      var openaikey = Parameter.GetValue(Parameter.AG_OPENAI_COM_ACCESS_KEY);
      httpsclient.DefaultRequestHeaders.Add("Authorization", $@"Bearer {openaikey}");
    }
    //// httpsclient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/1.0)");
    ////Debug.Print($"{content}");
    ////var json = System.Text.Json.JsonSerializer.Serialize(jcontent, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
    ////Debug.Print($"{json}");
    string s;
    if (Functions.MachNichts() == 0)
    {
      var task = httpsclient.PostAsJsonAsync(url, jcontent);
      task.Wait();
      var task2 = task.Result.Content.ReadAsStringAsync();
      task2.Wait();
      s = task2.Result;
    }
    else
      s = """{"model":"llama3_max","created_at":"2024-04-28T20:23:37.556685488Z","message":{"role":"assistant","content":"Das ist ein Test, okay! Ich bin bereit, um meine Fähigkeiten zu zeigen. Los geht's! Was ist das nächste Problem?"},"done":true,"total_duration":169306417245,"load_duration":24180638401,"prompt_eval_count":58,"prompt_eval_duration":16433220000,"eval_count":33,"eval_duration":128422227000}""";
    data = AiData.ParseRequestResponse(null, s, data);
    var json = JsonSerializer.Serialize(jcontent, new JsonSerializerOptions { WriteIndented = true });
    var d = new AgDialog
    {
      Mandant_Nr = daten.MandantNr,
      Api = api,
      Datum = daten.Heute,
      Nr = 0,
      Url = url,
      Frage = json,
      Antwort = s,
      Data = data,
    };
    System.Diagnostics.Debug.Print($"Question to {data.Model}: ({data.SystemPrompt}) {data.Prompt}");
    System.Diagnostics.Debug.Print($"{s}");
    var cc = data?.Messages?.FirstOrDefault();
    if (string.IsNullOrEmpty(cc))
      throw new Exception(s);
    System.Diagnostics.Debug.Print($"Answer: {cc}");
    return d;
  }

  /// <summary>
  /// Gets permission of a user.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="mandantNr">Affected client nummber.</param>
  /// <param name="benutzerId">Affected user id.</param>
  /// <returns>User permission.</returns>
  private static int GetBerechtigung(ServiceDaten daten, int mandantNr, string benutzerId)
  {
    var b = BenutzerRep.Get(daten, mandantNr, benutzerId);
    return b == null ? -1 : b.Berechtigung;
  }

  /// <summary>
  /// Gets a backup entry.
  /// </summary>
  /// <returns>Backup entry or null.</returns>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Affected ID.</param>
  private static BackupEntry GetBackupEntryIntern(ServiceDaten daten, string uid)
  {
    Functions.MachNichts(daten);
    BackupEntry e = null;
    var l = BackupEntry.GetBackupEntryList();
    if (!string.IsNullOrEmpty(uid) && l != null)
      e = l.FirstOrDefault(a => a.Uid == uid);
    return e;
  }

  /// <summary>
  /// Creates a random salt that will be used to encrypt your file. This method is required on FileEncrypt.
  /// </summary>
  /// <returns>Random salt.</returns>
  private static byte[] GenerateRandomSalt()
  {
    var data = new byte[32];
    using (var rng = RandomNumberGenerator.Create())
    {
      for (var i = 0; i < 10; i++)
      {
        // Fill the buffer with the generated data
        rng.GetBytes(data);
      }
    }
    return data;
  }

  /// <summary>
  /// Encrypts a file from its path and a plain password.
  /// </summary>
  /// <param name="inputFile">File to encrypt.</param>
  /// <param name="outputFile">Encrypted File.</param>
  /// <param name="password">Affected password.</param>
  private static void FileEncrypt(string inputFile, string outputFile, string password)
  {
    // http://stackoverflow.com/questions/27645527/aes-encryption-on-large-files
    // generate random salt
    var salt = GenerateRandomSalt();

    // create output file name
    File.Delete(outputFile);
    var fsCrypt = new FileStream(outputFile, FileMode.Create);

    // convert password string to byte arrray
    var passwordBytes = Encoding.UTF8.GetBytes(password);

    // Set Rijndael symmetric encryption algorithm
    var aes = Aes.Create();
    aes.KeySize = 256;
    aes.BlockSize = 128;
    aes.Padding = PaddingMode.PKCS7;

    // http://stackoverflow.com/questions/2659214/why-do-i-need-to-use-the-rfc2898derivebytes-class-in-net-instead-of-directly
    // "What it does is repeatedly hash the user password along with the salt." High iteration counts.
    var key = new Rfc2898DeriveBytes(passwordBytes, salt, 50000, HashAlgorithmName.SHA1);
    aes.Key = key.GetBytes(aes.KeySize / 8);
    aes.IV = key.GetBytes(aes.BlockSize / 8);

    // Cipher modes: http://security.stackexchange.com/questions/52665/which-is-the-best-cipher-mode-and-padding-mode-for-aes-encryption
    aes.Mode = CipherMode.CFB; // CBC

    // write salt to the begining of the output file, so in this case can be random every time
    fsCrypt.Write(salt, 0, salt.Length);

    var cs = new CryptoStream(fsCrypt, aes.CreateEncryptor(), CryptoStreamMode.Write);
    var fsIn = new FileStream(inputFile, FileMode.Open);

    // create a buffer (1mb) so only this amount will allocate in the memory and not the whole file
    var buffer = new byte[1048576]; // 1048576
    int read;

    try
    {
      while ((read = fsIn.Read(buffer, 0, buffer.Length)) > 0)
      {
        cs.Write(buffer, 0, read);
      }
      //// } catch (Exception ex) {
      ////    Console.WriteLine("Error: " + ex.Message);
    }
    finally
    {
      fsIn.Close();
      cs.Close();
      fsCrypt.Close();
    }
  }

  /// <summary>
  /// Decrypts an encrypted file with the FileEncrypt method through its path and the plain password.
  /// </summary>
  /// <param name="inputFile">Affected input file.</param>
  /// <param name="outputFile">Affected output file.</param>
  /// <param name="password">Affected password.</param>
  private static void FileDecrypt(string inputFile, string outputFile, string password)
  {
    var passwordBytes = Encoding.UTF8.GetBytes(password);
    var salt = new byte[32];
    var fsCrypt = new FileStream(inputFile, FileMode.Open);
    fsCrypt.Read(salt, 0, salt.Length);

    var aes = Aes.Create();
    aes.KeySize = 256;
    aes.BlockSize = 128;
    var key = new Rfc2898DeriveBytes(passwordBytes, salt, 50000, HashAlgorithmName.SHA1);
    aes.Key = key.GetBytes(aes.KeySize / 8);
    aes.IV = key.GetBytes(aes.BlockSize / 8);
    aes.Padding = PaddingMode.PKCS7;
    aes.Mode = CipherMode.CFB; // CBC

    var cs = new CryptoStream(fsCrypt, aes.CreateDecryptor(), CryptoStreamMode.Read);
    File.Delete(outputFile);
    var fsOut = new FileStream(outputFile, FileMode.Create);

    int read;
    var buffer = new byte[1048576]; // 1048576

    try
    {
      while ((read = cs.Read(buffer, 0, buffer.Length)) > 0)
      {
        fsOut.Write(buffer, 0, read);
      }

      // } catch (CryptographicException ex_CryptographicException) {
      //    Console.WriteLine("CryptographicException error: " + ex_CryptographicException.Message);
      // } catch (Exception ex) {
      //    Console.WriteLine("Error: " + ex.Message);
    }
    finally
    {
      try
      {
        cs.Close();
      }
      catch (Exception ex)
      {
        Functions.MachNichts(ex);
      }
      fsOut.Close();
      fsCrypt.Close();
    }
  }

  /// <summary>List of all replicable tables.</summary>
  private static List<ReplicationTable> GetAllTables()
  {
    var l = new List<ReplicationTable>
    {
      new ReplicationTable("AD_Adresse", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      new ReplicationTable("AD_Person", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      new ReplicationTable("AD_Sitz", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      new ReplicationTable("Benutzer", "Mandant_Nr", "Mandant_Nr, Benutzer_Id", true, true),
      new ReplicationTable("Byte_Daten", "Mandant_Nr", "Mandant_Nr, Typ, Uid, Lfd_Nr", true, true),
      new ReplicationTable("FZ_Buch", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      new ReplicationTable("FZ_Buchautor", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      new ReplicationTable("FZ_Buchserie", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      new ReplicationTable("FZ_Buchstatus", "Mandant_Nr", "Mandant_Nr, Buch_Uid", true, true),
      new ReplicationTable("FZ_Fahrrad", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      new ReplicationTable("FZ_Fahrradstand", "Mandant_Nr", "Mandant_Nr, Fahrrad_Uid, Datum, Nr", true, true),
      //// new ReplicationTable("FZ_Lektion", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      //// new ReplicationTable("FZ_Lektioninhalt", "Mandant_Nr", "Mandant_Nr, Lektion_Uid, Lfd_Nr", true, true),
      //// new ReplicationTable("FZ_Lektionstand", "Mandant_Nr", "Mandant_Nr, Lektion_Uid", true, true),
      new ReplicationTable("FZ_Notiz", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      new ReplicationTable("HH_Bilanz", "Mandant_Nr", "Mandant_Nr, Periode, Kz, Konto_Uid", true, true),
      new ReplicationTable("HH_Buchung", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      new ReplicationTable("HH_Ereignis", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      new ReplicationTable("HH_Konto", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      new ReplicationTable("HH_Periode", "Mandant_Nr", "Mandant_Nr, Nr", true, true),
      //// new ReplicationTable("HP_Behandlung", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      //// new ReplicationTable("HP_Behandlung_Leistung", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      //// new ReplicationTable("HP_Leistung", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      //// new ReplicationTable("HP_Leistungsgruppe", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      //// new ReplicationTable("HP_Patient", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      //// new ReplicationTable("HP_Rechnung", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      //// new ReplicationTable("HP_Status", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      //// new ReplicationTable("MA_Einstellung", "Mandant_Nr", "Mandant_Nr, Schluessel", false, false),
      new ReplicationTable("MA_Mandant", "Nr", "Nr", true, true),
      new ReplicationTable("MA_Parameter", "Mandant_Nr", "Mandant_Nr, Schluessel", true, true),
      //// new ReplicationTable("MA_Replikation", "Mandant_Nr", "Mandant_Nr, Tabellen_Nr, Replikation_Uid", true, false),
      //// new ReplicationTable("MO_Einteilung", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      //// new ReplicationTable("MO_Gottesdienst", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      //// new ReplicationTable("MO_Messdiener", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      //// new ReplicationTable("MO_Profil", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      new ReplicationTable("SB_Ereignis", "Mandant_Nr", "Mandant_Nr, Person_Uid, Familie_Uid, Typ", true, true),
      new ReplicationTable("SB_Familie", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      new ReplicationTable("SB_Kind", "Mandant_Nr", "Mandant_Nr, Familie_Uid, Kind_Uid", true, true),
      new ReplicationTable("SB_Person", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      new ReplicationTable("SB_Quelle", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      new ReplicationTable("TB_Eintrag", "Mandant_Nr", "Mandant_Nr, Datum", true, true),
      //// new ReplicationTable("VM_Abrechnung", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      //// new ReplicationTable("VM_Buchung", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      //// new ReplicationTable("VM_Ereignis", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      //// new ReplicationTable("VM_Haus", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      //// new ReplicationTable("VM_Konto", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      //// new ReplicationTable("VM_Miete", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      //// new ReplicationTable("VM_Mieter", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      //// new ReplicationTable("VM_Wohnung", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      new ReplicationTable("WP_Anlage", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      new ReplicationTable("WP_Buchung", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      new ReplicationTable("WP_Konfiguration", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      new ReplicationTable("WP_Stand", "Mandant_Nr", "Mandant_Nr, Wertpapier_Uid, Datum", true, true),
      new ReplicationTable("WP_Wertpapier", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      //// new ReplicationTable("zEinstellung", null, "Schluessel", false, false)
    };
    return l;
  }

  /// <summary>
  /// Prepares a backup.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="source">Source folder.</param>
  /// <param name="target">Target folder.</param>
  /// <param name="restore">Reverse direction of copying or not.</param>
  /// <param name="encrypted">Should the target folder be encrypted or not.</param>
  /// <param name="zipped">Should the target folder be compressed or not.</param>
  /// <param name="blist">Backup list to fill.</param>
  /// <param name="state">State of backup is always updated.</param>
  /// <param name="cancel">Cancel backup if not empty.</param>
  private static void PrepareBackup(ServiceDaten daten, string source, string target,
      bool restore, bool encrypted, bool zipped, List<BackupFile> blist,
      StringBuilder state, StringBuilder cancel)
  {
    Functions.MachNichts(daten);
    Functions.MachNichts(state);
    Functions.MachNichts(cancel);
    var tname = Path.GetFileName(Path.GetDirectoryName(source));
    var p = zipped ? Path.Combine(target, tname + ".zip")
        : Path.GetFullPath(Path.Combine(target, tname) + Path.DirectorySeparatorChar);
    if (zipped)
    {
      var paes = encrypted ? p + ".aes" : p;
      if (restore)
      {
        if (!File.Exists(paes))
          throw new MessageException(M1034(paes));
        Directory.CreateDirectory(source);
      }
      else if (!Directory.Exists(source))
        throw new MessageException(M1035(source));
      var tm = File.GetLastWriteTimeUtc(paes);
      var sm = Directory.GetLastWriteTimeUtc(source);
      if (restore || tm != sm)
        blist.Add(new BackupFile
        {
          Type = BackupType.ZipFolder,
          Path = source,
          Path2 = p,
          Modified = restore ? tm : sm,
        });
      return;
    }
    var s = restore ? p : source;
    var t = restore ? source : p;
    if (!Directory.Exists(s))
      throw new MessageException(M1035(s));
    Directory.CreateDirectory(t);
    blist.Add(new BackupFile
    {
      Type = BackupType.ModifyFolder,
      Path = t,
      Modified = Directory.GetLastWriteTimeUtc(s),
    });
    var sfiles = new Dictionary<string, BackupPreparation>();
    var sfolders = new Dictionary<string, BackupPreparation>();
    var tfiles = new Dictionary<string, BackupPreparation>();
    var tfolders = new Dictionary<string, BackupPreparation>();
    FillFileListRecursiv(s, s, sfiles, sfolders);
    FillFileListRecursiv(t, t, tfiles, tfolders);

    // Compare files
    var cfiles = new Dictionary<string, BackupPreparation>(); // Files to copy
    foreach (var f in sfiles.Keys)
    {
      var bf = sfiles[f];
      if (tfiles.ContainsKey(f))
      {
        // File in target
        if (bf.Modified != tfiles[f].Modified)
          cfiles.Add(f, bf); // File differs: copy
        tfiles.Remove(f);
      }
      else
      {
        cfiles.Add(f, bf); // File misses: copy
      }
    }

    // Compare folders
    var cfolders = new List<BackupPreparation>(); // Duplicate folders
    foreach (var f in sfolders.Values)
    {
      if (tfolders.ContainsKey(f.Name))
        cfolders.Add(f);
    }
    foreach (var f in cfolders)
    {
      sfolders.Remove(f.Name);
      var tf = tfolders[f.Name];
      blist.Add(new BackupFile
      {
        Type = BackupType.ModifyFolder,
        Path = tf.Path,
        Modified = f.Modified,
      });
      tfolders.Remove(f.Name);
    }

    // Create folders
    foreach (var f in sfolders.Values)
    {
      blist.Add(new BackupFile
      {
        Type = BackupType.CreateFolder,
        Path = Path.Combine(t, f.Name),
        Modified = f.Modified,
      });
    }

    // Deletes folders
    foreach (var f in tfolders.Values)
    {
      blist.Add(new BackupFile
      {
        Type = BackupType.DeleteFolder,
        Path = Path.Combine(t, f.Name),
        Modified = f.Modified,
      });
    }

    // Copies files
    foreach (var f in cfiles.Values)
    {
      blist.Add(new BackupFile
      {
        Type = BackupType.CopyFile,
        Path = Path.Combine(s, f.Name),
        Path2 = Path.Combine(t, f.Name),
        Modified = f.Modified,
      });
    }

    // Deletes files
    foreach (var f in tfiles.Values)
    {
      blist.Add(new BackupFile
      {
        Type = BackupType.DeleteFile,
        Path = Path.Combine(t, f.Name),
        Modified = f.Modified,
      });
    }
  }

  /// <summary>
  /// Fill the file and folder lists recursiv.
  /// </summary>
  /// <param name="folder">Affected Folder.</param>
  /// <param name="source">Source folder.</param>
  /// <param name="files">File list to fill.</param>
  /// <param name="folders">Folder list to fill.</param>
  private static void FillFileListRecursiv(string folder, string source, Dictionary<string, BackupPreparation> files,
      Dictionary<string, BackupPreparation> folders)
  {
    var dir = Functions.CutStart(folder, source);
    if (!string.IsNullOrEmpty(dir))
      folders.Add(dir, new BackupPreparation
      {
        Path = folder,
        Name = dir,
        Modified = Directory.GetLastWriteTimeUtc(folder),
      });
    foreach (var f in Directory.GetFiles(folder))
    {
      var bf = new BackupPreparation
      {
        Path = f,
        Name = Functions.CutStart(f, source),
        Modified = File.GetLastWriteTimeUtc(f),
      };
      files.Add(bf.Name, bf);
    }
    foreach (var f in Directory.GetDirectories(folder))
    {
      FillFileListRecursiv(f, source, files, folders);
    }
  }

  /// <summary>Regular expression for parsing parameter with number of days.</summary>
  [GeneratedRegex("^([a-z]+)(_([0-9]+)d?)?$", RegexOptions.Compiled)]
  private static partial Regex PreadRegex();
}
