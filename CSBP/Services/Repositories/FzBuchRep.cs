// <copyright file="FzBuchRep.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using CSBP.Apis.Models;
  using CSBP.Apis.Services;
  using CSBP.Base;
  using Microsoft.EntityFrameworkCore;

  /// <summary>
  /// Klasse für FZ_Buch-Repository.
  /// </summary>
  public partial class FzBuchRep
  {
    /// <summary>
    /// Gets a list of books.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="auuid">Affected author ID.</param>
    /// <param name="seuid">Affected series ID.</param>
    /// <param name="bouid">Affected book ID.</param>
    /// <param name="name">Affected name.</param>
    /// <param name="no">Affected serial number.</param>
    /// <param name="max">Maximal amount of records.</param>
    /// <param name="descseries">Sorting descending by serial number.</param>
    /// <returns>List of books.</returns>
    public List<FzBuch> GetList(ServiceDaten daten, string auuid, string seuid = null, string bouid = null,
      string name = null, int no = 0, int max = 0, bool descseries = false)
    {
      var db = GetDb(daten);
      var wl = db.FZ_Buch.Where(a => a.Mandant_Nr == daten.MandantNr);
      if (!string.IsNullOrEmpty(bouid))
        wl = wl.Where(a => a.Uid == bouid);
      if (!string.IsNullOrEmpty(auuid))
        wl = wl.Where(a => a.Autor_Uid == auuid);
      if (!string.IsNullOrEmpty(seuid))
        wl = wl.Where(a => a.Serie_Uid == seuid);
      if (Functions.IsLike(name))
        wl = wl.Where(a => EF.Functions.Like(a.Titel, name) || EF.Functions.Like(a.Untertitel, name));
      if (no > 0)
        wl = wl.Where(a => a.Seriennummer == no);
      var l = wl.Join(db.FZ_Buchautor.Where(a => a.Mandant_Nr == daten.MandantNr),
        a => a.Autor_Uid, b => b.Uid, (a, b) => new { book = a, author = b });
      var l1 = l.Join(db.FZ_Buchserie.Where(a => a.Mandant_Nr == daten.MandantNr),
        a => a.book.Serie_Uid, b => b.Uid, (a, b) => new { a.book, a.author, series = b });
      var l2 = l1.Join(db.FZ_Buchstatus.Where(a => a.Mandant_Nr == daten.MandantNr),
        a => a.book.Uid, b => b.Buch_Uid, (a, b) => new { a.book, a.author, a.series, state = b });
      var l3 = descseries ? l2.OrderByDescending(a => a.book.Serie_Uid).ThenByDescending(a => a.book.Seriennummer)
        : l2.OrderByDescending(a => a.state.Lesedatum).ThenByDescending(a => a.book.Serie_Uid).ThenByDescending(a => a.book.Seriennummer);
      var l4 = l3.Take(max <= 0 ? int.MaxValue : max).ToList()
        .Select(a =>
        {
          a.book.AuthorName = a.author.Name;
          a.book.AuthorFirstName = a.author.Vorname;
          a.book.SeriesName = a.series.Name;
          a.book.StatePossession = a.state.Ist_Besitz;
          a.book.StateRead = a.state.Lesedatum;
          a.book.StateHeard = a.state.Hoerdatum;
          return a.book;
        });
      return l4.ToList();
    }

    /// <summary>
    /// Gets number of books or pages.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="lnr">Affected language number.</param>
    /// <param name="suid">Affected series ID.</param>
    /// <param name="suidne">Not affected series ID.</param>
    /// <param name="createdle">Affected max. creation date.</param>
    /// <param name="readle">Affected max. creation date.</param>
    /// <param name="heardle">Affected max. creation date.</param>
    /// <returns>Number of books or pages.</returns>
    public int Count(ServiceDaten daten, int lnr, string suid = null, string suidne = null,
      DateTime? createdle = null, DateTime? readle = null, DateTime? heardle = null, bool pages = false)
    {
      var db = GetDb(daten);
      var l = db.FZ_Buch.Where(a => a.Mandant_Nr == daten.MandantNr);
      if (lnr >= 0)
        l = l.Where(a => a.Sprache_Nr == lnr);
      if (!string.IsNullOrEmpty(suid))
        l = l.Where(a => a.Serie_Uid == suid);
      if (!string.IsNullOrEmpty(suidne))
        l = l.Where(a => a.Serie_Uid != suidne);
      if (createdle.HasValue)
        l = l.Where(a => a.Angelegt_Am <= createdle.Value);
      var ls = db.FZ_Buchstatus.Where(a => a.Mandant_Nr == daten.MandantNr);
      if (readle.HasValue)
        ls = ls.Where(a => a.Lesedatum <= readle.Value);
      if (heardle.HasValue)
        ls = ls.Where(a => a.Hoerdatum <= heardle.Value);
      l = l.Join(ls, a => new { a.Uid }, b => new { Uid = b.Buch_Uid }, (a, b) => a);
      var c = pages ? l.Sum(a => a.Seiten) : l.Count();
      return c;
    }
  }
}
