using UnityEngine;

/// Golem Controller - Dieu khien ke thu Golem
/// Gom co: Di chuyen tuan tra, Duoi theo nguoi choi, Tan cong, Nhan sat thuong

[RequireComponent(typeof(Rigidbody2D))]
public class GolemController : MonoBehaviour
{
    // ====================
    // CAI DAT CO BAN
    // ====================

    [Header("Di Chuyen")]
    public float moveSpeed = 2f;
    public float chaseSpeed = 4f;
    public float patrolDistance = 5f;

    [Header("Tan Cong")]
    public float attackRange = 1.2f;
    public float attackDuration = 0.5f;
    public float idleAfterAttackDuration = 0.3f;
    public float attackRadius = 0.8f;
    public int attackDamage = 2;

    [Header("Mau")]
    public int maxHealth = 8;
    public float hurtDuration = 0.2f;

    [Header("Roi Do")]
    public GameObject goldCoinPrefab;
    public int goldDropAmount = 2;

    // ====================
    // THANH PHAN
    // ====================

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator animator;
    private Collider2D golemCollider;

    // ====================
    // TRANG THAI
    // ====================

    private bool isAttacking;
    private bool isIdleAfterAttack;
    private bool isHurting;
    private bool isDeath;
    private bool moveRight;

    // ====================
    // BO DEM THOI GIAN
    // ====================

    private float attackDurationTimer;
    private float idleAfterAttackTimer;
    private float hurtTimer;

    // ====================
    // BIEN KHAC
    // ====================

    private Vector3 startPos;
    private GameObject player;
    private bool hasDealtDamageThisAttack;
    private int currentHealth;

    // ====================
    // KHOI TAO
    // ====================

    void Start()
    {
        GetComponents();
        InitializeStats();
        FindPlayer();
        DisableEnemyCollisions();
    }

