using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New_Act", menuName = "Scriptable Objects/Act")]

public class ActData : ScriptableObject
{
    public string actNameOnEnglish;
    public string actNameOnRussian;
    public int index;

    public int stagesAtAll;
    public List<GameObject> enemiesPrefabs;
    public int[] stagesWithStrongEnemies;
}
