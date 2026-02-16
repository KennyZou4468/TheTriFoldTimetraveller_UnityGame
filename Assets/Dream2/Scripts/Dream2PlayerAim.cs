using UnityEngine;
using UnityEngine.InputSystem;

public class Dream2PlayerAim : MonoBehaviour
{

    public static Dream2PlayerAim Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    [Header("Aim Settings")]
    [SerializeField] private float displacementCoeff = 0.2f;
    
    [Header("References")]
    [SerializeField] private Dream2PlayerMovement playerMovement;
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Transform legsTransform;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private GameObject cursor;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private Dream2PlayerInteraction interaction;
    private void OnInteract(InputValue inputValue)
    {
        interaction.TryInteract();
    }
    
    private Vector2 mouseWorldPos;
    private Vector3 mouseScreenPos;
    public bool playerInRange = false;
    public bool mouseInRange = false;

    public bool isCameraLocked = false;

    private Dream2Interactable currentHover;

    private void Update()
    {
        if (!isCameraLocked)
        {
            mouseWorldPos = playerCamera.ScreenToWorldPoint(mouseScreenPos);
            if (playerMovement.MoveInput().magnitude > 0.1f)
            {
                LookAt(legsTransform, (Vector2)transform.position + playerMovement.MoveInput());
            }
            else
            {
                LookAt(legsTransform, mouseWorldPos);
            }
            LookAt(bodyTransform, mouseWorldPos);
            LookAt(transform, mouseWorldPos);
            
            CheckMouseHover();
            UpdateCursor();
            UpdateCamera();
        }
    }

    private void LookAt(Transform transform, Vector2 targetPos)
    {
        Vector2 aimDirection = (targetPos - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnLook(InputValue inputValue)
    {
        mouseScreenPos = inputValue.Get<Vector2>();
        mouseScreenPos.z = -playerCamera.transform.position.z;
    }

    private void UpdateCursor()
    {
        cursor.transform.position = mouseWorldPos;
        
        if (mouseInRange)
        {
            CursorHover(true);
            if (interaction.currentInteractables.Contains(currentHover))    
                    CursorSelect(true);
            else
            {
                CursorSelect(false);
            }
        }
        else
        {
            CursorHover(false);
            CursorSelect(false);
        }
    }

    private void UpdateCamera()
    {
        if (playerCamera != null)
        {
            Vector3 playerPos = transform.position;
            Vector3 targetPos = (Vector2)playerPos + displacementCoeff * (mouseWorldPos - (Vector2)playerPos);
            
            // Smooth camera follow with look ahead
            Vector3 cameraPos = Vector3.Lerp(playerCamera.transform.position, targetPos, Time.deltaTime * 10f);
            cameraPos.z = playerCamera.transform.position.z;
            playerCamera.transform.position = cameraPos;
        }
    }

    private void CheckMouseHover()
    {
        Vector2 mousePos = playerCamera.ScreenToWorldPoint(mouseScreenPos);

        // Raycast to check if mouse is over an interactable object
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, interactableLayer);

        if (hit.collider != null)
        {
            Dream2Interactable interactable = hit.collider.GetComponent<Dream2Interactable>();

            if (interactable != currentHover)
            {
                // Trigger OnMouseExit on previous interactable if hover changed
                currentHover?.MouseInRange(false);

                // Update current hovered interactable
                currentHover = interactable;
                currentHover.MouseInRange(true);
                mouseInRange = true;
            }
        }
        else
        {
            if (currentHover != null)
            {
                currentHover.MouseInRange(false);
                currentHover = null;
                mouseInRange = false;
            }
        }
    }

    public Vector2 MouseWorldPos()
    {
        return mouseWorldPos;
    }

    public Vector2 MouseScreenPos()
    {
        return mouseScreenPos;
    }

    private void CursorHover(bool isHovering)
    {
        cursor.GetComponent<Animator>().SetBool("isHovering", isHovering);
    }

    private void CursorSelect(bool isSelecting)
    {
        cursor.GetComponent<Animator>().SetBool("isSelecting", isSelecting);
    }
}