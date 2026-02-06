using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine;

public class IntroTextManager : MonoBehaviour
{
    [Header("UI elements")]
    [SerializeField] private TMP_Text textField;
    [SerializeField] private GameObject allGameElements;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Text")]
    [SerializeField] private List<string> sentences;

    [Header("Settings")]
    [SerializeField] private float typeSpeed = 0.05f;
    [SerializeField] private float sentenceDelay = 1f;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float scalePunch = 0.05f;
    [SerializeField] private float swayAmount = 0.5f; // градусы поворота
    [SerializeField] private float swayDuration = 2f; // время полного покачивания

    private int currentSentence = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    private bool waitingForNext = false;
    private Tween swayTween;

    private void Start()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, 0.5f);
        StartCoroutine(PlayIntro());
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (isTyping)
            {
                if (typingCoroutine != null)
                    StopCoroutine(typingCoroutine);

                textField.text = sentences[currentSentence];
                isTyping = false;
                waitingForNext = true;

                textField.transform.DOPunchScale(Vector3.one * scalePunch, 0.2f, 1, 0.5f);
            }
            else if (waitingForNext)
            {
                waitingForNext = false;
                currentSentence++;
                if (currentSentence < sentences.Count)
                {
                    typingCoroutine = StartCoroutine(TypeSentence(sentences[currentSentence]));
                }
                else
                {
                    OnIntroCompleted();
                }
            }
        }
    }

    private IEnumerator PlayIntro()
    {
        yield return new WaitForSeconds(0.5f);

        if (sentences.Count > 0)
            typingCoroutine = StartCoroutine(TypeSentence(sentences[currentSentence]));
    }

    private IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        textField.text = "";
        textField.alpha = 0f;

        // Плавное появление текста без изменения глобального масштаба
        textField.DOFade(1f, fadeDuration);

        foreach (char letter in sentence)
        {
            textField.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }

        // Начинаем очень слабое покачивание текста
        if (swayTween != null)
            swayTween.Kill();

        swayTween = textField.transform.DOLocalRotate(
            new Vector3(0f, 0f, swayAmount),
            swayDuration / 2f
        ).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);

        isTyping = false;
        waitingForNext = true;
    }

    public void OnIntroCompleted()
    {
        if (swayTween != null)
            swayTween.Kill();

        Camera.main.GetComponent<CameraShake>().OnShake(0.5f, 0.3f);
        allGameElements.SetActive(true);
        gameObject.SetActive(false);
    }
}
