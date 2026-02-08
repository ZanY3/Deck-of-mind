using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    [SerializeField] private int currentStage = 1;
    [SerializeField] private int numberOfStages;
    [SerializeField] private PlayerDefense playerDefense;

    [Space]
    [Header("Enemies indexes equal to stage enemies to spawn")]
    [SerializeField] private List<GameObject> enemiesPrefabs;

    [Space]
    [Header("Managers/Objects")]
    [SerializeField] private HandManager handManager;
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private RectTransform enemySlotPos;
    [SerializeField] private GameObject cardRewardPanel;
    [SerializeField] private GameObject gameCanvas;
    [SerializeField] private CardRewardManager rewardManager;
    [SerializeField] private DeckManager deckManager;

    private void Start()
    {
        StartStage();
    }
//---------------------------------------------------------------
    public void WinBattle()
    {
        rewardManager.GetRewardCards(3);
        if (currentStage != numberOfStages)
        {
            StartCoroutine(WaitForReward());
            currentStage++;
            playerDefense.RemoveAllArmor();
            battleManager.StartBattle();
            StartStage();
        }
        else
        {
            Debug.LogWarning("All stages was completed");
            RestartScene();
        }
    }
    IEnumerator WaitForReward()
    {
        gameCanvas.SetActive(false);
        cardRewardPanel.SetActive(true);
        
        yield return new WaitUntil(() => rewardManager.hasChosenCard);

        cardRewardPanel.SetActive(false);
        gameCanvas.SetActive(true);
    }

    public void StartStage()
    {
        InteractionState.isDraggingCard = false;
        var enemy = Instantiate(enemiesPrefabs[currentStage - 1], enemySlotPos.position, Quaternion.identity);
        enemy.transform.SetParent(enemySlotPos.transform, false);
        handManager.DrawHand();
    }
    public void RestartScene()
    {
        SceneManager.LoadScene("Game");
    }
}
