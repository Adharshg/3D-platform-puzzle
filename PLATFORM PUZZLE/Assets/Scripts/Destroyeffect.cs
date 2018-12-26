using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyeffect : MonoBehaviour {

	public float lifetime;
	
	// Update is called once per frame
	void Update () {
		
		Destroy(gameObject, lifetime);

	}
}
