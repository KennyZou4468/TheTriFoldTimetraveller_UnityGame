using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Dream2ToiletPaperInteractable : Dream2Interactable
{
    [SerializeField] private GameObject[] toiletPaperObjects;
    [SerializeField] private Dream2Item toiletPaper;

    private bool canInteract = true;

    private System.Collections.IEnumerator DisableAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }

    public override void Interact()
    {
        for (int i = 0; i < toiletPaperObjects.Length; i++)
        {
            if (toiletPaperObjects[i].activeSelf)
            {
                toiletPaperObjects[i].SetActive(false);
                Dream2Manager.Instance.AddItem(toiletPaper);
                if (i == toiletPaperObjects.Length - 1)
                {
                    canInteract = false;
                    StartCoroutine(DisableAfterDelay(0.01f));
                }
                return;
            }
        }
    }

    public override void PlayerInRange(bool _)
    {
        if (!canInteract)
        {
            playerInRange = false;
        }
        else
            base.PlayerInRange(_);
    }

    public override void MouseInRange(bool _)
    {
        if (!canInteract)
        {
            mouseInRange = false;
        }
        else
            base.MouseInRange(_);
    }
}