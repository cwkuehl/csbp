// <copyright file="ClientService.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services;

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using CSBP.Apis.Models.Extension;
using CSBP.Apis.Enums;
using CSBP.Apis.Models;
using CSBP.Apis.Services;
using CSBP.Base;
using CSBP.Resources;
using CSBP.Services.Base;
using static CSBP.Resources.Messages;
using static CSBP.Resources.M;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using CSBP.Services.Repositories.Base;
using CSBP.Services.Reports;
using CSBP.Services.Undo;

public class ClientService : ServiceBase, IClientService
{
  /// <summary>Holt oder setzt den Haushalt-Service.</summary>
  public IBudgetService BudgetService { private get; set; }

  /// <summary>Holt oder setzt den Privat-Service.</summary>
  public IPrivateService PrivateService { private get; set; }

  /// <summary>
  /// Initializes the database.
  /// </summary>
  /// <returns>Possibly errors.</returns>
  /// <param name="daten">Service data for database access.</param>
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
        dba.addTab0();
        dba.addTab1("Mandant_Nr", "D_INTEGER", false);
        dba.addTab1("Uid", "D_REPL_ID", false);
        dba.addTab1("Bezeichnung", "D_STRING_50", false);
        dba.addTab1("Kuerzel", "D_STRING_20", false); // vorher 10
        dba.addTab1("Parameter", "D_MEMO", true);
        dba.addTab1("Datenquelle", "D_STRING_35", false);
        dba.addTab1("Status", "D_STRING_10", false);
        dba.addTab1("Relation_Uid", "D_REPL_ID", true);
        dba.addTab1("Notiz", "D_MEMO", true);
        dba.addTab1("Angelegt_Von", "D_STRING_20", true);
        dba.addTab1("Angelegt_Am", "D_DATETIME", true);
        dba.addTab1("Geaendert_Von", "D_STRING_20", true);
        dba.addTab1("Geaendert_Am", "D_DATETIME", true);
        dba.addTab2(mout, tab, "Mandant_Nr, Uid", "Mandant_Nr, Uid");
        MaMandantRep.Execute(daten, mout);
        version = 54;
      }
      else if (version <= 54)
      {
        var mout = new List<string>();
        mout.Add("update sb_person set quelle_uid=null where quelle_uid='0'");
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
        dba.createTab0();
        dba.createTab1("Mandant_Nr", "D_INTEGER", false);
        dba.createTab1("Uid", "D_REPL_ID", false);
        dba.createTab1("Bezeichnung", "D_STRING_50", true);
        dba.createTab1("Breite", "D_GELDBETRAG", false);
        dba.createTab1("Laenge", "D_GELDBETRAG", false);
        dba.createTab1("Hoehe", "D_GELDBETRAG", false);
        dba.createTab1("Notiz", "D_MEMO", true);
        dba.createTab1("Angelegt_Von", "D_STRING_20", true);
        dba.createTab1("Angelegt_Am", "D_DATETIME", true);
        dba.createTab1("Geaendert_Von", "D_STRING_20", true);
        dba.createTab1("Geaendert_Am", "D_DATETIME", true);
        dba.createTab2(mout, tab, "Mandant_Nr, Uid");
        tab = "TB_Eintrag_Ort";
        dba.createTab0();
        dba.createTab1("Mandant_Nr", "D_INTEGER", false);
        dba.createTab1("Ort_Uid", "D_REPL_ID", false);
        dba.createTab1("Datum_Von", "D_DATE", false);
        dba.createTab1("Datum_Bis", "D_DATE", false);
        dba.createTab1("Angelegt_Von", "D_STRING_20", true);
        dba.createTab1("Angelegt_Am", "D_DATETIME", true);
        dba.createTab1("Geaendert_Von", "D_STRING_20", true);
        dba.createTab1("Geaendert_Am", "D_DATETIME", true);
        dba.createTab2(mout, tab, "Mandant_Nr, Ort_Uid, Datum_Von, Datum_Bis");
        // mout.Add("INSERT INTO TB_Ort(Mandant_Nr,Uid,Breite,Laenge,Hoehe,Notiz,Angelegt_Von,Angelegt_Am) VALUES(1,'2',3,4,5,'6','ich','2020-08-15');");
        // dba.addTab0();
        // dba.addTab1("Mandant_Nr", "D_INTEGER", false);
        // dba.addTab1("Uid", "D_REPL_ID", false);
        // dba.addTab1("Bezeichnung", "D_STRING_50", true);
        // dba.addTab1("Breite", "D_GELDBETRAG", false);
        // dba.addTab1("Laenge", "D_GELDBETRAG", false);
        // dba.addTab1("Hoehe", "D_GELDBETRAG", false);
        // dba.addTab1("Notiz", "D_MEMO", true);
        // dba.addTab1a("Nix", "D_GELDBETRAG", false, "27");
        // dba.addTab1("Angelegt_Von", "D_STRING_20", true);
        // dba.addTab1("Angelegt_Am", "D_DATETIME", true);
        // dba.addTab1("Geaendert_Von", "D_STRING_20", true);
        // dba.addTab1("Geaendert_Am", "D_DATETIME", true);
        // dba.addTab2(mout, tab, "Mandant_Nr, Uid", "Mandant_Nr, Uid");
        mout.Add($"DELETE FROM MA_PARAMETER WHERE MANDANT_NR=0 AND NOT SCHLUESSEL IN ('{Constants.EINST_DATENBANK}','{Constants.EINST_DB_INIT}','{Constants.EINST_DB_VERSION}');");
        MaMandantRep.Execute(daten, mout);
        version = 56;
      }
      else if (version <= 56)
      {
        var dba = new DbAlter(DatabaseTypeEnum.SqLite);
        var mout = new List<string>();
        var tab = "FZ_Buch";
        dba.addTab0();
        dba.addTab1("Mandant_Nr", "D_INTEGER", false);
        dba.addTab1("Uid", "D_REPL_ID", false);
        dba.addTab1("Autor_Uid", "D_REPL_ID", false);
        dba.addTab1("Serie_Uid", "D_REPL_ID", false);
        dba.addTab1("Seriennummer", "D_INTEGER", false);
        dba.addTab1("Titel", "D_STRING_255", false); // vorher 100
        dba.addTab1a("Untertitel", "D_STRING_255", true, "NULL");
        dba.addTab1("Seiten", "D_INTEGER", false);
        dba.addTab1("Sprache_Nr", "D_INTEGER", false);
        dba.addTab1a("Notiz", "D_MEMO", true, "NULL");
        dba.addTab1("Angelegt_Von", "D_STRING_20", true);
        dba.addTab1("Angelegt_Am", "D_DATETIME", true);
        dba.addTab1("Geaendert_Von", "D_STRING_20", true);
        dba.addTab1("Geaendert_Am", "D_DATETIME", true);
        dba.addTab2(mout, tab, "Mandant_Nr, Uid", "Mandant_Nr, Uid");
        tab = "FZ_Buchautor";
        dba.addTab0();
        dba.addTab1("Mandant_Nr", "D_INTEGER", false);
        dba.addTab1("Uid", "D_REPL_ID", false);
        dba.addTab1("Name", "D_STRING_255", false); // vorher 50
        dba.addTab1("Vorname", "D_STRING_255", true); // vorher 50
        dba.addTab1a("Notiz", "D_MEMO", true, "NULL");
        dba.addTab1("Angelegt_Von", "D_STRING_20", true);
        dba.addTab1("Angelegt_Am", "D_DATETIME", true);
        dba.addTab1("Geaendert_Von", "D_STRING_20", true);
        dba.addTab1("Geaendert_Am", "D_DATETIME", true);
        dba.addTab2(mout, tab, "Mandant_Nr, Uid", "Mandant_Nr, Uid");
        tab = "FZ_Buchserie";
        dba.addTab0();
        dba.addTab1("Mandant_Nr", "D_INTEGER", false);
        dba.addTab1("Uid", "D_REPL_ID", false);
        dba.addTab1("Name", "D_STRING_255", false);
        dba.addTab1a("Notiz", "D_MEMO", true, "NULL");
        dba.addTab1("Angelegt_Von", "D_STRING_20", true);
        dba.addTab1("Angelegt_Am", "D_DATETIME", true);
        dba.addTab1("Geaendert_Von", "D_STRING_20", true);
        dba.addTab1("Geaendert_Am", "D_DATETIME", true);
        dba.addTab2(mout, tab, "Mandant_Nr, Uid", "Mandant_Nr, Uid");
        MaMandantRep.Execute(daten, mout);
        version = 57;
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
  /// Gets list with clients.
  /// </summary>
  /// <returns>List with clients.</returns>
  /// <param name="daten">Service data for database access.</param>
  public ServiceErgebnis<List<MaMandant>> GetClientList(ServiceDaten daten)
  {
    var l = MaMandantRep.GetList(daten);
    var user = BenutzerRep.GetList(daten, -1, null).FirstOrDefault();
    var per = user == null ? (int)PermissionEnum.Without : user.Berechtigung;
    if (per <= (int)PermissionEnum.Admin)
      l = l.Where(a => a.Nr == daten.MandantNr).ToList();
    return new ServiceErgebnis<List<MaMandant>>(l);
  }

  /// <summary>
  /// Gets client by number.
  /// </summary>
  /// <returns>The client.</returns>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="nr">Affected client number.</param>
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
      nr = last == null ? 1 : (last.Nr + 1);
      if (BenutzerRep.Get(daten, nr, Constants.USER_ID) == null)
      {
        var b = new Benutzer
        {
          Mandant_Nr = nr,
          Benutzer_ID = Constants.USER_ID,
          Passwort = null,
          Berechtigung = (int)PermissionEnum.Admin,
          Akt_Periode = 0
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
    var liste = BenutzerRep.GetList(daten, 0, id);
    var r = new ServiceErgebnis<List<Benutzer>>(liste);
    return r;
  }

  /** Berechtigung eines Benutzers lesen */
  private int GetBerechtigung(ServiceDaten daten, int mandantNr, string benutzerId)
  {
    var b = BenutzerRep.Get(daten, mandantNr, benutzerId);
    return b == null ? -1 : b.Berechtigung;
  }

  /// <summary>
  /// Gets user by number.
  /// </summary>
  /// <returns>The user.</returns>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="nr">Affected user number.</param>
  public ServiceErgebnis<Benutzer> GetUser(ServiceDaten daten, int nr)
  {
    var e = BenutzerRep.GetList(daten, nr, null).FirstOrDefault();
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
    var liste = BenutzerRep.GetList(daten, 0, id, enr);
    if (liste.Count > 0)
    {
      throw new MessageException(AM011);
    }
    if (enr <= 0)
    {
      enr = 1;
      liste = BenutzerRep.GetList(daten, 0, null);
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
      throw new ArgumentException();
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
      //mp.Wert = mp.Wert ?? "";
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
  /// <param name="target">Affected target</param>
  /// <param name="sources">Affected sources</param>
  /// <param name="encrypted">Should the target copy be encrypted?</param>
  /// <param name="zipped">Should the target copy be zipped?</param>
  public ServiceErgebnis<BackupEntry> SaveBackupEntry(ServiceDaten daten, string uid,
      string target, string[] sources, bool encrypted, bool zipped)
  {
    var r = new ServiceErgebnis<BackupEntry>();
    if (string.IsNullOrEmpty(target))
      r.Errors.Add(Message.New(Messages.M2023));
    else if (Directory.Exists(target))
      target = Path.GetFullPath(target + Path.DirectorySeparatorChar);
    else
      r.Errors.Add(Message.New(M.M1038(target)));
    if (sources == null || sources.Length <= 0 || string.IsNullOrEmpty(sources[0]))
      r.Errors.Add(Message.New(Messages.M2024));
    else
    {
      for (var i = 0; i < sources.Length; i++)
        if (Directory.Exists(sources[i]))
          sources[i] = Path.GetFullPath(sources[i] + Path.DirectorySeparatorChar);
        else
          r.Errors.Add(Message.New(M.M1038(sources[i])));
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
  /// <param name="restore">Reverse direction of copying?</param>
  /// <param name="password">Password for encryption.</param>
  /// <param name="status">Status of backup is always updated.</param>
  /// <param name="cancel">Cancel backup if not empty.</param>
  public ServiceErgebnis MakeBackup(ServiceDaten daten, string uid, bool restore,
      string password, StringBuilder status, StringBuilder cancel)
  {
    if (status == null || cancel == null)
      throw new ArgumentException();
    var e = GetBackupEntryIntern(daten, uid);
    if (e == null)
      throw new MessageException(M1013);
    status.Clear().Append(M0(M1031));
    var blist = new List<BackupFile>();
    foreach (var source in e.Sources)
    {
      PrepareBackup(daten, source, e.Target, restore, e.Encrypted, e.Zipped, blist, status, cancel);
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
          status.Clear().Append($"({i}/{l}) Verzeichnis {b.Path} anlegen.");
          Directory.CreateDirectory(b.Path);
          break;
        case BackupType.DeleteFolder:
          status.Clear().Append($"({i}/{l}) Verzeichnis {b.Path} löschen.");
          Directory.Delete(b.Path);
          break;
        case BackupType.CopyFile:
          status.Clear().Append($"({i}/{l}) Datei {b.Path} kopieren.");
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
          status.Clear().Append($"({i}/{l}) Datei {b.Path} löschen.");
          File.Delete(b.Path);
          break;
        case BackupType.ZipFolder:
          status.Clear().Append($"({i}/{l}) Verzeichnis {b.Path} packen.");
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
    status.Clear().Append($"Ende nach dem Abgleich von {blist.Count} Verzeichnissen/Dateien.");
    return new ServiceErgebnis();
  }

  private static Regex pread = new Regex("^([a-z]+)(_([0-9]+)d?)?$", RegexOptions.Compiled);

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
    var m = pread.Match(mode);
    var days = Math.Max(1, m.Success ? Functions.ToInt32(m.Groups[3].Value) : 1);
    var ja = new JArray();
    if (string.IsNullOrEmpty(mode))
      r.Errors.Add(new Message("Modus fehlt.", true));
    else if (table == "TB_Eintrag")
    {
      var jo = (JObject)JToken.Parse(json ?? "");
      var jarr = (JArray)jo[table];
      foreach (var a in jarr)
      {
        var e = new TbEintrag
        {
          Mandant_Nr = daten.MandantNr,
          Datum = (DateTime)a["datum"], // 2020-03-27
          Eintrag = (string)a["eintrag"],
          Angelegt_Am = Functions.ToDateTimeLocal((DateTime?)a["angelegtAm"]), // 2020-03-27T16:39:20Z
          Angelegt_Von = (string)a["angelegtVon"],
          Geaendert_Am = Functions.ToDateTimeLocal((DateTime?)a["geaendertAm"]),
          Geaendert_Von = (string)a["geaendertVon"],
        };
        var es = TbEintragRep.Get(daten, daten.MandantNr, e.Datum);
        if (es == null)
          TbEintragRep.Save(daten, daten.MandantNr, e.Datum, e.Eintrag,
            e.Angelegt_Von, e.Angelegt_Am, e.Geaendert_Von, e.Geaendert_Am);
        else if (es.Eintrag != e.Eintrag)
        {
          // Wenn es.angelegtAm != e.angelegtAm, Einträge zusammenkopieren
          // Wenn es.angelegtAm == e.angelegtAm und (e.geaendertAm == null oder es.geaendertAm > e.geaendertAm), Eintrag lassen
          // Wenn es.angelegtAm == e.angelegtAm und es.geaendertAm <= e.geaendertAm, Eintrag überschreiben
          if (e.Angelegt_Am.HasValue && (!es.Angelegt_Am.HasValue || es.Angelegt_Am != e.Angelegt_Am))
          {
            // Zusammenkopieren
            es.Eintrag = @$"Server: {es.Eintrag}
Lokal: {e.Eintrag}";
            es.Angelegt_Am = e.Angelegt_Am;
            es.Angelegt_Von = e.Angelegt_Von;
            es.Geaendert_Am = e.Geaendert_Am;
            es.Geaendert_Von = e.Geaendert_Von;
            TbEintragRep.Save(daten, daten.MandantNr, es.Datum, es.Eintrag,
              es.Angelegt_Von, es.Angelegt_Am, es.Geaendert_Von, es.Geaendert_Am);
          }
          else if (es.Angelegt_Am.HasValue && e.Angelegt_Am.HasValue && es.Angelegt_Am == e.Angelegt_Am
            && es.Geaendert_Am.HasValue && (!e.Geaendert_Am.HasValue || es.Geaendert_Am > e.Geaendert_Am))
          {
            // Lassen
          }
          else
          {
            // Überschreiben
            es.Eintrag = e.Eintrag;
            es.Angelegt_Am = e.Angelegt_Am;
            es.Angelegt_Von = e.Angelegt_Von;
            es.Geaendert_Am = e.Geaendert_Am;
            es.Geaendert_Von = e.Geaendert_Von;
            TbEintragRep.Save(daten, daten.MandantNr, es.Datum, es.Eintrag,
              es.Angelegt_Von, es.Angelegt_Am, es.Geaendert_Von, es.Geaendert_Am);
          }
        }
      }
      var l = TbEintragRep.GetList(daten, daten.MandantNr, daten.Heute, days);
      foreach (var e in l)
      {
        var j = new JObject(
          new JProperty("datum", Functions.ToString(e.Datum)),
          new JProperty("eintrag", e.Eintrag),
          new JProperty("replid", "server"),
          new JProperty("angelegtAm", e.Angelegt_Am),
          new JProperty("angelegtVon", e.Angelegt_Von),
          new JProperty("geaendertAm", e.Geaendert_Am),
          new JProperty("geaendertVon", e.Geaendert_Von)
        );
        ja.Add(j);
      }
    }
    else if (table == "FZ_Notiz")
    {
      var l = FzNotizRep.GetList(daten, daten.MandantNr);
      foreach (var e in l)
      {
        var j = new JObject(
          new JProperty("uid", e.Uid),
          new JProperty("thema", e.Thema),
          new JProperty("notiz", e.Notiz),
          new JProperty("replid", "server"),
          new JProperty("angelegtAm", e.Angelegt_Am),
          new JProperty("angelegtVon", e.Angelegt_Von),
          new JProperty("geaendertAm", e.Geaendert_Am),
          new JProperty("geaendertVon", e.Geaendert_Von)
        );
        ja.Add(j);
      }
    }
    else if (table == "HH_Buchung")
    {
      var jo = (JObject)JToken.Parse(json ?? "");
      var jarr = (JArray)jo[table];
      var today = DateTime.Today;
      foreach (var a in jarr)
      {
        var e = new HhBuchung
        {
          Mandant_Nr = daten.MandantNr,
          Uid = (string)a["uid"],
          Soll_Valuta = Functions.ToDateTimeLocal((DateTime)a["sollValuta"]) ?? today,
          Haben_Valuta = Functions.ToDateTimeLocal((DateTime)a["habenValuta"]) ?? today,
          Kz = (string)a["kz"],
          Betrag = (decimal)a["betrag"],
          EBetrag = (decimal)a["ebetrag"],
          Soll_Konto_Uid = (string)a["sollKontoUid"],
          Haben_Konto_Uid = (string)a["habenKontoUid"],
          BText = (string)a["btext"],
          Beleg_Nr = (string)a["belegNr"],
          Beleg_Datum = Functions.ToDateTimeLocal((DateTime)a["belegDatum"]) ?? today,
          Angelegt_Am = Functions.ToDateTimeLocal((DateTime?)a["angelegtAm"]), // 2020-03-27T16:39:20Z
          Angelegt_Von = (string)a["angelegtVon"],
          Geaendert_Am = Functions.ToDateTimeLocal((DateTime?)a["geaendertAm"]),
          Geaendert_Von = (string)a["geaendertVon"],
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
          // Wenn datealt < date, Eintrag überschreiben, sonst Eintrag lassen
          if (datealt < date)
            save = true; // Überschreiben
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
        var j = new JObject(
          new JProperty("uid", e.Uid),
          new JProperty("sollValuta", Functions.ToString(e.Soll_Valuta)),
          new JProperty("habenValuta", Functions.ToString(e.Haben_Valuta)),
          new JProperty("kz", e.Kz),
          new JProperty("betrag", Functions.ToString(e.Betrag, 2, Functions.CultureInfoEn)),
          new JProperty("ebetrag", Functions.ToString(e.EBetrag, 2, Functions.CultureInfoEn)),
          new JProperty("sollKontoUid", e.Soll_Konto_Uid),
          new JProperty("habenKontoUid", e.Haben_Konto_Uid),
          new JProperty("btext", e.BText),
          new JProperty("belegNr", e.Beleg_Nr),
          new JProperty("belegDatum", Functions.ToString(e.Beleg_Datum)),
          new JProperty("replid", "server"),
          new JProperty("angelegtAm", e.Angelegt_Am),
          new JProperty("angelegtVon", e.Angelegt_Von),
          new JProperty("geaendertAm", e.Geaendert_Am),
          new JProperty("geaendertVon", e.Geaendert_Von)
        );
        ja.Add(j);
      }
    }
    else if (table == "HH_Ereignis")
    {
      var l = HhEreignisRep.GetList(daten, daten.MandantNr);
      foreach (var e in l)
      {
        var j = new JObject(
          new JProperty("uid", e.Uid),
          new JProperty("kz", e.Kz),
          new JProperty("sollKontoUid", e.Soll_Konto_Uid),
          new JProperty("habenKontoUid", e.Haben_Konto_Uid),
          new JProperty("bezeichnung", e.Bezeichnung),
          new JProperty("etext", e.EText),
          new JProperty("replid", "server"),
          new JProperty("angelegtAm", e.Angelegt_Am),
          new JProperty("angelegtVon", e.Angelegt_Von),
          new JProperty("geaendertAm", e.Geaendert_Am),
          new JProperty("geaendertVon", e.Geaendert_Von)
        );
        ja.Add(j);
      }
    }
    else if (table == "HH_Konto")
    {
      var l = HhKontoRep.GetList(daten, -1, -1, dle: daten.Heute.AddDays(-days));
      foreach (var e in l)
      {
        var j = new JObject(
          new JProperty("uid", e.Uid),
          new JProperty("sortierung", e.Sortierung),
          new JProperty("art", e.Art),
          new JProperty("kz", e.Kz),
          new JProperty("name", e.Name),
          new JProperty("gueltigVon", Functions.ToString(e.Gueltig_Von)),
          new JProperty("gueltigBis", Functions.ToString(e.Gueltig_Bis)),
          new JProperty("periodeVon", Functions.ToString(e.Periode_Von)),
          new JProperty("periodeBis", Functions.ToString(e.Periode_Bis)),
          new JProperty("betrag", Functions.ToString(e.Betrag, 2, Functions.CultureInfoEn)),
          new JProperty("ebetrag", Functions.ToString(e.EBetrag, 2, Functions.CultureInfoEn)),
          new JProperty("replid", "server"),
          new JProperty("angelegtAm", e.Angelegt_Am),
          new JProperty("angelegtVon", e.Angelegt_Von),
          new JProperty("geaendertAm", e.Geaendert_Am),
          new JProperty("geaendertVon", e.Geaendert_Von)
        );
        ja.Add(j);
      }
    }
    else if (table == "FZ_Fahrrad")
    {
      var l = FzFahrradRep.GetList(daten, daten.MandantNr);
      foreach (var e in l)
      {
        var j = new JObject(
          new JProperty("uid", e.Uid),
          new JProperty("bezeichnung", e.Bezeichnung),
          new JProperty("typ", e.Typ),
          new JProperty("replid", "server"),
          new JProperty("angelegtAm", e.Angelegt_Am),
          new JProperty("angelegtVon", e.Angelegt_Von),
          new JProperty("geaendertAm", e.Geaendert_Am),
          new JProperty("geaendertVon", e.Geaendert_Von)
        );
        ja.Add(j);
      }
    }
    else if (table == "FZ_Fahrradstand")
    {
      var jo = (JObject)JToken.Parse(json ?? "");
      var jarr = (JArray)jo[table];
      var today = DateTime.Today;
      foreach (var a in jarr)
      {
        var e = new FzFahrradstand
        {
          Mandant_Nr = daten.MandantNr,
          Fahrrad_Uid = (string)a["fahrradUid"],
          Datum = (DateTime)a["datum"], // 2020-03-27
          Nr = (int)a["nr"],
          Zaehler_km = (decimal)a["zaehlerKm"],
          Periode_km = (decimal)a["periodeKm"],
          Periode_Schnitt = (decimal)a["periodeSchnitt"],
          Beschreibung = (string)a["beschreibung"],
          Angelegt_Am = Functions.ToDateTimeLocal((DateTime?)a["angelegtAm"]), // 2020-03-27T16:39:20Z
          Angelegt_Von = (string)a["angelegtVon"],
          Geaendert_Am = Functions.ToDateTimeLocal((DateTime?)a["geaendertAm"]),
          Geaendert_Von = (string)a["geaendertVon"],
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
          // Wenn datealt < date, Eintrag überschreiben, sonst Eintrag lassen
          if (datealt < date)
            save = true; // Überschreiben
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
        var j = new JObject(
          new JProperty("fahrradUid", e.Fahrrad_Uid),
          new JProperty("datum", Functions.ToString(e.Datum)),
          new JProperty("nr", Functions.ToString(e.Nr, 0, Functions.CultureInfoEn)),
          new JProperty("zaehlerKm", Functions.ToString(e.Zaehler_km, 2, Functions.CultureInfoEn)),
          new JProperty("periodeKm", Functions.ToString(e.Periode_km, 2, Functions.CultureInfoEn)),
          new JProperty("periodeSchnitt", Functions.ToString(e.Periode_Schnitt, 2, Functions.CultureInfoEn)),
          new JProperty("beschreibung", e.Beschreibung),
          new JProperty("replid", "server"),
          new JProperty("angelegtAm", e.Angelegt_Am),
          new JProperty("angelegtVon", e.Angelegt_Von),
          new JProperty("geaendertAm", e.Geaendert_Am),
          new JProperty("geaendertVon", e.Geaendert_Von)
        );
        ja.Add(j);
      }
    }
    else
      r.Errors.Add(new Message("Falsche Tabelle {0}", true, table));
    r.Ergebnis = ja.ToString();
    return r;
  }

  /// <summary>
  /// Prepares a backup.
  /// </summary>
  /// <returns>Possibly errors.</returns>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="source">Source folder.</param>
  /// <param name="target">Target folder.</param>
  /// <param name="restore">Reverse direction of copying?</param>
  /// <param name="encrypted">Should the target folder be encrypted?</param>
  /// <param name="zipped">Should the target folder be compressed?</param>
  /// <param name="blist">Backup list to fill.</param>
  /// <param name="status">Status of backup is always updated.</param>
  /// <param name="cancel">Cancel backup if not empty.</param>
  void PrepareBackup(ServiceDaten daten, string source, string target,
      bool restore, bool encrypted, bool zipped, List<BackupFile> blist,
      StringBuilder status, StringBuilder cancel)
  {
    var tname = Path.GetFileName(Path.GetDirectoryName(source));
    var p = zipped ? Path.Combine(target, tname + ".zip")
        : Path.GetFullPath(Path.Combine(target, tname) + Path.DirectorySeparatorChar);
    if (zipped)
    {
      var paes = encrypted ? (p + ".aes") : p;
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
          Modified = restore ? tm : sm
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
      Modified = Directory.GetLastWriteTimeUtc(s)
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
        Modified = f.Modified
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
        Modified = f.Modified
      });
    }

    // Delete folders
    foreach (var f in tfolders.Values)
    {
      blist.Add(new BackupFile
      {
        Type = BackupType.DeleteFolder,
        Path = Path.Combine(t, f.Name),
        Modified = f.Modified
      });
    }

    // Copy files
    foreach (var f in cfiles.Values)
    {
      blist.Add(new BackupFile
      {
        Type = BackupType.CopyFile,
        Path = Path.Combine(s, f.Name),
        Path2 = Path.Combine(t, f.Name),
        Modified = f.Modified
      });
    }

    // Delete files
    foreach (var f in tfiles.Values)
    {
      blist.Add(new BackupFile
      {
        Type = BackupType.DeleteFile,
        Path = Path.Combine(t, f.Name),
        Modified = f.Modified
      });
    }
  }

  /// <summary>
  /// Get an table report.
  /// </summary>
  /// <returns>An table report.</returns>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="name">Affected table name.</param>
  /// <param name="lines">Affected table data.</param>
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
      Bytes = bytes
    };
    var ul = new UndoList();
    ul.Insert(e);
    Commit(ul);
    return r;
  }

  /// <summary>
  /// Fill the file and folder lists recursiv.
  /// </summary>
  /// <param name="folder">Affected Folder.</param>
  /// <param name="source">Source folder.</param>
  /// <param name="files">File list to fill.</param>
  /// <param name="folders">Folder list to fill.</param>
  void FillFileListRecursiv(string folder, string source, Dictionary<string, BackupPreparation> files,
      Dictionary<string, BackupPreparation> folders)
  {
    var dir = Functions.CutStart(folder, source);
    if (!string.IsNullOrEmpty(dir))
      folders.Add(dir, new BackupPreparation
      {
        Path = folder,
        Name = dir,
        Modified = Directory.GetLastWriteTimeUtc(folder)
      });
    foreach (var f in Directory.GetFiles(folder))
    {
      var bf = new BackupPreparation
      {
        Path = f,
        Name = Functions.CutStart(f, source),
        Modified = File.GetLastWriteTimeUtc(f)
      };
      files.Add(bf.Name, bf);
    }
    foreach (var f in Directory.GetDirectories(folder))
    {
      FillFileListRecursiv(f, source, files, folders);
    }
  }

  /// <summary>
  /// Gets a backup entry.
  /// </summary>
  /// <returns>Backup entry or null.</returns>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Affected ID.</param>
  private BackupEntry GetBackupEntryIntern(ServiceDaten daten, string uid)
  {
    BackupEntry e = null;
    var l = BackupEntry.GetBackupEntryList();
    if (!string.IsNullOrEmpty(uid) && l != null)
      e = l.FirstOrDefault(a => a.Uid == uid);
    return e;
  }

  /// <summary>
  /// Creates a random salt that will be used to encrypt your file. This method is required on FileEncrypt.
  /// </summary>
  /// <returns></returns>
  public static byte[] GenerateRandomSalt()
  {
    var data = new byte[32];
    using (var rng = RandomNumberGenerator.Create())
    {
      for (int i = 0; i < 10; i++)
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
  /// <param name="password"></param>
  public static void FileEncrypt(string inputFile, string outputFile, string password)
  {
    //http://stackoverflow.com/questions/27645527/aes-encryption-on-large-files

    //generate random salt
    var salt = GenerateRandomSalt();

    //create output file name
    File.Delete(outputFile);
    var fsCrypt = new FileStream(outputFile, FileMode.Create);

    //convert password string to byte arrray
    var passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);

    //Set Rijndael symmetric encryption algorithm
    var AES = Aes.Create();
    AES.KeySize = 256;
    AES.BlockSize = 128;
    AES.Padding = PaddingMode.PKCS7;

    //http://stackoverflow.com/questions/2659214/why-do-i-need-to-use-the-rfc2898derivebytes-class-in-net-instead-of-directly
    //"What it does is repeatedly hash the user password along with the salt." High iteration counts.
    var key = new Rfc2898DeriveBytes(passwordBytes, salt, 50000);
    AES.Key = key.GetBytes(AES.KeySize / 8);
    AES.IV = key.GetBytes(AES.BlockSize / 8);

    //Cipher modes: http://security.stackexchange.com/questions/52665/which-is-the-best-cipher-mode-and-padding-mode-for-aes-encryption
    AES.Mode = CipherMode.CFB; // CBC

    // write salt to the begining of the output file, so in this case can be random every time
    fsCrypt.Write(salt, 0, salt.Length);

    var cs = new CryptoStream(fsCrypt, AES.CreateEncryptor(), CryptoStreamMode.Write);
    var fsIn = new FileStream(inputFile, FileMode.Open);

    //create a buffer (1mb) so only this amount will allocate in the memory and not the whole file
    var buffer = new byte[1048576]; // 1048576
    int read;

    try
    {
      while ((read = fsIn.Read(buffer, 0, buffer.Length)) > 0)
      {
        cs.Write(buffer, 0, read);
      }
      //} catch (Exception ex) {
      //    Console.WriteLine("Error: " + ex.Message);
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
  /// <param name="inputFile"></param>
  /// <param name="outputFile"></param>
  /// <param name="password"></param>
  public static void FileDecrypt(string inputFile, string outputFile, string password)
  {
    var passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
    var salt = new byte[32];
    var fsCrypt = new FileStream(inputFile, FileMode.Open);
    fsCrypt.Read(salt, 0, salt.Length);

    var AES = Aes.Create();
    AES.KeySize = 256;
    AES.BlockSize = 128;
    var key = new Rfc2898DeriveBytes(passwordBytes, salt, 50000);
    AES.Key = key.GetBytes(AES.KeySize / 8);
    AES.IV = key.GetBytes(AES.BlockSize / 8);
    AES.Padding = PaddingMode.PKCS7;
    AES.Mode = CipherMode.CFB; // CBC

    var cs = new CryptoStream(fsCrypt, AES.CreateDecryptor(), CryptoStreamMode.Read);
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
      //} catch (CryptographicException ex_CryptographicException) {
      //    Console.WriteLine("CryptographicException error: " + ex_CryptographicException.Message);
      //} catch (Exception ex) {
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

  List<ReplicationTable> GetAllTables()
  {
    var l = new List<ReplicationTable> {
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
      //new ReplicationTable("FZ_Lektion", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      //new ReplicationTable("FZ_Lektioninhalt", "Mandant_Nr", "Mandant_Nr, Lektion_Uid, Lfd_Nr", true, true),
      //new ReplicationTable("FZ_Lektionstand", "Mandant_Nr", "Mandant_Nr, Lektion_Uid", true, true),
      new ReplicationTable("FZ_Notiz", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      new ReplicationTable("HH_Bilanz", "Mandant_Nr", "Mandant_Nr, Periode, Kz, Konto_Uid", true, true),
      new ReplicationTable("HH_Buchung", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      new ReplicationTable("HH_Ereignis", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      new ReplicationTable("HH_Konto", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      new ReplicationTable("HH_Periode", "Mandant_Nr", "Mandant_Nr, Nr", true, true),
      //new ReplicationTable("HP_Behandlung", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      //new ReplicationTable("HP_Behandlung_Leistung", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      //new ReplicationTable("HP_Leistung", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      //new ReplicationTable("HP_Leistungsgruppe", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      //new ReplicationTable("HP_Patient", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      //new ReplicationTable("HP_Rechnung", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      //new ReplicationTable("HP_Status", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      //new ReplicationTable("MA_Einstellung", "Mandant_Nr", "Mandant_Nr, Schluessel", false, false),
      new ReplicationTable("MA_Mandant", "Nr", "Nr", true, true), //
      new ReplicationTable("MA_Parameter", "Mandant_Nr", "Mandant_Nr, Schluessel", true, true),
      //new ReplicationTable("MA_Replikation", "Mandant_Nr", "Mandant_Nr, Tabellen_Nr, Replikation_Uid", true, false),
      //new ReplicationTable("MO_Einteilung", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      //new ReplicationTable("MO_Gottesdienst", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      //new ReplicationTable("MO_Messdiener", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      //new ReplicationTable("MO_Profil", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      new ReplicationTable("SB_Ereignis", "Mandant_Nr", "Mandant_Nr, Person_Uid, Familie_Uid, Typ", true, true),
      new ReplicationTable("SB_Familie", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      new ReplicationTable("SB_Kind", "Mandant_Nr", "Mandant_Nr, Familie_Uid, Kind_Uid", true, true),
      new ReplicationTable("SB_Person", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      new ReplicationTable("SB_Quelle", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      new ReplicationTable("TB_Eintrag", "Mandant_Nr", "Mandant_Nr, Datum", true, true),
      //new ReplicationTable("VM_Abrechnung", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      //new ReplicationTable("VM_Buchung", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      //new ReplicationTable("VM_Ereignis", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      //new ReplicationTable("VM_Haus", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      //new ReplicationTable("VM_Konto", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      //new ReplicationTable("VM_Miete", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      //new ReplicationTable("VM_Mieter", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      //new ReplicationTable("VM_Wohnung", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      new ReplicationTable("WP_Anlage", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      new ReplicationTable("WP_Buchung", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      new ReplicationTable("WP_Konfiguration", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      new ReplicationTable("WP_Stand", "Mandant_Nr", "Mandant_Nr, Wertpapier_Uid, Datum", true, true),
      new ReplicationTable("WP_Wertpapier", "Mandant_Nr", "Mandant_Nr, Uid", true, true),
      //new ReplicationTable("zEinstellung", null, "Schluessel", false, false)
    };
    return l;
  }
}

class ReplicationTable
{
  public ReplicationTable(string name, string clientNumber, string primaryKey,
    bool delete, bool copy)
  {
    Name = name;
    ClientNumber = clientNumber;
    PrimaryKey = primaryKey;
    Delete = delete;
    Copy = copy;
  }

  public string Name { get; set; }
  public string ClientNumber { get; set; }
  public string PrimaryKey { get; set; }
  public bool Delete { get; set; }
  public bool Copy { get; set; }
}

class BackupPreparation
{
  public string Path { get; set; }
  public string Name { get; set; }
  public DateTime Modified { get; set; }
}

/// <summary>
/// Backup type for sorting.
/// </summary>
enum BackupType { CreateFolder, CopyFile, DeleteFile, DeleteFolder, ModifyFolder, ZipFolder };

class BackupFile
{
  public BackupType Type { get; set; }
  public string Path { get; set; }
  public string Path2 { get; set; }
  public DateTime Modified { get; set; }
}
