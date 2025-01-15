using System;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(InventoryManager))]
public class PlayerPickup : PlayerEpress {
    [Tooltip("CollisionLayer of the player, basically whatever the picked up item should ignore")]
    public LayerMask exclusionMask;
    [Tooltip("CollisionLayer of the pickedup object, what the player should ignore")]
    public string PickUpMask;

    [Space(10)]
    public Transform holdingPosition;
    [Tooltip("The box collider that will be used to detect items")]
    public TriggerTracker boxCollider;
    public float speed = 5f;
    private InventoryManager inventoryManager;

    private void Start()
    {
        int maxItems = 1;
        inventoryManager = GetComponent<InventoryManager>();
        inventoryManager.SetMaxItems(maxItems);
    }

    public override void OnPress(){
        if (inventoryManager.GetInventorySize() == inventoryManager.GetItemCount())
        {
            LetGoItem();
        }else
        {
            PickupItem();
        }
    }

    public override float GetDistance() {
        if (inventoryManager.GetInventorySize() == inventoryManager.GetItemCount())
        {
            return 0;
        }else
        {
            foreach (Collider collider in boxCollider.colliders)
            {
                if (collider.CompareTag("PickUpItem") && collider.GetComponent<LerpTo>() == null)
                {
                    return MathUtil.GetDistanceXZ(transform.position, collider.transform.position);
                }
            }
            return Mathf.Infinity;
        }
    }

    public override float GetLookAt() {
        if (inventoryManager.GetInventorySize() == inventoryManager.GetItemCount())
        {
            return 1;
        }else
        {
            foreach (Collider collider in boxCollider.colliders)
            {
                if (collider.CompareTag("PickUpItem") && collider.GetComponent<LerpTo>() == null)
                {
                    Vector3 localPos = (collider.transform.position - transform.position).normalized;
                    return Vector3.Dot(transform.forward, localPos);
                }
            }
            return -1;
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

                if (collider.GetComponent<InteractPopUpRegistry>() != null)
                {
                    Destroy(collider.GetComponent<InteractPopUpRegistry>());
                }

                // Add the item to the inventory
                if (inventoryManager.AddItem(item))
                {
                    LerpTo lerpTo = collider.AddComponent<LerpTo>();
                    lerpTo.speed = speed;
                    lerpTo.target = holdingPosition;

                    // set exclusion layers picked up object
                    PlayerInventoryItem liveItem = GetCurrentItem();
                    Rigidbody rb = collider.GetComponent<Rigidbody>();
                    if (rb){
                        liveItem.layerMask = rb.excludeLayers;
                        rb.excludeLayers = exclusionMask | rb.excludeLayers;
                        rb.isKinematic = false;
                        rb.linearVelocity = Vector3.zero;
                        int DefaultMask = 0;
                        if (collider.gameObject.layer == DefaultMask)
                        {
                            collider.gameObject.layer = LayerMask.NameToLayer(PickUpMask);                        
                        }
                    }
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
        PlayerInventoryItem item = GetCurrentItem();
        Rigidbody rb = item.itemObject.GetComponent<Rigidbody>();
        if (rb){
            rb.excludeLayers = item.layerMask;
        }
        int DefaultMask = 0;
        if (item.itemObject.layer != DefaultMask)
        {
            item.itemObject.layer = DefaultMask;                        
        }
        // Remove the item from the inventory
        inventoryManager.RemoveItem(inventoryManager.GetInventorySize() - 1);

        // Remove the LerpTo component from the item
        Destroy(item.itemObject.GetComponent<LerpTo>());
    }

    /// <summary>
    /// Gets the currently held item
    /// </summary>
    /// <returns></returns>
    private PlayerInventoryItem GetCurrentItem(){
        return (PlayerInventoryItem)inventoryManager.GetItem(inventoryManager.GetInventorySize() - 1);
    }
}