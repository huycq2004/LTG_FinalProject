using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;


/// Soldier Controller - Dieu khien nhan vat nguoi choi
/// Gom co: Di chuyen, Nhay, Dash (iframe), Tan cong, Nhan sat thuong

public class SoldierController : MonoBehaviour
{
    // ====================
    // CAI DAT CO BAN
    // ====================

    [Header("Di Chuyen")]
    public float moveSpeed = 8f;
    public float jumpForce = 16f;
    public float dashForce = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    [Header("Tan Cong")]
    public float attackDuration = 0.3f;
    public float attackRadius = 0.5f;
    public int attackDamage = 1;

    [Header("Mau")]
    public int maxHealth = 5;
    public HealthBarUI healthBarUI;

    [Header("Shop")]
    public ShopPanel shopPanel;

    [Header("Knockback")]
    public float knockbackForce = 15f;
    public float knockbackDuration = 0.15f;
    public float hurtDuration = 0.2f;

    [Header("Iframe (Bat Tu)")]
    public float dashIframeDuration = 0.3f;   // Iframe khi dash

    [Header("Tag")]
    public string groundTag = "Ground";
    public string enemyTag = "Enemy";
    public string trapTag = "Trap";

    // ====================
    // THANH PHAN
    // ====================

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator animator;
    private Collider2D soldierCollider;

    // ====================
    // TRANG THAI
    // ====================

    private bool isDashing;
    private bool isAttacking;
    private bool isInKnockback;
    private bool isGrounded;
    private bool isHurting;
    private bool isInvincible;
    private bool isDeath;

    // ====================
    // BO DEM THOI GIAN
    // ====================

    private float dashTimeLeft;
    private float dashCooldownTimer;
    private float attackTimer;
    private float knockbackTimer;
    private float hurtTimer;
    private float invincibilityTimer;

    // ====================
    // BIEN KHAC
    // ====================

    private Vector2 moveInput;
    private int attackIndex;
    private int jumpCount;
    private bool hasDealtDamageThisAttack;
    private int currentHealth;
    private float baseSpeed;
    public float groundCheckDistance = 0.1f;
    public LayerMask groundLayer;
    public Transform groundCheckPoint;
    public bool isAbleToDoubleJump = false;

