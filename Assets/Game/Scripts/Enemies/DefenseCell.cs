using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class DefenseCell : MonoBehaviour, IDropHandler
{
    [SerializeField] private int defenseAmout;
    [SerializeField] private int defensePerTurn;
    [SerializeField] private bool refreshDefenseEveryTurn = false;
    [SerializeField] private EnemyDropTarget dropTarget;

    [Space]
    [Header("UI")]
    [SerializeField] private TMP_Text defenseAmoutTxt;

    //[HideInInspector] public bool canAttackEnemy = false;

    private Enemy enemy;
    private int startDefenseAmout;

    private void Start()
    {
        dropTarget.enabled = false;
        enemy = GetComponentInParent<Enemy>();
        startDefenseAmout = defenseAmout;
        UpdateUI();
    }
//--------------------------------------------------------------------------------------
    public void RefillDefense()
    {
        if(refreshDefenseEveryTurn)
        {
            defenseAmout = startDefenseAmout;
            UpdateUI();
        }
    }
    public void DecreaseDefense(int value)
    {
        if(defenseAmout > value)
        {
            defenseAmout -= value;
        }
        else if(defenseAmout <= value)
        {
            gameObject.SetActive(false);
            //canAttackEnemy = true;
            dropTarget.enabled = true;
        }
        UpdateUI();
    }
    public void UpdateUI()
    {
       defenseAmoutTxt.text = defenseAmout.ToString();
    }
    public void OnDrop(PointerEventData eventData) //OnCardDrop
    {
        CardData card = eventData.pointerDrag.GetComponent<CardDisplay>().cardToDisplay;

        if (card.type != CardData.CardType.Attack && card.effect != CardData.Effect.RandomPower)
        {
            return;
        }
        if(card.effect == CardData.Effect.RandomPower)
        {
            int damage = eventData.pointerDrag.GetComponent<CardEffects>().RandomPower();
            DecreaseDefense(damage);
        }
        else
        {
            DecreaseDefense(card.power);
        }
    }
}
