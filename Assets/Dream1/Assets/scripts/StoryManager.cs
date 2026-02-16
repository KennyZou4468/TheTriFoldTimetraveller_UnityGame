using UnityEngine;

public class StoryManager : MonoBehaviour
{
    // 对话UI面板（在Inspector里拖入）
    public GameObject dialoguePanel;
    public int backTrackCount = 0; // 玩家往回走的次数

    // 这是一个简单的状态机，用来处理你的剧情分支
    public void OnBackTrackTriggerEnter()
    {
        backTrackCount++;

        if (backTrackCount == 1)
        {
            // 第一次往回走：触发对话 "再考虑是否撤退？"
            ShowDialogue("这里不是撤退的路。你确定要放弃任务吗？\n1. 考虑撤退 \n2. 坚定进攻");
        }
        else if (backTrackCount >= 2)
        {
            // 这个状态需要配合具体的选择逻辑
            // 如果之前选了1，再次往回走道TriggerB -> 逃兵
        }
    }

    // UI按钮调用的函数
    public void PlayerChoseOption(int option)
    {
        if (option == 1) // 选择考虑撤退
        {
            // 关闭不可见墙，允许玩家继续向左走到“逃兵结局”触发器
            // 或者直接重置触发器，允许触发第二次对话
            Debug.Log("玩家动摇了...");
        }
        else if (option == 2) // 选择坚定进攻
        {
            GameManager.Instance.hasDetermination = true; // 设置Flag
            Debug.Log("玩家获得了决心，Boss现在可以被杀死了！");
            // 关闭对话框，让玩家回到战斗
            dialoguePanel.SetActive(false);
        }
    }

    void ShowDialogue(string text)
    {
        Time.timeScale = 0; // 暂停游戏
        dialoguePanel.SetActive(true);
        // 更新UI文字...
    }
}