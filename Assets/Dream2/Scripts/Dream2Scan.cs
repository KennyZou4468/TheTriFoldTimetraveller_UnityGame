using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Dream2Scan : MonoBehaviour
{
    // [Header("Scan Mode Settings")]
    // [SerializeField] private Color scanModeColor; // Color to turn the game world to in scan mode
    // [SerializeField] private Camera playerCamera; // Reference to the camera
    // [SerializeField] private GameObject scanUI;

    [SerializeField] private Dream2InventoryUI inventoryUI;

    private bool isScanning = false;

    private void LoadDreamScene(string sceneName)
    {
        Debug.Log($"Loading next dream: {sceneName}");
        SceneManager.LoadScene(sceneName);
        Dream2Data dream2Data = GameDataController.Instance.GetSceneData<Dream2Data>("Dream2");
        dream2Data.IsCleared = true;
        dream2Data.Score = 100;
        dream2Data.s4 = true;
    }
    private void OnScan(InputValue inputValue)
    {
        // float rawValue = inputValue.Get<float>();
        // bool wasScanning = isScanning;
        // isScanning = rawValue > 0.5f;
        
        // Debug.Log($"Raw value: {rawValue}, Was: {wasScanning}, Now: {isScanning}");
        // if (!wasScanning && isScanning)
        // {
        //     // Change the game's color to greyscale (or another effect)
        //     SetCameraColorFilter(scanModeColor);
        //     // Highlight interactable objects
        //     HighlightInteractables(true);
        // } 
        // else if (wasScanning && !isScanning)
        // {
        //     // Reset the game's color
        //     SetCameraColorFilter(Color.white);
        //     // Remove highlight from interactable objects
        //     HighlightInteractables(false);
        // }


        inventoryUI.ToggleInventory();
    }

    // private void SetCameraColorFilter(Color color)
    // {
    //     // Apply a color filter to the camera or adjust the render settings
    //     playerCamera.backgroundColor = color;
    // }

    private void HighlightInteractables(bool highlight)
    {
        // Enable or disable highlighting of interactables
        var interactables = FindObjectsOfType<Dream2Interactable>();
        foreach (var interactable in interactables)
        {
            interactable.ShowHighlight(highlight);
        }
    }
}
