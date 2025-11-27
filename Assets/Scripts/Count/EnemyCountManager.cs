using UnityEngine;
using System;

/// Enemy Count Manager - Quan ly so luong ke dich can ha guc
/// Su dung Singleton pattern giong nhu AudioManager va CurrencyManager
public class EnemyCountManager : MonoBehaviour
{
    // ====================
    // SINGLETON
    // ====================

    public static EnemyCountManager Instance { get; private set; }

    // ====================
    // THONG SO
    // ====================

    [Header("Enemy Count Settings")]
    public int targetEnemyCount = 10;  // So ke dich can ha guc de thang

    private int currentKillCount = 0;   // So ke dich da ha guc

    // Event de thong bao khi so luong thay doi
    public event Action<int, int> OnEnemyCountChanged;  // (current, target)
    public event Action OnTargetReached;  // Khi dat du so luong

    // ====================
    // KHOI TAO
    // ====================

    void Awake()
    {
        SetupSingleton();
        InitializeCount();
    }

    void SetupSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeCount()
    {
        currentKillCount = 0;
    }

    void Start()
    {
        // Trigger event de cap nhat UI
        OnEnemyCountChanged?.Invoke(currentKillCount, targetEnemyCount);
    }

    // ====================
    // QUAN LY SO LUONG
    // ====================

    /// Goi ham nay khi ha guc mot ke dich
    public void AddKill()
    {
        if (currentKillCount >= targetEnemyCount)
        {
            Debug.Log("Da dat du so luong ke dich!");
            return;
        }

        currentKillCount++;
        Debug.Log($"Ha guc ke dich! {currentKillCount}/{targetEnemyCount}");

        // Thong bao thay doi
        OnEnemyCountChanged?.Invoke(currentKillCount, targetEnemyCount);

        // Kiem tra da dat muc tieu chua
        if (currentKillCount >= targetEnemyCount)
        {
            OnTargetReached?.Invoke();
            Debug.Log("CHIEN THANG! Da ha guc du so luong ke dich!");
        }
    }

    // ====================
    // GETTER
    // ====================

    public int GetCurrentKillCount() => currentKillCount;
    public int GetTargetCount() => targetEnemyCount;
    public int GetRemainingCount() => Mathf.Max(0, targetEnemyCount - currentKillCount);
    public bool IsTargetReached() => currentKillCount >= targetEnemyCount;

    // ====================
    // RESET
    // ====================

    public void ResetCount()
    {
        currentKillCount = 0;
        OnEnemyCountChanged?.Invoke(currentKillCount, targetEnemyCount);
        Debug.Log("Reset so luong ke dich ha guc");
    }

    public void SetTargetCount(int newTarget)
    {
        targetEnemyCount = Mathf.Max(1, newTarget);
        OnEnemyCountChanged?.Invoke(currentKillCount, targetEnemyCount);
        Debug.Log($"Dat muc tieu moi: {targetEnemyCount} ke dich");
    }
}