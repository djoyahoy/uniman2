using UnityEngine;
using System.Collections;

public class CatBullet : MonoBehaviour {

	public Rigidbody2D body;

	void Awake() {
		Destroy (gameObject, 1.0f);
	}

	void FixedUpdate () {
		body.AddForce(Vector2.up * 768.0f);
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag("Player") && !other.isTrigger) {
			other.SendMessage("ApplyDamage", 4);
		}
	}
}
