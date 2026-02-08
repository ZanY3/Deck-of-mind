using UnityEngine;
using DG.Tweening;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;
    private void Awake()
    {
        Instance = this;
    }
    public void OnShake(float duration, float strength)
    {
        Debug.Log("Camera shake");
        transform.DOShakePosition(duration, strength);
        transform.DOShakeRotation(duration, strength);
    }
    public static void Shake(float duration, float strength) => Instance.OnShake(duration, strength);
}
