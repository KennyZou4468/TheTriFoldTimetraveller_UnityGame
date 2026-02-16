using UnityEngine;

public class HoleTrigger : MonoBehaviour
{
    // 这里不需要 public GameObject，因为 Prefab 没法拖拽场景物体
    // 我们用代码自动找 ChoiceManager
    private ChoiceManager uiManager;

    void Start()
    {
        // 🔍 1. 自动寻找：在当前场景里找那个挂了 ChoiceManager 的物体
        uiManager = FindObjectOfType<ChoiceManager>();

        // 如果忘记放 UI 了，报个错提醒自己
        if (uiManager == null)
        {
            Debug.LogError("❌ 严重错误：场景里找不到 [ChoiceManager]！\n请确保你创建了 ChoiceCanvas，并且上面挂载了 ChoiceManager 脚本。");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 🔍 2. 检测玩家
        if (other.CompareTag("Player"))
        {
            if (uiManager != null)
            {
                Debug.Log("🕳️ 玩家进洞，呼叫 UI 面板...");

                // 调用 Manager 的方法：打开界面、暂停游戏
                uiManager.ShowChoiceUI();
            }
        }
    }
}