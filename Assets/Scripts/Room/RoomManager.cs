using UnityEngine;
using System.Collections.Generic;

//每次进入room都会运行该脚本，展示data的状态，并且生成每个dream完成之后的特殊物品

public class RoomManager : MonoBehaviour
{
    // 定义所有我们关心的场景名称
    private static readonly List<string> DreamSceneNames = new List<string> {
        "Dream1", "Dream2", "Dream3"
    };
    [Header("Generation Prefabs for dream3")]
    public GameObject object1Prefabfordream3;
    public GameObject object2Prefabfordream3;
    public GameObject object3Prefabfordream3;
    [Header("Generation Prefabs for dream1")]
    public GameObject object1Prefabfordream1;
    public GameObject object2Prefabfordream1;

    public Vector3 spawnPositionfordream3 = new Vector3(1.5f, 0.1f, 0f);
    public Vector3 spawnPositionfordream1 = new Vector3(0.85f, 0.2f, 0f);
    public bool Dream3SpecialObjectIsSpawned = false;
    public bool Dream1SpecialObjectIsSpawned = false;
    bool globalClear = false;
    bool CanGoToBank = false;
    // 【新增】终局门的引用和状态
    [Header("Final Door Settings")]
    public GameObject finalDoorPrefab; // 拖入用于进入 Last_day 的门 Prefab
    public Vector3 finalDoorSpawnPosition = new Vector3(1.17f, 0.15f, 0f); // 终局门的生成位置
    private bool finalDoorIsSpawned = false; // 终局门的本地生成状态
    [Header("End Game Panels")]
    public GameObject BadendGamePanel; // ID 匹配时的结局 Panel

