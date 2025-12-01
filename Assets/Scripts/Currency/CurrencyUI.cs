using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CurrencyUI : MonoBehaviour
{
    [Header("UI References")]
    public Text goldText;             // Text hien thi so vang (Legacy UI)
    
    [Header("Settings")]
    public string prefix = "Gold: ";  // Tien to truoc so vang
    public string suffix = "";        // Hau to sau so vang

    void OnEnable()
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
            // Neu CurrencyManager chua san sang, cho va thu lai
            StartCoroutine(WaitForCurrencyManager());
        }
    }

    void OnDisable()
    {
        // Huy dang ky su kien khi disable
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnGoldChanged -= UpdateGoldDisplay;
        }
    }

    void Start()
    {
        // Dam bao hien thi dung khi khoi dong
        if (CurrencyManager.Instance != null)
        {
            UpdateGoldDisplay(CurrencyManager.Instance.GetGold());
        }
    }

    IEnumerator WaitForCurrencyManager()
    {
        // Doi CurrencyManager san sang
        while (CurrencyManager.Instance == null)
        {
            yield return null;
        }

        // Dang ky event
        CurrencyManager.Instance.OnGoldChanged += UpdateGoldDisplay;
        
        // Cap nhat UI
        UpdateGoldDisplay(CurrencyManager.Instance.GetGold());
        
        Debug.Log("CurrencyUI: Da ket noi voi CurrencyManager");
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
            Debug.Log("Cap nhat UI Gold: " + goldAmount);
        }
    }
}
