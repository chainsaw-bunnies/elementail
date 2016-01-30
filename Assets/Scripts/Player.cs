using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player : MonoBehaviour {

	private Vector3 moveDirection;

	void Update () {
		Rigidbody rb = GetComponent<Rigidbody>();

		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			transform.position + new Vector3(0, -1f, 0))		}
		else if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			rb.MovePosition(transform.position + new Vector3(0, 1f, 0));
		}
		else if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			rb.MovePosition(transform.position + new Vector3(-1f, 0, 0));
		}
		else if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			rb.MovePosition(transform.position + new Vector3(1f, 0, 0));
		}

		//moveDirection = new Vector3(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"), 0);
		//		moveDirection = transform.TransformDirection(moveDirection);
	
		//rb.MovePosition(transform.position + moveDirection);

	}
}
