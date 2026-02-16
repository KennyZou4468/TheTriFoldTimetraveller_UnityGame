using UnityEngine;
using System.Collections; // 引用协程

public class DoorController : MonoBehaviour
{
    [Header("设置")]
    public float autoCloseTime = 5f; // 自动关门时间 (秒)

    [Header("状态 (只读)")]
    public bool isOpen = false;
    private bool playerInRange = false;

    // 组件引用
    private SpriteRenderer sr;
    private BoxCollider2D solidCollider;
    public GameObject tipUI;

    // 计时器协程引用 (用来在中途取消倒计时)
    private Coroutine autoCloseCoroutine;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        // 自动找挡路用的实心碰撞体
        BoxCollider2D[] allBoxCols = GetComponents<BoxCollider2D>();
        foreach (var c in allBoxCols) { if (!c.isTrigger) solidCollider = c; }

        if (tipUI != null) tipUI.SetActive(false);
        ApplyDoorState(); // 初始化
    }

    void Update()
    {
        // 按 E 键交互
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (isOpen)
            {
                // 如果门是开的 -> 手动关门
                CloseDoor();
            }
            else
            {
                // 如果门是关的 -> 手动开门
                OpenDoor();
            }
        }
    }

    // --- 开门逻辑 ---
    void OpenDoor()
    {
        if (isOpen) return; // 已经是开的就别开了

        isOpen = true;
        ApplyDoorState();
        Debug.Log("🚪 门开了，将在 " + autoCloseTime + " 秒后自动关闭。");

        // 启动自动关门倒计时
        if (autoCloseCoroutine != null) StopCoroutine(autoCloseCoroutine); // 防止重复启动
        autoCloseCoroutine = StartCoroutine(AutoCloseRoutine());
    }

    // --- 关门逻辑 ---
    void CloseDoor()
    {
        if (!isOpen) return; // 已经是关的就别关了

        isOpen = false;
        ApplyDoorState();
        Debug.Log("🚪 门已关闭。");

        // 如果有关门动作，就取消还在跑的倒计时 (防止关门后倒计时还在跑)
        if (autoCloseCoroutine != null) StopCoroutine(autoCloseCoroutine);
    }

    // --- 自动关门倒计时 ---
    IEnumerator AutoCloseRoutine()
    {
        yield return new WaitForSeconds(autoCloseTime);

        // 时间到，检测门是否还开着
        if (isOpen)
        {
            Debug.Log("⏰ 时间到，自动关门！");
            CloseDoor();
        }
    }

    // --- 应用视觉和物理状态 ---
    void ApplyDoorState()
    {
        if (isOpen)
        {
            if (sr != null) sr.enabled = false;
            if (solidCollider != null) solidCollider.enabled = false;
        }
        else
        {
            if (sr != null) sr.enabled = true;
            if (solidCollider != null) solidCollider.enabled = true;
        }
    }

    // --- 触发检测 (单物体版) ---
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (tipUI != null) tipUI.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (tipUI != null) tipUI.SetActive(false);
        }
    }
}