using UnityEngine;
using System.Collections;

public class Pit : MonoBehaviour {

	public GameObject cont;

	void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag("Player")) {
			cont.SendMessage("Reload");	
		}
	}

}
