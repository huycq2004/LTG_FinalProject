using UnityEngine;
using UnityEngine.UI;


/// Shop Panel - Quan ly giao dien cua hang
/// Gom co: Hien thi item, Mua hang, Cap nhat chi so, Animation fade

public class ShopPanel : MonoBehaviour
{
    // ====================
    // CAI DAT CO BAN
    // ====================

    [Header("UI References")]
    public CanvasGroup canvasGroup;
    public Button closeButton;
    public Text goldDisplayText;

    [Header("Shop Items")]
    public ShopItem[] shopItems;

    [Header("Item Buttons")]
    public Button[] itemButtons;
    public Text[] itemNameTexts;
    public Text[] itemPriceTexts;

    [Header("Stats Display")]
    public Text statsDisplayText;

    [Header("Animation Settings")]
    public float fadeSpeed = 10f;

    // ====================
    // THANH PHAN
    // ====================

    private SoldierController soldierController;

    // ====================
    // TRANG THAI
    // ====================

    private bool isOpen;
    private bool isAnimating;

    // ====================
    // BIEN KHAC
    // ====================

    private float targetAlpha;

    // ====================
    // KHOI TAO
    // ====================

    void Start()
    {
        GetComponents();
        InitializePanel();
        SetupCloseButton();
        FindPlayerController();
        SetupItemButtons();
    }

