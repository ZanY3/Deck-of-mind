using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    [SerializeField] private int currentStage = 1;
    [SerializeField] private int numberOfStages;
    [SerializeField] private PlayerDefense playerDefense;

    [Header("Progress bar")]
    [SerializeField] private GameObject playerIcon;
    [SerializeField] private Image[] stagePointsImg;
    [SerializeField] private float playerMoveSpeed = 5f;

    [Header("Enemies")]
    [SerializeField] private List<GameObject> enemiesPrefabs;
    [SerializeField] private int[] stagesWithStrongEnemies;

    [Header("Managers/Objects")]
    [SerializeField] private GameObject endingPanel;
    [SerializeField] private HandManager handManager;
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private RectTransform enemySlotPos;
    [SerializeField] private GameObject cardRewardPanel;
    [SerializeField] private GameObject gameCanvas;
    [SerializeField] private CardRewardManager rewardManager;
    [SerializeField] private DeckManager deckManager;

    [Space]
    [Header("Sounds")]
    [SerializeField] private AudioClip allTimeMusic;
    [SerializeField] private AudioClip[] musicForBosses;
    [Range(0f, 1f)][SerializeField] private float allTimeMusicVolume;
    [Range(0f, 1f)][SerializeField] private float bossesMusicVolume;

    private float playerY; // фиксированная позиция по Y

    private void Start()
    {
        playerY = playerIcon.transform.position.y; // зафиксировали Y
        StartStage();
        playerIcon.transform.position = new Vector3(stagePointsImg[currentStage - 1].transform.position.x, playerY, 0);
    }

    public void WinBattle()
    {
        PlayAllGameMusic();
        if(currentStage == numberOfStages)
        {
            gameCanvas.SetActive(false);
            endingPanel.SetActive(true);
        }
        else
        {
            rewardManager.GetRewardCards(3);
            StartCoroutine(HandleLevelCompletion());
        }

    }

    private IEnumerator HandleLevelCompletion()
    {
        gameCanvas.SetActive(false);
        cardRewardPanel.SetActive(true);
        yield return new WaitUntil(() => rewardManager.hasChosenCard);
        cardRewardPanel.SetActive(false);
        gameCanvas.SetActive(true);

        // затемняем текущую точку
        int completedIndex = currentStage - 1;
        if (completedIndex >= 0 && completedIndex < stagePointsImg.Length)
        {
            Color c = stagePointsImg[completedIndex].color;
            c.a = 50f / 255f;
            stagePointsImg[completedIndex].color = c;
        }

        if (currentStage < numberOfStages)
        {
            currentStage++;
            Vector3 targetPos = new Vector3(stagePointsImg[currentStage - 1].transform.position.x, playerY, 0);
            StartCoroutine(MovePlayerIcon(targetPos));

            playerDefense.RemoveAllArmor();
            battleManager.StartBattle();
            StartStage();
        }
    }

    public void StartStage()
    {
        // музыка босса на стадии
        for (int i = 0; i < stagesWithStrongEnemies.Length; i++)
        {
            if (currentStage == stagesWithStrongEnemies[i])
            {
                int randNum = Random.Range(0, musicForBosses.Length);
                SoundManager.Instance.PlayMusic(musicForBosses[randNum], bossesMusicVolume);
            }
        }

        InteractionState.isDraggingCard = false;
        var enemy = Instantiate(enemiesPrefabs[currentStage - 1], enemySlotPos.position, Quaternion.identity);
        enemy.transform.SetParent(enemySlotPos.transform, false);
        handManager.DrawHand();
    }

    public void PlayAllGameMusic()
    {
        SoundManager.Instance.PlayMusic(allTimeMusic, allTimeMusicVolume, resume: true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Game");
    }

    private IEnumerator MovePlayerIcon(Vector3 targetPos)
    {
        Vector3 startPos = playerIcon.transform.position;
        float t = 0f;
        float distance = Vector3.Distance(startPos, targetPos);
        float duration = distance / playerMoveSpeed;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            playerIcon.transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        playerIcon.transform.position = targetPos;
    }
}
