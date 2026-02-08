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
    [SerializeField] private Image fadeImage; // для белого перехода

    [Header("Text & Images")]
    [SerializeField] private List<string> sentences;
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
    [SerializeField] private TMP_Text titleText;      // "Deck of Mind"
    [SerializeField] private float titleFadeDuration = 1f;
    [SerializeField] private float titleDisplayDuration = 1.5f; // сколько держать перед загрузкой сцены

    private int currentSentence = 0;
    private bool isTyping = false;
    private bool waitingForNext = false;
    private Coroutine typingCoroutine;

    private Tween swayTween;
    private Tween zoomTween;

    private void Start()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, 0.5f);

        fadeImage.color = new Color(1f, 1f, 1f, 0f);
        UpdateImage();

        // Скрываем текст заголовка заранее
        if (titleText != null)
        {
            Color c = titleText.color;
            titleText.color = new Color(c.r, c.g, c.b, 0f);
            titleText.gameObject.SetActive(false);
        }

        // Подготовка текста концовки
        if (textField != null)
        {
            Color c = textField.color;
            textField.color = new Color(c.r, c.g, c.b, 0f); // скрываем текст через цвет
        }

        typingCoroutine = StartCoroutine(TypeSentence(sentences[currentSentence]));
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                textField.text = sentences[currentSentence];
                isTyping = false;
                waitingForNext = true;
                textField.transform.DOPunchScale(Vector3.one * scalePunch, 0.2f);
                StartSway();
            }
            else if (waitingForNext)
            {
                waitingForNext = false;
                currentSentence++;
                if (currentSentence < sentences.Count)
                {
                    typingCoroutine = StartCoroutine(TypeSentence(sentences[currentSentence]));
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

        // Скрываем через цвет
        Color startColor = textField.color;
        textField.color = new Color(startColor.r, startColor.g, startColor.b, 0f);

        // Появление текста
        textField.DOColor(new Color(startColor.r, startColor.g, startColor.b, 1f), fadeDuration);

        foreach (char letter in sentence)
        {
            textField.text += letter;
            yield return new WaitForSeconds(typeSpeed);
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

        // Плавный zoom (дыхание)
        zoomTween = slideImage.transform.DOScale(zoomAmount, zoomDuration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

    private void OnEndingCompleted()
    {
        swayTween?.Kill();
        zoomTween?.Kill();

        FadeToWhiteAndShowTitle();
    }

    private void FadeToWhiteAndShowTitle()
    {
        fadeImage.raycastTarget = true;
        fadeImage.color = new Color(1f, 1f, 1f, 0f);

        slideImage.transform.DOScale(1.1f, fadeToWhiteDuration).SetEase(Ease.OutQuad);
        textField.transform.DOScale(1.05f, fadeToWhiteDuration).SetEase(Ease.OutQuad);

        fadeImage.DOFade(1f, fadeToWhiteDuration).OnComplete(() =>
        {
            StartCoroutine(ShowTitleAndLoadScene());
        });
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

            // Плавное появление без масштабирования
            titleText.DOColor(new Color(c.r, c.g, c.b, 1f), titleFadeDuration);
        }

        yield return new WaitForSeconds(titleDisplayDuration);

        SceneManager.LoadScene(sceneToLoadName);
    }
}
