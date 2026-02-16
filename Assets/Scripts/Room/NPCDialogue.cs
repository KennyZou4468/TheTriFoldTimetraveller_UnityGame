using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPCDialogueChoiceKeyboardWithPrompt : MonoBehaviour
{
    [System.Serializable]
    public class DialogueNode
    {
        [TextArea]
        public string sentence;           // NPC 说的话
        public string[] choices;          // 选项文字
        public int[] nextNodeIndex;       // 每个选项指向下一段对话（-1 结束）
    }

    public DialogueNode[] dialogueNodes;

    [Header("UI References")]
    public GameObject interactUI;       // 靠近显示提示，例如 "按 F 对话"
    public GameObject dialogueUI;       // 对话面板
    public TextMeshProUGUI dialogueText;
    public Transform choicesContainer;
    public GameObject choicePrefab;

    private int currentNode = -1;
    private bool inDialogue = false;
    private bool playerInRange = false;

    private List<GameObject> spawnedChoices = new List<GameObject>();
    private int currentSelection = 0;
    private Player1Controller playerController;

    void Start()
    {
        if (interactUI != null) interactUI.SetActive(false);
        dialogueUI.SetActive(false);
    }

    void Update()
    {
        if (playerInRange && !inDialogue && Input.GetKeyDown(KeyCode.F))
        {
            // 玩家按 F 开始对话
            StartDialogue();
        }

        if (inDialogue)
        {
            HandleSelectionMove();

            // 回车或空格确认选项
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                ConfirmChoice();
            }
        }
    }

    void StartDialogue()
    {
        inDialogue = true;
        currentNode = 0;
        dialogueUI.SetActive(true);
        if (interactUI != null) interactUI.SetActive(false);

        // 🔒 锁住玩家移动
        if (playerController == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerController = player.GetComponent<Player1Controller>();
        }
        if (playerController != null)
            playerController.LockMovement();

        ShowNode();
    }


    void ShowNode()
    {
        ClearChoices();
        DialogueNode node = dialogueNodes[currentNode];
        dialogueText.text = node.sentence;

        for (int i = 0; i < node.choices.Length; i++)
        {
            GameObject btn = Instantiate(choicePrefab, choicesContainer);
            TextMeshProUGUI txt = btn.GetComponentInChildren<TextMeshProUGUI>();
            txt.text = node.choices[i];
            spawnedChoices.Add(btn);
        }

        currentSelection = 0;
        UpdateChoiceHighlight();
    }

    void HandleSelectionMove()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            currentSelection = Mathf.Max(0, currentSelection - 1);
            UpdateChoiceHighlight();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            currentSelection = Mathf.Min(spawnedChoices.Count - 1, currentSelection + 1);
            UpdateChoiceHighlight();
        }
    }

    void UpdateChoiceHighlight()
    {
        for (int i = 0; i < spawnedChoices.Count; i++)
        {
            TextMeshProUGUI txt = spawnedChoices[i].GetComponentInChildren<TextMeshProUGUI>();
            txt.color = (i == currentSelection ? Color.red : Color.black); // 红色选中，黑色默认
        }
    }

    void ConfirmChoice()
    {
        DialogueNode node = dialogueNodes[currentNode];
        int next = node.nextNodeIndex[currentSelection];

        if (next == -1)
        {
            EndDialogue();
        }
        else
        {
            currentNode = next;
            ShowNode();
        }
    }

    void ClearChoices()
    {
        foreach (GameObject c in spawnedChoices)
            Destroy(c);
        spawnedChoices.Clear();
    }

    void EndDialogue()
    {
        inDialogue = false;
        dialogueUI.SetActive(false);
        ClearChoices();

        // 🔓 解锁玩家移动
        if (playerController != null)
            playerController.UnlockMovement();

        if (playerInRange && interactUI != null) interactUI.SetActive(true);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("OnTriggerEnter2D---NPC");
            playerInRange = true;
            if (!inDialogue && interactUI != null)
                interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactUI != null)
                interactUI.SetActive(false);
            if (inDialogue)
                EndDialogue();
        }
    }
}
