using UnityEngine;
using System;

/// Soul Manager - Quan ly he thong Soul giong Hollow Knight
/// Danh trung ke dich 5 lan se hoi mau
public class SoulManager : MonoBehaviour
{
    // ====================
    // SINGLETON
    // ====================

    public static SoulManager Instance { get; private set; }

    // ====================
    // THONG SO
    // ====================

    [Header("Soul Settings")]
    public int maxSoulCount = 5;  // So soul can de hoi mau
    public int healAmount = 1;    // Gia tri mac dinh, se duoc override boi PlayerDataManager

    private int currentSoulCount = 0;

    // Event de thong bao khi soul thay doi
    public event Action<int, int> OnSoulCountChanged;  // (current, max)
    public event Action OnSoulFull;  // Khi du soul de hoi mau
    public event Action OnSoulUsed;  // Khi da dung soul hoi mau

    // ====================
    // KHOI TAO
    // ====================

    void Awake()
    {
        SetupSingleton();
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

    void Start()
    {
        // Tai heal amount tu PlayerPrefs
        LoadHealAmount();
        
        // Trigger event de cap nhat UI
        OnSoulCountChanged?.Invoke(currentSoulCount, maxSoulCount);
    }

    // ====================
    // QUAN LY SOUL
    // ====================

    /// Goi ham nay khi danh trung ke dich
    public void AddSoul()
    {
        if (currentSoulCount >= maxSoulCount)
        {
            Debug.Log("Soul da day!");
            return;
        }

        currentSoulCount++;
        Debug.Log(string.Format("Thu thap Soul! {0}/{1}", currentSoulCount, maxSoulCount));

        // Thong bao thay doi
        OnSoulCountChanged?.Invoke(currentSoulCount, maxSoulCount);

        // Kiem tra da du soul chua
        if (currentSoulCount >= maxSoulCount)
        {
            OnSoulFull?.Invoke();
            TryHealPlayer();
        }
    }

    /// Tu dong hoi mau khi du soul
    void TryHealPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            // Tim component SoldierController
            MonoBehaviour[] components = playerObj.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour comp in components)
            {
                // Kiem tra neu la SoldierController
                if (comp.GetType().Name == "SoldierController")
                {
                    // Goi method Heal bang reflection
                    System.Reflection.MethodInfo healMethod = comp.GetType().GetMethod("Heal");

                    if (healMethod != null)
                    {
                        // Hoi mau khong can kiem tra mau da day chua
                        healMethod.Invoke(comp, new object[] { healAmount });
                        UseSoul();
                        Debug.Log("Hoi mau tu Soul!");
                    }
                    break;
                }
            }
        }
    }

    /// Reset soul ve 0 sau khi hoi mau
    void UseSoul()
    {
        currentSoulCount = 0;
        OnSoulUsed?.Invoke();
        OnSoulCountChanged?.Invoke(currentSoulCount, maxSoulCount);
    }

    // ====================
    // LUU VA TAI DU LIEU
    // ====================

    void LoadHealAmount()
    {
        if (PlayerDataManager.Instance != null)
        {
            healAmount = PlayerDataManager.Instance.LoadHealAmount();
            // Debug.Log("Tai heal amount tu PlayerPrefs: " + healAmount);
        }
    }

    public void IncreaseHealAmount(int amount)
    {
        healAmount += amount;
        Debug.Log("Tang luong mau hoi: +" + amount);

        // Luu heal amount
        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.SaveHealAmount(healAmount);
        }
    }

    public int GetHealAmount()
    {
        return healAmount;
    }

    // ====================
    // GETTER
    // ====================

    public int GetCurrentSoulCount()
    {
        return currentSoulCount;
    }

    public int GetMaxSoulCount()
    {
        return maxSoulCount;
    }

    public bool IsSoulFull()
    {
        return currentSoulCount >= maxSoulCount;
    }

    // ====================
    // RESET
    // ====================

    public void ResetSoul()
    {
        currentSoulCount = 0;
        OnSoulCountChanged?.Invoke(currentSoulCount, maxSoulCount);
        Debug.Log("Reset Soul ve 0");
    }
}
