using UnityEngine;
using DG.Tweening;

public class BossPhaseController : MonoBehaviour
{
    //[SerializeField] private float damageMultiplier;

    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private RectTransform[] enemiesSpawnPos;

    public int enemiesSummonedCount = 0;

    private RectTransform enemySlot;

    private void Start()
    {
        enemySlot = GameObject.FindGameObjectWithTag("EnemySlot").GetComponent<RectTransform>();
    }

    public void SummonEnemies()
    {
        if(enemiesSummonedCount == 0)
        {
            for (int i = 0; i < enemiesSpawnPos.Length; i++)
            {
                var enemy = Instantiate(enemyPrefab);

                enemy.transform.SetParent(enemySlot.transform, false);
                enemy.GetComponent<RectTransform>().position = enemiesSpawnPos[i].position;
                enemy.transform.DOShakePosition(0.5f, 6, 15).SetAutoKill(true).SetUpdate(true);
                enemiesSummonedCount++;
                enemy.GetComponent<Enemy>().ApplyStun();
            }
            Debug.Log("Boss spawned enemies");
        }
        else
        {
            Debug.Log("Enemies was already spawned");
        }

    }
}
