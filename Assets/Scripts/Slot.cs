using UnityEngine;

public class Slot : MonoBehaviour
{
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