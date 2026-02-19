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
    [SerializeField] private List<string> sentencesOnEnglish;
    [SerializeField] private List<string> sentencesOnRussian;

    [Header("Settings")]
    [SerializeField] private float typeSpeed = 0.05f;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float scalePunch = 0.05f;

    [SerializeField] private float swayAmount = 0.5f; // градусы поворота
    [SerializeField] private float swayDuration = 2f; // время полного покачивания

    [Space]
    [Header("Sounds")]
    [SerializeField] private AudioClip letterTypeSound;
    [SerializeField] private AudioClip sentenceEndSound;
    [Range(0f, 1f)][SerializeField] private float letterAndSentenceSoundVolume = 0.075f;
    [SerializeField] private AudioClip transitionSound;
    [Range(0f, 1f)][SerializeField] private float transitionSoundVolume = 0.125f;

    [Space]
    [Header("Other")]
    [SerializeField] private TutorialManager tutorialManager;

    [Header("Sound")]
    [SerializeField] private AudioClip allTimeMusic;
    [Range(0f, 1f)][SerializeField] private float allTimeMusicVolume;

    private int currentSentence = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    private bool waitingForNext = false;
    private Tween swayTween;

    private bool sentenceSoundPlayed = false;

    private void Start()
    {
        SoundManager.Instance.PlayMusic(allTimeMusic, allTimeMusicVolume);
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, 0.5f);
        StartCoroutine(PlayIntro());
        textField.font = InteractionState.currentFont;
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (isTyping)
            {
                if (typingCoroutine != null)
                    StopCoroutine(typingCoroutine);

                if (InteractionState.language == InteractionState.Language.English)
                    textField.text = sentencesOnEnglish[currentSentence];

                else if (InteractionState.language == InteractionState.Language.Russian)
                    textField.text = sentencesOnRussian[currentSentence];

                isTyping = false;
                waitingForNext = true;

                textField.transform.DOPunchScale(Vector3.one * scalePunch, 0.2f, 1, 0.5f);

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
                    if (InteractionState.language == InteractionState.Language.English)
                    {
                        typingCoroutine = StartCoroutine(TypeSentence(sentencesOnEnglish[currentSentence]));
                    }
                    else if (InteractionState.language == InteractionState.Language.Russian)
                    {
                        typingCoroutine = StartCoroutine(TypeSentence(sentencesOnRussian[currentSentence]));
                    }
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

        if (sentencesOnEnglish.Count > 0)
        {
            if (InteractionState.language == InteractionState.Language.English)
            {
                typingCoroutine = StartCoroutine(TypeSentence(sentencesOnEnglish[currentSentence]));
            }
            else if (InteractionState.language == InteractionState.Language.Russian)
            {
                typingCoroutine = StartCoroutine(TypeSentence(sentencesOnRussian[currentSentence]));
            }
        }
    }

    private IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        textField.text = "";
        textField.alpha = 0f;

        textField.DOFade(1f, fadeDuration);

        sentenceSoundPlayed = false;

        foreach (char letter in sentence)
        {
            textField.text += letter;

            if (letterTypeSound != null && letter != ' ')
            {
                float randPitch = Random.Range(0.85f, 1.15f);
                SoundManager.Instance.PlaySFX(letterTypeSound, randPitch, letterAndSentenceSoundVolume);
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
        if (swayTween != null)
            swayTween.Kill();

        textField.transform.localRotation = Quaternion.Euler(Vector3.zero);
        swayTween = textField.transform.DOLocalRotate(
            new Vector3(0f, 0f, swayAmount),
            swayDuration / 2f
        )
        .From(new Vector3(0f, 0f, -swayAmount))
        .SetLoops(-1, LoopType.Yoyo)
        .SetEase(Ease.InOutSine);
    }

    public void OnIntroCompleted()
    {
        if (transitionSound != null)
            SoundManager.Instance.PlaySFX(transitionSound, 1f, transitionSoundVolume);

        if (swayTween != null)
            swayTween.Kill();

        Camera.main.GetComponent<CameraShake>().OnShake(0.5f, 0.3f);
        allGameElements.SetActive(true);
        tutorialManager.StartTutorial();
        gameObject.SetActive(false);
    }
}
