using UnityEngine;
using UnityEngine.UI;

public class InspectManager : MonoBehaviour
{
    public static InspectManager Instance;

    public GameObject inspectPanel;
    public Text titleText;
    public Text contentText;

    void Awake()
    {
        if (Instance == null) Instance = this;
        if (inspectPanel != null) inspectPanel.SetActive(false);
    }

    // 打开
    public void ShowInfo(string title, string content)
    {
        if (inspectPanel == null) return;

        if (titleText != null) titleText.text = title;
        if (contentText != null) contentText.text = content;

        inspectPanel.SetActive(true);
    }

    // 关闭
    public void HideInfo()
    {
        if (inspectPanel != null) inspectPanel.SetActive(false);
    }
}
