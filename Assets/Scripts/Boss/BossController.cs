using UnityEngine;
using System.Collections;

/// Boss Controller - Dieu khien hanh vi cua Boss
/// Gom co: Patrol, Chase, Attack, Phase Transition
public class BossController : MonoBehaviour
{
    // ====================
    // THONG SO CAN THIET
    // ====================

    [Header("Thong So Co Ban")]
    public int maxHealth = 50;              // Mau toi da
    public float moveSpeed = 3f;            // Toc do di chuyen binh thuong
    public float chaseSpeed = 5f;           // Toc do duoi theo player

    [Header("Pham Vi")]
    public float detectionRange = 12f;      // Pham vi phat hien player
    public float meleeRange = 2f;           // Pham vi tan cong can chien
    public float rangedRange = 8f;          // Pham vi tan cong tam xa
    public float stoppingDistance = 1.5f;   // Khoang cach dung lai

    [Header("Sat Thuong")]
    public int meleeDamage = 3;             // Sat thuong can chien
    public int rangedDamage = 5;            // Sat thuong tam xa

    [Header("Thoi Gian Tan Cong")]
    public float attackCooldown = 2f;           // Thoi gian cho giua cac don
    public float comboAttackCooldown = 4f;      // Thoi gian cho sau combo

    [Header("Chuyen Pha")]
    public float phase2HealthPercent = 0.65f;   // Mau de vao phase 2 (65%)
    public float phase3HealthPercent = 0.35f;   // Mau de vao phase 3 (35%)

    [Header("Toc Do Di Chuyen")]
    public float dashSpeed = 15f;           // Toc do dash
    public float retreatSpeed = 6f;         // Toc do lui
    public float jumpForce = 15f;           // Luc nhay

    [Header("Ban Kinh Tan Cong")]
    public float meleeAttackRadius = 2f;        // Ban kinh tan cong can chien
    public float rangedAttackRadius = 3f;       // Ban kinh tan cong tam xa

    [Header("Tuan Tra")]
    public float patrolDistance = 5f;           // Khoang cach tuan tra

    // ====================
    // THANH PHAN
    // ====================

    private Rigidbody2D rb;                 // Vat ly
    private SpriteRenderer sr;              // Hinh anh
    private Animator animator;              // Animation
    private Transform playerTransform;      // Vi tri player
    private Collider2D bossCollider;        // Va cham boss
    private Collider2D playerCollider;      // Va cham player

    // ====================
    // TRANG THAI
    // ====================

    private int currentHealth;              // Mau hien tai
    private int currentPhase = 1;           // Phase hien tai (1, 2, 3)
    private bool isAttacking;               // Dang tan cong?
    private bool isEnraged;                 // Dang phan no?

    // ====================
    // BO DEM THOI GIAN
    // ====================

    private float attackCooldownTimer;      // Dem thoi gian cooldown
    private float enrageTimer;              // Dem thoi gian phan no

    // ====================
    // TUAN TRA
    // ====================

    private Vector3 patrolLeft;             // Diem tuan tra trai
    private Vector3 patrolRight;            // Diem tuan tra phai
    private int patrolDirection = 1;        // Huong di (1=phai, -1=trai)

    // ====================
    // LOAI TAN CONG
    // ====================

    private enum AttackType
    {
        MeleeBasic,     // Can chien co ban
        MeleeHeavy,     // Can chien manh
        Combo,          // Combo: Chem - Lui - Ban
        DashAttack,     // Dash toi roi chem
        AerialAttack,   // Bay tan cong (Phase 2+)
        Retreat         // Lui lai
    }

    // ====================
    // KHOI TAO
    // ====================

    void Start()
    {
        GetComponents();
        SetupPatrol();
        FindPlayer();
        DisableCollisionWithPlayer();

        currentHealth = maxHealth;
    }

    void Update()
    {
        if (IsDead()) return;

        UpdateAllTimers();
        CheckForPhaseChange();

        if (HasPlayer())
        {
            BossBehavior();
        }
        else
        {
            PatrolAround();
        }

        UpdateAnimationStates();
    }

    void FixedUpdate()
    {
        if (IsDead() || isAttacking) return;

        MoveBasedOnSituation();
    }

    // ====================
    // CAI DAT BAN DAU
    // ====================

    void GetComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        bossCollider = GetComponent<Collider2D>();

