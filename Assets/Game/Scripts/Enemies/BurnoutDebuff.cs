using UnityEngine;

public class BurnoutDebuff : MonoBehaviour
{
    public int turnsUntilDebuff = 2;
    private int startTurnsUntilDebuff;

    private void Start()
    {
        startTurnsUntilDebuff = turnsUntilDebuff;
    }

    public void DealBurnout()
    {
        EnergyManager energy = FindAnyObjectByType<EnergyManager>();

        energy.DecreaseEnergy(1);

        turnsUntilDebuff = startTurnsUntilDebuff;
    }
}
