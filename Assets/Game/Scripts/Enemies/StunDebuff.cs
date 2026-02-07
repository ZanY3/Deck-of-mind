using UnityEngine;
public class StunDebuff : MonoBehaviour
{
    public int turnsUntilStun = 2;
    private int startTurnsUntilStun;

    private void Start()
    {
        startTurnsUntilStun = turnsUntilStun;
    }

    public void DealStun()
    {
        PlayerHealth playerHealth = FindAnyObjectByType<PlayerHealth>();
        Debug.Log("=== STUN APPLIED TO PLAYER ===");
        playerHealth.ChangeStunState(true);
        playerHealth.turnsUntilStunRemove = 2; // ← ИЗМЕНИ С 1 НА 2
        Debug.Log("Player stunned, turns until remove: " + playerHealth.turnsUntilStunRemove);
        turnsUntilStun = startTurnsUntilStun;
    }
}