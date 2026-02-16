using UnityEngine;
using System.Collections.Generic;

// 存储单个场景的表现数据
[System.Serializable]
// 抽象类：不能被直接实例化，只能被继承
public abstract class BaseSceneData
{
    // 所有场景通用的数据
    public int Score = 0;       // 历史得分
    public bool IsCleared = false;  // 是否被通过

    // 【可选】可以在此添加一个通用的更新方法，让子类覆盖
    // public abstract void Update(int score, string customData); 
}
// Dream1Data.cs
[System.Serializable]
public class Dream1Data : BaseSceneData
{
    // Dream1 特有数据
    public bool TriggerVictoryEnding = false;
    public bool TriggerDeserterEnding = false;
    public bool s3 = false;

}

// Dream2Data.cs
[System.Serializable]
public class Dream2Data : BaseSceneData
{
    // Dream2 特有数据
    public int time;
    public bool s4 = false;

    public bool isFileDestroy = false;
    public bool isFileGot = false;
}

// Dream3Data.cs
[System.Serializable]
public class Dream3Data : BaseSceneData
{
    // Dream3 特有数据
    public bool bad = false;
    public bool good = false;
    public bool excellent = false;
    public int ExitDoorID = 0;
}

public class GameDataController : MonoBehaviour
{
    // 确保只有一个实例 (单例模式)
    public static GameDataController Instance { get; private set; }

    // 【核心数据结构】: Key=场景名 (如 "Dream3"), Value=该场景的表现数据
    public Dictionary<string, BaseSceneData> sceneDataMap =
        new Dictionary<string, BaseSceneData>();

    // 【新增】当前激活的场景名称，用于保存数据时识别
    private string lastPlayedScene = "";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // 【关键】: 使该对象在加载新场景时不会被销毁
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 如果已经存在实例，销毁新的实例
            Destroy(gameObject);
        }
    }

    // ==========================================
    // 供其他脚本调用的数据接口
    // ==========================================

    /// <summary>
    /// 当玩家从一个子场景返回主场景前，调用此方法保存数据
    /// </summary>
    // ==========================================
    // 辅助方法：根据场景名创建正确的初始数据实例
    // ==========================================
    private BaseSceneData CreateInitialSceneData(string sceneName)
    {
        switch (sceneName)
        {
            case "Dream1":
                return new Dream1Data();
            case "Dream2":
                return new Dream2Data();
            case "Dream3":
                return new Dream3Data();
            default:
                // 默认返回一个通用类型，或抛出错误
                Debug.LogError($"未知场景名: {sceneName}，请为该场景添加数据类型。");
                return new Dream1Data(); // 默认返回一个类型以防空指针
        }
    }
    public T GetSceneData<T>(string sceneName) where T : BaseSceneData
    {
        // 1. 确保数据存在，如果不存在则创建正确的初始数据
        if (!sceneDataMap.ContainsKey(sceneName))
        {
            sceneDataMap[sceneName] = CreateInitialSceneData(sceneName);
        }

        // 2. 将存储的基础类型对象安全地转换为请求的子类类型 (T)
        if (sceneDataMap[sceneName] is T specificData)
        {
            return specificData;
        }
        // 3. 错误处理：如果请求的类型与实际存储的类型不匹配
        Debug.LogError($"数据类型不匹配！场景 {sceneName} 存储了 {sceneDataMap[sceneName].GetType().Name}，但请求了 {typeof(T).Name}");
        // 如果出错，返回一个默认的空实例
        return (T)CreateInitialSceneData(sceneName);
    }

    public void SaveScenePerformance(string sceneName, int currentScore)
    {
        // 确保数据存在
        if (!sceneDataMap.ContainsKey(sceneName))
        {
            sceneDataMap[sceneName] = CreateInitialSceneData(sceneName);
        }

        // 获取该场景的历史数据
        BaseSceneData data = sceneDataMap[sceneName];

        // 更新通用数据 (最高分、是否通关)

        data.Score = currentScore;

        if (currentScore > 0)
        {
            Debug.Log("set iscleared to be true");
            data.IsCleared = true;
        }

        Debug.Log($"DataController: 已保存场景 {sceneName} 的得分: {data.Score}");
    }
}