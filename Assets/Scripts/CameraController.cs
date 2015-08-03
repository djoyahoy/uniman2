using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	private int cameraLayer =  1 << 11;

	// TODO : should just use camera game object
	public BoxCollider2D box;
	public GameObject player;

	void LateUpdate() {
		var v = Vector2.Lerp (box.bounds.center, player.transform.position, 1.0f);
		var d = v - (Vector2)transform.position;
		var x = MoveHorizontal (d.x);
		var y = MoveVertical (d.y);
		transform.Translate (new Vector2(x, y));
	}

	float MoveHorizontal(float dx) {
		if (dx == 0.0f) {
			return dx;
		}
		
		var min = box.bounds.min;
		var max = box.bounds.max;
		var center = box.bounds.center;
		
		float ret = dx;
		if (dx > 0) {
			var hit = Physics2D.Raycast (new Vector2 (max.x, center.y), Vector2.right, Mathf.Abs(dx), cameraLayer);
			if (hit.collider != null) {
				ret = Mathf.Min (ret, hit.distance);
			}
		} else if (dx < 0) {
			var hit = Physics2D.Raycast (new Vector2 (min.x, center.y), Vector2.left, Mathf.Abs(dx), cameraLayer);
			if (hit.collider != null) {
				ret = Mathf.Max (ret, -hit.distance);
			}
		}
		
		return ret;
	}
	
	float MoveVertical(float dy) {
		if (dy == 0.0f) {
			return dy;
		}
		
		var min = box.bounds.min;
		var max = box.bounds.max;
		var center = box.bounds.center;
		
		float ret = dy;
		if (dy > 0) {
			var hit = Physics2D.Raycast (new Vector2 (center.x, max.y), Vector2.up, Mathf.Abs(dy), cameraLayer);
			if (hit.collider != null) {
				ret = Mathf.Min (ret, hit.distance);
			}
		} else if (dy < 0) {
			var hit = Physics2D.Raycast (new Vector2 (center.x, min.y), Vector2.down, Mathf.Abs(dy), cameraLayer);
			if (hit.collider != null) {
				ret = Mathf.Max (ret, -hit.distance);
			}
		}
		
		return ret;
	}
}
