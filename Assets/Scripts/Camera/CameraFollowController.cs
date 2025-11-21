using UnityEngine;

public class CameraFollowController : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform player;
    public float smoothSpeed = 0.1f;
    public Vector3 offset = new Vector3(0, 1, -10);

    [Header("Zoom Settings")]
    public float cameraSize = 5f;

    [Header("Bounds")]
    public bool useBounds = false;
    public float minX = -10f;
    public float maxX = 10f;
    public float minY = -5f;
    public float maxY = 5f;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = GetComponent<Camera>();
        
        // Nếu chưa gán player, tự động tìm
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Đặt zoom camera
        if (mainCamera != null)
            mainCamera.orthographicSize = cameraSize;
    }

    void LateUpdate()
    {
        if (player == null) return;

        // Tính toán vị trí camera mục tiêu
        Vector3 desiredPosition = player.position + offset;

        // Smoothing - camera di chuyển mượt mà
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Áp dụng giới hạn (nếu bật)
        if (useBounds)
        {
            smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minX, maxX);
            smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minY, maxY);
        }

        // Cập nhật vị trí camera
        transform.position = smoothedPosition;
    }

    void OnDrawGizmosSelected()
    {
        // Vẽ offset trên Scene view
        if (player != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(player.position, player.position + offset);
        }

        // Vẽ bounds (nếu bật)
        if (useBounds)
        {
            Gizmos.color = Color.red;
            Vector3 topLeft = new Vector3(minX, maxY, 0);
            Vector3 topRight = new Vector3(maxX, maxY, 0);
            Vector3 bottomRight = new Vector3(maxX, minY, 0);
            Vector3 bottomLeft = new Vector3(minX, minY, 0);

            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(topRight, bottomRight);
            Gizmos.DrawLine(bottomRight, bottomLeft);
            Gizmos.DrawLine(bottomLeft, topLeft);
        }
    }
}
