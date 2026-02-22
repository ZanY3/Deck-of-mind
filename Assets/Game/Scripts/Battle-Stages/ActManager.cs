using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ActManager : MonoBehaviour
{
    [SerializeField] private ActData[] acts;

    [Header("UI")]
    [SerializeField] private ActProgressBarUI[] actUIs;

    [Header("Background Fade Settings")]
    [SerializeField] private float bgFadeDuration = 0.5f;

    private int currentAct = 1;
    public int CurrentAct => currentAct;

    private void Start()
    {
        for (int i = 0; i < actUIs.Length; i++)
        {
            bool isActive = i == currentAct - 1;
            actUIs[i].gameObject.SetActive(isActive);

            if (actUIs[i].actBackground != null)
                actUIs[i].actBackground.SetActive(isActive);
        }
    }

    public ActData GetCurrentAct()
    {
        return acts[currentAct - 1];
    }

    public Image[] GetCurrentActStagePoints()
    {
        return actUIs[currentAct - 1].stagePointsImg;
    }

    public bool IsActLast()
    {
        return currentAct >= acts.Length;
    }

    public void NextAct()
    {
        int currentIndex = currentAct - 1;

        if (currentIndex >= actUIs.Length)
        {
            Debug.LogError("No more acts!");
            return;
        }

        // Выключаем старый UI
        actUIs[currentIndex].gameObject.SetActive(false);

        // Плавно выключаем старый фон
        if (actUIs[currentIndex].actBackground != null)
            StartCoroutine(FadeOutAndDisable(actUIs[currentIndex].actBackground));

        currentAct++;

        int newIndex = currentAct - 1;

        if (newIndex >= actUIs.Length)
        {
            Debug.LogError("Act UI index out of range!");
            return;
        }

        // Включаем новый UI
        actUIs[newIndex].gameObject.SetActive(true);

        // Включаем новый фон
        if (actUIs[newIndex].actBackground != null)
        {
            actUIs[newIndex].actBackground.SetActive(true);
            StartCoroutine(FadeIn(actUIs[newIndex].actBackground));
        }
    }

    // ================= FADE LOGIC =================

    private IEnumerator FadeOutAndDisable(GameObject bg)
    {
        Image img = bg.GetComponent<Image>();
        if (img == null)
        {
            bg.SetActive(false);
            yield break;
        }

        float t = 0f;
        Color startColor = img.color;

        while (t < bgFadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(startColor.a, 0f, t / bgFadeDuration);
            img.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        img.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
        bg.SetActive(false);
    }

    private IEnumerator FadeIn(GameObject bg)
    {
        Image img = bg.GetComponent<Image>();
        if (img == null)
            yield break;

        float t = 0f;
        Color startColor = img.color;
        img.color = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (t < bgFadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, t / bgFadeDuration);
            img.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        img.color = new Color(startColor.r, startColor.g, startColor.b, 1f);
    }
}