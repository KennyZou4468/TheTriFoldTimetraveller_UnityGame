using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // 必须引用新版输入系统命名空间

public class IntroSceneController : MonoBehaviour
{
    public GameObject firstPanel;
    public GameObject secondPanel;
    public string targetSceneName = "Room";

    private int currentStep = 0;

    void Start()
    {
        if (firstPanel != null) firstPanel.SetActive(true);
        if (secondPanel != null) secondPanel.SetActive(false);
    }

    void Update()
    {
        // 关键点：使用 wasPressedThisFrame
        if (Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame)
        {
            HandleEnterPress();
        }
    }

    void HandleEnterPress()
    {
        if (currentStep == 0)
        {
            currentStep = 1;
            firstPanel.SetActive(false);
            secondPanel.SetActive(true);
        }
        else if (currentStep == 1)
        {
            currentStep = 2;
            SceneManager.LoadScene(targetSceneName);
        }
    }
}