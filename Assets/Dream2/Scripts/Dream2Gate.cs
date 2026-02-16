using UnityEngine;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;

public class Dream2Gate : MonoBehaviour
{
    [SerializeField] private Dream2LeaveUI leaveUI;

    public void CollideWithGate()
    {
        leaveUI.ToggleInventory();
    }
}
