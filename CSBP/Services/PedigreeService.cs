// <copyright file="PedigreeService.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CSBP.Services.Apis.Enums;
using CSBP.Apis.Models;
using CSBP.Apis.Services;
using CSBP.Base;
using CSBP.Services.Base;
using CSBP.Services.Pedigree;
using CSBP.Services.Reports;
using static CSBP.Resources.M;
using static CSBP.Resources.Messages;

/// <summary>
/// Implementation of pedigree service.
/// </summary>
public class PedigreeService : ServiceBase, IPedigreeService
{
  /// <summary>Regex for Level.</summary>
  private static readonly Regex Level = new("^([\\d]+) (.*)$", RegexOptions.Compiled);

  /// <summary>Regex for 0 INDI.</summary>
  private static readonly Regex Indi0 = new("^0 @I([\\da-fA-F;\\-]+)@ INDI$", RegexOptions.Compiled); // 297f9764:141887cfc37:-8000-

  /// <summary>Regex for 1 NAME.</summary>
  private static readonly Regex Name1 = new("^1 NAME [^/]*/([^/]+)/$", RegexOptions.Compiled);

  /// <summary>Regex for 2 GIVN.</summary>
  private static readonly Regex Givn2 = new("^2 GIVN (.*)$", RegexOptions.Compiled);

  /// <summary>Regex for 2 SURN.</summary>
  private static readonly Regex Surn2 = new("^2 SURN (.*)$", RegexOptions.Compiled);

  /// <summary>Regex for 1 SEX.</summary>
  private static readonly Regex Sex1 = new("^1 SEX ([M|F])$", RegexOptions.Compiled);

  /// <summary>Regex for 1 BIRT.</summary>
  private static readonly Regex Birt1 = new("^1 BIRT$", RegexOptions.Compiled);

  /// <summary>Regex for 1 BURI.</summary>
  private static readonly Regex Buri1 = new("^1 BURI$", RegexOptions.Compiled);

  /// <summary>Regex for 1 CHR.</summary>
  private static readonly Regex Chr1 = new("^1 CHR$", RegexOptions.Compiled);

  /// <summary>Regex for 1 DEAT.</summary>
  private static readonly Regex Deat1 = new("^1 DEAT$", RegexOptions.Compiled);

  /// <summary>Regex for 2 DATE.</summary>
  private static readonly Regex Date2 = new("^2 DATE (.*)$", RegexOptions.Compiled);

  /// <summary>Regex for 2 PLAC.</summary>
  private static readonly Regex Plac2 = new("^2 PLAC (.*)$", RegexOptions.Compiled);

  /// <summary>Regex for 2 NOTE.</summary>
  private static readonly Regex Note2 = new("^2 NOTE (.*)$", RegexOptions.Compiled);

  /// <summary>Regex for 3 CONT.</summary>
  private static readonly Regex Cont3 = new("^3 CONT (.*)$", RegexOptions.Compiled);

  /// <summary>Regex for 1 RELI.</summary>
  private static readonly Regex Reli1 = new("^1 RELI (.*)$", RegexOptions.Compiled);

  /// <summary>Regex for 1 NOTE.</summary>
  private static readonly Regex Note1 = new("^1 NOTE (.*)$", RegexOptions.Compiled);

  /// <summary>Regex for 2 CONT.</summary>
  private static readonly Regex Cont2 = new("^2 CONT (.*)$", RegexOptions.Compiled);

  /// <summary>Regex for 1 SOUR.</summary>
  private static readonly Regex Sour1 = new("^1 SOUR @Q([\\da-fA-F;\\-]+)@$", RegexOptions.Compiled);

  /// <summary>Regex for 0 FAM.</summary>
  private static readonly Regex Fam0 = new("^0 @F([\\da-fA-F;\\-]+)@ FAM$", RegexOptions.Compiled);

  /// <summary>Regex for 1 HUSB.</summary>
  private static readonly Regex Husb1 = new("^1 HUSB @I([\\da-fA-F;\\-]+)@$", RegexOptions.Compiled);

  /// <summary>Regex for 1 WIFE.</summary>
  private static readonly Regex Wife1 = new("^1 WIFE @I([\\da-fA-F;\\-]+)@$", RegexOptions.Compiled);

  /// <summary>Regex for 1 MARR.</summary>
  private static readonly Regex Marr1 = new("^1 MARR$", RegexOptions.Compiled);

  /// <summary>Regex for 1 CHIL.</summary>
  private static readonly Regex Chil1 = new("^1 CHIL @I([\\da-fA-F;\\-]+)@$", RegexOptions.Compiled);

  /// <summary>Regex for 0 SOUR.</summary>
  private static readonly Regex Sour0 = new("^0 @Q([\\da-fA-F;\\-]+)@ SOUR$", RegexOptions.Compiled);

  /// <summary>Regex for 1 AUTH.</summary>
  private static readonly Regex Auth1 = new("^1 AUTH (.*)$", RegexOptions.Compiled);

  /// <summary>Regex for 1 TITL.</summary>
  private static readonly Regex Titl1 = new("^1 TITL (.*)$", RegexOptions.Compiled);

  /// <summary>Regex for 1 TEXT.</summary>
  private static readonly Regex Text1 = new("^1 TEXT (.*)$", RegexOptions.Compiled);

  /// <summary>
  /// Gets a list of ancestors.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="name">Affected last or birth name.</param>
  /// <param name="firstname">Affected first name.</param>
  /// <param name="uid">ID of ancestor.</param>
  /// <returns>List of ancestors.</returns>
  public ServiceErgebnis<List<SbPerson>> GetAncestorList(ServiceDaten daten, string name = null, string firstname = null, string uid = null)
  {
    var l = SbPersonRep.GetList(daten, name, firstname);
    return new ServiceErgebnis<List<SbPerson>>(l);
  }

  /// <summary>
  /// Gets an ancestor.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">ID of ancestor.</param>
  /// <returns>Ancestor or null.</returns>
  public ServiceErgebnis<SbPerson> GetAncestor(ServiceDaten daten, string uid)
  {
    var e = SbPersonRep.GetList(daten, uid: uid).FirstOrDefault();
    return new ServiceErgebnis<SbPerson>(e);
  }

  /// <summary>
  /// Gets a list of bytes.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">ID of ancestor.</param>
  /// <returns>list of bytes.</returns>
  public ServiceErgebnis<List<ByteDaten>> GetBytes(ServiceDaten daten, string uid)
  {
    var r = new ServiceErgebnis<List<ByteDaten>>(ByteDatenRep.GetList(daten, null, uid));
    return r;
  }

  /// <summary>
  /// Saves an ancestor.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">ID of ancestor.</param>
  /// <param name="name">Affected name.</param>
  /// <param name="vorname">Affected first names.</param>
  /// <param name="gebname">Affected birth name.</param>
  /// <param name="geschlecht">Affected gender.</param>
  /// <param name="titel">Affected title.</param>
  /// <param name="konfession">Affected confession.</param>
  /// <param name="bemerkung">Affected memo.</param>
  /// <param name="quid">Affected source id.</param>
  /// <param name="status1">Affected state 1.</param>
  /// <param name="status2">Affected state 2.</param>
  /// <param name="status3">Affected state 3.</param>
  /// <param name="geburtsdatum">Affected birth day.</param>
  /// <param name="geburtsort">Affected birth place.</param>
  /// <param name="geburtsbem">Affected birth memo.</param>
  /// <param name="geburtsQuelle">Affected birth source.</param>
  /// <param name="taufdatum">Affected baptist date.</param>
  /// <param name="taufort">Affected baptist place.</param>
  /// <param name="taufbem">Affected baptist memo.</param>
  /// <param name="taufQuelle">Affected baptist source.</param>
  /// <param name="todesdatum">Affected death date.</param>
  /// <param name="todesort">Affected deatch place.</param>
  /// <param name="todesbem">Affected death memo.</param>
  /// <param name="todesQuelle">Affected deatch source.</param>
  /// <param name="begraebnisdatum">Affected funeral date.</param>
  /// <param name="begraebnisort">Affected funeral place.</param>
  /// <param name="begraebnisbem">Affected funeral memo.</param>
  /// <param name="begraebnisQuelle">Affected funeral source.</param>
  /// <param name="gatteNeu">New spouce uid.</param>
  /// <param name="vaterUidNeu">New father uid.</param>
  /// <param name="mutterUidNeu">New mother uid.</param>
  /// <param name="byteliste">List of pictured and descriptions.</param>
  /// <returns>Created or changed ancestor.</returns>
  public ServiceErgebnis<SbPerson> SaveAncestor(ServiceDaten daten, string uid, string name, string vorname,
    string gebname, string geschlecht, string titel, string konfession, string bemerkung, string quid, int status1,
    int status2, int status3, string geburtsdatum, string geburtsort, string geburtsbem, string geburtsQuelle,
    string taufdatum, string taufort, string taufbem, string taufQuelle, string todesdatum, string todesort,
    string todesbem, string todesQuelle, string begraebnisdatum, string begraebnisort, string begraebnisbem,
    string begraebnisQuelle, string gatteNeu, string vaterUidNeu, string mutterUidNeu, List<ByteDaten> byteliste)
  {
    var r = new ServiceErgebnis<SbPerson>();
    if (string.IsNullOrEmpty(gebname))
      r.Errors.Add(Message.New(SB001));
    if (!string.IsNullOrEmpty(gatteNeu) && (!string.IsNullOrEmpty(vaterUidNeu) || !string.IsNullOrEmpty(mutterUidNeu)))
      r.Errors.Add(Message.New(SB002));
    //// Typ prüfen
    var gesch = N(geschlecht);
    var g = (GenderEnum)gesch;
    if (g == GenderEnum.MANN)
      g = GenderEnum.MAENNLICH;
    else if (g == GenderEnum.FRAU)
      g = GenderEnum.WEIBLICH;
    gesch = g.ToString();
    var name2 = N(name);
    if (string.IsNullOrEmpty(name2))
      name2 = gebname;
    if (!r.Ok)
      return r;
    var e = SbPersonRep.Save(daten, daten.MandantNr, uid, N(name2), N(vorname), N(gebname), N(gesch), N(titel),
      N(konfession), N(bemerkung), N(quid), status1, status2, status3);
    SaveEvent(daten, e.Uid, "", GedcomEventEnum.BIRTH.ToString(), geburtsdatum, geburtsort, geburtsbem, geburtsQuelle);
    SaveEvent(daten, e.Uid, "", GedcomEventEnum.CHRIST.ToString(), taufdatum, taufort, taufbem, taufQuelle);
    SaveEvent(daten, e.Uid, "", GedcomEventEnum.DEATH.ToString(), todesdatum, todesort, todesbem, todesQuelle);
    SaveEvent(daten, e.Uid, "", GedcomEventEnum.BURIAL.ToString(), begraebnisdatum, begraebnisort, begraebnisbem, begraebnisQuelle);
    SaveChanges(daten);
    if (byteliste != null)
      ByteDatenRep.SaveList(daten, "SB_Person", e.Uid, byteliste);
    if (!string.IsNullOrEmpty(vaterUidNeu) || !string.IsNullOrEmpty(mutterUidNeu))
      NeueFamilie(daten, null, vaterUidNeu, mutterUidNeu, e.Uid, false);
    if (!string.IsNullOrEmpty(gatteNeu))
    {
      var fuid = SbFamilieRep.GetList(daten, null, null, null, gatteNeu).FirstOrDefault()?.Uid;
      if (gesch == GenderEnum.MAENNLICH.ToString())
        NeueFamilie(daten, fuid, e.Uid, gatteNeu, null, true);
      else
        NeueFamilie(daten, fuid, gatteNeu, e.Uid, null, true);
    }
    return r;
  }

