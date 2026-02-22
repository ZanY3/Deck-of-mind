using TMPro;
using UnityEngine;
using DG.Tweening;

public class PlayerDefense : MonoBehaviour
{
    [SerializeField] private GameObject armorIcon;
    [SerializeField] private TMP_Text armorTxt;
    private int armor = 0;
    [HideInInspector] public int Armor => armor;

    public void AddArmor(int value)
    {
        armor += value;
        UpdateUI();
    }
    public void RemoveAllArmor()
    {
        armor = 0;
        UpdateUI();
    }
    public int CalculateDamage(int damage)
    {
        int temp = damage - armor;
        armor -= damage;
        if (armor < 0)
            armor = 0;

        UpdateUI();

        return Mathf.Max(temp, 0);
    }
    public void UpdateUI()
    {
        if (armor != 0)
        {
            armorIcon.SetActive(true);
            RectTransform iconTransform = armorIcon.GetComponent<RectTransform>();
            Vector3 startScale = iconTransform.localScale;

            if(armor == 2)
            {
                iconTransform.DOScale(0, 0);
            }
            else
            {
                iconTransform.DOScale(0.5f, 0.1f);
            }
            iconTransform.DOScale(startScale, 0.1f);
        }
        else
        {
            armorIcon.SetActive(false);
        }
        armorTxt.text = armor.ToString();
    }
}
