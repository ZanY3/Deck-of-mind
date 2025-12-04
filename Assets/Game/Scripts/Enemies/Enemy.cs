using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private TMP_Text healthTxt;
    [SerializeField] private TMP_Text damageTxt;

    private int currentHealth;
    private int damage;

    private void Start()
    {
        ReadData();
        UpdateUI();
    }
    public void ReadData()
    {
        currentHealth = enemyData.health;
        damage = enemyData.damage;
    }
    public void UpdateUI()
    {
        healthTxt.text = currentHealth.ToString();
        damageTxt.text = damage.ToString();
    }
    public void TakeDamage(int value)
    {
        if (currentHealth >= damage)
        {
            currentHealth -= damage;
            UpdateUI();
            //Some effects
        }
        if(currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }


}
