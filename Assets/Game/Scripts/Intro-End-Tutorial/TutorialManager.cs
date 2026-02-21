using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject handHint;
    [SerializeField] private GameObject enemyHint;
    [SerializeField] private GameObject endBtnHint;
    [SerializeField] private GameObject finalHint;

    [Header("Sounds")]
    [SerializeField] private AudioClip clickSound;
    [Range(0f, 1f)][SerializeField] private float clickSoundVolume = 0.1f;

    private bool tutorialIsActive = false;
    private int step = 0;


    private void Update()
    {
        if (!InteractionState.showTutorial)
            return;

        if (!tutorialIsActive)
        {
            StartTutorial();
        }

        if (tutorialIsActive && Mouse.current.leftButton.wasPressedThisFrame)
        {
            float randPitch = Random.Range(0.9f, 1.1f);
            SoundManager.Instance.PlaySFX(clickSound, randPitch, clickSoundVolume);
            NextStep();
        }
    }

    public void StartTutorial()
    {
        gameObject.SetActive(true);

        step = 0;
        tutorialIsActive = true;

        handHint.SetActive(true);
        enemyHint.SetActive(false);
        endBtnHint.SetActive(false);
        finalHint.SetActive(false);
    }

    void NextStep()
    {
        step++;

        switch (step)
        {
            case 1:
                handHint.SetActive(false);
                enemyHint.SetActive(true);
                break;

            case 2:
                enemyHint.SetActive(false);
                endBtnHint.SetActive(true);
                break;

            case 3:
                endBtnHint.SetActive(false);
                finalHint.SetActive(true);
                break;

            case 4:
                finalHint.SetActive(false);
                InteractionState.showTutorial = false;
                tutorialIsActive = false;
                gameObject.SetActive(false);
                break;
        }
    }
}