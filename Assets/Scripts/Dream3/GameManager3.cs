using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement; // 管理场景切换
using UnityEngine.UI;
using TMPro; // 【新增】引入 TextMeshPro 命名空间
using System.Collections.Generic;
using System.Collections; // 【必须新增】引入 System.Collections 命名空间
public class GameManager3 : MonoBehaviour
{
    public static GameManager3 Instance { get; private set; }

    [Header("UI References")]
    public GameObject startPanel;
    public GameObject gameOverPanel; // 【新增】拖入 GameOverPanel
    public TextMeshProUGUI timerText;          // 【新增】拖入用于显示时间的 Text 组件
    public TextMeshProUGUI finalEvaluationText;
    // 【新增字段】：时间结束后显示的提示面板 (例如 "Time's up! Find the exit.")
    public GameObject exitHintPanel;
    [Header("Game State")]
    private int score = 0;
    private bool isGameStarted = false;
    private bool excellent = false;
    private bool good = false;
    private bool bad = false;
    private int door;

    // 【新增】计时变量
    [Header("Timer Settings")]
    public float gameTime = 60f; // 初始倒计时时间 (60秒)
    private float currentTime;

    private bool canExit = false;


    // 【新增】: 供 Dream3_door 脚本查询和调用的方法
    public bool CanPlayerExit()
    {
        return canExit;
    }
    public void ExitGameToRoom()
    {
        Debug.Log("玩家通过门退出，加载 Room 场景。");
        LoadRoomScene();
    }
    void Awake()
    {
        if (GameManager3.Instance == null)
        {
            GameManager3.Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        currentTime = gameTime; // 初始化当前时间
        UpdateTimerDisplay(currentTime); // 初始显示时间
        if (timerText != null)
        {
            timerText.gameObject.SetActive(false);
        }
        SetGameState(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // 1. 如果 StartPanel 激活 (游戏未开始) -> 启动游戏
            if (!isGameStarted && startPanel != null && startPanel.activeSelf)
            {
                StartGame();
                // 注意：此处必须使用 return 或 else if，否则会在同一帧尝试执行下面的逻辑
                return;
            }

            // 2. 如果 GameOverPanel 激活 (游戏已结束) -> 加载新场景
            if (!isGameStarted && gameOverPanel != null && gameOverPanel.activeSelf)
            {
                Debug.Log("Try to load scene");
                ExitGameToRoom();
                return;
            }
        }
        // 【新增】倒计时逻辑
        if (isGameStarted)
        {
            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime; // 持续减去时间
                UpdateTimerDisplay(currentTime);

                if (currentTime <= 0)
                {
                    currentTime = 0;
                    UpdateTimerDisplay(currentTime);
                    GameOver(); // 时间到，游戏结束
                }
            }
        }
    }

