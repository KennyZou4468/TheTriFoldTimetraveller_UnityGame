using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // 如果需要管理场景切换


public class Dream3_door : MonoBehaviour
{
    public GameObject interactUI; // 显示提示用的 UI（例如按E）
    private bool playerInRange = false;
    [Header("Door Settings")]
    public int doorID = 1;
    void Start()
    {
        if (interactUI != null)
            interactUI.SetActive(false);
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;       // 🔥 必须设置
            interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;      // 🔥 必须设置
            interactUI.SetActive(false);
        }
    }

    void Interact()
    {
        if (GameManager3.Instance == null) return;

        // 【核心判断】：检查 GameManager 是否允许退出
        if (GameManager3.Instance.CanPlayerExit())
        {
            Debug.Log("门已解锁，正在结算并退出...");

            // 1. 运行结算逻辑（显示评级和 GameOverPanel）
            // 我们现在将这些UI逻辑移到 Interact() 中执行

            // 确保只显示一次，避免重复
            if (GameManager3.Instance.gameOverPanel != null && !GameManager3.Instance.gameOverPanel.activeSelf)
            {
                GameManager3.Instance.gameOverPanel.SetActive(true);

                // 假设 GameManager 中有一个公共方法来获取最终得分
                int finalScore = GameManager3.Instance.GetScore();

                // 显示评级文本 (GetFinalScoreMessage 会设置 excellent/good/bad 字段)
                if (GameManager3.Instance.finalEvaluationText != null)
                {
                    // 注意：这里需要通过 GameManager 实例来调用其私有方法
                    // 如果 GetFinalScoreMessage 是 private，需要将其改为 public 或 internal
                    GameManager3.Instance.finalEvaluationText.text = GameManager3.Instance.GetFinalScoreMessage(finalScore);
                }
            }
            string currentSceneName = SceneManager.GetActiveScene().name;

            Dream3Data dream3Data = GameDataController.Instance.GetSceneData<Dream3Data>(currentSceneName);
            if (dream3Data != null)
            {
                // 记录玩家使用的出口编号
                dream3Data.ExitDoorID = this.doorID;
                Debug.Log($"已记录玩家从出口编号: {this.doorID} 退出。");

                // 确保调用保存，保存通用数据和门 ID
                GameDataController.Instance.SaveScenePerformance(currentSceneName, GameManager3.Instance.GetScore());
            }
            // 2. 切换回 Room 场景（可以延迟加载，让玩家有时间看评级）
            // 延迟加载，给玩家看评级结果的时间 (例如 3 秒后)
            // 禁用门互动
            playerInRange = false;
            if (interactUI != null) interactUI.SetActive(false);
        }
        else
        {
            Debug.Log("🔒 门已锁定。时间结束前无法退出。");
        }
    }
}
