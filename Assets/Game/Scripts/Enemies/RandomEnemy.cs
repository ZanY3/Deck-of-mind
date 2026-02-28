using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RandomEnemy : MonoBehaviour
{
    [SerializeField] private EnemyData[] enemyList;

    [Space]
    [Header("Only for debuffer")]
    [SerializeField] private Color colorForStunner;
    [SerializeField] private Color shadowColorForStunner;
    [SerializeField] private Image shadowImg;
    [SerializeField] private TMP_Text typeTxt;
    [SerializeField] private GameObject anxietyIcon;
    private Enemy enemy;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        enemy.enemyData = enemyList[Random.Range(0, enemyList.Length)];
        if (enemy.enemyData.enemyType == EnemyData.EnemyType.Debuffer && enemy.enemyData.nameOnEnglish == "Paralysis")
        {
            Destroy(GetComponent<AnxietyDebuff>());
            Destroy(GetComponent<BurnoutDebuff>());
            Destroy(GetComponent<FearDebuff>());
            shadowImg.color = shadowColorForStunner;
            typeTxt.color = colorForStunner;
            anxietyIcon.SetActive(false);
        }
        if (enemy.enemyData.enemyType == EnemyData.EnemyType.Debuffer && enemy.enemyData.nameOnEnglish == "Burnout")
        {
            Destroy(GetComponent<StunDebuff>());
            Destroy(GetComponent<AnxietyDebuff>());
            Destroy(GetComponent<FearDebuff>());
        }
        if (enemy.enemyData.enemyType == EnemyData.EnemyType.Debuffer && enemy.enemyData.nameOnEnglish == "Fear")
        {
            Destroy(GetComponent<StunDebuff>());
            Destroy(GetComponent<AnxietyDebuff>());
            Destroy(GetComponent<BurnoutDebuff>());
        }
        if (enemy.enemyData.enemyType == EnemyData.EnemyType.Defender && enemy.enemyData.nameOnEnglish == "Obsession")
        {
            enemy.GetComponentInChildren<DefenseCell>().refreshDefenseEveryTurn = true;
        }
        if (enemy.enemyData.enemyType == EnemyData.EnemyType.Debuffer && enemy.enemyData.nameOnEnglish != "Paralysis" || enemy.enemyData.name != "Burnout" || enemy.enemyData.name != "Fear")
        {
            Destroy(GetComponent<StunDebuff>());
            Destroy(GetComponent<BurnoutDebuff>());
            Destroy(GetComponent<FearDebuff>());
            if (enemy.enemyData.nameOnEnglish == "Dread")
            {
                GetComponent<AnxietyDebuff>().anxietyDamage = 4;
            }
        }
    }
}
