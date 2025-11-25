using UnityEngine;
using UnityEngine.UI;

/// Boss Health Bar UI - Hien thi va quan ly thanh mau cua Boss
/// Chi hien khi player vao vung detection range cua Boss
public class BossHealthBarUI : MonoBehaviour
{
    // ====================
    // THONG SO CO BAN
    // ====================

    [Header("Tham Chieu UI")]
    public Image healthBarFill;

    [Header("Mau Sac Thanh Mau")]
    public Color fullHealthColor = new Color(1f, 0f, 0f);
    public Color lowHealthColor = new Color(0.5f, 0f, 0f);

    [Header("Toc Do")]
    public float smoothSpeed = 5f;
    public float fadeSpeed = 2f;

    [Header("Tham Chieu Boss")]
    public BossController bossController;

    // ====================
    // BIEN NOI BO
    // ====================

    private float targetFillAmount = 1f;
    private float targetAlpha = 0f;
    private Transform playerTransform;
    private Transform bossTransform;
    private CanvasGroup canvasGroup;
    private bool hasBeenDiscovered;
    private float detectionRange;

    // ====================
    // KHOI TAO
    // ====================

    void Start()
    {
        InitializeHealthBar();
        InitializeCanvasGroup();
        FindPlayer();
        FindBoss();
    }

    void InitializeHealthBar()
    {
        if (healthBarFill != null)
            healthBarFill.fillAmount = 1f;
    }

    void InitializeCanvasGroup()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        
        canvasGroup.alpha = 0f;
    }

    void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerTransform = playerObj.transform;
    }

    void FindBoss()
    {
        // Uu tien: tham chieu truc tiep
        if (bossController == null)
        {
            // Tim tu parent
            if (transform.parent != null)
                bossController = transform.parent.GetComponent<BossController>();
        }

        if (bossController != null)
        {
            bossTransform = bossController.transform;
            detectionRange = bossController.detectionRange;
        }
    }

    // ====================
    // CAP NHAT
    // ====================

    void Update()
    {
        // Tim lai neu mat
        if (playerTransform == null)
        {
            FindPlayer();
            return;
        }

        if (bossTransform == null)
        {
            FindBoss();
            return;
        }

        CheckPlayerDistance();
        UpdateHealthBar();
        UpdateAlpha();
    }

    // ====================
    // KIEM TRA KHOANG CACH
    // ====================

    void CheckPlayerDistance()
    {
        float distance = Vector2.Distance(bossTransform.position, playerTransform.position);
        bool isInRange = distance <= detectionRange;

        if (isInRange)
        {
            if (!hasBeenDiscovered)
                hasBeenDiscovered = true;
            targetAlpha = 1f;
        }
        else
        {
            targetAlpha = 0f;
        }
    }

    // ====================
    // CAP NHAT THANH MAU
    // ====================

    void UpdateHealthBar()
    {
        if (healthBarFill == null) return;

        if (Mathf.Abs(healthBarFill.fillAmount - targetFillAmount) > 0.001f)
        {
            healthBarFill.fillAmount = Mathf.Lerp(
                healthBarFill.fillAmount, 
                targetFillAmount, 
                Time.deltaTime * smoothSpeed
            );
            
            healthBarFill.color = Color.Lerp(
                lowHealthColor, 
                fullHealthColor, 
                healthBarFill.fillAmount
            );
        }
    }

    void UpdateAlpha()
    {
        if (canvasGroup == null) return;

        if (Mathf.Abs(canvasGroup.alpha - targetAlpha) > 0.001f)
        {
            canvasGroup.alpha = Mathf.Lerp(
                canvasGroup.alpha, 
                targetAlpha, 
                Time.deltaTime * fadeSpeed
            );
        }
    }

    // ====================
    // GIAO DIEN CONG KHAI
    // ====================

    public void UpdateHealth(int currentHealth, int maxHealth)
    {
        if (healthBarFill == null || maxHealth <= 0) return;

        targetFillAmount = (float)currentHealth / maxHealth;

        if (currentHealth < maxHealth && hasBeenDiscovered)
        {
            float distance = Vector2.Distance(bossTransform.position, playerTransform.position);
            if (distance <= detectionRange)
                targetAlpha = 1f;
        }
    }

    public void ResetHealthBar(int maxHealth)
    {
        targetFillAmount = 1f;
        if (healthBarFill != null)
            healthBarFill.fillAmount = 1f;
    }

    public void ForceShow()
    {
        hasBeenDiscovered = true;
        targetAlpha = 1f;
    }

    public void ForceHide()
    {
        hasBeenDiscovered = false;
        targetAlpha = 0f;
    }
}