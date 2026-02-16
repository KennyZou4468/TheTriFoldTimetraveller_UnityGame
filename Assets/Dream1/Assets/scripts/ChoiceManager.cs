using UnityEngine;
using UnityEngine.UI;

public class ChoiceManager : MonoBehaviour
{
    [Header("UI 组件")]
    public GameObject choicePanel; // 拖入 Panel

    [Header("游戏逻辑引用")]
    public GameObject player;          // 拖入 Player (作为保底位置)
    public GameObject medalPrefab;     // 拖入 勋章 Prefab

    [Header("生成位置设置")]
    public Transform medalSpawnPoint;  // 🟢 拖入场景中你想让勋章出现的那个空物体

    void Start()
    {
        // 游戏开始时确保 UI 是关着的
        if (choicePanel != null) choicePanel.SetActive(false);
    }

    // 开启 UI 的方法
    public void ShowChoiceUI()
    {
        if (choicePanel != null)
        {
            choicePanel.SetActive(true); // 显示 UI
            Time.timeScale = 0;          // 暂停游戏
        }
        else
        {
            Debug.LogError("ChoiceManager: 报错！你忘了在 Inspector 里拖拽 ChoicePanel！");
        }
    }

    // 🔴 按钮 A: 逃跑
    public void OnClick_RunAway()
    {
        Debug.Log("玩家选择了逃跑...");
        Time.timeScale = 1; // 恢复游戏

        if (GameManager.Instance != null)
        {
            GameManager.Instance.TriggerDeserterEnding();
        }
    }

    // 🔵 按钮 B: 折返
    public void OnClick_Return()
    {
        Debug.Log("玩家选择了折返战斗！");

        Time.timeScale = 1; // 恢复游戏

        if (choicePanel != null) choicePanel.SetActive(false); // 关闭 UI

        // 1. 销毁黑洞 (断绝后路)
        // 查找名为 Tag 为 Hole 的物体并销毁
        GameObject hole = GameObject.FindGameObjectWithTag("Hole");
        if (hole != null) Destroy(hole);

        if (MissionManager.Instance != null)
            MissionManager.Instance.UpdateMission("拾取勋章并消灭 Boss！");
        // 2. 生成勋章
        if (medalPrefab != null)
        {
            Vector3 finalPos;

            // 🟢 优先使用指定的生成点
            if (medalSpawnPoint != null)
            {
                finalPos = medalSpawnPoint.position;
            }
            else
            {
                // 如果没设置生成点，就用玩家位置作为保底
                if (player == null) player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                    finalPos = player.transform.position + Vector3.right * 2;
                else
                    finalPos = Vector3.zero; // 实在找不到就在世界中心生成
            }

            Instantiate(medalPrefab, finalPos, Quaternion.identity);
            Debug.Log("✅ 勋章已生成在: " + finalPos);
        }
        else
        {
            Debug.LogError("❌ 勋章生成失败：请在 Inspector 里拖入 [Medal Prefab]！");
        }
    }
}
