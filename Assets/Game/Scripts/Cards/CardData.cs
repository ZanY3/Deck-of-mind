using UnityEngine;

[CreateAssetMenu(fileName = "New_Card", menuName = "Scriptable Objects/Card")]

public class CardData : ScriptableObject
{
    public enum CardType
    {
        Attack,
        Defence
    }
    public CardType type;
    public Sprite icon;
    public string description;
    public int power; //like attack damage value or defence value
}
