using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDrag : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Sounds")]
    [SerializeField] private AudioClip cardEnterSound;
    [SerializeField] private AudioClip cardExitSound;
    [SerializeField] private AudioClip cardStartDragSound;
    [SerializeField] private AudioClip cardEndDragSound;
    [SerializeField] private AudioClip notEnoughEnergySound;

    [Range(0f, 1f)][SerializeField] private float cardPointerEnterExitVolume = 0.015f;
    [Range(0f, 1f)][SerializeField] private float cardStartDragVolume = 0.035f;
    [Range(0f, 1f)][SerializeField] private float cardEndDragVolume = 0.035f;
    [Range(0f, 1f)][SerializeField] private float notEnoughEnergyVolume = 0.015f;

    private RectTransform rectTransform;
    private Vector2 startPosition;
    private Canvas cardCanvas;
    private CardDisplay cardDisplay;
    private EnergyManager energyManager;
    private CardDraggingManager draggingManager;
    private Vector3 startScale;

    private bool canStartDragging = true;
    private bool isLockedDuringDrag = false; // замок для предотвращения зависаний

    [SerializeField] private float dragCooldown = 0.25f;
    private void AllowNextDrag() => canStartDragging = true;

    private void Awake()
    {
        cardDisplay = GetComponent<CardDisplay>();
        rectTransform = GetComponent<RectTransform>();
        cardCanvas = GetComponent<Canvas>();
        energyManager = FindAnyObjectByType<EnergyManager>();
        draggingManager = FindAnyObjectByType<CardDraggingManager>();
        startScale = rectTransform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!energyManager.CheckIsEnoughOnCard(cardDisplay.cardToDisplay.energyCost))
        {
            eventData.pointerDrag = null;
            InteractionState.isDraggingCard = false;
            return;
        }
        if (InteractionState.isDraggingCard) return;

        float randPitch = Random.Range(0.85f, 1.15f);
        SoundManager.Instance.PlaySFX(cardEnterSound, randPitch, cardPointerEnterExitVolume);
        transform.DOScale(startScale * 1.15f, 0.1f).SetAutoKill(true).SetUpdate(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!energyManager.CheckIsEnoughOnCard(cardDisplay.cardToDisplay.energyCost))
        {
            eventData.pointerDrag = null;
            InteractionState.isDraggingCard = false;
            return;
        }
        if (InteractionState.isDraggingCard) return;

        float randPitch = Random.Range(0.85f, 1.15f);
        SoundManager.Instance.PlaySFX(cardExitSound, randPitch, cardPointerEnterExitVolume);
        transform.DOScale(startScale, 0.1f).SetAutoKill(true).SetUpdate(true);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!canStartDragging || InteractionState.isDraggingCard) return;

        startPosition = rectTransform.anchoredPosition;
        InteractionState.isDraggingCard = true;
        isLockedDuringDrag = true;

        if (!energyManager.CheckIsEnoughOnCard(cardDisplay.cardToDisplay.energyCost))
        {
            CameraShake.Instance.OnShake(0.1f, 0.1f);
            float randPitch1 = Random.Range(0.85f, 1.15f);
            SoundManager.Instance.PlaySFX(notEnoughEnergySound, randPitch1, notEnoughEnergyVolume);
            eventData.pointerDrag = null;
            InteractionState.isDraggingCard = false;
            isLockedDuringDrag = false;
            return;
        }

        // --- МИНИМАЛЬНЫЙ ФИКС ---
        foreach (var otherCard in FindObjectsByType<CardDrag>(FindObjectsSortMode.None))
        {
            if (otherCard != this)
            {
                otherCard.GetComponent<CanvasGroup>().blocksRaycasts = false;
            }
        }

        float randPitch = Random.Range(0.85f, 1.15f);
        SoundManager.Instance.PlaySFX(cardStartDragSound, randPitch, cardStartDragVolume);

        CardData.CardType type = cardDisplay.cardToDisplay.type;
        if (type == CardData.CardType.Attack || type == CardData.CardType.SkillOnEnemy) draggingManager.SetEnemiesTooltipState(true);
        if (type == CardData.CardType.Defence || type == CardData.CardType.SkillOnPlayer) draggingManager.SetPlayerTooltipState(true);

        transform.DOScale(startScale * 0.7f, 0.1f).SetAutoKill(true).SetUpdate(true);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        cardCanvas.overrideSorting = true;
        cardCanvas.sortingOrder = 100;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isLockedDuringDrag)
        {
            ReturnCard();
            return;
        }

        Vector3 worldPos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out worldPos))
        {
            rectTransform.position = worldPos;
        }
        else
        {
            ReturnCard();
            isLockedDuringDrag = false;
            InteractionState.isDraggingCard = false;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // --- ВОССТАНАВЛИВАЕМ интерактивность других карт ---
        foreach (var otherCard in FindObjectsByType<CardDrag>(FindObjectsSortMode.None))
        {
            if (otherCard != this)
            {
                otherCard.GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
        }

        if (!isLockedDuringDrag)
        {
            ReturnCard();
            Invoke(nameof(AllowNextDrag), dragCooldown);
            InteractionState.isDraggingCard = false;
            return;
        }

        isLockedDuringDrag = false;
        canStartDragging = false;
        InteractionState.isDraggingCard = false;

        transform.DOScale(startScale, 0.1f).SetAutoKill(true).SetUpdate(true);
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        cardCanvas.overrideSorting = false;

        CardData card = cardDisplay.cardToDisplay;

        if (card.type == CardData.CardType.Attack || card.type == CardData.CardType.SkillOnEnemy) draggingManager.SetEnemiesTooltipState(false);
        if (card.type == CardData.CardType.Defence || card.type == CardData.CardType.SkillOnPlayer) draggingManager.SetPlayerTooltipState(false);

        GameObject target = eventData.pointerEnter;
        Enemy enemy = target?.GetComponentInParent<Enemy>();
        PlayerHealth player = target?.GetComponent<PlayerHealth>();

        if (card.effect == CardData.Effect.Cleansing)
        {
            if (player != null && player.HasDebuffs())
            {
                FindAnyObjectByType<CardEffects>().Cleansing(player);
                energyManager.DecreaseEnergy(card.energyCost);
                float randPitchC = Random.Range(0.85f, 1.15f);
                SoundManager.Instance.PlaySFX(cardEndDragSound, randPitchC, cardEndDragVolume);
                transform.DOKill();
                Destroy(gameObject);
                Invoke(nameof(AllowNextDrag), dragCooldown);
                return;
            }
            else
            {
                ReturnCard();
                Invoke(nameof(AllowNextDrag), dragCooldown);
                return;
            }
        }
        if(card.effect == CardData.Effect.BloodDraw)
        {
            HandManager handManager = FindAnyObjectByType<HandManager>();
            if (player != null || handManager.HandCount() >= 4)
            {
                ReturnCard();
                Invoke(nameof(AllowNextDrag), dragCooldown);
                return;
            }
        }
        if(card.effect == CardData.Effect.HealthDrain && enemy != null)
        {
            if(enemy.enemyData.enemyType == EnemyData.EnemyType.Boss)
            {
                ReturnCard();
                Invoke(nameof(AllowNextDrag), dragCooldown);
                return;
            }
        }

        bool valid = true;
        if ((card.type == CardData.CardType.Attack || card.type == CardData.CardType.SkillOnEnemy) && enemy == null) valid = false;
        if ((card.type == CardData.CardType.Defence || card.type == CardData.CardType.SkillOnPlayer) && player == null) valid = false;
        if (!CanApplyEnemySkill(card, enemy)) valid = false;

        if (!valid)
        {
            ReturnCard();
            Invoke(nameof(AllowNextDrag), dragCooldown);
            return;
        }

        float randPitch2 = Random.Range(0.85f, 1.15f);
        SoundManager.Instance.PlaySFX(cardEndDragSound, randPitch2, cardEndDragVolume);

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
                if(card.effect == CardData.Effect.ShieldSlam)
                {
                    int damage = FindAnyObjectByType<CardEffects>().ShieldSlam();
                    shield.DecreaseDefense(damage);
                }
                if(card.effect == CardData.Effect.StrikeTheHelpless)
                {
                    int damage = FindAnyObjectByType<CardEffects>().StrikeTheHelpless(enemy);
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
                        Invoke(nameof(AllowNextDrag), dragCooldown);
                        return;
                    }
                    else
                    {
                        ReturnCard();
                        Invoke(nameof(AllowNextDrag), dragCooldown);
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
                Invoke(nameof(AllowNextDrag), dragCooldown);
                return;
            }
        }

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
                    Invoke(nameof(AllowNextDrag), dragCooldown);
                }
                else
                {
                    ReturnCard();
                    Invoke(nameof(AllowNextDrag), dragCooldown);
                }
                return;
            }
        }

        transform.DOKill();
        Destroy(gameObject);
        Invoke(nameof(AllowNextDrag), dragCooldown);
    }

    private bool CanApplyEnemySkill(CardData card, Enemy enemy)
    {
        if (card.type != CardData.CardType.SkillOnEnemy || enemy == null) return true;
        switch (card.effect)
        {
            case CardData.Effect.Stun: return !enemy.stunned;
            case CardData.Effect.HealthDrain:
                if (enemy.currentHealth == 1) return false;
                return !enemy.hpWeakened;
            case CardData.Effect.StrengthDrain:
                if (enemy.damage == 1) return false;
                return !enemy.strengthWeakened;
        }
        return true;
    }
    public void DestroySafely()
    {
        // снимаем глобальный drag
        InteractionState.isDraggingCard = false;
        isLockedDuringDrag = false;
        canStartDragging = true;

        // возвращаем всем картам raycast
        foreach (var card in FindObjectsByType<CardDrag>(FindObjectsSortMode.None))
        {
            card.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }

        // выключаем тултипы
        draggingManager.SetEnemiesTooltipState(false);
        draggingManager.SetPlayerTooltipState(false);

        GetComponent<CanvasGroup>().blocksRaycasts = true;
        cardCanvas.overrideSorting = false;

        transform.DOKill();
        Destroy(gameObject);
    }
    private void ReturnCard()
    {
        rectTransform.DOAnchorPos(startPosition, 0.2f)
            .SetEase(Ease.OutBack)
            .SetAutoKill(true)
            .SetUpdate(true)
            .OnComplete(() => Invoke(nameof(AllowNextDrag), dragCooldown));
    }
}
