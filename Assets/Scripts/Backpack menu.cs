using System;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Backpackmenu : MonoBehaviour, IDropHandler
{
    private Image img;

    private void Awake()
    {
        img = GetComponent<Image>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition =
                GetComponent<RectTransform>().anchoredPosition;
        }

    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Item"))
        {
            Debug.Log("hit");
            img.color = Color.black;

        }
    }
}

