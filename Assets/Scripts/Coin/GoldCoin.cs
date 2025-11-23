using UnityEngine;

public class GoldCoin : MonoBehaviour
{
    [Header("Coin Settings")]
    public int goldValue = 1;       // So vang khi nhat
    public string playerTag = "Player";  // Tag cua nguoi choi

    void OnTriggerEnter2D(Collider2D other)
    {
        // Kiem tra neu Player cham vao coin
        if (other.CompareTag(playerTag))
        {
            CollectCoin(other.gameObject);
        }
    }

    void CollectCoin(GameObject player)
    {
        // Them vang truc tiep vao CurrencyManager
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.AddGold(goldValue);
            Debug.Log("Nhat duoc " + goldValue + " vang!");
        }

        // Xoa coin
        Destroy(gameObject);
    }
}