  /// <summary>
  /// Deletes an ancestor.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Affected entity.</param>
  /// <returns>Possibly errors.</returns>
  public ServiceErgebnis DeleteAncestor(ServiceDaten daten, SbPerson e)
  {
    DeletePersonIntern(daten, e);
    return new ServiceErgebnis();
  }

  /// <summary>
  /// Gets the next ancestor by name.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">ID of ancestor.</param>
  /// <param name="name">Affected name.</param>
  /// <param name="firstname">Affected first name.</param>
  /// <returns>Ancestor uid or null.</returns>
  public ServiceErgebnis<string> GetNextAncestorName(ServiceDaten daten, string uid, string name, string firstname)
  {
    var r = new ServiceErgebnis<string>();
    var p0 = string.IsNullOrEmpty(uid) ? null : SbPersonRep.Get(daten, daten.MandantNr, uid);
    var p = SbPersonRep.GetNext(daten, p0, name, firstname);
    if (p == null && p0 != null)
    {
      // 1. Treffer suchen.
      p = SbPersonRep.GetNext(daten, null, name, firstname);
    }
    r.Ergebnis = p?.Uid;
    return r;
  }

  /// <summary>
  /// Gets the first child of the ancestor.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">ID of ancestor.</param>
  /// <param name="fuid">Affected family uid.</param>
  /// <returns>Ancestor uid or null.</returns>
  public ServiceErgebnis<string> GetFirstChild(ServiceDaten daten, string uid, string fuid = null)
  {
    string kuid = null;
    if (string.IsNullOrEmpty(fuid))
    {
      var f = GetElternFamilieIntern(daten, uid);
      if (f != null)
        fuid = f.Uid;
    }
    if (!string.IsNullOrEmpty(fuid))
    {
      var kliste = SbKindRep.GetList(daten, fuid);
      kuid = kliste.FirstOrDefault()?.Kind_Uid;
    }
    var r = new ServiceErgebnis<string>(kuid);
    return r;
  }

  /// <summary>
  /// Gets the next spouse of the ancestor.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">ID of ancestor.</param>
  /// <returns>Ancestor uid or null.</returns>
  public ServiceErgebnis<SbPerson> GetNextSpouse(ServiceDaten daten, string uid)
  {
    var r = new ServiceErgebnis<SbPerson>(null);
    if (uid == null)
      return r;
    var liste = GetAlleEhegatten(daten, uid).OrderBy(a => a).ToList();
    var i = liste.IndexOf(uid);
    if (i >= 0)
    {
      i = (i + 1) % liste.Count;
      r.Ergebnis = SbPersonRep.GetList(daten, uid: liste.ElementAt(i)).FirstOrDefault();
    }
    return r;
  }

  /// <summary>
  /// Gets all spouses of the ancestor.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">ID of ancestor.</param>
  /// <returns>List of Ancestors.</returns>
  public ServiceErgebnis<List<SbPerson>> GetSpouseList(ServiceDaten daten, string uid)
  {
    var r = new ServiceErgebnis<List<SbPerson>>(new List<SbPerson>());
    var liste = GetAlleEhegatten(daten, uid).OrderBy(a => a).ToList();
    foreach (var a in liste)
    {
      if (a != uid)
      {
        var p = SbPersonRep.Get(daten, daten.MandantNr, a);
        if (p != null)
          r.Ergebnis.Add(p);
      }
    }
    return r;
  }

  /// <summary>
  /// Gets the next sibling of the ancestor.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">ID of ancestor.</param>
  /// <returns>Ancestor uid or null.</returns>
  public ServiceErgebnis<SbPerson> GetNextSibling(ServiceDaten daten, string uid)
  {
    var r = new ServiceErgebnis<SbPerson>();
    if (string.IsNullOrEmpty(uid))
      return r;
    var p = SbPersonRep.GetList(daten, uid: uid).FirstOrDefault();
    var euid = p == null ? null : p.Mother == null ? p.Father?.Uid : p.Mother?.Uid;
    if (string.IsNullOrEmpty(euid))
      return r;
    var liste = new List<string>();
    var eliste = GetAlleEhegatten(daten, euid).OrderBy(a => a).ToList();
    foreach (var e in eliste)
    {
      var kliste = SbKindRep.GetList(daten, personuid: e);
      foreach (var c in kliste)
      {
        if (!liste.Contains(c.Kind_Uid))
          liste.Add(c.Kind_Uid);
      }
    }
    var i = liste.IndexOf(uid);
    if (i >= 0)
    {
      i = (i + 1) % liste.Count;
      r.Ergebnis = SbPersonRep.GetList(daten, uid: liste.ElementAt(i)).FirstOrDefault();
    }
    return r;
  }

  /// <summary>
  /// Gets a list of families.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <returns>List of families.</returns>
  public ServiceErgebnis<List<SbFamilie>> GetFamilyList(ServiceDaten daten)
  {
    var r = new ServiceErgebnis<List<SbFamilie>>(SbFamilieRep.GetList(daten));
    return r;
  }

  /// <summary>
  /// Gets a family.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">ID of family.</param>
  /// <param name="puid">Affected father or mother uid.</param>
  /// <param name="cuid">Affected child uid.</param>
  /// <returns>Familiy or null.</returns>
  public ServiceErgebnis<SbFamilie> GetFamily(ServiceDaten daten, string uid, string puid = null, string cuid = null)
  {
    var r = new ServiceErgebnis<SbFamilie>();
    if (string.IsNullOrEmpty(cuid))
      r.Ergebnis = SbFamilieRep.GetList(daten, uid, personuid: puid).FirstOrDefault();
    else
    {
      var k = GetKindFamilieIntern(daten, cuid);
      if (k != null)
        r.Ergebnis = SbFamilieRep.GetList(daten, k.Familie_Uid).FirstOrDefault();
    }
    return r;
  }

  /// <summary>
  /// Gets a list of children.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">ID of family.</param>
  /// <returns>list of children.</returns>
  public ServiceErgebnis<List<SbPerson>> GetChildList(ServiceDaten daten, string uid)
  {
    var r = new ServiceErgebnis<List<SbPerson>>(SbPersonRep.GetList(daten, fuid: uid));
    return r;
  }

  /// <summary>
  /// Saves a family.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">ID of family.</param>
  /// <param name="fuid">Affected father uid.</param>
  /// <param name="muid">Affected mother uid.</param>
  /// <param name="mdate">Affected marriage date.</param>
  /// <param name="mplace">Affected marriage place.</param>
  /// <param name="mmemo">Affected marriage memo.</param>
  /// <param name="suid">Affected source id.</param>
  /// <param name="children">Affected list of children.</param>
  /// <returns>Created or changed family.</returns>
  public ServiceErgebnis<SbFamilie> SaveFamily(ServiceDaten daten, string uid, string fuid, string muid,
    string mdate, string mplace, string mmemo, string suid, List<string> children)
  {
    var e = NeueFamilie(daten, uid, fuid, muid, null, true);
    SaveEvent(daten, "", e.Uid, GedcomEventEnum.MARRIAGE.ToString(), mdate, mplace, mmemo, suid);
    //// bestehende Kinder lesen
    var listeAlt = SbKindRep.GetList(daten, e.Uid);
    foreach (var i in children)
    {
      var vo = listeAlt.FirstOrDefault(a => a.Kind_Uid == i);
      if (vo == null)
        SbKindRep.Save(daten, daten.MandantNr, e.Uid, i);
      else
        listeAlt.Remove(vo);
    }
    //// überflüssige Kinder löschen.
    foreach (var vo in listeAlt)
    {
      SbKindRep.Delete(daten, vo);
    }
    var r = new ServiceErgebnis<SbFamilie>(e);
    return r;
  }

  /// <summary>
  /// Deletes a family.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Affected entity.</param>
  /// <returns>Possibly errors.</returns>
  public ServiceErgebnis DeleteFamily(ServiceDaten daten, SbFamilie e)
  {
    DeleteFamilieIntern(daten, e);
    var r = new ServiceErgebnis();
    return r;
  }

  /// <summary>
  /// Gets a list of sources.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <returns>List of sources.</returns>
  public ServiceErgebnis<List<SbQuelle>> GetSourceList(ServiceDaten daten)
  {
    var r = new ServiceErgebnis<List<SbQuelle>>(SbQuelleRep.GetList(daten, daten.MandantNr));
    return r;
  }

