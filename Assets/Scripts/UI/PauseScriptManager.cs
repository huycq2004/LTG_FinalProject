using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;              // panel toàn bộ menu pause

    public GameObject cursorContinue;          // con trỏ cố định của CONTINUE
    public GameObject cursorReturn;            // con trỏ cố định của RETURN TO MAIN MENU

    public TextMeshProUGUI[] menuTexts;        // 0 = Continue, 1 = Return to Main Menu

    public float normalScale = 1f;
    public float selectedScale = 1.3f;

    private int currentIndex = 0;
    private bool isPaused = false;

    void Start()
    {
        pausePanel.SetActive(false); // menu ẩn lúc đầu
        UpdateCursorVisibility();
        UpdateTextScaling();
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        // Nhấn Backspace để bật/tắt pause menu
        if (kb.backspaceKey.wasPressedThisFrame)
        {
            TogglePauseMenu();
        }

        if (!isPaused) return; // Nếu game chưa pause thì không xử lý menu

        // DI CHUYỂN XUỐNG
        if (kb.downArrowKey.wasPressedThisFrame || kb.sKey.wasPressedThisFrame)
        {
            currentIndex++;
            if (currentIndex > 1) currentIndex = 0;
            UpdateCursorVisibility();
            UpdateTextScaling();
        }

        // DI CHUYỂN LÊN
        if (kb.upArrowKey.wasPressedThisFrame || kb.wKey.wasPressedThisFrame)
        {
            currentIndex--;
            if (currentIndex < 0) currentIndex = 1;
            UpdateCursorVisibility();
            UpdateTextScaling();
        }

        // ENTER
        if (kb.enterKey.wasPressedThisFrame || kb.numpadEnterKey.wasPressedThisFrame)
        {
            SelectItem();
        }
    }

    // Bật / tắt menu pause
    void TogglePauseMenu()
    {
        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);

        Time.timeScale = isPaused ? 0 : 1;

        UpdateCursorVisibility();
        UpdateTextScaling();
    }

    // ẨN / HIỆN con trỏ
    void UpdateCursorVisibility()
    {
        cursorContinue.SetActive(currentIndex == 0);
        cursorReturn.SetActive(currentIndex == 1);
    }

    // Phóng to chữ
    void UpdateTextScaling()
    {
        for (int i = 0; i < menuTexts.Length; i++)
        {
            if (i == currentIndex)
                menuTexts[i].transform.localScale = Vector3.one * selectedScale;
            else
                menuTexts[i].transform.localScale = Vector3.one * normalScale;
        }
    }

    // Chọn mục menu
    void SelectItem()
    {
        if (currentIndex == 0)
        {
            // Continue
            TogglePauseMenu();
        }
        else if (currentIndex == 1)
        {
            // Return to main menu
            Time.timeScale = 1;
            SceneManager.LoadScene("START");
        }
    }
}
