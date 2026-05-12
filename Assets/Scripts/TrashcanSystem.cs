using System;
using NUnit.Framework.Internal.Filters;
using TMPro;
using UnityEngine;

public enum TrashType
{
    Waste,
    FoodWaste,
    Metal,
    Plastic
}

public class TrashcanSystem : MonoBehaviour
{
    public SpriteRenderer trashInventoryUI;
    public Playermovement player;
    public Slot[] Slots;

    public bool ExitTrashcan;

    public TrashType acceptedType;
    public Sprite[] trashcanType;
    public TextMeshProUGUI trashtext;
    public Slot slot;

    private void Start()
    {
        trashInventoryUI.gameObject.SetActive(false); // ensure it's hidden at start
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Player"))
        {
            
            trashInventoryUI.gameObject.SetActive(true);
            //ensures the trashcan is displayed as the correct type
            int spriteindex = (int)acceptedType;  
            trashInventoryUI.sprite = trashcanType[spriteindex];
            

            foreach (Slot slot in Slots)
            {
                //this foreach loop assigns the slots type to each type of trashcan like metal, plastic etc.
                if (slot.slotType == SlotType.Trash)
                {
                    slot.acceptedTrashType = acceptedType;
                    
                }
                
            }
            trashtext.text = ""+slot.acceptedTrashType;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            trashInventoryUI.gameObject.SetActive(false);
        }
    }
}