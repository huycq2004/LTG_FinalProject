using UnityEngine;

public class ShopTrigger : MonoBehaviour
{
    [Header("Settings")]
    public string playerTag = "Player";  // Tag cua nguoi choi
    public string shopManagerName = "ShopManager";  // Ten cua GameObject ShopManager
    
    private GameObject shopManagerObj;

    void Start()
    {
        // Tim ShopManager trong scene
        shopManagerObj = GameObject.Find(shopManagerName);
        if (shopManagerObj == null)
        {
            Debug.LogWarning("Khong tim thay ShopManager trong scene!");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Kiem tra neu la Player va cham vao trigger
        if (other.CompareTag(playerTag))
        {
            Debug.Log("Nguoi choi vao vung Shop!");
            
            // Thong bao cho ShopManager rang co the mo shop
            if (shopManagerObj != null)
            {
                shopManagerObj.SendMessage("SetCanOpenShop", true, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Kiem tra neu Player roi khoi trigger
        if (other.CompareTag(playerTag))
        {
            Debug.Log("Nguoi choi roi khoi vung Shop!");
            
            // Thong bao cho ShopManager rang khong the mo shop
            if (shopManagerObj != null)
            {
                shopManagerObj.SendMessage("SetCanOpenShop", false, SendMessageOptions.DontRequireReceiver);
                shopManagerObj.SendMessage("CloseShopIfOpen", SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}
