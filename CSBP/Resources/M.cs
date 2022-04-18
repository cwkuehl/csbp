// <copyright file="M.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Resources
{
  using System;

  public class M : Messages
  {
    public static string AD001(DateTime f, DateTime t) => string.Format(M0(AD001_), f, t);

    public static string AD002(DateTime d, string name, int y) => string.Format(M0(AD002_), d, name, y);

    public static string AD003(DateTime d) => string.Format(M0(AD003_), d);

    public static string AD010(int p, int pe, int s, int se, int a, int ae) => string.Format(M0(AD010_), p, pe, s, se, a, ae);

    public static string AD013(string u) => string.Format(M0(AD013_), u);

    public static string AM003(int client, string user) => string.Format(M0(AM003_), client, user);

    public static string FZ001(string s, DateTime? d, decimal n, bool cut = true) => string.Format(M0(FZ001_, cut), s, d, n);

    public static string FZ002(decimal s, bool cut = true) => string.Format(M0(FZ002_, cut), s);

    public static string FZ003(string s, bool cut = true) => string.Format(M0(FZ003_, cut), s);

    public static string FZ004(int l, bool cut = true) => string.Format(M0(FZ004_, cut), l);

    public static string FZ005(int l, bool cut = true) => string.Format(M0(FZ005_, cut), l);

    public static string FZ006(int l, bool cut = true) => string.Format(M0(FZ006_, cut), l);

    public static string FZ007(int l, bool cut = true) => string.Format(M0(FZ007_, cut), l);

    public static string FZ008(decimal l, bool cut = true) => string.Format(M0(FZ008_, cut), l);

    public static string FZ009(int l, bool cut = true) => string.Format(M0(FZ009_, cut), l);

    public static string FZ010(int l, bool cut = true) => string.Format(M0(FZ010_, cut), l);

    public static string FZ011(int l, bool cut = true) => string.Format(M0(FZ011_, cut), l);

    public static string FZ012(int l, bool cut = true) => string.Format(M0(FZ012_, cut), l);

    public static string FZ013(int l, bool cut = true) => string.Format(M0(FZ013_, cut), l);

    public static string FZ014(int l, bool cut = true) => string.Format(M0(FZ014_, cut), l);

    public static string FZ015(decimal l, bool cut = true) => string.Format(M0(FZ015_, cut), l);

    public static string FZ016(string s, decimal k, decimal y, bool cut = true) => string.Format(M0(FZ016_, cut), s, k, y);

    public static string FZ017(string s, decimal k, decimal y, bool cut = true) => string.Format(M0(FZ017_, cut), s, k, y);

    public static string FZ024(string uid, bool cut = false) => string.Format(M0(FZ024_, cut), uid);

    public static string FZ025(decimal d, bool cut = false) => string.Format(M0(FZ025_, cut), d);

    public static string FZ029(int i, bool cut = false) => string.Format(M0(FZ029_, cut), i);

    public static string FZ036(string s, bool cut = false) => string.Format(M0(FZ036_, cut), s);

    public static string HH005(int p, bool cut = false) => string.Format(M0(HH005_, cut), p);

    public static string HH006(int p, string kz, string sh, string nr, bool cut = true) => string.Format(M0(HH006_, cut), p, kz, sh, nr);

    public static string HH008(string t, bool cut = false) => string.Format(M0(HH008_, cut), t);

    public static string HH009(string a, bool cut = false) => string.Format(M0(HH009_, cut), a);

    public static string HH010(string a, bool cut = false) => string.Format(M0(HH010_, cut), a);

    public static string HH013(string a, bool cut = false) => string.Format(M0(HH013_, cut), a);

    public static string HH017(string d, bool cut = false) => string.Format(M0(HH017_, cut), d);

    public static string HH019(string nr, bool cut = false) => string.Format(M0(HH019_, cut), nr);

    public static string HH023(DateTime f, DateTime t, bool cut = true) => string.Format(M0(HH023_, cut), f, t);

    public static string HH025(string u, bool cut = true) => string.Format(M0(HH025_, cut), u);

    public static string HH026(int c, bool cut = false) => string.Format(M0(HH026_, cut), c);

    public static string HH038(DateTime d, bool cut = false) => string.Format(M0(HH038_, cut), d);

    public static string HH039(DateTime d, bool cut = false) => string.Format(M0(HH039_, cut), d);

    public static string HH041(DateTime d, bool cut = false) => string.Format(M0(HH041_, cut), d);

    public static string HH042(DateTime d, bool cut = false) => string.Format(M0(HH042_, cut), d);

    public static string HH043(string u, bool cut = false) => string.Format(M0(HH043_, cut), u);

    public static string HH046(string p, string t, DateTime d, bool cut = true) => string.Format(M0(HH046_, cut), p, t, d);

    public static string HH047(string p, string t, DateTime d, bool cut = true) => string.Format(M0(HH047_, cut), p, t, d);

    public static string HH051(string c, bool cut = false) => string.Format(M0(HH051_, cut), c);

    public static string HH053(int c, bool cut = true) => string.Format(M0(HH053_, cut), c);

    public static string HH054(int c, decimal s, bool cut = true) => string.Format(M0(HH054_, cut), c, s);

    public static string HH057(DateTime vd, decimal v, string d, string c, string t, bool cut = true) => string.Format(M0(HH057_, cut), vd, v, d, c, t);

    public static string HH058(string rd, bool cut = true) => string.Format(M0(HH058_, cut), rd);

    public static string HH059(DateTime rd, bool cut = true) => string.Format(M0(HH059_, cut), rd);

    public static string HH061(string b, bool cut = true) => string.Format(M0(HH061_, cut), b);

    public static string HH063(string t, int y, bool cut = true) => string.Format(M0(HH063_, cut), t, y);

    public static string HH070(string n, bool cut = true) => string.Format(M0(HH070_, cut), n);

    public static string HH071(string n, DateTime d, bool cut = true) => string.Format(M0(HH071_, cut), n, d);

    public static string HH072(string n, bool cut = true) => string.Format(M0(HH072_, cut), n);

    public static string HH073(string n, DateTime d, bool cut = true) => string.Format(M0(HH073_, cut), n, d);

    public static string SB003(string d, bool cut = false) => string.Format(M0(SB003_, cut), d);

    public static string SB010(string d, bool cut = false) => string.Format(M0(SB010_, cut), d);

    public static string SB012(string d, bool cut = false) => string.Format(M0(SB012_, cut), d);

    public static string SB017(string d, bool cut = false) => string.Format(M0(SB017_, cut), d);

    public static string SB018(DateTime d, bool cut = true) => string.Format(M0(SB018_, cut), d);

    public static string SB019(string d, int g, bool cut = true) => string.Format(M0(SB019_, cut), d, g);

    public static string SB021(string d, int g, string s, bool cut = true) => string.Format(M0(SB021_, cut), d, g, s);

    public static string SB026(int c, bool cut = true) => string.Format(M0(SB026_, cut), c);

    public static string SB028(int c, int cb, bool cut = true) => string.Format(M0(SB028_, cut), c, cb);

    public static string SB031(string s, bool cut = true) => string.Format(M0(SB031_, cut), s);

    public static string SB032(string s, bool cut = true) => string.Format(M0(SB032_, cut), s);

    public static string SB033(string s, bool cut = true) => string.Format(M0(SB033_, cut), s);

    public static string SB034(string s, string s2, bool cut = true) => string.Format(M0(SB034_, cut), s, s2);

    public static string SB035(string s, string s2, bool cut = true) => string.Format(M0(SB035_, cut), s, s2);

    public static string SO002(int c, bool cut = true) => string.Format(M0(SO002_, cut), c);

    public static string SO007(int r, int n, bool cut = false) => string.Format(M0(SO007_, cut), r, n);

    public static string SO008(int c, int n, bool cut = false) => string.Format(M0(SO008_, cut), c, n);

    public static string SO009(int b, int n, bool cut = false) => string.Format(M0(SO009_, cut), b, n);

    public static string SO010(int d, int r, int n, bool cut = false) => string.Format(M0(SO010_, cut), d, r, n);

    public static string TB002(DateTime d) => string.Format(M0(TB002_), d);

    public static string TB003(string[] s) => string.Format(M0(TB003_), s[0], s[1], s[2], s[3], s[4], s[5], s[6], s[7], s[8]);

    public static string TB004(DateTime d, string act, long exp, bool cut = false) => string.Format(M0(TB004_, cut), d, act, exp);

    public static string TB006(DateTime d, string p, string entry) => string.Format(M0(TB006_), d, p, entry);

    public static string TB013(DateTime d, bool cut = false) => string.Format(M0(TB013_, cut), d);

    public static string WP008(int i, int l, string desc, DateTime d, string ext = null, bool cut = true) => string.Format(M0(WP008_, cut), i, l, desc, d, ext);

    public static string TB010(string s) => string.Format(M0(TB010_), s);

    public static string TB011(DateTime? f, DateTime? t) => string.Format(M0(TB011_), f, t);

    public static string WP009(int i, int l, string desc, DateTime d, string ext = null, bool cut = true) => string.Format(M0(WP009_, cut), i, l, desc, d, ext);

    public static string WP012(int sc, bool cut = false) => string.Format(M0(WP012_, cut), sc);

    public static string WP023(decimal p, string pm, decimal s, decimal sv, decimal i, string str, string s2, bool cut = true) => string.Format(M0(WP023_, cut), p, pm, s, sv, i, str, s2);

    public static string WP024(decimal p, string str, decimal v, decimal pf, decimal ppc, bool cut = true) => string.Format(M0(WP024_, cut), p, str, v, pf, ppc);

    public static string WP025(string c, decimal p, string s, bool cut = true) => string.Format(M0(WP025_, cut), c, p, s);

    public static string WP026(DateTime d, bool cut = true) => string.Format(M0(WP026_, cut), d);

    public static string WP028(decimal s, decimal v, DateTime d, bool cut = true) => string.Format(M0(WP028_, cut), s, v, d);

    public static string WP029(int r, decimal s, decimal v, decimal p, decimal pp, decimal pd, bool cut = true) => string.Format(M0(WP029_, cut), r, s, v, p, pp, pd);

    public static string WP030(DateTime d, string p, string di, string s, string i, string desc, string text, bool cut = true) => string.Format(M0(WP030_, cut), d, p, di, s, i, desc, text);

    public static string WP039(int r, bool cut = false) => string.Format(M0(WP039_, cut), r);

    public static string WP046(int d, bool cut = true) => string.Format(M0(WP046_, cut), d);

    public static string WP047(DateTime d1, DateTime d2, bool cut = false) => string.Format(M0(WP047_, cut), d1, d2);

    public static string WP050(string f, string f2, bool cut = false) => string.Format(M0(WP050_, cut), f, f2);

    public static string WP051(DateTime d, bool cut = true) => string.Format(M0(WP051_, cut), d);

    public static string WP054(string c, string e, bool cut = true) => string.Format(M0(WP054_, cut), c, e);

    public static string M1001(string s, DateTime d) => string.Format(M0(M1001_), s, d);

    public static string M1011(DateTime date, string of, bool cut = false) => string.Format(M0(M1011_, cut), date, of);

    public static string M1019(int ch, string csv, bool cut = false) => string.Format(M0(M1019_, cut), ch, csv);

    public static string M1020(string csv, bool cut = false) => string.Format(M0(M1020_, cut), csv);

    public static string M1034(string file, bool cut = false) => string.Format(M0(M1034_, cut), file);

    public static string M1035(string folder, bool cut = false) => string.Format(M0(M1035_, cut), folder);

    public static string M1038(string folder, bool cut = false) => string.Format(M0(M1038_, cut), folder);

    public static string M0(string m, bool cut = true)
    {
      if (!cut)
        return m;
      if (string.IsNullOrEmpty(m) || m.Length < 5)
        return null;
      return m.Substring(5); // .Trim();
    }
  }
}
