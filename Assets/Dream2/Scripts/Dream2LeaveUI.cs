using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class Dream2LeaveUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject inventoryPanel;       // Inventory UI Panel
    [SerializeField] private GameObject gameCursor;
    [SerializeField] private GameObject uiCursor;             // Custom UI cursor GameObject

    [Header("Input Actions")]
    [SerializeField] private InputActionReference closeInventoryAction; // Action to close inventory (I key)

    [SerializeField] private PlayerInput playerInput;  // Reference to PlayerInput

    private void Start()
    {
        inventoryPanel.SetActive(false); // Inventory is closed by default
        uiCursor.SetActive(false);      // Hide the UI cursor initially
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

    public void OnClickYesButton()
    {
        Time.timeScale = 1f;
        Dream2Manager.Instance.EndDream2();
    }
}
