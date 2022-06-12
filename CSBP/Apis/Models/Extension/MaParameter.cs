// <copyright file="MaParameter.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models;

using System.ComponentModel.DataAnnotations.Schema;
using CSBP.Base;

/// <summary>
/// Entity class for table MA_Parameter.
/// </summary>
public partial class MaParameter : ModelBase
{
  /// <summary>Gets or sets the comment.</summary>
  [NotMapped]
  public string Comment { get; set; }

  /// <summary>Gets or sets the default value.</summary>
  [NotMapped]
  public string Default { get; set; }

  /// <summary>Gets or sets a value indicating whether the parameter is stored in the database.</summary>
  [NotMapped]
  public bool NotDatabase { get; set; }
}
