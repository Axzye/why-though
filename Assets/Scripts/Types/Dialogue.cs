using System;
using UnityEngine;

[Serializable]
public struct DlStory
{
    public DlPage[] pages;
}

[Serializable]
public struct DlPage
{
    [TextArea(5, 20)]
    public string text;
    public DlPersonData speaker;
    public DlEmotion emotion;
}

public enum DlEmotion
{
    None, Happy, Sad, Angry, Other
}