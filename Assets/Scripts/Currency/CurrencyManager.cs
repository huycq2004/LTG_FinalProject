using UnityEngine;
using System;
using System.Collections;

public class CurrencyManager : MonoBehaviour
{
    // Singleton pattern
    public static CurrencyManager Instance { get; private set; }

    private int currentGold;

    // Event de thong bao khi vang thay doi
    public event Action<int> OnGoldChanged;

    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Doi PlayerDataManager san sang roi moi tai vang
        StartCoroutine(InitializeGold());
    }

    // ====================
    // KHOI TAO VANG
    // ====================

    private IEnumerator InitializeGold()
    {
        // Doi cho den khi PlayerDataManager san sang
        while (PlayerDataManager.Instance == null)
        {
            Debug.Log("Dang doi PlayerDataManager khoi tao...");
            yield return null;
        }

        // Tai vang tu PlayerPrefs
        LoadGold();

        // Trigger event de cap nhat UI
        OnGoldChanged?.Invoke(currentGold);
    }

    // ====================
    // LUU VA TAI DU LIEU
    // ====================

    private void LoadGold()
    {
        if (PlayerDataManager.Instance != null)
        {
            currentGold = PlayerDataManager.Instance.LoadGold();
            Debug.Log("Tai vang tu PlayerPrefs: " + currentGold);
        }
        else
        {
            Debug.LogError("PlayerDataManager khong san sang khi tai vang!");
        }
    }

    private void SaveGold()
    {
        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.SaveGold(currentGold);
        }
    }

    // Lay so vang hien tai
    public int GetGold()
    {
        return currentGold;
    }

    // Them vang
    public void AddGold(int amount)
    {
        if (amount <= 0) return;

        currentGold += amount;
        Debug.Log("Them vang: +" + amount + " | Tong: " + currentGold);
        
        // Luu vang vao PlayerPrefs
        SaveGold();
        
        // Thong bao vang da thay doi
        OnGoldChanged?.Invoke(currentGold);
    }

    // Tru vang (tra ve true neu thanh cong)
    public bool SpendGold(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("So vang phai lon hon 0!");
            return false;
        }

        if (currentGold >= amount)
        {
            currentGold -= amount;
            Debug.Log("Tru vang: -" + amount + " | Con lai: " + currentGold);
            
            // Luu vang vao PlayerPrefs
            SaveGold();
            
            // Thong bao vang da thay doi
            OnGoldChanged?.Invoke(currentGold);
            return true;
        }
        else
        {
            Debug.Log("Khong du vang! Can: " + amount + " | Co: " + currentGold);
            return false;
        }
    }

    // Kiem tra co du vang hay khong
    public bool HasEnoughGold(int amount)
    {
        return currentGold >= amount;
    }

    // Reset vang ve gia tri mac dinh
    public void ResetGold()
    {
        // Lay gia tri mac dinh tu PlayerDataManager
        int defaultGold = PlayerDataManager.Instance != null ? 
            PlayerDataManager.Instance.defaultGold : 100;
            
        currentGold = defaultGold;
        SaveGold();
        OnGoldChanged?.Invoke(currentGold);
        Debug.Log("Reset vang ve: " + defaultGold);
    }
}
