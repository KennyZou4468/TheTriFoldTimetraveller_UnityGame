using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 将类名 Cash 改为 Enemy/Interactable 更规范，这里保留原名
public class Cash : MonoBehaviour
{
    Animator animator;
    SpriteRenderer sr;
    Rigidbody2D rb;

    // 移除了 health 相关的变量和属性

    [Header("Movement Settings")]
    public float moveSpeed = 1f;
    public float walkTimeMin = 1.5f;
    public float walkTimeMax = 3f;
    public float idleTimeMin = 1f;
    public float idleTimeMax = 2f;
    public LayerMask obstacleLayer;

    // 【新增】交互范围检测
    [Header("Interaction Settings")]
    public float interactionRange = 0.3f; // 玩家需要靠近的距离
    private Transform playerTransform;      // 存储玩家的Transform

    private Vector2 walkDirection;
    private bool isWalking = false;
    private float stuckCheckTimer = 0f;
    void Awake()
    {
        // 【新增/修改】将组件获取移到 Awake()
        // Awake 保证在 Start 之前执行，且在外部调用方法时组件已准备好。
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        // 【新增】尝试找到玩家（假设玩家标签为 "Player"）
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }

        //StartCoroutine(RandomWalk());
    }
    // Inside Cash.cs

    // 【新增】供 GameManager 调用，用于开始移动
    public void StartMovement()
    {
        StartCoroutine(RandomWalk());
    }

    // ... (RandomWalk 协程保持不变)
    void Update()
    {
        // 【新增】检测销毁输入
        CheckDestroyInput();
    }

    void FixedUpdate()
    {
        if (!isWalking) return;

        // 碰撞检测（略微简化）
        RaycastHit2D hit = Physics2D.Raycast(
            rb.position,
            walkDirection,
            moveSpeed * Time.fixedDeltaTime + 0.1f,
            obstacleLayer);

        if (hit.collider != null)
        {
            PickNewDirection();
        }

        // 移动
        Vector2 newPos = rb.position + walkDirection * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPos);

        // 左右翻转
        if (walkDirection.x != 0)
            sr.flipX = walkDirection.x < 0;

        // ------ 防止卡住强制换方向 ------
        stuckCheckTimer += Time.fixedDeltaTime;
        if (stuckCheckTimer > 1f) // 每1秒检测一次
        {
            // 使用速度检查卡住（注意：在 FixedUpdate 中用速度可能不精确，但作为辅助可以）
            if (rb.velocity.magnitude < 0.05f && walkDirection != Vector2.zero)
            {
                PickNewDirection();
            }
            stuckCheckTimer = 0f;
        }

        animator.SetBool("is_walking", true);
    }

    // 【新增】处理玩家输入销毁的函数
    void CheckDestroyInput()
    {
        if (GameManager3.Instance != null && !GameManager3.Instance.CanPlayerExit())
        {
            // 1. 检测 'J' 键是否按下
            if (Input.GetKeyDown(KeyCode.J))
            {
                // 2. 检查玩家是否在交互范围内 (推荐做法)
                if (playerTransform != null)
                {
                    float distance = Vector2.Distance(playerTransform.position, transform.position);

                    if (distance <= interactionRange)
                    {
                        // 在范围内，执行销毁逻辑
                        DestroyObject();
                    }
                }
                // 如果没有玩家Transform，但你坚持只要按J就销毁，则启用下一行
                // else { DestroyObject(); }
            }
        }
    }

    void DestroyObject()
    {
        // 1. 【关键】通知 Spawner 生成新的 Cash
        if (GameManager3.Instance != null)
        {
            GameManager3.Instance.AddScore(1); // 每次消灭加 1 分
        }
        if (CashSpawner.Instance != null)
        {
            CashSpawner.Instance.RespawnCash();
        }
        else
        {
            Debug.LogWarning("CashSpawner 未找到，无法重生 Cash.");
        }

        // 2. 销毁对象
        StopAllCoroutines();
        Destroy(gameObject);
    }

    IEnumerator RandomWalk()
    {
        while (true)
        {
            // ------ Idle ------
            isWalking = false;
            walkDirection = Vector2.zero;
            animator.SetBool("is_walking", false);
            yield return new WaitForSeconds(Random.Range(idleTimeMin, idleTimeMax));

            // ------ Walk ------
            isWalking = true;
            PickNewDirection();
            yield return new WaitForSeconds(Random.Range(walkTimeMin, walkTimeMax));
        }
    }

    void PickNewDirection()
    {
        // 随机方向
        walkDirection = Random.insideUnitCircle.normalized;

        // 尝试多几次找到无障碍方向
        int tries = 0;
        // 避免选择一个已经卡住的方向
        while (Physics2D.Raycast(rb.position, walkDirection, 0.2f, obstacleLayer) && tries < 8)
        {
            walkDirection = Random.insideUnitCircle.normalized;
            tries++;
        }

        // 重置卡住检测计时器
        stuckCheckTimer = 0f;
    }
}