using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Inventory : MonoBehaviour
{
    public GameObject[] slots;

    public GameObject[] items;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        randomPlace();
    }

    void randomPlace()
    {
        List<Backpackmenu> availableSlots = new List<Backpackmenu>();
    
        // populate the list first!
        foreach (GameObject slot in slots)
        {
            Backpackmenu bm = slot.GetComponent<Backpackmenu>();
            if (bm != null)
                availableSlots.Add(bm);
        }

        foreach (GameObject itemobj in items)
        {
            if (availableSlots.Count == 0) break;

            int randomIndex = Random.Range(0, availableSlots.Count);
            Backpackmenu chosenslot = availableSlots[randomIndex];
            availableSlots.RemoveAt(randomIndex);

            Item item = itemobj.GetComponent<Item>();
            itemobj.GetComponent<RectTransform>().anchoredPosition =
                chosenslot.GetComponent<RectTransform>().anchoredPosition;
            chosenslot.PlaceItem(item);
            item.SetSlot(chosenslot);
        }
    }
}
