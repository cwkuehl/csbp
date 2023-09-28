// <copyright file="AiData.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.NonService;

using System;
using System.Collections.Generic;

/// <summary>
/// Class for AI data.
/// </summary>
[Serializable]
public class AiData
{
  /// <summary>AI model constant for davinci.</summary>
  public const string Davinci = "text-davinci-003";

  /// <summary>AI model constant for GPT 3.5 (cheaper than davinci).</summary>
  public const string Gpt35 = "gpt-3.5-turbo";

  /// <summary>AI model constant for DALL E.</summary>
  public const string Dalle = "dalle";

  /// <summary>Gets or sets the AI model.</summary>
  public string Model { get; set; } = Gpt35;

  /// <summary>Gets or sets the maximal number of tokens.</summary>
  public int MaxTokens { get; set; } = 16;

  /// <summary>Gets or sets the temperature between 0 and 2, default 1.</summary>
  public decimal Temperature { get; set; } = 1;

  /// <summary>Gets or sets the top percentage for tokens, alternative to temperature, default 1.</summary>
  public decimal Topp { get; set; } = 1;

  /// <summary>Gets or sets the number of completions, default 1.</summary>
  public int N { get; set; } = 1;

  /// <summary>Gets or sets the input system prompt string for AI.</summary>
  public string SystemPrompt { get; set; }

  /// <summary>Gets or sets the input prompt string for AI.</summary>
  public string Prompt { get; set; }

  /// <summary>Gets the finish reasons: stop, length.</summary>
  public List<string> FinishReasons { get; private set; } = new();

  /// <summary>Gets the response messages.</summary>
  public List<string> Messages { get; private set; } = new();

  /// <summary>Gets response byte data arrays.</summary>
  public List<byte[]> Bytes { get; private set; } = new();

  /// <summary>Gets or sets the number of prompt tokens.</summary>
  public decimal PromptTokens { get; set; }

  /// <summary>Gets or sets the number of completion tokens.</summary>
  public decimal CompletionTokens { get; set; }
}
