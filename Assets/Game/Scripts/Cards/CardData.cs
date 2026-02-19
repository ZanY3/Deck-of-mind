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

    public string nameOnEnglish;
    public string nameOnRussian;
    public string descriptionOnEnglish;
    public string descriptionOnRussian;
    [Space]
    public CardType type;
    public Effect effect;
    public Sprite icon;
    public int power; //like attack damage or defence value
    public int energyCost;
}
