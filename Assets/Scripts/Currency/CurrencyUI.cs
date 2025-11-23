using UnityEngine;
using UnityEngine.UI;

public class CurrencyUI : MonoBehaviour
{
    [Header("UI References")]
    public Text goldText;             // Text hien thi so vang (Legacy UI)
    
    [Header("Settings")]
    public string prefix = "Gold: ";  // Tien to truoc so vang
    public string suffix = "";        // Hau to sau so vang

    void Start()
    {
        // Dang ky lang nghe su kien thay doi vang
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnGoldChanged += UpdateGoldDisplay;
            
            // Cap nhat lan dau
            UpdateGoldDisplay(CurrencyManager.Instance.GetGold());
        }
        else
        {
            Debug.LogWarning("CurrencyManager khong ton tai!");
        }
    }

    void OnDestroy()
    {
        // Huy dang ky su kien khi destroy
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnGoldChanged -= UpdateGoldDisplay;
        }
    }

    void UpdateGoldDisplay(int goldAmount)
    {
        string displayText = prefix + goldAmount.ToString() + suffix;

        // Cap nhat Text neu co
        if (goldText != null)
        {
            goldText.text = displayText;
        }
    }
}