    public void LoadRoomScene()
    {
        Debug.Log("Load Room");
        // SceneManager.LoadScene 需要场景名作为参数
        SceneManager.LoadScene("Room");
    }
    void UpdateTimerDisplay(float timeToDisplay)
    {
        if (timerText == null) return;

        // 确保显示的时间不为负数
        if (timeToDisplay < 0) timeToDisplay = 0;

        // 格式化时间为 分:秒 (00:00)
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        // 使用 String.Format 确保个位数秒数前面有 0
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    public void SetGameState(bool isRunning)
    {
        isGameStarted = isRunning;

        // 控制 UI 显示
        if (startPanel != null)
        {
            // 【关键修复】: 游戏运行时 (isRunning=true)，隐藏面板 (SetActive=false)
            startPanel.SetActive(!isRunning);
        }
        if (timerText != null)
        {
            // 只有游戏运行时 (isRunning=true) 才显示时钟
            timerText.gameObject.SetActive(isRunning);
        }

        if (isRunning)
        {
            // 确保场景中所有 Cash 开始移动
            Cash[] allCash = FindObjectsOfType<Cash>();
            foreach (Cash cash in allCash)
            {
                cash.StartMovement();
            }
            Pig[] allPigs = FindObjectsOfType<Pig>();
            foreach (Pig pig in allPigs)
            {
                pig.StartMovement();
            }
            Ring[] allRings = FindObjectsOfType<Ring>();
            foreach (Ring ring in allRings)
            {
                ring.StartMovement();
            }
            ChestMimic[] allChestMimics = FindObjectsOfType<ChestMimic>();
            foreach (ChestMimic chestMimic in allChestMimics)
            {
                Debug.Log("Start Chest Mimic Movement");
                chestMimic.StartMovement();
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
    public void StartGame()
    {
        Debug.Log("Game Started!");
        // 【新增】在游戏开始时，重置时间
        currentTime = gameTime;
        SetPlayerMovementLock(false);
        UpdateTimerDisplay(currentTime);

        SetGameState(true);
    }

    public void GameOver()
    {
        SetGameState(false); // 停止游戏
        startPanel.SetActive(false);
        if (timerText != null)
        {
            timerText.gameObject.SetActive(false);
            timerText.transform.parent.gameObject.SetActive(false);
        }
        StartCoroutine(ShowExitHintForDuration(3f)); // 显示 3 秒
        canExit = true;
        Debug.Log("时间到! 游戏结束。您的得分是: " + score);
        // 2. 【核心保存逻辑】
        if (GameDataController.Instance != null)
        {
            string currentSceneName = SceneManager.GetActiveScene().name;

            // 获取 Dream3Data 实例
            Dream3Data dream3Data = GameDataController.Instance.GetSceneData<Dream3Data>(currentSceneName);
            //获取到Dream3的data之后更新数据
            if (dream3Data != null)
            {
                GetFinalScoreMessage(score);//在这里更新评级
                Debug.Log("尝试更新Dream3的特殊数据");
                // 3. 根据 Manager 中已设置的 bool 变量，更新持久化数据对象
                dream3Data.bad = bad;       // 假设 s7 记录是否达到 bad
                dream3Data.good = good;      // 假设 s8 记录是否达到 good
                dream3Data.excellent = excellent; // 假设 s9 记录是否达到 excellent
            }
            Debug.Log("尝试更新Dream3的通用数据");

            // 4. 调用通用的保存方法，更新 HighScore, IsCleared, 并触发持久化
            GameDataController.Instance.SaveScenePerformance(
                currentSceneName,
                score
            );
        }
    }
    private IEnumerator ShowExitHintForDuration(float duration)
    {
        if (exitHintPanel == null) yield break; // 如果面板不存在，直接退出

        // 1. 显示面板
        exitHintPanel.SetActive(true);

        Debug.Log($"退出提示面板将在 {duration} 秒后自动隐藏。");
        SetPlayerMovementLock(true);
        // 2. 等待指定时间
        yield return new WaitForSeconds(duration);

        // 3. 隐藏面板
        exitHintPanel.SetActive(false);
        SetPlayerMovementLock(false);

    }
    public string GetFinalScoreMessage(int score)
    {
        // 1. 定义分数映射表: (达到该等级所需最小分数, 评价文本模板)
        var scoreMap = new List<(int MinScore, string Message)>
    {
        (0, "You did a bad job"),
        (20, "You did a good job"),
        (35, "Wow,You are a legend")
    };

        string scoreMessageTemplate = "无法评级。\n最终得分: {0}"; // 默认文本
        string finalMessage = "";

        // 2. 【新增】 清空所有评级状态
        excellent = false;
        good = false;
        bad = false;

        // 3. 倒序遍历 Map，找到玩家分数达到的最高等级
        for (int i = scoreMap.Count - 1; i >= 0; i--)
        {
            if (score >= scoreMap[i].MinScore)
            {
                scoreMessageTemplate = scoreMap[i].Message;

                // 4. 【新增】 根据等级设置对应的 bool 变量
                if (scoreMap[i].MinScore >= 26) // "Wow,You are a legend"
                {
                    excellent = true;
                }
                else if (scoreMap[i].MinScore >= 15 && scoreMap[i].MinScore <= 25) // "You did a good job"
                {
                    good = true;
                }
                else // if (scoreMap[i].MinScore >= 0) // "You did a bad job"
                {
                    bad = true;
                }

                break; // 找到最高等级后退出循环
            }
        }

        // 格式化文本，将 {0} 替换为实际分数 (如果模板中有 {0})
        finalMessage = scoreMessageTemplate.Replace("{0}", score.ToString());

        // 【调试检查】
        //Debug.Log($"评级结果：Excellent={excellent}, Good={good}, Bad={bad}");
        Debug.Log(finalMessage);
        return finalMessage;
    }
    /// </summary>
    public int GetScore()
    {
        return score;
    }

    /// <summary>
    /// 增加得分
    /// </summary>
    /// <param name="points">要增加的分数</param>
    public void AddScore(int points)
    {
        score += points;
        Debug.Log("得分增加! 当前分数: " + score);
    }

}