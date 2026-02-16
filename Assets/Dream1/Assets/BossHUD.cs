using UnityEngine;
using UnityEngine.UI;

public class BossHUD : MonoBehaviour
{
    public static BossHUD Instance;
    private Slider slider;
    private CanvasGroup canvasGroup; // 用来控制显示/隐藏

    void Awake()
    {
        if (Instance == null) Instance = this;
        slider = GetComponent<Slider>();

        // 一开始先隐藏 (把物体设为不激活)
        gameObject.SetActive(false);
    }

    // --- 显示血条 (Boss出现时调用) ---
    public void ShowBossHealth(int current, int max)
    {
        gameObject.SetActive(true); // 激活UI
        slider.maxValue = max;
        slider.value = current;
    }

    // --- 更新血量 (受伤时调用) ---
    public void UpdateHealth(int current)
    {
        if (slider != null)
        {
            slider.value = current;
        }

        // 如果血量归零，延迟隐藏
        if (current <= 0)
        {
            Invoke("Hide", 2f);
        }
    }

    void Hide()
    {
        gameObject.SetActive(false);
    }
}
