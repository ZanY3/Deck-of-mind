using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class DefenseCell : MonoBehaviour//, IDropHandler
{
    [SerializeField] private int defenseAmout;
    [SerializeField] private int defensePerTurn;
    [HideInInspector] public bool refreshDefenseEveryTurn = false;
    [SerializeField] private EnemyDropTarget dropTarget;

    [Space]
    [Header("UI")]
    [SerializeField] private TMP_Text defenseAmoutTxt;

    [Space]
    [Header("Sounds")]
    [SerializeField] private AudioClip debuffSound;
    [SerializeField] private AudioClip defenseBreakSound;
    [Range(0f, 1f)][SerializeField] private float debuffVolume;
    [Range(0f, 1f)][SerializeField] private float defenseBreakVolume;


    //[HideInInspector] public bool canAttackEnemy = false;

    private Enemy enemy;
    private int startDefenseAmout;

    public bool RefreshDefenseEveryTurn => refreshDefenseEveryTurn;
    public bool defenseIsActive = true;

    private void Start()
    {
        dropTarget.canBeAttacked = false;
        //dropTarget.enabled = false;
        enemy = GetComponentInParent<Enemy>();
        startDefenseAmout = defenseAmout;
        UpdateUI();
    }
//--------------------------------------------------------------------------------------
    public void RefillDefense()
    {
        float randPitch = Random.Range(0.9f, 1.1f);
        SoundManager.Instance.PlaySFX(debuffSound, randPitch, debuffVolume);
        if (refreshDefenseEveryTurn)
        {
            defenseAmout = startDefenseAmout;
            UpdateUI();
        }
    }
    public void DecreaseDefense(int value)
    {
        if(defenseAmout > value)
        {
            defenseAmout -= value;
        }
        else if (defenseAmout <= value)
        {
            float randPitch = Random.Range(0.9f, 1.1f);
            SoundManager.Instance.PlaySFX(defenseBreakSound, randPitch, defenseBreakVolume);
            defenseIsActive = false;
            dropTarget.canBeAttacked = true;
            //dropTarget.enabled = true;

            gameObject.SetActive(false);
        }
        UpdateUI();
    }
    public void UpdateUI()
    {
       defenseAmoutTxt.text = defenseAmout.ToString();
    }
}
