using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideTiles : MonoBehaviour {
 
	[SerializeField]
	private string tileTag;
 
	[SerializeField]
	private Vector3 tileSize;
 
	[SerializeField]
	private int maxDistance;
 
	private GameObject[] tiles;
 
	// Use this for initialization
	void Start () {
		this.tiles = GameObject.FindGameObjectsWithTag (tileTag);
		DeactivateDistantTiles ();
	}
 
	void DeactivateDistantTiles() {
		Vector3 playerPosition = this.gameObject.transform.position;
 
		foreach (GameObject tile in tiles) {
			Vector3 tilePosition = tile.gameObject.transform.position + (tileSize / 2f);
 
			float xDistance = Mathf.Abs(tilePosition.x - playerPosition.x);
			float zDistance = Mathf.Abs(tilePosition.z - playerPosition.z);
 
			if (xDistance + zDistance > maxDistance) {
				tile.SetActive (false);
			} else {
				tile.SetActive (true);
			}
		}
	}
 
	void Update () {
		DeactivateDistantTiles ();
	}
 
}