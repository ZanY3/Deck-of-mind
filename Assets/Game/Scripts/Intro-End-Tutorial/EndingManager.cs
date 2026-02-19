using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndingManager : MonoBehaviour
{
    [Header("UI elements")]
    [SerializeField] private TMP_Text textField;
    [SerializeField] private Image slideImage;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image fadeImage;

    [Header("Text & Images")]
    [SerializeField] private List<string> sentencesOnEnglish;
    [SerializeField] private List<string> sentencesOnRussian;
    [SerializeField] private List<Sprite> images;

    [Header("Settings")]
    [SerializeField] private float typeSpeed = 0.05f;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float scalePunch = 0.05f;

    [Header("Sway")]
    [SerializeField] private float swayAmount = 0.5f;
    [SerializeField] private float swayDuration = 2f;

    [Header("Image FX")]
    [SerializeField] private float zoomAmount = 1.05f;
    [SerializeField] private float zoomDuration = 6f;

    [Header("Fade to White & Scene")]
    [SerializeField] private float fadeToWhiteDuration = 2f;
    [SerializeField] private string sceneToLoadName = "Game";

    [Header("Title After Ending")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private float titleFadeDuration = 1f;
    [SerializeField] private float titleDisplayDuration = 1.5f;

    [Header("Sounds")]
    [SerializeField] private AudioClip letterTypeSound;
    [SerializeField] private AudioClip sentenceEndSound;
    [SerializeField] private AudioClip endChoirSound;
    [Range(0f, 1f)][SerializeField] private float letterAndSentenceSoundVolume = 0.075f;
    [Range(0f, 1f)][SerializeField] private float endChoirVolume;


    private int currentSentence = 0;
    private bool isTyping = false;
    private bool waitingForNext = false;
    private Coroutine typingCoroutine;

    private bool sentenceSoundPlayed = false; // флаг, чтобы звук конца предложения не дублировался

    private Tween swayTween;
    private Tween zoomTween;

    private void Start()
    {
        textField.font = InteractionState.currentFont;
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, 0.5f);

        fadeImage.color = new Color(1f, 1f, 1f, 0f);
        UpdateImage();

        if (titleText != null)
        {
            Color c = titleText.color;
            titleText.color = new Color(c.r, c.g, c.b, 0f);
            titleText.gameObject.SetActive(false);
        }

        Color tc = textField.color;
        textField.color = new Color(tc.r, tc.g, tc.b, 0f);

        if(InteractionState.language == InteractionState.Language.English)
        {
            typingCoroutine = StartCoroutine(TypeSentence(sentencesOnEnglish[currentSentence]));
        }
        else if(InteractionState.language == InteractionState.Language.Russian)
        {
            typingCoroutine = StartCoroutine(TypeSentence(sentencesOnRussian[currentSentence]));
        }
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (isTyping)
            {
                StopCoroutine(typingCoroutine);

                if(InteractionState.language == InteractionState.Language.English)
                {
                    textField.text = sentencesOnEnglish[currentSentence];
                }
                else if(InteractionState.language == InteractionState.Language.Russian)
                {
                    textField.text = sentencesOnRussian[currentSentence];
                }
                isTyping = false;
                waitingForNext = true;

                textField.transform.DOPunchScale(Vector3.one * scalePunch, 0.2f);

                if (!sentenceSoundPlayed && sentenceEndSound != null)
                {
                    SoundManager.Instance.PlaySFX(sentenceEndSound, 1f, letterAndSentenceSoundVolume);
                    sentenceSoundPlayed = true;
                }

                StartSway();
            }
            else if (waitingForNext)
            {
                waitingForNext = false;
                currentSentence++;

                if (currentSentence < sentencesOnEnglish.Count)
                {
                    if(InteractionState.language == InteractionState.Language.English)
                    {
                        typingCoroutine = StartCoroutine(TypeSentence(sentencesOnEnglish[currentSentence]));
                    }
                    else if(InteractionState.language == InteractionState.Language.Russian)
                    {
                        typingCoroutine = StartCoroutine(TypeSentence(sentencesOnRussian[currentSentence]));
                    }
                    UpdateImage();
                }
                else
                {
                    OnEndingCompleted();
                }
            }
        }
    }

    private IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        textField.text = "";

        Color c = textField.color;
        textField.color = new Color(c.r, c.g, c.b, 0f);
        textField.DOColor(new Color(c.r, c.g, c.b, 1f), fadeDuration);

        sentenceSoundPlayed = false;

        foreach (char letter in sentence)
        {
            textField.text += letter;

            if (letterTypeSound != null && letter != ' ')
            {
                float randomPitch = Random.Range(0.85f, 1.15f);
                SoundManager.Instance.PlaySFX(letterTypeSound, randomPitch, letterAndSentenceSoundVolume);
            }

            yield return new WaitForSeconds(typeSpeed);
        }

        if (!sentenceSoundPlayed && sentenceEndSound != null)
        {
            SoundManager.Instance.PlaySFX(sentenceEndSound, 1f, letterAndSentenceSoundVolume);
            sentenceSoundPlayed = true;
        }

        StartSway();

        isTyping = false;
        waitingForNext = true;
    }

    private void StartSway()
    {
        swayTween?.Kill();

        textField.transform.localRotation = Quaternion.identity;

        swayTween = textField.transform.DOLocalRotate(
            new Vector3(0, 0, swayAmount),
            swayDuration / 2f
        )
        .From(new Vector3(0, 0, -swayAmount))
        .SetLoops(-1, LoopType.Yoyo)
        .SetEase(Ease.InOutSine);
    }

    private void UpdateImage()
    {
        if (currentSentence >= images.Count) return;

        slideImage.sprite = images[currentSentence];
        slideImage.color = new Color(1, 1, 1, 0);
        slideImage.DOFade(1f, 0.6f);

        zoomTween?.Kill();

        slideImage.transform.localScale = Vector3.one;

        zoomTween = slideImage.transform
            .DOScale(zoomAmount, zoomDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    private void OnEndingCompleted()
    {
        swayTween?.Kill();
        zoomTween?.Kill();
        SoundManager.Instance.PlaySFX(endChoirSound, 1, endChoirVolume);
        FadeToWhiteAndShowTitle();
    }

    private void FadeToWhiteAndShowTitle()
    {
        fadeImage.raycastTarget = true;
        fadeImage.color = new Color(1f, 1f, 1f, 0f);

        slideImage.transform.DOScale(1.1f, fadeToWhiteDuration);
        textField.transform.DOScale(1.05f, fadeToWhiteDuration);

        fadeImage.DOFade(1f, fadeToWhiteDuration)
            .OnComplete(() => StartCoroutine(ShowTitleAndLoadScene()));
    }

    private IEnumerator ShowTitleAndLoadScene()
    {
        slideImage.gameObject.SetActive(false);
        textField.gameObject.SetActive(false);

        if (titleText != null)
        {
            Color c = titleText.color;
            titleText.color = new Color(c.r, c.g, c.b, 0f);
            titleText.gameObject.SetActive(true);

            titleText.DOColor(new Color(c.r, c.g, c.b, 1f), titleFadeDuration);
        }

        yield return new WaitForSeconds(titleDisplayDuration);

        SceneManager.LoadScene(sceneToLoadName);
    }
}
