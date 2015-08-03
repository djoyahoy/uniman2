using UnityEngine;
using System.Collections;

public class Bat : MonoBehaviour {

	private enum State {
		Idle,
		Chase,
		Retreat,
	};

	public Animator anim;
	public BoxCollider2D box;
	public GameObject player;
	public BoxCollider2D playerBox;
	public GameObject explode;
	public GameObject healthGlobe;

	private float speedx = 1.0f;
	private float speedy = 0.75f;
	private Vector3 initPos;
	private int health;
	private State state;
	private uint timer;
			
	void Start () {
		initPos = transform.position;
		health = 1;
		state = State.Idle;
		timer = 120;
	}

	void Update () {
		switch (state) {
		case State.Idle:
			timer += 1;
			break;
		}
	}

	void FixedUpdate() {
		switch (state) {
		case State.Chase:
			FixedChase();
			break;
		case State.Retreat:
			FixedRetreat();
			break;
		}
	}

	void FixedChase() {
		var v = Vector2.Lerp (transform.position, player.transform.position, 0.0625f);
		var d = v - (Vector2)transform.position;
		float dx = Mathf.Clamp(d.x, -speedx, speedx);
		float dy = Mathf.Clamp(d.y, -speedy, speedy);
		transform.Translate (new Vector2(dx, dy));
	}

	void FixedRetreat() {
		bool col;
		float dy = CC2D.MoveVertical (box, speedy, out col);
		transform.Translate (new Vector2 (0.0f, dy));
		if (col) {
			anim.SetTrigger("idle");
			state = State.Idle;
		}
	}

	void LateUpdate() {
		if (box.bounds.Intersects (playerBox.bounds)) {
			if (state.Equals(State.Chase)) {
				state = State.Retreat;
			}
			player.SendMessage("ApplyDamage", 4);
		}
	}

	void Activate() {
		if (state.Equals (State.Idle) && timer >= 120) {
			anim.SetTrigger("chase");
			state = State.Chase;
			timer = 0;
		}
	}
	
	void ApplyDamage(int dmg) {
		health = Mathf.Max (0, health - Mathf.Abs(dmg));
		if (health <= 0) {
			Instantiate(explode, transform.position, transform.rotation);
			if (Random.Range(0, 100) >= 80) {
				Instantiate(healthGlobe, transform.position, transform.rotation);
			}
			Destroy(gameObject);
		}
	}

	void Reset() {
		transform.position = initPos;
		health = 1;
		timer = 120;
		if (state.Equals(State.Chase) || state.Equals(State.Retreat)) {
			anim.ResetTrigger("chase");
			anim.SetTrigger("idle");
		}
		state = State.Idle;
	}

}
