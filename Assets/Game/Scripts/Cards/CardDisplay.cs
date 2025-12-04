using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    [SerializeField] private CardData cardToDisplay;
    [HideInInspector] public CardData Card => cardToDisplay;

    [SerializeField] private Image iconImg;
    [SerializeField] private TMP_Text nameTxt;
    [SerializeField] private TMP_Text descriptionTxt;
    [SerializeField] private TMP_Text typeTxt;

    [Space]
    [Header("Color settings")]
    [SerializeField] private Image body;
    [SerializeField] private Color attackCardColor;
    [SerializeField] private Color defenceCardColor;


    private void Start()
    {
        VisualizeCard(cardToDisplay);
    }

    public void VisualizeCard(CardData cardToVisualize)
    {
        if(cardToVisualize.type == CardData.CardType.Attack)
        {
            body.color = attackCardColor;
        }
        else if(cardToDisplay.type == CardData.CardType.Defence)
        {
            body.color = defenceCardColor;
        }

        nameTxt.text = cardToVisualize.name;
        descriptionTxt.text = cardToVisualize.description;
        typeTxt.text = cardToVisualize.type.ToString();

        iconImg.sprite = cardToVisualize.icon;
    }
}
