using DG.Tweening;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Boss : Enemy
{
    public enum BossType
    {
        TheMind,
        MindGuardian
    };
    public Sprite phase2Sprite;
    public Sprite phase3Sprite;

    [SerializeField] private BossType type;

    [Header("Sounds")]
    [SerializeField] private AudioClip bossPhaseTransitionSound;
    [SerializeField] private AudioClip summonSound;
    [Range(0f, 1f)][SerializeField] private float bossPhaseTransitionVolume;
    [Range(0f, 1f)][SerializeField] private float summonVolume;

    private int attackTurnCounter = 0;
    private Image image;
    private BossPhaseController phaseController;

    protected override void Start()
    {
        base.Start();
        image = transform.GetChild(1).GetComponentInChildren<Image>();
        phaseController = GetComponent<BossPhaseController>();
    }

    public override void AttackPlayer()
    {
        attackTurnCounter++;
        base.AttackPlayer();
        if (type == BossType.TheMind)
        {
            if((attackTurnCounter == 2 || attackTurnCounter == 6 || attackTurnCounter == 10) && stunned == false)
            {
                float randPitch = Random.Range(0.95f, 1.05f);
                SoundManager.Instance.PlaySFX(summonSound, randPitch, summonVolume);
                DOVirtual.DelayedCall(0.5f, () =>
                {
                    phaseController.SummonEnemies();
                });
            }

            if(currentHealth <= 10)// PHASE 2
            {
                SoundManager.Instance.PlaySFX(bossPhaseTransitionSound, 1, bossPhaseTransitionVolume);
                image.sprite = phase2Sprite;
                damage += 2;
                UpdateUI();
                attackTurnCounter = 0;
            }
        }
        else if(type == BossType.MindGuardian)
        {
            if(currentHealth <= 16)
            {
                SoundManager.Instance.PlaySFX(bossPhaseTransitionSound, 1, bossPhaseTransitionVolume);
                image.sprite = phase2Sprite;
                atkPattern = EnemyData.AttackPattern.Scaling;
            }
            if(currentHealth <= 10)
            {
                SoundManager.Instance.PlaySFX(bossPhaseTransitionSound, 1, bossPhaseTransitionVolume);
                image.sprite = phase3Sprite;
                atkPattern = EnemyData.AttackPattern.Random;
            }
        }
    }
}
