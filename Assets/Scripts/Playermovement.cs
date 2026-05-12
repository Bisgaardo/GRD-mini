using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
    public GameObject backpacktextobject;

    public int foodAmount = 0;
    public RandomizedFood RandomizedFood;

    public bool trashbag;
    public GameObject trashbagInv;

    public GameObject NPC;

    public int emptycheck;

    public int littercheck;

    public bool npcCheck;
    

    void Start()
    {
        MoveAction.Enable();
        Inv.SetActive(true);
        backpacktext.gameObject.SetActive(false);
        backpacktextobject.SetActive(false);
        NPC.SetActive(false);
        trashbagInv.SetActive(false);
        
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
        Vector2 position = (Vector2)transform.position + move.normalized * 0.02f;
        transform.position = position;

        checkanimation();
    }

    public void openInv(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            
            if (foodAmount != 0)
            {
                // Can't close — too full
                Inv.SetActive(true);
                backpacktext.gameObject.SetActive(true);
                backpacktext.text = "Assign all items to a slot first!";
                backpacktextobject.SetActive(true);
                return;
            }

            if (foodAmount == 0) 
            {
                backpacktextobject.SetActive(false);
                backpacktext.gameObject.SetActive(false);

            }
            if (trashbag)
            {
                trashbagInv.SetActive(!trashbagInv.activeSelf);
            }

            if (trashbag && NPC!= null || npcCheck)
            {
                NPC.SetActive(false);
            }
            

            Inv.SetActive(!Inv.activeSelf);
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
        emptycheck++;
        empty();
    }

    public void playerthrow(InputAction.CallbackContext context)
    {
        if (context.performed && Inv.activeSelf)
        {
            if (selectedUsedItem != null && selectedUsedItem.isUsed)
            {
                if (trashbag)
                {
                    return;
                }
                if (littercheck > 3) 
                {
                    NPC.SetActive(true);
                    backpacktextobject.SetActive(true);
                    backpacktext.gameObject.SetActive(true);
                    backpacktext.text = "Hey! dont litter the beautiful nature. Take this trash bag instead!";
                    trashbag = true;
                    trashbagInv.SetActive(true);
                }
                

                littercheck++;
                selectedUsedItem.throwaway();
                //emptycheck++;
            }
            else if (selectedItem != null)
            {
                if (trashbag)
                {
                    return;
                }
                if (littercheck > 3) 
                {
                    NPC.SetActive(true);
                    backpacktextobject.SetActive(true);
                    backpacktext.gameObject.SetActive(true);
                    backpacktext.text = "Hey! dont litter the beautiful nature. Take this trash bag instead!";
                    trashbag = true;
                    trashbagInv.SetActive(true);
                }
                
                littercheck++;
                selectedItem.throwaway();
                //emptycheck++;
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

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Finish"))
        {
            SceneManager.LoadScene("EndScene");
        }
    }

    public void empty()
    {
        if (emptycheck > 2)
        {
            NPC.SetActive(true);
            backpacktextobject.SetActive(true);
            backpacktext.gameObject.SetActive(true);
            backpacktext.text = "Wow you look hungry here have some food";
            RandomizedFood.newFood();
            
            emptycheck = 0;
            npcCheck = true;
        }
    }
}