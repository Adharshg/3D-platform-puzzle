using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attachplatform : MonoBehaviour {

	public GameObject Player;

	private void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
		{
			Player.transform.parent = transform;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if(other.tag == "Player")
		{
			Player.transform.parent = null;
		}
	}
}
