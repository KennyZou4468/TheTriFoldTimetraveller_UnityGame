using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Dream2/Item")]
public class Dream2Item : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public ItemType type;
    public int outfitIndex;

    // public string filePath; 
}

public enum ItemType
{
    Generic,
    Clothing,
    File
}
