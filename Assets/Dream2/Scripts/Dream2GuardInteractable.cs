using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class Dream2GuardInteractable : Dream2Interactable
{
    [SerializeField] private string npcName;
    [SerializeField] private Dream2Item conferenceRoomKey;
    [SerializeField] private Dream2Item janitorUniform;

    private bool takeKeyAsJanitor = false;

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
        if (Dream2Manager.Instance.GetFlag("NoticedSomethingOdd"))
        {
            if (!Dream2Manager.Instance.HasItem(conferenceRoomKey) && 
                Dream2Manager.Instance.GetEquippedOutfit() != janitorUniform)
            {
                Dream2Manager.Instance.AddItem(conferenceRoomKey);
                takeKeyAsJanitor = false;
                return new List<string> {
                "Nah, it's just another guard and a man got drunk.",
                "They've been drinking way too much with those assholes next door.", 
                "Can't handle their liquor, idiots.",
                "If you're really that worried, take this key and go check for yourself. ", 
                "Don't bother me anymore."
                };
            } 
            else if (Dream2Manager.Instance.HasItem(conferenceRoomKey) && 
                Dream2Manager.Instance.GetEquippedOutfit() != janitorUniform && !takeKeyAsJanitor)
            {
                return new List<string> {
                "Oh, it's you again.",
                "Look, I don't want any trouble. Just doing my job here.",
                "If you really want to check the conference room, just go ahead.",
                "But don't cause any scenes, alright?"
                };
            }
            else if (Dream2Manager.Instance.HasItem(conferenceRoomKey) && 
                Dream2Manager.Instance.GetEquippedOutfit() != janitorUniform && takeKeyAsJanitor)
            {
                return new List<string> {
                "Don't bother me. I'm doing my job."
                };
            }

            if (!Dream2Manager.Instance.HasItem(conferenceRoomKey) && 
                Dream2Manager.Instance.GetEquippedOutfit() == janitorUniform)
            {
                Dream2Manager.Instance.AddItem(conferenceRoomKey);
                takeKeyAsJanitor = true;
                return new List<string> {
                "Hey, you're the janitor here, right?",
                "The meeting room over there got messed up by those assholes throwing a party.",
                "Go clean it up. Here's the key. ", 
                "Make sure to return it after you're done cleaning."
                };
            }
            else if (Dream2Manager.Instance.HasItem(conferenceRoomKey) && 
                Dream2Manager.Instance.GetEquippedOutfit() == janitorUniform && takeKeyAsJanitor)
            {
                return new List<string> {
                "Done cleaning yet? If not, get to it"
                };
            } 
            else if (Dream2Manager.Instance.HasItem(conferenceRoomKey) && 
                Dream2Manager.Instance.GetEquippedOutfit() == janitorUniform && !takeKeyAsJanitor)
            {
                return new List<string>
                {
                "Hey, you're the janitor here, right?",
                "The meeting room over there got messed up by those assholes throwing a party.",
                "Go clean it up. It should be open already."
                };
            }
        }
        return new List<string> {"Don't bother me. I'm doing my job."};
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
