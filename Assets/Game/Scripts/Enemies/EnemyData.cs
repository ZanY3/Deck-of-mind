using UnityEngine;

[CreateAssetMenu(fileName = "New_Enemy", menuName = "Scriptable Objects/Enemy")]
public class EnemyData : ScriptableObject
{
    public string nameOnEnglish;
    public string nameOnRussian;
    public string descriptionOnEnglish;
    public string descriptionOnRussian;
    [Space]
    public int health;
    public int damage;

    public Sprite artwork;
    public enum EnemyType
    {
        Attacker,
        Debuffer,
        Defender,
        Boss
    }
    public enum AttackPattern
    {
        Normal,
        Scaling,
        Random
    }
    public EnemyType enemyType;
    public AttackPattern attackPattern;

    //Attacker - just deals fixed amout of damage every turn COLORS: WHITE AND RED(MB)
    //Debuffer - also damage us, but value of damage is small and enemy gives some kind of negative effect on player(like poison) COLORS: WHITE + GREEN + YELLOW EYES
    //Defender - this enemy have slot with defence, we'll be able to attack him ONLY after we beat this slot
    //defender have value of defence that he will add to slot at next turn, when slot will be broken, enemy get stun effect
}
