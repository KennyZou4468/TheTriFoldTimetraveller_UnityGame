using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Dream2DrunkInteractable : Dream2Interactable
{
    [SerializeField] private GameObject body;
    [SerializeField] private GameObject bodyNaked;
    [SerializeField] private Dream2Item clothItem;

    private bool canInteract = true;

    private System.Collections.IEnumerator DisableAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.GetComponent<Collider2D>().enabled = false;
    }

    public override void Interact()
    {
        Dream2Manager.Instance.AddItem(clothItem);
        body.SetActive(false);
        bodyNaked.SetActive(true);
        canInteract = false;
        StartCoroutine(DisableAfterDelay(0.01f));
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