  /// <summary>
  /// Gets a source.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">ID of source.</param>
  /// <returns>Source or null.</returns>
  public ServiceErgebnis<SbQuelle> GetSource(ServiceDaten daten, string uid)
  {
    var r = new ServiceErgebnis<SbQuelle>(SbQuelleRep.Get(daten, daten.MandantNr, uid));
    return r;
  }

  /// <summary>
  /// Saves a source.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">ID of source.</param>
  /// <param name="author">Affected author.</param>
  /// <param name="desc">Affected description.</param>
  /// <param name="quotation">Affected quotation.</param>
  /// <param name="memo">Affected memo.</param>
  /// <returns>Created or changed source.</returns>
  public ServiceErgebnis<SbQuelle> SaveSource(ServiceDaten daten, string uid, string author, string desc,
    string quotation, string memo)
  {
    var r = new ServiceErgebnis<SbQuelle>();
    author = Functions.Cut(N(author), 255);
    desc = Functions.Cut(N(desc), 255);
    if (string.IsNullOrEmpty(author))
      r.Errors.Add(Message.New(SB013));
    if (string.IsNullOrEmpty(desc))
      r.Errors.Add(Message.New(SB014));
    if (!r.Ok)
      return r;
    r.Ergebnis = SbQuelleRep.Save(daten, daten.MandantNr, uid, desc, N(quotation), N(memo), author, 0, 0, 0);
    return r;
  }

  /// <summary>
  /// Deletes a source.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Affected entity.</param>
  /// <returns>Possibly errors.</returns>
  public ServiceErgebnis DeleteSource(ServiceDaten daten, SbQuelle e)
  {
    if (e != null)
      DeleteQuelleIntern(daten, e);
    var r = new ServiceErgebnis();
    return r;
  }

  /// <summary>
  /// Gets ancestor report as pdf file in bytes.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Affected Ahnancestor uid.</param>
  /// <param name="generations">Number of generations.</param>
  /// <param name="siblings">With siblings or not.</param>
  /// <param name="descendants">List of descendants or not.</param>
  /// <param name="forebears">List of forebears or not.</param>
  /// <returns>Ancestor report as pdf file in bytes.</returns>
  public ServiceErgebnis<byte[]> GetAncestorReport(ServiceDaten daten, string uid, int generations, bool siblings, bool descendants, bool forebears)
  {
    // Nachfahren-Liste
    // 1 Vorname <b>Name</b> * tt.mm.jjjj Ort
    // + Vorname <b>Name</b> * tt.mm.jjjj Ort
    //   2 Vorname <b>Name</b> * tt.mm.jjjj Ort
    //   2 Vorname <b>Name</b> * tt.mm.jjjj Ort
    //   2 Vorname <b>Name</b> * tt.mm.jjjj Ort
    // Vorfahren-Liste
    // 1 Vorname <b>Name</b> * tt.mm.jjjj Ort
    // + Vorname <b>Name</b> * tt.mm.jjjj Ort
    // + Vorname <b>Name</b> * tt.mm.jjjj Ort
    //   2 Vorname <b>Name</b> * tt.mm.jjjj Ort
    //   2 Vorname <b>Name</b> * tt.mm.jjjj Ort
    var r = new ServiceErgebnis<byte[]>();
    var anzahl = Math.Max(1, generations);
    var p = SbPersonRep.Get(daten, daten.MandantNr, uid) ?? throw new MessageException(SB017(uid));
    var dlist = new List<SbPerson>();
    if (descendants)
      GetNachfahrenRekursiv(daten, uid, 0, 1, anzahl, dlist);
    var flist = new List<SbPerson>();
    if (forebears)
      GetVorfahrenRekursiv(daten, uid, true, siblings, true, 1, anzahl, flist);
    var ueberschrift = SB018(daten.Jetzt);
    var untertitel = SB019(p.AncestorName, anzahl);
    var untertitel2 = SB021(p.AncestorName, anzahl, siblings ? M0(SB022) : "");
    var rp = new AncestorReport
    {
      Caption = ueberschrift,
      Subtitle = untertitel,
      SubtitleForebears = untertitel2,
      Descendants = dlist,
      Forebears = flist,
    };
    r.Ergebnis = rp.Generate();
    return r;
  }

