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
        // Used items allowed in trash and player inventory
        if (slotType == SlotType.Trash)
            return true;

        if (slotType == SlotType.Player)
            return true;

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