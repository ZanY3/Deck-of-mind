using UnityEngine;

[CreateAssetMenu(fileName = "New_Card", menuName = "Scriptable Objects/Card")]

public class CardData : ScriptableObject
{
    public enum CardType
    {
        Attack,
        Defence,
        SkillOnPlayer,
        SkillOnEnemy
    }
    public enum Effect
    {
        Empty,
        Cleansing,
        Stun,
        RandomPower,
        BloodPact,
        BloodDraw,
        HealthDrain,
        StrengthDrain
    }

    public CardType type;
    public Effect effect;
    public Sprite icon;
    public string description;
    public int power; //like attack damage or defence value
    public int energyCost;
}
