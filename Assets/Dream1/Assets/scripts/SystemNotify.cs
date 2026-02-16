using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class SystemNotify : MonoBehaviour
{
    public static SystemNotify Instance;

    [Header("UI 组件")]
    public GameObject notifyPanel; // 拖入那个黑底面板
    public TextMeshProUGUI notifyText;        // 拖入显示文字的 Text

    void Awake()
    {
        // 单例模式：为了让别的脚本能找到它
        if (Instance == null) Instance = this;

        // 游戏开始时默认隐藏
        if (notifyPanel != null) notifyPanel.SetActive(false);
    }

    // --- 外部调用的方法 ---
    public void ShowNotification(string message, float duration = 3f)
    {
        StopAllCoroutines();
        StartCoroutine(ShowRoutine(message, duration));
    }

    IEnumerator ShowRoutine(string msg, float duration)
    {
        if (notifyPanel != null && notifyText != null)
        {
            notifyPanel.SetActive(true); // 打开
            notifyText.text = msg;       // 改字

            yield return new WaitForSeconds(duration); // 等待

            notifyPanel.SetActive(false); // 关闭
        }
    }
}