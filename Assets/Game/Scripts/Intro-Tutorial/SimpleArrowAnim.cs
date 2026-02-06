using UnityEngine;

public class SimpleArrowAnim : MonoBehaviour
{
    private Vector3 startPos;

    public float moveAmount = 10f;
    public float speed = 2f;

    private void Start()
    {
        startPos = transform.localPosition;
    }

    private void Update()
    {
        float y = Mathf.Sin(Time.time * speed) * moveAmount;
        transform.localPosition = startPos + Vector3.up * y;
    }
}
