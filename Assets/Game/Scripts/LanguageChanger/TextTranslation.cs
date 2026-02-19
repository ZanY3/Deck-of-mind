using TMPro;
using UnityEngine;

public class TextTranslation : MonoBehaviour
{
    [SerializeField] private string russianVariant;
    [SerializeField] private string englishVariant;

    private TMP_Text textComponent;

    private void Awake()
    {
        textComponent = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        InteractionState.OnLanguageChanged += UpdateLanguage;
        UpdateLanguage();
    }

    private void OnDisable()
    {
        InteractionState.OnLanguageChanged -= UpdateLanguage;
    }

    private void UpdateLanguage()
    {
        if (InteractionState.language == InteractionState.Language.English)
            textComponent.text = englishVariant;
        else
            textComponent.text = russianVariant;

        if (InteractionState.currentFont != null)
            textComponent.font = InteractionState.currentFont;
    }
}
