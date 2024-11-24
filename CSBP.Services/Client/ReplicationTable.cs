// <copyright file="ReplicationTable.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services;

/// <summary>
/// Replication table.
/// </summary>
internal class ReplicationTable
{
  /// <summary>
  /// Initializes a new instance of the <see cref="ReplicationTable"/> class.
  /// </summary>
  /// <param name="name">Affected table name.</param>
  /// <param name="clientNumber">Affected client number.</param>
  /// <param name="primaryKey">Affected primary key as string.</param>
  /// <param name="delete">Deleting for replication or not.</param>
  /// <param name="copy">Copying for replication or not.</param>
  public ReplicationTable(string name, string clientNumber, string primaryKey,
    bool delete, bool copy)
  {
    Name = name;
    ClientNumber = clientNumber;
    PrimaryKey = primaryKey;
    Delete = delete;
    Copy = copy;
  }

  /// <summary>Gets or sets table name.</summary>
  public string Name { get; set; }

  /// <summary>Gets or sets the client number.</summary>
  public string ClientNumber { get; set; }

  /// <summary>Gets or sets the primary key.</summary>
  public string PrimaryKey { get; set; }

  /// <summary>Gets or sets a value indicating whether deleting for replication or not.</summary>
  public bool Delete { get; set; }

  /// <summary>Gets or sets a value indicating whether copying for replication or not.</summary>
  public bool Copy { get; set; }
}
