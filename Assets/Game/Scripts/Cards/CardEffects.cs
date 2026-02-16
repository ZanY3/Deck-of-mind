using UnityEngine;

public class CardEffects : MonoBehaviour
{
//--------------Effects on player---------------------------
    public void Cleansing(PlayerHealth playerHealth)
    {
        playerHealth.Heal(4);
        playerHealth.ClearAllDebuffs();
        Debug.Log("All debufs had cleaned!");
    }
    public void BloodPact(PlayerHealth playerHealth)
    {
        playerHealth.TakeDamage(2, false);
        FindAnyObjectByType<EnergyManager>().IncreaseEnergy(1);
    }
    public void BloodDraw(PlayerHealth playerHealth)
    {
        playerHealth.TakeDamage(2, false);
        FindAnyObjectByType<HandManager>().DrawOneCard();
        FindAnyObjectByType<HandManager>().DrawOneCard();
    }

//--------------Effects on enemy---------------------------
    public void Stun(Enemy enemy)
    {
        enemy.ApplyStun();
        Debug.Log("Enemy stunned!");
    }
    public int RandomPower()
    {
        return Random.Range(1, 4);
    }
    public void HealthWeaken(Enemy enemy, bool state, int turns)
    {
        enemy.hpWeakenedTurnsLeft = turns;
        if (state)
        {
            enemy.currentHealth /= 2;
            enemy.hpWeakened = true;
        }
        else
        {
            enemy.currentHealth *= 2;
            enemy.hpWeakened = false;
        }
        enemy.GetComponentInChildren<EnemyToolTip>().UpdateHpWeaknededTooltip(state, turns);
        enemy.UpdateUI();
    }
    public void StrengthWeaken(Enemy enemy, bool state, int turns)
    {
        enemy.strengthWeakenedTurnsLeft = turns;
        if (state)
        {
            enemy.damage /= 2;
            enemy.strengthWeakened = true;
        }
        else
        {
            enemy.damage = enemy.enemyData.damage;
            enemy.strengthWeakened = false;
        }
        enemy.GetComponentInChildren<EnemyToolTip>().UpdateStrengthTooltip(state, turns);
        enemy.UpdateUI();
    }
}
