using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("UI References")]
    public Image healthBarFill;  // Thanh mau UI

    [Header("Health Bar Settings")]
    public Color fullHealthColor = new Color(1f, 0f, 0f);  // Mau do khi mau day
    public Color lowHealthColor = new Color(0.5f, 0f, 0f);  // Mau do dam khi mau thap
    public float smoothSpeed = 5f;  // Toc do chuyen doi

    private float targetFillAmount;
    private int currentHearts;

    void Start()
    {
        // Khoi tao thanh mau day
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = 1f;
            targetFillAmount = 1f;
        }
    }

    void Update()
    {
        // Cap nhat thanh mau theo frame
        if (healthBarFill != null && Mathf.Abs(healthBarFill.fillAmount - targetFillAmount) > 0.001f)
        {
            // Su dung Lerp de tao hieu ung chuyen doi mem
            healthBarFill.fillAmount = Mathf.Lerp(healthBarFill.fillAmount, targetFillAmount, Time.deltaTime * smoothSpeed);
            
            // Thay doi mau thanh mau dua tren fillAmount hien tai
            healthBarFill.color = Color.Lerp(lowHealthColor, fullHealthColor, healthBarFill.fillAmount);
        }
    }

    // Cap nhat thanh mau dua tren mau hien tai va mau toi da
    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (healthBarFill != null && maxHealth > 0)
        {
            // Tinh ti le mau (0-1)
            targetFillAmount = (float)currentHealth / maxHealth;
        }
    }

    // Dat lai thanh mau ve trang thai day
    public void ResetHealthBar(int maxHealth)
    {
        UpdateHealthBar(maxHealth, maxHealth);
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = 1f;
            targetFillAmount = 1f;
        }
    }
}
