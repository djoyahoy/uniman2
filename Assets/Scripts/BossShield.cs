using UnityEngine;
using System.Collections;

public class BossShield : MonoBehaviour {	
	 
	private float speed = 0.0f;
	
	void Update () {
		transform.Rotate(new Vector3(0.0f, 0.0f, 5.0f));
	}

	void FixedUpdate() {
		//transform.Translate(new Vector2(speed, 0.0f));
		var p = transform.position;
		p.x += speed;
		transform.position = p;
	}
	
	void ApplyDamage(int dmg) {
		// Ignore
	}

	void Move(float s) {
		speed = s;
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag("Player")) {
			other.SendMessage("ApplyDamage", 8);
		}
	}
	
}