    // ====================
    // KHOI TAO
    // ====================
    void Start()
    {
        GetComponents();
        InitializeStats();
        UpdateHealthUI();

        // Phat nhac gameplay khi bat dau
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGameplayMusic();
        }
    }

    void GetComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        soldierCollider = GetComponent<Collider2D>();
    }

    void InitializeStats()
    {
        currentHealth = maxHealth;
        baseSpeed = moveSpeed;
    }

    void UpdateHealthUI()
    {
        if (healthBarUI != null)
        {
            healthBarUI.ResetHealthBar(maxHealth);
        }
    }

    // ====================
    // CAP NHAT CHINH
    // ====================

    void Update()
    {
        if (IsDead()) return;

        HandleInput();
        UpdateAllTimers();
        UpdateFacingDirection();
        UpdateAnimations();
        UpdateIsGrounded();
    }

    void FixedUpdate()
    {
        if (IsDead()) return;

        ApplyMovement();
    }

    // ====================
    // KIEM TRA TRANG THAI
    // ====================

    bool IsDead()
    {
        return isDeath;
    }

    bool IsShopOpen()
    {
        return shopPanel != null && shopPanel.IsOpen();
    }

    bool CanMove()
    {
        return !isDashing && !isInKnockback;
    }

    bool CanJump()
    {   
        // Debug.Log("Player nhan nut nhay!, jumpCount: " + jumpCount + "isGrounded: " + isGrounded);
            
        //player on ground
        if(isGrounded == true) 
        {
            return true;
        }
        //double jump handling
        //player in air but hasn't jumped yet
        else if (isGrounded == false && jumpCount == 0 && isAbleToDoubleJump == true) 
        {
            //Count as player has jumped
            jumpCount++;
            return true;
        } 
        //player in air and has jumped once
        else if (isGrounded == false && jumpCount == 1 && isAbleToDoubleJump == true) 
        {
            return true;
        }
        return false;
    }

    bool CanDash()
    {
        return !isDashing && dashCooldownTimer <= 0;
    }

    bool CanAttack()
    {
        return !isAttacking;
    }

    // ====================
    // XU LY INPUT
    // ====================

    void HandleInput()
    {
        if (IsShopOpen())
        {
            moveInput = Vector2.zero;
            return;
        }

        ReadMovementInput();
        HandleJumpInput();
        HandleAttackInput();
        HandleDashInput();
    }

    void ReadMovementInput()
    {
        moveInput = Vector2.zero;
        if (Keyboard.current.aKey.isPressed) moveInput.x = -1;
        else if (Keyboard.current.dKey.isPressed) moveInput.x = 1;
    }

    void HandleJumpInput()
    {
        if (Keyboard.current.gKey.wasPressedThisFrame && CanJump())
        {
            Jump();
        }
    }

    void HandleAttackInput()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame && CanAttack())
        {
            StartAttack();
        }
    }

    void HandleDashInput()
    {
        if (Keyboard.current.hKey.wasPressedThisFrame && CanDash())
        {
            StartDash();
        }
    }

    // ====================
    // HANH DONG: NHAY
    // ====================

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        jumpCount++;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayJumpSound();
        }

        // Debug.Log("Player nhay!");
    }

    // ====================
    // HANH DONG: DASH (DODGE ROLL)
    // ====================

    void StartDash()
    {
        isDashing = true;
        dashTimeLeft = dashDuration;
        dashCooldownTimer = dashCooldown;

        float dashDirection = sr.flipX ? -1 : 1;
        rb.linearVelocity = new Vector2(dashDirection * dashForce, 0);
        rb.gravityScale = 0;

        // IFRAME KHI DASH - CHI CO BAT TU, KHONG XUYEN QUA
        isInvincible = true;
        invincibilityTimer = dashIframeDuration;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayDashSound();
        }

        Debug.Log("Player dash! Iframe trong " + dashIframeDuration + " giay");
    }

    void StopDash()
    {
        isDashing = false;
        rb.gravityScale = 3;
    }

    // ====================
    // HANH DONG: TAN CONG
    // ====================

    void StartAttack()
    {
        isAttacking = true;
        attackTimer = attackDuration;

        attackIndex = (attackIndex + 1) % 2;
        animator.SetInteger("attackIndex", attackIndex);
        animator.SetBool("isAttacking", true);

        hasDealtDamageThisAttack = false;
    }

    void ExecuteAttackDamage()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            transform.position,
            attackRadius,
            LayerMask.GetMask("Enemy")
        );

        foreach (Collider2D hitEnemy in hitEnemies)
        {
            DamageEnemy(hitEnemy);
        }
    }

    void DamageEnemy(Collider2D enemyCollider)
    {
        Debug.Log("Player danh trung: " + enemyCollider.name);

        // THEM DONG NAY: Thu thap Soul khi danh trung
        if (SoulManager.Instance != null)
        {
            SoulManager.Instance.AddSoul();
        }

        // Thu danh Boss
        BossController boss = enemyCollider.GetComponent<BossController>();
        if (boss != null)
        {
            boss.TakeDamage(attackDamage);
            return;
        }

        // Thu danh Orc
        OrcController orc = enemyCollider.GetComponent<OrcController>();
        if (orc != null)
        {
            orc.TakeDamage(attackDamage);
            return;
        }

        // Thu danh Golem
        GolemController golem = enemyCollider.GetComponent<GolemController>();
        if (golem != null)
        {
            golem.TakeDamage(attackDamage);
            return;
        }
    }

    // ====================
    // CAP NHAT BO DEM
    // ====================

    void UpdateAllTimers()
    {
        UpdateDashTimer();
        UpdateKnockbackTimer();
        UpdateHurtTimer();
        UpdateInvincibilityTimer();
        UpdateAttackTimer();
    }

    void UpdateDashTimer()
    {
        if (dashCooldownTimer > 0)
            dashCooldownTimer -= Time.deltaTime;

        if (isDashing)
        {
            dashTimeLeft -= Time.deltaTime;
            if (dashTimeLeft <= 0)
                StopDash();
        }
    }

    void UpdateKnockbackTimer()
    {
        if (isInKnockback)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0)
                isInKnockback = false;
        }
    }

    void UpdateHurtTimer()
    {
        if (isHurting)
        {
            hurtTimer -= Time.deltaTime;
            if (hurtTimer <= 0)
                isHurting = false;
        }
    }

    void UpdateInvincibilityTimer()
    {
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0)
            {
                isInvincible = false;
                invincibilityTimer = 0f;
            }
        }
    }

    void UpdateAttackTimer()
    {
        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;

            // Gay sat thuong o giua animation
            if (!hasDealtDamageThisAttack && attackTimer <= attackDuration / 2f)
            {
                ExecuteAttackDamage();
                hasDealtDamageThisAttack = true;
            }

            // Ket thuc tan cong
            if (attackTimer <= 0)
            {
                isAttacking = false;
                animator.SetBool("isAttacking", false);
            }
        }
    }
    void UpdateIsGrounded()
    {
        // Cast a ray down from the player's position
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance + 0.1f, groundLayer);
        // Debug.Log("Ground Check Raycast hit: " + (hit.collider != null ? hit.collider.name : "None"));
        if (hit.collider == null)
        {
            isGrounded = false;
        }
        else if (hit.collider.name == "Ground_1")
        {
            jumpCount = 0;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
        
        // Optional: debug
        // Debug.DrawRay(transform.position, Vector3.down * (groundCheckDistance + 0.1f), isGrounded ? Color.green : Color.red);
    }

    // ====================
    // NHAN SAT THUONG
    // ====================

    public void TakeDamage(int damage, Vector2 knockbackDirection)
    {
        // IFRAME - Bo qua sat thuong
        if (isInvincible)
        {
            Debug.Log("Player dang iframe! Bo qua sat thuong");
            return;
        }

        if (IsDead()) return;

        ApplyDamage(damage);
        ApplyKnockback(knockbackDirection);
        ShowHurtAnimation();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayHitSound();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void ApplyDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Player nhan " + damage + " sat thuong. Con " + currentHealth + " mau");

        if (healthBarUI != null)
        {
            healthBarUI.UpdateHealthBar(currentHealth, maxHealth);
        }
    }

    void ApplyKnockback(Vector2 direction)
    {
        isInKnockback = true;
        knockbackTimer = knockbackDuration;

        direction.Normalize();
        rb.linearVelocity = new Vector2(direction.x * knockbackForce, rb.linearVelocity.y);
    }

    void ShowHurtAnimation()
    {
        isHurting = true;
        hurtTimer = hurtDuration;
    }

    void Die()
    {
        isDeath = true;
        isHurting = true;
        rb.linearVelocity = Vector2.zero;

        Debug.Log("Player chet!");

        if (animator != null)
        {
            animator.SetBool("isDeath", true);
        }
    }

    // ====================
    // HOI PHUC & NANG CAP
    // ====================

    public void Heal(int amount)
    {
        if (IsDead()) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log("Hoi " + amount + " mau. Mau hien tai: " + currentHealth);

        if (healthBarUI != null)
        {
            healthBarUI.UpdateHealthBar(currentHealth, maxHealth);
        }
    }

    public void IncreaseMaxHealth(int amount)
    {
        if (IsDead()) return;

        maxHealth += amount;
        Debug.Log("Tang mau toi da: +" + amount);

        if (healthBarUI != null)
        {
            healthBarUI.UpdateHealthBar(currentHealth, maxHealth);
        }
    }

    public void IncreaseDamage(int amount)
    {
        if (IsDead()) return;

        attackDamage += amount;
        Debug.Log("Tang sat thuong: +" + amount);
    }

    public void IncreaseSpeed(float amount)
    {
        if (IsDead()) return;

        moveSpeed += amount;
        Debug.Log("Tang toc do: +" + amount);
    }

    public void CollectGold(int amount)
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.AddGold(amount);
        }
    }

    // ====================
    // GETTER
    // ====================

    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
    public int GetAttackDamage() => attackDamage;
    public float GetMoveSpeed() => moveSpeed;

    // ====================
    // DI CHUYEN & ANIMATION
    // ====================

    void ApplyMovement()
    {
        if (CanMove())
        {
            rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
        }
    }

    void UpdateFacingDirection()
    {
        if (moveInput.x > 0) sr.flipX = false;
        else if (moveInput.x < 0) sr.flipX = true;
    }

    void UpdateAnimations()
    {
        if (animator == null) return;

        bool isWalking = !isAttacking && moveInput.x != 0;
        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isHurting", isHurting);
    }

    // ====================
    // VA CHAM
    // ====================

    void OnCollisionEnter2D(Collision2D collision)
    {
        // if (collision.gameObject.CompareTag(groundTag))
        // {
        //     isGrounded = true;
        //     jumpCount = 0;
        // }

        if (collision.gameObject.CompareTag(trapTag))
        {
            Debug.Log("Player cham Trap! Chet ngay!");
            Die();
        }
    }

    // ====================
    // DEBUG
    // ====================

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}