    void GetComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        golemCollider = GetComponent<Collider2D>();
    }

    void InitializeStats()
    {
        rb.gravityScale = 3f;
        rb.freezeRotation = true;

        currentHealth = maxHealth;
        startPos = transform.position;
    }

    void FindPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void DisableEnemyCollisions()
    {
        DisableGolemCollisions();
        DisableOrcCollisions();
        DisablePlayerCollision();
    }

    void DisableGolemCollisions()
    {
        GolemController[] allGolems = FindObjectsByType<GolemController>(FindObjectsSortMode.None);
        foreach (GolemController otherGolem in allGolems)
        {
            if (otherGolem != this && golemCollider != null)
            {
                Collider2D otherCollider = otherGolem.GetComponent<Collider2D>();
                if (otherCollider != null)
                {
                    Physics2D.IgnoreCollision(golemCollider, otherCollider, true);
                }
            }
        }
    }

    void DisableOrcCollisions()
    {
        OrcController[] allOrcs = FindObjectsByType<OrcController>(FindObjectsSortMode.None);
        foreach (OrcController orc in allOrcs)
        {
            Collider2D orcCollider = orc.GetComponent<Collider2D>();
            if (orcCollider != null && golemCollider != null)
            {
                Physics2D.IgnoreCollision(golemCollider, orcCollider, true);
            }
        }
    }

    void DisablePlayerCollision()
    {
        if (player != null && golemCollider != null)
        {
            Collider2D playerCollider = player.GetComponent<Collider2D>();
            if (playerCollider != null)
            {
                Physics2D.IgnoreCollision(golemCollider, playerCollider, true);
                Debug.Log("Golem: Da tat va cham voi Player");
            }
        }
    }

    // ====================
    // CAP NHAT CHINH
    // ====================

    void FixedUpdate()
    {
        if (IsDead()) return;

        UpdateAllTimers();
        HandleBehavior();
        UpdateAnimation();
    }

    // ====================
    // KIEM TRA TRANG THAI
    // ====================

    bool IsDead()
    {
        return isDeath;
    }

    bool CanMove()
    {
        return !isAttacking && !isIdleAfterAttack;
    }

    bool CanAttack()
    {
        return !isAttacking;
    }

    bool IsPlayerInRange()
    {
        return player != null && Vector2.Distance(player.transform.position, transform.position) <= attackRange;
    }

    bool IsPlayerDetected()
    {
        return player != null && Vector2.Distance(player.transform.position, transform.position) <= patrolDistance + 3f;
    }

    // ====================
    // CAP NHAT BO DEM
    // ====================

    void UpdateAllTimers()
    {
        UpdateHurtTimer();
        UpdateAttackTimer();
        UpdateIdleTimer();
    }

    void UpdateHurtTimer()
    {
        if (isHurting)
        {
            hurtTimer -= Time.fixedDeltaTime;
            if (hurtTimer <= 0)
            {
                isHurting = false;
            }
        }
    }

    void UpdateAttackTimer()
    {
        if (isAttacking)
        {
            attackDurationTimer -= Time.fixedDeltaTime;

            // Gay sat thuong o giua animation
            if (!hasDealtDamageThisAttack && attackDurationTimer <= attackDuration / 2f)
            {
                ExecuteAttackDamage();
                hasDealtDamageThisAttack = true;
            }

            // Ket thuc tan cong
            if (attackDurationTimer <= 0f)
            {
                StopAttack();
            }
        }
    }


    void UpdateIdleTimer()
    {
        if (isIdleAfterAttack)
        {
            idleAfterAttackTimer -= Time.fixedDeltaTime;
            if (idleAfterAttackTimer <= 0f)
            {
                isIdleAfterAttack = false;
            }
        }
    }

    // ====================
    // HANH VI
    // ====================

    void HandleBehavior()
    {
        if (isAttacking || isIdleAfterAttack)
        {
            StopMovement();
            return;
        }

        if (IsPlayerInRange())
        {
            StopMovement();
            TryAttack();
        }
        else if (IsPlayerDetected())
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    void StopMovement()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    // ====================
    // DI CHUYEN: TUAN TRA
    // ====================

    void Patrol()
    {
        float direction = moveRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);

        CheckPatrolBounds();
        UpdateFacingDirection(direction);
    }

    void CheckPatrolBounds()
    {
        if (moveRight && transform.position.x > startPos.x + patrolDistance)
        {
            moveRight = false;
        }
        else if (!moveRight && transform.position.x < startPos.x - patrolDistance)
        {
            moveRight = true;
        }
    }

    // ====================
    // DI CHUYEN: DUOI THEO
    // ====================

    void ChasePlayer()
    {
        float direction = GetDirectionToPlayer();
        rb.linearVelocity = new Vector2(direction * chaseSpeed, rb.linearVelocity.y);
        UpdateFacingDirection(direction);
    }

    float GetDirectionToPlayer()
    {
        return (player.transform.position.x - transform.position.x) >= 0 ? 1f : -1f;
    }

    // ====================
    // HANH DONG: TAN CONG
    // ====================

    void TryAttack()
    {
        if (!CanAttack()) return;

        FacePlayer();
        StartAttack();
    }

    void FacePlayer()
    {
        float direction = GetDirectionToPlayer();
        UpdateFacingDirection(direction);
    }

    void StartAttack()
    {
        isAttacking = true;
        attackDurationTimer = attackDuration;
        hasDealtDamageThisAttack = false;
    }

    void StopAttack()
    {
        isAttacking = false;
        isIdleAfterAttack = true;
        idleAfterAttackTimer = idleAfterAttackDuration;
        hasDealtDamageThisAttack = false;
    }

    void ExecuteAttackDamage()
    {
        if (player == null) return;

        Collider2D hitPlayer = Physics2D.OverlapCircle(
            transform.position,
            attackRadius,
            LayerMask.GetMask("Player")
        );

        if (hitPlayer != null)
        {
            DamagePlayer(hitPlayer);
        }
    }

    void DamagePlayer(Collider2D playerCollider)
    {
        Debug.Log("Golem danh trung Player: " + playerCollider.name);

        Vector2 knockbackDirection = (playerCollider.transform.position - transform.position).normalized;

        SoldierController soldierController = playerCollider.GetComponent<SoldierController>();
        if (soldierController != null)
        {
            soldierController.TakeDamage(attackDamage, knockbackDirection);
        }
    }

    // ====================
    // NHAN SAT THUONG
    // ====================

    public void TakeDamage(int damage)
    {
        if (IsDead()) return;

        ApplyDamage(damage);
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
        Debug.Log("Golem nhan " + damage + " sat thuong. Con " + currentHealth + " mau");
    }

    void ShowHurtAnimation()
    {
        isHurting = true;
        hurtTimer = hurtDuration;
    }

    // ====================
    // CHET
    // ====================

    void Die()
    {
        isDeath = true;
        isHurting = true;

        Debug.Log("Golem chet!");

        StopMovement();
        SetDeathAnimation();
        DropGold();

        if (EnemyCountManager.Instance != null)
        {
            EnemyCountManager.Instance.AddKill();
        }

        Destroy(gameObject, 0.5f);
    }

    void SetDeathAnimation()
    {
        if (animator != null)
        {
            animator.SetBool("IsDeath", true);
        }
    }

    // ====================
    // ROI DO
    // ====================

    void DropGold()
    {
        if (goldCoinPrefab == null)
        {
            Debug.LogWarning("Gold Coin Prefab chua duoc gan!");
            return;
        }

        for (int i = 0; i < goldDropAmount; i++)
        {
            CreateGoldCoin();
        }
    }

    void CreateGoldCoin()
    {
        GameObject coin = Instantiate(goldCoinPrefab, transform.position, Quaternion.identity);

        Rigidbody2D coinRB = coin.GetComponent<Rigidbody2D>();
        if (coinRB != null)
        {
            ApplyRandomDropForce(coinRB);
        }

        Debug.Log("Roi 1 coin vang!");
    }

    void ApplyRandomDropForce(Rigidbody2D coinRB)
    {
        float randomX = Random.Range(-1f, 1f);
        Vector2 dropDirection = new Vector2(randomX, 1f).normalized;
        // Co the them luc neu can: coinRB.AddForce(dropDirection * force);
    }

    // ====================
    // ANIMATION & HUONG
    // ====================

    void UpdateFacingDirection(float direction)
    {
        if (sr != null)
        {
            sr.flipX = direction < 0;
        }
    }

    void UpdateAnimation()
    {
        if (animator == null) return;

        bool isWalking = Mathf.Abs(rb.linearVelocity.x) > 0.01f && CanMove();

        animator.SetBool("IsWalking", isWalking);
        animator.SetBool("IsAttacking", isAttacking);
        animator.SetBool("IsHurting", isHurting);
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