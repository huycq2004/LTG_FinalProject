using UnityEngine;
using System.Collections;

/// Boss Controller - Dieu khien hanh vi cua Boss
/// Gom co: Patrol, Chase, Attack, Phase Transition, Projectile Attack
public class BossController : MonoBehaviour
{
    // ====================
    // THONG SO CAN THIET
    // ====================

    [Header("Thong So Co Ban")]
    public int maxHealth = 50;
    public float moveSpeed = 3f;
    public float chaseSpeed = 5f;

    [Header("Pham Vi")]
    public float detectionRange = 12f;
    public float meleeRange = 2f;
    public float rangedRange = 8f;
    public float stoppingDistance = 1.5f;

    [Header("Sat Thuong")]
    public int meleeDamage = 3;
    public int rangedDamage = 5;

    [Header("Thoi Gian Tan Cong")]
    public float attackCooldown = 2f;
    public float comboAttackCooldown = 4f;

    [Header("Chuyen Pha")]
    public float phase2HealthPercent = 0.65f;
    public float phase3HealthPercent = 0.35f;

    [Header("Toc Do Di Chuyen")]
    public float dashSpeed = 15f;
    public float retreatSpeed = 6f;
    public float jumpForce = 15f;

    [Header("Ban Kinh Tan Cong")]
    public float meleeAttackRadius = 2f;
    public float rangedAttackRadius = 3f;

    [Header("He Thong Ban Dan")]
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;

    [Header("Thong So Dan")]
    public float projectileSpeed = 10f;
    public float projectileLifetime = 5f;

    [Header("Thanh Mau")]
    public BossHealthBarUI healthBarUI;

    // ====================
    // THANH PHAM
    // ====================

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator animator;
    private Transform playerTransform;
    private Collider2D bossCollider;
    private Collider2D playerCollider;

    // ====================
    // TRANG THAI
    // ====================

    private int currentHealth;
    private int currentPhase = 1;
    private bool isAttacking;
    private bool isEnraged;
    private bool isAggro;  // Trang thai khi bi tac cong - tu dong duoi theo
    private bool hasTriggeredBossMusic = false;

    // ====================
    // BO DEM THOI GIAN
    // ====================

    private float attackCooldownTimer;
    private float enrageTimer;

    // ====================
    // LOAI TAN CONG
    // ====================

    private enum AttackType
    {
        MeleeBasic,
        MeleeHeavy,
        Combo,
        DashAttack,
        AerialAttack,
        Retreat
    }

    // ====================
    // KHOI TAO
    // ====================

    void Start()
    {
        GetComponents();
        FindPlayer();
        DisableCollisionWithPlayer();
        SetupProjectileSpawnPoint();

        currentHealth = maxHealth;

        if (healthBarUI != null)
        {
            healthBarUI.ResetHealthBar(maxHealth);
        }
    }

