using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class last_day_door : MonoBehaviour
{
    public GameObject interactUI; // 显示提示用的 UI（例如按E）
    private bool playerInRange = false;

    [Header("Door Settings")]
    public int doorID = 1; // 门的编号 (1, 2, 或 3)

    private RealityBankManager manager; // 引用 RealityBankManager

    void Start()
    {
        // 确保 Interact UI 初始隐藏
        if (interactUI != null)
            interactUI.SetActive(false);

        // 查找场景中的管理器
        manager = FindObjectOfType<RealityBankManager>();
        if (manager == null)
        {
            Debug.LogError("RealityBankDoor 无法找到场景中的 RealityBankManager！");
        }
    }

    void Update()
    {
        // 当玩家在范围内且按下 E 键时触发互动
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactUI != null)
                interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactUI != null)
                interactUI.SetActive(false);
        }
    }

    void Interact()
    {
        if (manager == null) return;

        // 【核心判断】：检查倒计时是否正在运行 (即是否允许选择门)
        // 我们通过 RealityBankManager 暴露一个状态来检查是否处于逃跑阶段。
        // 假设 RealityBankManager 有一个公共属性 IsEscapeTimeRunning。

        // ⚠️ 注意：我们需要在 RealityBankManager 中添加一个公共属性/方法来检查状态
        if (manager.isEscapeTimeRunning) // 假设 RealityBankManager 中有这个属性
        {
            Debug.Log($"玩家选择了门 {this.doorID}。通知管理器。");

            // 1. 通知 RealityBankManager 玩家选择了这个门
            manager.OnDoorSelected(this.doorID);
            LockPlayerMovement();
            // 2. 禁用门互动
            playerInRange = false;
            if (interactUI != null) interactUI.SetActive(false);

            // 3. 禁用 Collider，防止再次互动
            Collider2D col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

        }
        else
        {
            Debug.Log("🔒 逃跑阶段未开始或已结束，无法选择门。");
        }
    }
    private void LockPlayerMovement()
    {

        Player1Controller player = FindObjectOfType<Player1Controller>();
        if (player != null)
        {
            player.LockMovement();
            Debug.Log("🔑 玩家移动已锁定。");
        }
        else
        {
            Debug.LogWarning("无法找到 Player1Controller 来锁定移动。");
        }
    }
}
