using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public RectTransform selectorStart;   // con trỏ trước START
    public RectTransform selectorExit;    // con trỏ trước EXIT

    public RectTransform[] menuItems;      // 0 = START, 1 = EXIT
    public TextMeshProUGUI[] menuTexts;    // Text của menu

    public float moveSpeed = 10f;
    public float normalScale = 1f;
    public float selectedScale = 1.3f;

    int currentIndex = 0;

    void Start()
    {
        UpdateCursorVisibility();
        UpdateCursorPosition();
        UpdateTextScaling();
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        // DI XUỐNG
        if (kb.downArrowKey.wasPressedThisFrame || kb.sKey.wasPressedThisFrame)
        {
            currentIndex++;
            if (currentIndex > menuItems.Length - 1) currentIndex = 0;

            UpdateCursorVisibility();
            UpdateCursorPosition();
            UpdateTextScaling();
        }

        // DI LÊN
        if (kb.upArrowKey.wasPressedThisFrame || kb.wKey.wasPressedThisFrame)
        {
            currentIndex--;
            if (currentIndex < 0) currentIndex = menuItems.Length - 1;

            UpdateCursorVisibility();
            UpdateCursorPosition();
            UpdateTextScaling();
        }

        // ENTER
        if (kb.enterKey.wasPressedThisFrame || kb.numpadEnterKey.wasPressedThisFrame)
        {
            SelectItem();
        }
    }

    // ẨN/HIỆN 2 CON TRỎ
    void UpdateCursorVisibility()
    {
        selectorStart.gameObject.SetActive(currentIndex == 0);
        selectorExit.gameObject.SetActive(currentIndex == 1);
    }

    // CẬP NHẬT VỊ TRÍ CỦA CON TRỎ ĐANG HIỆN
    void UpdateCursorPosition()
    {
        float offsetX = -80f;

        Vector2 targetPos = new Vector2(
            menuItems[currentIndex].anchoredPosition.x + offsetX,
            menuItems[currentIndex].anchoredPosition.y
        );

        if (currentIndex == 0)
        {
            selectorStart.anchoredPosition = Vector2.Lerp(
                selectorStart.anchoredPosition,
                targetPos,
                moveSpeed * Time.deltaTime
            );
        }
        else
        {
            selectorExit.anchoredPosition = Vector2.Lerp(
                selectorExit.anchoredPosition,
                targetPos,
                moveSpeed * Time.deltaTime
            );
        }
    }

    // PHÓNG TO CHỮ
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

    // NHẤN ENTER
    void SelectItem()
    {
        switch (currentIndex)
        {
            case 0:
                Debug.Log("START GAME");
                SceneManager.LoadScene("OPEN SCENE");
                break;

            case 1:
                Debug.Log("EXIT GAME");
                Application.Quit();
                break;
        }
    }
}
