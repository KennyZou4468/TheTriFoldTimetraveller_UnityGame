using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class Dream2JanitorInteractable : Dream2Interactable
{
    [SerializeField] private string npcName;
    [SerializeField] private Dream2Item janitorRoomKey;
    [SerializeField] private Dream2Item janitorUniform;
    [SerializeField] private Dream2Item policeUniform;


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
        if (Dream2Manager.Instance.GetFlag("SeekingToiletPaper"))
        {
            if (!Dream2Manager.Instance.HasItem(janitorRoomKey))
            {
                Dream2Manager.Instance.AddItem(janitorRoomKey);
                return new List<string> {
                "Toilet paper used up? Go to the janitor room to get some.", "Here's the key."};
            }
            else
            {
                return new List<string> {
                "Can't find janitor room?", 
                "It's right next to the monitor room.", 
                "Go out, turn right, and walk until you reach the end."};
            }
        }

        if (Dream2Manager.Instance.GetEquippedOutfit() == janitorUniform)
        {
            return new List<string> {
                "I thought I was the only one on duty today."};
        } 
        else if (Dream2Manager.Instance.GetEquippedOutfit() == policeUniform)
        {
            return new List<string> {
                "Hello officer, also on duty this weekend?"};
        }

        return new List<string> {"Hello."};
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
