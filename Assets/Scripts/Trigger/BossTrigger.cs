using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class BossTrigger : MonoBehaviour
{
    [Header("Settings")]
    public string playerTag = "Player";
    private bool playerInTrigger = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInTrigger = true;
            Debug.Log("Bạn đã đến cổng Boss! Nhấn Enter để vào.");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInTrigger = false;
        }
    }

    void Update()
    {
        if (playerInTrigger && Keyboard.current.enterKey.wasPressedThisFrame)
        {
            CheckAndLoadBossScene();
        }
    }

    void CheckAndLoadBossScene()
    {
        // Kiểm tra kill count đủ hay chưa
        if (EnemyCountManager.Instance.IsTargetReached())
        {
            Debug.Log("Đủ kill rồi! Vào BOSS SCENE!");
            SceneManager.LoadScene("BOSS SCENE");
        }
        else
        {
            int remaining = EnemyCountManager.Instance.GetRemainingCount();
            Debug.Log($"Chưa đủ kill! Còn cần {remaining} kill nữa.");
        }
    }
}