    void Update()
    {
        if (IsDead()) return;

        UpdateAllTimers();
        CheckForPhaseChange();

        if (HasPlayer())
        {
            CheckBossMusicTrigger();
            BossBehavior();
        }
        else
        {
            StayIdle();
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
        if (bossCollider != null && playerCollider != null)
        {
            Physics2D.IgnoreCollision(bossCollider, playerCollider, true);
        }
    }

    void SetupProjectileSpawnPoint()
    {
        // Neu chua co spawn point thi tao mot cai
        if (projectileSpawnPoint == null)
        {
            GameObject spawnObj = new GameObject("ProjectileSpawnPoint");
            spawnObj.transform.SetParent(transform);
            spawnObj.transform.localPosition = new Vector3(1f, 0.5f, 0);
            projectileSpawnPoint = spawnObj.transform;
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
    // KICH HOAT NHAC BOSS
    // ====================

    void CheckBossMusicTrigger()
    {
        // Chi kich hoat nhac 1 lan duy nhat
        if (hasTriggeredBossMusic) return;

        float distance = GetDistanceToPlayer();

        // Khi player vao pham vi tan cong cua boss
        if (distance <= detectionRange)
        {
            TriggerBossMusic();
        }
    }

    void TriggerBossMusic()
    {
        hasTriggeredBossMusic = true;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBossMusic();
            Debug.Log("Player vao pham vi Boss! Bat nhac Boss!");
        }
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

        moveSpeed *= 1.2f;
        chaseSpeed *= 1.2f;
        dashSpeed *= 1.1f;
        jumpForce *= 1.1f;

        Debug.Log("Boss vao Phase " + newPhase + "!");
    }

    // ====================
    // HANH VI BOSS
    // ====================

    void BossBehavior()
    {
        if (isAttacking) return;

        float distance = GetDistanceToPlayer();
        LookAtPlayer();

        // Khi bi tac cong, khong can co khoang cach detect, tu dong duoi theo
        float effectiveDetectionRange = isAggro ? float.MaxValue : detectionRange;

        if (distance > effectiveDetectionRange)
        {
            StayIdle();
            return;
        }

        if (attackCooldownTimer > 0)
        {
            ChasePlayerSafely(distance);
            return;
        }

        ChooseAndExecuteAttack(distance);
    }

    void LookAtPlayer()
    {
        if (!HasPlayer()) return;

        bool shouldFlipLeft = playerTransform.position.x < transform.position.x;
        sr.flipX = shouldFlipLeft;
    }

    void ChasePlayerSafely(float currentDistance)
    {
        if (isAttacking) return;

        if (currentDistance <= stoppingDistance)
        {
            StopMoving();
            return;
        }

        float direction = playerTransform.position.x > transform.position.x ? 1 : -1;
        rb.linearVelocity = new Vector2(direction * chaseSpeed, rb.linearVelocity.y);
    }

    void StopMoving()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    void StayIdle()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    // ====================
    // CHON TAN CONG
    // ====================

    void ChooseAndExecuteAttack(float distance)
    {
        AttackType chosenAttack;

        if (distance <= meleeRange)
        {
            chosenAttack = ChooseCloseRangeAttack();
        }
        else if (distance <= rangedRange)
        {
            chosenAttack = ChooseMidRangeAttack();
        }
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
            return AttackType.Retreat;
        else if (rand > 0.5f)
            return AttackType.Combo;
        else if (rand > 0.25f)
            return AttackType.MeleeBasic;
        else
            return AttackType.MeleeHeavy;
    }

    AttackType ChooseMidRangeAttack()
    {
        int choice = Random.Range(0, 10);

        if (choice < 4)
            return AttackType.DashAttack;
        else if (choice < 7 && currentPhase >= 2)
            return AttackType.AerialAttack;
        else
            return Random.value > 0.5f ? AttackType.MeleeBasic : AttackType.MeleeHeavy;
    }

    AttackType ChooseLongRangeAttack()
    {
        if (currentPhase >= 2 && Random.value > 0.5f)
            return AttackType.AerialAttack;
        else
            return AttackType.DashAttack;
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

        yield return new WaitForSeconds(0.3f);

        // BAN DAN TRONG ATTACK2
        FireProjectile();

        yield return new WaitForSeconds(0.5f);
        animator.SetBool("isAttacking2", false);

        attackCooldownTimer = CalculateCooldown(attackCooldown * 1.5f);
    }

    IEnumerator ComboAttack()
    {
        // BUOC 1: CHEM
        animator.SetBool("isAttacking1", true);
        HitEnemiesInRange(meleeAttackRadius, meleeDamage);
        yield return new WaitForSeconds(0.2f);
        animator.SetBool("isAttacking1", false);

        // BUOC 2: DASH LUI
        animator.SetBool("isDashing", true);
        DashBackwards();
        yield return new WaitForSeconds(0.5f);
        StopMoving();
        animator.SetBool("isDashing", false);

        // BUOC 3: BAN DAN
        yield return new WaitForSeconds(0.2f);
        animator.SetBool("isAttacking2", true);
        yield return new WaitForSeconds(0.4f);

        // BAN DAN TRONG COMBO
        FireProjectile();

        yield return new WaitForSeconds(0.4f);
        animator.SetBool("isAttacking2", false);

        attackCooldownTimer = CalculateCooldown(comboAttackCooldown);
    }

    IEnumerator DashAttack()
    {
        animator.SetBool("isDashing", true);
        DashTowardsPlayer();

        yield return new WaitForSeconds(0.4f);

        StopMoving();
        animator.SetBool("isDashing", false);

        yield return new WaitForSeconds(0.1f);
        animator.SetBool("isAttacking1", true);
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
    // BAN DAN
    // ====================

    void FireProjectile()
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("Projectile Prefab chua duoc gan!");
            return;
        }

        if (!HasPlayer()) return;

        Vector3 spawnPosition = GetProjectileSpawnPosition();
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);

        SetupProjectile(projectile);

        Debug.Log("Boss ban dan!");
    }

    Vector3 GetProjectileSpawnPosition()
    {
        if (projectileSpawnPoint != null)
        {
            return projectileSpawnPoint.position;
        }

        // Neu khong co spawn point thi ban tu vi tri boss + offset
        float offsetX = sr.flipX ? -1f : 1f;
        return transform.position + new Vector3(offsetX, 0.5f, 0);
    }

    void SetupProjectile(GameObject projectile)
    {
        BossProjectile projectileScript = projectile.GetComponent<BossProjectile>();
        if (projectileScript != null)
        {
            Vector2 direction = GetDirectionToPlayer();
            projectileScript.SetDirection(direction);
            projectileScript.speed = projectileSpeed;
            projectileScript.damage = CalculateDamage(rangedDamage);
            projectileScript.lifetime = projectileLifetime;
        }
    }

    Vector2 GetDirectionToPlayer()
    {
        if (!HasPlayer()) return Vector2.right;

        Vector2 direction = (playerTransform.position - transform.position);

        // CHI LAY HUONG NGANG - loai bo thanh phan Y
        direction.y = 0;

        return direction.normalized;
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
        float multiplier = 1f + (currentPhase - 1) * 0.3f;
        if (isEnraged) multiplier *= 1.2f;

        return Mathf.RoundToInt(baseDamage * multiplier);
    }

    float CalculateCooldown(float baseCooldown)
    {
        float multiplier = 1f - (currentPhase - 1) * 0.15f;
        if (isEnraged) multiplier *= 0.8f;

        return baseCooldown * multiplier;
    }

    // ====================
    // NHAN SAT THUONG
    // ====================

    public void TakeDamage(int damage)
    {
        if (IsDead()) return;

        isAggro = true;  // Kich hoat che do khieu chien khi bi tac cong
        currentHealth -= damage;
        Debug.Log("Boss nhan " + damage + " sat thuong. Con " + currentHealth + " mau");

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayHitSound();
        }

        if (healthBarUI != null)
        {
            healthBarUI.UpdateHealth(currentHealth, maxHealth);
        }

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

        if (healthBarUI != null)
        {
            healthBarUI.ForceHide();
        }

        // Quay lai nhac gameplay khi boss chet
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGameplayMusic();
        }

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
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        float distance = GetDistanceToPlayer();

        if (distance < detectionRange && distance > stoppingDistance)
        {
            ChasePlayerSafely(distance);
        }
        else if (distance >= detectionRange)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);
    }
}