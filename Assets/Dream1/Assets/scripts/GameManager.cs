using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("玩家数据")]
    public int currentMoney = 0;
    public bool hasDetermination = false; // 是否拿到勋章（杀Boss资格）

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    // --- 经济系统 ---
    public void AddMoney(int amount)
    {
        currentMoney += amount;
        Debug.Log($"[系统] 获得金钱: {amount}，当前总额: {currentMoney}");
        if (HUDManager.Instance != null) HUDManager.Instance.UpdateMoney(currentMoney);
    }

    // --- 第一梦：三种结局分支 ---

    // 路线 A: 梦境同步完成 (即玩家在梦里被打死 -> 醒来继续治病)
    public void TriggerDreamDeath()
    {
        Debug.Log(">> [剧情推进] 梦境过于激烈导致意识中断，正在唤醒...");
        SaveData("Dream1");
        // 这里不重置金钱，把你梦里赚的钱带到第二天
        SceneManager.LoadScene("Room"); // 你的下一关场景名
    }

    // 路线 B: 逃兵结局 (进洞选了逃跑)
    public void TriggerDeserterEnding()
    {
        Debug.Log(">> [结局] 实验室小白鼠 (Bad End)");
        currentMoney = 0; // 惩罚
        SaveDeserterData("Dream1");

        SceneManager.LoadScene("Room");
    }

    // 路线 C: 胜利结局 (拿勋章杀了Boss)
    public void TriggerVictoryEnding()
    {
        Debug.Log(">> [结局] 英雄后代/月球基地 (True End)");
        SaveVictoryData("Dream1");

        SceneManager.LoadScene("Room");
    }
    private void SaveVictoryData(string sceneName)
    {
        Debug.Log($"Loading next dream: {sceneName}");
        Dream1Data dream1Data = GameDataController.Instance.GetSceneData<Dream1Data>("Dream1");
        dream1Data.IsCleared = true;
        dream1Data.TriggerVictoryEnding = true;
        dream1Data.Score = currentMoney / 100;
    }
    private void SaveDeserterData(string sceneName)
    {
        Debug.Log($"Loading next dream: {sceneName}");
        Dream1Data dream1Data = GameDataController.Instance.GetSceneData<Dream1Data>("Dream1");
        dream1Data.IsCleared = true;
        dream1Data.TriggerDeserterEnding = true;
        dream1Data.Score = currentMoney / 100;
    }
    private void SaveData(string sceneName)
    {
        Debug.Log($"Loading next dream: {sceneName}");
        Dream1Data dream1Data = GameDataController.Instance.GetSceneData<Dream1Data>("Dream1");
        dream1Data.IsCleared = true;
        dream1Data.Score = currentMoney / 100;
    }
}
