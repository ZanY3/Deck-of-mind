using DG.Tweening;
using TMPro;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnemyToolTip : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private EnemyData enemyData;

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

    private Vector3 startScale;
    private Tween clueTween;

    private void Start()
    {
        startScale = tooltip.GetComponent<RectTransform>().localScale;
        tooltip.transform.DOScale(0, 0); //to make an animation in the future
        FillUI();
    }
//--------------------------------------------------------------------------------------
    public void FillUI()
    {
        nameTxt.text = enemyData.name;
        descriptionTxt.text = enemyData.description;
        typeTxt.text = enemyData.enemyType.ToString();
        iconImg.sprite = enemyData.artwork;
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
        hpWeaknededTurnsLeftText.text = "Turns left: " + turnsLeft.ToString();
        hpWeaknededTooltip.SetActive(state);
    }
    public void UpdateStrengthTooltip(bool state, int turnsLeft)
    {
        strWeaknededTurnsLeftText.text = "Turns left: " + turnsLeft.ToString();
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
