using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic; // 引用队列需要这个
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI 组件")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;

    // 消息队列：存着所有还没说的话
    private Queue<string> sentences = new Queue<string>();

    private bool isSpeaking = false; // 标记当前是否正在说话

    void Awake()
    {
        if (Instance == null) Instance = this;
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    // --- 外部调用 ---
    public void ShowMessage(string msg)
    {
        // 把新消息加入队列
        sentences.Enqueue(msg);

        // 如果当前没人在说话，就开始播放下一条
        if (!isSpeaking)
        {
            StartCoroutine(PlayNextSentence());
        }
    }

    IEnumerator PlayNextSentence()
    {
        isSpeaking = true; // 占用麦克风

        // 只要队列里还有话
        while (sentences.Count > 0)
        {
            string currentMsg = sentences.Dequeue(); // 取出第一条

            dialoguePanel.SetActive(true);
            dialogueText.text = "";

            // 打字效果
            foreach (char letter in currentMsg.ToCharArray())
            {
                dialogueText.text += letter;
                yield return new WaitForSeconds(0.05f);
            }

            // 说完这句话，悬停 3 秒给玩家看
            yield return new WaitForSeconds(3f);
        }

        // 所有话都说完了
        dialoguePanel.SetActive(false);
        isSpeaking = false; // 释放麦克风
    }
}