using System;
using System.Net.Mime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Backpackmenu : MonoBehaviour, IDropHandler
{
    private Item currentitem;

    public bool isThrow;
    
    private void Awake()
    {
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            Item draggeditem = eventData.pointerDrag.GetComponent<Item>(); //gets the dragged item
            if (draggeditem == null)
            {
                return; //stops if loop 
            }

            if (currentitem != null)
            {
                Debug.Log("occupied!");
                return;
            }
            Debug.Log("Drop");
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition =
                GetComponent<RectTransform>().anchoredPosition;
            currentitem = draggeditem;
            draggeditem.SetSlot(this);
        }

        if (isThrow)
        {
            Destroy(currentitem.gameObject);
        }

    }
    public void ClearSlot()
    {
        Debug.Log("clear slot");
        currentitem = null;
    }
    
    public void PlaceItem(Item item)
    {
        currentitem = item;
    }
}

