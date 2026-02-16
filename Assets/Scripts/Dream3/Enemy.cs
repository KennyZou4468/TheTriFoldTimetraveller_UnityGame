using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Animator animator;
    SpriteRenderer sr;
    Rigidbody2D rb;

    public float health = 1;
    public float moveSpeed = 1.5f;

    public float walkTimeMin = 1.5f;
    public float walkTimeMax = 3f;
    public float idleTimeMin = 1f;
    public float idleTimeMax = 2f;

    public LayerMask obstacleLayer;

    private Vector2 walkDirection;
    private bool isWalking = false;
    private float stuckCheckTimer = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        StartCoroutine(RandomWalk());
    }

    void FixedUpdate()
    {
        if (!isWalking) return;

        // 碰到障碍物自动换方向
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
            if (rb.velocity.magnitude < 0.05f)  // 基本没动，算卡住
            {
                PickNewDirection();
            }
            stuckCheckTimer = 0f;
        }

        animator.SetBool("is_walking", true);
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
        while (Physics2D.Raycast(rb.position, walkDirection, 0.2f, obstacleLayer) && tries < 8)
        {
            walkDirection = Random.insideUnitCircle.normalized;
            tries++;
        }
    }

    public float Health
    {
        get { return health; }
        set
        {
            health = value;
            if (health <= 0) Defeated();
        }
    }

    public void Defeated()
    {
        animator.SetTrigger("Defeated");
    }

    public void removeEnemy()
    {
        Destroy(gameObject);
    }
}