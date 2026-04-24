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

    public Hunger hunger; // ✅ reference to hunger system

    private Item selectedItem;
    private UsedItem selectedUsedItem;
    public TextMeshProUGUI backpacktext;

    public int backpacksize = 9;

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
            if (backpacksize != 0) 
            {
                //items not assigned slots
                backpacktext.gameObject.SetActive(true);
                Inv.SetActive(true);

            }

            if (backpacksize == 0)
            {
                Inv.SetActive(!Inv.activeSelf);
                backpacktext.gameObject.SetActive(!backpacktext.gameObject.activeSelf);

            }

            if (backpacksize >= 9)
            {
                backpacktext.gameObject.SetActive(true);
                backpacktext.text = "You have too much stuff, eat or throwaway something!";

            }
            
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