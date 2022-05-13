// <copyright file="Messages.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Resources
{
  using System;

  /// <summary>
  /// Resource class for Messages.
  /// </summary>
  public partial class Messages
  {
    private static System.Resources.ResourceManager rm =
      new System.Resources.ResourceManager("CSBP.Resources.Messages", typeof(Messages).Assembly);

    // private static System.Globalization.CultureInfo rc;

    // internal static System.Globalization.CultureInfo Culture
    // {
    //   get { return rc; }
    //   set { rc = value; }
    // }

    public static string Get(string key)
    {
      return rm.GetString(key);
    }

    public static string parm_AG_ANWENDUNGS_TITEL_value
    {
      get { return rm.GetString("parm.AG_ANWENDUNGS_TITEL.value"); }
    }

    public static string parm_AG_ANWENDUNGS_TITEL_text
    {
      get { return rm.GetString("parm.AG_ANWENDUNGS_TITEL.text"); }
    }

    public static string parm_AG_HILFE_DATEI_value
    {
      get { return rm.GetString("parm.AG_HILFE_DATEI.value"); }
    }

    public static string parm_AG_HILFE_DATEI_text
    {
      get { return rm.GetString("parm.AG_HILFE_DATEI.text"); }
    }

    public static string parm_AG_STARTDIALOGE_value
    {
      get { return rm.GetString("parm.AG_STARTDIALOGE.value"); }
    }

    public static string parm_AG_STARTDIALOGE_text
    {
      get { return rm.GetString("parm.AG_STARTDIALOGE.text"); }
    }

    public static string parm_AG_TEMP_PFAD_value
    {
      get { return rm.GetString("parm.AG_TEMP_PFAD.value"); }
    }

    public static string parm_AG_TEMP_PFAD_text
    {
      get { return rm.GetString("parm.AG_TEMP_PFAD.text"); }
    }

    public static string parm_AG_TEST_PRODUKTION_value
    {
      get { return rm.GetString("parm.AG_TEST_PRODUKTION.value"); }
    }

    public static string parm_AG_TEST_PRODUKTION_text
    {
      get { return rm.GetString("parm.AG_TEST_PRODUKTION.text"); }
    }

    public static string parm_SB_SUBMITTER_value
    {
      get { return rm.GetString("parm.SB_SUBMITTER.value"); }
    }

    public static string parm_SB_SUBMITTER_text
    {
      get { return rm.GetString("parm.SB_SUBMITTER.text"); }
    }

    public static string parm_WP_FIXER_IO_ACCESS_KEY_value
    {
      get { return rm.GetString("parm.WP_FIXER_IO_ACCESS_KEY.value"); }
    }

    public static string parm_WP_FIXER_IO_ACCESS_KEY_text
    {
      get { return rm.GetString("parm.WP_FIXER_IO_ACCESS_KEY.text"); }
    }

    public static string AD001_
    {
      get { return rm.GetString("AD001_"); }
    }

    public static string AD002_
    {
      get { return rm.GetString("AD002_"); }
    }

    public static string AD003_
    {
      get { return rm.GetString("AD003_"); }
    }

    public static string AD005
    {
      get { return rm.GetString("AD005"); }
    }

    public static string AD010_
    {
      get { return rm.GetString("AD010_"); }
    }

    public static string AD011
    {
      get { return rm.GetString("AD011"); }
    }

    public static string AD012
    {
      get { return rm.GetString("AD012"); }
    }

    public static string AD013_
    {
      get { return rm.GetString("AD013_"); }
    }

    public static string AD014
    {
      get { return rm.GetString("AD014"); }
    }

    public static string AG001
    {
      get { return rm.GetString("AG001"); }
    }

    public static string AG002
    {
      get { return rm.GetString("AG002"); }
    }

    public static string AG003
    {
      get { return rm.GetString("AG003"); }
    }

    public static string AM001
    {
      get { return rm.GetString("AM001"); }
    }

    public static string AM002
    {
      get { return rm.GetString("AM002"); }
    }

    public static string AM003_
    {
      get { return rm.GetString("AM003_"); }
    }

    public static string AM004
    {
      get { return rm.GetString("AM004"); }
    }

    public static string AM005
    {
      get { return rm.GetString("AM005"); }
    }

    public static string AM008
    {
      get { return rm.GetString("AM008"); }
    }

    public static string AM009
    {
      get { return rm.GetString("AM009"); }
    }

    public static string AM010
    {
      get { return rm.GetString("AM010"); }
    }

    public static string AM011
    {
      get { return rm.GetString("AM011"); }
    }

    public static string AM012
    {
      get { return rm.GetString("AM012"); }
    }

    public static string FZ001_
    {
      get { return rm.GetString("FZ001_"); }
    }

    public static string FZ002_
    {
      get { return rm.GetString("FZ002_"); }
    }

    public static string FZ003_
    {
      get { return rm.GetString("FZ003_"); }
    }

    public static string FZ004_
    {
      get { return rm.GetString("FZ004_"); }
    }

    public static string FZ005_
    {
      get { return rm.GetString("FZ005_"); }
    }

    public static string FZ006_
    {
      get { return rm.GetString("FZ006_"); }
    }

    public static string FZ007_
    {
      get { return rm.GetString("FZ007_"); }
    }

    public static string FZ008_
    {
      get { return rm.GetString("FZ008_"); }
    }

    public static string FZ009_
    {
      get { return rm.GetString("FZ009_"); }
    }

    public static string FZ010_
    {
      get { return rm.GetString("FZ010_"); }
    }

    public static string FZ011_
    {
      get { return rm.GetString("FZ011_"); }
    }

    public static string FZ012_
    {
      get { return rm.GetString("FZ012_"); }
    }

    public static string FZ013_
    {
      get { return rm.GetString("FZ013_"); }
    }

    public static string FZ014_
    {
      get { return rm.GetString("FZ014_"); }
    }

    public static string FZ015_
    {
      get { return rm.GetString("FZ015_"); }
    }

    public static string FZ016_
    {
      get { return rm.GetString("FZ016_"); }
    }

    public static string FZ017_
    {
      get { return rm.GetString("FZ017_"); }
    }

    public static string FZ018
    {
      get { return rm.GetString("FZ018"); }
    }

    public static string FZ019
    {
      get { return rm.GetString("FZ019"); }
    }

    public static string FZ020
    {
      get { return rm.GetString("FZ020"); }
    }

    public static string FZ021
    {
      get { return rm.GetString("FZ021"); }
    }

    public static string FZ022
    {
      get { return rm.GetString("FZ022"); }
    }

    public static string FZ024_
    {
      get { return rm.GetString("FZ024_"); }
    }

    public static string FZ025_
    {
      get { return rm.GetString("FZ025_"); }
    }

    public static string FZ026
    {
      get { return rm.GetString("FZ026"); }
    }

    public static string FZ027
    {
      get { return rm.GetString("FZ027"); }
    }

    public static string FZ028
    {
      get { return rm.GetString("FZ028"); }
    }

    public static string FZ029_
    {
      get { return rm.GetString("FZ029_"); }
    }

    public static string FZ030
    {
      get { return rm.GetString("FZ030"); }
    }

    public static string FZ031
    {
      get { return rm.GetString("FZ031"); }
    }

    public static string FZ032
    {
      get { return rm.GetString("FZ032"); }
    }

    public static string FZ033
    {
      get { return rm.GetString("FZ033"); }
    }

    public static string FZ034
    {
      get { return rm.GetString("FZ034"); }
    }

    public static string FZ035
    {
      get { return rm.GetString("FZ035"); }
    }

    public static string FZ036_
    {
      get { return rm.GetString("FZ036_"); }
    }

    public static string FZ037
    {
      get { return rm.GetString("FZ037"); }
    }

    public static string FZ038
    {
      get { return rm.GetString("FZ038"); }
    }

    public static string FZ039
    {
      get { return rm.GetString("FZ039"); }
    }

    public static string FZ040
    {
      get { return rm.GetString("FZ040"); }
    }

    public static string FZ041
    {
      get { return rm.GetString("FZ041"); }
    }

    public static string FZ042
    {
      get { return rm.GetString("FZ042"); }
    }

    public static string FZ043
    {
      get { return rm.GetString("FZ043"); }
    }

    public static string FZ044
    {
      get { return rm.GetString("FZ044"); }
    }

    public static string FZ045
    {
      get { return rm.GetString("FZ045"); }
    }

    public static string HH001
    {
      get { return rm.GetString("HH001"); }
    }

    public static string HH002
    {
      get { return rm.GetString("HH002"); }
    }

    public static string HH003
    {
      get { return rm.GetString("HH003"); }
    }

    public static string HH004
    {
      get { return rm.GetString("HH004"); }
    }

    public static string HH005_
    {
      get { return rm.GetString("HH005_"); }
    }

    public static string HH006_
    {
      get { return rm.GetString("HH006_"); }
    }

    public static string HH007
    {
      get { return rm.GetString("HH007"); }
    }

    public static string HH008_
    {
      get { return rm.GetString("HH008_"); }
    }

    public static string HH009_
    {
      get { return rm.GetString("HH009_"); }
    }

    public static string HH010_
    {
      get { return rm.GetString("HH010_"); }
    }

    public static string HH011
    {
      get { return rm.GetString("HH011"); }
    }

    public static string HH012
    {
      get { return rm.GetString("HH012"); }
    }

    public static string HH013_
    {
      get { return rm.GetString("HH013_"); }
    }

    public static string HH014
    {
      get { return rm.GetString("HH014"); }
    }

    public static string HH015
    {
      get { return rm.GetString("HH015"); }
    }

    public static string HH016
    {
      get { return rm.GetString("HH016"); }
    }

    public static string HH017_
    {
      get { return rm.GetString("HH017_"); }
    }

    public static string HH018
    {
      get { return rm.GetString("HH018"); }
    }

    public static string HH019_
    {
      get { return rm.GetString("HH019_"); }
    }

    public static string HH020
    {
      get { return rm.GetString("HH020"); }
    }

    public static string HH021
    {
      get { return rm.GetString("HH021"); }
    }

    public static string HH022
    {
      get { return rm.GetString("HH022"); }
    }

    public static string HH023_
    {
      get { return rm.GetString("HH023_"); }
    }

    public static string HH024
    {
      get { return rm.GetString("HH024"); }
    }

    public static string HH025_
    {
      get { return rm.GetString("HH025_"); }
    }

    public static string HH026_
    {
      get { return rm.GetString("HH026_"); }
    }

    public static string HH027
    {
      get { return rm.GetString("HH027"); }
    }

    public static string HH028
    {
      get { return rm.GetString("HH028"); }
    }

    public static string HH029
    {
      get { return rm.GetString("HH029"); }
    }

    public static string HH030
    {
      get { return rm.GetString("HH030"); }
    }

    public static string HH031
    {
      get { return rm.GetString("HH031"); }
    }

    public static string HH032
    {
      get { return rm.GetString("HH032"); }
    }

    public static string HH033
    {
      get { return rm.GetString("HH033"); }
    }

    public static string HH034
    {
      get { return rm.GetString("HH034"); }
    }

    public static string HH035
    {
      get { return rm.GetString("HH035"); }
    }

    public static string HH036
    {
      get { return rm.GetString("HH036"); }
    }

    public static string HH037
    {
      get { return rm.GetString("HH037"); }
    }

    public static string HH038_
    {
      get { return rm.GetString("HH038_"); }
    }

    public static string HH039_
    {
      get { return rm.GetString("HH039_"); }
    }

    public static string HH040
    {
      get { return rm.GetString("HH040"); }
    }

    public static string HH041_
    {
      get { return rm.GetString("HH041_"); }
    }

    public static string HH042_
    {
      get { return rm.GetString("HH042_"); }
    }

    public static string HH043_
    {
      get { return rm.GetString("HH043_"); }
    }

    public static string HH045
    {
      get { return rm.GetString("HH045"); }
    }

    public static string HH046_
    {
      get { return rm.GetString("HH046_"); }
    }

    public static string HH047_
    {
      get { return rm.GetString("HH047_"); }
    }

    public static string HH048
    {
      get { return rm.GetString("HH048"); }
    }

    public static string HH049
    {
      get { return rm.GetString("HH049"); }
    }

    public static string HH050
    {
      get { return rm.GetString("HH050"); }
    }

    public static string HH051_
    {
      get { return rm.GetString("HH051_"); }
    }

    public static string HH052
    {
      get { return rm.GetString("HH052"); }
    }

    public static string HH053_
    {
      get { return rm.GetString("HH053_"); }
    }

    public static string HH054_
    {
      get { return rm.GetString("HH054_"); }
    }

    public static string HH057_
    {
      get { return rm.GetString("HH057_"); }
    }

    public static string HH058_
    {
      get { return rm.GetString("HH058_"); }
    }

    public static string HH059_
    {
      get { return rm.GetString("HH059_"); }
    }

    public static string HH060
    {
      get { return rm.GetString("HH060"); }
    }

    public static string HH061_
    {
      get { return rm.GetString("HH061_"); }
    }

    public static string HH063_
    {
      get { return rm.GetString("HH063_"); }
    }

    public static string HH064
    {
      get { return rm.GetString("HH064"); }
    }

    public static string HH065
    {
      get { return rm.GetString("HH065"); }
    }

    public static string HH066
    {
      get { return rm.GetString("HH066"); }
    }

    public static string HH067
    {
      get { return rm.GetString("HH067"); }
    }

    public static string HH068
    {
      get { return rm.GetString("HH068"); }
    }

    public static string HH069
    {
      get { return rm.GetString("HH069"); }
    }

    public static string HH070_
    {
      get { return rm.GetString("HH070_"); }
    }

    public static string HH071_
    {
      get { return rm.GetString("HH071_"); }
    }

    public static string HH072_
    {
      get { return rm.GetString("HH072_"); }
    }

    public static string HH073_
    {
      get { return rm.GetString("HH073_"); }
    }

    public static string HH074
    {
      get { return rm.GetString("HH074"); }
    }

    public static string HH076
    {
      get { return rm.GetString("HH076"); }
    }

    public static string HH077
    {
      get { return rm.GetString("HH077"); }
    }

    public static string HH078
    {
      get { return rm.GetString("HH078"); }
    }

    public static string HH079
    {
      get { return rm.GetString("HH079"); }
    }

    public static string HH080
    {
      get { return rm.GetString("HH080"); }
    }

    public static string HH081
    {
      get { return rm.GetString("HH081"); }
    }

    public static string HH082
    {
      get { return rm.GetString("HH082"); }
    }

    public static string HH083
    {
      get { return rm.GetString("HH083"); }
    }

    public static string SB001
    {
      get { return rm.GetString("SB001"); }
    }

    public static string SB002
    {
      get { return rm.GetString("SB002"); }
    }

    public static string SB003_
    {
      get { return rm.GetString("SB003_"); }
    }

    public static string SB004
    {
      get { return rm.GetString("SB004"); }
    }

    public static string SB005
    {
      get { return rm.GetString("SB005"); }
    }

    public static string SB006
    {
      get { return rm.GetString("SB006"); }
    }

    public static string SB007
    {
      get { return rm.GetString("SB007"); }
    }

    public static string SB008
    {
      get { return rm.GetString("SB008"); }
    }

    public static string SB009
    {
      get { return rm.GetString("SB009"); }
    }

    public static string SB010_
    {
      get { return rm.GetString("SB010_"); }
    }

    public static string SB011
    {
      get { return rm.GetString("SB011"); }
    }

    public static string SB012_
    {
      get { return rm.GetString("SB012_"); }
    }

    public static string SB013
    {
      get { return rm.GetString("SB013"); }
    }

    public static string SB014
    {
      get { return rm.GetString("SB014"); }
    }

    public static string SB015
    {
      get { return rm.GetString("SB015"); }
    }

    public static string SB016
    {
      get { return rm.GetString("SB016"); }
    }

    public static string SB017_
    {
      get { return rm.GetString("SB017_"); }
    }

    public static string SB018_
    {
      get { return rm.GetString("SB018_"); }
    }

    public static string SB019_
    {
      get { return rm.GetString("SB019_"); }
    }

    public static string SB021_
    {
      get { return rm.GetString("SB021_"); }
    }

    public static string SB022
    {
      get { return rm.GetString("SB022"); }
    }

    public static string SB023
    {
      get { return rm.GetString("SB023"); }
    }

    public static string SB024
    {
      get { return rm.GetString("SB024"); }
    }

    public static string SB025
    {
      get { return rm.GetString("SB025"); }
    }

    public static string SB026_
    {
      get { return rm.GetString("SB026_"); }
    }

    public static string SB027
    {
      get { return rm.GetString("SB027"); }
    }

    public static string SB028_
    {
      get { return rm.GetString("SB028_"); }
    }

    public static string SB029
    {
      get { return rm.GetString("SB029"); }
    }

    public static string SB030
    {
      get { return rm.GetString("SB030"); }
    }

    public static string SB031_
    {
      get { return rm.GetString("SB031_"); }
    }

    public static string SB032_
    {
      get { return rm.GetString("SB032_"); }
    }

    public static string SB033_
    {
      get { return rm.GetString("SB033_"); }
    }

    public static string SB034_
    {
      get { return rm.GetString("SB034_"); }
    }

    public static string SB035_
    {
      get { return rm.GetString("SB035_"); }
    }

    public static string SO002_
    {
      get { return rm.GetString("SO002_"); }
    }

    public static string SO007_
    {
      get { return rm.GetString("SO007_"); }
    }

    public static string SO008_
    {
      get { return rm.GetString("SO008_"); }
    }

    public static string SO009_
    {
      get { return rm.GetString("SO009_"); }
    }

    public static string SO010_
    {
      get { return rm.GetString("SO010_"); }
    }

    public static string SO011
    {
      get { return rm.GetString("SO011"); }
    }

    public static string SO012
    {
      get { return rm.GetString("SO012"); }
    }

    public static string SO013
    {
      get { return rm.GetString("SO013"); }
    }

    public static string SO014
    {
      get { return rm.GetString("SO014"); }
    }

    public static string SO015
    {
      get { return rm.GetString("SO015"); }
    }

    public static string TB001
    {
      get { return rm.GetString("TB001"); }
    }

    public static string TB002_
    {
      get { return rm.GetString("TB002_"); }
    }

    public static string TB003_
    {
      get { return rm.GetString("TB003_"); }
    }

    public static string TB004_
    {
      get { return rm.GetString("TB004_"); }
    }

    public static string TB005
    {
      get { return rm.GetString("TB005"); }
    }

    public static string TB006_
    {
      get { return rm.GetString("TB006_"); }
    }

    public static string TB007
    {
      get { return rm.GetString("TB007"); }
    }

    public static string TB008
    {
      get { return rm.GetString("TB008"); }
    }

    public static string TB009
    {
      get { return rm.GetString("TB009"); }
    }

    public static string TB010_
    {
      get { return rm.GetString("TB010_"); }
    }

    public static string TB011_
    {
      get { return rm.GetString("TB011_"); }
    }

    public static string TB012
    {
      get { return rm.GetString("TB012"); }
    }

    public static string TB013_
    {
      get { return rm.GetString("TB013_"); }
    }

    public static string WP001
    {
      get { return rm.GetString("WP001"); }
    }

    public static string WP002
    {
      get { return rm.GetString("WP002"); }
    }

    public static string WP003
    {
      get { return rm.GetString("WP003"); }
    }

    public static string WP004
    {
      get { return rm.GetString("WP004"); }
    }

    public static string WP005
    {
      get { return rm.GetString("WP005"); }
    }

    public static string WP006
    {
      get { return rm.GetString("WP006"); }
    }

    public static string WP007
    {
      get { return rm.GetString("WP007"); }
    }

    public static string WP008_
    {
      get { return rm.GetString("WP008_"); }
    }

    public static string WP009_
    {
      get { return rm.GetString("WP009_"); }
    }

    public static string WP010
    {
      get { return rm.GetString("WP010"); }
    }

    public static string WP012_
    {
      get { return rm.GetString("WP012_"); }
    }

    public static string WP014
    {
      get { return rm.GetString("WP014"); }
    }

    public static string WP015
    {
      get { return rm.GetString("WP015"); }
    }

    public static string WP016
    {
      get { return rm.GetString("WP016"); }
    }

    public static string WP017
    {
      get { return rm.GetString("WP017"); }
    }

    public static string WP018
    {
      get { return rm.GetString("WP018"); }
    }

    public static string WP019
    {
      get { return rm.GetString("WP019"); }
    }

    public static string WP021
    {
      get { return rm.GetString("WP021"); }
    }

    public static string WP022
    {
      get { return rm.GetString("WP022"); }
    }

    public static string WP023_
    {
      get { return rm.GetString("WP023_"); }
    }

    public static string WP024_
    {
      get { return rm.GetString("WP024_"); }
    }

    public static string WP025_
    {
      get { return rm.GetString("WP025_"); }
    }

    public static string WP026_
    {
      get { return rm.GetString("WP026_"); }
    }

    public static string WP028_
    {
      get { return rm.GetString("WP028_"); }
    }

    public static string WP029_
    {
      get { return rm.GetString("WP029_"); }
    }

    public static string WP030_
    {
      get { return rm.GetString("WP030_"); }
    }

    public static string WP031
    {
      get { return rm.GetString("WP031"); }
    }

    public static string WP032
    {
      get { return rm.GetString("WP032"); }
    }

    public static string WP033
    {
      get { return rm.GetString("WP033"); }
    }

    public static string WP034
    {
      get { return rm.GetString("WP034"); }
    }

    public static string WP035
    {
      get { return rm.GetString("WP035"); }
    }

    public static string WP039_
    {
      get { return rm.GetString("WP039_"); }
    }

    public static string WP045
    {
      get { return rm.GetString("WP045"); }
    }

    public static string WP046_
    {
      get { return rm.GetString("WP046_"); }
    }

    public static string WP047_
    {
      get { return rm.GetString("WP047_"); }
    }

    public static string WP048_
    {
      get { return rm.GetString("WP048_"); }
    }

    public static string WP049
    {
      get { return rm.GetString("WP049"); }
    }

    public static string WP050_
    {
      get { return rm.GetString("WP050_"); }
    }

    public static string WP051_
    {
      get { return rm.GetString("WP051_"); }
    }

    public static string WP052
    {
      get { return rm.GetString("WP052"); }
    }

    public static string WP053
    {
      get { return rm.GetString("WP053"); }
    }

    public static string WP054_
    {
      get { return rm.GetString("WP054_"); }
    }

    public static string WP055
    {
      get { return rm.GetString("WP055"); }
    }

    public static string WP056_
    {
      get { return rm.GetString("WP056_"); }
    }

    public static string M0000
    {
      get { return rm.GetString("M0000"); }
    }

    public static string M1000
    {
      get { return rm.GetString("M1000"); }
    }

    public static string M1001_
    {
      get { return rm.GetString("M1001_"); }
    }

    public static string M1011_
    {
      get { return rm.GetString("M1011_"); }
    }

    public static string M1012
    {
      get { return rm.GetString("M1012"); }
    }

    public static string M1013
    {
      get { return rm.GetString("M1013"); }
    }

    public static string M1019_
    {
      get { return rm.GetString("M1019_"); }
    }

    public static string M1020_
    {
      get { return rm.GetString("M1020_"); }
    }

    public static string M1031
    {
      get { return rm.GetString("M1031"); }
    }

    public static string M1032
    {
      get { return rm.GetString("M1032"); }
    }

    public static string M1033_
    {
      get { return rm.GetString("M1033_"); }
    }

    public static string M1034_
    {
      get { return rm.GetString("M1034_"); }
    }

    public static string M1035_
    {
      get { return rm.GetString("M1035_"); }
    }

    public static string M1038_
    {
      get { return rm.GetString("M1038_"); }
    }

    public static string M1057
    {
      get { return rm.GetString("M1057"); }
    }

    public static string M2023
    {
      get { return rm.GetString("M2023"); }
    }

    public static string M2024
    {
      get { return rm.GetString("M2024"); }
    }

    public static string M2096
    {
      get { return rm.GetString("M2096"); }
    }

    public static string M2097
    {
      get { return rm.GetString("M2097"); }
    }

    public static string Main_title
    {
      get { return rm.GetString("Main.title"); }
    }

    public static string Menu_file
    {
      get { return rm.GetString("Menu.file"); }
    }

    public static string Menu_undo
    {
      get { return rm.GetString("Menu.undo"); }
    }

    public static string Menu_redo
    {
      get { return rm.GetString("Menu.redo"); }
    }

    public static string Menu_clients
    {
      get { return rm.GetString("Menu.clients"); }
    }

    public static string Menu_users
    {
      get { return rm.GetString("Menu.users"); }
    }

    public static string Menu_backups
    {
      get { return rm.GetString("Menu.backups"); }
    }

    public static string Menu_quit
    {
      get { return rm.GetString("Menu.quit"); }
    }

    public static string Menu_user
    {
      get { return rm.GetString("Menu.user"); }
    }

    public static string Menu_login
    {
      get { return rm.GetString("Menu.login"); }
    }

    public static string Menu_logout
    {
      get { return rm.GetString("Menu.logout"); }
    }

    public static string Menu_pwchange
    {
      get { return rm.GetString("Menu.pwchange"); }
    }

    public static string Menu_options
    {
      get { return rm.GetString("Menu.options"); }
    }

    public static string Menu_dialogs
    {
      get { return rm.GetString("Menu.dialogs"); }
    }

    public static string Menu_reset
    {
      get { return rm.GetString("Menu.reset"); }
    }

    public static string Menu_private1
    {
      get { return rm.GetString("Menu.private1"); }
    }

    public static string Menu_diary
    {
      get { return rm.GetString("Menu.diary"); }
    }

    public static string Menu_positions
    {
      get { return rm.GetString("Menu.positions"); }
    }

    public static string Menu_notes
    {
      get { return rm.GetString("Menu.notes"); }
    }

    public static string Menu_persons
    {
      get { return rm.GetString("Menu.persons"); }
    }

    public static string Menu_mileages
    {
      get { return rm.GetString("Menu.mileages"); }
    }

    public static string Menu_bikes
    {
      get { return rm.GetString("Menu.bikes"); }
    }

    public static string Menu_books
    {
      get { return rm.GetString("Menu.books"); }
    }

    public static string Menu_authors
    {
      get { return rm.GetString("Menu.authors"); }
    }

    public static string Menu_series
    {
      get { return rm.GetString("Menu.series"); }
    }

    public static string Menu_statistics
    {
      get { return rm.GetString("Menu.statistics"); }
    }

    public static string Menu_sudoku
    {
      get { return rm.GetString("Menu.sudoku"); }
    }

    public static string Menu_household2
    {
      get { return rm.GetString("Menu.household2"); }
    }

    public static string Menu_bookings
    {
      get { return rm.GetString("Menu.bookings"); }
    }

    public static string Menu_events
    {
      get { return rm.GetString("Menu.events"); }
    }

    public static string Menu_accounts
    {
      get { return rm.GetString("Menu.accounts"); }
    }

    public static string Menu_periods
    {
      get { return rm.GetString("Menu.periods"); }
    }

    public static string Menu_finalbalance
    {
      get { return rm.GetString("Menu.finalbalance"); }
    }

    public static string Menu_plbalance
    {
      get { return rm.GetString("Menu.plbalance"); }
    }

    public static string Menu_openingbalance
    {
      get { return rm.GetString("Menu.openingbalance"); }
    }

    public static string Menu_pedigree5
    {
      get { return rm.GetString("Menu.pedigree5"); }
    }

    public static string Menu_ancestors
    {
      get { return rm.GetString("Menu.ancestors"); }
    }

    public static string Menu_families
    {
      get { return rm.GetString("Menu.families"); }
    }

    public static string Menu_sources
    {
      get { return rm.GetString("Menu.sources"); }
    }

    public static string Menu_stock6
    {
      get { return rm.GetString("Menu.stock6"); }
    }

    public static string Menu_stocks
    {
      get { return rm.GetString("Menu.stocks"); }
    }

    public static string Menu_configurations
    {
      get { return rm.GetString("Menu.configurations"); }
    }

    public static string Menu_investments
    {
      get { return rm.GetString("Menu.investments"); }
    }

    public static string Menu_bookings3
    {
      get { return rm.GetString("Menu.bookings3"); }
    }

    public static string Menu_prices
    {
      get { return rm.GetString("Menu.prices"); }
    }

    public static string Menu_chart
    {
      get { return rm.GetString("Menu.chart"); }
    }

    public static string Menu_help
    {
      get { return rm.GetString("Menu.help"); }
    }

    public static string Menu_about
    {
      get { return rm.GetString("Menu.about"); }
    }

    public static string Menu_help2
    {
      get { return rm.GetString("Menu.help2"); }
    }

    public static string Menu_table_addrow
    {
      get { return rm.GetString("Menu.table.addrow"); }
    }

    public static string Menu_table_addrow2
    {
      get { return rm.GetString("Menu.table.addrow2"); }
    }

    public static string Menu_table_delrow
    {
      get { return rm.GetString("Menu.table.delrow"); }
    }

    public static string Menu_table_addcol
    {
      get { return rm.GetString("Menu.table.addcol"); }
    }

    public static string Menu_table_addcol2
    {
      get { return rm.GetString("Menu.table.addcol2"); }
    }

    public static string Menu_table_delcol
    {
      get { return rm.GetString("Menu.table.delcol"); }
    }

    public static string Menu_table_bold
    {
      get { return rm.GetString("Menu.table.bold"); }
    }

    public static string Menu_table_normal
    {
      get { return rm.GetString("Menu.table.normal"); }
    }

    public static string Menu_table_copy
    {
      get { return rm.GetString("Menu.table.copy"); }
    }

    public static string Menu_table_print
    {
      get { return rm.GetString("Menu.table.print"); }
    }

    public static string Enum_dialog_new
    {
      get { return rm.GetString("Enum.dialog.new"); }
    }

    public static string Enum_dialog_copy
    {
      get { return rm.GetString("Enum.dialog.copy"); }
    }

    public static string Enum_dialog_copy2
    {
      get { return rm.GetString("Enum.dialog.copy2"); }
    }

    public static string Enum_dialog_edit
    {
      get { return rm.GetString("Enum.dialog.edit"); }
    }

    public static string Enum_dialog_delete
    {
      get { return rm.GetString("Enum.dialog.delete"); }
    }

    public static string Enum_dialog_reverse
    {
      get { return rm.GetString("Enum.dialog.reverse"); }
    }

    public static string Enum_permission_no
    {
      get { return rm.GetString("Enum.permission.no"); }
    }

    public static string Enum_permission_user
    {
      get { return rm.GetString("Enum.permission.user"); }
    }

    public static string Enum_permission_admin
    {
      get { return rm.GetString("Enum.permission.admin"); }
    }

    public static string Enum_permission_all
    {
      get { return rm.GetString("Enum.permission.all"); }
    }

    public static string Enum_bike_tour
    {
      get { return rm.GetString("Enum.bike.tour"); }
    }

    public static string Enum_bike_weekly
    {
      get { return rm.GetString("Enum.bike.weekly"); }
    }

    public static string Enum_language_german
    {
      get { return rm.GetString("Enum.language.german"); }
    }

    public static string Enum_language_english
    {
      get { return rm.GetString("Enum.language.english"); }
    }

    public static string Enum_language_french
    {
      get { return rm.GetString("Enum.language.french"); }
    }

    public static string Enum_language_other
    {
      get { return rm.GetString("Enum.language.other"); }
    }

    public static string Enum_state_active
    {
      get { return rm.GetString("Enum.state.active"); }
    }

    public static string Enum_state_inactive
    {
      get { return rm.GetString("Enum.state.inactive"); }
    }

    public static string Enum_state_nocalc
    {
      get { return rm.GetString("Enum.state.nocalc"); }
    }

    public static string Enum_scale_fix
    {
      get { return rm.GetString("Enum.scale.fix"); }
    }

    public static string Enum_scale_pc
    {
      get { return rm.GetString("Enum.scale.pc"); }
    }

    public static string Enum_scale_dyn
    {
      get { return rm.GetString("Enum.scale.dyn"); }
    }

    public static string Enum_method_hl
    {
      get { return rm.GetString("Enum.method.hl"); }
    }

    public static string Enum_method_hlr
    {
      get { return rm.GetString("Enum.method.hlr"); }
    }

    public static string Enum_method_ohlc
    {
      get { return rm.GetString("Enum.method.ohlc"); }
    }

    public static string Enum_method_tp
    {
      get { return rm.GetString("Enum.method.tp"); }
    }

    public static string Enum_method_c
    {
      get { return rm.GetString("Enum.method.c"); }
    }

    public static string Action_copy
    {
      get { return rm.GetString("Action.copy"); }
    }

    public static string Action_paste
    {
      get { return rm.GetString("Action.paste"); }
    }

    public static string Action_undo
    {
      get { return rm.GetString("Action.undo"); }
    }

    public static string Action_redo
    {
      get { return rm.GetString("Action.redo"); }
    }

    public static string Action_save
    {
      get { return rm.GetString("Action.save"); }
    }

    public static string Action_refresh
    {
      get { return rm.GetString("Action.refresh"); }
    }

    public static string Action_new
    {
      get { return rm.GetString("Action.new"); }
    }

    public static string Action_edit
    {
      get { return rm.GetString("Action.edit"); }
    }

    public static string Action_delete
    {
      get { return rm.GetString("Action.delete"); }
    }

    public static string Action_chart
    {
      get { return rm.GetString("Action.chart"); }
    }

    public static string Action_floppy
    {
      get { return rm.GetString("Action.floppy"); }
    }

    public static string Action_details
    {
      get { return rm.GetString("Action.details"); }
    }

    public static string Forms_ok
    {
      get { return rm.GetString("Forms.ok"); }
    }

    public static string Forms_ok_tt
    {
      get { return rm.GetString("Forms.ok.tt"); }
    }

    public static string Forms_oknew
    {
      get { return rm.GetString("Forms.oknew"); }
    }

    public static string Forms_cancel
    {
      get { return rm.GetString("Forms.cancel"); }
    }

    public static string Forms_cancel_tt
    {
      get { return rm.GetString("Forms.cancel.tt"); }
    }

    public static string Forms_delete
    {
      get { return rm.GetString("Forms.delete"); }
    }

    public static string Forms_reverse
    {
      get { return rm.GetString("Forms.reverse"); }
    }

    public static string Forms_unreverse
    {
      get { return rm.GetString("Forms.unreverse"); }
    }

    public static string Forms_created
    {
      get { return rm.GetString("Forms.created"); }
    }

    public static string Forms_created_tt
    {
      get { return rm.GetString("Forms.created.tt"); }
    }

    public static string Forms_changed
    {
      get { return rm.GetString("Forms.changed"); }
    }

    public static string Forms_changed_tt
    {
      get { return rm.GetString("Forms.changed.tt"); }
    }

    public static string Forms_selectfile
    {
      get { return rm.GetString("Forms.selectfile"); }
    }

    public static string Forms_select
    {
      get { return rm.GetString("Forms.select"); }
    }

    public static string Date_unknown
    {
      get { return rm.GetString("Date.unknown"); }
    }

    public static string Date_unknown_tt
    {
      get { return rm.GetString("Date.unknown.tt"); }
    }

    public static string Date_yesterday
    {
      get { return rm.GetString("Date.yesterday"); }
    }

    public static string Date_yesterday_tt
    {
      get { return rm.GetString("Date.yesterday.tt"); }
    }

    public static string Date_today
    {
      get { return rm.GetString("Date.today"); }
    }

    public static string Date_today_tt
    {
      get { return rm.GetString("Date.today.tt"); }
    }

    public static string Date_tomorrow
    {
      get { return rm.GetString("Date.tomorrow"); }
    }

    public static string Date_tomorrow_tt
    {
      get { return rm.GetString("Date.tomorrow.tt"); }
    }

    public static string AD100_title
    {
      get { return rm.GetString("AD100.title"); }
    }

    public static string AD100_personen
    {
      get { return rm.GetString("AD100.personen"); }
    }

    public static string AD100_personen_tt
    {
      get { return rm.GetString("AD100.personen.tt"); }
    }

    public static string AD100_personen_columns
    {
      get { return rm.GetString("AD100.personen.columns"); }
    }

    public static string AD100_suche
    {
      get { return rm.GetString("AD100.suche"); }
    }

    public static string AD100_suche_tt
    {
      get { return rm.GetString("AD100.suche.tt"); }
    }

    public static string AD100_name
    {
      get { return rm.GetString("AD100.name"); }
    }

    public static string AD100_name_tt
    {
      get { return rm.GetString("AD100.name.tt"); }
    }

    public static string AD100_vorname
    {
      get { return rm.GetString("AD100.vorname"); }
    }

    public static string AD100_vorname_tt
    {
      get { return rm.GetString("AD100.vorname.tt"); }
    }

    public static string AD100_alle
    {
      get { return rm.GetString("AD100.alle"); }
    }

    public static string AD100_alle_tt
    {
      get { return rm.GetString("AD100.alle.tt"); }
    }

    public static string AD100_sitzNeu
    {
      get { return rm.GetString("AD100.sitzNeu"); }
    }

    public static string AD100_sitzNeu_tt
    {
      get { return rm.GetString("AD100.sitzNeu.tt"); }
    }

    public static string AD100_sitzEins
    {
      get { return rm.GetString("AD100.sitzEins"); }
    }

    public static string AD100_sitzEins_tt
    {
      get { return rm.GetString("AD100.sitzEins.tt"); }
    }

    public static string AD100_gebListe
    {
      get { return rm.GetString("AD100.gebListe"); }
    }

    public static string AD100_gebListe_tt
    {
      get { return rm.GetString("AD100.gebListe.tt"); }
    }

    public static string AD110_title
    {
      get { return rm.GetString("AD110.title"); }
    }

    public static string AD110_nr
    {
      get { return rm.GetString("AD110.nr"); }
    }

    public static string AD110_nr_tt
    {
      get { return rm.GetString("AD110.nr.tt"); }
    }

    public static string AD110_sitzNr_tt
    {
      get { return rm.GetString("AD110.sitzNr.tt"); }
    }

    public static string AD110_adressNr_tt
    {
      get { return rm.GetString("AD110.adressNr.tt"); }
    }

    public static string AD110_titel
    {
      get { return rm.GetString("AD110.titel"); }
    }

    public static string AD110_titel_tt
    {
      get { return rm.GetString("AD110.titel.tt"); }
    }

    public static string AD110_vorname
    {
      get { return rm.GetString("AD110.vorname"); }
    }

    public static string AD110_vorname_tt
    {
      get { return rm.GetString("AD110.vorname.tt"); }
    }

    public static string AD110_praedikat
    {
      get { return rm.GetString("AD110.praedikat"); }
    }

    public static string AD110_praedikat_tt
    {
      get { return rm.GetString("AD110.praedikat.tt"); }
    }

    public static string AD110_name1
    {
      get { return rm.GetString("AD110.name1"); }
    }

    public static string AD110_name1_tt
    {
      get { return rm.GetString("AD110.name1.tt"); }
    }

    public static string AD110_name2
    {
      get { return rm.GetString("AD110.name2"); }
    }

    public static string AD110_name2_tt
    {
      get { return rm.GetString("AD110.name2.tt"); }
    }

    public static string AD110_geschlecht
    {
      get { return rm.GetString("AD110.geschlecht"); }
    }

    public static string AD110_neutrum
    {
      get { return rm.GetString("AD110.neutrum"); }
    }

    public static string AD110_neutrum_tt
    {
      get { return rm.GetString("AD110.neutrum.tt"); }
    }

    public static string AD110_mann
    {
      get { return rm.GetString("AD110.mann"); }
    }

    public static string AD110_mann_tt
    {
      get { return rm.GetString("AD110.mann.tt"); }
    }

    public static string AD110_frau
    {
      get { return rm.GetString("AD110.frau"); }
    }

    public static string AD110_frau_tt
    {
      get { return rm.GetString("AD110.frau.tt"); }
    }

    public static string AD110_geburt
    {
      get { return rm.GetString("AD110.geburt"); }
    }

    public static string AD110_geburt_tt
    {
      get { return rm.GetString("AD110.geburt.tt"); }
    }

    public static string AD110_personStatus
    {
      get { return rm.GetString("AD110.personStatus"); }
    }

    public static string AD110_personAktuell
    {
      get { return rm.GetString("AD110.personAktuell"); }
    }

    public static string AD110_personAktuell_tt
    {
      get { return rm.GetString("AD110.personAktuell.tt"); }
    }

    public static string AD110_personAlt
    {
      get { return rm.GetString("AD110.personAlt"); }
    }

    public static string AD110_personAlt_tt
    {
      get { return rm.GetString("AD110.personAlt.tt"); }
    }

    public static string AD110_name
    {
      get { return rm.GetString("AD110.name"); }
    }

    public static string AD110_name_tt
    {
      get { return rm.GetString("AD110.name.tt"); }
    }

    public static string AD110_strasse
    {
      get { return rm.GetString("AD110.strasse"); }
    }

    public static string AD110_strasse_tt
    {
      get { return rm.GetString("AD110.strasse.tt"); }
    }

    public static string AD110_hausnr
    {
      get { return rm.GetString("AD110.hausnr"); }
    }

    public static string AD110_hausnr_tt
    {
      get { return rm.GetString("AD110.hausnr.tt"); }
    }

    public static string AD110_postfach
    {
      get { return rm.GetString("AD110.postfach"); }
    }

    public static string AD110_postfach_tt
    {
      get { return rm.GetString("AD110.postfach.tt"); }
    }

    public static string AD110_staat
    {
      get { return rm.GetString("AD110.staat"); }
    }

    public static string AD110_staat_tt
    {
      get { return rm.GetString("AD110.staat.tt"); }
    }

    public static string AD110_plz
    {
      get { return rm.GetString("AD110.plz"); }
    }

    public static string AD110_plz_tt
    {
      get { return rm.GetString("AD110.plz.tt"); }
    }

    public static string AD110_ort
    {
      get { return rm.GetString("AD110.ort"); }
    }

    public static string AD110_ort_tt
    {
      get { return rm.GetString("AD110.ort.tt"); }
    }

    public static string AD110_telefon
    {
      get { return rm.GetString("AD110.telefon"); }
    }

    public static string AD110_telefon_tt
    {
      get { return rm.GetString("AD110.telefon.tt"); }
    }

    public static string AD110_fax
    {
      get { return rm.GetString("AD110.fax"); }
    }

    public static string AD110_fax_tt
    {
      get { return rm.GetString("AD110.fax.tt"); }
    }

    public static string AD110_mobil
    {
      get { return rm.GetString("AD110.mobil"); }
    }

    public static string AD110_mobil_tt
    {
      get { return rm.GetString("AD110.mobil.tt"); }
    }

    public static string AD110_homepage
    {
      get { return rm.GetString("AD110.homepage"); }
    }

    public static string AD110_homepage_tt
    {
      get { return rm.GetString("AD110.homepage.tt"); }
    }

    public static string AD110_email
    {
      get { return rm.GetString("AD110.email"); }
    }

    public static string AD110_email_tt
    {
      get { return rm.GetString("AD110.email.tt"); }
    }

    public static string AD110_notiz
    {
      get { return rm.GetString("AD110.notiz"); }
    }

    public static string AD110_notiz_tt
    {
      get { return rm.GetString("AD110.notiz.tt"); }
    }

    public static string AD110_sitzStatus
    {
      get { return rm.GetString("AD110.sitzStatus"); }
    }

    public static string AD110_sitzAktuell
    {
      get { return rm.GetString("AD110.sitzAktuell"); }
    }

    public static string AD110_sitzAktuell_tt
    {
      get { return rm.GetString("AD110.sitzAktuell.tt"); }
    }

    public static string AD110_sitzAlt
    {
      get { return rm.GetString("AD110.sitzAlt"); }
    }

    public static string AD110_sitzAlt_tt
    {
      get { return rm.GetString("AD110.sitzAlt.tt"); }
    }

    public static string AD110_adresseAnzahl
    {
      get { return rm.GetString("AD110.adresseAnzahl"); }
    }

    public static string AD110_adresseAnzahl_tt
    {
      get { return rm.GetString("AD110.adresseAnzahl.tt"); }
    }

    public static string AD110_adresseDupl
    {
      get { return rm.GetString("AD110.adresseDupl"); }
    }

    public static string AD110_adresseDupl_tt
    {
      get { return rm.GetString("AD110.adresseDupl.tt"); }
    }

    public static string AD110_adresseWechseln
    {
      get { return rm.GetString("AD110.adresseWechseln"); }
    }

    public static string AD110_adresseWechseln_tt
    {
      get { return rm.GetString("AD110.adresseWechseln.tt"); }
    }

    public static string AD120_title
    {
      get { return rm.GetString("AD120.title"); }
    }

    public static string AD120_datum
    {
      get { return rm.GetString("AD120.datum"); }
    }

    public static string AD120_datum_tt
    {
      get { return rm.GetString("AD120.datum.tt"); }
    }

    public static string AD120_tage
    {
      get { return rm.GetString("AD120.tage"); }
    }

    public static string AD120_tage_tt
    {
      get { return rm.GetString("AD120.tage.tt"); }
    }

    public static string AD120_geburtstage
    {
      get { return rm.GetString("AD120.geburtstage"); }
    }

    public static string AD120_geburtstage_tt
    {
      get { return rm.GetString("AD120.geburtstage.tt"); }
    }

    public static string AD120_starten
    {
      get { return rm.GetString("AD120.starten"); }
    }

    public static string AD120_starten_tt
    {
      get { return rm.GetString("AD120.starten.tt"); }
    }

    public static string AD130_title
    {
      get { return rm.GetString("AD130.title"); }
    }

    public static string AD130_adressen
    {
      get { return rm.GetString("AD130.adressen"); }
    }

    public static string AD130_adressen_tt
    {
      get { return rm.GetString("AD130.adressen.tt"); }
    }

    public static string AD130_adressen_columns
    {
      get { return rm.GetString("AD130.adressen.columns"); }
    }

    public static string AD200_title
    {
      get { return rm.GetString("AD200.title"); }
    }

    public static string AD200_loeschen
    {
      get { return rm.GetString("AD200.loeschen"); }
    }

    public static string AD200_loeschen_tt
    {
      get { return rm.GetString("AD200.loeschen.tt"); }
    }

    public static string AD200_datei
    {
      get { return rm.GetString("AD200.datei"); }
    }

    public static string AD200_datei_tt
    {
      get { return rm.GetString("AD200.datei.tt"); }
    }

    public static string AD200_dateiAuswahl
    {
      get { return rm.GetString("AD200.dateiAuswahl"); }
    }

    public static string AD200_dateiAuswahl_tt
    {
      get { return rm.GetString("AD200.dateiAuswahl.tt"); }
    }

    public static string AD200_export
    {
      get { return rm.GetString("AD200.export"); }
    }

    public static string AD200_export_tt
    {
      get { return rm.GetString("AD200.export.tt"); }
    }

    public static string AD200_importieren
    {
      get { return rm.GetString("AD200.importieren"); }
    }

    public static string AD200_importieren_tt
    {
      get { return rm.GetString("AD200.importieren.tt"); }
    }

    public static string AD200_select_file
    {
      get { return rm.GetString("AD200.select.file"); }
    }

    public static string AD200_select_ext
    {
      get { return rm.GetString("AD200.select.ext"); }
    }

    public static string AG100_title
    {
      get { return rm.GetString("AG100.title"); }
    }

    public static string AG100_mandanten
    {
      get { return rm.GetString("AG100.mandanten"); }
    }

    public static string AG100_mandanten_tt
    {
      get { return rm.GetString("AG100.mandanten.tt"); }
    }

    public static string AG100_mandanten_columns
    {
      get { return rm.GetString("AG100.mandanten.columns"); }
    }

    public static string AG110_title
    {
      get { return rm.GetString("AG110.title"); }
    }

    public static string AG110_nr
    {
      get { return rm.GetString("AG110.nr"); }
    }

    public static string AG110_nr_tt
    {
      get { return rm.GetString("AG110.nr.tt"); }
    }

    public static string AG110_beschreibung
    {
      get { return rm.GetString("AG110.beschreibung"); }
    }

    public static string AG110_beschreibung_tt
    {
      get { return rm.GetString("AG110.beschreibung.tt"); }
    }

    public static string AG200_title
    {
      get { return rm.GetString("AG200.title"); }
    }

    public static string AG200_benutzer
    {
      get { return rm.GetString("AG200.benutzer"); }
    }

    public static string AG200_benutzer_tt
    {
      get { return rm.GetString("AG200.benutzer.tt"); }
    }

    public static string AG200_benutzer_columns
    {
      get { return rm.GetString("AG200.benutzer.columns"); }
    }

    public static string AG210_title
    {
      get { return rm.GetString("AG210.title"); }
    }

    public static string AG210_nr
    {
      get { return rm.GetString("AG210.nr"); }
    }

    public static string AG210_nr_tt
    {
      get { return rm.GetString("AG210.nr.tt"); }
    }

    public static string AG210_benutzerId
    {
      get { return rm.GetString("AG210.benutzerId"); }
    }

    public static string AG210_benutzerId_tt
    {
      get { return rm.GetString("AG210.benutzerId.tt"); }
    }

    public static string AG210_kennwort
    {
      get { return rm.GetString("AG210.kennwort"); }
    }

    public static string AG210_kennwort_tt
    {
      get { return rm.GetString("AG210.kennwort.tt"); }
    }

    public static string AG210_berechtigung
    {
      get { return rm.GetString("AG210.berechtigung"); }
    }

    public static string AG210_berechtigung1
    {
      get { return rm.GetString("AG210.berechtigung1"); }
    }

    public static string AG210_berechtigung1_tt
    {
      get { return rm.GetString("AG210.berechtigung1.tt"); }
    }

    public static string AG210_berechtigung2
    {
      get { return rm.GetString("AG210.berechtigung2"); }
    }

    public static string AG210_berechtigung2_tt
    {
      get { return rm.GetString("AG210.berechtigung2.tt"); }
    }

    public static string AG210_berechtigung3
    {
      get { return rm.GetString("AG210.berechtigung3"); }
    }

    public static string AG210_berechtigung3_tt
    {
      get { return rm.GetString("AG210.berechtigung3.tt"); }
    }

    public static string AG210_geburt
    {
      get { return rm.GetString("AG210.geburt"); }
    }

    public static string AG210_geburt_tt
    {
      get { return rm.GetString("AG210.geburt.tt"); }
    }

    public static string AG400_title
    {
      get { return rm.GetString("AG400.title"); }
    }

    public static string AG400_verzeichnisse
    {
      get { return rm.GetString("AG400.verzeichnisse"); }
    }

    public static string AG400_verzeichnisse_tt
    {
      get { return rm.GetString("AG400.verzeichnisse.tt"); }
    }

    public static string AG400_verzeichnisse_columns
    {
      get { return rm.GetString("AG400.verzeichnisse.columns"); }
    }

    public static string AG400_sicherung
    {
      get { return rm.GetString("AG400.sicherung"); }
    }

    public static string AG400_sicherung_tt
    {
      get { return rm.GetString("AG400.sicherung.tt"); }
    }

    public static string AG400_diffSicherung
    {
      get { return rm.GetString("AG400.diffSicherung"); }
    }

    public static string AG400_diffSicherung_tt
    {
      get { return rm.GetString("AG400.diffSicherung.tt"); }
    }

    public static string AG400_rueckSicherung
    {
      get { return rm.GetString("AG400.rueckSicherung"); }
    }

    public static string AG400_rueckSicherung_tt
    {
      get { return rm.GetString("AG400.rueckSicherung.tt"); }
    }

    public static string AG400_sqlSicherung
    {
      get { return rm.GetString("AG400.sqlSicherung"); }
    }

    public static string AG400_sqlSicherung_tt
    {
      get { return rm.GetString("AG400.sqlSicherung.tt"); }
    }

    public static string AG400_status
    {
      get { return rm.GetString("AG400.status"); }
    }

    public static string AG400_statusText_tt
    {
      get { return rm.GetString("AG400.statusText.tt"); }
    }

    public static string AG400_mandantKopieren
    {
      get { return rm.GetString("AG400.mandantKopieren"); }
    }

    public static string AG400_mandantKopieren_tt
    {
      get { return rm.GetString("AG400.mandantKopieren.tt"); }
    }

    public static string AG400_mandantRepKopieren
    {
      get { return rm.GetString("AG400.mandantRepKopieren"); }
    }

    public static string AG400_mandantRepKopieren_tt
    {
      get { return rm.GetString("AG400.mandantRepKopieren.tt"); }
    }

    public static string AG400_mandant
    {
      get { return rm.GetString("AG400.mandant"); }
    }

    public static string AG400_mandant_tt
    {
      get { return rm.GetString("AG400.mandant.tt"); }
    }

    public static string AG410_title
    {
      get { return rm.GetString("AG410.title"); }
    }

    public static string AG410_nr
    {
      get { return rm.GetString("AG410.nr"); }
    }

    public static string AG410_nr_tt
    {
      get { return rm.GetString("AG410.nr.tt"); }
    }

    public static string AG410_ziel
    {
      get { return rm.GetString("AG410.ziel"); }
    }

    public static string AG410_ziel_tt
    {
      get { return rm.GetString("AG410.ziel.tt"); }
    }

    public static string AG410_encrypted
    {
      get { return rm.GetString("AG410.encrypted"); }
    }

    public static string AG410_encrypted_tt
    {
      get { return rm.GetString("AG410.encrypted.tt"); }
    }

    public static string AG410_zipped
    {
      get { return rm.GetString("AG410.zipped"); }
    }

    public static string AG410_zipped_tt
    {
      get { return rm.GetString("AG410.zipped.tt"); }
    }

    public static string AG410_quelle
    {
      get { return rm.GetString("AG410.quelle"); }
    }

    public static string AG410_quelle_tt
    {
      get { return rm.GetString("AG410.quelle.tt"); }
    }

    public static string AG420_title
    {
      get { return rm.GetString("AG420.title"); }
    }

    public static string AG420_target
    {
      get { return rm.GetString("AG420.target"); }
    }

    public static string AG420_target_tt
    {
      get { return rm.GetString("AG420.target.tt"); }
    }

    public static string AG420_password
    {
      get { return rm.GetString("AG420.password"); }
    }

    public static string AG420_password_tt
    {
      get { return rm.GetString("AG420.password.tt"); }
    }

    public static string AM000_title
    {
      get { return rm.GetString("AM000.title"); }
    }

    public static string AM000_client
    {
      get { return rm.GetString("AM000.client"); }
    }

    public static string AM000_client_tt
    {
      get { return rm.GetString("AM000.client.tt"); }
    }

    public static string AM000_user
    {
      get { return rm.GetString("AM000.user"); }
    }

    public static string AM000_user_tt
    {
      get { return rm.GetString("AM000.user.tt"); }
    }

    public static string AM000_password
    {
      get { return rm.GetString("AM000.password"); }
    }

    public static string AM000_password_tt
    {
      get { return rm.GetString("AM000.password.tt"); }
    }

    public static string AM000_save
    {
      get { return rm.GetString("AM000.save"); }
    }

    public static string AM000_save_tt
    {
      get { return rm.GetString("AM000.save.tt"); }
    }

    public static string AM000_login
    {
      get { return rm.GetString("AM000.login"); }
    }

    public static string AM000_login_tt
    {
      get { return rm.GetString("AM000.login.tt"); }
    }

    public static string AM000_reset
    {
      get { return rm.GetString("AM000.reset"); }
    }

    public static string AM000_reset_tt
    {
      get { return rm.GetString("AM000.reset.tt"); }
    }

    public static string AM000_cancel
    {
      get { return rm.GetString("AM000.cancel"); }
    }

    public static string AM000_cancel_tt
    {
      get { return rm.GetString("AM000.cancel.tt"); }
    }

    public static string AM100_title
    {
      get { return rm.GetString("AM100.title"); }
    }

    public static string AM100_mandant
    {
      get { return rm.GetString("AM100.mandant"); }
    }

    public static string AM100_mandant_tt
    {
      get { return rm.GetString("AM100.mandant.tt"); }
    }

    public static string AM100_benutzer
    {
      get { return rm.GetString("AM100.benutzer"); }
    }

    public static string AM100_benutzer_tt
    {
      get { return rm.GetString("AM100.benutzer.tt"); }
    }

    public static string AM100_kennwortAlt
    {
      get { return rm.GetString("AM100.kennwortAlt"); }
    }

    public static string AM100_kennwortAlt_tt
    {
      get { return rm.GetString("AM100.kennwortAlt.tt"); }
    }

    public static string AM100_kennwortNeu
    {
      get { return rm.GetString("AM100.kennwortNeu"); }
    }

    public static string AM100_kennwortNeu_tt
    {
      get { return rm.GetString("AM100.kennwortNeu.tt"); }
    }

    public static string AM100_kennwortNeu2
    {
      get { return rm.GetString("AM100.kennwortNeu2"); }
    }

    public static string AM100_kennwortNeu2_tt
    {
      get { return rm.GetString("AM100.kennwortNeu2.tt"); }
    }

    public static string AM100_speichern
    {
      get { return rm.GetString("AM100.speichern"); }
    }

    public static string AM100_speichern_tt
    {
      get { return rm.GetString("AM100.speichern.tt"); }
    }

    public static string AM500_title
    {
      get { return rm.GetString("AM500.title"); }
    }

    public static string AM500_einstellungen
    {
      get { return rm.GetString("AM500.einstellungen"); }
    }

    public static string AM500_einstellungen_tt
    {
      get { return rm.GetString("AM500.einstellungen.tt"); }
    }

    public static string AM500_einstellungen_columns
    {
      get { return rm.GetString("AM500.einstellungen.columns"); }
    }

    public static string AM510_title
    {
      get { return rm.GetString("AM510.title"); }
    }

    public static string AM510_dialoge
    {
      get { return rm.GetString("AM510.dialoge"); }
    }

    public static string AM510_dialoge_tt
    {
      get { return rm.GetString("AM510.dialoge.tt"); }
    }

    public static string AM510_dialoge_columns
    {
      get { return rm.GetString("AM510.dialoge.columns"); }
    }

    public static string AM510_zuordnen
    {
      get { return rm.GetString("AM510.zuordnen"); }
    }

    public static string AM510_zuordnen_tt
    {
      get { return rm.GetString("AM510.zuordnen.tt"); }
    }

    public static string AM510_entfernen
    {
      get { return rm.GetString("AM510.entfernen"); }
    }

    public static string AM510_entfernen_tt
    {
      get { return rm.GetString("AM510.entfernen.tt"); }
    }

    public static string AM510_zudialoge
    {
      get { return rm.GetString("AM510.zudialoge"); }
    }

    public static string AM510_zudialoge_tt
    {
      get { return rm.GetString("AM510.zudialoge.tt"); }
    }

    public static string AM510_zudialoge_columns
    {
      get { return rm.GetString("AM510.zudialoge.columns"); }
    }

    public static string AM510_oben
    {
      get { return rm.GetString("AM510.oben"); }
    }

    public static string AM510_oben_tt
    {
      get { return rm.GetString("AM510.oben.tt"); }
    }

    public static string AM510_unten
    {
      get { return rm.GetString("AM510.unten"); }
    }

    public static string AM510_unten_tt
    {
      get { return rm.GetString("AM510.unten.tt"); }
    }

    public static string FZ100_title
    {
      get { return rm.GetString("FZ100.title"); }
    }

    public static string FZ100_datum
    {
      get { return rm.GetString("FZ100.datum"); }
    }

    public static string FZ100_datum_tt
    {
      get { return rm.GetString("FZ100.datum.tt"); }
    }

    public static string FZ100_bilanz
    {
      get { return rm.GetString("FZ100.bilanz"); }
    }

    public static string FZ100_bilanz_tt
    {
      get { return rm.GetString("FZ100.bilanz.tt"); }
    }

    public static string FZ100_buecher
    {
      get { return rm.GetString("FZ100.buecher"); }
    }

    public static string FZ100_buecher_tt
    {
      get { return rm.GetString("FZ100.buecher.tt"); }
    }

    public static string FZ100_fahrrad
    {
      get { return rm.GetString("FZ100.fahrrad"); }
    }

    public static string FZ100_fahrrad_tt
    {
      get { return rm.GetString("FZ100.fahrrad.tt"); }
    }

    public static string FZ200_title
    {
      get { return rm.GetString("FZ200.title"); }
    }

    public static string FZ200_bikes
    {
      get { return rm.GetString("FZ200.bikes"); }
    }

    public static string FZ200_bikes_tt
    {
      get { return rm.GetString("FZ200.bikes.tt"); }
    }

    public static string FZ200_bikes_columns
    {
      get { return rm.GetString("FZ200.bikes.columns"); }
    }

    public static string FZ210_title
    {
      get { return rm.GetString("FZ210.title"); }
    }

    public static string FZ210_nr
    {
      get { return rm.GetString("FZ210.nr"); }
    }

    public static string FZ210_nr_tt
    {
      get { return rm.GetString("FZ210.nr.tt"); }
    }

    public static string FZ210_bezeichnung
    {
      get { return rm.GetString("FZ210.bezeichnung"); }
    }

    public static string FZ210_bezeichnung_tt
    {
      get { return rm.GetString("FZ210.bezeichnung.tt"); }
    }

    public static string FZ210_typ
    {
      get { return rm.GetString("FZ210.typ"); }
    }

    public static string FZ210_typ1
    {
      get { return rm.GetString("FZ210.typ1"); }
    }

    public static string FZ210_typ1_tt
    {
      get { return rm.GetString("FZ210.typ1.tt"); }
    }

    public static string FZ210_typ2
    {
      get { return rm.GetString("FZ210.typ2"); }
    }

    public static string FZ210_typ2_tt
    {
      get { return rm.GetString("FZ210.typ2.tt"); }
    }

    public static string FZ250_title
    {
      get { return rm.GetString("FZ250.title"); }
    }

    public static string FZ250_fahrradstaende
    {
      get { return rm.GetString("FZ250.fahrradstaende"); }
    }

    public static string FZ250_fahrradstaende_tt
    {
      get { return rm.GetString("FZ250.fahrradstaende.tt"); }
    }

    public static string FZ250_fahrradstaende_columns
    {
      get { return rm.GetString("FZ250.fahrradstaende.columns"); }
    }

    public static string FZ250_fahrrad
    {
      get { return rm.GetString("FZ250.fahrrad"); }
    }

    public static string FZ250_fahrrad_tt
    {
      get { return rm.GetString("FZ250.fahrrad.tt"); }
    }

    public static string FZ250_text
    {
      get { return rm.GetString("FZ250.text"); }
    }

    public static string FZ250_text_tt
    {
      get { return rm.GetString("FZ250.text.tt"); }
    }

    public static string FZ250_alle
    {
      get { return rm.GetString("FZ250.alle"); }
    }

    public static string FZ250_alle_tt
    {
      get { return rm.GetString("FZ250.alle.tt"); }
    }

    public static string FZ260_title
    {
      get { return rm.GetString("FZ260.title"); }
    }

    public static string FZ260_nr
    {
      get { return rm.GetString("FZ260.nr"); }
    }

    public static string FZ260_nr_tt
    {
      get { return rm.GetString("FZ260.nr.tt"); }
    }

    public static string FZ260_fahrrad
    {
      get { return rm.GetString("FZ260.fahrrad"); }
    }

    public static string FZ260_fahrrad_tt
    {
      get { return rm.GetString("FZ260.fahrrad.tt"); }
    }

    public static string FZ260_datum
    {
      get { return rm.GetString("FZ260.datum"); }
    }

    public static string FZ260_datum_tt
    {
      get { return rm.GetString("FZ260.datum.tt"); }
    }

    public static string FZ260_zaehler
    {
      get { return rm.GetString("FZ260.zaehler"); }
    }

    public static string FZ260_zaehler_tt
    {
      get { return rm.GetString("FZ260.zaehler.tt"); }
    }

    public static string FZ260_km
    {
      get { return rm.GetString("FZ260.km"); }
    }

    public static string FZ260_km_tt
    {
      get { return rm.GetString("FZ260.km.tt"); }
    }

    public static string FZ260_schnitt
    {
      get { return rm.GetString("FZ260.schnitt"); }
    }

    public static string FZ260_schnitt_tt
    {
      get { return rm.GetString("FZ260.schnitt.tt"); }
    }

    public static string FZ260_beschreibung
    {
      get { return rm.GetString("FZ260.beschreibung"); }
    }

    public static string FZ260_beschreibung_tt
    {
      get { return rm.GetString("FZ260.beschreibung.tt"); }
    }

    public static string FZ300_title
    {
      get { return rm.GetString("FZ300.title"); }
    }

    public static string FZ300_autoren
    {
      get { return rm.GetString("FZ300.autoren"); }
    }

    public static string FZ300_autoren_tt
    {
      get { return rm.GetString("FZ300.autoren.tt"); }
    }

    public static string FZ300_autoren_columns
    {
      get { return rm.GetString("FZ300.autoren.columns"); }
    }

    public static string FZ300_name
    {
      get { return rm.GetString("FZ300.name"); }
    }

    public static string FZ300_name_tt
    {
      get { return rm.GetString("FZ300.name.tt"); }
    }

    public static string FZ300_alle
    {
      get { return rm.GetString("FZ300.alle"); }
    }

    public static string FZ300_alle_tt
    {
      get { return rm.GetString("FZ300.alle.tt"); }
    }

    public static string FZ310_title
    {
      get { return rm.GetString("FZ310.title"); }
    }

    public static string FZ310_nr
    {
      get { return rm.GetString("FZ310.nr"); }
    }

    public static string FZ310_nr_tt
    {
      get { return rm.GetString("FZ310.nr.tt"); }
    }

    public static string FZ310_name
    {
      get { return rm.GetString("FZ310.name"); }
    }

    public static string FZ310_name_tt
    {
      get { return rm.GetString("FZ310.name.tt"); }
    }

    public static string FZ310_vorname
    {
      get { return rm.GetString("FZ310.vorname"); }
    }

    public static string FZ310_vorname_tt
    {
      get { return rm.GetString("FZ310.vorname.tt"); }
    }

    public static string FZ310_notiz
    {
      get { return rm.GetString("FZ310.notiz"); }
    }

    public static string FZ310_notiz_tt
    {
      get { return rm.GetString("FZ310.notiz.tt"); }
    }

    public static string FZ320_title
    {
      get { return rm.GetString("FZ320.title"); }
    }

    public static string FZ320_serien
    {
      get { return rm.GetString("FZ320.serien"); }
    }

    public static string FZ320_serien_tt
    {
      get { return rm.GetString("FZ320.serien.tt"); }
    }

    public static string FZ320_serien_columns
    {
      get { return rm.GetString("FZ320.serien.columns"); }
    }

    public static string FZ320_name
    {
      get { return rm.GetString("FZ320.name"); }
    }

    public static string FZ320_name_tt
    {
      get { return rm.GetString("FZ320.name.tt"); }
    }

    public static string FZ320_alle
    {
      get { return rm.GetString("FZ320.alle"); }
    }

    public static string FZ320_alle_tt
    {
      get { return rm.GetString("FZ320.alle.tt"); }
    }

    public static string FZ330_title
    {
      get { return rm.GetString("FZ330.title"); }
    }

    public static string FZ330_nr
    {
      get { return rm.GetString("FZ330.nr"); }
    }

    public static string FZ330_nr_tt
    {
      get { return rm.GetString("FZ330.nr.tt"); }
    }

    public static string FZ330_name
    {
      get { return rm.GetString("FZ330.name"); }
    }

    public static string FZ330_name_tt
    {
      get { return rm.GetString("FZ330.name.tt"); }
    }

    public static string FZ330_notiz
    {
      get { return rm.GetString("FZ330.notiz"); }
    }

    public static string FZ330_notiz_tt
    {
      get { return rm.GetString("FZ330.notiz.tt"); }
    }

    public static string FZ340_title
    {
      get { return rm.GetString("FZ340.title"); }
    }

    public static string FZ340_buecher
    {
      get { return rm.GetString("FZ340.buecher"); }
    }

    public static string FZ340_buecher_tt
    {
      get { return rm.GetString("FZ340.buecher.tt"); }
    }

    public static string FZ340_buecher_columns
    {
      get { return rm.GetString("FZ340.buecher.columns"); }
    }

    public static string FZ340_titel
    {
      get { return rm.GetString("FZ340.titel"); }
    }

    public static string FZ340_titel_tt
    {
      get { return rm.GetString("FZ340.titel.tt"); }
    }

    public static string FZ340_autor
    {
      get { return rm.GetString("FZ340.autor"); }
    }

    public static string FZ340_autor_tt
    {
      get { return rm.GetString("FZ340.autor.tt"); }
    }

    public static string FZ340_serie
    {
      get { return rm.GetString("FZ340.serie"); }
    }

    public static string FZ340_serie_tt
    {
      get { return rm.GetString("FZ340.serie.tt"); }
    }

    public static string FZ340_alle
    {
      get { return rm.GetString("FZ340.alle"); }
    }

    public static string FZ340_alle_tt
    {
      get { return rm.GetString("FZ340.alle.tt"); }
    }

    public static string FZ350_title
    {
      get { return rm.GetString("FZ350.title"); }
    }

    public static string FZ350_nr
    {
      get { return rm.GetString("FZ350.nr"); }
    }

    public static string FZ350_nr_tt
    {
      get { return rm.GetString("FZ350.nr.tt"); }
    }

    public static string FZ350_titel
    {
      get { return rm.GetString("FZ350.titel"); }
    }

    public static string FZ350_titel_tt
    {
      get { return rm.GetString("FZ350.titel.tt"); }
    }

    public static string FZ350_untertitel
    {
      get { return rm.GetString("FZ350.untertitel"); }
    }

    public static string FZ350_untertitel_tt
    {
      get { return rm.GetString("FZ350.untertitel.tt"); }
    }

    public static string FZ350_autor
    {
      get { return rm.GetString("FZ350.autor"); }
    }

    public static string FZ350_autor_tt
    {
      get { return rm.GetString("FZ350.autor.tt"); }
    }

    public static string FZ350_autorneu
    {
      get { return rm.GetString("FZ350.autorneu"); }
    }

    public static string FZ350_autorneu_tt
    {
      get { return rm.GetString("FZ350.autorneu.tt"); }
    }

    public static string FZ350_serie
    {
      get { return rm.GetString("FZ350.serie"); }
    }

    public static string FZ350_serie_tt
    {
      get { return rm.GetString("FZ350.serie.tt"); }
    }

    public static string FZ350_serieneu
    {
      get { return rm.GetString("FZ350.serieneu"); }
    }

    public static string FZ350_serieneu_tt
    {
      get { return rm.GetString("FZ350.serieneu.tt"); }
    }

    public static string FZ350_seriennummer
    {
      get { return rm.GetString("FZ350.seriennummer"); }
    }

    public static string FZ350_seriennummer_tt
    {
      get { return rm.GetString("FZ350.seriennummer.tt"); }
    }

    public static string FZ350_seiten
    {
      get { return rm.GetString("FZ350.seiten"); }
    }

    public static string FZ350_seiten_tt
    {
      get { return rm.GetString("FZ350.seiten.tt"); }
    }

    public static string FZ350_sprache
    {
      get { return rm.GetString("FZ350.sprache"); }
    }

    public static string FZ350_sprache1
    {
      get { return rm.GetString("FZ350.sprache1"); }
    }

    public static string FZ350_sprache1_tt
    {
      get { return rm.GetString("FZ350.sprache1.tt"); }
    }

    public static string FZ350_sprache2
    {
      get { return rm.GetString("FZ350.sprache2"); }
    }

    public static string FZ350_sprache2_tt
    {
      get { return rm.GetString("FZ350.sprache2.tt"); }
    }

    public static string FZ350_sprache3
    {
      get { return rm.GetString("FZ350.sprache3"); }
    }

    public static string FZ350_sprache3_tt
    {
      get { return rm.GetString("FZ350.sprache3.tt"); }
    }

    public static string FZ350_sprache4
    {
      get { return rm.GetString("FZ350.sprache4"); }
    }

    public static string FZ350_sprache4_tt
    {
      get { return rm.GetString("FZ350.sprache4.tt"); }
    }

    public static string FZ350_besitz
    {
      get { return rm.GetString("FZ350.besitz"); }
    }

    public static string FZ350_besitz_tt
    {
      get { return rm.GetString("FZ350.besitz.tt"); }
    }

    public static string FZ350_lesedatum
    {
      get { return rm.GetString("FZ350.lesedatum"); }
    }

    public static string FZ350_lesedatum_tt
    {
      get { return rm.GetString("FZ350.lesedatum.tt"); }
    }

    public static string FZ350_hoerdatum
    {
      get { return rm.GetString("FZ350.hoerdatum"); }
    }

    public static string FZ350_hoerdatum_tt
    {
      get { return rm.GetString("FZ350.hoerdatum.tt"); }
    }

    public static string FZ350_notiz
    {
      get { return rm.GetString("FZ350.notiz"); }
    }

    public static string FZ350_notiz_tt
    {
      get { return rm.GetString("FZ350.notiz.tt"); }
    }

    public static string FZ700_title
    {
      get { return rm.GetString("FZ700.title"); }
    }

    public static string FZ700_notizen
    {
      get { return rm.GetString("FZ700.notizen"); }
    }

    public static string FZ700_notizen_tt
    {
      get { return rm.GetString("FZ700.notizen.tt"); }
    }

    public static string FZ700_notizen_columns
    {
      get { return rm.GetString("FZ700.notizen.columns"); }
    }

    public static string FZ700_alle
    {
      get { return rm.GetString("FZ700.alle"); }
    }

    public static string FZ700_alle_tt
    {
      get { return rm.GetString("FZ700.alle.tt"); }
    }

    public static string FZ700_text
    {
      get { return rm.GetString("FZ700.text"); }
    }

    public static string FZ700_text_tt
    {
      get { return rm.GetString("FZ700.text.tt"); }
    }

    public static string FZ710_title
    {
      get { return rm.GetString("FZ710.title"); }
    }

    public static string FZ710_nr
    {
      get { return rm.GetString("FZ710.nr"); }
    }

    public static string FZ710_nr_tt
    {
      get { return rm.GetString("FZ710.nr.tt"); }
    }

    public static string FZ710_thema
    {
      get { return rm.GetString("FZ710.thema"); }
    }

    public static string FZ710_thema_tt
    {
      get { return rm.GetString("FZ710.thema.tt"); }
    }

    public static string FZ710_notiz
    {
      get { return rm.GetString("FZ710.notiz"); }
    }

    public static string FZ710_notiz_tt
    {
      get { return rm.GetString("FZ710.notiz.tt"); }
    }

    public static string FZ710_tabelle_tt
    {
      get { return rm.GetString("FZ710.tabelle.tt"); }
    }

    public static string FZ710_tabelle_columns
    {
      get { return rm.GetString("FZ710.tabelle.columns"); }
    }

    public static string HH100_title
    {
      get { return rm.GetString("HH100.title"); }
    }

    public static string HH100_perioden
    {
      get { return rm.GetString("HH100.perioden"); }
    }

    public static string HH100_perioden_tt
    {
      get { return rm.GetString("HH100.perioden.tt"); }
    }

    public static string HH100_perioden_columns
    {
      get { return rm.GetString("HH100.perioden.columns"); }
    }

    public static string HH100_anfang
    {
      get { return rm.GetString("HH100.anfang"); }
    }

    public static string HH100_anfang_tt
    {
      get { return rm.GetString("HH100.anfang.tt"); }
    }

    public static string HH100_ende
    {
      get { return rm.GetString("HH100.ende"); }
    }

    public static string HH100_ende_tt
    {
      get { return rm.GetString("HH100.ende.tt"); }
    }

    public static string HH100_laenge
    {
      get { return rm.GetString("HH100.laenge"); }
    }

    public static string HH100_laenge1
    {
      get { return rm.GetString("HH100.laenge1"); }
    }

    public static string HH100_laenge1_tt
    {
      get { return rm.GetString("HH100.laenge1.tt"); }
    }

    public static string HH100_laenge2
    {
      get { return rm.GetString("HH100.laenge2"); }
    }

    public static string HH100_laenge2_tt
    {
      get { return rm.GetString("HH100.laenge2.tt"); }
    }

    public static string HH100_laenge3
    {
      get { return rm.GetString("HH100.laenge3"); }
    }

    public static string HH100_laenge3_tt
    {
      get { return rm.GetString("HH100.laenge3.tt"); }
    }

    public static string HH100_laenge4
    {
      get { return rm.GetString("HH100.laenge4"); }
    }

    public static string HH100_laenge4_tt
    {
      get { return rm.GetString("HH100.laenge4.tt"); }
    }

    public static string HH100_art
    {
      get { return rm.GetString("HH100.art"); }
    }

    public static string HH100_art1
    {
      get { return rm.GetString("HH100.art1"); }
    }

    public static string HH100_art1_tt
    {
      get { return rm.GetString("HH100.art1.tt"); }
    }

    public static string HH100_art2
    {
      get { return rm.GetString("HH100.art2"); }
    }

    public static string HH100_art2_tt
    {
      get { return rm.GetString("HH100.art2.tt"); }
    }

    public static string HH200_title
    {
      get { return rm.GetString("HH200.title"); }
    }

    public static string HH200_konten
    {
      get { return rm.GetString("HH200.konten"); }
    }

    public static string HH200_konten_tt
    {
      get { return rm.GetString("HH200.konten.tt"); }
    }

    public static string HH200_konten_columns
    {
      get { return rm.GetString("HH200.konten.columns"); }
    }

    public static string HH200_alle
    {
      get { return rm.GetString("HH200.alle"); }
    }

    public static string HH200_alle_tt
    {
      get { return rm.GetString("HH200.alle.tt"); }
    }

    public static string HH200_text
    {
      get { return rm.GetString("HH200.text"); }
    }

    public static string HH200_text_tt
    {
      get { return rm.GetString("HH200.text.tt"); }
    }

    public static string HH210_title
    {
      get { return rm.GetString("HH210.title"); }
    }

    public static string HH210_nr
    {
      get { return rm.GetString("HH210.nr"); }
    }

    public static string HH210_nr_tt
    {
      get { return rm.GetString("HH210.nr.tt"); }
    }

    public static string HH210_bezeichnung
    {
      get { return rm.GetString("HH210.bezeichnung"); }
    }

    public static string HH210_bezeichnung_tt
    {
      get { return rm.GetString("HH210.bezeichnung.tt"); }
    }

    public static string HH210_kennzeichen
    {
      get { return rm.GetString("HH210.kennzeichen"); }
    }

    public static string HH210_kennzeichen1
    {
      get { return rm.GetString("HH210.kennzeichen1"); }
    }

    public static string HH210_kennzeichen1_tt
    {
      get { return rm.GetString("HH210.kennzeichen1.tt"); }
    }

    public static string HH210_kennzeichen2
    {
      get { return rm.GetString("HH210.kennzeichen2"); }
    }

    public static string HH210_kennzeichen2_tt
    {
      get { return rm.GetString("HH210.kennzeichen2.tt"); }
    }

    public static string HH210_kennzeichen3
    {
      get { return rm.GetString("HH210.kennzeichen3"); }
    }

    public static string HH210_kennzeichen3_tt
    {
      get { return rm.GetString("HH210.kennzeichen3.tt"); }
    }

    public static string HH210_kennzeichen4
    {
      get { return rm.GetString("HH210.kennzeichen4"); }
    }

    public static string HH210_kennzeichen4_tt
    {
      get { return rm.GetString("HH210.kennzeichen4.tt"); }
    }

    public static string HH210_kontoart
    {
      get { return rm.GetString("HH210.kontoart"); }
    }

    public static string HH210_kontoart1
    {
      get { return rm.GetString("HH210.kontoart1"); }
    }

    public static string HH210_kontoart1_tt
    {
      get { return rm.GetString("HH210.kontoart1.tt"); }
    }

    public static string HH210_kontoart2
    {
      get { return rm.GetString("HH210.kontoart2"); }
    }

    public static string HH210_kontoart2_tt
    {
      get { return rm.GetString("HH210.kontoart2.tt"); }
    }

    public static string HH210_kontoart3
    {
      get { return rm.GetString("HH210.kontoart3"); }
    }

    public static string HH210_kontoart3_tt
    {
      get { return rm.GetString("HH210.kontoart3.tt"); }
    }

    public static string HH210_kontoart4
    {
      get { return rm.GetString("HH210.kontoart4"); }
    }

    public static string HH210_kontoart4_tt
    {
      get { return rm.GetString("HH210.kontoart4.tt"); }
    }

    public static string HH210_von
    {
      get { return rm.GetString("HH210.von"); }
    }

    public static string HH210_von_tt
    {
      get { return rm.GetString("HH210.von.tt"); }
    }

    public static string HH210_bis
    {
      get { return rm.GetString("HH210.bis"); }
    }

    public static string HH210_bis_tt
    {
      get { return rm.GetString("HH210.bis.tt"); }
    }

    public static string HH210_betrag
    {
      get { return rm.GetString("HH210.betrag"); }
    }

    public static string HH210_betrag_tt
    {
      get { return rm.GetString("HH210.betrag.tt"); }
    }

    public static string HH300_title
    {
      get { return rm.GetString("HH300.title"); }
    }

    public static string HH300_ereignisse
    {
      get { return rm.GetString("HH300.ereignisse"); }
    }

    public static string HH300_ereignisse_tt
    {
      get { return rm.GetString("HH300.ereignisse.tt"); }
    }

    public static string HH300_ereignisse_columns
    {
      get { return rm.GetString("HH300.ereignisse.columns"); }
    }

    public static string HH300_alle
    {
      get { return rm.GetString("HH300.alle"); }
    }

    public static string HH300_alle_tt
    {
      get { return rm.GetString("HH300.alle.tt"); }
    }

    public static string HH300_text
    {
      get { return rm.GetString("HH300.text"); }
    }

    public static string HH300_text_tt
    {
      get { return rm.GetString("HH300.text.tt"); }
    }

    public static string HH310_title
    {
      get { return rm.GetString("HH310.title"); }
    }

    public static string HH310_nr
    {
      get { return rm.GetString("HH310.nr"); }
    }

    public static string HH310_nr_tt
    {
      get { return rm.GetString("HH310.nr.tt"); }
    }

    public static string HH310_bezeichnung
    {
      get { return rm.GetString("HH310.bezeichnung"); }
    }

    public static string HH310_bezeichnung_tt
    {
      get { return rm.GetString("HH310.bezeichnung.tt"); }
    }

    public static string HH310_kennzeichen
    {
      get { return rm.GetString("HH310.kennzeichen"); }
    }

    public static string HH310_kennzeichen_tt
    {
      get { return rm.GetString("HH310.kennzeichen.tt"); }
    }

    public static string HH310_eText
    {
      get { return rm.GetString("HH310.eText"); }
    }

    public static string HH310_eText_tt
    {
      get { return rm.GetString("HH310.eText.tt"); }
    }

    public static string HH310_sollkonto
    {
      get { return rm.GetString("HH310.sollkonto"); }
    }

    public static string HH310_sollkonto_tt
    {
      get { return rm.GetString("HH310.sollkonto.tt"); }
    }

    public static string HH310_sollkonto_columns
    {
      get { return rm.GetString("HH310.sollkonto.columns"); }
    }

    public static string HH310_habenkonto
    {
      get { return rm.GetString("HH310.habenkonto"); }
    }

    public static string HH310_habenkonto_tt
    {
      get { return rm.GetString("HH310.habenkonto.tt"); }
    }

    public static string HH310_habenkonto_columns
    {
      get { return rm.GetString("HH310.habenkonto.columns"); }
    }

    public static string HH310_kontentausch
    {
      get { return rm.GetString("HH310.kontentausch"); }
    }

    public static string HH310_kontentausch_tt
    {
      get { return rm.GetString("HH310.kontentausch.tt"); }
    }

    public static string HH400_title
    {
      get { return rm.GetString("HH400.title"); }
    }

    public static string HH400_buchungen
    {
      get { return rm.GetString("HH400.buchungen"); }
    }

    public static string HH400_buchungen_tt
    {
      get { return rm.GetString("HH400.buchungen.tt"); }
    }

    public static string HH400_buchungen_columns
    {
      get { return rm.GetString("HH400.buchungen.columns"); }
    }

    public static string HH400_buchungenStatus
    {
      get { return rm.GetString("HH400.buchungenStatus"); }
    }

    public static string HH400_kennzeichen
    {
      get { return rm.GetString("HH400.kennzeichen"); }
    }

    public static string HH400_kennzeichen1
    {
      get { return rm.GetString("HH400.kennzeichen1"); }
    }

    public static string HH400_kennzeichen1_tt
    {
      get { return rm.GetString("HH400.kennzeichen1.tt"); }
    }

    public static string HH400_kennzeichen2
    {
      get { return rm.GetString("HH400.kennzeichen2"); }
    }

    public static string HH400_kennzeichen2_tt
    {
      get { return rm.GetString("HH400.kennzeichen2.tt"); }
    }

    public static string HH400_von
    {
      get { return rm.GetString("HH400.von"); }
    }

    public static string HH400_von_tt
    {
      get { return rm.GetString("HH400.von.tt"); }
    }

    public static string HH400_bis
    {
      get { return rm.GetString("HH400.bis"); }
    }

    public static string HH400_bis_tt
    {
      get { return rm.GetString("HH400.bis.tt"); }
    }

    public static string HH400_bText
    {
      get { return rm.GetString("HH400.bText"); }
    }

    public static string HH400_bText_tt
    {
      get { return rm.GetString("HH400.bText.tt"); }
    }

    public static string HH400_betrag
    {
      get { return rm.GetString("HH400.betrag"); }
    }

    public static string HH400_betrag_tt
    {
      get { return rm.GetString("HH400.betrag.tt"); }
    }

    public static string HH400_konto
    {
      get { return rm.GetString("HH400.konto"); }
    }

    public static string HH400_konto_tt
    {
      get { return rm.GetString("HH400.konto.tt"); }
    }

    public static string HH400_alle
    {
      get { return rm.GetString("HH400.alle"); }
    }

    public static string HH400_alle_tt
    {
      get { return rm.GetString("HH400.alle.tt"); }
    }

    public static string HH400_select_file
    {
      get { return rm.GetString("HH400.select.file"); }
    }

    public static string HH410_title
    {
      get { return rm.GetString("HH410.title"); }
    }

    public static string HH410_nr
    {
      get { return rm.GetString("HH410.nr"); }
    }

    public static string HH410_nr_tt
    {
      get { return rm.GetString("HH410.nr.tt"); }
    }

    public static string HH410_valuta
    {
      get { return rm.GetString("HH410.valuta"); }
    }

    public static string HH410_valuta_tt
    {
      get { return rm.GetString("HH410.valuta.tt"); }
    }

    public static string HH410_betrag
    {
      get { return rm.GetString("HH410.betrag"); }
    }

    public static string HH410_betrag_tt
    {
      get { return rm.GetString("HH410.betrag.tt"); }
    }

    public static string HH410_summe
    {
      get { return rm.GetString("HH410.summe"); }
    }

    public static string HH410_summe_tt
    {
      get { return rm.GetString("HH410.summe.tt"); }
    }

    public static string HH410_ereignis
    {
      get { return rm.GetString("HH410.ereignis"); }
    }

    public static string HH410_ereignis_tt
    {
      get { return rm.GetString("HH410.ereignis.tt"); }
    }

    public static string HH410_ereignis_columns
    {
      get { return rm.GetString("HH410.ereignis.columns"); }
    }

    public static string HH410_sollkonto
    {
      get { return rm.GetString("HH410.sollkonto"); }
    }

    public static string HH410_sollkonto_tt
    {
      get { return rm.GetString("HH410.sollkonto.tt"); }
    }

    public static string HH410_sollkonto_columns
    {
      get { return rm.GetString("HH410.sollkonto.columns"); }
    }

    public static string HH410_habenkonto
    {
      get { return rm.GetString("HH410.habenkonto"); }
    }

    public static string HH410_habenkonto_tt
    {
      get { return rm.GetString("HH410.habenkonto.tt"); }
    }

    public static string HH410_habenkonto_columns
    {
      get { return rm.GetString("HH410.habenkonto.columns"); }
    }

    public static string HH410_bText
    {
      get { return rm.GetString("HH410.bText"); }
    }

    public static string HH410_bText_tt
    {
      get { return rm.GetString("HH410.bText.tt"); }
    }

    public static string HH410_belegNr
    {
      get { return rm.GetString("HH410.belegNr"); }
    }

    public static string HH410_belegNr_tt
    {
      get { return rm.GetString("HH410.belegNr.tt"); }
    }

    public static string HH410_neueNr
    {
      get { return rm.GetString("HH410.neueNr"); }
    }

    public static string HH410_neueNr_tt
    {
      get { return rm.GetString("HH410.neueNr.tt"); }
    }

    public static string HH410_belegDatum
    {
      get { return rm.GetString("HH410.belegDatum"); }
    }

    public static string HH410_belegDatum_tt
    {
      get { return rm.GetString("HH410.belegDatum.tt"); }
    }

    public static string HH410_buchung
    {
      get { return rm.GetString("HH410.buchung"); }
    }

    public static string HH410_buchung_tt
    {
      get { return rm.GetString("HH410.buchung.tt"); }
    }

    public static string HH410_kontentausch
    {
      get { return rm.GetString("HH410.kontentausch"); }
    }

    public static string HH410_kontentausch_tt
    {
      get { return rm.GetString("HH410.kontentausch.tt"); }
    }

    public static string HH410_addition
    {
      get { return rm.GetString("HH410.addition"); }
    }

    public static string HH410_addition_tt
    {
      get { return rm.GetString("HH410.addition.tt"); }
    }

    public static string HH500_title
    {
      get { return rm.GetString("HH500.title"); }
    }

    public static string HH500_title_EB
    {
      get { return rm.GetString("HH500.title.EB"); }
    }

    public static string HH500_title_GV
    {
      get { return rm.GetString("HH500.title.GV"); }
    }

    public static string HH500_title_SB
    {
      get { return rm.GetString("HH500.title.SB"); }
    }

    public static string HH500_berechnen_tt
    {
      get { return rm.GetString("HH500.berechnen.tt"); }
    }

    public static string HH500_soll
    {
      get { return rm.GetString("HH500.soll"); }
    }

    public static string HH500_soll_EB
    {
      get { return rm.GetString("HH500.soll.EB"); }
    }

    public static string HH500_soll_GV
    {
      get { return rm.GetString("HH500.soll.GV"); }
    }

    public static string HH500_soll_tt
    {
      get { return rm.GetString("HH500.soll.tt"); }
    }

    public static string HH500_soll_columns
    {
      get { return rm.GetString("HH500.soll.columns"); }
    }

    public static string HH500_haben
    {
      get { return rm.GetString("HH500.haben"); }
    }

    public static string HH500_haben_EB
    {
      get { return rm.GetString("HH500.haben.EB"); }
    }

    public static string HH500_haben_GV
    {
      get { return rm.GetString("HH500.haben.GV"); }
    }

    public static string HH500_haben_tt
    {
      get { return rm.GetString("HH500.haben.tt"); }
    }

    public static string HH500_haben_columns
    {
      get { return rm.GetString("HH500.haben.columns"); }
    }

    public static string HH500_sollSumme
    {
      get { return rm.GetString("HH500.sollSumme"); }
    }

    public static string HH500_habenSumme
    {
      get { return rm.GetString("HH500.habenSumme"); }
    }

    public static string HH500_von
    {
      get { return rm.GetString("HH500.von"); }
    }

    public static string HH500_von_EB
    {
      get { return rm.GetString("HH500.von.EB"); }
    }

    public static string HH500_von_tt
    {
      get { return rm.GetString("HH500.von.tt"); }
    }

    public static string HH500_bis
    {
      get { return rm.GetString("HH500.bis"); }
    }

    public static string HH500_bis_tt
    {
      get { return rm.GetString("HH500.bis.tt"); }
    }

    public static string HH500_konto
    {
      get { return rm.GetString("HH500.konto"); }
    }

    public static string HH500_oben
    {
      get { return rm.GetString("HH500.oben"); }
    }

    public static string HH500_oben_tt
    {
      get { return rm.GetString("HH500.oben.tt"); }
    }

    public static string HH500_unten
    {
      get { return rm.GetString("HH500.unten"); }
    }

    public static string HH500_unten_tt
    {
      get { return rm.GetString("HH500.unten.tt"); }
    }

    public static string HH510_title
    {
      get { return rm.GetString("HH510.title"); }
    }

    public static string HH510_titel
    {
      get { return rm.GetString("HH510.titel"); }
    }

    public static string HH510_titel_tt
    {
      get { return rm.GetString("HH510.titel.tt"); }
    }

    public static string HH510_von
    {
      get { return rm.GetString("HH510.von"); }
    }

    public static string HH510_von_tt
    {
      get { return rm.GetString("HH510.von.tt"); }
    }

    public static string HH510_bis
    {
      get { return rm.GetString("HH510.bis"); }
    }

    public static string HH510_bis_tt
    {
      get { return rm.GetString("HH510.bis.tt"); }
    }

    public static string HH510_berichte0
    {
      get { return rm.GetString("HH510.berichte0"); }
    }

    public static string HH510_eb
    {
      get { return rm.GetString("HH510.eb"); }
    }

    public static string HH510_eb_tt
    {
      get { return rm.GetString("HH510.eb.tt"); }
    }

    public static string HH510_gv
    {
      get { return rm.GetString("HH510.gv"); }
    }

    public static string HH510_gv_tt
    {
      get { return rm.GetString("HH510.gv.tt"); }
    }

    public static string HH510_sb
    {
      get { return rm.GetString("HH510.sb"); }
    }

    public static string HH510_sb_tt
    {
      get { return rm.GetString("HH510.sb.tt"); }
    }

    public static string HH510_kassenbericht
    {
      get { return rm.GetString("HH510.kassenbericht"); }
    }

    public static string HH510_kassenbericht_tt
    {
      get { return rm.GetString("HH510.kassenbericht.tt"); }
    }

    public static string HH510_datei
    {
      get { return rm.GetString("HH510.datei"); }
    }

    public static string HH510_datei_tt
    {
      get { return rm.GetString("HH510.datei.tt"); }
    }

    public static string HH510_dateiAuswahl
    {
      get { return rm.GetString("HH510.dateiAuswahl"); }
    }

    public static string HH510_dateiAuswahl_tt
    {
      get { return rm.GetString("HH510.dateiAuswahl.tt"); }
    }

    public static string HH510_loeschen
    {
      get { return rm.GetString("HH510.loeschen"); }
    }

    public static string HH510_loeschen_tt
    {
      get { return rm.GetString("HH510.loeschen.tt"); }
    }

    public static string HH510_import1
    {
      get { return rm.GetString("HH510.import1"); }
    }

    public static string HH510_import1_tt
    {
      get { return rm.GetString("HH510.import1.tt"); }
    }

    public static string HH510_select_file
    {
      get { return rm.GetString("HH510.select.file"); }
    }

    public static string HH510_select_ext
    {
      get { return rm.GetString("HH510.select.ext"); }
    }

    public static string SB200_title
    {
      get { return rm.GetString("SB200.title"); }
    }

    public static string SB200_ahnen
    {
      get { return rm.GetString("SB200.ahnen"); }
    }

    public static string SB200_ahnen_tt
    {
      get { return rm.GetString("SB200.ahnen.tt"); }
    }

    public static string SB200_ahnen_columns
    {
      get { return rm.GetString("SB200.ahnen.columns"); }
    }

    public static string SB200_ahnenStatus
    {
      get { return rm.GetString("SB200.ahnenStatus"); }
    }

    public static string SB200_name
    {
      get { return rm.GetString("SB200.name"); }
    }

    public static string SB200_name_tt
    {
      get { return rm.GetString("SB200.name.tt"); }
    }

    public static string SB200_vorname
    {
      get { return rm.GetString("SB200.vorname"); }
    }

    public static string SB200_vorname_tt
    {
      get { return rm.GetString("SB200.vorname.tt"); }
    }

    public static string SB200_filtern
    {
      get { return rm.GetString("SB200.filtern"); }
    }

    public static string SB200_filtern_tt
    {
      get { return rm.GetString("SB200.filtern.tt"); }
    }

    public static string SB200_alle
    {
      get { return rm.GetString("SB200.alle"); }
    }

    public static string SB200_alle_tt
    {
      get { return rm.GetString("SB200.alle.tt"); }
    }

    public static string SB200_springen
    {
      get { return rm.GetString("SB200.springen"); }
    }

    public static string SB200_spName
    {
      get { return rm.GetString("SB200.spName"); }
    }

    public static string SB200_spName_tt
    {
      get { return rm.GetString("SB200.spName.tt"); }
    }

    public static string SB200_spVater
    {
      get { return rm.GetString("SB200.spVater"); }
    }

    public static string SB200_spVater_tt
    {
      get { return rm.GetString("SB200.spVater.tt"); }
    }

    public static string SB200_spMutter
    {
      get { return rm.GetString("SB200.spMutter"); }
    }

    public static string SB200_spMutter_tt
    {
      get { return rm.GetString("SB200.spMutter.tt"); }
    }

    public static string SB200_spKind
    {
      get { return rm.GetString("SB200.spKind"); }
    }

    public static string SB200_spKind_tt
    {
      get { return rm.GetString("SB200.spKind.tt"); }
    }

    public static string SB200_spEhegatte
    {
      get { return rm.GetString("SB200.spEhegatte"); }
    }

    public static string SB200_spEhegatte_tt
    {
      get { return rm.GetString("SB200.spEhegatte.tt"); }
    }

    public static string SB200_spGeschwister
    {
      get { return rm.GetString("SB200.spGeschwister"); }
    }

    public static string SB200_spGeschwister_tt
    {
      get { return rm.GetString("SB200.spGeschwister.tt"); }
    }

    public static string SB200_spFamilie
    {
      get { return rm.GetString("SB200.spFamilie"); }
    }

    public static string SB200_spFamilie_tt
    {
      get { return rm.GetString("SB200.spFamilie.tt"); }
    }

    public static string SB200_spFamilienKind
    {
      get { return rm.GetString("SB200.spFamilienKind"); }
    }

    public static string SB200_spFamilienKind_tt
    {
      get { return rm.GetString("SB200.spFamilienKind.tt"); }
    }

    public static string SB210_title
    {
      get { return rm.GetString("SB210.title"); }
    }

    public static string SB210_nr
    {
      get { return rm.GetString("SB210.nr"); }
    }

    public static string SB210_nr_tt
    {
      get { return rm.GetString("SB210.nr.tt"); }
    }

    public static string SB210_geburtsname
    {
      get { return rm.GetString("SB210.geburtsname"); }
    }

    public static string SB210_geburtsname_tt
    {
      get { return rm.GetString("SB210.geburtsname.tt"); }
    }

    public static string SB210_vorname
    {
      get { return rm.GetString("SB210.vorname"); }
    }

    public static string SB210_vorname_tt
    {
      get { return rm.GetString("SB210.vorname.tt"); }
    }

    public static string SB210_name
    {
      get { return rm.GetString("SB210.name"); }
    }

    public static string SB210_name_tt
    {
      get { return rm.GetString("SB210.name.tt"); }
    }

    public static string SB210_geschlecht
    {
      get { return rm.GetString("SB210.geschlecht"); }
    }

    public static string SB210_geschlecht1
    {
      get { return rm.GetString("SB210.geschlecht1"); }
    }

    public static string SB210_geschlecht1_tt
    {
      get { return rm.GetString("SB210.geschlecht1.tt"); }
    }

    public static string SB210_geschlecht2
    {
      get { return rm.GetString("SB210.geschlecht2"); }
    }

    public static string SB210_geschlecht2_tt
    {
      get { return rm.GetString("SB210.geschlecht2.tt"); }
    }

    public static string SB210_geschlecht3
    {
      get { return rm.GetString("SB210.geschlecht3"); }
    }

    public static string SB210_geschlecht3_tt
    {
      get { return rm.GetString("SB210.geschlecht3.tt"); }
    }

    public static string SB210_bilder_tt
    {
      get { return rm.GetString("SB210.bilder.tt"); }
    }

    public static string SB210_bilddaten
    {
      get { return rm.GetString("SB210.bilddaten"); }
    }

    public static string SB210_bilddaten_tt
    {
      get { return rm.GetString("SB210.bilddaten.tt"); }
    }

    public static string SB210_geburtsdatum
    {
      get { return rm.GetString("SB210.geburtsdatum"); }
    }

    public static string SB210_geburtsdatum_tt
    {
      get { return rm.GetString("SB210.geburtsdatum.tt"); }
    }

    public static string SB210_geburtsort
    {
      get { return rm.GetString("SB210.geburtsort"); }
    }

    public static string SB210_geburtsort_tt
    {
      get { return rm.GetString("SB210.geburtsort.tt"); }
    }

    public static string SB210_geburtsbem
    {
      get { return rm.GetString("SB210.geburtsbem"); }
    }

    public static string SB210_geburtsbem_tt
    {
      get { return rm.GetString("SB210.geburtsbem.tt"); }
    }

    public static string SB210_taufdatum
    {
      get { return rm.GetString("SB210.taufdatum"); }
    }

    public static string SB210_taufdatum_tt
    {
      get { return rm.GetString("SB210.taufdatum.tt"); }
    }

    public static string SB210_taufort
    {
      get { return rm.GetString("SB210.taufort"); }
    }

    public static string SB210_taufort_tt
    {
      get { return rm.GetString("SB210.taufort.tt"); }
    }

    public static string SB210_taufbem
    {
      get { return rm.GetString("SB210.taufbem"); }
    }

    public static string SB210_taufbem_tt
    {
      get { return rm.GetString("SB210.taufbem.tt"); }
    }

    public static string SB210_todesdatum
    {
      get { return rm.GetString("SB210.todesdatum"); }
    }

    public static string SB210_todesdatum_tt
    {
      get { return rm.GetString("SB210.todesdatum.tt"); }
    }

    public static string SB210_todesort
    {
      get { return rm.GetString("SB210.todesort"); }
    }

    public static string SB210_todesort_tt
    {
      get { return rm.GetString("SB210.todesort.tt"); }
    }

    public static string SB210_todesbem
    {
      get { return rm.GetString("SB210.todesbem"); }
    }

    public static string SB210_todesbem_tt
    {
      get { return rm.GetString("SB210.todesbem.tt"); }
    }

    public static string SB210_begraebnisdatum
    {
      get { return rm.GetString("SB210.begraebnisdatum"); }
    }

    public static string SB210_begraebnisdatum_tt
    {
      get { return rm.GetString("SB210.begraebnisdatum.tt"); }
    }

    public static string SB210_begraebnisort
    {
      get { return rm.GetString("SB210.begraebnisort"); }
    }

    public static string SB210_begraebnisort_tt
    {
      get { return rm.GetString("SB210.begraebnisort.tt"); }
    }

    public static string SB210_begraebnisbem
    {
      get { return rm.GetString("SB210.begraebnisbem"); }
    }

    public static string SB210_begraebnisbem_tt
    {
      get { return rm.GetString("SB210.begraebnisbem.tt"); }
    }

    public static string SB210_konfession
    {
      get { return rm.GetString("SB210.konfession"); }
    }

    public static string SB210_konfession_tt
    {
      get { return rm.GetString("SB210.konfession.tt"); }
    }

    public static string SB210_titel
    {
      get { return rm.GetString("SB210.titel"); }
    }

    public static string SB210_titel_tt
    {
      get { return rm.GetString("SB210.titel.tt"); }
    }

    public static string SB210_bemerkung
    {
      get { return rm.GetString("SB210.bemerkung"); }
    }

    public static string SB210_bemerkung_tt
    {
      get { return rm.GetString("SB210.bemerkung.tt"); }
    }

    public static string SB210_gatte
    {
      get { return rm.GetString("SB210.gatte"); }
    }

    public static string SB210_gatte_tt
    {
      get { return rm.GetString("SB210.gatte.tt"); }
    }

    public static string SB210_gatteNr
    {
      get { return rm.GetString("SB210.gatteNr"); }
    }

    public static string SB210_gatteNr_tt
    {
      get { return rm.GetString("SB210.gatteNr.tt"); }
    }

    public static string SB210_vater
    {
      get { return rm.GetString("SB210.vater"); }
    }

    public static string SB210_vater_tt
    {
      get { return rm.GetString("SB210.vater.tt"); }
    }

    public static string SB210_vaterNr
    {
      get { return rm.GetString("SB210.vaterNr"); }
    }

    public static string SB210_vaterNr_tt
    {
      get { return rm.GetString("SB210.vaterNr.tt"); }
    }

    public static string SB210_mutter
    {
      get { return rm.GetString("SB210.mutter"); }
    }

    public static string SB210_mutter_tt
    {
      get { return rm.GetString("SB210.mutter.tt"); }
    }

    public static string SB210_mutterNr
    {
      get { return rm.GetString("SB210.mutterNr"); }
    }

    public static string SB210_mutterNr_tt
    {
      get { return rm.GetString("SB210.mutterNr.tt"); }
    }

    public static string SB210_quelle
    {
      get { return rm.GetString("SB210.quelle"); }
    }

    public static string SB210_quelle_tt
    {
      get { return rm.GetString("SB210.quelle.tt"); }
    }

    public static string SB210_status1
    {
      get { return rm.GetString("SB210.status1"); }
    }

    public static string SB210_status1_tt
    {
      get { return rm.GetString("SB210.status1.tt"); }
    }

    public static string SB210_status2
    {
      get { return rm.GetString("SB210.status2"); }
    }

    public static string SB210_status2_tt
    {
      get { return rm.GetString("SB210.status2.tt"); }
    }

    public static string SB210_status3
    {
      get { return rm.GetString("SB210.status3"); }
    }

    public static string SB210_status3_tt
    {
      get { return rm.GetString("SB210.status3.tt"); }
    }

    public static string SB210_hinzufuegen
    {
      get { return rm.GetString("SB210.hinzufuegen"); }
    }

    public static string SB210_hinzufuegen_tt
    {
      get { return rm.GetString("SB210.hinzufuegen.tt"); }
    }

    public static string SB210_select_file
    {
      get { return rm.GetString("SB210.select.file"); }
    }

    public static string SB210_select_ext
    {
      get { return rm.GetString("SB210.select.ext"); }
    }

    public static string SB220_title
    {
      get { return rm.GetString("SB220.title"); }
    }

    public static string SB220_person
    {
      get { return rm.GetString("SB220.person"); }
    }

    public static string SB220_person_tt
    {
      get { return rm.GetString("SB220.person.tt"); }
    }

    public static string SB220_generation
    {
      get { return rm.GetString("SB220.generation"); }
    }

    public static string SB220_generation_tt
    {
      get { return rm.GetString("SB220.generation.tt"); }
    }

    public static string SB220_vorfahren
    {
      get { return rm.GetString("SB220.vorfahren"); }
    }

    public static string SB220_vorfahren_tt
    {
      get { return rm.GetString("SB220.vorfahren.tt"); }
    }

    public static string SB220_geschwister
    {
      get { return rm.GetString("SB220.geschwister"); }
    }

    public static string SB220_geschwister_tt
    {
      get { return rm.GetString("SB220.geschwister.tt"); }
    }

    public static string SB220_nachfahren
    {
      get { return rm.GetString("SB220.nachfahren"); }
    }

    public static string SB220_nachfahren_tt
    {
      get { return rm.GetString("SB220.nachfahren.tt"); }
    }

    public static string SB300_title
    {
      get { return rm.GetString("SB300.title"); }
    }

    public static string SB300_familien
    {
      get { return rm.GetString("SB300.familien"); }
    }

    public static string SB300_familien_tt
    {
      get { return rm.GetString("SB300.familien.tt"); }
    }

    public static string SB300_familien_columns
    {
      get { return rm.GetString("SB300.familien.columns"); }
    }

    public static string SB300_springen
    {
      get { return rm.GetString("SB300.springen"); }
    }

    public static string SB300_spVater
    {
      get { return rm.GetString("SB300.spVater"); }
    }

    public static string SB300_spVater_tt
    {
      get { return rm.GetString("SB300.spVater.tt"); }
    }

    public static string SB300_spMutter
    {
      get { return rm.GetString("SB300.spMutter"); }
    }

    public static string SB300_spMutter_tt
    {
      get { return rm.GetString("SB300.spMutter.tt"); }
    }

    public static string SB300_spKind
    {
      get { return rm.GetString("SB300.spKind"); }
    }

    public static string SB300_spKind_tt
    {
      get { return rm.GetString("SB300.spKind.tt"); }
    }

    public static string SB310_title
    {
      get { return rm.GetString("SB310.title"); }
    }

    public static string SB310_nr
    {
      get { return rm.GetString("SB310.nr"); }
    }

    public static string SB310_nr_tt
    {
      get { return rm.GetString("SB310.nr.tt"); }
    }

    public static string SB310_vater
    {
      get { return rm.GetString("SB310.vater"); }
    }

    public static string SB310_vater_tt
    {
      get { return rm.GetString("SB310.vater.tt"); }
    }

    public static string SB310_mutter
    {
      get { return rm.GetString("SB310.mutter"); }
    }

    public static string SB310_mutter_tt
    {
      get { return rm.GetString("SB310.mutter.tt"); }
    }

    public static string SB310_heiratsdatum
    {
      get { return rm.GetString("SB310.heiratsdatum"); }
    }

    public static string SB310_heiratsdatum_tt
    {
      get { return rm.GetString("SB310.heiratsdatum.tt"); }
    }

    public static string SB310_heiratsort
    {
      get { return rm.GetString("SB310.heiratsort"); }
    }

    public static string SB310_heiratsort_tt
    {
      get { return rm.GetString("SB310.heiratsort.tt"); }
    }

    public static string SB310_heiratsbem
    {
      get { return rm.GetString("SB310.heiratsbem"); }
    }

    public static string SB310_heiratsbem_tt
    {
      get { return rm.GetString("SB310.heiratsbem.tt"); }
    }

    public static string SB310_kinder
    {
      get { return rm.GetString("SB310.kinder"); }
    }

    public static string SB310_kinder_tt
    {
      get { return rm.GetString("SB310.kinder.tt"); }
    }

    public static string SB310_kinder_columns
    {
      get { return rm.GetString("SB310.kinder.columns"); }
    }

    public static string SB310_kind
    {
      get { return rm.GetString("SB310.kind"); }
    }

    public static string SB310_kind_tt
    {
      get { return rm.GetString("SB310.kind.tt"); }
    }

    public static string SB310_hinzufuegen
    {
      get { return rm.GetString("SB310.hinzufuegen"); }
    }

    public static string SB310_hinzufuegen_tt
    {
      get { return rm.GetString("SB310.hinzufuegen.tt"); }
    }

    public static string SB310_entfernen
    {
      get { return rm.GetString("SB310.entfernen"); }
    }

    public static string SB310_entfernen_tt
    {
      get { return rm.GetString("SB310.entfernen.tt"); }
    }

    public static string SB400_title
    {
      get { return rm.GetString("SB400.title"); }
    }

    public static string SB400_quellen
    {
      get { return rm.GetString("SB400.quellen"); }
    }

    public static string SB400_quellen_tt
    {
      get { return rm.GetString("SB400.quellen.tt"); }
    }

    public static string SB400_quellen_columns
    {
      get { return rm.GetString("SB400.quellen.columns"); }
    }

    public static string SB410_title
    {
      get { return rm.GetString("SB410.title"); }
    }

    public static string SB410_nr
    {
      get { return rm.GetString("SB410.nr"); }
    }

    public static string SB410_nr_tt
    {
      get { return rm.GetString("SB410.nr.tt"); }
    }

    public static string SB410_autor
    {
      get { return rm.GetString("SB410.autor"); }
    }

    public static string SB410_autor_tt
    {
      get { return rm.GetString("SB410.autor.tt"); }
    }

    public static string SB410_beschreibung
    {
      get { return rm.GetString("SB410.beschreibung"); }
    }

    public static string SB410_beschreibung_tt
    {
      get { return rm.GetString("SB410.beschreibung.tt"); }
    }

    public static string SB410_zitat
    {
      get { return rm.GetString("SB410.zitat"); }
    }

    public static string SB410_zitat_tt
    {
      get { return rm.GetString("SB410.zitat.tt"); }
    }

    public static string SB410_bemerkung
    {
      get { return rm.GetString("SB410.bemerkung"); }
    }

    public static string SB410_bemerkung_tt
    {
      get { return rm.GetString("SB410.bemerkung.tt"); }
    }

    public static string SB500_title
    {
      get { return rm.GetString("SB500.title"); }
    }

    public static string SB500_name
    {
      get { return rm.GetString("SB500.name"); }
    }

    public static string SB500_name_tt
    {
      get { return rm.GetString("SB500.name.tt"); }
    }

    public static string SB500_datei
    {
      get { return rm.GetString("SB500.datei"); }
    }

    public static string SB500_datei_tt
    {
      get { return rm.GetString("SB500.datei.tt"); }
    }

    public static string SB500_dateiAuswahl
    {
      get { return rm.GetString("SB500.dateiAuswahl"); }
    }

    public static string SB500_dateiAuswahl_tt
    {
      get { return rm.GetString("SB500.dateiAuswahl.tt"); }
    }

    public static string SB500_filter
    {
      get { return rm.GetString("SB500.filter"); }
    }

    public static string SB500_filter_tt
    {
      get { return rm.GetString("SB500.filter.tt"); }
    }

    public static string SB500_export
    {
      get { return rm.GetString("SB500.export"); }
    }

    public static string SB500_export_tt
    {
      get { return rm.GetString("SB500.export.tt"); }
    }

    public static string SB500_importieren
    {
      get { return rm.GetString("SB500.importieren"); }
    }

    public static string SB500_importieren_tt
    {
      get { return rm.GetString("SB500.importieren.tt"); }
    }

    public static string SB500_select_file
    {
      get { return rm.GetString("SB500.select.file"); }
    }

    public static string SB500_select_ext
    {
      get { return rm.GetString("SB500.select.ext"); }
    }

    public static string SO100_title
    {
      get { return rm.GetString("SO100.title"); }
    }

    public static string SO100_sudoku
    {
      get { return rm.GetString("SO100.sudoku"); }
    }

    public static string SO100_anzahl
    {
      get { return rm.GetString("SO100.anzahl"); }
    }

    public static string SO100_zug
    {
      get { return rm.GetString("SO100.zug"); }
    }

    public static string SO100_zug_tt
    {
      get { return rm.GetString("SO100.zug.tt"); }
    }

    public static string SO100_loesen
    {
      get { return rm.GetString("SO100.loesen"); }
    }

    public static string SO100_loesen_tt
    {
      get { return rm.GetString("SO100.loesen.tt"); }
    }

    public static string SO100_test
    {
      get { return rm.GetString("SO100.test"); }
    }

    public static string SO100_test_tt
    {
      get { return rm.GetString("SO100.test.tt"); }
    }

    public static string SO100_diagonal
    {
      get { return rm.GetString("SO100.diagonal"); }
    }

    public static string SO100_diagonal_tt
    {
      get { return rm.GetString("SO100.diagonal.tt"); }
    }

    public static string SO100_leery
    {
      get { return rm.GetString("SO100.leery"); }
    }

    public static string TB100_title
    {
      get { return rm.GetString("TB100.title"); }
    }

    public static string TB100_before1
    {
      get { return rm.GetString("TB100.before1"); }
    }

    public static string TB100_before1_tt
    {
      get { return rm.GetString("TB100.before1.tt"); }
    }

    public static string TB100_before2
    {
      get { return rm.GetString("TB100.before2"); }
    }

    public static string TB100_before2_tt
    {
      get { return rm.GetString("TB100.before2.tt"); }
    }

    public static string TB100_before3
    {
      get { return rm.GetString("TB100.before3"); }
    }

    public static string TB100_before3_tt
    {
      get { return rm.GetString("TB100.before3.tt"); }
    }

    public static string TB100_after1
    {
      get { return rm.GetString("TB100.after1"); }
    }

    public static string TB100_after1_tt
    {
      get { return rm.GetString("TB100.after1.tt"); }
    }

    public static string TB100_after2
    {
      get { return rm.GetString("TB100.after2"); }
    }

    public static string TB100_after2_tt
    {
      get { return rm.GetString("TB100.after2.tt"); }
    }

    public static string TB100_after3
    {
      get { return rm.GetString("TB100.after3"); }
    }

    public static string TB100_after3_tt
    {
      get { return rm.GetString("TB100.after3.tt"); }
    }

    public static string TB100_date
    {
      get { return rm.GetString("TB100.date"); }
    }

    public static string TB100_date_tt
    {
      get { return rm.GetString("TB100.date.tt"); }
    }

    public static string TB100_entry
    {
      get { return rm.GetString("TB100.entry"); }
    }

    public static string TB100_entry_tt
    {
      get { return rm.GetString("TB100.entry.tt"); }
    }

    public static string TB100_positions
    {
      get { return rm.GetString("TB100.positions"); }
    }

    public static string TB100_positions_tt
    {
      get { return rm.GetString("TB100.positions.tt"); }
    }

    public static string TB100_positions_columns
    {
      get { return rm.GetString("TB100.positions.columns"); }
    }

    public static string TB100_new_tt
    {
      get { return rm.GetString("TB100.new.tt"); }
    }

    public static string TB100_remove_tt
    {
      get { return rm.GetString("TB100.remove.tt"); }
    }

    public static string TB100_posbefore
    {
      get { return rm.GetString("TB100.posbefore"); }
    }

    public static string TB100_posbefore_tt
    {
      get { return rm.GetString("TB100.posbefore.tt"); }
    }

    public static string TB100_position
    {
      get { return rm.GetString("TB100.position"); }
    }

    public static string TB100_position_tt
    {
      get { return rm.GetString("TB100.position.tt"); }
    }

    public static string TB100_add_tt
    {
      get { return rm.GetString("TB100.add.tt"); }
    }

    public static string TB100_search0
    {
      get { return rm.GetString("TB100.search0"); }
    }

    public static string TB100_clear_tt
    {
      get { return rm.GetString("TB100.clear.tt"); }
    }

    public static string TB100_search1
    {
      get { return rm.GetString("TB100.search1"); }
    }

    public static string TB100_search2
    {
      get { return rm.GetString("TB100.search2"); }
    }

    public static string TB100_search3
    {
      get { return rm.GetString("TB100.search3"); }
    }

    public static string TB100_search4
    {
      get { return rm.GetString("TB100.search4"); }
    }

    public static string TB100_search5
    {
      get { return rm.GetString("TB100.search5"); }
    }

    public static string TB100_search6
    {
      get { return rm.GetString("TB100.search6"); }
    }

    public static string TB100_search7
    {
      get { return rm.GetString("TB100.search7"); }
    }

    public static string TB100_search8
    {
      get { return rm.GetString("TB100.search8"); }
    }

    public static string TB100_search9
    {
      get { return rm.GetString("TB100.search9"); }
    }

    public static string TB100_search10
    {
      get { return rm.GetString("TB100.search10"); }
    }

    public static string TB100_search11
    {
      get { return rm.GetString("TB100.search11"); }
    }

    public static string TB100_search12
    {
      get { return rm.GetString("TB100.search12"); }
    }

    public static string TB100_position2
    {
      get { return rm.GetString("TB100.position2"); }
    }

    public static string TB100_position2_tt
    {
      get { return rm.GetString("TB100.position2.tt"); }
    }

    public static string TB100_from
    {
      get { return rm.GetString("TB100.from"); }
    }

    public static string TB100_from_tt
    {
      get { return rm.GetString("TB100.from.tt"); }
    }

    public static string TB100_to
    {
      get { return rm.GetString("TB100.to"); }
    }

    public static string TB100_to_tt
    {
      get { return rm.GetString("TB100.to.tt"); }
    }

    public static string TB100_first_tt
    {
      get { return rm.GetString("TB100.first.tt"); }
    }

    public static string TB100_back_tt
    {
      get { return rm.GetString("TB100.back.tt"); }
    }

    public static string TB100_forward_tt
    {
      get { return rm.GetString("TB100.forward.tt"); }
    }

    public static string TB100_last_tt
    {
      get { return rm.GetString("TB100.last.tt"); }
    }

    public static string TB100_save_tt
    {
      get { return rm.GetString("TB100.save.tt"); }
    }

    public static string TB110_title
    {
      get { return rm.GetString("TB110.title"); }
    }

    public static string TB110_nr
    {
      get { return rm.GetString("TB110.nr"); }
    }

    public static string TB110_nr_tt
    {
      get { return rm.GetString("TB110.nr.tt"); }
    }

    public static string TB110_bezeichnung
    {
      get { return rm.GetString("TB110.bezeichnung"); }
    }

    public static string TB110_bezeichnung_tt
    {
      get { return rm.GetString("TB110.bezeichnung.tt"); }
    }

    public static string TB110_datum
    {
      get { return rm.GetString("TB110.datum"); }
    }

    public static string TB110_datum_tt
    {
      get { return rm.GetString("TB110.datum.tt"); }
    }

    public static string TB200_title
    {
      get { return rm.GetString("TB200.title"); }
    }

    public static string TB200_positions
    {
      get { return rm.GetString("TB200.positions"); }
    }

    public static string TB200_positions_tt
    {
      get { return rm.GetString("TB200.positions.tt"); }
    }

    public static string TB200_positions_columns
    {
      get { return rm.GetString("TB200.positions.columns"); }
    }

    public static string TB200_search
    {
      get { return rm.GetString("TB200.search"); }
    }

    public static string TB200_search_tt
    {
      get { return rm.GetString("TB200.search.tt"); }
    }

    public static string TB200_all
    {
      get { return rm.GetString("TB200.all"); }
    }

    public static string TB200_all_tt
    {
      get { return rm.GetString("TB200.all.tt"); }
    }

    public static string TB210_title
    {
      get { return rm.GetString("TB210.title"); }
    }

    public static string TB210_nr
    {
      get { return rm.GetString("TB210.nr"); }
    }

    public static string TB210_nr_tt
    {
      get { return rm.GetString("TB210.nr.tt"); }
    }

    public static string TB210_bezeichnung
    {
      get { return rm.GetString("TB210.bezeichnung"); }
    }

    public static string TB210_bezeichnung_tt
    {
      get { return rm.GetString("TB210.bezeichnung.tt"); }
    }

    public static string TB210_breite
    {
      get { return rm.GetString("TB210.breite"); }
    }

    public static string TB210_breite_tt
    {
      get { return rm.GetString("TB210.breite.tt"); }
    }

    public static string TB210_laenge
    {
      get { return rm.GetString("TB210.laenge"); }
    }

    public static string TB210_laenge_tt
    {
      get { return rm.GetString("TB210.laenge.tt"); }
    }

    public static string TB210_hoehe
    {
      get { return rm.GetString("TB210.hoehe"); }
    }

    public static string TB210_hoehe_tt
    {
      get { return rm.GetString("TB210.hoehe.tt"); }
    }

    public static string TB210_notiz
    {
      get { return rm.GetString("TB210.notiz"); }
    }

    public static string TB210_notiz_tt
    {
      get { return rm.GetString("TB210.notiz.tt"); }
    }

    public static string WP100_title
    {
      get { return rm.GetString("WP100.title"); }
    }

    public static string WP100_daten
    {
      get { return rm.GetString("WP100.daten"); }
    }

    public static string WP100_daten_tt
    {
      get { return rm.GetString("WP100.daten.tt"); }
    }

    public static string WP100_chart
    {
      get { return rm.GetString("WP100.chart"); }
    }

    public static string WP100_daten_columns
    {
      get { return rm.GetString("WP100.daten.columns"); }
    }

    public static string WP100_von
    {
      get { return rm.GetString("WP100.von"); }
    }

    public static string WP100_von_tt
    {
      get { return rm.GetString("WP100.von.tt"); }
    }

    public static string WP100_bis
    {
      get { return rm.GetString("WP100.bis"); }
    }

    public static string WP100_bis_tt
    {
      get { return rm.GetString("WP100.bis.tt"); }
    }

    public static string WP100_wertpapier
    {
      get { return rm.GetString("WP100.wertpapier"); }
    }

    public static string WP100_wertpapier_tt
    {
      get { return rm.GetString("WP100.wertpapier.tt"); }
    }

    public static string WP100_box
    {
      get { return rm.GetString("WP100.box"); }
    }

    public static string WP100_box_tt
    {
      get { return rm.GetString("WP100.box.tt"); }
    }

    public static string WP100_skala_tt
    {
      get { return rm.GetString("WP100.skala.tt"); }
    }

    public static string WP100_umkehr
    {
      get { return rm.GetString("WP100.umkehr"); }
    }

    public static string WP100_umkehr_tt
    {
      get { return rm.GetString("WP100.umkehr.tt"); }
    }

    public static string WP100_methode
    {
      get { return rm.GetString("WP100.methode"); }
    }

    public static string WP100_methode_tt
    {
      get { return rm.GetString("WP100.methode.tt"); }
    }

    public static string WP100_relativ
    {
      get { return rm.GetString("WP100.relativ"); }
    }

    public static string WP100_relativ_tt
    {
      get { return rm.GetString("WP100.relativ.tt"); }
    }

    public static string WP200_title
    {
      get { return rm.GetString("WP200.title"); }
    }

    public static string WP200_wertpapiere
    {
      get { return rm.GetString("WP200.wertpapiere"); }
    }

    public static string WP200_wertpapiere_tt
    {
      get { return rm.GetString("WP200.wertpapiere.tt"); }
    }

    public static string WP200_wertpapiere_columns
    {
      get { return rm.GetString("WP200.wertpapiere.columns"); }
    }

    public static string WP200_status
    {
      get { return rm.GetString("WP200.status"); }
    }

    public static string WP200_bis
    {
      get { return rm.GetString("WP200.bis"); }
    }

    public static string WP200_bis_tt
    {
      get { return rm.GetString("WP200.bis.tt"); }
    }

    public static string WP200_alle
    {
      get { return rm.GetString("WP200.alle"); }
    }

    public static string WP200_alle_tt
    {
      get { return rm.GetString("WP200.alle.tt"); }
    }

    public static string WP200_berechnen
    {
      get { return rm.GetString("WP200.berechnen"); }
    }

    public static string WP200_berechnen_tt
    {
      get { return rm.GetString("WP200.berechnen.tt"); }
    }

    public static string WP200_bezeichnung
    {
      get { return rm.GetString("WP200.bezeichnung"); }
    }

    public static string WP200_bezeichnung_tt
    {
      get { return rm.GetString("WP200.bezeichnung.tt"); }
    }

    public static string WP200_muster
    {
      get { return rm.GetString("WP200.muster"); }
    }

    public static string WP200_muster_tt
    {
      get { return rm.GetString("WP200.muster.tt"); }
    }

    public static string WP200_auchinaktiv
    {
      get { return rm.GetString("WP200.auchinaktiv"); }
    }

    public static string WP200_auchinaktiv_tt
    {
      get { return rm.GetString("WP200.auchinaktiv.tt"); }
    }

    public static string WP200_konfiguration
    {
      get { return rm.GetString("WP200.konfiguration"); }
    }

    public static string WP200_konfiguration_tt
    {
      get { return rm.GetString("WP200.konfiguration.tt"); }
    }

    public static string WP210_title
    {
      get { return rm.GetString("WP210.title"); }
    }

    public static string WP210_nr
    {
      get { return rm.GetString("WP210.nr"); }
    }

    public static string WP210_nr_tt
    {
      get { return rm.GetString("WP210.nr.tt"); }
    }

    public static string WP210_bezeichnung
    {
      get { return rm.GetString("WP210.bezeichnung"); }
    }

    public static string WP210_bezeichnung_tt
    {
      get { return rm.GetString("WP210.bezeichnung.tt"); }
    }

    public static string WP210_provider
    {
      get { return rm.GetString("WP210.provider"); }
    }

    public static string WP210_provider_tt
    {
      get { return rm.GetString("WP210.provider.tt"); }
    }

    public static string WP210_kuerzel
    {
      get { return rm.GetString("WP210.kuerzel"); }
    }

    public static string WP210_kuerzel_tt
    {
      get { return rm.GetString("WP210.kuerzel.tt"); }
    }

    public static string WP210_status
    {
      get { return rm.GetString("WP210.status"); }
    }

    public static string WP210_status_tt
    {
      get { return rm.GetString("WP210.status.tt"); }
    }

    public static string WP210_aktKurs
    {
      get { return rm.GetString("WP210.aktKurs"); }
    }

    public static string WP210_aktKurs_tt
    {
      get { return rm.GetString("WP210.aktKurs.tt"); }
    }

    public static string WP210_stopKurs
    {
      get { return rm.GetString("WP210.stopKurs"); }
    }

    public static string WP210_stopKurs_tt
    {
      get { return rm.GetString("WP210.stopKurs.tt"); }
    }

    public static string WP210_signalKurs1
    {
      get { return rm.GetString("WP210.signalKurs1"); }
    }

    public static string WP210_signalKurs1_tt
    {
      get { return rm.GetString("WP210.signalKurs1.tt"); }
    }

    public static string WP210_muster
    {
      get { return rm.GetString("WP210.muster"); }
    }

    public static string WP210_muster_tt
    {
      get { return rm.GetString("WP210.muster.tt"); }
    }

    public static string WP210_typ
    {
      get { return rm.GetString("WP210.typ"); }
    }

    public static string WP210_typ_tt
    {
      get { return rm.GetString("WP210.typ.tt"); }
    }

    public static string WP210_waehrung
    {
      get { return rm.GetString("WP210.waehrung"); }
    }

    public static string WP210_waehrung_tt
    {
      get { return rm.GetString("WP210.waehrung.tt"); }
    }

    public static string WP210_sortierung
    {
      get { return rm.GetString("WP210.sortierung"); }
    }

    public static string WP210_sortierung_tt
    {
      get { return rm.GetString("WP210.sortierung.tt"); }
    }

    public static string WP210_relation
    {
      get { return rm.GetString("WP210.relation"); }
    }

    public static string WP210_relation_tt
    {
      get { return rm.GetString("WP210.relation.tt"); }
    }

    public static string WP210_notiz
    {
      get { return rm.GetString("WP210.notiz"); }
    }

    public static string WP210_notiz_tt
    {
      get { return rm.GetString("WP210.notiz.tt"); }
    }

    public static string WP210_anlage
    {
      get { return rm.GetString("WP210.anlage"); }
    }

    public static string WP210_anlage_tt
    {
      get { return rm.GetString("WP210.anlage.tt"); }
    }

    public static string WP250_title
    {
      get { return rm.GetString("WP250.title"); }
    }

    public static string WP250_anlagen
    {
      get { return rm.GetString("WP250.anlagen"); }
    }

    public static string WP250_anlagen_tt
    {
      get { return rm.GetString("WP250.anlagen.tt"); }
    }

    public static string WP250_anlagen_columns
    {
      get { return rm.GetString("WP250.anlagen.columns"); }
    }

    public static string WP250_anlagenStatus
    {
      get { return rm.GetString("WP250.anlagenStatus"); }
    }

    public static string WP250_bis
    {
      get { return rm.GetString("WP250.bis"); }
    }

    public static string WP250_bis_tt
    {
      get { return rm.GetString("WP250.bis.tt"); }
    }

    public static string WP250_bezeichnung
    {
      get { return rm.GetString("WP250.bezeichnung"); }
    }

    public static string WP250_bezeichnung_tt
    {
      get { return rm.GetString("WP250.bezeichnung.tt"); }
    }

    public static string WP250_wertpapier
    {
      get { return rm.GetString("WP250.wertpapier"); }
    }

    public static string WP250_wertpapier_tt
    {
      get { return rm.GetString("WP250.wertpapier.tt"); }
    }

    public static string WP250_alle
    {
      get { return rm.GetString("WP250.alle"); }
    }

    public static string WP250_alle_tt
    {
      get { return rm.GetString("WP250.alle.tt"); }
    }

    public static string WP250_berechnen
    {
      get { return rm.GetString("WP250.berechnen"); }
    }

    public static string WP250_berechnen_tt
    {
      get { return rm.GetString("WP250.berechnen.tt"); }
    }

    public static string WP250_auchinaktiv
    {
      get { return rm.GetString("WP250.auchinaktiv"); }
    }

    public static string WP250_auchinaktiv_tt
    {
      get { return rm.GetString("WP250.auchinaktiv.tt"); }
    }

    public static string WP260_title
    {
      get { return rm.GetString("WP260.title"); }
    }

    public static string WP260_nr
    {
      get { return rm.GetString("WP260.nr"); }
    }

    public static string WP260_nr_tt
    {
      get { return rm.GetString("WP260.nr.tt"); }
    }

    public static string WP260_wertpapier
    {
      get { return rm.GetString("WP260.wertpapier"); }
    }

    public static string WP260_wertpapier_tt
    {
      get { return rm.GetString("WP260.wertpapier.tt"); }
    }

    public static string WP260_wpdetails
    {
      get { return rm.GetString("WP260.wpdetails"); }
    }

    public static string WP260_wpdetails_tt
    {
      get { return rm.GetString("WP260.wpdetails.tt"); }
    }

    public static string WP260_bezeichnung
    {
      get { return rm.GetString("WP260.bezeichnung"); }
    }

    public static string WP260_bezeichnung_tt
    {
      get { return rm.GetString("WP260.bezeichnung.tt"); }
    }

    public static string WP260_status
    {
      get { return rm.GetString("WP260.status"); }
    }

    public static string WP260_status_tt
    {
      get { return rm.GetString("WP260.status.tt"); }
    }

    public static string WP260_depot
    {
      get { return rm.GetString("WP260.depot"); }
    }

    public static string WP260_depot_tt
    {
      get { return rm.GetString("WP260.depot.tt"); }
    }

    public static string WP260_abrechnung
    {
      get { return rm.GetString("WP260.abrechnung"); }
    }

    public static string WP260_abrechnung_tt
    {
      get { return rm.GetString("WP260.abrechnung.tt"); }
    }

    public static string WP260_ertrag
    {
      get { return rm.GetString("WP260.ertrag"); }
    }

    public static string WP260_ertrag_tt
    {
      get { return rm.GetString("WP260.ertrag.tt"); }
    }

    public static string WP260_notiz
    {
      get { return rm.GetString("WP260.notiz"); }
    }

    public static string WP260_notiz_tt
    {
      get { return rm.GetString("WP260.notiz.tt"); }
    }

    public static string WP260_daten
    {
      get { return rm.GetString("WP260.daten"); }
    }

    public static string WP260_daten_tt
    {
      get { return rm.GetString("WP260.daten.tt"); }
    }

    public static string WP260_stand
    {
      get { return rm.GetString("WP260.stand"); }
    }

    public static string WP260_stand_tt
    {
      get { return rm.GetString("WP260.stand.tt"); }
    }

    public static string WP300_title
    {
      get { return rm.GetString("WP300.title"); }
    }

    public static string WP300_konfigurationen
    {
      get { return rm.GetString("WP300.konfigurationen"); }
    }

    public static string WP300_konfigurationen_tt
    {
      get { return rm.GetString("WP300.konfigurationen.tt"); }
    }

    public static string WP300_konfigurationen_columns
    {
      get { return rm.GetString("WP300.konfigurationen.columns"); }
    }

    public static string WP310_title
    {
      get { return rm.GetString("WP310.title"); }
    }

    public static string WP310_nr
    {
      get { return rm.GetString("WP310.nr"); }
    }

    public static string WP310_nr_tt
    {
      get { return rm.GetString("WP310.nr.tt"); }
    }

    public static string WP310_bezeichnung
    {
      get { return rm.GetString("WP310.bezeichnung"); }
    }

    public static string WP310_bezeichnung_tt
    {
      get { return rm.GetString("WP310.bezeichnung.tt"); }
    }

    public static string WP310_box
    {
      get { return rm.GetString("WP310.box"); }
    }

    public static string WP310_box_tt
    {
      get { return rm.GetString("WP310.box.tt"); }
    }

    public static string WP310_skala_tt
    {
      get { return rm.GetString("WP310.skala.tt"); }
    }

    public static string WP310_umkehr
    {
      get { return rm.GetString("WP310.umkehr"); }
    }

    public static string WP310_umkehr_tt
    {
      get { return rm.GetString("WP310.umkehr.tt"); }
    }

    public static string WP310_methode
    {
      get { return rm.GetString("WP310.methode"); }
    }

    public static string WP310_methode_tt
    {
      get { return rm.GetString("WP310.methode.tt"); }
    }

    public static string WP310_dauer
    {
      get { return rm.GetString("WP310.dauer"); }
    }

    public static string WP310_dauer_tt
    {
      get { return rm.GetString("WP310.dauer.tt"); }
    }

    public static string WP310_relativ
    {
      get { return rm.GetString("WP310.relativ"); }
    }

    public static string WP310_relativ_tt
    {
      get { return rm.GetString("WP310.relativ.tt"); }
    }

    public static string WP310_status
    {
      get { return rm.GetString("WP310.status"); }
    }

    public static string WP310_status_tt
    {
      get { return rm.GetString("WP310.status.tt"); }
    }

    public static string WP310_notiz
    {
      get { return rm.GetString("WP310.notiz"); }
    }

    public static string WP310_notiz_tt
    {
      get { return rm.GetString("WP310.notiz.tt"); }
    }

    public static string WP400_title
    {
      get { return rm.GetString("WP400.title"); }
    }

    public static string WP400_buchungen
    {
      get { return rm.GetString("WP400.buchungen"); }
    }

    public static string WP400_buchungen_tt
    {
      get { return rm.GetString("WP400.buchungen.tt"); }
    }

    public static string WP400_buchungen_columns
    {
      get { return rm.GetString("WP400.buchungen.columns"); }
    }

    public static string WP400_bezeichnung
    {
      get { return rm.GetString("WP400.bezeichnung"); }
    }

    public static string WP400_bezeichnung_tt
    {
      get { return rm.GetString("WP400.bezeichnung.tt"); }
    }

    public static string WP400_anlage
    {
      get { return rm.GetString("WP400.anlage"); }
    }

    public static string WP400_anlage_tt
    {
      get { return rm.GetString("WP400.anlage.tt"); }
    }

    public static string WP400_alle
    {
      get { return rm.GetString("WP400.alle"); }
    }

    public static string WP400_alle_tt
    {
      get { return rm.GetString("WP400.alle.tt"); }
    }

    public static string WP410_title
    {
      get { return rm.GetString("WP410.title"); }
    }

    public static string WP410_nr
    {
      get { return rm.GetString("WP410.nr"); }
    }

    public static string WP410_nr_tt
    {
      get { return rm.GetString("WP410.nr.tt"); }
    }

    public static string WP410_anlage
    {
      get { return rm.GetString("WP410.anlage"); }
    }

    public static string WP410_anlage_tt
    {
      get { return rm.GetString("WP410.anlage.tt"); }
    }

    public static string WP410_valuta
    {
      get { return rm.GetString("WP410.valuta"); }
    }

    public static string WP410_valuta_tt
    {
      get { return rm.GetString("WP410.valuta.tt"); }
    }

    public static string WP410_preis
    {
      get { return rm.GetString("WP410.preis"); }
    }

    public static string WP410_preis_tt
    {
      get { return rm.GetString("WP410.preis.tt"); }
    }

    public static string WP410_betrag
    {
      get { return rm.GetString("WP410.betrag"); }
    }

    public static string WP410_betrag_tt
    {
      get { return rm.GetString("WP410.betrag.tt"); }
    }

    public static string WP410_rabatt
    {
      get { return rm.GetString("WP410.rabatt"); }
    }

    public static string WP410_rabatt_tt
    {
      get { return rm.GetString("WP410.rabatt.tt"); }
    }

    public static string WP410_anteile
    {
      get { return rm.GetString("WP410.anteile"); }
    }

    public static string WP410_anteile_tt
    {
      get { return rm.GetString("WP410.anteile.tt"); }
    }

    public static string WP410_preis2
    {
      get { return rm.GetString("WP410.preis2"); }
    }

    public static string WP410_preis2_tt
    {
      get { return rm.GetString("WP410.preis2.tt"); }
    }

    public static string WP410_zinsen
    {
      get { return rm.GetString("WP410.zinsen"); }
    }

    public static string WP410_zinsen_tt
    {
      get { return rm.GetString("WP410.zinsen.tt"); }
    }

    public static string WP410_bText
    {
      get { return rm.GetString("WP410.bText"); }
    }

    public static string WP410_bText_tt
    {
      get { return rm.GetString("WP410.bText.tt"); }
    }

    public static string WP410_buchung
    {
      get { return rm.GetString("WP410.buchung"); }
    }

    public static string WP410_buchung_tt
    {
      get { return rm.GetString("WP410.buchung.tt"); }
    }

    public static string WP410_hhbuchung
    {
      get { return rm.GetString("WP410.hhbuchung"); }
    }

    public static string WP410_hhbuchung_tt
    {
      get { return rm.GetString("WP410.hhbuchung.tt"); }
    }

    public static string WP410_hhaendern
    {
      get { return rm.GetString("WP410.hhaendern"); }
    }

    public static string WP410_hhaendern_tt
    {
      get { return rm.GetString("WP410.hhaendern.tt"); }
    }

    public static string WP410_hhstorno
    {
      get { return rm.GetString("WP410.hhstorno"); }
    }

    public static string WP410_hhstorno_tt
    {
      get { return rm.GetString("WP410.hhstorno.tt"); }
    }

    public static string WP410_hhvaluta
    {
      get { return rm.GetString("WP410.hhvaluta"); }
    }

    public static string WP410_hhvaluta_tt
    {
      get { return rm.GetString("WP410.hhvaluta.tt"); }
    }

    public static string WP410_hhbetrag
    {
      get { return rm.GetString("WP410.hhbetrag"); }
    }

    public static string WP410_hhbetrag_tt
    {
      get { return rm.GetString("WP410.hhbetrag.tt"); }
    }

    public static string WP410_hhereignis
    {
      get { return rm.GetString("WP410.hhereignis"); }
    }

    public static string WP410_hhereignis_tt
    {
      get { return rm.GetString("WP410.hhereignis.tt"); }
    }

    public static string WP410_hhereignis_columns
    {
      get { return rm.GetString("WP410.hhereignis.columns"); }
    }

    public static string WP500_title
    {
      get { return rm.GetString("WP500.title"); }
    }

    public static string WP500_staende
    {
      get { return rm.GetString("WP500.staende"); }
    }

    public static string WP500_staende_tt
    {
      get { return rm.GetString("WP500.staende.tt"); }
    }

    public static string WP500_staende_columns
    {
      get { return rm.GetString("WP500.staende.columns"); }
    }

    public static string WP500_wertpapier
    {
      get { return rm.GetString("WP500.wertpapier"); }
    }

    public static string WP500_wertpapier_tt
    {
      get { return rm.GetString("WP500.wertpapier.tt"); }
    }

    public static string WP500_alle
    {
      get { return rm.GetString("WP500.alle"); }
    }

    public static string WP500_alle_tt
    {
      get { return rm.GetString("WP500.alle.tt"); }
    }

    public static string WP500_von
    {
      get { return rm.GetString("WP500.von"); }
    }

    public static string WP500_von_tt
    {
      get { return rm.GetString("WP500.von.tt"); }
    }

    public static string WP500_bis
    {
      get { return rm.GetString("WP500.bis"); }
    }

    public static string WP500_bis_tt
    {
      get { return rm.GetString("WP500.bis.tt"); }
    }

    public static string WP510_title
    {
      get { return rm.GetString("WP510.title"); }
    }

    public static string WP510_wertpapier
    {
      get { return rm.GetString("WP510.wertpapier"); }
    }

    public static string WP510_wertpapier_tt
    {
      get { return rm.GetString("WP510.wertpapier.tt"); }
    }

    public static string WP510_valuta
    {
      get { return rm.GetString("WP510.valuta"); }
    }

    public static string WP510_valuta_tt
    {
      get { return rm.GetString("WP510.valuta.tt"); }
    }

    public static string WP510_betrag
    {
      get { return rm.GetString("WP510.betrag"); }
    }

    public static string WP510_betrag_tt
    {
      get { return rm.GetString("WP510.betrag.tt"); }
    }
  }
}
