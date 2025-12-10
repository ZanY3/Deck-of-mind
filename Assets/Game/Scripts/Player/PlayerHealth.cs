using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    [SerializeField] private BattleManager battleManager;

    private int currentHealth;
    private PlayerDefense defense;

    [Space]
    [Header("UI/HealthBar")]

    [SerializeField] private Image healthBarImage;
    [SerializeField] private TMP_Text healthTxt;

    private void Start()
    {
        defense = GetComponent<PlayerDefense>();
        currentHealth = maxHealth;
        UpdateUI();
    }
    public void TakeDamage(int damage)
    {
        if(currentHealth > 0)
        {
            currentHealth -= defense.CalculateDamage(damage);
            UpdateUI();
        }
        if(currentHealth <= 0)
        {
            currentHealth = 0;
            UpdateUI();
            Debug.Log("PLAYER DIED!!!");
            battleManager.PlayerLose();//Fix bugs with UI and with enemy disable
        }
    }
    public void UpdateUI()
    {
        if(healthTxt != null && healthBarImage != null)
        {
            healthBarImage.fillAmount = (float)currentHealth / maxHealth;
            healthTxt.text = currentHealth.ToString() + "/" + maxHealth.ToString();
        }
    }
}
