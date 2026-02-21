using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private int maxHealth;

    private int currentHealth;
    private PlayerDefense defense;

    [HideInInspector] public bool hasAnxiety = false;
    [HideInInspector] public int anxietyDamage = 0;
    [HideInInspector] public bool stunned = false;

    [Space]
    [Header("UI/HealthBar")]
    [SerializeField] private GameObject stunClue;
    [SerializeField] private GameObject anxietyDebuffImg;
    [SerializeField] private GameObject stunDebuffImg;
    [SerializeField] private GameObject cardDraggingImg;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private TMP_Text healthTxt;

    [SerializeField] private Image bloodVignetteImg;

    [Header("Sound")]
    [SerializeField] private AudioClip takeDamageSound;
    [SerializeField] private AudioClip healSound;
    [Range(0f, 1f)][SerializeField] private float healVolume;
    [Range(0f, 1f)][SerializeField] private float takeDamageVolume = 0.1f;

    [HideInInspector] public int turnsUntilStunRemove = 0;
    private Tween clueTween;

    // ===== VIGNETTE SETTINGS =====
    private float maxVignetteAlpha = 150f / 255f;
    private float vignetteSmooth = 5f;

    private void Start()
    {
        defense = GetComponent<PlayerDefense>();
        currentHealth = maxHealth;
        UpdateUI();
    }

    //--------------------------------------------------------------------------------------

    public void TakeDamage(int damage, bool useShield)
    {
        float randPitch = Random.Range(0.7f, 1.2f);
        SoundManager.Instance.PlaySFX(takeDamageSound, randPitch, takeDamageVolume);

        if (currentHealth > 0)
        {
            CameraShake.Shake(0.2f, 0.3f);

            if (useShield)
                currentHealth -= defense.CalculateDamage(damage);
            else
                currentHealth -= damage;

            UpdateUI();
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            UpdateUI();
            battleManager.PlayerLose();
        }
    }

    public void ChangeStunState(bool state)
    {
        stunned = state;
        stunDebuffImg.SetActive(state);
        UpdateUI();
    }

    public void ChangeStunClueState(bool state)
    {
        stunClue.SetActive(state);
    }

    public void ChangeDraggingClueState(bool state)
    {
        cardDraggingImg.SetActive(state);

        Image image = cardDraggingImg.GetComponent<Image>();
        clueTween?.Kill();

        if (state)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
            clueTween = image.DOFade(0.1f, 0.2f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetAutoKill(true)
                .SetUpdate(true);
        }
        else
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        }
    }

    public void ClearAllDebuffs()
    {
        anxietyDamage = 0;
        hasAnxiety = false;
        turnsUntilStunRemove = 0;
        ChangeStunState(false);
        UpdateUI();
    }

    public void Heal(int amoutToHeal)
    {
        float randPitch = Random.Range(0.85f, 1.05f);
        SoundManager.Instance.PlaySFX(healSound, randPitch, healVolume);
        currentHealth += amoutToHeal;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        UpdateUI();
    }
    public bool CanApplyHeal(int amout)
    {
        if(currentHealth == maxHealth)
            return false;

        return true;
    }

    public void UpdateUI()
    {
        if (healthTxt != null && healthBarImage != null)
        {
            anxietyDebuffImg.SetActive(hasAnxiety);
            stunDebuffImg.SetActive(stunned);

            // ===== ВОЗВРАЩЕНО: цвет игрока по дебаффам =====
            if (hasAnxiety && !stunned)
            {
                GetComponent<Image>().color = new Color32(224, 255, 194, 255); // #E0FFC2
            }
            else if (stunned)
            {
                GetComponent<Image>().color = new Color32(206, 126, 255, 255); // #CE7EFF
            }
            else
            {
                GetComponent<Image>().color = Color.white;
            }

            healthBarImage.fillAmount = (float)currentHealth / maxHealth;
            healthTxt.text = currentHealth + "/" + maxHealth;

            // ===== BLOOD VIGNETTE =====
            if (bloodVignetteImg != null)
            {
                if (currentHealth <= 60)
                {
                    float t = Mathf.InverseLerp(60, 0, currentHealth); // 0 → 1
                    float targetAlpha = Mathf.Lerp(0f, maxVignetteAlpha, t);

                    Color c = bloodVignetteImg.color;
                    c.a = Mathf.Lerp(c.a, targetAlpha, Time.deltaTime * vignetteSmooth);
                    bloodVignetteImg.color = c;
                }
                else
                {
                    Color c = bloodVignetteImg.color;
                    c.a = Mathf.Lerp(c.a, 0f, Time.deltaTime * vignetteSmooth);
                    bloodVignetteImg.color = c;
                }
            }
        }
    }

    public bool HasDebuffs()
    {
        return hasAnxiety;
    }
}
