using UnityEngine;
using UnityEngine.InputSystem;

public class MinimalCameraFollow : MonoBehaviour
{
    [Header("Movement settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float moveAmount = 0.5f;

    [Header("Camera bounds")]
    [SerializeField] private Vector2 minBounds;
    [SerializeField] private Vector2 maxBounds;

    [HideInInspector] public bool canMoveCam = true;
    private Camera mainCamera;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if(canMoveCam)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(mousePos);
            mouseWorld.z = transform.position.z;

            float offsetX = Mathf.Clamp(mouseWorld.x - transform.position.x, -moveAmount, moveAmount);
            float offsetY = Mathf.Clamp(mouseWorld.y - transform.position.y, -moveAmount, moveAmount);

            Vector3 targetPos = new Vector3(
                transform.position.x + offsetX,
                transform.position.y + offsetY,
                transform.position.z
            );

            targetPos.x = Mathf.Clamp(targetPos.x, minBounds.x, maxBounds.x);
            targetPos.y = Mathf.Clamp(targetPos.y, minBounds.y, maxBounds.y);

            transform.position = Vector3.Lerp(transform.position, targetPos, moveSpeed * Time.deltaTime);
        }
    }
}
