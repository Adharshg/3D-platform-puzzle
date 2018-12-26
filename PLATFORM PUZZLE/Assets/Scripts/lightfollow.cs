using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lightfollow : MonoBehaviour {

	public Transform target;
	private Vector3 offset;

	// Use this for initialization
	void Start () {
		offset = target.position - transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = target.transform.position + offset;
	}
}
