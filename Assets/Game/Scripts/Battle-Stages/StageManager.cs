using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    [SerializeField] private int currentStage = 1;
    [SerializeField] private PlayerDefense playerDefense;

    [Header("Progress bar")]
    [SerializeField] private GameObject playerIcon;
    [SerializeField] private float playerMoveSpeed = 5f;

    [Header("Managers/Objects")]
    [SerializeField] private GameObject endingPanel;
    [SerializeField] private HandManager handManager;
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private RectTransform enemySlotPos;
    [SerializeField] private GameObject cardRewardPanel;
    [SerializeField] private GameObject gameCanvas;
    [SerializeField] private CardRewardManager rewardManager;
    //[SerializeField] private GameObject winPanel;
    [SerializeField] private ActManager actManager;
    [SerializeField] private ActIntroUI actIntroUI;

    [Space]
    [Header("Sounds")]
    [SerializeField] private AudioClip allTimeMusic;
    [SerializeField] private AudioClip[] musicForBosses;
    [Range(0f, 1f)][SerializeField] private float allTimeMusicVolume;
    [Range(0f, 1f)][SerializeField] private float bossesMusicVolume;

    private float playerY;

    private int numberOfStages;
    private List<GameObject> enemiesPrefabs;
    private int[] stagesWithStrongEnemies;
    private Image[] stagePointsImg;

    private void Start()
    {
        InitAct();
    }

    private void InitAct()
    {
        //winPanel.SetActive(false);
        var act = actManager.GetCurrentAct();

        actIntroUI.Show(act);

        currentStage = 1;

        stagePointsImg = actManager.GetCurrentActStagePoints();
        numberOfStages = act.stagesAtAll;
        enemiesPrefabs = act.enemiesPrefabs;
        stagesWithStrongEnemies = act.stagesWithStrongEnemies;

        if (stagePointsImg == null || stagePointsImg.Length != numberOfStages)
        {
            Debug.LogError("StagePoints count does not match stages count");
            return;
        }

        playerY = playerIcon.transform.position.y;

        // сброс прозрачности точек
        foreach (var img in stagePointsImg)
        {
            Color c = img.color;
            c.a = 1f;
            img.color = c;
        }

        playerIcon.transform.position =
            new Vector3(stagePointsImg[0].transform.position.x, playerY, 0);

        ClearEnemies();
        StartStage();
    }

    public void WinBattle()
    {
        PlayAllGameMusic();

        if (currentStage == numberOfStages)
        {
            if (!actManager.IsActLast())
            {
                actManager.NextAct();
                InitAct();
            }
            else
            {
                gameCanvas.SetActive(false);
                endingPanel.SetActive(true);
            }
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

            Vector3 targetPos =
                new Vector3(stagePointsImg[currentStage - 1].transform.position.x, playerY, 0);

            StartCoroutine(MovePlayerIcon(targetPos));

            playerDefense.RemoveAllArmor();
            playerDefense.GetComponent<PlayerHealth>().ClearAllDebuffs();
            battleManager.StartBattle();

            ClearEnemies();
            StartStage();
        }
    }

    public void StartStage()
    {
        // музыка босса
        for (int i = 0; i < stagesWithStrongEnemies.Length; i++)
        {
            if (currentStage == stagesWithStrongEnemies[i])
            {
                int randNum = Random.Range(0, musicForBosses.Length);
                SoundManager.Instance.PlayMusic(musicForBosses[randNum], bossesMusicVolume);
            }
        }

        InteractionState.isDraggingCard = false;

        var enemy = Instantiate(
            enemiesPrefabs[currentStage - 1],
            enemySlotPos.position,
            Quaternion.identity
        );

        enemy.transform.SetParent(enemySlotPos, false);

        handManager.DrawHand();
        battleManager.StartBattle();
    }

    private void ClearEnemies()
    {
        foreach (Transform child in enemySlotPos)
        {
            Destroy(child.gameObject);
        }
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