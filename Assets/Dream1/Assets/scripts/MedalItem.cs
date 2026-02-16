using UnityEngine;

public class MedalItem : MonoBehaviour
{
    [Header("奖励数值")]
    public int damageBuff = 50; // 加多少攻
    public float invincibleTime = 10f; // 无敌多少秒

    void OnTriggerEnter2D(Collider2D other)
    {
        // 只有碰到玩家才触发
        if (other.CompareTag("Player"))
        {
            Debug.Log("✨ 捡到奖牌！");

            // 1. 加攻击力
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.playerDamage += damageBuff;
                if (HUDManager.Instance != null) HUDManager.Instance.UpdateAttack(pc.playerDamage);
            }

            // 2. 开启无敌 (调用刚才写的那个变身方法)
            Health health = other.GetComponent<Health>();
            if (health != null)
            {
                health.StartTemporaryInvincibility(invincibleTime);
            }

            // 3. 解除 Boss 无敌
            if (GameManager.Instance != null) GameManager.Instance.hasDetermination = true;
            BossController boss = FindObjectOfType<BossController>();
            if (boss != null) boss.BreakInvincibility();

            // 4. 弹窗提示
            if (SystemNotify.Instance != null)
            {
                SystemNotify.Instance.ShowNotification($"\nATK+{damageBuff}， invincible {invincibleTime} S!", 4f);
            }

            // 5. 销毁自己
            Destroy(gameObject);
        }
    }
}
