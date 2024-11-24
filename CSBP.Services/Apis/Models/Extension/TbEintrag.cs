// <copyright file="TbEintrag.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Apis.Models;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CSBP.Services.Base;

/// <summary>
/// Entity class for table TB_Eintrag.
/// </summary>
public partial class TbEintrag : ModelBase
{
  /// <summary>Gets or sets the positions.</summary>
  [NotMapped]
  public List<TbEintragOrt> Positions { get; set; }
}
