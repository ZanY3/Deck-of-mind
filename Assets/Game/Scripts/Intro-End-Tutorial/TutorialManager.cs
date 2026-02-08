using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject handHint;
    [SerializeField] private GameObject enemyHint;
    [SerializeField] private GameObject endBtnHint;
    [SerializeField] private GameObject finalHint;

    private bool tutorialIsActive = false;
    private int step = 0;

    public void StartTutorial()
    {
        if (InteractionState.showTutorial)
        {
            step = 0;
            tutorialIsActive = true;
            handHint.SetActive(true);
            enemyHint.SetActive(false);
            endBtnHint.SetActive(false);
            finalHint.SetActive(false);
        }
        else
        {
            return;
        }
    }

    void Update()
    {
        if (!InteractionState.showTutorial) return;

        if (tutorialIsActive && Mouse.current.leftButton.wasPressedThisFrame)
        {
            NextStep();
        }
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
                gameObject.SetActive(false);
                tutorialIsActive = false;
                break;
        }
    }
}
