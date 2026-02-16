using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // 如果需要管理场景切换

public class BedMachine : MonoBehaviour
{
    private static readonly List<string> DreamOrder = new List<string> {
        "Dream1", "Dream2", "Dream3"
    };

    public GameObject interactUI; // 显示提示用的 UI（例如按E）
    private bool playerInRange = false;
    [Header("Debug Settings")]
    public bool debugAutoClearDream = false;
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
            playerInRange = true;
            interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            interactUI.SetActive(false);
        }
    }

    void Interact()
    {
        Debug.Log("BedMachine Interact...");

        if (GameDataController.Instance == null)
        {
            Debug.LogError("GameDataController 未初始化！无法检查梦境进度。");
            return;
        }
        BaseSceneData targetData = null; // 用于存储当前检查的梦境数据
        string targetDreamName = null;   // 用于存储当前检查的梦境名称
        bool allDreamsCleared = true;

        // 1. 遍历定义的梦境顺序
        foreach (string dreamName in DreamOrder)
        {
            // 2. 动态获取数据（需要用到具体的子类类型）
            if (dreamName == "Dream1")
            {
                targetData = GameDataController.Instance.GetSceneData<Dream1Data>(dreamName);
            }
            else if (dreamName == "Dream2")
            {
                targetData = GameDataController.Instance.GetSceneData<Dream2Data>(dreamName);
            }
            else if (dreamName == "Dream3")
            {
                targetData = GameDataController.Instance.GetSceneData<Dream3Data>(dreamName);
            }

            if (targetData != null && !targetData.IsCleared)
            {
                Debug.Log("Find one unCompleted dream: " + dreamName);
                // 发现第一个未完成的梦境！
                targetDreamName = dreamName;
                allDreamsCleared = false;

                // =============================================
                // 【核心新增：调试自动清除逻辑】
                // =============================================
                /*
                if (debugAutoClearDream && (targetDreamName == "Dream1" || targetDreamName == "Dream2"))
                {
                    // 标记当前梦境已清除
                    targetData.IsCleared = true;
                    Debug.LogWarning($"[DEBUG AUTO-CLEAR]: 梦境 {targetDreamName} 已自动标记为 IsCleared = true。");

                    // 🚨 必须调用 SaveScenePerformance 来更新和保存 IsCleared 状态！
                    // 为了让 GameDataController 知道状态改变，我们调用 SaveScenePerformance。
                    // 传入当前分数 (data.Score) 或一个非零值，以确保 IsCleared 被正确设置。
                    GameDataController.Instance.SaveScenePerformance(targetDreamName, targetData.Score > 0 ? targetData.Score : 1);

                    // 跳出循环
                    break;
                }*/
                // =============================================

                // 正常模式：加载找到的未完成梦境
                LoadDreamScene(targetDreamName);
                return; // 立即退出，进入场景
            }
        }

        // 3. 如果循环结束，allDreamsCleared 仍为 true
        if (allDreamsCleared)
        {
            Debug.Log("恭喜！所有梦境都已完成。");
        }
    }
    private void LoadDreamScene(string sceneName)
    {
        Debug.Log($"Loading next dream: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }
}
