using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDrag : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Vector2 startPosition;
    private Canvas cardCanvas;
    private CardDisplay cardDisplay;
    private EnergyManager energyManager;
    private CardDraggingManager draggingManager;
    private Vector3 startScale;
    private bool canStartDragging = true;

    private void Awake()
    {
        cardDisplay = GetComponent<CardDisplay>();
        rectTransform = GetComponent<RectTransform>();
        cardCanvas = GetComponent<Canvas>();
        energyManager = FindAnyObjectByType<EnergyManager>();
        draggingManager = FindAnyObjectByType<CardDraggingManager>();
        startScale = rectTransform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData) => transform.DOScale(startScale * 1.2f, 0.1f).SetAutoKill(true).SetUpdate(true);
    public void OnPointerExit(PointerEventData eventData) => transform.DOScale(startScale, 0.1f).SetAutoKill(true).SetUpdate(true);

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canStartDragging)
        {
            startPosition = rectTransform.anchoredPosition;
        }

        InteractionState.isDraggingCard = true;
        if (!energyManager.CheckIsEnoughOnCard(cardDisplay.cardToDisplay.energyCost))
        {
            eventData.pointerDrag = null;
            return;
        }

        CardData.CardType type = cardDisplay.cardToDisplay.type;


        if (type == CardData.CardType.Attack || type == CardData.CardType.SkillOnEnemy)
            draggingManager.SetEnemiesTooltipState(true);
        if (type == CardData.CardType.Defence || type == CardData.CardType.SkillOnPlayer)
            draggingManager.SetPlayerTooltipState(true);

        transform.DOScale(startScale * 0.7f, 0.1f).SetAutoKill(true).SetUpdate(true);
        GetComponent<CanvasGroup>().blocksRaycasts = false;

        cardCanvas.overrideSorting = true;
        cardCanvas.sortingOrder = 100;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 worldPos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out worldPos);
        rectTransform.position = worldPos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canStartDragging = false;
        InteractionState.isDraggingCard = false;

        // Возвращаем масштаб карты
        transform.DOScale(startScale, 0.1f).SetAutoKill(true).SetUpdate(true);
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        cardCanvas.overrideSorting = false;

        CardData card = cardDisplay.cardToDisplay;

        // Выключаем подсказки
        if (card.type == CardData.CardType.Attack || card.type == CardData.CardType.SkillOnEnemy)
            draggingManager.SetEnemiesTooltipState(false);
        if (card.type == CardData.CardType.Defence || card.type == CardData.CardType.SkillOnPlayer)
            draggingManager.SetPlayerTooltipState(false);

        GameObject target = eventData.pointerEnter;

        // Нет цели — возвращаем карту
        if (target == null)
        {
            rectTransform.DOAnchorPos(startPosition, 0.2f).SetEase(Ease.OutBack)
                .SetAutoKill(true).SetUpdate(true)
                .OnComplete(() => canStartDragging = true);
            return;
        }

        Enemy enemy = target.GetComponentInParent<Enemy>();
        PlayerHealth player = target.GetComponent<PlayerHealth>();

        // ---------------------- Снимаем энергию сразу ----------------------
        energyManager.DecreaseEnergy(card.energyCost);

        // ---------------------- Проверка щита ----------------------
        if ((card.type == CardData.CardType.Attack || card.type == CardData.CardType.SkillOnEnemy) && enemy != null)
        {
            DefenseCell shield = enemy.GetComponentInChildren<DefenseCell>(true);

            if (shield != null && shield.defenseIsActive)
            {
                // Атакуем щит
                if (card.effect == CardData.Effect.RandomPower)
                {
                    int damage = FindAnyObjectByType<CardEffects>().RandomPower();
                    shield.DecreaseDefense(damage);
                }
                else
                {
                    shield.DecreaseDefense(card.power);
                }

                // Карта уничтожается, враг не получает урон
                transform.DOKill();
                Destroy(gameObject);
                return;
            }
        }

        // ---------------------- Проверка цели ----------------------
        bool valid = true;

        if ((card.type == CardData.CardType.Attack || card.type == CardData.CardType.SkillOnEnemy) && enemy == null)
            valid = false;

        if ((card.type == CardData.CardType.Defence || card.type == CardData.CardType.SkillOnPlayer) && player == null)
            valid = false;

        if (!valid)
        {
            rectTransform.DOAnchorPos(startPosition, 0.2f).SetEase(Ease.OutBack)
                .SetAutoKill(true).SetUpdate(true)
                .OnComplete(() => canStartDragging = true);
            return;
        }

        // ---------------------- Применяем эффекты ----------------------
        if (enemy != null)
        {
            var dropTarget = enemy.GetComponent<EnemyDropTarget>();
            if (dropTarget != null && dropTarget.enabled)
            {
                dropTarget.ApplyAttack(card);
            }
        }
        // ---------------------- Уничтожаем карту ----------------------
        transform.DOKill();
        Destroy(gameObject);
    }
}
