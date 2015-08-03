using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public Rigidbody2D body;
	public GameObject explode;
	public string colTag;
	public int dmg;

	void Flip() {
		var s = transform.localScale;
		if (body.velocity.x > 0) {
			s.x = s.x * -1.0f;
		} else {
			s.x = s.x * 1.0f;
		}
		transform.localScale = s;
	}

	void Awake() {
		Flip ();
		Destroy (gameObject, 2.0f);
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag(colTag) && !other.isTrigger) {
			other.SendMessage("ApplyDamage", dmg);
			var e = Instantiate(explode, transform.position, transform.rotation) as GameObject;
			e.transform.localScale = new Vector3(0.75f, 0.75f, 1.0f);
			Destroy(gameObject);
		}
	}

}
