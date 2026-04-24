using UnityEngine;

public enum SlotType
{
    Player,
    Trash
}

public class Slot : MonoBehaviour
{
    public SlotType slotType;

    private Item currentItem;
    private UsedItem currentUsedItem;

    public TrashType acceptedTrashType;

    public bool IsOccupied()
    {
        return currentItem != null;
    }

    public bool usedIsOccupied()
    {
        return currentUsedItem != null;
    }

    public bool CanPlaceItem(Item item)
    {
        // Normal items NOT allowed in trash
        if (slotType == SlotType.Trash)
            return false;

        return true;
    }

    public bool CanPlaceUsedItem(UsedItem item)
    {
        // Player inventory accepts all used items
        if (slotType == SlotType.Player)
            return true;

        // Trash inventory: only accept matching type
        if (slotType == SlotType.Trash)
        {
            return item.trashType == acceptedTrashType;
        }

        return false;
    }

    public void OccupySlot(Item item)
    {
        currentItem = item;
    }

    public void usedOccupySlot(UsedItem usedItem)
    {
        currentUsedItem = usedItem;
    }

    public void ClearSlot()
    {
        currentItem = null;
        currentUsedItem = null;
    }
}