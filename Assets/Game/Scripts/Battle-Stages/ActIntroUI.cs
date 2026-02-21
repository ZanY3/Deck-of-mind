using UnityEngine;
using TMPro;
using DG.Tweening;

public class ActIntroUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup panelCanvasGroup;
    [SerializeField] private TMP_Text actIndexTxt;
    [SerializeField] private TMP_Text actNameTxt;

    [SerializeField] private float textFadeDuration = 0.6f;
    [SerializeField] private float showDelay = 0.3f;
    [SerializeField] private float showDuration = 1.6f;
    [SerializeField] private float fadeOutDuration = 0.9f;

    [SerializeField] private AudioClip actIntroSound;
    [SerializeField] private float soundVolume = 1f;

    private CanvasGroup textCanvasGroup;

    private void Awake()
    {
        textCanvasGroup = actIndexTxt.GetComponent<CanvasGroup>();
        if (textCanvasGroup == null)
            textCanvasGroup = actIndexTxt.gameObject.AddComponent<CanvasGroup>();

        gameObject.SetActive(false);
    }

    public void Show(ActData actData)
    {
        panelCanvasGroup.alpha = 1f;
        textCanvasGroup.alpha = 0f;

        gameObject.SetActive(true);
        transform.SetAsLastSibling();

        actIndexTxt.font = InteractionState.currentFont;
        actNameTxt.font = InteractionState.currentFont;
        if (InteractionState.language == InteractionState.Language.English)
        {
            actIndexTxt.text = "ACT " + actData.index;
            actNameTxt.text = actData.actNameOnEnglish;
        }
        else
        {
            actIndexTxt.text = "АКТ " + actData.index;
            actNameTxt.text = actData.actNameOnRussian;
        }

        actIndexTxt.transform.localScale = Vector3.one * 0.95f;
        actNameTxt.transform.localScale = Vector3.one * 0.95f;

        if (actIntroSound != null)
            SoundManager.Instance.PlaySFX(actIntroSound, 1f, soundVolume);

        Sequence seq = DOTween.Sequence();

        seq.AppendInterval(showDelay);

        seq.Append(textCanvasGroup.DOFade(1f, textFadeDuration));

        seq.Join(actIndexTxt.transform
            .DOScale(1f, textFadeDuration)
            .SetEase(Ease.OutCubic));

        seq.Join(actNameTxt.transform
            .DOScale(1f, textFadeDuration)
            .SetEase(Ease.OutCubic));

        seq.AppendInterval(showDuration);

        seq.Append(panelCanvasGroup.DOFade(0f, fadeOutDuration)
            .SetEase(Ease.InOutQuad));

        seq.OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}