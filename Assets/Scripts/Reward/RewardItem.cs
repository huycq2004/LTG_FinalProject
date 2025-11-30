using UnityEngine;

/// Reward Item - Vat pham nguoi choi nhan duoc khi chay map
/// Co the la HealAmount (so mau hoi khi dung Soul), Damage, Speed, hoac Gold
public class RewardItem : MonoBehaviour
{
    // ====================
    // LOAI BUFF
    // ====================

    public enum RewardType
    {
        HealAmount,     // Tang so mau hoi khi dung Soul
        Damage,         // Tang sat thuong
        Speed,          // Tang toc do di chuyen
        Gold            // Tang vang
    }

    // ====================
    // CAI DAT
    // ====================

    [Header("Loai Reward")]
    [Tooltip("Chon loai buff ma vat pham nay se tang")]
    public RewardType rewardType = RewardType.Gold;

    [Header("Gia Tri")]
    [Tooltip("Gia tri tang them (so luong)")]
    public int rewardValue = 1;

    [Header("Auto Destroy")]
    [Tooltip("Tu dong xoa sau khi nhat")]
    public bool destroyOnPickup = true;

    [Tooltip("Thoi gian cho truoc khi xoa (giay)")]
    public float destroyDelay = 0.1f;

    // ====================
    // TRANG THAI
    // ====================

    private bool hasBeenCollected = false;

    // ====================
    // VA CHAM VOI NGUOI CHOI
    // ====================

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiem tra neu la nguoi choi va chua nhat
        if (collision.CompareTag("Player") && !hasBeenCollected)
        {
            CollectReward(collision.gameObject);
        }
    }

    // ====================
    // XU LY NHAT VAT PHAM
    // ====================

    void CollectReward(GameObject player)
    {
        hasBeenCollected = true;

        // Ap dung buff theo loai
        ApplyRewardBuff(player);

        // Xoa vat pham neu can
        if (destroyOnPickup)
        {
            Destroy(gameObject, destroyDelay);
        }
    }

    void ApplyRewardBuff(GameObject player)
    {
        switch (rewardType)
        {
            case RewardType.HealAmount:
                ApplyHealAmountBuff();
                break;

            case RewardType.Damage:
                ApplyDamageBuff(player);
                break;

            case RewardType.Speed:
                ApplySpeedBuff(player);
                break;

            case RewardType.Gold:
                ApplyGoldBuff();
                break;
        }

        Debug.Log($"Nhat duoc {rewardType}! Gia tri: +{rewardValue}");
    }

    // ====================
    // CAC LOAI BUFF
    // ====================

    void ApplyHealAmountBuff()
    {
        if (SoulManager.Instance != null)
        {
            SoulManager.Instance.healAmount += rewardValue;
            Debug.Log($"Heal Amount tang len {SoulManager.Instance.healAmount}");
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPurchaseSound();
        }
        else
        {
            Debug.LogWarning("SoulManager khong ton tai!");
        }
    }

    void ApplyDamageBuff(GameObject player)
    {
        SoldierController soldierController = player.GetComponent<SoldierController>();
        if (soldierController != null)
        {
            soldierController.attackDamage += rewardValue;
            Debug.Log($"Attack Damage tang len {soldierController.attackDamage}");
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPurchaseSound();
        }
        else
        {
            Debug.LogWarning("SoldierController khong ton tai tren player!");
        }
    }

    void ApplySpeedBuff(GameObject player)
    {
        SoldierController soldierController = player.GetComponent<SoldierController>();
        if (soldierController != null)
        {
            soldierController.moveSpeed += rewardValue;
            Debug.Log($"Move Speed tang len {soldierController.moveSpeed}");
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPurchaseSound();
        }
        else
        {
            Debug.LogWarning("SoldierController khong ton tai tren player!");
        }
    }

    void ApplyGoldBuff()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.AddGold(rewardValue);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPurchaseSound();
        }
        else
        {
            Debug.LogWarning("CurrencyManager khong ton tai!");
        }
    }



    // ====================
    // GIZMOS (DE HIEN THI TRONG EDITOR)
    // ====================

    void OnDrawGizmos()
    {
        // Hien thi pham vi trigger
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }
}
