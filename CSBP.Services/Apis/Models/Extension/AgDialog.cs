// <copyright file="AgDialog.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Apis.Models;

using System.ComponentModel.DataAnnotations.Schema;
using CSBP.Services.Base;
using CSBP.Services.NonService;

/// <summary>
/// Entity class for table AG_Dialog.
/// </summary>
public partial class AgDialog : ModelBase
{
  /// <summary>Gets or sets the data.</summary>
  [NotMapped]
  public AiData Data { get; set; }
}
