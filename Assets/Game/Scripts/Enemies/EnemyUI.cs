using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private EnemyData enemyData;

    [Space]
    [Header("UI/Tooltip")]
    [SerializeField] private GameObject tooltip;
    [SerializeField] private Image iconImg;
    [SerializeField] private TMP_Text nameTxt;
    [SerializeField] private TMP_Text descriptionTxt;
    [SerializeField] private TMP_Text typeTxt;
    public void FillUI()
    {
        nameTxt.text = enemyData.name;
        descriptionTxt.text = enemyData.description;
        typeTxt.text = enemyData.enemyType.ToString();
        iconImg.sprite = enemyData.artwork;
        
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowUI();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        HideUI();
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
