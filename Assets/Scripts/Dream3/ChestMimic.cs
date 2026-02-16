using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestMimic : MonoBehaviour
{
    Animator animator;
    SpriteRenderer sr;
    Rigidbody2D rb;

    [Header("Mimic Settings")]
    [Tooltip("碰到玩家时扣除的分数")]
    public int scorePenaltyOnTouch = -2;

    [Header("Movement Settings")]
    public float moveSpeed = 0.5f; // 宝箱怪可以移动慢一点
    public float walkTimeMin = 2f;
    public float walkTimeMax = 4f;
    public float idleTimeMin = 1.5f;
    public float idleTimeMax = 3f;
    public LayerMask obstacleLayer;

    private Vector2 walkDirection;
    private bool isWalking = false;
    private float stuckCheckTimer = 0f;

    private bool hasCollidedRecently = false;
    private const float collisionCooldown = 0.5f; // 避免在接触时连续扣分

    void Awake()
    {
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // 宝箱怪默认为站立状态
        if (animator != null)
        {
            animator.SetBool("is_walking", false);
        }
    }

    // 供 Spawner/GameManager 调用，开始移动
    public void StartMovement()
    {
        StartCoroutine(RandomWalk());
    }

    void FixedUpdate()
    {
        if (!isWalking)
        {
            animator.SetBool("is_walking", false);
            return;
        }

        // 碰撞检测（略微简化）
        RaycastHit2D hit = Physics2D.Raycast(
            rb.position,
            walkDirection,
            moveSpeed * Time.fixedDeltaTime + 0.1f,
            obstacleLayer);

        if (hit.collider != null)
        {
            // 如果撞墙，尝试换个方向
            PickNewDirection();
        }

        // 移动
        Vector2 newPos = rb.position + walkDirection * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPos);

        // 左右翻转 (假设 flipX=false 时精灵面朝左)
        if (walkDirection.x != 0)
        {
            sr.flipX = walkDirection.x < 0; // 当向左 (<0) 时，flipX=true (翻转到面向左)
        }

        // ------ 防止卡住强制换方向 ------
        stuckCheckTimer += Time.fixedDeltaTime;
        if (stuckCheckTimer > 1f) // 每1秒检测一次
        {
            if (rb.velocity.magnitude < 0.05f && walkDirection != Vector2.zero)
            {
                PickNewDirection();
            }
            stuckCheckTimer = 0f;
        }

        animator.SetBool("is_walking", true);
    }

    /// <summary>
    /// 循环切换 空闲(Idle) 和 行走(Walk) 状态。
    /// </summary>
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

    /// <summary>
    /// 选取一个新的随机方向，并确保该方向没有即时障碍。
    /// </summary>
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


    void OnTriggerEnter2D(Collider2D other)
    {
        // 检查碰撞到的对象是否为玩家
        if (other.CompareTag("Player") && !hasCollidedRecently)
        {
            Debug.Log("宝箱怪接触玩家 (Trigger)，扣分！");

            if (GameManager3.Instance != null)
            {
                // 传入负值，实现扣分
                GameManager3.Instance.AddScore(scorePenaltyOnTouch);
            }

            // 启动冷却
            StartCoroutine(CollisionCooldown());
        }
    }

    IEnumerator CollisionCooldown()
    {
        hasCollidedRecently = true;
        yield return new WaitForSeconds(collisionCooldown);
        hasCollidedRecently = false;
    }
}