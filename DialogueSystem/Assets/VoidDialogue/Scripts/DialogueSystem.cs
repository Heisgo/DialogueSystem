using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Text.RegularExpressions;

#region ChatData
public class ChatData
{
    public string SpeakerName;

    public string FullMessage = string.Empty;
    public bool SkipAllowed = true;
    public UnityAction OnComplete;

    public List<ChatAction> Actions = new();
    public ChatOptionList Options = new();
    public ChatFormatting Formatting = new();

    public ChatData(string rawMessage, string speaker = "", UnityAction onComplete = null, bool allowSkip = true)
    {
        ProcessRawMessage(rawMessage);
        SpeakerName = speaker;
        OnComplete = onComplete;
        SkipAllowed = allowSkip;
    }

    private void ProcessRawMessage(string rawMessage)
    {
        string tempText = string.Empty;
        for (int i = 0; i < rawMessage.Length; i++)
        {
            if (rawMessage[i] != '/')
            {
                tempText += rawMessage[i];
            }
            else
            {
                if (!string.IsNullOrEmpty(tempText))
                {
                    Actions.Add(new ChatAction(ActionType.Display, tempText));
                    tempText = string.Empty;
                }

                int endDelimiter = rawMessage.IndexOf('/', i + 1);
                if (endDelimiter == -1)
                    break;

                string fragment = rawMessage.Substring(i + 1, endDelimiter - i - 1);
                ChatAction action = InterpretFragment(fragment);
                if (action != null)
                    Actions.Add(action);

                i = endDelimiter;
            }
        }

        if (!string.IsNullOrEmpty(tempText))
            Actions.Add(new ChatAction(ActionType.Display, tempText));
    }

    private ChatAction InterpretFragment(string fragment)
    {
        string[] parts = fragment.Split(':');
        if (Enum.TryParse(parts[0], out ActionType actType))
        {
            return parts.Length >= 2
                   ? new ChatAction(actType, parts[1])
                   : new ChatAction(actType);
        }
        else
        {
            Debug.LogError($"Failed to interpret {fragment}");
            return null;
        }
    }
}
#endregion
#region MoodData
[Serializable]
public class MoodData
{
    private Dictionary<string, Sprite> moodMapping;
    public Dictionary<string, Sprite> MoodMapping
    {
        get
        {
            if (moodMapping == null)
                InitializeMoodMapping();
            return moodMapping;
        }
    }

    public string[] EmotionLabels = new string[] { "Neutral" };
    public Sprite[] Images;

    private void InitializeMoodMapping()
    {
        moodMapping = new Dictionary<string, Sprite>();

        if (EmotionLabels.Length != Images.Length)
        {
            Debug.LogError("The Emotion and Sprite must have the same length");
            return;
        }

        for (int i = 0; i < EmotionLabels.Length; i++)
            moodMapping.Add(EmotionLabels[i], Images[i]);
    }
}
#endregion
#region Actions & Options
public class ChatAction
{
    public ActionType type;
    public string parameter;

    public ChatAction(ActionType type, string parameter = "")
    {
        this.type = type;
        this.parameter = parameter;
    }
}

public class ChatOptionList
{
    private readonly List<ChatOption> options = new();

    public int Count => options.Count;

    public ChatOption GetOption(int index) => options[index];

    public List<ChatOption> GetAllOptions() => options;

    public string GetValue(string key)
    {
        ChatOption option = options.Find(o => o.HasKey(key));
        return option != null ? option.Value : "";
    }

    public void Clear() => options.Clear();

    public void AddOption(string key, string value) => options.Add(new ChatOption(key, value));

    public void RemoveOption(string key)
    {
        ChatOption option = options.Find(o => o.HasKey(key));
        if (option != null)
            options.Remove(option);
    }
}

public class ChatOption
{
    public string Key;
    public string Value;

    public ChatOption(string key, string value)
    {
        Key = key;
        Value = value;
    }

    public bool HasKey(string key) => Key == key;
}
#endregion
#region ChatFormatting
public class ChatFormatting
{
    public string DefaultFontSize = "56";
    private readonly string baseColor = "white";

    private string currentColor;
    private string currentSize;

    public ChatFormatting(string fontSize = "", string fontColor = "")
    {
        currentColor = string.Empty;
        currentSize = string.Empty;

        if (!string.IsNullOrEmpty(fontSize))
            DefaultFontSize = fontSize;
        if (!string.IsNullOrEmpty(fontColor))
            baseColor = fontColor;
    }

    public string Color
    {
        set
        {
            if (ValidateColor(value))
            {
                currentColor = value;
                if (string.IsNullOrEmpty(currentSize))
                    currentSize = DefaultFontSize;
            }
        }
        get => currentColor;
    }

    public string Size
    {
        set
        {
            if (ValidateSize(value))
            {
                currentSize = value;
                if (string.IsNullOrEmpty(currentColor))
                    currentColor = baseColor;
            }
        }
        get => currentSize;
    }

    public string OpeningTag => (IsFormatted) ? $"<color={Color}><size={Size}>" : "";
    public string ClosingTag => (IsFormatted) ? "</size></color>" : "";

    public void ChangeSize(string instruction)
    {
        if (string.IsNullOrEmpty(currentSize))
            Size = DefaultFontSize;

        currentSize = instruction switch
        {
            "up" => (int.Parse(currentSize) + 10).ToString(),
            "down" => (int.Parse(currentSize) - 10).ToString(),
            "reset" => DefaultFontSize,
            _ => instruction,
        };
    }

    private bool IsFormatted => !string.IsNullOrEmpty(currentColor) && !string.IsNullOrEmpty(currentSize);

    private bool ValidateColor(string inputColor)
    {
        Regex hexPattern = new("^#(?:[0-9a-fA-F]{3}){1,2}$");
        return Enum.TryParse(inputColor, result: out ColorName parsedColor) || hexPattern.IsMatch(inputColor);
    }

    private bool ValidateSize(string inputSize)
    {
        return float.TryParse(inputSize, out _);
    }
}
#endregion
#region Enums
public enum ChatState
{
    Active,
    Paused,
    Inactive
}

public enum ActionType
{
    Display,
    Terminate,
    ChangeMood,
    PlayAudio,
    Delay,
    AdjustSpeed,
    OnClick,
    Resize,
    SetColor
}

public enum ColorName
{
    White,
    Yellow,
    Black,
    Blue,
    Brown,
    Magenta,
    Cyan,
    DarkBlue,
    Green,
    LightBlue,
    Gray,
    Aqua,
    Purple,
    Lime,
    Orange,
    Red
}
#endregion