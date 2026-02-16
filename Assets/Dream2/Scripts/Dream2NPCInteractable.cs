using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class Dream2NPCInteractable : Dream2Interactable
{
    [SerializeField] private string npcName;
    [SerializeField] private List<string> dialogueLines;


    private List<string> currentDialogue; // The active dialogue sequence
    private int index = 0;                // Current line in the dialogue


    private bool canInteract = true;
    private System.Collections.IEnumerator DisableInteraction(float delay)
    {
        canInteract = false;
        yield return new WaitForSeconds(delay);
        canInteract = true;
    }

    protected override void Start()
    {
        base.Start();
    }

    public override void Interact()
    {
        if (canInteract)
        {
            if (!isTalking)
            {
                currentDialogue = ChooseDialogue();
                index = 0;
                isTalking = true;
                ShowDialogueLine();
                if (currentDialogue.Count ==1)
                {
                    textHideCoroutine = StartCoroutine(HideTextAfterDelay(textDisplayDuration));
                }
            }
            else
            {
                index++;
                if (index >= currentDialogue.Count)
                {
                    EndDialogue();
                }
                else if (index == currentDialogue.Count - 1)
                {
                    ShowDialogueLine();
                    textHideCoroutine = StartCoroutine(HideTextAfterDelay(textDisplayDuration));
                }
                else
                {
                    ShowDialogueLine();
                }
            }
        }
    }

    private List<string> ChooseDialogue()
    {
        return dialogueLines;
    }

    private void ShowDialogueLine()
    {
        textMeshProUGUI.text = $"<color={redColorHex}>{npcName + ": "}</color>" + $"<color={blueColorHex}>{currentDialogue[index]}</color>";
        textMeshProUGUI.gameObject.SetActive(true);
        isShowingText = true;
    }

    private void EndDialogue()
    {
        isTalking = false;
        index = 0;
        if (textMeshProUGUI != null)
            textMeshProUGUI.gameObject.SetActive(false);
        isShowingText = false;
        if (textHideCoroutine != null)
            StopCoroutine(textHideCoroutine);
        StartCoroutine(DisableInteraction(3f));
    }
}
