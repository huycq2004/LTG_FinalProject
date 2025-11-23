using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GolemController : MonoBehaviour
{
    [Header("Enemy Settings")]
    public float moveSpeed = 2f;
    public float chaseSpeed = 4f;
    public float patrolDistance = 5f;
    public float attackRange = 1.2f;
    public float attackDuration = 0.5f;
    public float idleAfterAttackDuration = 0.3f;
    public float hurtDuration = 0.2f;

    [Header("Health Settings")]
    public int maxHealth = 8;

    [Header("Attack Settings")]
    public float attackRadius = 0.8f;
    public int attackDamage = 2;

    [Header("Drop Settings")]
    public GameObject goldCoinPrefab;  // Prefab coin vang
    public int goldDropAmount = 2;     // So luong coin roi khi chet
    public float dropForce = 5f;       // Luc roi cua coin

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator animator;
    private Collider2D golemCollider;
    private Vector3 startPos;
    private GameObject player;

    // Các trạng thái
    private bool moveRight;
    private bool isAttacking;
    private bool isIdleAfterAttack;
    private bool isHurting;
    private bool isDeath;

    // Bộ đếm thời gian
    private float attackDurationTimer;
    private float idleAfterAttackTimer;
    private float hurtTimer;
    private bool hasDealtDamageThisAttack;

    // Máu
    private int currentHealth;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        golemCollider = GetComponent<Collider2D>();
        startPos = transform.position;

        rb.gravityScale = 3f;
        rb.freezeRotation = true;

        player = GameObject.FindGameObjectWithTag("Player");

        // Tắt collision giữa các Enemy với nhau
        DisableEnemyCollisions();

        // Khởi tạo máu
        currentHealth = maxHealth;
    }

    void FixedUpdate()
    {
        // Nếu chết thì không làm gì
        if (isDeath) return;

        // Giảm thời gian bị đánh (hurt)
        if (isHurting)
        {
            hurtTimer -= Time.fixedDeltaTime;
            if (hurtTimer <= 0)
                isHurting = false;
        }

        // Xử lý các trạng thái khác nhau dựa trên trạng thái hiện tại
        if (isAttacking)
            HandleAttack();
        else if (isIdleAfterAttack)
            HandleIdleAfterAttack();
        else
            HandleMovement();

        UpdateAnimation();
    }

    void HandleAttack()
    {
        // Dừng lại khi đang tấn công - vô hiệu hóa di chuyển
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        
        // Giảm thời gian tấn công còn lại
        attackDurationTimer -= Time.fixedDeltaTime;

        // Gây sát thương ở giữa animation tấn công để tránh nhiều lần đánh
        if (!hasDealtDamageThisAttack && attackDurationTimer <= attackDuration / 2f)
        {
            DealDamage();
            hasDealtDamageThisAttack = true;
        }

        // Kết thúc tấn công, chuyển sang idle
        if (attackDurationTimer <= 0f)
        {
            isAttacking = false;
            isIdleAfterAttack = true;
            idleAfterAttackTimer = idleAfterAttackDuration;
            hasDealtDamageThisAttack = false;
        }
    }

    void HandleIdleAfterAttack()
    {
        // Đứng im sau tấn công - vô hiệu hóa di chuyển
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        
        // Giảm thời gian idle
        idleAfterAttackTimer -= Time.fixedDeltaTime;
        
        // Khi idle kết thúc, có thể di chuyển trở lại
        if (idleAfterAttackTimer <= 0f)
            isIdleAfterAttack = false;
    }

    void HandleMovement()
    {
        // Nếu Player tồn tại, quyết định xem tấn công hay di chuyển
        if (player != null)
        {
            // Tính khoảng cách giữa Golem và Player
            float distance = Vector2.Distance(player.transform.position, transform.position);

            // Nếu Player trong vùng tấn công - dừng di chuyển và tấn công
            if (distance <= attackRange)
            {
                rb.linearVelocity = Vector2.zero;
                TryAttack();
            }
            // Nếu Player trong vùng xác định được - chạy tới
            else if (distance <= patrolDistance + 3f)
                ChasePlayer();
            // Nếu Player quá xa - di chuyển bình thường
            else
                Patrol();
        }
        else
            // Nếu không có Player - di chuyển bình thường
            Patrol();
    }

    void Patrol()
    {
        // Tính hướng di chuyển dựa trên moveRight (1 = phải, -1 = trái)
        float dir = moveRight ? 1f : -1f;
        
        // Di chuyển theo hướng hiện tại
        rb.linearVelocity = new Vector2(dir * moveSpeed, rb.linearVelocity.y);

        // Đổi hướng khi chạm ranh giới patrol
        if (moveRight && transform.position.x > startPos.x + patrolDistance)
            moveRight = false;
        else if (!moveRight && transform.position.x < startPos.x - patrolDistance)
            moveRight = true;

        // Xoay sprite theo hướng di chuyển
        if (sr != null)
            sr.flipX = dir < 0;
    }

    void ChasePlayer()
    {
        // Tính hướng từ Golem đến Player (1 = Player bên phải, -1 = Player bên trái)
        float dir = (player.transform.position.x - transform.position.x) >= 0 ? 1f : -1f;
        
        // Chạy về phía Player
        rb.linearVelocity = new Vector2(dir * chaseSpeed, rb.linearVelocity.y);

        // Xoay sprite theo hướng đi
        if (sr != null)
            sr.flipX = dir < 0;
    }

    void TryAttack()
    {
        // Quay hướng về phía Player trước khi tấn công
        float dir = (player.transform.position.x - transform.position.x) >= 0 ? 1f : -1f;
        if (sr != null)
            sr.flipX = dir < 0;

        // Kiểm tra có thể tấn công không (không đang tấn công)
        if (!isAttacking)
        {
            // Golem chỉ có 1 loại tấn công (không cần attackIndex như Orc)
            // Bắt đầu trạng thái tấn công
            isAttacking = true;
            attackDurationTimer = attackDuration;
            hasDealtDamageThisAttack = false;
        }
    }

    void DealDamage()
    {
        // Kiểm tra Player còn tồn tại không
        if (player == null) return;

        // Tìm Player trong phạm vi tấn công bằng vòng tròn
        Collider2D hitPlayer = Physics2D.OverlapCircle(transform.position, attackRadius, LayerMask.GetMask("Player"));
        
        if (hitPlayer != null)
        {
            Debug.Log("Golem đánh trúng Player: " + hitPlayer.name);

            // Tính hướng knockback từ Golem đến Player (để đẩy Player ra)
            Vector2 knockbackDirection = (hitPlayer.transform.position - transform.position).normalized;

            // Gọi hàm nhận sát thương trên SoldierController
            SoldierController soldierController = hitPlayer.GetComponent<SoldierController>();
            if (soldierController != null)
                soldierController.TakeDamage(attackDamage, knockbackDirection);
        }
    }

    void DisableEnemyCollisions()
    {
        // Tắt collision giữa các Enemy với nhau để có thể xuyên qua được
        GolemController[] allGolems = FindObjectsByType<GolemController>(FindObjectsSortMode.None);
        foreach (GolemController otherGolem in allGolems)
        {
            // Không tắt collision với chính mình
            if (otherGolem != this)
            {
                Collider2D otherCollider = otherGolem.GetComponent<Collider2D>();
                // Nếu cả hai đều có collider - tắt collision
                if (otherCollider != null && golemCollider != null)
                    Physics2D.IgnoreCollision(golemCollider, otherCollider, true);
            }
        }

        // Tắt collision với Orc
        OrcController[] allOrcs = FindObjectsByType<OrcController>(FindObjectsSortMode.None);
        foreach (OrcController orc in allOrcs)
        {
            Collider2D orcCollider = orc.GetComponent<Collider2D>();
            if (orcCollider != null && golemCollider != null)
                Physics2D.IgnoreCollision(golemCollider, orcCollider, true);
        }
    }

    // Nhận sát thương từ Player - giảm máu
    public void TakeDamage(int damage)
    {
        // Nếu đã chết thì không chịu sát thương thêm
        if (isDeath) return;

        Debug.Log("Golem nhận " + damage + " sát thương");
        
        // Trừ máu
        currentHealth -= damage;
        Debug.Log("Golem còn " + currentHealth + " máu");

        // Đặt trạng thái bị đánh để hiển thị animation
        isHurting = true;
        hurtTimer = hurtDuration;

        // Kiểm tra nếu máu <= 0 thì chết
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Đặt trạng thái chết - sẽ bỏ qua tất cả logic hoạt động
        isDeath = true;
        isHurting = true;

        Debug.Log("Golem chết!");

        // Dừng di chuyển ngay lập tức
        rb.linearVelocity = Vector2.zero;

        // Set animation IsDeath cho Animator (lưu ý: chữ I viết hoa)
        if (animator != null)
            animator.SetBool("IsDeath", true);

        // Rơi vàng khi chết
        DropGold();

        // Xoá GameObject sau 0.5 giây để có thể hiển thị animation chết
        Destroy(gameObject, 0.5f);
    }

    void DropGold()
    {
        // Kiểm tra có prefab coin không
        if (goldCoinPrefab == null)
        {
            Debug.LogWarning("Gold Coin Prefab chưa được gán!");
            return;
        }

        // Tạo số lượng coin rơi
        for (int i = 0; i < goldDropAmount; i++)
        {
            // Tạo coin tại vị trí quái chết
            GameObject coin = Instantiate(goldCoinPrefab, transform.position, Quaternion.identity);
            
            // Lấy Rigidbody2D của coin để thêm lực rơi
            Rigidbody2D coinRB = coin.GetComponent<Rigidbody2D>();
            if (coinRB != null)
            {
                // Tạo hướng rơi ngẫu nhiên (trái hoặc phải)
                float randomX = Random.Range(-1f, 1f);
                Vector2 dropDirection = new Vector2(randomX, 1f).normalized;
                
                // Áp dụng lực rơi
                coinRB.linearVelocity = dropDirection * dropForce;
            }
            
            Debug.Log("Rơi 1 coin vàng!");
        }
    }

    void UpdateAnimation()
    {
        // Nếu không có Animator thì bỏ qua
        if (animator != null)
        {
            // Cập nhật trạng thái đi bộ - đúng khi di chuyển và không đang tấn công
            bool isWalking = Mathf.Abs(rb.linearVelocity.x) > 0.01f && !isAttacking && !isIdleAfterAttack;
            animator.SetBool("IsWalking", isWalking);
            
            // Cập nhật trạng thái tấn công (lưu ý: Animator có tên là IsAttackin không có 'g')
            animator.SetBool("IsAttacking", isAttacking);
            
            // Cập nhật trạng thái bị đánh
            animator.SetBool("IsHurting", isHurting);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Vẽ phạm vi tấn công
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
