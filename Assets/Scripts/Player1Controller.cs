using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // 记得加这一行

public class Player1Controller : MonoBehaviour
{

    [Header("Movement")]
    Vector2 movementinput;
    private bool isInDream3 = false; // 【新增】用于记录是否在 Dream3 场景
    SpriteRenderer spriteRenderer;
    public float moveSpeed = 1f;
    public float sprintMultiplier = 2f; // Shift加速倍数

    [Header("Score Penalty")]
    public float baseMoveSpeed = 1f;      // 玩家初始（0分时）的速度
    public float speedPenaltyPerPoint = 0.01f; // 每增加 1 分，速度减少多少 (例如 0.01f)
    public float minMoveSpeed = 0.2f;      // 玩家速度的下限

    public float collisionOffset = 0.01f;
    public ContactFilter2D movementFilter;
    List<RaycastHit2D> castCollision = new List<RaycastHit2D>();
    Rigidbody2D rb;
    Animator animator;
    bool canMove = true;

    [Header("Sprint & Stamina")]
    public bool allowSprint = false;   // Inspector勾选是否允许加速
    public float maxStamina = 100f;
    public float stamina;
    public float staminaDecreaseRate = 40f; // 每秒消耗
    public float staminaRecoverRate = 20f;  // 每秒恢复
    public float walkStaminaRecoverRate = 10f; // <-- 【新增】走路时的恢复速率 (设为10)
    private bool isSprinting = false;
    // 【新增】：体力恢复区域的倍数
    private float staminaRechargeMultiplier = 1f;

    // 【新增】：外部调用方法
    public void SetRechargeMultiplier(float multiplier)
    {
        staminaRechargeMultiplier = multiplier;
    }


