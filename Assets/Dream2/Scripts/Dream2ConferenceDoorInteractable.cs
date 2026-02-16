using UnityEngine;
using UnityEngine.UIElements;

public class Dream2ConferenceDoorInteractable : Dream2Interactable
{
    [Header("Door Settings")]
    [SerializeField] private Dream2Item key;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform physicalCollider;
    [SerializeField] private string lockedDialogue = "This door is locked";
    [SerializeField] private string openDialogue = "Door unlocked";
    [SerializeField] private string lockDialogue = "Door locked";


    private bool isLocked = true;
    private bool hasKey = false;

    private Vector3 position;
    private Quaternion rotation;

    protected override void Start()
    {
        base.Start();
        rb.isKinematic = true;
        position = physicalCollider.position;
        rotation = physicalCollider.rotation;
    }

    private void Update()
    {
        transform.SetPositionAndRotation(physicalCollider.position, physicalCollider.rotation);
    }

    public override void Interact()
    {
        hasKey = Dream2Manager.Instance.HasItem(key);
        // hasKey = true;
        if (isLocked && !hasKey)
        {
            ShowLockedDialogue();
        } 
        else if (isLocked && hasKey)
        {
            isLocked = false;
            rb.isKinematic = false;
            ShowOpenDialogue();
        } 
        else if (!isLocked && hasKey)
        {
            ShowLockDialogue();
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            isLocked = true;
            rb.isKinematic = true;
            rb.transform.SetPositionAndRotation(position, rotation);
        }
    }

    private void ShowLockedDialogue()
    {
        Dream2Manager.Instance.SetFlag("TriedConferenceDoor", true);
        if (textMeshProUGUI != null)
        {
            textMeshProUGUI.gameObject.SetActive(true);
            textMeshProUGUI.text = lockedDialogue;
            if (textHideCoroutine != null)
                StopCoroutine(textHideCoroutine);
            textHideCoroutine = StartCoroutine(HideTextAfterDelay(textDisplayDuration));
        }
    }

    private void ShowOpenDialogue()
    {
        if (textMeshProUGUI != null)
        {
            textMeshProUGUI.gameObject.SetActive(true);
            textMeshProUGUI.text = openDialogue;
            if (textHideCoroutine != null)
                StopCoroutine(textHideCoroutine);
            textHideCoroutine = StartCoroutine(HideTextAfterDelay(textDisplayDuration));
        }
    }   

    private void ShowLockDialogue()
    {
        if (textMeshProUGUI != null)
        {
            textMeshProUGUI.gameObject.SetActive(true);
            textMeshProUGUI.text = lockDialogue;
            if (textHideCoroutine != null)
                StopCoroutine(textHideCoroutine);
            textHideCoroutine = StartCoroutine(HideTextAfterDelay(textDisplayDuration));
        }
    }
}