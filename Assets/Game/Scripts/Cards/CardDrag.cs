using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class CardDrag : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Vector2 startPosition;
    private Canvas canvas;
    private Canvas cardCanvas;
    private CardDisplay cardDisplay;
    private EnergyManager energyManager;
    private CardDraggingManager draggingManager;

    private Vector3 startScale;

    [HideInInspector] public bool droppedOnTarget;

    private void Awake()
    {
        cardDisplay = GetComponent<CardDisplay>();
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        cardCanvas = GetComponent<Canvas>();
        energyManager = FindAnyObjectByType<EnergyManager>();
        draggingManager = FindAnyObjectByType<CardDraggingManager>();

        startScale = rectTransform.localScale;
    }
    //--------------------------------------------------------------------------------------
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
        {
            transform.DOScale(startScale * 1.2f, 0.1f).SetEase(Ease.Linear);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
        {
            transform.GetComponent<Transform>().DOScale(startScale, 0.1f).SetEase(Ease.Linear);
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!energyManager.CheckIsEnoughOnCard(cardDisplay.cardToDisplay.energyCost))
        {
            eventData.pointerDrag = null;
            return;
        }

        CardData.CardType type = cardDisplay.cardToDisplay.type;
        if (type == CardData.CardType.Attack || type == CardData.CardType.SkillOnEnemy)
        {
            draggingManager.SetEnemiesTooltipState(true);
        }
        if(type == CardData.CardType.Defence || type == CardData.CardType.SkillOnPlayer)
        {
            draggingManager.SetPlayerTooltipState(true);
        }

        transform.DOScale(startScale * 0.7f, 0.1f).SetEase(Ease.Linear);

        InteractionState.isDraggingCard = true;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        droppedOnTarget = false;

        startPosition = rectTransform.anchoredPosition;

        cardCanvas.overrideSorting = true;
        cardCanvas.sortingOrder = 100;
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 worldPos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
        rectTransform,
        eventData.position,
        eventData.pressEventCamera,
        out worldPos
        );

        rectTransform.position = worldPos;
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        transform.DOScale(startScale, 0.1f).SetEase(Ease.Linear);

        InteractionState.isDraggingCard = false;
        cardCanvas.overrideSorting = false;
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        CardData card = cardDisplay.cardToDisplay;

        CardData.CardType type = cardDisplay.cardToDisplay.type;
        if (type == CardData.CardType.Attack || type == CardData.CardType.SkillOnEnemy)
        {
            draggingManager.SetEnemiesTooltipState(false);
        }
        if (type == CardData.CardType.Defence || type == CardData.CardType.SkillOnPlayer)
        {
            draggingManager.SetPlayerTooltipState(false);
        }

        Enemy enemy = eventData.pointerEnter?.GetComponent<Enemy>();
        if (!IsValidTarget(eventData))
        {
            rectTransform.DOAnchorPos(startPosition, 0.2f).SetEase(Ease.OutBack);
        }
        else
        {
            energyManager.DecreaseEnergy(card.energyCost);
            Destroy(gameObject);
            
        }
    }
    public bool IsValidTarget(PointerEventData eventData)
    {
        GameObject target = eventData.pointerEnter;

        if (target == null || !droppedOnTarget)
            return false;

        PlayerHealth playerOnTarget = target.GetComponent<PlayerHealth>();
        Enemy enemyOnTarget = target.GetComponent<Enemy>();
        CardData card = cardDisplay.cardToDisplay;


        if (
        (playerOnTarget != null && card.type == CardData.CardType.Attack) ||
        (enemyOnTarget != null && card.type == CardData.CardType.Defence) ||
        (enemyOnTarget != null && card.type == CardData.CardType.SkillOnPlayer) ||
        (playerOnTarget != null && card.type == CardData.CardType.SkillOnEnemy))
        //(enemyOnTarget != null && card.effect == CardData.Effect.Stun && enemyOnTarget.stunned)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
