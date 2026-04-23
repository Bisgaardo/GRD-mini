using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Item : MonoBehaviour
{
    private Vector3 originalPosition;
    private bool isDragging;
    private Camera cam;

    private List<Slot> lockedSlots = new List<Slot>();

    public int Size;
    public Playermovement playermovement;
    public GameObject used;


    void Awake()
    {
        cam = Camera.main;
        used.SetActive(false);
    }

    void OnMouseDown()
    {
        // Release previously locked slots
        foreach (Slot slot in lockedSlots)
            slot.ClearSlot();
        lockedSlots.Clear();

        originalPosition = transform.position;
        isDragging = true;
        playermovement.setSelected(this);
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
        playermovement.setSelected(null);
    }
    
    void TrySnap()
    {
        // Find all slots currently overlapping this item's collider
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position,
            GetComponent<Collider2D>().bounds.size, 0f);

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
            if (slot.IsOccupied() && slot.usedIsOccupied())
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
            slot.OccupySlot(this);
            lockedSlots.Add(slot);
        }

        originalPosition = transform.position;
    }

    void ReturnToOrigin()
    {
        transform.position = originalPosition;

        // Re-lock original slots
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position,
            GetComponent<Collider2D>().bounds.size, 0f);

        foreach (Collider2D hit in hits)
        {
            Slot slot = hit.GetComponent<Slot>();
            if (slot != null)
            {
                slot.OccupySlot(this);
                lockedSlots.Add(slot);
            }
        }
    }

    public void eat(Hunger hunger)
    {
        if (hunger != null)
        {
            hunger.RestoreHunger(1f);
        }
        else
        {
            Debug.LogError("Hunger was null in Item.eat()");
        }

        used.SetActive(true);
        Destroy(gameObject);
    }
    public void throwaway()
    {
        Destroy(gameObject);
    }
}