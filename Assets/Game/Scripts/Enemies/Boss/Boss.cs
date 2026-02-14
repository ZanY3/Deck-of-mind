using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Boss : Enemy
{
    public Sprite phase2Sprite;
    private BossPhaseController phaseController;

    [Header("Sounds")]
    [SerializeField] private AudioClip bossPhaseTransitionSound;
    [SerializeField] private AudioClip summonSound;
    [Range(0f, 1f)][SerializeField] private float bossPhaseTransitionVolume;
    [Range(0f, 1f)][SerializeField] private float summonVolume;

    private int attackTurnCounter = 0;
    private Image image;

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

        if((attackTurnCounter == 2 || attackTurnCounter == 6 || attackTurnCounter == 10) && stunned == false)
        {
            float randPitch = Random.Range(0.95f, 1.05f);
            SoundManager.Instance.PlaySFX(summonSound, randPitch, summonVolume);
            DOVirtual.DelayedCall(0.5f, () =>
            {
                phaseController.SummonEnemies();
            });
        }

        if(currentHealth < maxHealth / 2.5f)// PHASE 2
        {
            SoundManager.Instance.PlaySFX(bossPhaseTransitionSound, 1, bossPhaseTransitionVolume);
            image.sprite = phase2Sprite;
            damage += 2;
            UpdateUI();
            attackTurnCounter = 0;
        }
    }
}