        rb.gravityScale = 3f;
        rb.freezeRotation = true;
    }

    void SetupPatrol()
    {
        patrolLeft = transform.position - Vector3.right * patrolDistance;
        patrolRight = transform.position + Vector3.right * patrolDistance;
    }

    void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            playerCollider = playerObj.GetComponent<Collider2D>();
        }
    }

    void DisableCollisionWithPlayer()
    {
        // Tat va cham vat ly de tranh day nhau
        if (bossCollider != null && playerCollider != null)
        {
            Physics2D.IgnoreCollision(bossCollider, playerCollider, true);
        }
    }

    // ====================
    // KIEM TRA TRANG THAI
    // ====================

    bool IsDead()
    {
        return currentHealth <= 0;
    }

    bool HasPlayer()
    {
        return playerTransform != null;
    }

    float GetDistanceToPlayer()
    {
        if (!HasPlayer()) return float.MaxValue;
        return Vector2.Distance(transform.position, playerTransform.position);
    }

    // ====================
    // CAP NHAT BO DEM
    // ====================

    void UpdateAllTimers()
    {
        if (attackCooldownTimer > 0)
            attackCooldownTimer -= Time.deltaTime;

        if (enrageTimer > 0)
        {
            enrageTimer -= Time.deltaTime;
            if (enrageTimer <= 0)
                isEnraged = false;
        }
    }

    // ====================
    // QUAN LY PHASE
    // ====================

    void CheckForPhaseChange()
    {
        float healthPercent = (float)currentHealth / maxHealth;

        if (healthPercent <= phase3HealthPercent && currentPhase < 3)
        {
            EnterPhase(3);
        }
        else if (healthPercent <= phase2HealthPercent && currentPhase < 2)
        {
            EnterPhase(2);
        }
    }

    void EnterPhase(int newPhase)
    {
        currentPhase = newPhase;
        isEnraged = true;
        enrageTimer = 10f;

        // Tang suc manh moi phase
        moveSpeed *= 1.2f;
        chaseSpeed *= 1.2f;
        dashSpeed *= 1.1f;
        jumpForce *= 1.1f;

        Debug.Log("Boss vao Phase " + newPhase + "!");
    }

    // ====================
    // TUAN TRA
    // ====================

    void PatrolAround()
    {
        float targetX = (patrolDirection == 1) ? patrolRight.x : patrolLeft.x;

        // Doi huong khi den cuoi duong
        if (patrolDirection == 1 && transform.position.x >= targetX)
        {
            ChangeDirection(-1);
        }
        else if (patrolDirection == -1 && transform.position.x <= targetX)
        {
            ChangeDirection(1);
        }

        rb.linearVelocity = new Vector2(patrolDirection * moveSpeed, rb.linearVelocity.y);
    }

    void ChangeDirection(int newDirection)
    {
        patrolDirection = newDirection;
        sr.flipX = (newDirection == -1);
    }

    // ====================
    // HANH VI BOSS
    // ====================

    void BossBehavior()
    {
        if (isAttacking) return;

        float distance = GetDistanceToPlayer();
        LookAtPlayer();

        // Qua xa - tuan tra
        if (distance > detectionRange)
        {
            PatrolAround();
            return;
        }

        // Dang cooldown - duoi theo
        if (attackCooldownTimer > 0)
        {
            ChasePlayerSafely(distance);
            return;
        }

        // Chon loai tan cong dua vao khoang cach
        ChooseAndExecuteAttack(distance);
    }

    void LookAtPlayer()
    {
        if (!HasPlayer()) return;

        bool shouldFlipLeft = playerTransform.position.x < transform.position.x;
        sr.flipX = shouldFlipLeft;
        patrolDirection = shouldFlipLeft ? -1 : 1;
    }

    void ChasePlayerSafely(float currentDistance)
    {
        if (isAttacking) return;

        // Dung lai neu qua gan
        if (currentDistance <= stoppingDistance)
        {
            StopMoving();
            return;
        }

        // Duoi theo
        float direction = playerTransform.position.x > transform.position.x ? 1 : -1;
        rb.linearVelocity = new Vector2(direction * chaseSpeed, rb.linearVelocity.y);
    }

    void StopMoving()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    // ====================
    // CHON TAN CONG
    // ====================

    void ChooseAndExecuteAttack(float distance)
    {
        AttackType chosenAttack;

        // GAN (can chien)
        if (distance <= meleeRange)
        {
            chosenAttack = ChooseCloseRangeAttack();
        }
        // TAM TRUNG (hon hop)
        else if (distance <= rangedRange)
        {
            chosenAttack = ChooseMidRangeAttack();
        }
        // XA (tam xa)
        else
        {
            chosenAttack = ChooseLongRangeAttack();
        }

        StartCoroutine(ExecuteAttack(chosenAttack));
    }

    AttackType ChooseCloseRangeAttack()
    {
        float rand = Random.value;

        if (rand > 0.8f)
            return AttackType.Retreat;          // 20% lui
        else if (rand > 0.5f)
            return AttackType.Combo;            // 30% combo
        else if (rand > 0.25f)
            return AttackType.MeleeBasic;       // 25% can chien co ban
        else
            return AttackType.MeleeHeavy;       // 25% can chien manh
    }

    AttackType ChooseMidRangeAttack()
    {
        int choice = Random.Range(0, 10);

        if (choice < 4)
            return AttackType.DashAttack;       // 40% dash
        else if (choice < 7 && currentPhase >= 2)
            return AttackType.AerialAttack;     // 30% bay (neu phase 2+)
        else
            return Random.value > 0.5f ? AttackType.MeleeBasic : AttackType.MeleeHeavy;
    }

    AttackType ChooseLongRangeAttack()
    {
        if (currentPhase >= 2 && Random.value > 0.5f)
            return AttackType.AerialAttack;     // 50% bay (phase 2+)
        else
            return AttackType.DashAttack;       // 50% dash
    }

    // ====================
    // THUC HIEN TAN CONG
    // ====================

    IEnumerator ExecuteAttack(AttackType attackType)
    {
        StartAttacking();

        switch (attackType)
        {
            case AttackType.MeleeBasic:
                yield return MeleeBasicAttack();
                break;

            case AttackType.MeleeHeavy:
                yield return MeleeHeavyAttack();
                break;

            case AttackType.Combo:
                yield return ComboAttack();
                break;

            case AttackType.DashAttack:
                yield return DashAttack();
                break;

            case AttackType.AerialAttack:
                yield return AerialAttack();
                break;

            case AttackType.Retreat:
                yield return RetreatAttack();
                break;
        }

        StopAttacking();
    }

    void StartAttacking()
    {
        isAttacking = true;
        StopMoving();
    }

    void StopAttacking()
    {
        isAttacking = false;
    }

    // ====================
    // CAC LOAI TAN CONG
    // ====================

    IEnumerator MeleeBasicAttack()
    {
        animator.SetBool("isAttacking1", true);

        yield return new WaitForSeconds(0.3f);
        HitEnemiesInRange(meleeAttackRadius, meleeDamage);

        yield return new WaitForSeconds(0.3f);
        animator.SetBool("isAttacking1", false);

        attackCooldownTimer = CalculateCooldown(attackCooldown);
    }

    IEnumerator MeleeHeavyAttack()
    {
        animator.SetBool("isAttacking2", true);

        yield return new WaitForSeconds(0.5f);
        HitEnemiesInRange(meleeAttackRadius, meleeDamage * 2);

        yield return new WaitForSeconds(0.5f);
        animator.SetBool("isAttacking2", false);

        attackCooldownTimer = CalculateCooldown(attackCooldown * 1.5f);
    }

    IEnumerator ComboAttack()
    {
        // BUOC 1: CHEM
        animator.SetBool("isAttacking1", true);
        yield return new WaitForSeconds(0.3f);
        HitEnemiesInRange(meleeAttackRadius, meleeDamage);
        yield return new WaitForSeconds(0.2f);
        animator.SetBool("isAttacking1", false);

        // BUOC 2: DASH LUI
        animator.SetBool("isDashing", true);
        DashBackwards();
        yield return new WaitForSeconds(0.5f);
        StopMoving();
        animator.SetBool("isDashing", false);

        // BUOC 3: BAN XA
        yield return new WaitForSeconds(0.2f);
        animator.SetBool("isAttacking2", true);
        yield return new WaitForSeconds(0.4f);
        HitEnemiesInRange(rangedAttackRadius, rangedDamage);
        yield return new WaitForSeconds(0.4f);
        animator.SetBool("isAttacking2", false);

        attackCooldownTimer = CalculateCooldown(comboAttackCooldown);
    }

    IEnumerator DashAttack()
    {
        // BUOC 1: DASH TOI
        animator.SetBool("isDashing", true);
        DashTowardsPlayer();

        yield return new WaitForSeconds(0.4f);

        StopMoving();
        animator.SetBool("isDashing", false);

        // BUOC 2: CHEM NHANH
        yield return new WaitForSeconds(0.1f);
        animator.SetBool("isAttacking1", true);

        yield return new WaitForSeconds(0.2f);
        HitEnemiesInRange(meleeAttackRadius, meleeDamage);

        yield return new WaitForSeconds(0.2f);
        animator.SetBool("isAttacking1", false);

        attackCooldownTimer = CalculateCooldown(attackCooldown * 2f);
    }

    IEnumerator AerialAttack()
    {
        if (currentPhase < 2) yield break;

        animator.SetBool("isFlying", true);
        ReduceGravity();
        JumpTowardsPlayer();

        yield return new WaitForSeconds(0.6f);
        HitEnemiesInRange(rangedAttackRadius, rangedDamage * 2);

        yield return new WaitForSeconds(0.6f);
        RestoreGravity();
        animator.SetBool("isFlying", false);

        yield return new WaitForSeconds(0.3f);
        attackCooldownTimer = CalculateCooldown(attackCooldown * 3f);
    }

    IEnumerator RetreatAttack()
    {
        animator.SetBool("isDashing", true);
        DashBackwards();

        yield return new WaitForSeconds(0.6f);
        StopMoving();
        animator.SetBool("isDashing", false);

        yield return new WaitForSeconds(0.2f);
        attackCooldownTimer = CalculateCooldown(attackCooldown * 0.5f);
    }

    // ====================
    // XU LY DI CHUYEN
    // ====================

    void DashTowardsPlayer()
    {
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * dashSpeed, 0);
    }

    void DashBackwards()
    {
        int direction = sr.flipX ? 1 : -1;
        rb.linearVelocity = new Vector2(direction * retreatSpeed, 2f);
    }

    void JumpTowardsPlayer()
    {
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * (dashSpeed * 0.6f), jumpForce);
    }

    void ReduceGravity()
    {
        rb.gravityScale = 1.5f;
    }

    void RestoreGravity()
    {
        rb.gravityScale = 3f;
    }

    // ====================
    // XU LY SAT THUONG
    // ====================

    void HitEnemiesInRange(float radius, int damage)
    {
        int finalDamage = CalculateDamage(damage);

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            radius,
            LayerMask.GetMask("Player")
        );

        foreach (Collider2D hit in hits)
        {
            SoldierController player = hit.GetComponent<SoldierController>();
            if (player != null)
            {
                Vector2 knockbackDirection = (hit.transform.position - transform.position).normalized;
                player.TakeDamage(finalDamage, knockbackDirection);
            }
        }
    }

    int CalculateDamage(int baseDamage)
    {
        float multiplier = 1f + (currentPhase - 1) * 0.3f;  // Tang 30% moi phase
        if (isEnraged) multiplier *= 1.2f;                  // Tang 20% khi phan no

        return Mathf.RoundToInt(baseDamage * multiplier);
    }

    float CalculateCooldown(float baseCooldown)
    {
        float multiplier = 1f - (currentPhase - 1) * 0.15f; // Giam 15% moi phase
        if (isEnraged) multiplier *= 0.8f;                  // Giam 20% khi phan no

        return baseCooldown * multiplier;
    }

    // ====================
    // NHAN SAT THUONG
    // ====================

    public void TakeDamage(int damage)
    {
        if (IsDead()) return;

        currentHealth -= damage;
        Debug.Log("Boss nhan " + damage + " sat thuong. Con " + currentHealth + " mau");

        if (!isAttacking)
        {
            StartCoroutine(ShowHurtAnimation());
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator ShowHurtAnimation()
    {
        animator.SetBool("isHurting", true);
        yield return new WaitForSeconds(0.3f);
        animator.SetBool("isHurting", false);
    }

    void Die()
    {
        Debug.Log("Boss da chet!");
        animator.SetBool("isDie", true);
        StopMoving();
        StopAllCoroutines();
        enabled = false;
    }

    // ====================
    // DI CHUYEN TU DONG
    // ====================

    void MoveBasedOnSituation()
    {
        if (isAttacking) return;

        if (!HasPlayer())
        {
            rb.linearVelocity = new Vector2(patrolDirection * moveSpeed, rb.linearVelocity.y);
            return;
        }

        float distance = GetDistanceToPlayer();

        if (distance < detectionRange && distance > stoppingDistance)
        {
            ChasePlayerSafely(distance);
        }
        else if (distance >= detectionRange)
        {
            rb.linearVelocity = new Vector2(patrolDirection * moveSpeed, rb.linearVelocity.y);
        }
        else
        {
            StopMoving();
        }
    }

    // ====================
    // CAP NHAT ANIMATION
    // ====================

    void UpdateAnimationStates()
    {
        if (animator == null) return;

        bool isMoving = !isAttacking && Mathf.Abs(rb.linearVelocity.x) > 0.1f;
        animator.SetBool("isRunning", isMoving);
    }

    // ====================
    // HIEN THI DEBUG
    // ====================

    void OnDrawGizmosSelected()
    {
        // Tan cong can chien (do)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeAttackRadius);

        // Tan cong tam xa (xanh duong)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, rangedAttackRadius);

        // Phat hien (vang)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Dung lai (hong)
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);

        // Duong tuan tra (xanh la)
        Gizmos.color = Color.green;
        Vector3 left = transform.position - Vector3.right * patrolDistance;
        Vector3 right = transform.position + Vector3.right * patrolDistance;
        Gizmos.DrawLine(left, right);
    }
}