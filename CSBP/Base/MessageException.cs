// <copyright file="MessageException.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Base
{
  using System;

  public class MessageException : Exception
  {
    private Message message { get; set; }

    public MessageException(string m)
    {
      message = new Message(m);
    }

    public MessageException(Message m)
    {
      message = m;
    }

    public override string Message
    {
      get { return message.Text; }
    }

    public Message GetMessage()
    {
      return message;
    }
  }
}
