using UnityEngine;

/// Player Arrow - Mui ten ban ra tu cung cua Soldier
/// Tu dong bay theo huong va gay sat thuong cho Enemy

public class PlayerArrow : MonoBehaviour
{
    // ====================
    // CAI DAT CO BAN
    // ====================

    [Header("Thong So Mui Ten")]
    public float speed = 25f;
    public float lifetime = 8f;

    // ====================
    // BIEN KHAC
    // ====================

    private Vector2 direction;
    private int damage;
    private Rigidbody2D rb;
    private bool hasHitEnemy;

    // ====================
    // KHOI TAO
    // ====================

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.gravityScale = 0;
            rb.bodyType = RigidbodyType2D.Dynamic;
        }
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void FixedUpdate()
    {
        MoveArrow();
    }

    // ====================
    // KHOI TAO THONG SO
    // ====================

    public void Initialize(Vector2 shootDirection, int arrowDamage)
    {
        direction = shootDirection.normalized;
        damage = arrowDamage;

        // Xoay mui ten theo huong ban
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    // ====================
    // DI CHUYEN
    // ====================

    void MoveArrow()
    {
        if (rb != null && direction != Vector2.zero)
        {
            rb.linearVelocity = direction * speed;
        }
    }

    // ====================
    // VA CHAM
    // ====================

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Chi gay sat thuong mot lan
        if (hasHitEnemy)
            return;

        if (collision.CompareTag("Enemy"))
        {
            DamageEnemy(collision);
            hasHitEnemy = true;
            Destroy(gameObject);
        }
    }

    void DamageEnemy(Collider2D enemyCollider)
    {
        Debug.Log("Mui ten trung: " + enemyCollider.name);

        // Goi TakeDamage bang reflection - khong can biet loai class
        var component = enemyCollider.GetComponent("BossController") 
            ?? enemyCollider.GetComponent("OrcController") 
            ?? enemyCollider.GetComponent("GolemController");

        if (component != null)
        {
            var method = component.GetType().GetMethod("TakeDamage");
            if (method != null)
            {
                method.Invoke(component, new object[] { damage });
                Debug.Log("Da gay " + damage + " sat thuong cho " + enemyCollider.name);
            }
        }
    }
}
