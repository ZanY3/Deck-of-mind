using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LanguageManager : MonoBehaviour
{
    [SerializeField] private TMP_FontAsset englishFont;
    [SerializeField] private TMP_FontAsset russianFont;

    [Header("Sounds")]
    [SerializeField] private AudioClip clickSound;

    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    private void Start()
    {
        ChangeLanguage(InteractionState.Language.English);
    }

    public void ChangeLanguageToRussian()
    {
        ChangeLanguage(InteractionState.Language.Russian);
    }

    public void ChangeLanguageToEnglish()
    {
        ChangeLanguage(InteractionState.Language.English);
    }

    private void ChangeLanguage(InteractionState.Language language)
    {
        if (source && clickSound)
            source.PlayOneShot(clickSound);

        TMP_FontAsset font = language == InteractionState.Language.Russian
            ? russianFont
            : englishFont;

        InteractionState.SetLanguage(language, font);
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("Game");
    }
}