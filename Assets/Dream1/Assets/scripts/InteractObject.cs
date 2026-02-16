using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [Header("物品资料")]
    public string itemName = "未命名";
    [TextArea(3, 10)] public string itemDescription = "内容...";

    [Header("视觉反馈")]
    public Color selectColor = new Color(0.7f, 0.7f, 0.7f, 1f);
    private Color originalColor = Color.white;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) originalColor = sr.color;
    }

    // 🔴 没有任何按键检测了，全靠碰撞

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 变色
            if (sr != null) sr.color = selectColor;

            // 打开 UI
            if (InspectManager.Instance != null)
            {
                InspectManager.Instance.ShowInfo(itemName, itemDescription);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 恢复颜色
            if (sr != null) sr.color = originalColor;

            // 关闭 UI
            if (InspectManager.Instance != null)
            {
                InspectManager.Instance.HideInfo();
            }
        }
    }
}