  /// <summary>
  /// Erstellen einer GEDCOM-Datei der Version 4.0 oder 5.5.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="skript">Name der Datei mit Pfad.</param>
  /// <param name="name">Name des Stammbaums in der Datei.</param>
  /// <param name="filter">Filter-Kriterium für Ahnen.</param>
  /// <param name="version">Version kann null sein, dann gilt Version "5.5".</param>
  /// <returns>Datei als String-Array.</returns>
  public ServiceErgebnis<List<string>> ExportAncestorList(ServiceDaten daten, string skript, string name, string filter, string version = null)
  {
    // Status2=1 heißt Export in Datei, sonst nicht.
    if (string.IsNullOrEmpty(version))
      version = "5.5";
    if (string.IsNullOrEmpty(name))
      throw new MessageException(SB023);
    if (string.IsNullOrEmpty(skript))
      throw new MessageException(M1012);
    if (!(version == "4.0" || version == "5.5"))
      throw new MessageException(SB024);
    var op = ""; // ">=";
    var tot = 0;
    var anc = "";
    var desc = "";
    if (string.IsNullOrEmpty(filter))
    {
      // Select all acestors and families.
      // SbPersonRep.UpdateStatus2(daten, null, 0, status2);
      // SaveChanges(daten);
      // SbFamilieRep.UpdateStatus2(daten, status2);
      // SaveChanges(daten);
      // SbQuelleRep.UpdateStatus2(daten, status2);
      // SaveChanges(daten);
    }
    else
    {
      var re = new Regex("^(tot|death|status1)\\s*(<|<=|=|>=|>)\\s*(\\d+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
      var m = re.Match(filter);
      if (m.Success)
      {
        op = m.Groups[2].Value;
        tot = Functions.ToInt32(m.Groups[3].Value);
        CalculateDeathYear(daten);
      }
      else
      {
        var re2 = new Regex("^(vorfahre|ancestor|status2)\\s*(=)\\s*([a-z\\d\\-:]+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        m = re2.Match(filter);
        if (m.Success)
        {
          anc = m.Groups[3].Value;
          CalculateAncestor(daten, anc);
        }
        else
        {
          var re3 = new Regex("^(nachfahre|descendant|status3)\\s*(=)\\s*([a-z\\d\\-:]+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
          m = re3.Match(filter);
          if (m.Success)
          {
            desc = m.Groups[3].Value;
            CalculateDescendant(daten, desc);
          }
        }
      }
      if (string.IsNullOrEmpty(op) && string.IsNullOrEmpty(anc) && string.IsNullOrEmpty(desc))
        throw new MessageException(SB025);

      // status2 = 0; // Unselect all ancestors.
      // SbPersonRep.UpdateStatus2(daten, null, 0, status2);
      // SaveChanges(daten);
      // var c = SbPersonRep.CountStatus2(daten, status2);
      // SbFamilieRep.UpdateStatus2(daten, status2);
      // SaveChanges(daten);
      // SbQuelleRep.UpdateStatus2(daten, status2);
      // SaveChanges(daten);
      // status2 = 1; // Select affected ancestors.
      // SbPersonRep.UpdateStatus2(daten, op, tot, status2);
      // SaveChanges(daten);
      // c = SbPersonRep.CountStatus2(daten, status2);
      // SbFamilieRep.UpdateParentStatus2(daten, status2);
      // SaveChanges(daten);
      // SbQuelleRep.UpdatePersonStatus2(daten, status2);
      // SaveChanges(daten);
    }
    var list = new List<string>();
    var map = new Dictionary<string, int>();
    WriteHead(daten, list, version, skript);
    WriteIndividual(daten, list, map, version, op, tot, anc, desc);
    WriteFamily(daten, list, map, op, tot, anc, desc);
    WriteSource(daten, list, map);
    WriteFoot(daten, list, version, name);

    // var l = map.Where(a => a.Value < 0).Select(a => a.Key).ToList();
    // foreach (var i in l)
    //   Debug.WriteLine(i);
    var r = new ServiceErgebnis<List<string>>(list);
    return r;
  }

  /// <summary>
  /// Imports list of ancestors from GEDCOM file.
  /// </summary>
  /// <returns>Message of import.</returns>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="lines">GEDCOM file as list of lines.</param>
  public ServiceErgebnis<string> ImportAncestorList(ServiceDaten daten, List<string> lines)
  {
    var pliste = SbPersonRep.GetList(daten, daten.MandantNr);
    foreach (var p in pliste)
    {
      DeletePersonIntern(daten, p);
    }
    SaveChanges(daten);
    var fliste = SbFamilieRep.GetList(daten, daten.MandantNr);
    foreach (var f in fliste)
    {
      DeleteFamilieIntern(daten, f);
    }
    SaveChanges(daten);
    var qliste = SbQuelleRep.GetList(daten, daten.MandantNr);
    foreach (var q in qliste)
    {
      DeleteQuelleIntern(daten, q);
    }
    SaveChanges(daten);
    var map = new Dictionary<string, string>();
    var anzahl = ImportAncestors(daten, lines, map);
    var r = new ServiceErgebnis<string>(SB026(anzahl));
    return r;
  }

  /// <summary>
  /// Calculate the death year of all ancestors (Status1).
  /// </summary>
  /// <returns>Message of import.</returns>
  /// <param name="daten">Service data for database access.</param>
  public ServiceErgebnis CalculateDeathYear(ServiceDaten daten)
  {
    const int alter = 80;
    const int generation = 30;
    var r = new ServiceErgebnis();
    SbPersonRep.UpdateStatus1(daten, 0);
    SaveChanges(daten);
    var anzahl1 = SbPersonRep.CountStatus1(daten, 0);
    Debug.WriteLine($"CalculateDeathYear for {anzahl1} ancestors.");
    var plist = SbPersonRep.GetList(daten, status1: 0);
    var pd = new PedigreeTimeData();
    foreach (var p in plist)
    {
      if (!string.IsNullOrEmpty(p.Deathdate))
      {
        pd.Parse(p.Deathdate);
        if (pd.Date1.Jahr != 0)
          p.Status1 = pd.Date1.Jahr;
        else if (pd.Date2.Jahr != 0)
          p.Status1 = pd.Date2.Jahr;
      }
      if (p.Status1 == 0 && !string.IsNullOrEmpty(p.Burialdate))
      {
        pd.Parse(p.Burialdate);
        if (pd.Date1.Jahr != 0)
          p.Status1 = pd.Date1.Jahr;
        else if (pd.Date2.Jahr != 0)
          p.Status1 = pd.Date2.Jahr;
      }
      if (p.Status1 == 0 && !string.IsNullOrEmpty(p.Birthdate))
      {
        pd.Parse(p.Birthdate);
        if (pd.Date1.Jahr != 0)
          p.Status1 = -(pd.Date1.Jahr + alter);
        else if (pd.Date2.Jahr != 0)
          p.Status1 = -(pd.Date2.Jahr + alter);
      }
      if (p.Status1 == 0 && !string.IsNullOrEmpty(p.Christdate))
      {
        pd.Parse(p.Christdate);
        if (pd.Date1.Jahr != 0)
          p.Status1 = -(pd.Date1.Jahr + alter);
        else if (pd.Date2.Jahr != 0)
          p.Status1 = -(pd.Date2.Jahr + alter);
      }
      //// if (p.Status1 != 0)
      ////  SbPersonRep.Update(daten, p);
    }
    SaveChanges(daten);
    var anzahl3 = SbPersonRep.CountStatus1(daten, 0);
    Debug.WriteLine($"{anzahl3} ancestors without deatch date.");
    int anzahl2;
    do
    {
      anzahl2 = anzahl3;
      plist = SbPersonRep.GetList(daten, status1: 0);
      foreach (var p in plist)
      {
        var flist = SbFamilieRep.GetList(daten, personuid: p.Uid);
        foreach (var f in flist)
        {
          if (p.Status1 != 0)
            continue;
          //// State of wife or spouse.
          if ((f.Father?.Status1 ?? 0) != 0)
            p.Status1 = -Math.Abs(f.Father.Status1);
          else if ((f.Mother?.Status1 ?? 0) != 0)
            p.Status1 = -Math.Abs(f.Mother.Status1);
          else
          {
            // State of child
            var clist = SbKindRep.GetList(daten, fuid: f.Uid);
            foreach (var c in clist)
            {
              if (p.Status1 != 0)
                continue;
              if ((c.Child?.Status1 ?? 0) != 0)
                p.Status1 = -(Math.Abs(c.Child.Status1) - generation);
            }
          }
        }
        if (p.Status1 != 0)
          continue;
        var c2list = SbKindRep.GetList(daten, kuid: p.Uid);
        foreach (var c2 in c2list)
        {
          if (p.Status1 != 0)
            continue;
          flist = SbFamilieRep.GetList(daten, uid: c2.Familie_Uid);
          foreach (var f in flist)
          {
            if (p.Status1 != 0)
              continue;
            //// State of father or mother.
            if ((f.Father?.Status1 ?? 0) != 0)
              p.Status1 = -(Math.Abs(f.Father.Status1) + generation);
            else if ((f.Mother?.Status1 ?? 0) != 0)
              p.Status1 = -(Math.Abs(f.Mother.Status1) + generation);
          }
        }
      }
      SaveChanges(daten);
      anzahl3 = SbPersonRep.CountStatus1(daten, 0);
      Debug.WriteLine($"{anzahl3} ancestors without deatch date.");
    }
    while (anzahl3 > 0 && anzahl3 != anzahl2);
    var f2list = SbFamilieRep.GetList(daten, uid: null);
    foreach (var f in f2list)
    {
      // Maximum of Status1 for family.
      var max = f.Father?.Status1 ?? 0;
      if (max != 0 && Math.Abs(max) < Math.Abs(f.Mother?.Status1 ?? 0))
        max = f.Mother.Status1;
      f.Status1 = max;
    }
    var slist = SbQuelleRep.GetList(daten, uid: null);
    foreach (var s in slist)
    {
      // Minimum of Status1 for source.
      var p2list = SbPersonRep.GetList(daten, suid: s.Uid);
      var min = int.MaxValue;
      foreach (var p in p2list)
      {
        if (Math.Abs(min) > Math.Abs(p.Status1))
          min = p.Status1;
      }
      if (min != int.MaxValue)
        s.Status1 = min;
    }
    return r;
  }

  /// <summary>
  /// Calculate the ancestors of a person (Status2).
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="anc">Affected number of root ancestor.</param>
  /// <returns>Possibly errors.</returns>
  private static ServiceErgebnis CalculateAncestor(ServiceDaten daten, string anc)
  {
    var r = new ServiceErgebnis();
    var plist = SbPersonRep.GetList(daten, daten.MandantNr);
    anc ??= "";
    Debug.WriteLine($"CalculateAncestor for {plist.Count} ancestors.");
    foreach (var p in plist)
      p.Status2 = (p.Uid == anc) ? 1 : 0;
    var flist = SbFamilieRep.GetList(daten, daten.MandantNr);
    foreach (var f in flist)
      f.Status2 = 0;
    SaveChanges(daten);
    var c0 = 0;
    do
    {
      plist = SbPersonRep.GetList(daten, status2: 1);
      var c1 = plist.Count;
      if (c0 == c1)
        break;
      Debug.WriteLine($"{c1} selected ancestors.");
      foreach (var p in plist)
      {
        // Debug.WriteLine($"{p.Geburtsname} {p.Name} {p.Vorname}.");
        flist = SbFamilieRep.GetList(daten, personuid: p.Uid, status2: 0);
        var clist = SbKindRep.GetList(daten, kuid: p.Uid);
        foreach (var c in clist)
        {
          flist.AddRange(SbFamilieRep.GetList(daten, c.Familie_Uid));
        }
        foreach (var f in flist)
        {
          if (anc.StartsWith(f.Father?.Uid ?? "", StringComparison.CurrentCultureIgnoreCase)
            || anc.StartsWith(f.Mother?.Uid ?? "", StringComparison.CurrentCultureIgnoreCase))
            continue;
          f.Status2 = 1;
          if ((f.Father?.Status2 ?? 0) == 0)
          {
            var fa = SbPersonRep.Get(daten, daten.MandantNr, f.Father?.Uid);
            if (fa != null)
              fa.Status2 = 1;
          }
          if ((f.Mother?.Status2 ?? 0) == 0)
          {
            var mo = SbPersonRep.Get(daten, daten.MandantNr, f.Mother?.Uid);
            if (mo != null)
              mo.Status2 = 1;
          }
        }
      }
      SaveChanges(daten);
      c0 = c1;
    }
    while (true);
    return r;
  }

  /// <summary>
  /// Calculate the descendants of a person (Status3).
  /// </summary>
  /// <returns>Possibly errors.</returns>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="desc">Affected number of root descendant.</param>
  private static ServiceErgebnis CalculateDescendant(ServiceDaten daten, string desc)
  {
    var r = new ServiceErgebnis();
    var plist = SbPersonRep.GetList(daten, daten.MandantNr);
    desc ??= "";
    Debug.WriteLine($"CalculateDescendant for {plist.Count} descendants.");
    foreach (var p in plist)
      p.Status3 = (p.Uid == desc) ? 1 : 0;
    var flist = SbFamilieRep.GetList(daten, daten.MandantNr);
    foreach (var f in flist)
      f.Status3 = 0;
    SaveChanges(daten);
    var c0 = 0;
    do
    {
      plist = SbPersonRep.GetList(daten, status3: 1);
      var c1 = plist.Count;
      if (c0 == c1)
        break;
      Debug.WriteLine($"{c1} selected descendants.");
      foreach (var p in plist)
      {
        // Debug.WriteLine($"{p.Geburtsname} {p.Name} {p.Vorname}.");
        flist = SbFamilieRep.GetList(daten, personuid: p.Uid, status3: 0);
        foreach (var f in flist)
        {
          f.Status3 = 1;
          if ((f.Father?.Status3 ?? 0) == 0)
          {
            var fa = SbPersonRep.Get(daten, daten.MandantNr, f.Father?.Uid);
            if (fa != null)
              fa.Status3 = 1;
          }
          if ((f.Mother?.Status3 ?? 0) == 0)
          {
            var mo = SbPersonRep.Get(daten, daten.MandantNr, f.Mother?.Uid);
            if (mo != null)
              mo.Status3 = 1;
          }
          var clist = SbKindRep.GetList(daten, fuid: f.Uid);
          foreach (var c in clist)
          {
            var ch = SbPersonRep.Get(daten, daten.MandantNr, c.Kind_Uid);
            if (ch != null)
              ch.Status3 = 1;
          }
        }
      }
      SaveChanges(daten);
      c0 = c1;
    }
    while (true);
    return r;
  }

  /// <summary>
  /// Imports Ancestors.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="datei">File as list of lines.</param>
  /// <param name="map">Mapping dictionary.</param>
  /// <returns>Number of imported individuals.</returns>
  private static int ImportAncestors(ServiceDaten daten, List<string> datei, Dictionary<string, string> map)
  {
    Match m;
    var v = new List<string>();
    var anzahl = 0;

    foreach (var s in datei)
    {
      var str = s ?? "";
      if (str.StartsWith("0"))
      {
        try
        {
          if (v.Any())
          {
            var str0 = v[0];
            var g = false;
            v.RemoveAt(0);
            m = Indi0.Match(str0);
            string uid;
            if (m.Success)
            {
              uid = GetUid(map, m.Groups[1].Value);
              ImportIndividual(daten, v, uid, map);
              anzahl++;
              g = true;
            }
            if (!g)
            {
              m = Fam0.Match(str0);
              if (m.Success)
              {
                uid = GetUid(map, m.Groups[1].Value);
                ImportFamily(daten, v, uid, map);
                g = true;
              }
            }
            if (!g)
            {
              m = Sour0.Match(str0);
              if (m.Success)
              {
                uid = GetUid(map, m.Groups[1].Value);
                //// ImportSource(daten, v, uid);
                g = true;
              }
            }
          }
        }
        finally
        {
          v.Clear();
          v.Add(str);
        }
      }
      else
        v.Add(str);
    }
    return anzahl;
  }

  /// <summary>
  /// Imports an individual.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="v">Affected lines.</param>
  /// <param name="uid">Affected ancestor uid.</param>
  /// <param name="map">Mapping dictionary.</param>
  private static void ImportIndividual(ServiceDaten daten, List<string> v, string uid, Dictionary<string, string> map)
  {
    var ve = new List<string>();
    var p = new SbPerson
    {
      Mandant_Nr = daten.MandantNr,
      Uid = uid,
      Geschlecht = GenderEnum.MAENNLICH.ToString(),
    };
    while (v.Any())
    {
      var str = v[0];
      v.RemoveAt(0);
      var g = false;
      var m = Name1.Match(str);
      if (m.Success)
      {
        p.Name = N(m.Groups[1].Value);
        g = true;
      }
      if (!g)
      {
        m = Givn2.Match(str);
        if (m.Success)
        {
          p.Vorname = N(m.Groups[1].Value);
          g = true;
        }
      }
      if (!g)
      {
        m = Surn2.Match(str);
        if (m.Success)
        {
          p.Geburtsname = N(m.Groups[1].Value);
          g = true;
        }
      }
      if (!g)
      {
        m = Sex1.Match(str);
        if (m.Success)
        {
          if (m.Groups[1].Value == "F")
            p.Geschlecht = GenderEnum.WEIBLICH.ToString();
          g = true;
        }
      }
      if (!g)
      {
        m = Birt1.Match(str);
        if (m.Success)
        {
          ve.Clear();
          while (GetLevel(v[0]) > 1)
          {
            ve.Add(v[0]);
            v.RemoveAt(0);
          }
          ImportEvent(daten, ve, uid, "INDI", GedcomEventEnum.BIRTH);
          g = true;
        }
      }
      if (!g)
      {
        m = Buri1.Match(str);
        if (m.Success)
        {
          ve.Clear();
          while (GetLevel(v[0]) > 1)
          {
            ve.Add(v[0]);
            v.RemoveAt(0);
          }
          ImportEvent(daten, ve, uid, "INDI", GedcomEventEnum.BURIAL);
          g = true;
        }
      }
      if (!g)
      {
        m = Chr1.Match(str);
        if (m.Success)
        {
          ve.Clear();
          while (GetLevel(v[0]) > 1)
          {
            ve.Add(v[0]);
            v.RemoveAt(0);
          }
          ImportEvent(daten, ve, uid, "INDI", GedcomEventEnum.CHRIST);
          g = true;
        }
      }
      if (!g)
      {
        m = Deat1.Match(str);
        if (m.Success)
        {
          ve.Clear();
          while (GetLevel(v[0]) > 1)
          {
            ve.Add(v[0]);
            v.RemoveAt(0);
          }
          ImportEvent(daten, ve, uid, "INDI", GedcomEventEnum.DEATH);
          g = true;
        }
      }
      if (!g)
      {
        m = Reli1.Match(str);
        if (m.Success)
        {
          p.Konfession = N(m.Groups[1].Value);
          g = true;
        }
      }
      if (!g)
      {
        m = Note1.Match(str);
        if (m.Success)
        {
          p.Bemerkung = N(m.Groups[1].Value);
          g = true;
        }
      }
      if (!g)
      {
        m = Cont2.Match(str);
        if (m.Success)
        {
          p.Bemerkung = Functions.Append(p.Bemerkung, null, N(m.Groups[1].Value));
          g = true;
        }
      }
      if (!g)
      {
        m = Sour1.Match(str);
        if (m.Success)
        {
          p.Quelle_Uid = GetUid(map, m.Groups[1].Value);
        }
      }
    }
    SbPersonRep.Insert(daten, p);
  }

  /// <summary>
  /// Imports a family.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="v">Affected lines.</param>
  /// <param name="uid">Affected family uid.</param>
  /// <param name="map">Mapping dictionary.</param>
  private static void ImportFamily(ServiceDaten daten, List<string> v, string uid, Dictionary<string, string> map)
  {
    var ve = new List<string>();
    var f = new SbFamilie
    {
      Mandant_Nr = daten.MandantNr,
      Uid = uid,
    };
    while (v.Any())
    {
      var str = v[0];
      v.RemoveAt(0);
      var g = false;
      var m = Husb1.Match(str);
      if (m.Success)
      {
        f.Mann_Uid = GetUid(map, m.Groups[1].Value);
        g = true;
      }
      if (!g)
      {
        m = Wife1.Match(str);
        if (m.Success)
        {
          f.Frau_Uid = GetUid(map, m.Groups[1].Value);
          g = true;
        }
      }
      if (!g)
      {
        m = Marr1.Match(str);
        if (m.Success)
        {
          ve.Clear();
          while (GetLevel(v[0]) > 1)
          {
            ve.Add(v[0]);
            v.RemoveAt(0);
          }
          ImportEvent(daten, ve, uid, "FAM", GedcomEventEnum.MARRIAGE);
          g = true;
        }
      }
      if (!g)
      {
        m = Chil1.Match(str);
        if (m.Success)
        {
          var k = new SbKind
          {
            Mandant_Nr = daten.MandantNr,
            Familie_Uid = uid,
            Kind_Uid = GetUid(map, m.Groups[1].Value),
          };
          SbKindRep.Insert(daten, k);
        }
      }
    }
    SbFamilieRep.Insert(daten, f);
  }

#pragma warning disable IDE0051

  /// <summary>
  /// Imports sources.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="v">List of lines.</param>
  /// <param name="uid">Affected source uid.</param>
  private static void ImportSource(ServiceDaten daten, List<string> v, string uid)
  {
    var zuletzt = 0;
    var q = new SbQuelle
    {
      Mandant_Nr = daten.MandantNr,
      Uid = uid,
    };
    while (v.Any())
    {
      var str = v[0];
      v.RemoveAt(0);
      var g = false;
      var m = Auth1.Match(str);
      if (m.Success)
      {
        q.Autor = N(m.Groups[1].Value);
        zuletzt = 1;
        g = true;
      }
      if (!g)
      {
        m = Titl1.Match(str);
        if (m.Success)
        {
          q.Beschreibung = N(m.Groups[1].Value);
          zuletzt = 2;
          g = true;
        }
      }
      if (!g)
      {
        m = Text1.Match(str);
        if (m.Success)
        {
          q.Zitat = N(m.Groups[1].Value);
          zuletzt = 3;
          g = true;
        }
      }
      if (!g)
      {
        m = Note1.Match(str);
        if (m.Success)
        {
          q.Bemerkung = N(m.Groups[1].Value);
          zuletzt = 4;
          g = true;
        }
      }
      if (!g)
      {
        m = Cont2.Match(str);
        if (m.Success)
        {
          if (zuletzt == 1)
            q.Autor = Functions.Append(q.Autor, null, N(m.Groups[1].Value));
          else if (zuletzt == 2)
            q.Beschreibung = Functions.Append(q.Beschreibung, null, N(m.Groups[1].Value));
          else if (zuletzt == 3)
            q.Zitat = Functions.Append(q.Zitat, null, N(m.Groups[1].Value));
          else if (zuletzt == 4)
            q.Bemerkung = Functions.Append(q.Bemerkung, null, N(m.Groups[1].Value));
        }
      }
    }
    SbQuelleRep.Insert(daten, q);
  }
#pragma warning restore IDE0051

  /// <summary>
  /// Imports event.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="v">Affected lines.</param>
  /// <param name="uid">Affected ancestor or family uid.</param>
  /// <param name="nrTyp">INDI or FAM.</param>
  /// <param name="typ">Affected event type.</param>
  private static void ImportEvent(ServiceDaten daten, List<string> v, string uid, string nrTyp, GedcomEventEnum typ)
  {
    var datum = new PedigreeTimeData();
    var e = new SbEreignis
    {
      Mandant_Nr = daten.MandantNr,
      Person_Uid = nrTyp == "INDI" ? uid : "",
      Familie_Uid = nrTyp == "INDI" ? "" : uid,
      Typ = typ.ToString(),
    };
    while (v.Any())
    {
      var str = v[0];
      v.RemoveAt(0);
      var g = false;
      var m = Date2.Match(str);
      if (m.Success)
      {
        datum.Parse(m.Groups[1].Value, true);
        e.Datum_Typ = datum.DateType;
        e.Tag1 = datum.Date1.Tag;
        e.Monat1 = datum.Date1.Monat;
        e.Jahr1 = datum.Date1.Jahr;
        e.Tag2 = datum.Date2.Tag;
        e.Monat2 = datum.Date2.Monat;
        e.Jahr2 = datum.Date2.Jahr;
        g = true;
      }
      if (!g)
      {
        m = Plac2.Match(str);
        if (m.Success)
        {
          e.Ort = N(m.Groups[1].Value);
          g = true;
        }
      }
      if (!g)
      {
        m = Note2.Match(str);
        if (m.Success)
        {
          e.Bemerkung = N(m.Groups[1].Value);
          g = true;
        }
      }
      if (!g)
      {
        m = Cont3.Match(str);
        if (m.Success)
        {
          e.Bemerkung = Functions.Append(e.Bemerkung, null, N(m.Groups[1].Value));
        }
      }
    }
    e.Replikation_Uid = Functions.GetUid();
    SbEreignisRep.Insert(daten, e);
  }

  /// <summary>
  /// Gets GEDCOM level from string.
  /// </summary>
  /// <param name="str">Affected string.</param>
  /// <returns>Level number.</returns>
  private static int GetLevel(string str)
  {
    var l = -1;
    if (!string.IsNullOrEmpty(str))
    {
      var m = Level.Match(str);
      if (m.Success)
        l = Functions.ToInt32(m.Groups[1].Value);
    }
    return l;
  }

  /// <summary>
  /// Gets uid from object reference xref and mapping dictionary.
  /// </summary>
  /// <param name="map">Dictionary with mapping between xref and uio.</param>
  /// <param name="xref">Affected xref.</param>
  /// <returns>Fitting uid.</returns>
  private static string GetUid(Dictionary<string, string> map, string xref)
  {
    if (xref == null)
      return null;
    if (map == null)
      return Functions.ToUid(xref);
    if (!map.TryGetValue(xref, out var uid))
    {
      uid = xref.Length <= 6 ? Functions.GetUid() : Functions.ToUid(xref);
      map[xref] = uid;
    }
    return uid;
  }

  /// <summary>
  /// Write header to GEDCOM file.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="l">List of lines for GEDCOM file.</param>
  /// <param name="version">Affected version of GEDCOM file, e.g. 5.5.</param>
  /// <param name="filename">Affected file name.</param>
  private static void WriteHead(ServiceDaten daten, List<string> l, string version, string filename)
  {
    l.Add("0 HEAD");
    l.Add("1 SOUR WKUEHL");
    l.Add("2 VERS 0.1");
    l.Add("2 NAME CSBP-Programm");
    l.Add("1 DEST ANSTFILE or TempleReady");
    l.Add("1 DATE " + daten.Jetzt.ToString("dd MMM yyyy", Functions.CultureInfoEn).ToUpper());
    l.Add("2 TIME " + daten.Jetzt.ToString("hh:mm:ss", Functions.CultureInfoEn));
    if (version.CompareTo("5.5") >= 0)
    {
      l.Add("1 SUBM @999999@"); // submitter
      l.Add("1 SUBN @999998@"); // submission
    }
    l.Add("1 FILE " + filename);
    l.Add("1 GEDC");
    l.Add("2 VERS " + version);
    l.Add("2 FORM LINEAGE-LINKED");
    //// l.Add("1 CHAR ANSI");
    //// UNICODE: Fehler wegen fehlendem BOM
    l.Add("1 CHAR UTF-8");
  }

  /// <summary>
  /// Write all affected inviduals to GEDCOM file.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="l">List of lines for GEDCOM file.</param>
  /// <param name="map">Mapping for xref.</param>
  /// <param name="version">Affected version of GEDCOM file, e.g. 5.5.</param>
  /// <param name="op">Affected operator for Status1.</param>
  /// <param name="tot">Affected comparison value for Status1.</param>
  /// <param name="anc">Affected ancestor uid.</param>
  /// <param name="desc">Affected descendant uid.</param>
  private static void WriteIndividual(ServiceDaten daten, List<string> l, Dictionary<string, int> map, string version,
    string op, int tot, string anc = null, string desc = null)
  {
    var liste = SbPersonRep.GetList(daten); // , status2: status2);
    var status2 = !string.IsNullOrEmpty(anc) ? (int?)null : 1;
    var status3 = !string.IsNullOrEmpty(desc) ? (int?)null : 1;
    foreach (var p in liste)
    {
      if ((!string.IsNullOrEmpty(op) && !Functions.VergleicheInt(Math.Abs(p.Status1), op, tot))
        || (!string.IsNullOrEmpty(anc) && p.Status2 != 1) || (!string.IsNullOrEmpty(desc) && p.Status3 != 1))
        continue;
      var uid = p.Uid;
      var vn = p.Vorname;
      var gn = p.Geburtsname;
      var pn = Functions.Append(vn, " ", "/" + p.Name + "/");
      var bem = p.Bemerkung;
      var konf = p.Konfession;
      var quid = p.Quelle_Uid;
      l.Add($"0 {GetXref(map, "INDI", uid)} INDI");
      l.Add("1 NAME " + pn);
      if (version.CompareTo("5.5") >= 0)
      {
        if (!string.IsNullOrEmpty(vn))
          l.Add("2 GIVN " + vn);
        if (!string.IsNullOrEmpty(gn))
          l.Add("2 SURN " + gn);
      }
      if (p.Geschlecht != null)
      {
        var geschlecht = p.Geschlecht.ToUpper();
        geschlecht = geschlecht.Replace('W', 'F');
        if (Regex.Match(geschlecht, "M|F").Success)
          l.Add("1 SEX " + geschlecht);
      }
      //// Events
      var ereignisse = SbEreignisRep.GetList(daten, uid, null, null, null);
      foreach (var e in ereignisse)
      {
        var zeitangabe = new PedigreeTimeData(e);
        l.Add("1 " + e.Typ);
        l.Add("2 DATE " + zeitangabe.Deparse(true).ToUpper());
        var ort = e.Ort;
        var ebem = e.Bemerkung;
        if (!string.IsNullOrEmpty(ort))
          l.Add("2 PLAC " + ort);
        if (!string.IsNullOrEmpty(ebem))
          SchreibeFortsetzung(l, 2, "NOTE", ebem);
      }
      if (!string.IsNullOrEmpty(konf))
        l.Add("1 RELI " + konf);
      if (!string.IsNullOrEmpty(quid))
        l.Add("1 SOUR " + GetXref(map, "SOUR", quid));
      if (!string.IsNullOrEmpty(bem))
        SchreibeFortsetzung(l, 1, "NOTE", bem);
      var kind = SbKindRep.GetList(daten, kuid: uid, status2: status2, status3: status3).FirstOrDefault();
      if (kind != null)
        l.Add("1 FAMC " + GetXref(map, "FAM", kind.Familie_Uid));
      var eltern = SbFamilieRep.GetList(daten, personuid: uid, status2: status2, status3: status3);
      string fuid = null;
      foreach (var f in eltern)
      {
        // No duplicate entries.
        if (f.Uid != fuid)
          l.Add("1 FAMS " + GetXref(map, "FAM", f.Uid));
        fuid = f.Uid;
      }
    }
  }

  /// <summary>
  /// Write all affected families to GEDCOM file.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="l">List of lines for GEDCOM file.</param>
  /// <param name="map">Mapping for xref.</param>
  /// <param name="op">Affected operator for Status1.</param>
  /// <param name="tot">Affected comparison value for Status1.</param>
  /// <param name="anc">Affected ancestor uid.</param>
  /// <param name="desc">Affected descendant uid.</param>
  private static void WriteFamily(ServiceDaten daten, List<string> l, Dictionary<string, int> map,
    string op, int tot, string anc = null, string desc = null)
  {
    var familien = SbFamilieRep.GetList(daten);
    foreach (var f in familien)
    {
      var familienUid = f.Uid;
      var muid = f.Mann_Uid;
      var fuid = f.Frau_Uid;
      //// var mannTot = f.Father?.Status1 ?? 0;
      //// var frauTot = f.Mother?.Status1 ?? 0;
      if ((!string.IsNullOrEmpty(op) && !Functions.VergleicheInt(Math.Abs(f.Father?.Status1 ?? 0), op, tot))
        || (!string.IsNullOrEmpty(anc) && (f.Father?.Status3 ?? 0) != 1) || (!string.IsNullOrEmpty(desc) && (f.Father?.Status3 ?? 0) != 1))
        muid = null;
      if ((!string.IsNullOrEmpty(op) && !Functions.VergleicheInt(Math.Abs(f.Mother?.Status1 ?? 0), op, tot))
        || (!string.IsNullOrEmpty(anc) && (f.Mother?.Status3 ?? 0) != 1) || (!string.IsNullOrEmpty(desc) && (f.Mother?.Status3 ?? 0) != 1))
        fuid = null;
      if (string.IsNullOrEmpty(muid) && string.IsNullOrEmpty(fuid))
        continue;
      l.Add(("0 " + GetXref(map, "FAM", familienUid)) + " FAM");
      var ereignisse = SbEreignisRep.GetList(daten, null, familienUid);
      foreach (var e in ereignisse)
      {
        var zeitangabe = new PedigreeTimeData(e);
        l.Add("1 " + e.Typ);
        l.Add("2 DATE " + zeitangabe.Deparse(true).ToUpper());
        var ort = e.Ort;
        var ebem = e.Bemerkung;
        if (!string.IsNullOrEmpty(ort))
          l.Add("2 PLAC " + ort);
        if (!string.IsNullOrEmpty(ebem))
          SchreibeFortsetzung(l, 2, "NOTE", ebem);
      }
      if (!string.IsNullOrEmpty(muid))
        l.Add("1 HUSB " + GetXref(map, "INDI", muid));
      if (!string.IsNullOrEmpty(fuid))
        l.Add("1 WIFE " + GetXref(map, "INDI", fuid));
      var children = SbKindRep.GetList(daten, familienUid);
      foreach (var c in children)
      {
        var kindUid = c.Kind_Uid;
        var kindTot = c.Child?.Status1 ?? 0;
        if (!Functions.VergleicheInt(Math.Abs(kindTot), op, tot))
          kindUid = null;
        if (!string.IsNullOrEmpty(kindUid))
          l.Add("1 CHIL " + GetXref(map, "INDI", c.Kind_Uid));
      }
    }
  }

  /// <summary>
  /// Write all affected sources to GEDCOM file.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="l">List of lines for GEDCOM file.</param>
  /// <param name="map">Mapping for xref.</param>
  private static void WriteSource(ServiceDaten daten, List<string> l, Dictionary<string, int> map)
  {
    var quellen = SbQuelleRep.GetList(daten, null); // , status2);
    foreach (var q in quellen)
    {
      // if (!Functions.VergleicheInt(Math.Abs(q.Status1), op, tot))
      //   continue;
      if (!map.ContainsKey(q.Uid))
        continue;
      var quid = q.Uid;
      var autor = q.Autor;
      var beschreibung = q.Beschreibung;
      var zitat = q.Zitat;
      var bemerkung = q.Bemerkung;
      l.Add($"0 {GetXref(map, "SOUR", quid)} SOUR");
      SchreibeFortsetzung(l, 1, "AUTH", autor);
      SchreibeFortsetzung(l, 1, "TITL", beschreibung);
      SchreibeFortsetzung(l, 1, "TEXT", zitat);
      SchreibeFortsetzung(l, 1, "NOTE", bemerkung);
    }
  }

  /// <summary>
  /// Write footer to GEDCOM file.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="l">List of lines for GEDCOM file.</param>
  /// <param name="version">Affected version of GEDCOM file, e.g. 5.5.</param>
  /// <param name="name">Affected pedigree name.</param>
  private static void WriteFoot(ServiceDaten daten, List<string> l, string version, string name)
  {
    Functions.MachNichts(daten);
    if (version.CompareTo("5.5") >= 0)
    {
      var p = Parameter.GetValue(Parameter.SB_SUBMITTER);
      if (!string.IsNullOrEmpty(p))
      {
        var arr = p.Split(';');
        l.Add("0 @999999@ SUBM");
        if (arr.Length > 0)
          l.Add("1 NAME " + arr[0]);
        if (arr.Length > 1)
          l.Add("1 ADDR " + arr[1]);
        if (arr.Length > 2)
          l.Add("2 CONT " + arr[2]);
        if (arr.Length > 3)
          l.Add("2 CONT " + arr[3]);
        if (arr.Length > 4)
          l.Add("2 CONT " + arr[4]);
        //// l.Add("1 PHON 00000-11111");
      }
    }
    if (version.CompareTo("5.5") >= 0)
    {
      // submission
      l.Add("0 @999998@ SUBN");
      l.Add("1 SUBM @999999@");
      l.Add("1 FAMF " + name);
    }
    l.Add("0 TRLR");
  }

  /// <summary>
  /// Gets object reference xref from type and uid.
  /// </summary>
  /// <param name="map">Mapping dictionary.</param>
  /// <param name="type">FAM for family, INDI for individual, SOUR for source.</param>
  /// <param name="uid">Affected uid.</param>
  /// <returns>Fittiing object reference.</returns>
  private static string GetXref(Dictionary<string, int> map, string type, string uid)
  {
    string s;
    if (map == null)
      s = Functions.ToXref(uid);
    else
    {
      if (!map.TryGetValue(uid, out var i))
      {
        i = map.Count + 1;
        //// if (typ == "SOUR")
        ////   i = -i;
        map[uid] = i;
      }
      s = i.ToString();
    }
    if (type == "FAM")
      return $"@F{s}@";
    else if (type == "INDI")
      return $"@I{s}@";
    else
    {
      // SOUR
      return $"@Q{s}@";
    }
  }

  /// <summary>
  /// Adds a longer text in GEDCOM format to a list of lines. The text is broken up after 248 characters.
  /// </summary>
  /// <param name="l">Affected list of lines.</param>
  /// <param name="level">Affected level.</param>
  /// <param name="type">Entry type, e.g. TITL, TEXT, AUTH.</param>
  /// <param name="text">Affected text.</param>
  private static void SchreibeFortsetzung(List<string> l, int level, string type, string text)
  {
    // Maximal Länge einer GEDCOM-Zeile.
    const int MAX_GEDCOM_ZEILE = 248;
    var iLen = text == null ? 0 : text.Length;
    if (iLen > 0)
    {
      var iMax = ((iLen - 1) / MAX_GEDCOM_ZEILE) + 1;
      for (var i = 1; i <= iMax; i++)
      {
        string str;
        if (i < iMax)
          str = text.Substring((i - 1) * MAX_GEDCOM_ZEILE, i * MAX_GEDCOM_ZEILE);
        else
          str = text[((i - 1) * MAX_GEDCOM_ZEILE)..]; // i == iMax
        if (i == 1)
          l.Add(level + " " + type + " " + str);
        else
          l.Add((level + 1) + " CONT " + str);
      }
    }
  }

  /// <summary>
  /// Deletes a source.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="q">Affected entity.</param>
  private static void DeleteQuelleIntern(ServiceDaten daten, SbQuelle q)
  {
    var pliste = SbPersonRep.GetList(daten, suid: q.Uid);
    if (pliste.Any())
      throw new MessageException(SB015);
    var eliste = SbEreignisRep.GetList(daten, suid: q.Uid);
    if (eliste.Any())
      throw new MessageException(SB016);
    SbQuelleRep.Delete(daten, q);
  }

  /// <summary>
  /// Saves a person or family event.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="puid">Affected ancestor uid.</param>
  /// <param name="fuid">Affected familiy uid.</param>
  /// <param name="type">Affected event type, e.g. BIRT or MARR.</param>
  /// <param name="date">Affected event date.</param>
  /// <param name="place">Affected place.</param>
  /// <param name="memo">Affected memo.</param>
  /// <param name="quid">Affected source uid.</param>
  private static void SaveEvent(ServiceDaten daten, string puid, string fuid, string type, string date,
    string place, string memo, string quid)
  {
    var zeitangabe = new PedigreeTimeData();
    if (zeitangabe.Parse(date))
      throw new MessageException(SB003(date));
    if (string.IsNullOrEmpty(puid) && string.IsNullOrEmpty(fuid))
      throw new MessageException(SB004);
    if (string.IsNullOrEmpty(type))
      throw new MessageException(SB005);
    if (string.IsNullOrEmpty(zeitangabe.DateType) && !zeitangabe.Date1.Empty)
      throw new MessageException(SB006);
    var loeschen = zeitangabe.Date1.Empty && string.IsNullOrEmpty(zeitangabe.DateType) && zeitangabe.Date2.Empty &&
      string.IsNullOrEmpty(place) && string.IsNullOrEmpty(memo);
    if (loeschen)
    {
      var e = SbEreignisRep.Get(daten, daten.MandantNr, puid, fuid, type);
      if (e != null)
        SbEreignisRep.Delete(daten, e);
      return;
    }
    SbEreignisRep.Save(daten, daten.MandantNr, puid, fuid, type, zeitangabe.Date1.Tag, zeitangabe.Date1.Monat,
      zeitangabe.Date1.Jahr, zeitangabe.Date2.Tag, zeitangabe.Date2.Monat, zeitangabe.Date2.Jahr,
      zeitangabe.DateType, place, memo, quid);
  }

  /// <summary>
  /// Saves a new family and adds a child relation.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Affected family uid.</param>
  /// <param name="mannUid">Affected father uid.</param>
  /// <param name="frauUid">Affected mother uid.</param>
  /// <param name="kindUid">Affected child uid.</param>
  /// <param name="doppelt">Throws an exception if this family with father and mother already exists or not.</param>
  /// <returns>New family.</returns>
  private static SbFamilie NeueFamilie(ServiceDaten daten, string uid, string mannUid, string frauUid, string kindUid,
    bool doppelt)
  {
    var fuid = uid;
    string fuid2 = null;
    SbPerson sbPerson;
    var maUid = mannUid;
    var frUid = frauUid;
    if (string.IsNullOrEmpty(maUid) && string.IsNullOrEmpty(frUid))
      throw new MessageException(SB007);
    if (!string.IsNullOrEmpty(maUid))
    {
      sbPerson = SbPersonRep.Get(daten, daten.MandantNr, maUid);
      if (sbPerson == null || GenderEnum.MAENNLICH.ToString() != sbPerson.Geschlecht)
        throw new MessageException(SB008);
    }
    if (!string.IsNullOrEmpty(frUid))
    {
      sbPerson = SbPersonRep.Get(daten, daten.MandantNr, frUid);
      if (sbPerson == null || GenderEnum.WEIBLICH.ToString() != sbPerson.Geschlecht)
        throw new MessageException(SB009);
    }
    var fliste = SbFamilieRep.GetList(daten, null, maUid, frUid, null, fuid);
    var sbFamilie = fliste.FirstOrDefault();
    if (sbFamilie != null)
    {
      fuid2 = sbFamilie.Uid;
      maUid = sbFamilie.Mann_Uid;
      frUid = sbFamilie.Frau_Uid;
    }
    if (!string.IsNullOrEmpty(fuid2))
    {
      if (doppelt)
        throw new MessageException(SB010(fuid2));
      fuid = fuid2;
    }
    var f = IuFamilie(daten, fuid, maUid, frUid);
    fuid = f.Uid;
    if (!string.IsNullOrEmpty(kindUid))
      IuKind(daten, fuid, kindUid);
    return f;
  }

  /// <summary>
  /// Saves a family.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Affected family uid.</param>
  /// <param name="mannUid">Affected father uid.</param>
  /// <param name="frauUid">Affected mother uid.</param>
  /// <returns>Affected entity.</returns>
  private static SbFamilie IuFamilie(ServiceDaten daten, string uid, string mannUid, string frauUid)
  {
    if (string.IsNullOrEmpty(mannUid) && string.IsNullOrEmpty(frauUid))
      throw new MessageException(SB007);
    var f = SbFamilieRep.Save(daten, daten.MandantNr, uid, mannUid, frauUid, 0, 0, 0);
    return f;
  }

  /// <summary>
  /// Saves a family child relation.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="fuid">Affected family uid.</param>
  /// <param name="kindUid">Affected child uid.</param>
  /// <returns>Affected entity.</returns>
  private static SbKind IuKind(ServiceDaten daten, string fuid, string kindUid)
  {
    if (string.IsNullOrEmpty(fuid) || string.IsNullOrEmpty(kindUid))
      throw new MessageException(SB011);
    var kliste = SbKindRep.GetList(daten, null, kindUid, fuid);
    var vo2 = kliste.FirstOrDefault();
    if (vo2 != null)
      throw new MessageException(SB012(vo2.Familie_Uid));
    var k = SbKindRep.Save(daten, daten.MandantNr, fuid, kindUid);
    return k;
  }

  /// <summary>
  /// Deletes an ancestor with all relations like family, child, events, pictures.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="p">Affected entity.</param>
  private static void DeletePersonIntern(ServiceDaten daten, SbPerson p)
  {
    // Delets person from family.
    var liste = SbFamilieRep.GetList(daten, null, null, null, p.Uid);
    foreach (var f in liste)
    {
      if (f.Mann_Uid == p.Uid)
        f.Mann_Uid = null;
      if (f.Frau_Uid == p.Uid)
        f.Frau_Uid = null;
      if (string.IsNullOrEmpty(f.Mann_Uid) && string.IsNullOrEmpty(f.Frau_Uid))
        DeleteFamilieIntern(daten, f);
      else
        SbFamilieRep.Update(daten, f);
    }

    // Person als Kind löschen
    var kliste = SbKindRep.GetList(daten, null, p.Uid);
    foreach (var k in kliste)
    {
      SbKindRep.Delete(daten, k);
    }

    // Person-Ereignisse löschen
    var eliste = SbEreignisRep.GetList(daten, p.Uid);
    foreach (var e in eliste)
    {
      SbEreignisRep.Delete(daten, e);
    }

    // Bilder löschen
    var bliste = ByteDatenRep.GetList(daten, "SB_Person", p.Uid);
    foreach (var b in bliste)
    {
      ByteDatenRep.Delete(daten, b);
    }
    SbPersonRep.Delete(daten, p);
  }

  /// <summary>
  /// Deletes a family with children and events.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="f">Affected family.</param>
  private static void DeleteFamilieIntern(ServiceDaten daten, SbFamilie f)
  {
    // Kinder löschen
    var liste = SbKindRep.GetList(daten, f.Uid);
    foreach (var k in liste)
    {
      SbKindRep.Delete(daten, k);
    }

    // Familien-Ereignisse löschen
    var eliste = SbEreignisRep.GetList(daten, null, f.Uid, null, null);
    foreach (var e in eliste)
    {
      SbEreignisRep.Delete(daten, e);
    }

    // Holt die unveränderte Familie.
    f = SbFamilieRep.Get(daten, daten.MandantNr, f.Uid);
    if (f != null)
      SbFamilieRep.Delete(daten, f);
  }

  /// <summary>
  /// Gets all spouces of an ancestor.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="puid">Affected ancestor.</param>
  /// <returns>List of all spouces.</returns>
  private static List<string> GetAlleEhegatten(ServiceDaten daten, string puid)
  {
    var liste = new List<string>();
    if (puid == null)
      return liste;

    var liste0 = new List<string>();
    var liste1 = new List<string>();
    liste0.Add(puid);
    var anz = -1;
    while (anz < liste.Count)
    {
      anz = liste.Count;
      liste1.Clear();
      foreach (var s in liste0)
      {
        var l = GetEhegatten0(daten, s);
        foreach (var e in l)
        {
          if (!liste.Contains(e))
          {
            liste.Add(e);
            liste1.Add(e);
          }
        }
      }
      liste0.Clear();
      liste0.AddRange(liste1);
    }
    return liste;
  }

  /// <summary>
  /// Gets list of uids of spouces of an ancestor.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="puid">Affected ancestor.</param>
  /// <returns>List of uids of spouces of an ancestor.</returns>
  private static List<string> GetEhegatten0(ServiceDaten daten, string puid)
  {
    var liste = new List<string>();
    if (!string.IsNullOrEmpty(puid))
    {
      var fliste = SbFamilieRep.GetList(daten, null, null, null, puid);
      foreach (var f in fliste)
      {
        if (!string.IsNullOrEmpty(f.Mann_Uid) && puid != f.Mann_Uid)
          liste.Add(f.Mann_Uid);
        if (!string.IsNullOrEmpty(f.Frau_Uid) && puid != f.Frau_Uid)
          liste.Add(f.Frau_Uid);
      }
    }
    return liste;
  }

  /// <summary>
  /// Gets first family with ancestor as father or mother.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Affected ancestor uid.</param>
  /// <returns>Affected family.</returns>
  private static SbFamilie GetElternFamilieIntern(ServiceDaten daten, string uid)
  {
    var fliste = SbFamilieRep.GetList(daten, personuid: uid);
    return fliste.FirstOrDefault();
  }

  /// <summary>
  /// Gets child relation where the ancestor is a child.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Affected ancestor uid.</param>
  /// <returns>Affected child relation.</returns>
  private static SbKind GetKindFamilieIntern(ServiceDaten daten, string uid)
  {
    var k = SbKindRep.GetList(daten, null, uid).FirstOrDefault();
    return k;
  }

  /// <summary>
  /// Gets descendants of an ancestor in recursive manner.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Affected ancestor uid.</param>
  /// <param name="stufe">Affected level: 0 with events, with spouces; 1 without events, with spouces;
  /// 2 with events, without spouces; 3 without event, without spouces.</param>
  /// <param name="generation">Current generation, also for indentation.</param>
  /// <param name="max">Maximum number of generations.</param>
  /// <param name="liste">List of descendants will be filled.</param>
  private void GetNachfahrenRekursiv(ServiceDaten daten, string uid, int stufe, int generation, int max, List<SbPerson> liste)
  {
    var mitEreignis = (stufe & 1) == 0;
    var mitPartner = (stufe & 2) == 0;
    var zeile = new StringBuilder();
    var sbPerson = SbPersonRep.Get(daten, daten.MandantNr, uid) ?? throw new MessageException(SB017(uid));
    var strGen = mitPartner ? "" + generation : "+";
    var strPraefix = new string(' ', (Math.Abs(generation) - 1) * 1);
    zeile.Append(strPraefix).Append(strGen).Append(' ').Append(
      Functions.AhnString(sbPerson.Uid, sbPerson.Geburtsname, sbPerson.Vorname, true));
    if (mitEreignis)
    {
      var ereignisse = SbEreignisRep.GetList(daten, uid);
      foreach (var e in ereignisse)
      {
        zeile.Append(' ').Append(((GedcomEventEnum)e.Typ).ToSymbol());
        var zeitangabe = new PedigreeTimeData(e);
        zeile.Append(zeitangabe.Deparse());
        Functions.Append(zeile, " in ", e.Ort);
        Functions.Append(zeile, ", ", e.Bemerkung);
      }
    }
    sbPerson.Bemerkung = zeile.ToString();
    liste.Add(sbPerson);
    if (mitPartner)
    {
      var familien = SbFamilieRep.GetList(daten, personuid: uid);
      foreach (var f in familien)
      {
        if (f.Mann_Uid == uid)
        {
          if (!string.IsNullOrEmpty(f.Frau_Uid))
          {
            GetNachfahrenRekursiv(daten, f.Frau_Uid, stufe | 2, generation, max, liste);
            //// auchKinder = true;
          }
        }
        else
        {
          if (!string.IsNullOrEmpty(f.Mann_Uid))
          {
            GetNachfahrenRekursiv(daten, f.Mann_Uid, stufe | 2, generation, max, liste);
            //// auchKinder = true;
          }
        }
        var auchKinder = true;
        if (auchKinder && generation <= max)
        {
          var kinder = SbKindRep.GetList(daten, f.Uid);
          foreach (var k in kinder)
          {
            GetNachfahrenRekursiv(daten, k.Kind_Uid, stufe, generation + 1, max, liste);
          }
        }
      }
    }
  }

  /// <summary>
  /// Gets forebears of an ancestor in recursive manner.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Affected ancestor uid.</param>
  /// <param name="mitEreignis">With events or not.</param>
  /// <param name="mitGeschwistern">With siblings or not.</param>
  /// <param name="mitEltern">With parents or not.</param>
  /// <param name="generation">Current generation, also for indentation.</param>
  /// <param name="max">Maximum number of generations.</param>
  /// <param name="liste">List of forebears will be filled.</param>
  private void GetVorfahrenRekursiv(ServiceDaten daten, string uid, bool mitEreignis, bool mitGeschwistern,
    bool mitEltern, int generation, int max, List<SbPerson> liste)
  {
    var zeile = new StringBuilder();
    var sbPerson = SbPersonRep.Get(daten, daten.MandantNr, uid) ?? throw new MessageException(SB017(uid));
    var strGen = mitEltern ? "" + generation : "+";
    var strPraefix = new string(' ', (Math.Abs(generation) - 1) * 1);
    zeile.Append(strPraefix).Append(strGen).Append(' ').Append(
      Functions.AhnString(sbPerson.Uid, sbPerson.Geburtsname, sbPerson.Vorname, true));
    if (mitEreignis)
    {
      var ereignisse = SbEreignisRep.GetList(daten, uid);
      foreach (var e in ereignisse)
      {
        zeile.Append(' ').Append(((GedcomEventEnum)e.Typ).ToSymbol());
        var zeitangabe = new PedigreeTimeData(e);
        zeile.Append(zeitangabe.Deparse());
        Functions.Append(zeile, " in ", e.Ort);
        Functions.Append(zeile, ", ", e.Bemerkung);
      }
    }
    sbPerson.Bemerkung = zeile.ToString();
    liste.Add(sbPerson);
    var familieUid = GetKindFamilieIntern(daten, uid)?.Familie_Uid;
    if (mitGeschwistern && !string.IsNullOrEmpty(familieUid))
    {
      var kinder = SbKindRep.GetList(daten, familieUid, null, null, uid);
      foreach (var k in kinder)
      {
        GetVorfahrenRekursiv(daten, k.Kind_Uid, mitEreignis, false, false, generation, max, liste);
      }
    }
    if (mitEltern && generation < max && !string.IsNullOrEmpty(familieUid))
    {
      var f = SbFamilieRep.Get(daten, daten.MandantNr, familieUid);
      if (f != null)
      {
        if (!string.IsNullOrEmpty(f.Mann_Uid))
          GetVorfahrenRekursiv(daten, f.Mann_Uid, mitEreignis, mitGeschwistern, true, generation + 1, max, liste);
        if (!string.IsNullOrEmpty(f.Frau_Uid))
          GetVorfahrenRekursiv(daten, f.Frau_Uid, mitEreignis, mitGeschwistern, true, generation + 1, max, liste);
      }
    }
  }
}
