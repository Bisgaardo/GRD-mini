using System.Collections.Generic;
using UnityEngine;
using static TrashcanSystem;

public class UsedItem : MonoBehaviour
{
   private Vector3 originalPosition;
    private bool isDragging;
    private Camera cam;

    private List<Slot> lockedSlots = new List<Slot>();

    public int Size;
    public Playermovement playermovement;
    public bool isUsed;
    public LayerMask backpack;
    public bool assigned;

    public TrashType trashType;

    void Awake()
    {
        cam = Camera.main;
    }

    void OnMouseDown()
    {
        // Release previously locked slots
        foreach (Slot slot in lockedSlots)
            slot.ClearSlot();
        lockedSlots.Clear();

        originalPosition = transform.position;
        isDragging = true;
        playermovement.setUsedSelected(this);
    }

    void OnMouseDrag()
    {
        if (!isDragging) return;
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mousePos.x, mousePos.y, 0);
    }

    void OnMouseUp()
    {
        isDragging = false;
        TrySnap();
        playermovement.setUsedSelected(null);
        if (assigned && !isUsed)
        {
            playermovement.foodAmount--;
        }
    }
    
    void TrySnap()
    {
        // Find all slots currently overlapping this item's collider
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position,
            GetComponent<Collider2D>().bounds.size, 0f,backpack);

        List<Slot> overlappedSlots = new List<Slot>();
        foreach (Collider2D hit in hits)
        {
            Slot slot = hit.GetComponent<Slot>();
            if (slot != null)
                overlappedSlots.Add(slot);
        }

        // Check all overlapped slots are free
        foreach (Slot slot in overlappedSlots)
        {
            // Block invalid slot types
            if (!slot.CanPlaceUsedItem(this))
            {
                ReturnToOrigin();
                return;
            }

            // Block occupied slots
            if (slot.IsOccupied() || slot.usedIsOccupied())
            {
                ReturnToOrigin();
                return;
            }
        }

        // Need at least 1 slot to snap to
        if (overlappedSlots.Count != Size)
        {
            ReturnToOrigin();
            return;
        }

        // Snap position to center of overlapped slots
        Vector3 center = Vector3.zero;
        foreach (Slot slot in overlappedSlots)
            center += slot.transform.position;
        center /= overlappedSlots.Count;

        transform.position = center;

        // Lock all overlapped slots to this item
        foreach (Slot slot in overlappedSlots)
        {
            slot.usedOccupySlot(this);
            lockedSlots.Add(slot);
        }

        assigned = true;
        originalPosition = transform.position;
        
    }

    void ReturnToOrigin()
    {
        assigned = false;
        transform.position = originalPosition;

        // Re-lock original slots
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position,
            GetComponent<Collider2D>().bounds.size, 0f,backpack);

        foreach (Collider2D hit in hits)
        {
            
            Slot slot = hit.GetComponent<Slot>();
            if (slot != null)
            {
                slot.usedOccupySlot(this);
                lockedSlots.Add(slot);
            }
        }
    }
    
    public void throwaway()
    {
        foreach (Slot slot in lockedSlots)
            slot.ClearSlot();
        lockedSlots.Clear();
        Destroy(gameObject);
    }
}