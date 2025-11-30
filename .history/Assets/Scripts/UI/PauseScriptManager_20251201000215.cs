using UnityEngine;

public class PauseScriptManager : MonoBehaviour
{
    [Header("Menu Canvas")]
    [SerializeField] private GameObject menuCanvas;

    private bool isPaused = false;

    void Start()
    {
        // Đảm bảo MenuCanvas bị ẩn khi bắt đầu game
        if (menuCanvas != null)
        {
            menuCanvas.SetActive(false);
        }
    }

    void Update()
    {
        // Kiểm tra khi nhấn phím ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // Dừng thời gian trong game
        
        if (menuCanvas != null)
        {
            menuCanvas.SetActive(true);
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // Tiếp tục thời gian trong game
        
        if (menuCanvas != null)
        {
            menuCanvas.SetActive(false);
        }
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}
