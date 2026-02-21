using UnityEngine;
using UnityEngine.UI;

public class ActManager : MonoBehaviour
{
    [SerializeField] private ActData[] acts;

    [Space]
    [Header("UI")]
    [SerializeField] private ActProgressBarUI[] actUIs;
    private int currentAct = 1;
    [HideInInspector] public int CurrentAct => currentAct;

    private void Start()
    {
        for (int i = 0; i < actUIs.Length; i++)
        {
            actUIs[i].gameObject.SetActive(i == currentAct - 1);
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
        if(currentAct >= acts.Length)
        {
            return true;
        }
        return false;
    }

    public void NextAct()
    {
        int currentIndex = currentAct - 1;

        actUIs[currentIndex].gameObject.SetActive(false);


        if (currentAct > acts.Length)
        {
            Debug.LogError("No more acts!");
            return;
        }
        currentAct++;

        int newIndex = currentAct - 1;

        actUIs[newIndex].gameObject.SetActive(true);
    }
}
