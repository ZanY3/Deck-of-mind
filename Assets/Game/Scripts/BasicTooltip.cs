using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class BasicTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject tooltip;

    private Vector3 startScale;

    private void Start()
    {
        startScale = tooltip.GetComponent<RectTransform>().localScale;
        tooltip.transform.DOScale(0, 0); //to make an animation in the future
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!InteractionState.isDraggingCard)
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
