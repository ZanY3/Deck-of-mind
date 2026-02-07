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
        if(enemy.enemyData.enemyType == EnemyData.EnemyType.Debuffer && enemy.enemyData.name == "Paralysis")
        {
            Destroy(GetComponent<AnxietyDebuff>());
            shadowImg.color = shadowColorForStunner;
            typeTxt.color = colorForStunner;
            anxietyIcon.SetActive(false);
        }
        else if(enemy.enemyData.enemyType == EnemyData.EnemyType.Debuffer && enemy.enemyData.name != "Paralysis")
        {
            Destroy(GetComponent<StunDebuff>());
            if(enemy.enemyData.name == "Dread")
            {
                GetComponent<AnxietyDebuff>().anxietyDamage = 4;
            }
        }
        if(enemy.enemyData.enemyType == EnemyData.EnemyType.Defender && enemy.enemyData.name != "Obsession")
        {
            enemy.GetComponentInChildren<DefenseCell>().refreshDefenseEveryTurn = true;
        }
    }
}
