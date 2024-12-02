using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(InventoryManager))]
public class PlayerPickup : MonoBehaviour {
    public Transform holdingPosition;
    [Tooltip("The box collider that will be used to detect items")]
    public TriggerTracker boxCollider;
    public float reachDistance = 2f;
    public float speed = 5f;
    private InventoryManager inventoryManager;

    private void Start()
    {
        int maxItems = 1;
        inventoryManager = GetComponent<InventoryManager>();
        inventoryManager.SetMaxItems(maxItems);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (inventoryManager.GetInventorySize() == inventoryManager.GetItemCount())
            {
                LetGoItem();
            }else
            {
                PickupItem();
            }
        }
    }

    /// <summary>
    /// Pick up an item
    /// </summary>
    private void PickupItem()
    {
        // Check if the player is near an item
        foreach (Collider collider in boxCollider.colliders)
        {
            if (collider.CompareTag("PickUpItem") && collider.GetComponent<LerpTo>() == null)
            {
                // Get the item component from the hit object
                PlayerInventoryItem item = new PlayerInventoryItem(collider.gameObject.name, collider.gameObject);

                // Add the item to the inventory
                if (inventoryManager.AddItem(item))
                {
                    LerpTo lerpTo = collider.AddComponent<LerpTo>();
                    lerpTo.speed = speed;
                    lerpTo.target = holdingPosition;
                }else{
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Let go of the last item in the inventory
    /// </summary>
    private void LetGoItem()
    {
        // Get the last item in the inventory
        PlayerInventoryItem item = (PlayerInventoryItem)inventoryManager.GetItem(inventoryManager.GetInventorySize() - 1);

        // Remove the item from the inventory
        inventoryManager.RemoveItem(inventoryManager.GetInventorySize() - 1);

        // Remove the LerpTo component from the item
        Destroy(item.itemObject.GetComponent<LerpTo>());
    }
}