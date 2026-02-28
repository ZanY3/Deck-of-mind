using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class RewardCardUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private CardRewardManager rewardManager;

    [Header("Sounds")]
    [SerializeField] private AudioClip cardEnterSound;
    [SerializeField] private AudioClip cardExitSound;
    [SerializeField] private AudioClip cardChoosedSound;

    [Range(0f, 1f)][SerializeField] private float cardEnterExitVolume;
    [Range(0f, 1f)][SerializeField] private float cardChoosedVolume;

    private Vector3 startScale;
    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startScale = rectTransform.localScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        float randPitch = Random.Range(0.95f, 1.1f);
        SoundManager.Instance.PlaySFX(cardChoosedSound, randPitch, cardChoosedVolume);

        rewardManager.hasChosenCard = true;

        CardData currentData = GetComponent<CardDisplay>().cardToDisplay;
        rewardManager.ChooseCard(currentData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        float randPitch = Random.Range(0.8f, 1.1f);
        SoundManager.Instance.PlaySFX(cardEnterSound, randPitch, cardEnterExitVolume);
        transform.DOScale(startScale * 1.25f, 0.2f).SetEase(Ease.Linear);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        float randPitch = Random.Range(0.8f, 1.1f);
        SoundManager.Instance.PlaySFX(cardExitSound, randPitch, cardEnterExitVolume);
        transform.DOScale(startScale, 0.2f).SetEase(Ease.Linear);
    }
}