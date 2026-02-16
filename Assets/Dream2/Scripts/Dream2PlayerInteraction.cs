using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Dream2PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public List<Dream2Interactable> currentInteractables = new List<Dream2Interactable>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Dream2Interactable>(out Dream2Interactable interactable))
        {
            if (other.isTrigger)
            {
                interactable.PlayerInRange(true);
                if (!currentInteractables.Contains(interactable))
                {
                    currentInteractables.Add(interactable);
                    Dream2PlayerAim.Instance.playerInRange = true;
                }
            }
        } 
        else if (other.TryGetComponent<Dream2Gate>(out Dream2Gate gate))
        {
            if (other.isTrigger)
            {
                gate.CollideWithGate();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<Dream2Interactable>(out Dream2Interactable interactable))
        {
            if (other.isTrigger)
            {
                interactable.PlayerInRange(false);
                currentInteractables.Remove(interactable);
                Dream2PlayerAim.Instance.playerInRange = false;
            }
        }
    }

    public void TryInteract()
    {
        if (currentInteractables.Count > 0)
        {
            foreach (Dream2Interactable interactable in currentInteractables)
            {
                interactable.TryInteract();
            }
        }
    }
}