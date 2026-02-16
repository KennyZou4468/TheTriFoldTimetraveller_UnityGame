using UnityEngine;

public class LaserScan : MonoBehaviour
{
    [Header("扫描设置")]
    public float speed = 5f;
    public float startX = -10f; // 走廊左边
    public float endX = 10f;    // 走廊右边
    public int damage = 9999;   // 触之即死

    private int direction = 1; // 1向右，-1向左

    void Update()
    {
        // 来回移动
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);

        // 碰到边界反弹
        if (transform.position.x >= endX) direction = -1;
        else if (transform.position.x <= startX) direction = 1;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 检查玩家是否躲在掩体后面 (这需要一点物理射线检测，或者简单点：)
            // 只要碰到 Trigger 就死，玩家必须手动绕着墙走
            Debug.Log("被 MRI 激光扫过，意识瓦解！");
            other.GetComponent<Health>()?.TakeDamage(damage);
        }
    }
}
