using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Health : MonoBehaviour
{
    [Header("生命值设置")]
    public int maxHealth = 10;
    public int currentHealth;
    public bool isInvincible = false;
    public Slider healthBar;

    [Header("特效与掉落")]
    public GameObject deathEffect;
    public GameObject moneyDropPrefab;
    [Range(0, 100)] public int dropChance = 50;

    // 防止死亡逻辑被连续触发两次
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible || isDead) return; // 如果无敌或已死，不扣血

        currentHealth -= damage;

        // 🔴 关键修复：确保血量最低是 0，不能是负数
        if (currentHealth < 0) currentHealth = 0;

        // 更新 UI
        UpdateHealthBar();
        if (gameObject.CompareTag("Player") && HUDManager.Instance != null)
        {
            HUDManager.Instance.UpdateHealth(currentHealth, maxHealth);
        }

        // 检查死亡
        if (currentHealth == 0)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        if (healthBar != null) healthBar.value = (float)currentHealth / maxHealth;
    }

    // --- 临时无敌逻辑 ---
    public void StartTemporaryInvincibility(float duration)
    {
        StartCoroutine(InvincibleRoutine(duration));
    }

    private IEnumerator InvincibleRoutine(float duration)
    {
        isInvincible = true;
        Debug.Log("🛡️ 开启无敌！");

        // 变色逻辑
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color oldColor = Color.white;
        if (sr != null) { oldColor = sr.color; sr.color = new Color(1f, 0.9f, 0.2f); }

        yield return new WaitForSeconds(duration);

        isInvincible = false;
        if (sr != null) sr.color = oldColor;
        Debug.Log("🛡️ 无敌结束！");
    }

    // --- 死亡逻辑 ---
    void Die()
    {
        if (isDead) return; // 双重保险：不会死两次
        isDead = true;

        if (deathEffect != null) Instantiate(deathEffect, transform.position, Quaternion.identity);

        if (moneyDropPrefab != null && Random.Range(0, 100) < dropChance)
        {
            Instantiate(moneyDropPrefab, transform.position, Quaternion.identity);
        }

        // 🔴 触发结局
        if (gameObject.CompareTag("Player"))
        {
            Debug.Log("玩家生命归零 -> 触发梦境死亡结局");
            GameManager.Instance?.TriggerDreamDeath();
        }
        else if (gameObject.CompareTag("Boss"))
        {
            Debug.Log("Boss die");
            GameManager.Instance?.TriggerVictoryEnding();
        }

        Destroy(gameObject);
    }
}
