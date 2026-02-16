using UnityEngine;
using System.Collections.Generic;
using System.Linq; // 需要使用 LINQ 扩展方法

public class CashSpawner : MonoBehaviour
{
    // ==========================================
    // 【修改】：定义三种不同 Cash Prefab 及其概率
    // ==========================================
    [System.Serializable]
    public class CashItem
    {
        [Tooltip("拖入 Cash 的预制件 (Prefab)")]
        public GameObject cashPrefab;

        [Tooltip("生成权重 (70, 25, 5)")]
        [Range(0, 100)]
        public int spawnWeight = 1;
    }

    [Header("Cash Prefabs & Weights")]
    public List<CashItem> cashItems = new List<CashItem>();

    [Tooltip("拖入所有可能的生成点 (Transform)")]
    public List<Transform> spawnPoints;

    public static CashSpawner Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 游戏开始时生成第一个 Cash
        // 确保列表不为空，并且总权重大于零
        if (cashItems.Count > 0 && cashItems.Sum(item => item.spawnWeight) > 0)
        {
            RespawnCash();
        }
        else
        {
            Debug.LogError("CashSpawner: 未设置 Cash 预制件或总权重为零!");
        }
    }

    // Inside CashSpawner.cs
    public void RespawnCash()
    {
        if (spawnPoints.Count == 0)
        {
            Debug.LogError("CashSpawner: 场景中未设置生成点!");
            return;
        }

        // 1. 根据权重选择 Cash Prefab
        GameObject cashToSpawn = SelectCashByWeight();

        if (cashToSpawn == null)
        {
            Debug.LogWarning("无法根据权重选择 Cash Prefab。");
            return;
        }

        // 2. 随机选择一个生成点
        int randomIndex = Random.Range(0, spawnPoints.Count);
        Transform randomPoint = spawnPoints[randomIndex];

        // 3. 在该点生成新的 Cash 实例
        GameObject newCashObj = Instantiate(cashToSpawn, randomPoint.position, Quaternion.identity);

        // 4. 获取组件并启动移动
        // 我们假设这三种 Cash Prefab 上都挂载了名为 Cash 的组件
        Cash newCash = newCashObj.GetComponent<Cash>();
        if (newCash != null)
        {
            newCash.StartMovement();
        }
        else if (newCashObj.GetComponent<Pig>() != null)
        {
            Pig newPig = newCashObj.GetComponent<Pig>();
            newPig.StartMovement();
        }
        else if (newCashObj.GetComponent<Ring>() != null)
        {
            Ring newRing = newCashObj.GetComponent<Ring>();
            newRing.StartMovement();
        }
        else
        {
            Debug.LogError($"新生成的对象 {cashToSpawn.name} 上没有找到组件。");
        }

    }

    /// <summary>
    /// 根据权重随机选择一个 Cash Prefab。
    /// </summary>
    private GameObject SelectCashByWeight()
    {
        // 计算总权重
        int totalWeight = cashItems.Sum(item => item.spawnWeight);

        if (totalWeight <= 0) return null;

        // 生成一个 0 到总权重之间的随机数
        int randomNumber = Random.Range(0, totalWeight);
        int currentWeight = 0;

        // 遍历所有物品，累加权重直到超过随机数
        foreach (var item in cashItems)
        {
            currentWeight += item.spawnWeight;

            if (randomNumber < currentWeight)
            {
                // 命中该物品
                return item.cashPrefab;
            }
        }

        // 理论上不会运行到这里，但以防万一
        return null;
    }
}