using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class Dream2InventoryUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject inventoryPanel;       // Inventory UI Panel
    [SerializeField] private GameObject gameCursor;
    [SerializeField] private GameObject uiCursor;             // Custom UI cursor GameObject
    [SerializeField] private GameObject itemSlotPrefab;       // Inventory slot prefab for items
    [SerializeField] private Transform itemGridParent;        // Parent of all inventory slots (GridLayoutGroup)

    [Header("Input Actions")]
    [SerializeField] private InputActionReference uiMoveAction;  // Mouse movement action for UI
    [SerializeField] private InputActionReference uiClickAction; // Mouse click action for UI
    [SerializeField] private InputActionReference closeInventoryAction; // Action to close inventory (I key)
    
    [SerializeField] private PlayerInput playerInput;  // Reference to PlayerInput

    private void Start()
    {
        closeInventoryAction.action.performed += _ => ToggleInventory();  // "I" key for inventory toggle
        inventoryPanel.SetActive(false); // Inventory is closed by default
        uiCursor.SetActive(false);      // Hide the UI cursor initially
        RefreshUI();                    // Initialize the UI

        Dream2Inventory.Instance.InventoryChanged += RefreshUI;
    }


    private IEnumerator ChangeModeForDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        uiCursor.SetActive(false);
        Dream2PlayerAim.Instance.isCameraLocked = false;
        gameCursor.GetComponent<SpriteRenderer>().enabled = true;
    }

    public void ToggleInventory()
    {
        bool show = !inventoryPanel.activeSelf;

        if (show)
        {
            Time.timeScale = 0f;
            uiCursor.SetActive(true);
            gameCursor.GetComponent<SpriteRenderer>().enabled = false;
            Dream2PlayerAim.Instance.isCameraLocked = true;
            playerInput.SwitchCurrentActionMap("UI");
        }
        else
        {
            playerInput.SwitchCurrentActionMap("Player");
            Time.timeScale = 1f;
            StartCoroutine(ChangeModeForDelay(0.01f));
        }
        if (show)
            RefreshUI();
        
        inventoryPanel.SetActive(show);
    }

    private void Update()
    {
        if (uiCursor.activeSelf)
        {
            Vector2 cursorPos = Mouse.current.position.ReadValue();
            uiCursor.transform.position = cursorPos;
        }
    }

    private void RefreshUI()
    {
        // Clear existing UI slots
        foreach (Transform child in itemGridParent)
            Destroy(child.gameObject);

        // Add inventory slots based on the player's items
        foreach (Dream2Item item in Dream2Inventory.Instance.GetItemList())
        {
            GameObject slot = Instantiate(itemSlotPrefab, itemGridParent);
            Dream2InventorySlot slotScript = slot.GetComponent<Dream2InventorySlot>();
            slotScript.SetItem(item);
        }
    }
}
