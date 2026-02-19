using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class HoverScaler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float scaleMultiplier = 1.1f;
    [SerializeField] private float duration = 0.15f;
    [SerializeField] private Ease ease = Ease.OutQuad;

    private Vector3 originalScale;
    private Tween currentTween;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AnimateScale(originalScale * scaleMultiplier);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        AnimateScale(originalScale);
    }

    private void AnimateScale(Vector3 target)
    {
        currentTween?.Kill();
        currentTween = transform.DOScale(target, duration).SetEase(ease);
    }
}
