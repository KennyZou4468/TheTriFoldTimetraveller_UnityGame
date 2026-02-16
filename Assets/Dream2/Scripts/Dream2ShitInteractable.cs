using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class Dream2ShitInteractable : Dream2Interactable
{
    [SerializeField] private string npcName;
    [SerializeField] private Dream2Item toiletPaper;

    private List<string> currentDialogue; // The active dialogue sequence
    private int index = 0;                // Current line in the dialogue

    private bool canInteract = true;

    protected override void Start()
    {
    }

    private System.Collections.IEnumerator DisableInteraction(float delay)
    {
        canInteract = false;
        yield return new WaitForSeconds(delay);
        canInteract = true;
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
                if (currentDialogue.Count == 1)
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
        if (!Dream2Manager.Instance.GetFlag("SeekingToiletPaper"))
        {
            Dream2Manager.Instance.SetFlag("SeekingToiletPaper", true);
            return new List<string> {
                $"Fuck, thank god someone finally showed up.",
                "Can you figure out a way to get me some toilet paper?",
                "We're out of everything here!",
                "Can you go ask the janitor or something?",
                "Please, I've been stuck here for over an hour!"};
        }
        else if (Dream2Manager.Instance.GetFlag("SeekingToiletPaper") && !Dream2Manager.Instance.HasItem(toiletPaper))
        {
            return new List<string> {
                "Hey, did you manage to get that toilet paper for me?",
                "I'm really in a bind here...",
                "Please hurry! Please!!!"};
        }
        else if (Dream2Manager.Instance.GetFlag("SeekingToiletPaper") && Dream2Manager.Instance.HasItem(toiletPaper)
                && !Dream2Manager.Instance.GetFlag("YouAreSoKind"))
        {
            Dream2Manager.Instance.SetFlag("YouAreSoKind", true);
            Dream2Manager.Instance.RemoveItem(toiletPaper);
            return new List<string> {
                "Fuck, thanks so much, you've saved my hemorrhoids!",
                "I‘m really grateful, don't even know how to repay you."

            };
        }
        else
        {
            return new List<string> {
                "Don't worry, just a little constipated."
            };
        }


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
