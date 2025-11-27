using UnityEngine;
using UnityEngine.UI;

/// Enemy Count UI - Hien thi so luong ke dich can ha guc tren Canvas
/// Su dung Legacy Text de hien thi
public class EnemyCountUI : MonoBehaviour
{
    [Header("UI References")]
    public Text countdownText;  // Legacy Text component

    [Header("Display Settings")]
    public string prefixText = "Ke dich: ";  // Text truoc so
    public Color normalColor = Color.white;
    public Color completedColor = Color.green;

    // ====================
    // KHOI TAO
    // ====================

    void Start()
    {
        ValidateComponents();
        SubscribeToEvents();
        // QUAN TRONG: Cap nhat UI ngay lap tuc thay vi cho event
        ForceUpdateDisplay();
    }

    void ValidateComponents()
    {
        if (countdownText == null)
        {
            Debug.LogError("Countdown Text chua duoc gan trong Inspector!");
        }
    }

    void SubscribeToEvents()
    {
        if (EnemyCountManager.Instance != null)
        {
            // Dang ky lang nghe su kien thay doi
            EnemyCountManager.Instance.OnEnemyCountChanged += UpdateDisplay;
            EnemyCountManager.Instance.OnTargetReached += OnTargetComplete;
        }
        else
        {
            Debug.LogWarning("EnemyCountManager Instance khong ton tai!");
        }
    }

    void ForceUpdateDisplay()
    {
        if (EnemyCountManager.Instance != null)
        {
            int current = EnemyCountManager.Instance.GetCurrentKillCount();
            int target = EnemyCountManager.Instance.GetTargetCount();
            UpdateDisplay(current, target);
        }
        else
        {
            // Hien thi mac dinh neu chua co Manager
            UpdateDisplay(0, 0);
        }
    }

    void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    void UnsubscribeFromEvents()
    {
        if (EnemyCountManager.Instance != null)
        {
            EnemyCountManager.Instance.OnEnemyCountChanged -= UpdateDisplay;
            EnemyCountManager.Instance.OnTargetReached -= OnTargetComplete;
        }
    }

    // ====================
    // CAP NHAT UI
    // ====================

    void UpdateDisplay(int current, int target)
    {
        if (countdownText == null) return;

        // Hien thi format: "Ke dich: 5/10"
        countdownText.text = string.Format("{0}{1}/{2}", prefixText, current, target);

        // Doi mau neu hoan thanh
        if (current >= target)
        {
            countdownText.color = completedColor;
        }
        else
        {
            countdownText.color = normalColor;
        }
    }

    void OnTargetComplete()
    {
        Debug.Log("UI: Hoan thanh muc tieu!");

        // Co the them hieu ung o day (animation, particle, etc.)
        // Vi du: PlayCompletionAnimation();
    }
}