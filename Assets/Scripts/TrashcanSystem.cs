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
    public GameObject trashInventoryUI;
    public Playermovement player;

    public bool ExitTrashcan;

    public TrashType acceptedType;


    private void Start()
    {
        trashInventoryUI.SetActive(false); // ensure it's hidden at start
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Entered trigger with: " + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered trashcan");
            trashInventoryUI.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            trashInventoryUI.SetActive(false);
        }
    }
}