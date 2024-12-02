using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour {
    private List<InventoryItem> items = new List<InventoryItem>();
    [SerializeField]
    private int maxItems = 1;

    /// <summary>
    /// Add an item to the inventory
    /// </summary>
    /// <param name="item"></param>
    public bool AddItem(InventoryItem item)
    {
        if (items.Count >= maxItems)
        {
            return false;
        }
        items.Add(item);
        return true;
    }

    /// <summary>
    /// Remove an item from the inventory
    /// </summary>
    /// <param name="item"></param>
    public void RemoveItem(InventoryItem item)
    {
        items.Remove(item);
    }

    /// <summary>
    /// Remove an item from the inventory by index
    /// </summary>
    /// <param name="index"></param>
    public void RemoveItem(int index)
    {
        items.RemoveAt(index);
    }

    /// <summary>
    /// Get an item from the inventory by index
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public InventoryItem GetItem(int index)
    {
        if (index < 0 || index >= items.Count)
        {
            return null;
        }
        return items[index];
    }

    /// <summary>
    /// Get the size of the inventory
    /// </summary>
    /// <returns></returns>
    public int GetInventorySize()
    {
        return maxItems;
    }

    public int GetItemCount()
    {
        return items.Count;
    }

    /// <summary>
    /// Clears the inventory
    /// </summary>
    public void ClearInventory()
    {
        items.Clear();
    }

    /// <summary>
    /// Check if the inventory contains an item
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool ContainsItem(InventoryItem item)
    {
        return items.Contains(item);
    }

    /// <summary>
    /// Check if the inventory contains an item by name
    /// </summary>
    /// <param name="itemName"></param>
    /// <returns></returns>
    public bool ContainsItem(string itemName)
    {
        foreach (InventoryItem item in items)
        {
            if (item.name == itemName)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Get the entire inventory
    /// </summary>
    /// <returns></returns>
    public List<InventoryItem> GetInventoryItems()
    {
        return items;
    }

    /// <summary>
    /// Set the maximum number of items that can be held in the inventory
    /// </summary>
    /// <param name="maxItems"></param>
    public void SetMaxItems(int maxItems)
    {
        this.maxItems = maxItems;
    }
}