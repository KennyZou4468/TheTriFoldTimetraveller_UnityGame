using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("组件引用 (必填)")]
    public Animator legsAnimator;   // 拖入 Legs 的 Animator
    public Transform bodyTransform; // 拖入 Body
    public Transform legsTransform; // 拖入 Legs

    [Header("属性设置")]
    public float speed = 5f;
    public int playerDamage = 1;

    [Header("射击设置")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.2f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 facingDir = Vector2.right;
    private float nextFire;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        bool isMoving = moveInput.sqrMagnitude > 0.01f;

        // 🛑 所有的秘密都在这里，看控制台输出什么！
        if (isMoving)
        {
            Debug.Log($"[调试] 正在移动! 输入: {moveInput}, 理论上应该 SetBool('IsRunning', true)");
        }
        else
        {
            // 如果你没动，这行会疯狂刷屏，可以暂时注释掉
            // Debug.Log("[调试] 停止中... SetBool('IsRunning', false)");
        }

        // 设置动画
        if (legsAnimator != null)
        {
            legsAnimator.SetBool("IsRunning", isMoving);

            // 🛑 检查动画机是否接收到了
            bool animatorVal = legsAnimator.GetBool("IsRunning");
            if (isMoving && !animatorVal)
            {
                Debug.Log("❌ 严重错误：代码尝试设为 true，但 Animator 里的值还是 false！");
            }
        }
        else
        {
            Debug.Log("❌ 严重错误：legsAnimator 是空的！你没拖拽赋值！");
        }

        // 4. 旋转与朝向逻辑
        if (isMoving)
        {
            RotateCharacter();
            facingDir = moveInput.normalized;
        }

        // 5. 射击
        if (Input.GetKey(KeyCode.Space) && Time.time >= nextFire)
        {
            Shoot();
            nextFire = Time.time + fireRate;
        }
    }

    void FixedUpdate()
    {
        // 物理移动
        if (rb != null)
        {
            rb.MovePosition(rb.position + moveInput.normalized * speed * Time.fixedDeltaTime);
        }
    }

    // 🟢 旋转逻辑：让人物朝向移动方向
    void RotateCharacter()
    {
        if (bodyTransform == null || legsTransform == null) return;

        // 计算角度 (Atan2 返回弧度，转为角度)
        float angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg;

        // 创建旋转 (绕 Z 轴转)
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

        // 应用旋转
        bodyTransform.rotation = targetRotation;
        legsTransform.rotation = targetRotation;
    }

    void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject b = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

            // 这里的 damage 传进去
            b.GetComponent<Bullet>().Setup(facingDir, BulletType.PlayerBullet, playerDamage);
        }
    }
}