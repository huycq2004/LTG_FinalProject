using UnityEngine;
using System;

public class CurrencyManager : MonoBehaviour
{
    // Singleton pattern
    public static CurrencyManager Instance { get; private set; }

    [Header("Currency Settings")]
    public int startingGold = 100;  // So vang khoi dau

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

        // Khoi tao vang
        currentGold = startingGold;
    }

    void Start()
    {
        // Trigger event de cap nhat UI
        OnGoldChanged?.Invoke(currentGold);
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
        currentGold = startingGold;
        OnGoldChanged?.Invoke(currentGold);
        Debug.Log("Reset vang ve: " + startingGold);
    }
}
