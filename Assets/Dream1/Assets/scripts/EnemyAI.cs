using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("移动设置")]
    public float speed = 2f;          // 移动速度
    public float stopDistance = 3f;   // 离玩家多远开始停下攻击
    public float chaseRange = 8f;     // 玩家进入多大范围开始追击

    [Header("攻击设置")]
    public GameObject bulletPrefab;   // 怪物子弹
    public Transform firePoint;       // 发射点
    public float fireRate = 1.5f;     // 几秒打一发

    private Transform player;
    private float nextFireTime = 0f;
    private SpriteRenderer sp;        // 用来翻转图片朝向

    // 状态标记
    private bool isChasing = false;

    void Start()
    {
        // 自动寻找场景里的玩家
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        sp = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (player == null) return; // 玩家如果死了或者没找到，就不动

        float distance = Vector2.Distance(transform.position, player.position);

        // 1. 状态检测：如果玩家进入追击范围，开始追
        if (distance < chaseRange)
        {
            isChasing = true;
        }

        // 如果你在追击状态
        if (isChasing)
        {
            // 2. 面朝向处理 (简单的左右翻转)
            if (player.position.x < transform.position.x)
                sp.flipX = true; // 玩家在左边，翻转
            else
                sp.flipX = false; // 玩家在右边，正常

            // 3. 移动逻辑
            if (distance > stopDistance)
            {
                // 离得远，走过去
                transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
            }
            else if (distance <= stopDistance)
            {
                // 离得近了，停下，开火
                // (这里已经是停止移动了，因为 MoveTowards 不执行)

                // 4. 攻击逻辑
                if (Time.time >= nextFireTime)
                {
                    Shoot();
                    nextFireTime = Time.time + fireRate;
                }
            }
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        // 生成子弹
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // 计算子弹飞向玩家的方向
        Vector2 dir = (player.position - firePoint.position).normalized;

        // 初始化子弹 (设为 BossBullet 类型，反正都是敌对阵营，能打玩家就行)
        // 这里的 1 是伤害值，你可以改成 public 变量配置
        bullet.GetComponent<Bullet>().Setup(dir, BulletType.BossBullet);
    }

    // 为了调试方便，在编辑器里画个圈圈显示攻击范围
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange); // 警戒范围圆圈

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopDistance); // 攻击范围圆圈
    }
}
