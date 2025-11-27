using UnityEngine;
using UnityEngine.UI;

/// Soul UI - Hien thi so soul da thu thap tren Canvas
/// Hien thi tung frame soul khi danh trung ke dich
public class SoulUI : MonoBehaviour
{
    [Header("UI References")]
    public Image[] soulImages;  // Mang 5 Image de hien thi soul frames

    [Header("Soul Sprite")]
    public Sprite soulSprite;   // Sprite cua soul frame

    [Header("Display Settings")]
    public Color activeColor = Color.white;     // Mau khi co soul
    public Color inactiveColor = new Color(1f, 1f, 1f, 0.3f);  // Mau khi chua co soul (transparent)

    // ====================
    // KHOI TAO
    // ====================

    void Start()
    {
        ValidateComponents();
        SetupSoulImages();
        SubscribeToEvents();
        ForceUpdateDisplay();
    }

    void ValidateComponents()
    {
        if (soulImages == null || soulImages.Length == 0)
        {
            Debug.LogError("Soul Images chua duoc gan trong Inspector!");
        }

        if (soulSprite == null)
        {
            Debug.LogWarning("Soul Sprite chua duoc gan!");
        }
    }

    void SetupSoulImages()
    {
        // Gan sprite cho tat ca cac image
        foreach (Image img in soulImages)
        {
            if (img != null && soulSprite != null)
            {
                img.sprite = soulSprite;
                img.color = inactiveColor;  // Mac dinh la inactive
            }
        }
    }

    void SubscribeToEvents()
    {
        if (SoulManager.Instance != null)
        {
            SoulManager.Instance.OnSoulCountChanged += UpdateDisplay;
            SoulManager.Instance.OnSoulFull += OnSoulFull;
            SoulManager.Instance.OnSoulUsed += OnSoulUsed;
        }
        else
        {
            Debug.LogWarning("SoulManager Instance khong ton tai!");
        }
    }

    void ForceUpdateDisplay()
    {
        if (SoulManager.Instance != null)
        {
            int current = SoulManager.Instance.GetCurrentSoulCount();
            int max = SoulManager.Instance.GetMaxSoulCount();
            UpdateDisplay(current, max);
        }
    }

    void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    void UnsubscribeFromEvents()
    {
        if (SoulManager.Instance != null)
        {
            SoulManager.Instance.OnSoulCountChanged -= UpdateDisplay;
            SoulManager.Instance.OnSoulFull -= OnSoulFull;
            SoulManager.Instance.OnSoulUsed -= OnSoulUsed;
        }
    }

    // ====================
    // CAP NHAT UI
    // ====================

    void UpdateDisplay(int currentCount, int maxCount)
    {
        if (soulImages == null) return;

        // Cap nhat mau cho tung soul image
        for (int i = 0; i < soulImages.Length; i++)
        {
            if (soulImages[i] != null)
            {
                // Neu index nho hon currentCount thi hien thi active
                if (i < currentCount)
                {
                    soulImages[i].color = activeColor;
                }
                else
                {
                    soulImages[i].color = inactiveColor;
                }
            }
        }

        Debug.Log(string.Format("Soul UI: {0}/{1}", currentCount, maxCount));
    }

    void OnSoulFull()
    {
        Debug.Log("Soul UI: Day du!");
        // Co the them hieu ung o day (animation, particle, flash, etc.)
    }

    void OnSoulUsed()
    {
        Debug.Log("Soul UI: Da su dung soul de hoi mau!");
        // Co the them hieu ung o day
    }
}