    void GetComponents()
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
    }

    void InitializePanel()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    void SetupCloseButton()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Close);
        }
    }

    void FindPlayerController()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            soldierController = player.GetComponent<SoldierController>();
        }
    }

    void SetupItemButtons()
    {
        for (int i = 0; i < itemButtons.Length && i < shopItems.Length; i++)
        {
            SetupSingleButton(i);
            UpdateItemDisplay(i);
        }
    }

    void SetupSingleButton(int index)
    {
        if (itemButtons[index] != null)
        {
            itemButtons[index].onClick.AddListener(() => BuyItem(index));
        }
    }

    void UpdateItemDisplay(int index)
    {
        UpdateItemName(index);
        UpdateItemPrice(index);
    }

    void UpdateItemName(int index)
    {
        if (IsValidItemDisplay(index, itemNameTexts))
        {
            itemNameTexts[index].text = shopItems[index].itemName;
        }
    }

    void UpdateItemPrice(int index)
    {
        if (IsValidItemDisplay(index, itemPriceTexts))
        {
            // Kiem tra neu la item Bow da mua thi hien thi "DA MUA"
            if (shopItems[index].itemType == ItemType.Bow)
            {
                bool hasBow = PlayerDataManager.Instance != null && PlayerDataManager.Instance.LoadHasBow();
                if (hasBow)
                {
                    itemPriceTexts[index].text = "SOLD";
                    return;
                }
            }

            itemPriceTexts[index].text = shopItems[index].price.ToString();
        }
    }

    bool IsValidItemDisplay(int index, Text[] textArray)
    {
        return index < textArray.Length
            && textArray[index] != null
            && shopItems[index] != null;
    }

    // ====================
    // CAP NHAT CHINH
    // ====================

    void Update()
    {
        HandleFadeAnimation();
        UpdateGoldDisplay();

        if (isOpen)
        {
            UpdateStatsDisplay();
        }
    }

    // ====================
    // ANIMATION
    // ====================

    void HandleFadeAnimation()
    {
        if (!isAnimating) return;

        LerpAlpha();
        CheckAnimationComplete();
    }

    void LerpAlpha()
    {
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
    }

    void CheckAnimationComplete()
    {
        if (Mathf.Abs(canvasGroup.alpha - targetAlpha) < 0.01f)
        {
            CompleteAnimation();
        }
    }

    void CompleteAnimation()
    {
        canvasGroup.alpha = targetAlpha;
        isAnimating = false;

        if (targetAlpha == 0)
        {
            DisablePanelInteraction();
        }
    }

    void EnablePanelInteraction()
    {
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    void DisablePanelInteraction()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    // ====================
    // CAP NHAT HIEN THI
    // ====================

    void UpdateGoldDisplay()
    {
        if (CanUpdateGoldDisplay())
        {
            goldDisplayText.text = CurrencyManager.Instance.GetGold().ToString();
        }
    }

    bool CanUpdateGoldDisplay()
    {
        return goldDisplayText != null && CurrencyManager.Instance != null;
    }

    void UpdateStatsDisplay()
    {
        if (!CanUpdateStatsDisplay()) return;

        statsDisplayText.text = BuildStatsText();
    }

    bool CanUpdateStatsDisplay()
    {
        return statsDisplayText != null && soldierController != null;
    }

    string BuildStatsText()
    {
        return string.Format(
            "Máu: {0}/{1}\nSát Thương: {2}\nTốc Độ: {3:F1}",
            soldierController.GetCurrentHealth(),
            soldierController.GetMaxHealth(),
            soldierController.GetAttackDamage(),
            soldierController.GetMoveSpeed()
        );
    }

    void ClearStatsDisplay()
    {
        if (statsDisplayText != null)
        {
            statsDisplayText.text = "";
        }
    }

    // ====================
    // MO/DONG SHOP
    // ====================

    public void Open()
    {
        if (isOpen) return;

        isOpen = true;
        targetAlpha = 1f;
        isAnimating = true;

        EnablePanelInteraction();
        UpdateButtonStates();

        Debug.Log("Shop mo!");
    }

    public void Close()
    {
        if (!isOpen) return;

        isOpen = false;
        isAnimating = true;
        targetAlpha = 0;

        ClearStatsDisplay();

        Debug.Log("Shop dong!");
    }

    public bool IsOpen()
    {
        return isOpen;
    }

    // ====================
    // MUA ITEM
    // ====================

    void BuyItem(int itemIndex)
    {
        if (!IsValidItemIndex(itemIndex))
        {
            Debug.LogWarning("Item index khong hop le!");
            return;
        }

        ShopItem item = shopItems[itemIndex];
        if (item == null)
        {
            Debug.LogWarning("Item null!");
            return;
        }

        if (TryPurchaseItem(item))
        {
            ProcessSuccessfulPurchase(item);
        }
        else
        {
            ProcessFailedPurchase(item);
        }
    }

    bool IsValidItemIndex(int index)
    {
        return index >= 0 && index < shopItems.Length;
    }

    bool TryPurchaseItem(ShopItem item)
    {
        return CurrencyManager.Instance != null
            && CurrencyManager.Instance.SpendGold(item.price);
    }

    void ProcessSuccessfulPurchase(ShopItem item)
    {
        Debug.Log("Da mua: " + item.itemName + " gia " + item.price + " vang");
        ApplyItemEffect(item);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPurchaseSound();
        }

        UpdateButtonStates();
    }

    void ProcessFailedPurchase(ShopItem item)
    {
        Debug.Log("Khong du vang de mua " + item.itemName + "!");
    }

    // ====================
    // AP DUNG HIEU UNG ITEM
    // ====================

    void ApplyItemEffect(ShopItem item)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("Khong tim thay Player!");
            return;
        }

        switch (item.itemType)
        {
            case ItemType.Health:
                ApplyHealthEffect(player, item);
                break;

            case ItemType.MaxHealth:
                ApplyMaxHealthEffect(player, item);
                break;

            case ItemType.Damage:
                ApplyDamageEffect(player, item);
                break;

            case ItemType.Speed:
                ApplySpeedEffect(player, item);
                break;

            case ItemType.Bow:
                ApplyBowEffect(player, item);
                break;

            default:
                Debug.LogWarning("Loai item chua duoc xu ly: " + item.itemType);
                break;
        }
    }

    void ApplyHealthEffect(GameObject player, ShopItem item)
    {
        player.SendMessage("Heal", item.value, SendMessageOptions.DontRequireReceiver);
        Debug.Log("Hoi " + item.value + " mau");
    }

    void ApplyMaxHealthEffect(GameObject player, ShopItem item)
    {
        player.SendMessage("IncreaseMaxHealth", item.value, SendMessageOptions.DontRequireReceiver);
        Debug.Log("Tang mau toi da: +" + item.value);
    }

    void ApplyDamageEffect(GameObject player, ShopItem item)
    {
        player.SendMessage("IncreaseDamage", item.value, SendMessageOptions.DontRequireReceiver);
        Debug.Log("Tang sat thuong: +" + item.value);
    }

    void ApplySpeedEffect(GameObject player, ShopItem item)
    {
        player.SendMessage("IncreaseSpeed", (float)item.value, SendMessageOptions.DontRequireReceiver);
        Debug.Log("Tang toc do: +" + item.value);
    }

    void ApplyBowEffect(GameObject player, ShopItem item)
    {
        player.SendMessage("UnlockBow", SendMessageOptions.DontRequireReceiver);
        Debug.Log("Da mo khoa cung!");
    }

    // ====================
    // CAP NHAT TRANG THAI NUT
    // ====================

    void UpdateButtonStates()
    {
        if (CurrencyManager.Instance == null) return;

        int currentGold = CurrencyManager.Instance.GetGold();

        for (int i = 0; i < itemButtons.Length && i < shopItems.Length; i++)
        {
            UpdateSingleButtonState(i, currentGold);
        }
    }
    void UpdateSingleButtonState(int index, int currentGold)
    {
        if (itemButtons[index] != null && shopItems[index] != null)
        {
            bool canAfford = currentGold >= shopItems[index].price;

            // Kiem tra neu la item Bow - chi enable neu chua mua
            if (shopItems[index].itemType == ItemType.Bow)
            {
                bool hasBow = PlayerDataManager.Instance != null && PlayerDataManager.Instance.LoadHasBow();
                itemButtons[index].interactable = canAfford && !hasBow;
            }
            else
            {
                itemButtons[index].interactable = canAfford;
            }
        }
    }
}