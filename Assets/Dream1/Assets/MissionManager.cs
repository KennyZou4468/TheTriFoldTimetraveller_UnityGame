using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance;

    [Header("UI 组件")]
    public TextMeshProUGUI missionText; // 拖入那个显示内容的 Text_Content

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        // 游戏一开始的默认任务
        UpdateMission("探索公司大楼内部...");
    }

    // 🟢 给外部调用的方法：更新任务文案
    public void UpdateMission(string newObjective)
    {
        if (missionText != null)
        {
            missionText.text = "- " + newObjective;

            // (可选) 可以加个简单的闪烁动画提醒玩家任务变了
            StartCoroutine(BlinkEffect());
        }
    }

    System.Collections.IEnumerator BlinkEffect()
    {
        missionText.color = Color.green; // 变绿提示
        yield return new WaitForSeconds(0.5f);
        missionText.color = Color.white; // 变回白色
    }
}