    void Start()
    {
        // 确保 GameDataController 已经初始化
        if (GameDataController.Instance == null)
        {
            Debug.LogError("GameDataController 未找到。请确保它在加载 Room 场景时不会被销毁。");
            return;
        }
        if (BadendGamePanel != null) BadendGamePanel.SetActive(false);

        Debug.Log("=========================================");
        Debug.Log("🏠 Room Scene Manager: 正在加载所有 Dream 场景数据...");
        Debug.Log("=========================================");

        LoadAndDisplayAllDreamData();
        CheckAndSpawnDream3Reward();
        CheckAndSpawnDream1SpecialObject();
        // 【新增】：检查是否可以生成终局门
        CheckDreamComplete();
        if (globalClear)
        {
            CheckCanGoToBank();
            if (CanGoToBank)
            {
                SpawnFinalDoor();
            }
            else
            {
                //Show fail scene
                if (BadendGamePanel != null)
                {
                    BadendGamePanel.SetActive(true);
                    Debug.Log("Bad ending");
                }
            }
        }
    }
    private void SpawnFinalDoor()
    {
        // 如果所有梦境都已通关，生成门
        if (globalClear)
        {
            Debug.Log("[Room] 🎉 所有梦境已通关！正在生成终局门...");

            // 1. 实例化终局门
            GameObject doorInstance = Instantiate(finalDoorPrefab, finalDoorSpawnPosition, Quaternion.identity);

            // 2. 标记状态
            finalDoorIsSpawned = true;

            // 3. 【关键】：确保这个门知道要加载哪个场景
            // 假设终局门的 Prefab 上挂载了一个名为 FinalDoorController 的脚本
            FinalDoorController finalDoor = doorInstance.GetComponent<FinalDoorController>();
            if (finalDoor != null)
            {
                // 如果 FinalDoorController 有一个设置目标场景的方法
                // finalDoor.SetTargetScene("Last_day");
            }
        }
    }
    private void CheckDreamComplete()
    {
        if (finalDoorIsSpawned || GameDataController.Instance == null || finalDoorPrefab == null)
        {
            return;
        }

        bool allCleared = true;

        // 遍历所有梦境，检查 IsCleared 状态
        foreach (string sceneName in DreamSceneNames)
        {
            BaseSceneData data = null;

            // 动态获取数据 (与 LoadAndDisplayAllDreamData 中的逻辑类似)
            if (sceneName == "Dream1")
                data = GameDataController.Instance.GetSceneData<Dream1Data>(sceneName);
            else if (sceneName == "Dream2")
                data = GameDataController.Instance.GetSceneData<Dream2Data>(sceneName);
            else if (sceneName == "Dream3")
                data = GameDataController.Instance.GetSceneData<Dream3Data>(sceneName);

            // 检查 IsCleared
            if (data == null || !data.IsCleared)
            {
                allCleared = false;
                Debug.Log($"[Room] 梦境 {sceneName} 尚未通关，不生成终局门。");
                break; // 只要有一个未通关，立即退出循环
            }
        }
        globalClear = allCleared;
        if (globalClear)
        {
            Debug.Log("All dreams are cleared");
        }

    }
    private void CheckCanGoToBank()
    {
        Dream1Data targetData1 = null;
        Dream2Data targetData2 = null;
        Dream3Data targetData3 = null;
        foreach (string dreamName in DreamSceneNames)
        {
            // 2. 动态获取数据（需要用到具体的子类类型）
            if (dreamName == "Dream1")
            {
                targetData1 = GameDataController.Instance.GetSceneData<Dream1Data>(dreamName);
            }
            else if (dreamName == "Dream2")
            {
                targetData2 = GameDataController.Instance.GetSceneData<Dream2Data>(dreamName);
            }
            else if (dreamName == "Dream3")
            {
                targetData3 = GameDataController.Instance.GetSceneData<Dream3Data>(dreamName);
            }
        }
        //Todo: check condition
        if (calculateScore(targetData1, targetData2, targetData3) > 0)
        {
            CanGoToBank = true;
        }
        else
        {
            CanGoToBank = false;
        }
        if (CanGoToBank)
        {
            Debug.Log("Can go to bank");
        }
    }
    private int calculateScore(Dream1Data data1, Dream2Data data2, Dream3Data data3)
    {
        int totalScore = 0;
        if (data1 != null)
        {
            int score = data1.Score;
            if (score >= 12)
            {
                totalScore += 1;
            }
            else if (score >= 8)
            {
                totalScore += 0;
            }
            else
            {
                totalScore += -1;
            }
            //calculate your score condition here
        }
        if (data2 != null)
        {
            float remainingTime = data2.time;
            if (remainingTime >= 300)
            {
                totalScore += 1;
            }
            else if (remainingTime >= 0)
            {
                totalScore += 0;
            }
            else
            {
                totalScore += -1;
            }
            //calculate your score condition here

        }
        if (data3 != null)
        {
            //calculate your score condition here
            if (data3.excellent)
            {
                totalScore += 1;
            }
            else if (data3.good)
            {
                totalScore += 0;
            }
            else if (data3.bad)
            {
                totalScore += -1;
            }
        }
        return totalScore;
    }
    private void LoadAndDisplayAllDreamData()
    {
        foreach (string sceneName in DreamSceneNames)
        {
            // 对于 Dream1, Dream2, Dream3，我们需要分别获取它们的特有数据类型
            if (sceneName == "Dream1")
            {
                DisplayDream1Data(sceneName);
            }
            else if (sceneName == "Dream2")
            {
                DisplayDream2Data(sceneName);
            }
            else if (sceneName == "Dream3")
            {
                DisplayDream3Data(sceneName);
            }
        }
    }

    // ==========================================
    // 各个 Dream 场景的特定数据打印方法
    // ==========================================


