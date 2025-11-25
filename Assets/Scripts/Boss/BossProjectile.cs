using UnityEngine;

/// Boss Projectile - Vien dan cua Boss
/// Tu dong bay ve phia truoc va gay sat thuong cho Player
public class BossProjectile : MonoBehaviour
{
    // ====================
    // CAI DAT CO BAN
    // ====================

    [Header("Thong So Dan")]
    [HideInInspector] public float speed;
    [HideInInspector] public int damage;
    [HideInInspector] public float lifetime;

    [Header("Hieu Ung No")]
    public GameObject explosionEffectPrefab;
    public float explosionLifetime = 1f;

    // ====================
    // BIEN KHAC
    // ====================

    private Vector2 direction;
    private Rigidbody2D rb;

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
        MoveProjectile();
    }

    // ====================
    // DI CHUYEN
    // ====================

    void MoveProjectile()
    {
        if (rb != null && direction != Vector2.zero)
        {
            rb.linearVelocity = direction * speed;
        }
    }

    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection.normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    // ====================
    // VA CHAM
    // ====================

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            DamagePlayer(collision);
            DestroyProjectile();
        }
        else if (collision.CompareTag("Ground"))
        {
            DestroyProjectile();
        }
    }

    void DamagePlayer(Collider2D playerCollider)
    {
        SoldierController player = playerCollider.GetComponent<SoldierController>();
        if (player != null)
        {
            Vector2 knockbackDirection = direction;
            player.TakeDamage(damage, knockbackDirection);
            Debug.Log("Vien dan trung Player!");
        }
    }

    void DestroyProjectile()
    {
        SpawnExplosionEffect();
        Destroy(gameObject);
    }

    void SpawnExplosionEffect()
    {
        if (explosionEffectPrefab != null)
        {
            GameObject explosion = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            Destroy(explosion, explosionLifetime);
            Debug.Log("Tao hieu ung no!");
        }
    }
}