    public Slider staminaSlider; // Inspector拖入你的 StaminaBar
    public Image fillImage;
    void UpdateStaminaBar()
    {
        if (staminaSlider == null) return;

        // 设置填充量（左端固定）
        float fillAmount = stamina / maxStamina;
        RectTransform rt = staminaSlider.GetComponent<RectTransform>();
        rt.localScale = new Vector3(fillAmount, 1f, 1f);

        // 根据体力设置颜色
        if (fillImage != null)
        {
            if (stamina >= 60f)
                fillImage.color = Color.green;
            else if (stamina >= 30f)
                fillImage.color = Color.yellow;
            else
                fillImage.color = Color.red;
        }
        else
        {
            Debug.LogError("can not find image");
        }
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        stamina = maxStamina;
        moveSpeed = baseMoveSpeed;
        if (staminaSlider != null)
        {
            staminaSlider.maxValue = maxStamina;
            staminaSlider.value = stamina;
        }

        if (SceneManager.GetActiveScene().name == "Dream3")
        {
            allowSprint = true;
            isInDream3 = true; // 标记不处于 Dream3
            // 【关键】在 Dream3 场景中，默认锁定移动，等待 GameManager 的 StartGame() 指令。
            LockMovement();
        }
        else
        {
            allowSprint = false;
            isInDream3 = false; // 标记不处于 Dream3
            // 【关键】在其他场景中（如 Room），玩家可以立即移动。
            UnlockMovement();
        }
    }
    void FixedUpdate()
    {
        if (!canMove)
        {
            animator.SetBool("is_walking", false);
            return;
        }
        // ===================================
        // 【新增】得分速度惩罚计算
        // ===================================
        float penalty = 0f;
        int currentScore = 0;
        // 尝试获取分数 (假设 GameManager 是单例)
        if (GameManager3.Instance != null)
        {
            currentScore = GameManager3.Instance.GetScore();
        }

        // 计算惩罚值: 惩罚 = 得分 * 每分惩罚值
        penalty = currentScore * speedPenaltyPerPoint;

        // 计算最终基础速度: 初始速度 - 惩罚
        float penalizedSpeed = baseMoveSpeed - penalty;

        // 限制速度下限
        moveSpeed = Mathf.Max(penalizedSpeed, minMoveSpeed);

        Vector2 currentDirection = movementinput;
        float currentSpeed = moveSpeed; // 默认速度

        // 在每一帧开始时，默认设置为不冲刺
        isSprinting = false;

        // 默认恢复速率设为0
        float currentRecoverRate = 0f;

        // 判断是否允许冲刺，且玩家正在移动
        if (allowSprint && movementinput != Vector2.zero)
        {
            // ===================================
            // 1. 冲刺和消耗体力 (当在移动时)
            // ===================================
            if (Keyboard.current.leftShiftKey.isPressed && stamina > 0f)
            {
                // A. 冲刺：消耗体力
                isSprinting = true;
                currentSpeed *= sprintMultiplier;
                stamina -= staminaDecreaseRate * Time.fixedDeltaTime;
                stamina = Mathf.Max(stamina, 0f);
                if (stamina <= 0f)
                {
                    isSprinting = false;
                    currentSpeed = moveSpeed; // 立即将速度降回走路速度
                }
            }
            else
            {
                // B. 走路：慢恢复 (移动但不冲刺)
                currentRecoverRate = walkStaminaRecoverRate; // 走路恢复速率 10
            }
        }
        else if (allowSprint && movementinput == Vector2.zero)
        {
            // ===================================
            // 2. 静止：快恢复 (站着不动)
            // ===================================
            currentRecoverRate = staminaRecoverRate; // 站立恢复速率 20
        }
        // ===================================
        // 3. 应用体力恢复 (在循环外统一处理)
        // ===================================
        if (currentRecoverRate > 0f)
        {
            // 【核心修改】：应用加速恢复倍数
            float finalRecoveryRate = currentRecoverRate * staminaRechargeMultiplier;

            stamina += finalRecoveryRate * Time.fixedDeltaTime;
            stamina = Mathf.Min(stamina, maxStamina);
        }
        // ===================================


        // 更新体力条 (保持不变)
        if (staminaSlider != null)
            staminaSlider.value = stamina;

        // 尝试移动 (保持不变)
        bool moved = false;
        if (movementinput != Vector2.zero)
        {
            moved = TryMove(currentDirection, currentSpeed);
            if (!moved)
            {
                moved = TryMove(new Vector2(currentDirection.x, 0), currentSpeed);
                if (!moved)
                    moved = TryMove(new Vector2(0, currentDirection.y), currentSpeed);
            }
        }
        UpdateStaminaBar();
        animator.SetBool("is_walking", moved);

        if (movementinput.x != 0)
        {
            if (isInDream3)
            {
                // 在 Dream3 中：反转逻辑 (x > 0 翻转，x < 0 不翻转)
                if (movementinput.x < 0)
                    spriteRenderer.flipX = true;  // 向左移动，精灵不翻转 (面向左) -> **FlipX = true**
                else if (movementinput.x > 0)
                    spriteRenderer.flipX = false; // 向右移动，精灵翻转 (面向右) -> **FlipX = false**
            }
            else
            {
                // 在其他场景中：标准逻辑 (x > 0 不翻转，x < 0 翻转)
                if (movementinput.x < 0)
                    spriteRenderer.flipX = false; // 向左移动，精灵不翻转 (面向左)
                else if (movementinput.x > 0)
                    spriteRenderer.flipX = true;  // 向右移动，精灵翻转 (面向右)
            }
        }
    }
    private bool TryMove(Vector2 direction, float speed)
    {
        if (direction != Vector2.zero)
        {
            int count = rb.Cast(
                direction,
                movementFilter,
                castCollision,
                speed * Time.fixedDeltaTime + collisionOffset
            );

            if (count == 0)
            {
                rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
                return true;
            }
        }
        return false;
    }

    void OnMove(InputValue movementValue)
    {
        movementinput = movementValue.Get<Vector2>();
    }

    public void LockMovement()
    {
        canMove = false;
    }

    public void UnlockMovement()
    {
        canMove = true;
    }

    // 可以添加获取体力的接口
    public float GetStamina()
    {
        return stamina;
    }

    public bool IsSprinting()
    {
        return isSprinting;
    }
}