    private void DisplayDream1Data(string sceneName)
    {
        // 使用泛型获取特定的 Dream1Data
        Dream1Data data = GameDataController.Instance.GetSceneData<Dream1Data>(sceneName);

        Debug.Log($"--- {sceneName} 数据 ---");
        Debug.Log($"  - 通用数据: 最高分={data.Score}, 已通关={data.IsCleared}");
        Debug.Log($"  - 特有数据: TriggerVictoryEnding={data.TriggerVictoryEnding}, TriggerDeserterEnding={data.TriggerDeserterEnding}, s3={data.s3}");
    }
    private void DisplayDream2Data(string sceneName)
    {
        // 使用泛型获取特定的 Dream2Data
        Dream2Data data = GameDataController.Instance.GetSceneData<Dream2Data>(sceneName);

        Debug.Log($"--- {sceneName} 数据 ---");
        Debug.Log($"  - 通用数据: 最高分={data.Score}, 已通关={data.IsCleared}");
        Debug.Log($"  - 特有数据: s4={data.time}, s5={data.isFileDestroy}, s6={data.isFileGot}");
    }
    private void DisplayDream3Data(string sceneName)
    {
        // 使用泛型获取特定的 Dream3Data
        Dream3Data data = GameDataController.Instance.GetSceneData<Dream3Data>(sceneName);

        Debug.Log($"--- {sceneName} 数据 ---");
        Debug.Log($"  - 通用数据: 最高分={data.Score}, 已通关={data.IsCleared}");
        // 使用 Dream3 特有的 bool 字段名称
        Debug.Log($"  - 特有数据: bad={data.bad}, good={data.good}, excellent={data.excellent},doorid = {data.ExitDoorID}");
    }
    private void CheckAndSpawnDream3Reward()
    {
        const string dreamName = "Dream3";

        // 1. 获取 Dream3 的数据
        // 使用 GetSceneData，如果数据不存在会创建默认值
        Dream3Data data = GameDataController.Instance.GetSceneData<Dream3Data>(dreamName);

        if (data == null) return;

        GameObject objectToSpawn = null;

        // 2. 根据 LastExitDoorID 确定要生成的物品
        switch (data.ExitDoorID)
        {
            case 1:
                objectToSpawn = object1Prefabfordream3;
                Debug.Log($"[Room] 发现 Dream3 退出 ID 为 1，准备生成 Object 1.");
                break;
            case 2:
                objectToSpawn = object2Prefabfordream3;
                Debug.Log($"[Room] 发现 Dream3 退出 ID 为 2，准备生成 Object 2.");
                break;
            case 3:
                objectToSpawn = object3Prefabfordream3;
                Debug.Log($"[Room] 发现 Dream3 退出 ID 为 3，准备生成 Object 3.");
                break;
            default:
                Debug.Log("[Room] Dream3 退出 ID 为 0 或未知，不生成特定物品。");
                break;
        }

        // 3. 实例化物品
        if (objectToSpawn != null && Dream3SpecialObjectIsSpawned == false)
        {
            Dream3SpecialObjectIsSpawned = true;
            Instantiate(objectToSpawn, spawnPositionfordream3, Quaternion.identity);
            // 重新保存数据，以确保重置后的状态被持久化
            // 这是一个假设的持久化函数，实际取决于您如何实现 GameDataController 的 Save/Load
            // GameDataController.Instance.SaveAllDataToDisk();

            Debug.Log($"[Room] 成功生成物品: {objectToSpawn.name}");
        }
    }
    private void CheckAndSpawnDream1SpecialObject()
    {
        const string dreamName = "Dream1";

        // 1. 获取 Dream1 的数据
        // 使用 GetSceneData，如果数据不存在会创建默认值
        Dream1Data data = GameDataController.Instance.GetSceneData<Dream1Data>(dreamName);

        if (data == null) return;

        GameObject objectToSpawn = null;
        if (data.TriggerVictoryEnding)
        {
            objectToSpawn = object1Prefabfordream1;
            Debug.Log($"[Room] 发现 Dream1 触发胜利结局，准备生成 Object 1.");
        }
        else if (data.TriggerDeserterEnding)
        {
            objectToSpawn = object2Prefabfordream1;
            Debug.Log($"[Room] 发现 Dream1 触发逃兵结局，准备生成 Object 2.");
        }
        else
        {
            Debug.Log("[Room] Dream1 未触发特殊结局，不生成特定物品。");
            return;
        }
        // 3. 实例化物品
        if (objectToSpawn != null && Dream1SpecialObjectIsSpawned == false)
        {
            Dream1SpecialObjectIsSpawned = true;
            Instantiate(objectToSpawn, spawnPositionfordream1, Quaternion.identity);
            Debug.Log($"[Room] 成功生成物品: {objectToSpawn.name}");
        }
    }
}