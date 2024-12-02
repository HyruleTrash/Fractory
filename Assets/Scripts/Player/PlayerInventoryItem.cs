using UnityEngine;

public class PlayerInventoryItem : InventoryItem{
    public GameObject itemObject;

    public PlayerInventoryItem(string name, GameObject itemObject) : base(name)
    {
        this.itemObject = itemObject;
    }
}