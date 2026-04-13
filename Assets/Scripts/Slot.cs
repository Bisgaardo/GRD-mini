using UnityEngine;

public class Slot : MonoBehaviour
{
    private Item currentItem;

    public bool IsOccupied()
    {
        return currentItem != null;
    }

    public void OccupySlot(Item item)
    {
        currentItem = item;
    }

    public void ClearSlot()
    {
        currentItem = null;
    }
}