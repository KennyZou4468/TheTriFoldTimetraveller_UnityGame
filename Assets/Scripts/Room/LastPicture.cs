using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastPicture : MonoBehaviour
{
    [Header("UI 面板引用")]
    [Tooltip("按下 'E' 键要展示的 UI Panel 对象")]
    public GameObject interactionPanel;

    [Header("交互设置")]
    [Tooltip("玩家与道具的最大交互距离")]
    public float interactionRange = 0.3f;
    public GameObject interactUI; // 显示提示用的 UI（例如按E）


    // 存储玩家的 Transform，用于计算距离
    private Transform playerTransform;

    // 缓存当前是否在交互范围内
    private bool isInRange = false;

    void Start()
    {
        // 尝试找到玩家对象（假设玩家标签为 "Player"）
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
        else
        {
            Debug.LogError("InteractableItem: 场景中未找到 Tag 为 'Player' 的对象！");
        }
        GameObject canvasObj = GameObject.FindWithTag("Ending");
        if (canvasObj != null)
        {
            Debug.Log("find parent panel");
            // 查找 Canvas 的非活动子对象 Panel
            Transform panelTransform = canvasObj.transform.Find("DeserterEnding-disableisable");
            if (panelTransform != null)
            {
                Debug.Log("find panel");
                interactionPanel = panelTransform.gameObject;
                // ... 继续你的逻辑
            }
        }
        // 确保 Panel 在开始时是隐藏的
        if (interactionPanel != null)
        {
            interactionPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (playerTransform == null || interactionPanel == null) return;

        // 1. 检查是否在交互范围内
        float distance = Vector2.Distance(playerTransform.position, transform.position);

        bool currentlyInRange = (distance <= interactionRange);

        // 2. 处理交互输入 (如果处于范围内)
        if (currentlyInRange)
        {
            interactUI.SetActive(true);

            // ❗ 注意：使用新的 Input System 需导入 using UnityEngine.InputSystem;
            // 并在项目设置中启用 'Input System Package'。

            // 使用 Legacy Input System (旧输入系统)
            if (Input.GetKeyDown(KeyCode.E))
            {
                // 切换面板的显示状态
                TogglePanel();
            }
        }
        else
        {
            interactUI.SetActive(false);

        }
        // 更新范围状态
        isInRange = currentlyInRange;
    }

    /// <summary>
    /// 切换交互面板的显示状态。
    /// </summary>
    public void TogglePanel()
    {
        if (interactionPanel != null)
        {
            bool isActive = interactionPanel.activeSelf;
            interactionPanel.SetActive(!isActive);

            // 可以添加暂停游戏时间的功能，例如：
            // Time.timeScale = isActive ? 1f : 0f;

            Debug.Log($"道具交互面板状态切换到: {!isActive}");
        }
    }
}
