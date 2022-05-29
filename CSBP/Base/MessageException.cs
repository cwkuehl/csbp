// <copyright file="MessageException.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Base
{
  using System;

  public class MessageException : Exception
  {
    private Message Mess { get; set; }

    public MessageException(string m)
    {
      Mess = new Message(m);
    }

    public MessageException(Message m)
    {
      Mess = m;
    }

    public override string Message
    {
      get { return Mess.Text; }
    }

    public Message GetMessage()
    {
      return Mess;
    }
  }
}
