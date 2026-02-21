using System;
using TMPro;
using UnityEngine;

public static class InteractionState
{
    public enum Language
    {
        English,
        Russian
    }

    public static Language language = Language.English;
    public static TMP_FontAsset currentFont;

    public static event Action OnLanguageChanged;

    public static void SetLanguage(Language newLanguage, TMP_FontAsset font)
    {
        language = newLanguage;
        currentFont = font;
        OnLanguageChanged?.Invoke();
    }

    [HideInInspector] public static bool isDraggingCard = false;
    [HideInInspector] public static bool showTutorial = true;
}