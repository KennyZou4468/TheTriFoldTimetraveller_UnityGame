using UnityEngine;

public class RunnerAI : MonoBehaviour
{
    [Header("设置")]
    public float runSpeed = 5f;
    public Transform escapeTarget;    // 目标点：左边墙角
    public GameObject holePrefab;     // 【洞】的预制体
    public GameObject moneyPrefab;    // 【钱】的预制体（死后掉落）

    private Rigidbody2D rb;
    private bool isRunning = false;
    private bool hasDugHole = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // 由房间管理器激活时调用
    public void StartPanic()
    {
        isRunning = true;
    }

    void Update()
    {
        if (isRunning && !hasDugHole && escapeTarget != null)
        {
            // 1. 向左跑
            Vector2 dir = (escapeTarget.position - transform.position).normalized;
            rb.velocity = dir * runSpeed;

            // 2. 到达目的地（墙角）
            if (Vector2.Distance(transform.position, escapeTarget.position) < 0.5f)
            {
                DigHoleAndDespawn();
            }
        }
    }

    void DigHoleAndDespawn()
    {
        hasDugHole = true;
        rb.velocity = Vector2.zero;

        // 关键分歧点：生成黑洞
        if (holePrefab != null)
        {
            Instantiate(holePrefab, transform.position, Quaternion.identity);
            Debug.Log("Jonny挖了个洞逃走了...");
        }

        Destroy(gameObject); // 自己消失
    }

    // 被玩家打死时调用（由 Health 脚本触发）
    void OnDestroy()
    {
        // 如果还没挖洞就被杀了
        if (!hasDugHole && gameObject.scene.isLoaded)
        {
            Debug.Log("博士：漂亮的击杀！");
            // 掉钱
            if (moneyPrefab != null) Instantiate(moneyPrefab, transform.position, Quaternion.identity);
            // 此时因为对象被销毁，DigHoleAndDespawn永远不会被调用，洞也永远不会生成 -> 只能打普通结局
        }
    }
}
