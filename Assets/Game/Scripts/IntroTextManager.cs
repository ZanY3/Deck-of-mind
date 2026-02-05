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

    private int currentSentence = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    private bool waitingForNext = false;

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
                // Если текст печатается — показываем весь
                if (typingCoroutine != null)
                {
                    StopCoroutine(typingCoroutine);
                    textField.text = sentences[currentSentence];
                    isTyping = false;
                    waitingForNext = true;
                }
            }
            else if (waitingForNext)
            {
                // Если текст уже напечатан — идём к следующему
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
        // Ждём пока экран полностью проявится
        yield return new WaitForSeconds(0.5f);

        if (sentences.Count > 0)
            typingCoroutine = StartCoroutine(TypeSentence(sentences[currentSentence]));
    }

    private IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        textField.text = "";

        foreach (char letter in sentence)
        {
            textField.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;
        waitingForNext = true;
    }

    public void OnIntroCompleted()
    {
        Camera.main.GetComponent<CameraShake>().OnShake(1f, 0.5f);
        allGameElements.SetActive(true);
        gameObject.SetActive(false);
    }
}
