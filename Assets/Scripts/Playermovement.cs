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

	private Item selectedItem;

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

	public void rotate(InputAction.CallbackContext context)
	{
		if (context.performed && Inv)
		{
			selectedItem.transform.Rotate(0,0,90);
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
}
