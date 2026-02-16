using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Boss 属性")]
    public bool startInvincible = true; // 初始是否无敌

    [Header("攻击设置 (扇形散弹)")]
    public GameObject bulletPrefab;     // 子弹Prefab
    public Transform firePoint;         // 发射点
    public float fireRate = 1.5f;       // 攻击间隔
    public int pelletCount = 5;         // 一次发几颗子弹
    public float spreadAngle = 60f;     // 扇形角度 (例如60度)

    private Health myHealth;
    private float nextFire;

    void Start()
    {
        myHealth = GetComponent<Health>();
        nextFire = Time.time + 1f; // 进场后延迟1秒再开火

        // --- 核心逻辑：无敌判定 ---
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.hasDetermination)
            {
                // 有勋章：解除无敌
                if (myHealth) myHealth.isInvincible = false;
                Debug.Log("Boss: 居然有人能伤到我？(无敌解除)");
            }
            else
            {
                // 没勋章：开启无敌
                if (myHealth) myHealth.isInvincible = true;
                Debug.Log("Boss: 凡人，你的攻击无效。(无敌开启)");
            }
        }
        if (BossHUD.Instance != null && myHealth != null)
        {
            BossHUD.Instance.ShowBossHealth(myHealth.currentHealth, myHealth.maxHealth);
        }
        Invoke("ShowHealthBar", 0.1f);
    }

    void Update()
    {
        // 自动倒计时攻击
        if (Time.time >= nextFire)
        {
            FireScatterShot();
            nextFire = Time.time + fireRate;
        }
    }

    // 💥 扇形散弹发射逻辑
    void FireScatterShot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        // 计算每颗子弹的角度间隔
        float angleStep = spreadAngle / Mathf.Max(1, pelletCount - 1);

        // 设定起始角度：这里假设Boss在右边，默认朝左发射(180度)
        // 如果你的Boss朝向不同，请修改 baseAngle (0=右, 90=上, 180=左, 270=下)
        float baseAngle = 180f;
        float startAngle = baseAngle - (spreadAngle / 2f);

        for (int i = 0; i < pelletCount; i++)
        {
            float currentAngle = startAngle + (i * angleStep);

            // 将角度转换为方向向量 (数学魔法)
            float rad = currentAngle * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

            // 生成子弹
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

            // 10 表示 Boss 伤害，你可以根据需要修改
            bullet.GetComponent<Bullet>().Setup(dir, BulletType.BossBullet, 10);
        }
    }

    // 给外部调用：勋章脚本调用此方法
    public void BreakInvincibility()
    {
        if (myHealth)
        {
            myHealth.isInvincible = false;
            Debug.Log("💥 剧情触发：Boss护盾破碎！");
        }
    }
    void ShowHealthBar()
    {
        if (BossHUD.Instance != null && myHealth != null)
        {
            BossHUD.Instance.ShowBossHealth(myHealth.currentHealth, myHealth.maxHealth);
        }
        else
        {
            Debug.LogError("BossUI 还是没找到！");
        }
    }
}
