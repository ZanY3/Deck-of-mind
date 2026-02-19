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

    private void Start()
    {
        source = GetComponent<AudioSource>();
        ChangeLanguageToEnglish();
    }

    public void ChangeLanguageToRussian()
    {
        source.PlayOneShot(clickSound);
        InteractionState.SetLanguage(InteractionState.Language.Russian, russianFont);
    }

    public void ChangeLanguageToEnglish()
    {
        source.PlayOneShot(clickSound);
        InteractionState.SetLanguage(InteractionState.Language.English, englishFont);
    }
    public void LoadGame()
    {
        SceneManager.LoadScene("Game");
    }
}
