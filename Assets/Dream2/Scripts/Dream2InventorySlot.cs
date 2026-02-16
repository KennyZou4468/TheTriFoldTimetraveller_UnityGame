using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Dream2InventorySlot : MonoBehaviour
{
    [Header("UI Elements")]
    public Image iconImage;  // Icon of the item
    public TextMeshProUGUI itemNameText;  // Name of the item
    public TextMeshProUGUI bgText;
    private Dream2Item item;

    private void Start()
    {
        itemNameText.gameObject.SetActive(false);
        bgText.gameObject.SetActive(false);

    }
    public void SetItem(Dream2Item newItem)
    {
        item = newItem;
        iconImage.sprite = newItem.icon;
        itemNameText.text = newItem.itemName;
        bgText.text = newItem.itemName;
    }

    public void OnHoverEnter()
    {
        itemNameText.gameObject.SetActive(true);
        bgText.gameObject.SetActive(true);
    }

    public void OnHoverExit()
    {
        itemNameText.gameObject.SetActive(false);
        bgText.gameObject.SetActive(false);
    }

    public void OnClick()
    {
        if (item != null && item.type == ItemType.Clothing)
        {
            Dream2Inventory.Instance.EquipItem(item);
        } 
        else if (item != null && item.type == ItemType.File)
        {
            Dream2Inventory.Instance.EquipItem(item);
        }
    }
}
