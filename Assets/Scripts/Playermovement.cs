using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Playermovement : MonoBehaviour
{ 
	public InputAction MoveAction;
	public Vector2 move;
	
	public Animator Animator;
	public GameObject Inv;
	public GameObject tut;

    public Hunger hunger; // reference to hunger system

    private Item selectedItem;
    private UsedItem selectedUsedItem;
    public TextMeshProUGUI backpacktext;

<<<<<<< Updated upstream
    public int backpacksize = 0;
    public RandomizedFood RandomizedFood;
=======
    public int backpacksize = 9;
    
>>>>>>> Stashed changes

    void Start()
    {
        MoveAction.Enable();
        Inv.SetActive(false);
        backpacktext.gameObject.SetActive(false);
    }

    void Update()
    {
        // Stop all movement when inventory is open
        if (Inv.activeSelf)
        {
            checkanimation(); // optional: keeps animations reset
            return;
        }

        move = MoveAction.ReadValue<Vector2>();
        Vector2 position = (Vector2)transform.position + move.normalized * 0.01f;
        transform.position = position;

        checkanimation();
    }

    public void openInv(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (backpacksize >= RandomizedFood.foodamount)
            {
                // Can't close — too full
                Inv.SetActive(true);
                backpacktext.gameObject.SetActive(true);
                backpacktext.text = "You have too much stuff, eat or throwaway something!";
                return;
            }

            if (backpacksize > 0)
            {
                // Can't close — items still unassigned
                Inv.SetActive(true);
                backpacktext.gameObject.SetActive(true);
                backpacktext.text = "Assign all items to a slot first!";
                return;
            }
            // backpacksize == 0, free to toggle
            Inv.SetActive(!Inv.activeSelf);
            backpacktext.gameObject.SetActive(!backpacktext.gameObject.activeSelf);

        }
    }

    public void setSelected(Item item)
    {
        selectedItem = item;
    }

    public void setUsedSelected(UsedItem usedItem)
    {
        selectedUsedItem = usedItem;
    }

    public void rotate(InputAction.CallbackContext context)
    {
        if (context.performed && Inv.activeSelf)
        {
            if (selectedUsedItem != null)
            {
                selectedUsedItem.transform.Rotate(0, 0, 90);
            }
            else if (selectedItem != null)
            {
                selectedItem.transform.Rotate(0, 0, 90);
            }
        }
    }

    public void playereat(InputAction.CallbackContext context)
    {
        if (!context.performed || !Inv.activeSelf) return;

        if (selectedItem == null)
        {
            Debug.LogWarning("No item selected to eat!");
            return;
        }

        if (hunger == null)
        {
            Debug.LogError("Hunger reference is missing on PlayerMovement!");
            return;
        }

        selectedItem.eat(hunger);
    }

    public void playerthrow(InputAction.CallbackContext context)
    {
        if (context.performed && Inv.activeSelf)
        {
            if (selectedUsedItem != null && selectedUsedItem.isUsed)
            {
                selectedUsedItem.throwaway();
            }
            else if (selectedItem != null)
            {
                selectedItem.throwaway();
            }
        }
    }

    void checkanimation()
    {
        Animator.SetBool("walkDown", false);
        Animator.SetBool("walkUp", false);
        Animator.SetBool("walkLeft", false);
        Animator.SetBool("walkRight", false);

        if (move.y > 0)
            Animator.SetBool("walkUp", true);

        if (move.y < 0)
            Animator.SetBool("walkDown", true);

        if (move.x < 0)
            Animator.SetBool("walkLeft", true);

        if (move.x > 0)
            Animator.SetBool("walkRight", true);
    }

    public void tutorial(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            tut.SetActive(!tut.activeSelf);
        }
    }
}