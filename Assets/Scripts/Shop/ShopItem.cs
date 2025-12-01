using UnityEngine;

[System.Serializable]
public class ShopItem
{
    public string itemName;          // Ten item
    public string description;       // Mo ta item
    public int price;                // Gia item
    public ItemType itemType;        // Loai item
    public int value;                // Gia tri item (vd: +2 mau, +10 damage)
}

public enum ItemType
{
    Health,        // Hoi mau
    MaxHealth,     // Tang mau toi da
    Damage,        // Tang sat thuong
    Speed,         // Tang toc do
    Defense,       // Tang phong thu
    Bow            // Mo khoa khong nhan cung
}
