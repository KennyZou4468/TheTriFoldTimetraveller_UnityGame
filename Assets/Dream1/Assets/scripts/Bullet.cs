using UnityEngine;

public enum BulletType
{
    PlayerBullet,
    BossBullet
}

public class Bullet : MonoBehaviour
{
    [Header("子弹属性")]
    public BulletType type;
    public float speed = 10f;
    public int damage = 1; // 🔴 这颗子弹通过 Setup 接收到的最终伤害

    private Vector2 moveDirection;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // 🔴 修改后的 Setup，接收伤害值
    // dmg = 1 是默认值，如果有小怪代码忘了传伤害，默认就造成1点伤害
    public void Setup(Vector2 direction, BulletType bulletType, int dmg = 1)
    {
        moveDirection = direction.normalized;
        type = bulletType;
        this.damage = dmg; // 接收发射者传来的伤害

        // 使用 Velocity 移动防止穿墙 (如果刚体是 Dynamic)
        if (rb != null)
        {
            rb.velocity = moveDirection * speed;
        }

        // 5秒自动销毁
        Destroy(gameObject, 5f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 1. 撞墙
        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
            return;
        }

        // 2. 伤害逻辑
        Health targetHealth = other.GetComponent<Health>();

        if (targetHealth != null)
        {
            // 情况A: 玩家子弹 打中 Boss 或 Enemy
            if (type == BulletType.PlayerBullet && (other.CompareTag("Boss") || other.CompareTag("Enemy")))
            {
                targetHealth.TakeDamage(damage); // 造成伤害
                Destroy(gameObject);
            }
            // 情况B: Boss子弹 打中 玩家
            else if (type == BulletType.BossBullet && other.CompareTag("Player"))
            {
                targetHealth.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}
