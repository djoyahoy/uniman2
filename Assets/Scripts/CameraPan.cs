using UnityEngine;
using System.Collections;

public class CameraPan : MonoBehaviour {

	public GameObject[] reset;
	public bool vertical;
	public Vector2 moveA;
	public Vector2 moveB;
	public GameObject cam;
	public GameObject player;

	private bool ready = false;
	private bool shouldReset = false;
	private Vector2 camMove = Vector2.zero;
	
	void FixedUpdate() {
		if (ready) {
			var v = Vector2.Lerp (cam.transform.position, camMove, 0.25f);

			var d = v - (Vector2)cam.transform.position;
			if (Mathf.Abs(d.x) < 1.0f && Mathf.Abs(d.y) < 1.0f) {
				cam.transform.position = new Vector3(camMove.x, camMove.y, cam.transform.position.z);

				var c = cam.GetComponent<CameraController>();
				c.enabled = true;

				var p = player.GetComponent<Player>();
				p.enabled = true;

				ready = false;

				if (shouldReset) {
					foreach (var r in reset) {
						if (r != null) {
							r.SendMessage("Reset");
						}
					}
				}
				shouldReset = false;

				return;
			}

			cam.transform.Translate (d);
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (!ready && other.CompareTag("Player")) {
			if (vertical) {
				if (other.transform.position.y < transform.position.y) {
					camMove = moveA;
				} else {
					camMove = moveB;
					shouldReset = true;
				}
			} else {
				if (other.transform.position.x < transform.position.x) {
					camMove = moveB;
					shouldReset = true;
				} else {
					camMove = moveA;
				}
			}

			var c = cam.GetComponent<CameraController>();
			c.enabled = false;

			var p = player.GetComponent<Player>();
			p.enabled = false;

			ready = true;
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if (!ready && other.CompareTag ("Player")) {
			if (vertical) {
				var py = other.transform.position.y;
				var ty = transform.position.y;
				var cy = cam.transform.position.y;
				if (py > ty && cy < ty) {
					camMove = moveA;
				} else if (py < ty && cy > ty) {
					camMove = moveB;
				}
			} else {
				var px = other.transform.position.x;
				var tx = transform.position.x;
				var cx = cam.transform.position.x;
				if (px > tx && cx < tx) {
					camMove = moveB;
				} else if (px < tx && cx > tx) {
					camMove = moveA;
				}
			}

			var c = cam.GetComponent<CameraController>();
			c.enabled = false;
			
			var p = player.GetComponent<Player>();
			p.enabled = false;
			
			ready = true;
		}
	}
}
