using UnityEngine;

public class CameraFollowController : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform player;
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0, 1, -10);

    [Header("Look Ahead")]
    public float lookAheadDistance = 3f;   // Camera nhìn trước
    public float lookAheadSmooth = 3f;     // Mượt khi đổi hướng

    private Vector3 currentLookAhead;

    [Header("Zoom Settings")]
    public float cameraSize = 5f;

    [Header("Bounds")]
    public bool useBounds = false;
    public float minX = -10f;
    public float maxX = 10f;
    public float minY = -5f;
    public float maxY = 5f;

    private Camera mainCamera;
    private SpriteRenderer playerRenderer;

    void Start()
    {
        mainCamera = GetComponent<Camera>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player != null)
            playerRenderer = player.GetComponent<SpriteRenderer>();

        if (mainCamera != null)
            mainCamera.orthographicSize = cameraSize;
    }

    void LateUpdate()
    {
        if (player == null) return;

        // ============================
        // 1. LOOK AHEAD (Dead Cells)
        // ============================
        float lookAheadX = 0;

        if (playerRenderer != null)
        {
            lookAheadX = playerRenderer.flipX ? -lookAheadDistance : lookAheadDistance;
        }

        // Mượt khi đổi hướng
        currentLookAhead = Vector3.Lerp(
            currentLookAhead,
            new Vector3(lookAheadX, 0, 0),
            Time.deltaTime * lookAheadSmooth
        );

        // ============================
        // 2. CAMERA TARGET POSITION
        // ============================
        Vector3 desiredPosition =
            player.position + offset + currentLookAhead;

        // ============================
        // 3. APPLY SMOOTH FOLLOW
        // ============================
        Vector3 smoothedPosition = Vector3.Lerp(
            transform.position,
            desiredPosition,
            Time.deltaTime * smoothSpeed
        );

        // ============================
        // 4. MAP BOUNDS
        // ============================
        if (useBounds)
        {
            smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minX, maxX);
            smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minY, maxY);
        }

        transform.position = smoothedPosition;
    }
}
