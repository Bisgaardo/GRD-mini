using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.InputSystem;
public class Playermovement : MonoBehaviour
{ 
	public InputAction MoveAction;
	public Vector2 move;
	
	public Animator Animator;
	public GameObject Inv;
	public GameObject tut;

	private Item selectedItem;
	private UsedItem selectedUsedItem;

	void Start()
	{ 
		MoveAction.Enable();
		Inv.SetActive(false);
	}
   
	void Update()
	{ 
		move = MoveAction.ReadValue<Vector2>();
		Vector2 position = (Vector2)transform.position + move.normalized * 0.01f;
		transform.position = position;
		checkanimation();
	}

	public void openInv(InputAction.CallbackContext context)
	{
		if (context.started)
		{
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
		if (context.performed && Inv)
		{
			if (selectedUsedItem)
			{
				selectedUsedItem.transform.Rotate(0,0,90);
			}
			else
			{
				selectedItem.transform.Rotate(0,0,90);
			}
		}
	}

	public void playereat(InputAction.CallbackContext context)
	{
		if (context.performed && Inv)
		{
			selectedItem.eat();
		}
	}
	public void playerthrow(InputAction.CallbackContext context)
	{
		if (context.performed && Inv)
		{
			if (selectedUsedItem.isUsed)
			{
				selectedUsedItem.throwaway();
			}
			else
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
