using TMPro;
using UnityEngine;

public abstract class Dream2Interactable : MonoBehaviour
{
    [Header("Base Interaction Settings")]
    [SerializeField] protected GameObject highlightEffect;
    [SerializeField] protected TextMeshProUGUI textMeshProUGUI;
    protected string blueColorHex = "#4FF6FF";
    protected string redColorHex = "#FF5651";

    
    [Header("Base Dialogue Settings")]
    [SerializeField] protected float textDisplayDuration = 3f;
    
    public bool playerInRange = false;
    public bool mouseInRange = false;
    
    protected bool isShowingText = false;
    protected bool isTalking = false;

    protected Coroutine textHideCoroutine;

    protected virtual void Start()
    {
        if (highlightEffect != null)
            highlightEffect.SetActive(false);
        
        if (textMeshProUGUI != null)
            textMeshProUGUI.gameObject.SetActive(false);
    }


    public void TryInteract()
    {
        if (playerInRange && mouseInRange)
        {
            Interact();
        }
    }

    public virtual void Interact(){}
    
    protected virtual System.Collections.IEnumerator HideTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideText();
    }

    protected virtual void HideText()
    {
        if (textMeshProUGUI != null)
        {
            textMeshProUGUI.gameObject.SetActive(false);
            isShowingText = false;
            isTalking = false;
        }
    }

    public virtual void PlayerInRange(bool playerInRange)
    {
        this.playerInRange = playerInRange;
    }

    public virtual void MouseInRange(bool mouseInRange)
    {
        this.mouseInRange = mouseInRange;
    }

    public virtual void ShowHighlight(bool show)
    {
        if (highlightEffect != null)
            highlightEffect.SetActive(show);
    }
}