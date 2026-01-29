using UnityEngine;

public class CardEffects : MonoBehaviour
{
//--------------Effects on player---------------------------
    public void RemoveAllDebuffs(PlayerHealth playerHealth)
    {
        playerHealth.ClearAllDebuffs();
        Debug.Log("All debufs had cleaned!");
    }
    public void BloodPact(PlayerHealth playerHealth)
    {
        GetComponent<PlayerHealth>().TakeDamage(4);
        FindAnyObjectByType<EnergyManager>().IncreaseEnergy(1);
    }
    public void BloodDraw(PlayerHealth playerHealth)
    {
        GetComponent<PlayerHealth>().TakeDamage(2);
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
        return Random.Range(1, 6);
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
        enemy.UpdateUI();
    }
    public void StrengthWeaken(Enemy enemy, bool state, int turns)
    {
        enemy.ApplyHealthWeaken(state, turns);
    }
}
