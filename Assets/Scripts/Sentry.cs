using UnityEngine;
using System.Collections;

public class Sentry : MonoBehaviour {

	public GameObject obj;

	void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag("Player")) {
			obj.SendMessage("Activate");
		}
	}
	
	void OnTriggerStay2D(Collider2D other) {
		OnTriggerEnter2D (other);
	}

}
