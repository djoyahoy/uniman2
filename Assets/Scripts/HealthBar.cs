using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour {

	private int prevHealth = 48;
	
	void SetHealth(int health) {
		var s = transform.localScale;
		s.y = (float)health / 16.0f;
		transform.localScale = s;

		transform.Translate(new Vector2(0.0f, -(prevHealth - health) / 2.0f));

		prevHealth = health;
	}

}
