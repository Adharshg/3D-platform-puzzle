using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController_2 : MonoBehaviour {
	public Transform target;
	public Vector3 offset;
	public bool useOffset;
	public float rotateSpeed;
	public Transform pivot;

	public float maxview;
	//public float minview;

	public bool invertY;

	// Use this for initialization
	void Start () {
		if(!useOffset){
			offset = target.position - transform.position;
		}

		pivot.transform.position = target.transform.position;
		pivot.transform.parent = null;


		Cursor.lockState = CursorLockMode.Locked;
	}
	
	void LateUpdate () {

		pivot.transform.position = target.transform.position;

		//Get x of mouse and rotate the target
		float horizontal = Input.GetAxis("Mouse X") * rotateSpeed;
		pivot.Rotate(0, horizontal, 0);

		//Get y of mouse and rotate the pivot
		float vertical = Input.GetAxis("Mouse Y") * rotateSpeed;
		//pivot.Rotate(-vertical, 0, 0);
		if(invertY)
		{
			pivot.Rotate(vertical, 0, 0);
		}else
		{
			pivot.Rotate(-vertical, 0, 0);
		}


		//Limit the up/down camera rotation
		if(pivot.rotation.eulerAngles.x > maxview && pivot.rotation.eulerAngles.x <180f)
		{
			pivot.rotation = Quaternion.Euler( maxview, 0, 0);
		}
		
		if(pivot.rotation.eulerAngles.x > 180f && pivot.rotation.eulerAngles.x < 300f)
		{
			pivot.rotation = Quaternion.Euler( 300f, 0, 0);
		}


		//Move camera based on the current rotation of the target and the original offset
		float desiredYAngle = pivot.eulerAngles.y;
		float desiredXAngle = pivot.eulerAngles.x;
		Quaternion rotation = Quaternion.Euler( desiredXAngle, desiredYAngle, 0);
		transform.position = target.position -(rotation * offset);

		//transform.position = target.position - offset;
		
		if(transform.position.y < target.position.y)
		{
			transform.position = new Vector3(transform.position.x, target.position.y - 0.5f, transform.position.z);
		}

		transform.LookAt(target);

	}
}
