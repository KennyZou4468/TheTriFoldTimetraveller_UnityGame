using UnityEngine;
using UnityEngine.UIElements;

public class Dream2LabDoorInteractable : Dream2Interactable
{
    [Header("Lab Door Settings")]
    [SerializeField] private Dream2Item uniform;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform physicalCollider;
    [SerializeField] private string lockedDialogue;
    [SerializeField] private string openDialogue;
    [SerializeField] private string npcName = "Guard";
    [SerializeField] private Animator guardAnimator;

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
        hasKey = Dream2Manager.Instance.GetEquippedOutfit() == uniform;
        if (isLocked && !hasKey)
        {
            ShowDialogue(lockedDialogue);
            guardAnimator.SetTrigger("trigger");
        } 
        else if (isLocked && hasKey)
        {
            isLocked = false;
            rb.isKinematic = false;
            ShowDialogue(openDialogue);
        } 
    }

    private void ShowDialogue(string dialogue)
    {
        if (textMeshProUGUI != null)
        {
            textMeshProUGUI.gameObject.SetActive(true);
            textMeshProUGUI.text = 
                $"<color={redColorHex}>{npcName + ": "}</color>" + $"<color={blueColorHex}>{dialogue}</color>";
            if (textHideCoroutine != null)
                StopCoroutine(textHideCoroutine);
            textHideCoroutine = StartCoroutine(HideTextAfterDelay(textDisplayDuration));
        }
    }
}