using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class RealityBankManager : MonoBehaviour
{
    // ==========================================
    // UI 引用
    // ==========================================
    [Header("UI References")]
    public GameObject panel1;
    public GameObject panel2;
    public TextMeshProUGUI countdownText; // 逃跑倒计时文本
    [Header("End Game Panels")]
    public GameObject endGamePanel1; // ID 匹配时的结局 Panel
    public GameObject endGamePanel2; // ID 不匹配时的结局 Panel
    public GameObject timeOutPanel;

    [Header("Timing Settings")]
    public float panelDisplayTime = 3f; // 每个 Panel 的显示时间
    public float escapeTime = 10f;     // 玩家选择门的倒计时时间

    public bool isEscapeTimeRunning = false;
    private float currentEscapeTime;

    void Start()
    {
        // 确保所有面板初始都是隐藏的
        panel1.SetActive(false);
        panel2.SetActive(false);
        if (endGamePanel1 != null) endGamePanel1.SetActive(false);
        if (endGamePanel2 != null) endGamePanel2.SetActive(false);
        if (timeOutPanel != null) timeOutPanel.SetActive(false);
        if (countdownText != null && countdownText.transform.parent != null)
        {
            countdownText.transform.parent.gameObject.SetActive(false);
        }

        StartCoroutine(StartRealitySequence());
    }

    void Update()
    {
        if (isEscapeTimeRunning)
        {
            if (currentEscapeTime > 0)
            {
                currentEscapeTime -= Time.deltaTime;
                UpdateCountdownDisplay(currentEscapeTime);

                if (currentEscapeTime <= 0)
                {
                    currentEscapeTime = 0;
                    isEscapeTimeRunning = false;

                    // 【时间到】：玩家未选择门，触发失败结局
                    Debug.Log("逃跑时间结束，未选择门。触发失败结局。");

                    // 1. 隐藏倒计时 UI
                    if (countdownText != null && countdownText.transform.parent != null)
                    {
                        countdownText.transform.parent.gameObject.SetActive(false);
                    }

                    // 2. 【核心新增】：显示时间耗尽结局 Panel
                    if (timeOutPanel != null)
                    {
                        timeOutPanel.SetActive(true);
                    }
                    LockPlayerMovement();
                }
            }
        }
    }
    private void LockPlayerMovement()
    {

        Player1Controller player = FindObjectOfType<Player1Controller>();
        if (player != null)
        {
            player.LockMovement();
            Debug.Log("🔑 玩家移动已锁定。");
        }
        else
        {
            Debug.LogWarning("无法找到 Player1Controller 来锁定移动。");
        }
    }
    /// <summary>
    /// 更新倒计时文本显示
    /// </summary>
    private void UpdateCountdownDisplay(float timeToDisplay)
    {
        if (countdownText == null) return;

        // 格式化为整数秒
        int seconds = Mathf.CeilToInt(timeToDisplay);
        countdownText.text = seconds.ToString();
    }

    /// <summary>
    /// 场景的核心时序控制流程
    /// </summary>
    private IEnumerator StartRealitySequence()
    {
        Debug.Log("现实银行场景开始。");

        // --- 1. 播放 Panel 1 ---
        panel1.SetActive(true);
        yield return new WaitForSeconds(panelDisplayTime);
        panel1.SetActive(false);

        yield return new WaitForSeconds(4f); // 等待4秒钟，给玩家缓冲时间

        // --- 2. 播放 Panel 2 ---
        panel2.SetActive(true);
        SetPlayerMovementLock(true);
        yield return new WaitForSeconds(panelDisplayTime);
        panel2.SetActive(false);
        SetPlayerMovementLock(false);
        // --- 3. 启动逃跑选择阶段 ---
        StartEscapePhase();
    }

    private void StartEscapePhase()
    {
        // 激活门选择UI
        if (countdownText != null && countdownText.transform.parent != null)
        {
            countdownText.transform.parent.gameObject.SetActive(true);
        }

        // 启动倒计时
        currentEscapeTime = escapeTime;
        isEscapeTimeRunning = true;
        Debug.Log("逃跑选择阶段开始，倒计时启动。");

        // TODO: 解锁玩家移动（如果需要）
    }

    /// <summary>
    /// 当玩家选择了一个门后，外部脚本应调用此方法
    /// </summary>
    public void OnDoorSelected(int doorIndex)
    {
        // 这里的 isEscapeTimeRunning 检查可以作为双重保险
        if (!isEscapeTimeRunning) return;

        isEscapeTimeRunning = false; // 停止倒计时

        // 假设 DoorSelectPanel 包含倒计时文本和门本身的 UI 提示
        if (countdownText != null && countdownText.transform.parent != null)
        {
            countdownText.transform.parent.gameObject.SetActive(false);
        }

        Debug.Log($"玩家选择了门 {doorIndex}。正在检查 Dream3 历史数据...");
        int dream3ExitDoorID = 0;

        if (GameDataController.Instance != null)
        {
            // ... (获取 dream3ExitDoorID 的逻辑保持不变) ...
            Dream3Data dream3Data = GameDataController.Instance.GetSceneData<Dream3Data>("Dream3");
            if (dream3Data != null)
            {
                dream3ExitDoorID = dream3Data.ExitDoorID;
            }
        }
        else
        {
            Debug.LogError("GameDataController 未找到！无法读取 Dream3 决策数据。");
        }

        // ===============================================
        // 【核心修改】：激活结局 Panel
        // ===============================================

        if (doorIndex == dream3ExitDoorID)
        {
            // ID 匹配：显示 EndGamePanel 1
            if (endGamePanel1 != null)
            {
                endGamePanel1.SetActive(true);
                Debug.Log("门 ID 匹配，显示 EndGamePanel 1 (成功结局)。");
            }
        }
        else
        {
            // ID 不匹配：显示 EndGamePanel 2
            if (endGamePanel2 != null)
            {
                endGamePanel2.SetActive(true);
                Debug.Log("门 ID 不匹配，显示 EndGamePanel 2 (普通/失败结局)。");
            }
        }
    }
    public void SetPlayerMovementLock(bool isLocked)
    {
        Player1Controller player = FindObjectOfType<Player1Controller>();
        if (player != null)
        {
            if (isLocked)
            {
                player.LockMovement();
            }
            else
            {
                player.UnlockMovement();
            }
        }
    }
}