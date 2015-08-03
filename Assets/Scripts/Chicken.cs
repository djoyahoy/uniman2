using UnityEngine;
using System.Collections;

public class Chicken : MonoBehaviour {

	private enum State {
		Idle,
		Active,
		Jump,
		Fall,
	};

	public BoxCollider2D box;
	public GameObject player;
	public BoxCollider2D playerBox;
	public GameObject explode;
	public GameObject healthGlobe;
	public Animator anim;

	private int health;
	private State state;
	private uint jumpCtr;
	private float vert;
	private float speed;
	private bool jumped;

	void Start() {
		health = 3;
		state = State.Idle;
		jumpCtr = 0;
		vert = 0.0f;
		speed = -3.0f;
		jumped = false;
	}
	
	void FixedUpdate() {
		switch(state) {
		case State.Active:
			FixedActive ();
			break;
		case State.Jump:
			FixedJump ();
			break;
		case State.Fall:
			FixedFall ();
			break;
		}
	}

	void FixedActive() {
		RunLeft();

		if (!CC2D.IsGrounded(box)) {
			vert = 0.0f;
			anim.SetBool("fall", true);
			state = State.Jump;
		}
	}

	void FixedJump() {
		RunLeft();
		
		vert = 3.0f;
		transform.Translate (new Vector2 (0.0f, CC2D.MoveVertical (box, vert)));
		
		if (++jumpCtr >= 12) {
			jumpCtr = 0;
			state = State.Fall;
		}
	}

	void FixedFall() {
		RunLeft();
		vert = Mathf.Max (vert - 0.25f, -10.0f);
		transform.Translate (new Vector2 (0.0f, CC2D.MoveVertical (box, vert)));
		if (CC2D.IsGrounded (box)) {
			vert = 0.0f;
			anim.SetBool("fall", false);
			state = State.Active;
		}
	}

	void RunLeft() {
		bool col = false;
		float dx = CC2D.MoveHorizontal(box, speed, out col);
		if (col) {
			Instantiate(explode, transform.position, transform.rotation);
			Destroy(gameObject);
		} else {
			transform.Translate(new Vector2(dx, 0.0f));
		}
	}

	void LateUpdate() {
		if (box.bounds.Intersects (playerBox.bounds)) {
			player.SendMessage("ApplyDamage", 4);
		} else if(box.bounds.min.x - 32 <= playerBox.bounds.max.x) {
			if (state.Equals(State.Active) && !jumped) {
				jumped = true;
				anim.SetBool("fall", true);
				state = State.Jump;
			}
		}
	}
	
	void Activate() {
		if (state.Equals(State.Idle)) {
			state = State.Active;
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

}
