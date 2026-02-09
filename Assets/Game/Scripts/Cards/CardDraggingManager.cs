using UnityEngine;

public class CardDraggingManager : MonoBehaviour
{
    [SerializeField] private PlayerHealth player;
    [SerializeField] private BattleManager battleManager;

    public void SetEnemiesTooltipState(bool state)
    {
        for(int i = 0; i < battleManager.enemies.Count; i++)
        {
            battleManager.enemies[i].GetComponentInChildren<EnemyToolTip>().UpdateDragTooltip(state);
        }
    }
    public void SetPlayerTooltipState(bool state)
    {
        player.ChangeDraggingClueState(state);
    }
}
