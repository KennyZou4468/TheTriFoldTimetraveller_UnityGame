using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // 如果需要管理场景切换

public class Dream2Manager : MonoBehaviour
{
    public static Dream2Manager Instance;
    public GameObject interactionPanel;


    [Header("References")]
    public Dream2Inventory inventory;
    public GameObject player;

    public bool isFileGot = false;

    public bool fileDeleted = false;

    [SerializeField] Dream2Item corpoSuit;


    // Generic game state flags: quests, events, conditions
    private Dictionary<string, bool> flagDict = new Dictionary<string, bool>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        interactionPanel.SetActive(false);
        Cursor.visible = false;
        AddItem(corpoSuit);
        EquipOutfit(corpoSuit);
        Time.timeScale = 1f;
    }

    public bool HasItem(Dream2Item item)
    {
        return inventory.HasItem(item);
    }

    public void AddItem(Dream2Item item)
    {
        inventory.AddItem(item);
    }

    public void RemoveItem(Dream2Item item)
    {
        inventory.RemoveItem(item);
    }

    public void SetFlag(string flagName, bool value)
    {
        flagDict[flagName] = value;
    }

    public bool GetFlag(string flagName)
    {
        if (flagDict.ContainsKey(flagName))
            return flagDict[flagName];

        return false;
    }

    public Dream2Item GetEquippedOutfit()
    {
        return inventory.equippedOutfit;
    }

    public void EquipOutfit(Dream2Item outfit)
    {
        inventory.EquipItem(outfit);
    }


    [SerializeField] private Dream2Timer dream2Timer;
    public void EndDream2()
    {
        int timeLeft = (int)dream2Timer.timeLeft;
        Dream2Data dream2Data = GameDataController.Instance.GetSceneData<Dream2Data>("Dream2");
        //获取到Dream2的data之后更新数据
        if (dream2Data != null)
        {
            dream2Data.time = timeLeft;
            dream2Data.isFileDestroy = fileDeleted;
            dream2Data.isFileGot = isFileGot;
            dream2Data.IsCleared = true;
        }
        //fileDeleted = true;
        if (fileDeleted)
        {
            Debug.Log("文件已删除，触发特殊结局 ");
            if (interactionPanel != null)
            {
                bool isActive = interactionPanel.activeSelf;
                interactionPanel.SetActive(!isActive);
                Time.timeScale = 0f;
            }
        }
        else
        {
            SceneManager.LoadScene("Room");
        }
    }
}
