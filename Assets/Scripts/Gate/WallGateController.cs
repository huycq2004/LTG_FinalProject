using UnityEngine;

/// Wall Gate Controller - Kiem tra so luong quai da tieu diet va keo tuong/sprite len
/// Gan vao GameObject tuong hoac cong de mo duong khi du dieu kien
/// Tuong se block nguoi choi cho den khi du so luong quai
public class WallGateController : MonoBehaviour
{
    // ====================
    // CAI DAT
    // ====================

    [Header("Gate Settings")]
    [Tooltip("Vi tri Y tuong se di chuyen den (vi tri mo)")]
    public float targetYPosition = 5f;

    [Tooltip("Toc do di chuyen cua tuong")]
    public float moveSpeed = 2f;

    [Header("Player Detection")]
    [Tooltip("Khoang cach phat hien nguoi choi")]
    public float detectionRange = 3f;

    [Tooltip("Tag cua nguoi choi")]
    public string playerTag = "Player";

    // ====================
    // THANH PHAN
    // ====================

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Transform playerTransform;

    // ====================
    // TRANG THAI
    // ====================

    private bool isOpening = false;
    private bool isOpened = false;
    private bool canOpen = false;

    // ====================
    // KHOI TAO
    // ====================

    void Start()
    {
        SubscribeToEvents();
        SaveInitialPosition();
        FindPlayer();
    }

    void SubscribeToEvents()
    {
        if (EnemyCountManager.Instance != null)
        {
            EnemyCountManager.Instance.OnTargetReached += OnEnemyTargetReached;

            // Kiem tra neu da dat muc tieu tu truoc
            if (EnemyCountManager.Instance.IsTargetReached())
            {
                canOpen = true;
                Debug.Log("Da du so luong quai, cho nguoi choi den gan...");
            }
        }
        else
        {
            Debug.LogWarning("EnemyCountManager khong ton tai! WallGateController se khong hoat dong.");
        }
    }

    void SaveInitialPosition()
    {
        startPosition = transform.position;
        targetPosition = new Vector3(startPosition.x, targetYPosition, startPosition.z);
    }

    void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning($"Khong tim thay GameObject voi tag '{playerTag}'!");
        }
    }

    void OnDestroy()
    {
        // Huy dang ky event khi destroy
        if (EnemyCountManager.Instance != null)
        {
            EnemyCountManager.Instance.OnTargetReached -= OnEnemyTargetReached;
        }
    }

    // ====================
    // EVENT HANDLER
    // ====================

    void OnEnemyTargetReached()
    {
        canOpen = true;
        Debug.Log("Da du so luong quai! Tuong se mo khi nguoi choi den gan.");
    }

    // ====================
    // CAP NHAT
    // ====================

    void Update()
    {
        // Kiem tra neu da du dieu kien va chua mo
        if (canOpen && !isOpened && !isOpening)
        {
            CheckPlayerDistance();
        }

        // Xu ly di chuyen tuong
        if (isOpening)
        {
            MoveGateUp();
        }
    }

    void CheckPlayerDistance()
    {
        if (playerTransform == null)
        {
            FindPlayer(); // Thu tim lai neu chua co
            return;
        }

        // Tinh khoang cach giua nguoi choi va tuong
        float distance = Vector3.Distance(playerTransform.position, transform.position);

        // Neu nguoi choi den gan du, mo tuong
        if (distance <= detectionRange)
        {
            OpenGate();
        }
    }

    // ====================
    // MO CONG/TUONG
    // ====================

    void OpenGate()
    {
        if (isOpened || isOpening)
        {
            return;
        }

        Debug.Log("Mo cong/tuong!");

        isOpening = true;

    }


    void MoveGateUp()
    {
        // Di chuyen tuong len den vi tri muc tieu
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        // Kiem tra da den vi tri muc tieu chua
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            transform.position = targetPosition;
            isOpening = false;
            isOpened = true;
            Debug.Log("Cong/tuong da mo hoan toan!");
        }
    }

    // ====================
    // PUBLIC METHODS
    // ====================

    /// Kiem tra xem cong da mo chua
    public bool IsOpened() => isOpened;

    /// Kiem tra xem cong dang mo hay khong
    public bool IsOpening() => isOpening;

    /// Kiem tra xem da du dieu kien de mo chua
    public bool CanOpen() => canOpen;


    // ====================
    // GIZMOS (DE HIEN THI TRONG EDITOR)
    // ====================

    void OnDrawGizmos()
    {
        // Hien thi vi tri muc tieu trong Editor
        Vector3 startPos = transform.position;
        Vector3 targetPos = new Vector3(startPos.x, targetYPosition, startPos.z);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(startPos, targetPos);
        Gizmos.DrawWireSphere(targetPos, 0.3f);

        // Ve mui ten chi huong
        Gizmos.color = Color.yellow;
        Vector3 direction = (targetPos - startPos).normalized;
        Vector3 arrowTip = startPos + direction * Vector3.Distance(startPos, targetPos) * 0.5f;
        Gizmos.DrawSphere(arrowTip, 0.2f);

        // Hien thi pham vi phat hien nguoi choi
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawWireSphere(startPos, detectionRange);
    }
}
