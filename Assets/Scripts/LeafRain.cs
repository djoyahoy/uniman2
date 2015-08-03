using UnityEngine;
using System.Collections;

public class LeafRain : MonoBehaviour {

	void Awake() {
		Destroy(gameObject, 5.0f);
	}

	void FixedUpdate () {
		transform.Translate(Vector2.down * 3.0f);
	}
}
