using TMPro;
using UnityEngine;

public class PlayerDefense : MonoBehaviour
{
    [SerializeField] private GameObject armorIcon;
    [SerializeField] private TMP_Text armorTxt;
    private int armor = 0;

    public void AddArmor(int value)
    {
        armor += value;
        UpdateUI();
    }

    public int CalculateDamage(int damage)
    {
        int temp = damage - armor;
        armor -= damage;
        if (armor < 0)
            armor = 0;

        UpdateUI();

        return Mathf.Max(temp, 0);//Max returns to us what num is higher
    }
    public void UpdateUI()
    {
        if (armor != 0)
        {
            armorIcon.SetActive(true);
        }
        else
        {
            armorIcon.SetActive(false);
        }
        armorTxt.text = armor.ToString();
    }
}
