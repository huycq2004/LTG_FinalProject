using UnityEngine;
using UnityEngine.InputSystem;

public class SoldierController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float jumpForce = 16f;
    public float dashForce = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    [Header("Attack Settings")]
    public float attackDuration = 0.3f;
    public float attackRadius = 0.5f;
    public int attackDamage = 1;

    [Header("Health Settings")]
    public int maxHealth = 5;
    public HealthBarUI healthBarUI;  // Reference đến UI thanh máu

    [Header("Shop Settings")]
    public ShopPanel shopPanel;  // Reference đến ShopPanel

    [Header("Knockback Settings")]
    public float knockbackForce = 15f;
    public float knockbackDuration = 0.15f;
    public float hurtDuration = 0.2f;

    [Header("Invincibility Settings")]
    public float invincibilityDuration = 0.5f;

    [Header("Ground Settings")]
    public string groundTag = "Ground";
    public string enemyTag = "Enemy";

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator animator;
    private Collider2D soldierCollider;

    // Các trạng thái
    private bool isDashing;
    private bool isAttacking;
    private bool isJumping;
    private bool isInKnockback;
    private bool isGrounded;
    private bool isHurting;
    private bool isInvincible;
    private bool isDeath;

    // Bộ đếm thời gian
    private float dashTimeLeft;
    private float dashCooldownTimer;
    private float attackTimer;
    private float knockbackTimer;
    private float hurtTimer;
    private float invincibilityTimer;

    private Vector2 moveInput;
    private int attackIndex;
    private int jumpCount;
    private bool hasDealtDamageThisAttack;

    // Máu
    private int currentHealth;

    // Luu toc do goc de tinh toan upgrade
    private float baseSpeed;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        soldierCollider = GetComponent<Collider2D>();

        // Khởi tạo máu
        currentHealth = maxHealth;

        // Luu toc do goc
        baseSpeed = moveSpeed;

        // Cập nhật UI thanh máu ban đầu
        if (healthBarUI != null)
        {
            healthBarUI.ResetHealthBar(maxHealth);
        }
    }

    void Update()
    {
        // Nếu chết thì không làm gì
        if (isDeath) return;

        // Xử lý input
        HandleInput();
        
        // Cập nhật các bộ đếm thời gian
        UpdateTimers();
        
        // Xoay sprite theo hướng di chuyển
        if (moveInput.x > 0) sr.flipX = false;
        else if (moveInput.x < 0) sr.flipX = true;

        // Cập nhật animation
        UpdateAnimation();
    }

    void FixedUpdate()
    {
        // Nếu chết thì không di chuyển
        if (isDeath) return;

        // Nếu không đang dash hoặc knockback thì di chuyển bình thường
        if (!isDashing && !isInKnockback)
            rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
    }

    void HandleInput()
    {
        // Nếu shop đang mở thì không xử lý input di chuyển/tấn công
        if (shopPanel != null && shopPanel.IsOpen())
        {
            moveInput = Vector2.zero;
            return;
        }

        // Đọc input di chuyển từ bàn phím A/D
        moveInput = Vector2.zero;
        if (Keyboard.current.aKey.isPressed) moveInput.x = -1;  // A = di chuyển trái
        else if (Keyboard.current.dKey.isPressed) moveInput.x = 1;  // D = di chuyển phải

        // NHẢY - G key (có thể nhảy 2 lần - double jump)
        if (Keyboard.current.gKey.wasPressedThisFrame && (isGrounded || jumpCount < 2))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpCount++;
            isGrounded = false;
            isJumping = true;
            // Bất khả xâm phạm khi nhảy - không bị đánh trong lúc nhảy
            isInvincible = true;
            invincibilityTimer = invincibilityDuration;
            EnablePhaseThrough();  // Xuyên qua Enemy khi nhảy
        }

        // TẤN CÔNG - F key
        if (Keyboard.current.fKey.wasPressedThisFrame && !isAttacking)
            StartAttack();

        // DASH - H key (có cooldown và không thể dash khi đang dash)
        if (Keyboard.current.hKey.wasPressedThisFrame && !isDashing && dashCooldownTimer <= 0)
            StartDash();
    }

    void UpdateTimers()
    {
        // Giảm cooldown dash - khi hết cooldown có thể dash lại
        if (dashCooldownTimer > 0)
            dashCooldownTimer -= Time.deltaTime;

        // Giảm thời gian knockback - khi hết knockback di chuyển bình thường
        if (isInKnockback)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0)
                isInKnockback = false;
        }

        // Giảm thời gian bị đánh (hurt state) - dùng cho animation bị đánh
        if (isHurting)
        {
            hurtTimer -= Time.deltaTime;
            if (hurtTimer <= 0)
                isHurting = false;
        }

        // Giảm thời gian bất khả xâm phạm - khi hết có thể bị đánh
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0)
                isInvincible = false;
        }

        // Dừng nhảy khi chạm đất
        if (isGrounded && isJumping)
        {
            isJumping = false;
            DisablePhaseThrough();  // Bật lại va chạm với Enemy
        }

        // Giảm thời gian dash - khi hết kết thúc dash
        if (isDashing)
        {
            dashTimeLeft -= Time.deltaTime;
            if (dashTimeLeft <= 0)
                StopDash();
        }
    }

    void StartDash()
    {
        // Bắt đầu trạng thái dash
        isDashing = true;
        dashTimeLeft = dashDuration;
        dashCooldownTimer = dashCooldown;  // Bắt đầu cooldown
        
        // Tính hướng dash (1 = phải, -1 = trái) từ hướng sprite
        float dashDir = sr.flipX ? -1 : 1;
        
        // Áp dụng lực dash
        rb.linearVelocity = new Vector2(dashDir * dashForce, 0);
        rb.gravityScale = 0;  // Tắt trọng lực khi dash

        // Bất khả xâm phạm khi dash - không bị đánh trong lúc dash
        isInvincible = true;
        invincibilityTimer = dashDuration;

        EnablePhaseThrough();  // Xuyên qua Enemy khi dash
    }

    void StopDash()
    {
        // Kết thúc dash
        isDashing = false;
        rb.gravityScale = 3;  // Bật lại trọng lực

        // Bật lại collision an toàn (kiểm tra không nằm trong Enemy)
        DisablePhaseThrough();
    }

    void StartAttack()
    {
        // Bắt đầu trạng thái tấn công
        isAttacking = true;
        attackTimer = attackDuration;  // Timer để kiểm soát thời gian animation
        
        // Lặp giữa 2 animation tấn công (0 và 1)
        attackIndex = (attackIndex + 1) % 2;
        animator.SetInteger("attackIndex", attackIndex);
        animator.SetBool("isAttacking", true);
        hasDealtDamageThisAttack = false;  // Reset để tấn công mới gây sát thương
    }

    // Nhận sát thương từ Enemy - bị knockback
    public void TakeDamage(int damage, Vector2 knockbackDirection)
    {
        // Nếu đang bất khả xâm phạm (nhảy/dash) thì bỏ qua sát thương
        if (isInvincible)
        {
            Debug.Log("Player đang bất khả xâm phạm!");
            return;
        }

        // Nếu đã chết thì không chịu sát thương thêm
        if (isDeath)
        {
            return;
        }

        Debug.Log("Soldier nhận " + damage + " sát thương");

        // Trừ máu
        currentHealth -= damage;
        Debug.Log("Soldier còn " + currentHealth + " máu");

        // Cập nhật UI thanh máu
        if (healthBarUI != null)
        {
            healthBarUI.UpdateHealthBar(currentHealth, maxHealth);
        }

        // Kiểm tra nếu máu <= 0 thì chết
        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        // Bắt đầu knockback - áp dụng lực đẩy
        isInKnockback = true;
        knockbackTimer = knockbackDuration;
        
        // Đặt trạng thái bị đánh để hiển thị animation
        isHurting = true;
        hurtTimer = hurtDuration;
        
        // Chuẩn hóa và áp dụng hướng knockback
        knockbackDirection.Normalize();
        rb.linearVelocity = new Vector2(knockbackDirection.x * knockbackForce, rb.linearVelocity.y);
    }

    void Die()
    {
        // Đặt trạng thái chết
        isDeath = true;
        isHurting = true;

        Debug.Log("Soldier chết!");

        // Dừng di chuyển ngay lập tức
        rb.linearVelocity = Vector2.zero;

        // Set animation isDeath cho Animator (nếu có)
        if (animator != null)
        {
            animator.SetBool("isDeath", true);
        }

        // Có thể thêm logic respawn hoặc game over ở đây
        // Ví dụ: Invoke("Respawn", 2f);
    }

    /// Hồi máu cho Soldier
    public void Heal(int amount)
    {
        if (isDeath) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log("Soldier hồi " + amount + " máu. Máu hiện tại: " + currentHealth);

        // Cập nhật UI thanh máu
        if (healthBarUI != null)
        {
            healthBarUI.UpdateHealthBar(currentHealth, maxHealth);
        }
    }

    // Tang mau toi da
    public void IncreaseMaxHealth(int amount)
    {
        if (isDeath) return;

        maxHealth += amount;
        currentHealth += amount;  // Cung tang mau hien tai
        
        Debug.Log("Tang mau toi da: +" + amount + " | Mau toi da moi: " + maxHealth);

        // Cap nhat UI
        if (healthBarUI != null)
        {
            healthBarUI.UpdateHealthBar(currentHealth, maxHealth);
        }
    }

    // Tang sat thuong
    public void IncreaseDamage(int amount)
    {
        if (isDeath) return;

        attackDamage += amount;
        Debug.Log("Tang sat thuong: +" + amount + " | Sat thuong moi: " + attackDamage);
    }

    // Tang toc do
    public void IncreaseSpeed(float amount)
    {
        if (isDeath) return;

        moveSpeed += amount;
        Debug.Log("Tang toc do: +" + amount + " | Toc do moi: " + moveSpeed);
    }

    // Nhat vang khi cham vao coin
    public void CollectGold(int amount)
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.AddGold(amount);
        }
    }

    void EnablePhaseThrough()
    {
        // Tắt va chạm với Enemy layer để có thể xuyên qua khi dash/nhảy
        Collider2D[] enemyColliders = FindObjectsByType<Collider2D>(FindObjectsSortMode.None);
        foreach (Collider2D enemyCollider in enemyColliders)
        {
            if (enemyCollider.CompareTag(enemyTag) && soldierCollider != null)
                Physics2D.IgnoreCollision(soldierCollider, enemyCollider, true);
        }
    }

    void DisablePhaseThrough()
    {
        // Bật lại va chạm với Enemy layer - trở về bình thường
        Collider2D[] enemyColliders = FindObjectsByType<Collider2D>(FindObjectsSortMode.None);
        foreach (Collider2D enemyCollider in enemyColliders)
        {
            if (enemyCollider.CompareTag(enemyTag) && soldierCollider != null)
                Physics2D.IgnoreCollision(soldierCollider, enemyCollider, false);
        }
    }

    void UpdateAnimation()
    {
        // Nếu không có Animator thì bỏ qua
        if (animator == null) return;

        // Cập nhật animation tấn công
        if (isAttacking)
        {
            // Giảm timer tấn công
            attackTimer -= Time.deltaTime;
            
            // Gây sát thương ở giữa animation (chỉ 1 lần duy nhất)
            if (!hasDealtDamageThisAttack && attackTimer <= attackDuration / 2f)
            {
                DealDamage();
                hasDealtDamageThisAttack = true;
            }
            
            // Khi animation kết thúc
            if (attackTimer <= 0)
            {
                isAttacking = false;
                animator.SetBool("isAttacking", false);
            }
        }

        // Cập nhật animation di chuyển - đúng khi di chuyển ngang và không đang tấn công
        bool isWalking = !isAttacking && moveInput.x != 0;
        animator.SetBool("isWalking", isWalking);
        
        // Cập nhật animation bị đánh
        animator.SetBool("isHurting", isHurting);
    }

    void DealDamage()
    {
        // Tìm tất cả Enemy trong phạm vi tấn công
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRadius, LayerMask.GetMask("Enemy"));
        
        // Gây sát thương cho mỗi Enemy bị trúng
        foreach (Collider2D hitEnemy in hitEnemies)
        {
            Debug.Log("Player đánh trúng Enemy: " + hitEnemy.name);
            
            // Lấy OrcController và gọi TakeDamage
            OrcController orcController = hitEnemy.GetComponent<OrcController>();
            if (orcController != null)
            {
                orcController.TakeDamage(attackDamage);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(groundTag))
        {
            isGrounded = true;
            jumpCount = 0;
        }
    }
}
