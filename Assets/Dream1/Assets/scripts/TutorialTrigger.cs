using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [Header("对话内容 (可以在Inspector里写多句)")]
    [TextArea(3, 10)]
    public string[] messages; // 🟢 改成数组了！

    private bool hasTriggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;

            if (DialogueManager.Instance != null)
            {
                // 🟢 循环发送每一句话
                foreach (string msg in messages)
                {
                    // 给每句话加个前缀（如果你还没加的话）
                    

                    // 发送给管理器去排队
                    DialogueManager.Instance.ShowMessage(msg);
                }
            }
        }
    }
}
