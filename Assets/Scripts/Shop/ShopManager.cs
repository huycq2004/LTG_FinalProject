using UnityEngine;
using UnityEngine.InputSystem;

public class ShopManager : MonoBehaviour
{
    [Header("Shop References")]
    public ShopPanel shopPanel;  // Reference den ShopPanel

    private bool canOpenShop = false;  // Co the mo shop hay khong

    void Update()
    {
        // Kiem tra neu nhan Space va cho phep mo shop
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (canOpenShop)
            {
                ToggleShop();
            }
            else
            {
                Debug.Log("Ban khong o gan Shop!");
            }
        }
    }

    void ToggleShop()
    {
        if (shopPanel == null)
        {
            Debug.LogWarning("ShopPanel chua duoc gan!");
            return;
        }

        if (shopPanel.IsOpen())
        {
            shopPanel.Close();
        }
        else
        {
            shopPanel.Open();
        }
    }

    // Duoc goi boi ShopTrigger
    public void SetCanOpenShop(bool canOpen)
    {
        canOpenShop = canOpen;
        
    }

    // Dong shop neu dang mo (khi nguoi choi roi khoi vung)
    public void CloseShopIfOpen()
    {
        if (shopPanel != null && shopPanel.IsOpen())
        {
            shopPanel.Close();
        }
    }
}
