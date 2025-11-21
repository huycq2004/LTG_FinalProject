using UnityEngine;

public class GoldCoin : MonoBehaviour
{
    [Header("Coin Settings")]
    public int goldValue = 10;       // So vang khi nhat
    public string playerTag = "Player";  // Tag cua nguoi choi
    
    [Header("Effects")]
    public GameObject collectEffect;  // Hieu ung khi nhat (optional)
    public AudioClip collectSound;    // Am thanh khi nhat (optional)

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

        // Tao hieu ung collect neu co
        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }

        // Phat am thanh neu co
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }

        // Xoa coin
        Destroy(gameObject);
    }
}
