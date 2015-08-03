using UnityEngine;
using System.Collections;

public class LeafBullet : MonoBehaviour {
	
	void Update () {
		transform.Rotate(new Vector3(0.0f, 0.0f, 15.0f));
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag("Player")) {
			other.SendMessage("ApplyDamage", 4);
		}
	}
}
