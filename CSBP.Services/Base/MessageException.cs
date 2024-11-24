// <copyright file="MessageException.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Base;

using System;

/// <summary>
/// Exception with Message.
/// </summary>
public class MessageException : Exception
{
  /// <summary>
  /// Initializes a new instance of the <see cref="MessageException"/> class.
  /// </summary>
  /// <param name="m">Text for message.</param>
  public MessageException(string m)
  {
    Mess = new Message(m);
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="MessageException"/> class.
  /// </summary>
  /// <param name="m">Affected message.</param>
  public MessageException(Message m)
  {
    Mess = m;
  }

  /// <summary>
  /// Gets message text.
  /// </summary>
  public override string Message
  {
    get { return Mess.Text; }
  }

  /// <summary>
  /// Gets or sets containing message.
  /// </summary>
  private Message Mess { get; set; }

  /// <summary>
  /// Gets the containing message.
  /// </summary>
  /// <returns>Containing message.</returns>
  public Message GetMessage()
  {
    return Mess;
  }
}
