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
            rectTransform.DOAnchorPos(startPosition, 0.2f)
                .SetEase(Ease.OutBack)
                .SetAutoKill(true)
                .SetUpdate(true)
                .OnComplete(() => canStartDragging = true);
            return;
        }

        Enemy enemy = target.GetComponentInParent<Enemy>();
        PlayerHealth player = target.GetComponent<PlayerHealth>();

        // Проверка цели
        bool valid = true;
        if ((card.type == CardData.CardType.Attack || card.type == CardData.CardType.SkillOnEnemy) && enemy == null)
            valid = false;
        if ((card.type == CardData.CardType.Defence || card.type == CardData.CardType.SkillOnPlayer) && player == null)
            valid = false;
        if(card.effect == CardData.Effect.Cleansing && !player.HasDebuffs())
            valid = false;

        if (!valid)
        {
            rectTransform.DOAnchorPos(startPosition, 0.2f)
                .SetEase(Ease.OutBack)
                .SetAutoKill(true)
                .SetUpdate(true)
                .OnComplete(() => canStartDragging = true);
            return;
        }

        // ---------------------- Проверка щита ----------------------
        if ((card.type == CardData.CardType.Attack || card.type == CardData.CardType.SkillOnEnemy) && enemy != null)
        {
            DefenseCell shield = enemy.GetComponentInChildren<DefenseCell>(true);

            if (shield != null && shield.defenseIsActive)
            {
                if (card.effect == CardData.Effect.RandomPower)
                {
                    int damage = FindAnyObjectByType<CardEffects>().RandomPower();
                    shield.DecreaseDefense(damage);
                }
                else if (card.type == CardData.CardType.SkillOnEnemy)
                {
                    var dropTarget = enemy.GetComponent<EnemyDropTarget>();
                    if (CanApplyEnemySkill(card, enemy))
                    {
                        dropTarget.ApplyAttack(card);
                        energyManager.DecreaseEnergy(card.energyCost);
                        transform.DOKill();
                        Destroy(gameObject);
                        return;
                    }
                    else
                    {
                        // Эффект уже есть — карта возвращается
                        rectTransform.DOAnchorPos(startPosition, 0.2f)
                            .SetEase(Ease.OutBack)
                            .SetAutoKill(true)
                            .SetUpdate(true)
                            .OnComplete(() => canStartDragging = true);
                        return;
                    }
                }
                else
                {
                    shield.DecreaseDefense(card.power);
                }

                energyManager.DecreaseEnergy(card.energyCost);
                transform.DOKill();
                Destroy(gameObject);
                return;
            }
        }

        // ---------------------- Применяем эффекты на врага ----------------------
        if (enemy != null)
        {
            var dropTarget = enemy.GetComponent<EnemyDropTarget>();
            if (dropTarget != null && dropTarget.canBeAttacked)
            {
                if (CanApplyEnemySkill(card, enemy))
                {
                    dropTarget.ApplyAttack(card);
                    energyManager.DecreaseEnergy(card.energyCost);
                    transform.DOKill();
                    Destroy(gameObject);
                }
                else
                {
                    // Эффект уже есть — возвращаем карту
                    rectTransform.DOAnchorPos(startPosition, 0.2f)
                        .SetEase(Ease.OutBack)
                        .SetAutoKill(true)
                        .SetUpdate(true)
                        .OnComplete(() => canStartDragging = true);
                }
                return;
            }
        }

        transform.DOKill();
        Destroy(gameObject);
    }
    private bool CanApplyEnemySkill(CardData card, Enemy enemy)
    {
        if (card.type != CardData.CardType.SkillOnEnemy || enemy == null)
            return true;

        switch (card.effect)
        {
            case CardData.Effect.Stun:
                return !enemy.stunned;

            case CardData.Effect.HealthDrain:
                return !enemy.hpWeakened;

            case CardData.Effect.StrengthDrain:
                return !enemy.strengthWeakened;
        }

        return true;
    }
}
