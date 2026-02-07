using UnityEngine;

public class RandomEnemy : MonoBehaviour
{
    [SerializeField] private EnemyData[] enemyList;
    private Enemy enemy;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();

        enemy.enemyData = enemyList[Random.Range(0, enemyList.Length)];
        Debug.Log("Enemy rolled");
    }
}
