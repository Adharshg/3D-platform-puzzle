using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {

	public float moveSpeed = 10f;
	//public Rigidbody Player;
	public float jumpForce = 5f;
	public CharacterController controller;

	private Vector3 moveDirection;
	public float gravityScale = 1;

	public Animator anim;
	public Transform pivot;
	public float rotateSpeed;

	public GameObject playerModel;

	public float knockBackForce;
	public float knockBackTIme;
	private float knockBackCounter;

	// Use this for initialization
//void Start () {
		//Player = GetComponent<Rigidbody>();
	//}
	
	// Update is called once per frame
	void Update () {
		//Player.velocity = new Vector3(Input.GetAxis("Horizontal") * moveSpeed, Player.velocity.y, Input.GetAxis("Vertical") * moveSpeed);

		/*if(Input.GetButtonDown("Jump"))
		{
			Player.velocity = new Vector3(Player.velocity.x, jumpForce, Player.velocity.z);
		}*/

		if(knockBackCounter <= 0)
		{

			//moveDirection = new Vector3(Input.GetAxis("Horizontal") * moveSpeed, moveDirection.y, Input.GetAxis("Vertical") * moveSpeed);
			float yStore = moveDirection.y;
			moveDirection = (transform.forward * Input.GetAxis("Vertical")) + (transform.right * Input.GetAxis("Horizontal"));
			moveDirection = moveDirection.normalized * moveSpeed;
			moveDirection.y = yStore;
			if(controller.isGrounded)
			{
				moveDirection.y = -20f;

				if(Input.GetButtonDown("Jump"))
				{
					moveDirection.y = jumpForce;
				}
			}
		}else
		{
			knockBackCounter -= Time.deltaTime;
		}

		moveDirection.y = moveDirection.y + (Physics.gravity.y * gravityScale);
		controller.Move(moveDirection * Time.deltaTime);

		//Move the player in different directions based on the camera position w.r.t to the player
		if(Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
		{
			transform.rotation = Quaternion.Euler(0f, pivot.rotation.eulerAngles.y, 0f);
			Quaternion newRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0f, moveDirection.z));
			playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, newRotation, rotateSpeed * Time.deltaTime);
		}

		anim.SetBool("isGrounded", controller.isGrounded);
		anim.SetFloat("Speed", (Mathf.Abs(Input.GetAxis("Vertical")) + Mathf.Abs(Input.GetAxis("Horizontal"))));
	}

	public void Knockback(Vector3 direction)
	{
		knockBackCounter = knockBackTIme;

		moveDirection = direction * knockBackForce;
		moveDirection.y = knockBackForce;
	}
}
