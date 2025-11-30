using UnityEngine;
using UnityEngine.UI;

public class SelectorBlink : MonoBehaviour
{
    private Image img;      // Image cần blink
    public float speed = 4f;

    void Awake()
    {
        img = GetComponent<Image>();
        if (img == null)
            Debug.LogError("Không tìm thấy Image trên object Selector!");
    }

    void Update()
    {
        float a = (Mathf.Sin(Time.time * speed) + 1) / 2f;  // 0 → 1
        Color c = img.color;
        c.a = a;
        img.color = c;
    }
}
