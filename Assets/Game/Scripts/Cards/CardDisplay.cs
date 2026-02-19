using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    [HideInInspector] public CardData cardToDisplay;

    [SerializeField] private Image iconImg;
    [SerializeField] private Image energyImg;
    [SerializeField] private TMP_Text nameTxt;
    [SerializeField] private TMP_Text descriptionTxt;
    [SerializeField] private TMP_Text typeTxt;
    [SerializeField] private TMP_Text energyCostTxt;

    [Space]
    [Header("Color settings")]
    [SerializeField] private Image body;
    [SerializeField] private Color attackCardColor;
    [SerializeField] private Color defenseCardColor;
    [SerializeField] private Color skillCardColor;

    [SerializeField][Range(0f, 1f)] private float energyImgDarkerFactor;


    public void VisualizeCard()
    {
        if (cardToDisplay == null)
        {
            Debug.LogError("VisualizeCard called with NULL CardData");
            return;
        }

        if (cardToDisplay.type == CardData.CardType.Attack)
        {
            body.color = attackCardColor;
            energyImg.color = attackCardColor;
        }
        else if (cardToDisplay.type == CardData.CardType.Defence)
        {
            body.color = defenseCardColor;
            energyImg.color = defenseCardColor;
        }
        else if(cardToDisplay.type == CardData.CardType.SkillOnPlayer || cardToDisplay.type == CardData.CardType.SkillOnEnemy)
        {
            body.color = skillCardColor;
            energyImg.color = skillCardColor;
        }
        energyImg.color = Darken(energyImg.color, energyImgDarkerFactor);

        nameTxt.font = InteractionState.currentFont;
        descriptionTxt.font = InteractionState.currentFont;
        typeTxt.font = InteractionState.currentFont;

        if(InteractionState.language == InteractionState.Language.English)
        {
            nameTxt.text = cardToDisplay.nameOnEnglish;
            descriptionTxt.text = cardToDisplay.descriptionOnEnglish;
        }
        else if(InteractionState.language == InteractionState.Language.Russian)
        {
            nameTxt.text = cardToDisplay.nameOnRussian;
            descriptionTxt.text = cardToDisplay.descriptionOnRussian;
        }

        typeTxt.text = cardToDisplay.type.ToString();
        energyCostTxt.text = cardToDisplay.energyCost.ToString();

        iconImg.sprite = cardToDisplay.icon;
    }
    Color Darken(Color c, float factor)
    {
        return new Color(
            c.r * factor,
            c.g * factor,
            c.b * factor,
            c.a
        );
    }
}
