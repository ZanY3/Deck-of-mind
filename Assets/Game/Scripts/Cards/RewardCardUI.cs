using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class RewardCardUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private CardRewardManager rewardManager;
    private Vector3 startScale;
    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startScale = rectTransform.localScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        rewardManager.hasChosenCard = true;
        // Берём актуальную карту прямо из CardDisplay
        CardData currentData = GetComponent<CardDisplay>().cardToDisplay;
        rewardManager.ChooseCard(currentData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(startScale * 1.25f, 0.2f).SetEase(Ease.Linear);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(startScale, 0.2f).SetEase(Ease.Linear);
    }
}
