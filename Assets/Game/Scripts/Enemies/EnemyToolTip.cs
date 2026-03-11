using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnemyToolTip : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] public EnemyData enemyData;

    [Space]
    [Header("UI/Tooltip")]
    [SerializeField] private GameObject tooltip;
    [SerializeField] private GameObject cardDragTooltip;
    [SerializeField] private GameObject stunTooltip;
    [Header("Debuffs")]
    [SerializeField] private GameObject hpWeaknededTooltip;
    [SerializeField] private TMP_Text hpWeaknededTurnsLeftText;

    [SerializeField] private GameObject strWeaknededTooltip;
    [SerializeField] private TMP_Text strWeaknededTurnsLeftText;
    [Space]
    [SerializeField] private Image iconImg;
    [SerializeField] private TMP_Text nameTxt;
    [SerializeField] private TMP_Text descriptionTxt;
    [SerializeField] private TMP_Text typeTxt;

    [Space]
    [Header("Not required")]
    [SerializeField] private GameObject stunnerClue;
    [SerializeField] private GameObject burnoutClue;

    [SerializeField] private TMP_Text anxietyDamageTxt;
    private Vector3 startScale;
    private Tween clueTween;

    private void Start()
    {
        startScale = tooltip.GetComponent<RectTransform>().localScale;
        tooltip.transform.DOScale(0, 0); //to make an animation in the future

        strWeaknededTurnsLeftText.font = InteractionState.currentFont;
        hpWeaknededTurnsLeftText.font = InteractionState.currentFont;
        nameTxt.font = InteractionState.currentFont;
        descriptionTxt.font = InteractionState.currentFont;
        typeTxt.font = InteractionState.currentFont;

        FillUI();
    }
//--------------------------------------------------------------------------------------
    public void FillUI()
    {
        if(InteractionState.language == InteractionState.Language.English)
        {
            nameTxt.text = enemyData.nameOnEnglish;
            descriptionTxt.text = enemyData.descriptionOnEnglish;
            typeTxt.text = enemyData.enemyType.ToString();
        }
        else if(InteractionState.language == InteractionState.Language.Russian)
        {
            nameTxt.text = enemyData.nameOnRussian;
            descriptionTxt.text = enemyData.descriptionOnRussian;
            EnemyData.EnemyType type = enemyData.enemyType;
            if (type == EnemyData.EnemyType.Attacker)
                typeTxt.text = "Атакующий";
            else if (type == EnemyData.EnemyType.Debuffer)
                typeTxt.text = "Дебаффер";
            else if (type == EnemyData.EnemyType.Defender)
                typeTxt.text = "Защитник";
            else if(type == EnemyData.EnemyType.Boss)
                typeTxt.text = "Босс";
        }

        iconImg.sprite = enemyData.artwork;
        if(GetComponentInParent<AnxietyDebuff>() != null && anxietyDamageTxt != null)
        {
            anxietyDamageTxt.text = GetComponentInParent<AnxietyDebuff>().anxietyDamage.ToString();
        }
    }
    public void UpdateStunToolTip(bool state)
    {
        stunTooltip.SetActive(state);
        if(state)
        {
            iconImg.color = new Color32(216, 196, 255, 255); // #D8C4FF
        }
        else
        {
            iconImg.color = Color.white;
        }
    }
    public void UpdateHpWeaknededTooltip(bool state, int turnsLeft)
    {
        if(InteractionState.language == InteractionState.Language.English)
            hpWeaknededTurnsLeftText.text = "Turns left: " + turnsLeft.ToString();
        else if(InteractionState.language == InteractionState.Language.Russian)
            hpWeaknededTurnsLeftText.text = "Осталось ходов: " + turnsLeft.ToString();

        hpWeaknededTooltip.SetActive(state);
    }
    public void UpdateStrengthTooltip(bool state, int turnsLeft)
    {
        if (InteractionState.language == InteractionState.Language.English)
            strWeaknededTurnsLeftText.text = "Turns left: " + turnsLeft.ToString();
        else if (InteractionState.language == InteractionState.Language.Russian)
            strWeaknededTurnsLeftText.text = "Осталось ходов: " + turnsLeft.ToString();

        strWeaknededTooltip.SetActive(state);
    }
    public void UpdateDragTooltip(bool state)
    {
        cardDragTooltip.SetActive(state);
        Image image = cardDragTooltip.GetComponent<Image>();
        clueTween?.Kill();
        if (state)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
            clueTween = image.DOFade(0.2f, 0.2f).SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        }
    }
    public void UpdateStunClue(bool state)
    {
        stunnerClue.SetActive(state);
    }
    public void UpdateBurnoutClue(bool state)
    {
        burnoutClue.SetActive(state);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!InteractionState.isDraggingCard)
        { 
            ShowUI();
            tooltip.transform.DOScale(startScale, 0.1f);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!InteractionState.isDraggingCard)
        {
            HideUI();
            tooltip.transform.DOScale(0, 0.1f);
        }
    }
    public void ShowUI()
    {
        tooltip.SetActive(true);
    }
    public void HideUI()
    {
        tooltip.SetActive(false);
    }

}
