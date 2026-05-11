using System;
using NUnit.Framework.Internal.Filters;
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


    private void Start()
    {
        trashInventoryUI.gameObject.SetActive(false); // ensure it's hidden at start
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Entered trigger with: " + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered trashcan");
            trashInventoryUI.gameObject.SetActive(true);
            //ensures the trashcan is displayed as the correct type
            int spriteindex = (int)acceptedType;  
            trashInventoryUI.sprite = trashcanType[spriteindex];

            foreach (Slot slot in Slots)
            {
                //this foreach loop assigns the slots type to each type of trashcan like metal, plastic etc.
                if (slot.slotType == SlotType.Trash)
                {
                    Debug.Log(acceptedType);
                    slot.acceptedTrashType = acceptedType;
                }
            }
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