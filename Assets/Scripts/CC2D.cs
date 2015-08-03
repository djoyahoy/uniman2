using UnityEngine;
using System.Collections;

public class CC2D {
	
	private static int tileLayer = 1 << 8;
	private static int ladderEndLayer = 1 << 10;

	public static bool IsGrounded(BoxCollider2D box) {
		var min = box.bounds.min;
		var max = box.bounds.max;
		
		for (int i = (int)min.x + 1; i <= (int)max.x - 1; i += 1) {
			var hit = Physics2D.Raycast (new Vector2 (i, min.y), Vector2.down, 1.0f, tileLayer | ladderEndLayer);
			if (hit.collider != null) {
				return true;
			}
		}
		
		return false;
	}
	
	// TODO : use mathf.approx
	public static float MoveHorizontal(BoxCollider2D box, float dx, out bool col) {
		col = false;
		if (dx == 0.0f) {
			return dx;
		}
		
		var min = box.bounds.min;
		var max = box.bounds.max;
		
		float ret = dx;
		if (dx > 0) {
			for (int i = (int)min.y + 1; i <= (int)max.y - 1; i += 1) {
				var hit = Physics2D.Raycast (new Vector2 (max.x, i), Vector2.right, Mathf.Abs(dx), tileLayer);
				if (hit.collider != null) {
					col = true;
					ret = Mathf.Min (ret, hit.distance);
				}
			}
		} else if (dx < 0) {
			for (int i = (int)min.y + 1; i <= (int)max.y - 1; i += 1) {
				var hit = Physics2D.Raycast (new Vector2 (min.x, i), Vector2.left, Mathf.Abs(dx), tileLayer);
				if (hit.collider != null) {
					col = true;
					ret = Mathf.Max (ret, -hit.distance);
				}
			}
		}
		
		return ret;
	}

	public static float MoveHorizontal(BoxCollider2D box, float dx) {
		bool col;
		return MoveHorizontal(box, dx, out col);
	}
	
	public static float MoveVertical(BoxCollider2D box, float dy, out bool col) {
		col = false;
		if (dy == 0.0f) {
			return dy;
		}
		
		var min = box.bounds.min;
		var max = box.bounds.max;
		
		float ret = dy;
		if (dy > 0) {
			for (int i = (int)min.x + 1; i <= (int)max.x - 1; i += 1) {
				var hit = Physics2D.Raycast (new Vector2 (i, max.y), Vector2.up, Mathf.Abs(dy), tileLayer);
				if (hit.collider != null) {
					col = true;
					ret = Mathf.Min (ret, hit.distance);
				}
			}
		} else if (dy < 0) {
			for (int i = (int)min.x + 1; i <= (int)max.x - 1; i += 1) {
				var hit = Physics2D.Raycast (new Vector2 (i, min.y), Vector2.down, Mathf.Abs(dy), tileLayer | ladderEndLayer);
				if (hit.collider != null) {
					col = true;
					ret = Mathf.Max (ret, -hit.distance);
				}
			}
		}
		
		return ret;
	}

	public static float MoveVertical(BoxCollider2D box, float dx) {
		bool col;
		return MoveVertical(box, dx, out col);
	}

}
