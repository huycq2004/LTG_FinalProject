using UnityEngine;
using UnityEngine.SceneManagement;


/// BossSceneTrigger - Trigger chuyen sang Boss Scene
/// Gan script nay vao GameObject co Collider2D (Is Trigger)

public class BossSceneTrigger : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Ten scene Boss (phai trung voi ten trong Build Settings)")]
    public string bossSceneName = "BossScene";

    [Header("Player Tag")]
    [Tooltip("Tag cua nguoi choi")]
    public string playerTag = "Player";

    void OnTriggerEnter2D(Collider2D other)
    {
        // Kiem tra neu la Player va cham vao trigger
        if (other.CompareTag(playerTag))
        {
            Debug.Log("Player vao Boss Trigger! Chuyen sang Boss Scene...");
            LoadBossScene();
        }
    }

    void LoadBossScene()
    {
        // Chuyen sang Boss Scene
        Debug.Log("Chuyen sang scene: " + bossSceneName);
        SceneManager.LoadScene(bossSceneName);
    }

    // Hien thi vung trigger trong Scene view (mau do)
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            Gizmos.DrawWireCube(transform.position + (Vector3)boxCollider.offset, boxCollider.size);
        }
    }
}
