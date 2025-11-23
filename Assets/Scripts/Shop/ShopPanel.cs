using UnityEngine;
using UnityEngine.UI;

public class ShopPanel : MonoBehaviour
{
    [Header("UI References")]
    public CanvasGroup canvasGroup;
    public Button closeButton;       // Nut dong shop
    public Text goldDisplayText;     // Hien thi so vang trong shop (Legacy UI)
    
    [Header("Shop Items")]
    public ShopItem[] shopItems;     // Danh sach item ban trong shop
    
    [Header("Item Buttons")]
    public Button[] itemButtons;     // Cac nut mua item
    public Text[] itemNameTexts;     // Ten item (Legacy UI)
    public Text[] itemPriceTexts;    // Gia item (Legacy UI)

    [Header("Stats Display")]
    public Text statsDisplayText;    // Hien thi chi so nhan vat (Health, Damage, Speed)

    [Header("Animation Settings")]
    public float fadeSpeed = 10f;    // Toc do fade in/out

    private bool isOpen = false;
    private bool isAnimating = false;
    private float targetAlpha;
    private SoldierController soldierController;

    void Start()
    {
        // Khoi tao - panel tat ban dau
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // Gan su kien cho nut dong
        if (closeButton != null)
            closeButton.onClick.AddListener(Close);

        // Tim SoldierController de lay thong tin chi so
        soldierController = GameObject.FindGameObjectWithTag("Player")?.GetComponent<SoldierController>();

        // Khoi tao cac nut mua item
        SetupItemButtons();
    }

    void SetupItemButtons()
    {
        // Gan su kien cho tung nut mua item
        for (int i = 0; i < itemButtons.Length && i < shopItems.Length; i++)
        {
            int index = i;  // Capture index cho closure
            
            if (itemButtons[i] != null)
            {
                itemButtons[i].onClick.AddListener(() => BuyItem(index));
            }

            // Cap nhat text hien thi
            if (i < itemNameTexts.Length && itemNameTexts[i] != null && i < shopItems.Length && shopItems[i] != null)
            {
                itemNameTexts[i].text = shopItems[i].itemName;
            }

            if (i < itemPriceTexts.Length && itemPriceTexts[i] != null && i < shopItems.Length && shopItems[i] != null)
            {
                itemPriceTexts[i].text = shopItems[i].price.ToString();
            }
        }
    }

    void Update()
    {
        // Xu ly fade in/out animation
        if (isAnimating)
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
            
            // Hoan thanh animation khi alpha gan bang target
            if (Mathf.Abs(canvasGroup.alpha - targetAlpha) < 0.01f)
            {
                canvasGroup.alpha = targetAlpha;
                isAnimating = false;

                // Tat UI khi alpha = 0
                if (targetAlpha == 0)
                {
                    canvasGroup.interactable = false;
                    canvasGroup.blocksRaycasts = false;
                }
            }
        }

        // Cap nhat hien thi vang
        UpdateGoldDisplay();
        
        // Cap nhat hien thi chi so nhan vat khi shop mo
        if (isOpen)
        {
            UpdateStatsDisplay();
        }
    }

    void UpdateGoldDisplay()
    {
        if (goldDisplayText != null && CurrencyManager.Instance != null)
        {
            goldDisplayText.text = CurrencyManager.Instance.GetGold().ToString();
        }
    }

    void UpdateStatsDisplay()
    {
        if (statsDisplayText == null || soldierController == null)
            return;

        string statsText = string.Format(
            "Máu: {0}/{1}\nSát Thương: {2}\nTốc Độ: {3:F1}",
            soldierController.GetCurrentHealth(),
            soldierController.GetMaxHealth(),
            soldierController.GetAttackDamage(),
            soldierController.GetMoveSpeed()
        );

        statsDisplayText.text = statsText;
    }

    public void Open()
    {
        if (isOpen) return;

        isOpen = true;
        targetAlpha = 1f;
        
        // Bat tuong tac
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        
        // Su dung fade animation
        isAnimating = true;
        
        // Cap nhat trang thai cac nut (kich hoat/vo hieu hoa neu khong du tien)
        UpdateButtonStates();
        
        Debug.Log("Shop mo!");
    }

    public void Close()
    {
        if (!isOpen) return;

        isOpen = false;
        isAnimating = true;
        targetAlpha = 0;
        
        // Tat hien thi chi so khi dong shop
        if (statsDisplayText != null)
        {
            statsDisplayText.text = "";
        }
        
        Debug.Log("Shop dong!");
    }

    public bool IsOpen()
    {
        return isOpen;
    }

    void BuyItem(int itemIndex)
    {
        // Kiem tra index hop le
        if (itemIndex < 0 || itemIndex >= shopItems.Length)
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

        // Kiem tra va tru vang
        if (CurrencyManager.Instance != null && CurrencyManager.Instance.SpendGold(item.price))
        {
            Debug.Log("Da mua: " + item.itemName + " gia " + item.price + " vang");
            
            // Ap dung hieu ung item
            ApplyItemEffect(item);
            
            // Cap nhat trang thai cac nut
            UpdateButtonStates();
        }
        else
        {
            Debug.Log("Khong du vang de mua " + item.itemName + "!");
        }
    }

    void ApplyItemEffect(ShopItem item)
    {
        // Tim Soldier de ap dung hieu ung
        GameObject soldierObj = GameObject.FindGameObjectWithTag("Player");
        if (soldierObj == null)
        {
            Debug.LogWarning("Khong tim thay Player!");
            return;
        }

        // Su dung SendMessage de tranh loi reference
        switch (item.itemType)
        {
            case ItemType.Health:
                soldierObj.SendMessage("Heal", item.value, SendMessageOptions.DontRequireReceiver);
                Debug.Log("Hoi " + item.value + " mau");
                break;

            case ItemType.MaxHealth:
                soldierObj.SendMessage("IncreaseMaxHealth", item.value, SendMessageOptions.DontRequireReceiver);
                Debug.Log("Tang mau toi da: +" + item.value);
                break;

            case ItemType.Damage:
                soldierObj.SendMessage("IncreaseDamage", item.value, SendMessageOptions.DontRequireReceiver);
                Debug.Log("Tang sat thuong: +" + item.value);
                break;

            case ItemType.Speed:
                soldierObj.SendMessage("IncreaseSpeed", (float)item.value, SendMessageOptions.DontRequireReceiver);
                Debug.Log("Tang toc do: +" + item.value);
                break;

            default:
                Debug.LogWarning("Loai item chua duoc xu ly: " + item.itemType);
                break;
        }
    }

    void UpdateButtonStates()
    {
        // Kich hoat/vo hieu hoa cac nut dua tren so vang hien tai
        if (CurrencyManager.Instance == null) return;

        int currentGold = CurrencyManager.Instance.GetGold();

        for (int i = 0; i < itemButtons.Length && i < shopItems.Length; i++)
        {
            if (itemButtons[i] != null && shopItems[i] != null)
            {
                bool canAfford = currentGold >= shopItems[i].price;
                itemButtons[i].interactable = canAfford;
            }
        }
    }
}
