using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {

	public BoxCollider2D box;

	private int amt = 0;
	private float vert = 0.0f;

	void Awake() {
		if (Random.Range(0, 100) <= 75) {
			amt = 8;
			transform.localScale = new Vector3(0.75f, 0.75f, 1.0f);
		} else {
			amt = 32;
		}
		Destroy(gameObject, 5.0f);
	}

	void FixedUpdate() {
		if (!CC2D.IsGrounded(box)) {
			vert = Mathf.Max (vert - 0.25f, -10.0f);
			transform.Translate (new Vector2 (0.0f, CC2D.MoveVertical (box, vert)));
		} else {
			vert = 0.0f;
		}
	}

	
	void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag("Player")) {
			other.SendMessage("ApplyHealth", amt);
			Destroy(gameObject);
		}
	}

}
