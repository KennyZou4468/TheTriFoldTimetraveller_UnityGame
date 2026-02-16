using UnityEngine;

public class StoryTrigger : MonoBehaviour
{
    [Header("设置")]
    public bool isDeserterEndingLine = false; // 勾选这个代表是“逃兵结局线”
    public GameObject dialoguePanel; // 拖入你的UI对话面板

    // 记录是否已经触发过第一次警告
    private bool hasTriggeredWarning = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        // 只检测玩家
        if (other.CompareTag("Player"))
        {
            // 情况1：这条线是最后面的“逃兵结局触发线”
            if (isDeserterEndingLine)
            {
                Debug.Log("触发结局：逃兵");
                // GameManager.Instance.TriggerDeserterEnding(); // 调用你的结局逻辑
                return;
            }

            // 情况2：这是第一次回头的警告线
            if (!hasTriggeredWarning)
            {
                // 只有没触发过才执行
                hasTriggeredWarning = true;

                Debug.Log("触发对话：你确定要撤退吗？");

                // 暂停游戏
                Time.timeScale = 0;

                // 显示UI (请在UI按钮里写好继续或撤退的逻辑)
                if (dialoguePanel != null)
                {
                    dialoguePanel.SetActive(true);
                }
            }
        }
    }
}
