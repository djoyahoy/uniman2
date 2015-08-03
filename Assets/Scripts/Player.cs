using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	private static int tileLayer = 1 << 8;
	private static int ladderLayer = 1 << 9;
	private static int ladderEndLayer = 1 << 10;

	private enum Face {
		Left,
		Right,
	};

	private enum State {
		Idle,
		Fall,
		Move,
		Jump,
		Climb,
	};

	public Animator anim;
	public BoxCollider2D box;
	public SpriteRenderer rend;
	public Rigidbody2D bullet;
	public GameObject hbar;
	public float speed;
	public float gravity;
	public float maxGravity;
	public int maxJump;
	public int health;
	public GameObject cont;
	
	private State state;
	private Face face;
	private Dictionary<string, bool> input;
	private float vert;
	private int jumpCtr;
	private bool jump;
	private bool invuln;
	
	void Start() {
		state = State.Idle;

		face = Face.Right;

		input = new Dictionary<string, bool> () {
			{"up", false},
			{"down", false},
			{"left", false},
			{"right", false},
		};

		vert = 0.0f;

		jumpCtr = 0;
		jump = false;

		invuln = false;
	}
	
	void Update() {
		List<string> keys = new List<string>(input.Keys);
		foreach (var k in keys) {
			if (Input.GetKey(k)) {
				input[k] = true;
			} else {
				input[k] = false;
			}
		}

		if (Input.GetKeyDown (KeyCode.X)) {
			jump = true;
		} else {
			jump = false;
		}

		// TODO : fix shoot animations while moving.
		// TODO : fix where bullet spawns based on sate.
		if (Input.GetKeyDown (KeyCode.Z)) {
			var rb = Instantiate (bullet, transform.position, transform.rotation) as Rigidbody2D;
			Physics2D.IgnoreCollision(rb.GetComponent<BoxCollider2D>(), box);

			switch (face) {
			case Face.Left:
				rb.AddForce (Vector2.left * 256.0f, ForceMode2D.Impulse);
				break;
			case Face.Right:
				rb.AddForce (Vector2.right * 256.0f, ForceMode2D.Impulse);
				break;
			}

			anim.SetTrigger ("shoot");
		}

		switch (state) {
		case State.Idle:
			UpdateIdle();
			break;
		case State.Fall:
			UpdateFall();
			break;
		case State.Move:
			UpdateMove();
			break;
		case State.Jump:
			UpdateJump();
			break;
		case State.Climb:
			UpdateClimb();
			break;
		}
	}

	void FixedUpdate() {
		switch (state) {
		case State.Idle:
			FixedIdle();
			break;
		case State.Fall:
			FixedFall();
			break;
		case State.Move:
			FixedMove();
			break;
		case State.Jump:
			FixedJump();
			break;
		case State.Climb:
			FixedClimb();
			break;
		}
	}

	void SetState(State s) {
		if (s.Equals (State.Move)) {
			anim.SetBool ("moving", true);
		} else {
			anim.SetBool("moving", false);
		}

		if (s.Equals (State.Climb)) {
			anim.SetBool ("climb", true);
		} else {
			anim.SetBool("climb", false);
		}

		state = s;
	}

	void Flip(Face f) {
		var s = transform.localScale;
		if (f.Equals (Face.Left)) {
			s.x = -1.0f;
		} else {
			s.x = 1.0f;
		}
		transform.localScale = s;
		face = f;
	}

	void UpdateIdle() {
		if (jump) {
			anim.SetBool ("grounded", false);
			SetState (State.Jump);
		} else if (input ["down"] && OverLadder ()) {
			SetState(State.Climb);
		} else if (input ["up"] && OnLadder ()) {
			SetState(State.Climb);
		} else if (input["left"] || input["right"]) {
			SetState(State.Move);
		}
	}

	void FixedIdle() {
		if (!IsGrounded ()) {
			anim.SetBool("grounded", false);
			SetState(State.Fall);
		}
	}

	void UpdateFall() {
		if (input ["up"] && OnLadder ()) {
			vert = 0.0f;
			anim.SetBool("grounded", true);
			SetState(State.Climb);
		} 
	}

	void FixedFall() {
		float dx = 0.0f;
		if (input ["right"]) {
			Flip(Face.Right);
			dx = speed;
		} else if (input ["left"]) {
			Flip(Face.Left);
			dx = -speed;
		}
		transform.Translate (new Vector2 (MoveHorizontal (dx), 0.0f));
		
		vert = Mathf.Max (vert + gravity, maxGravity);
		transform.Translate (new Vector2 (0.0f, MoveVertical (vert)));
		
		if (IsGrounded ()) {
			vert = 0.0f;
			anim.SetBool("grounded", true);
			SetState (State.Idle);
		}
	}

	void UpdateMove() {
		if (jump) {
			anim.SetBool ("grounded", false);
			SetState (State.Jump);
		} else if (input ["down"] && OverLadder ()) {
			SetState(State.Climb);
		} else if (input ["up"] && OnLadder ()) {
			SetState(State.Climb);
		} else if (!input["left"] && !input["right"]) {
			SetState(State.Idle);
		}
	}

	void FixedMove() {
		float dx = 0.0f;
		if (input ["right"]) {
			Flip(Face.Right);
			dx = speed;
		} else if (input ["left"]) {
			Flip(Face.Left);
			dx = -speed;
		}
		transform.Translate (new Vector2 (MoveHorizontal (dx), 0.0f));
				
		if (!IsGrounded ()) {
			anim.SetBool("grounded", false);
			SetState (State.Fall);
		}
	}

	void UpdateJump() {
		if (input ["up"] && OnLadder ()) {
			jumpCtr = 0;
			anim.SetBool("grounded", true);
			SetState(State.Climb);
		} else if (!Input.GetKey(KeyCode.X)) {
			jumpCtr = 0;
			SetState (State.Fall);
		}
	}

	void FixedJump() {
		float dx = 0.0f;
		if (input ["right"]) {
			Flip(Face.Right);
			dx = speed;
		} else if (input ["left"]) {
			Flip(Face.Left);
			dx = -speed;
		}
		transform.Translate (new Vector2 (MoveHorizontal (dx), 0.0f));

		vert = 3.0f;
		transform.Translate (new Vector2 (0.0f, MoveVertical (vert)));

		if (++jumpCtr >= maxJump) {
			jumpCtr = 0;
			SetState(State.Fall);
		}
	}

	void UpdateClimb() {
		if (jump) {
			anim.SetBool("grounded", false);
			SetState(State.Fall);
		}
	}

	void FixedClimb() {
		if (input["left"]) {
			Flip(Face.Left);
		} else if (input["right"]) {
			Flip(Face.Right);
		}


		float dy = 0.0f;
		if (input ["up"]) {
			anim.SetBool("moving", true);
			dy = 2.0f;
		} else if (input ["down"]) {
			anim.SetBool("moving", true);
			dy = -2.0f;
		} else {
			anim.SetBool("moving", false);
		}

		var center = box.bounds.center;
		var max = box.bounds.max;
		var min = box.bounds.min;

		if (dy > 0) {
			var hit = Physics2D.Raycast(new Vector2(center.x, min.y), Vector2.up, Mathf.Abs(dy), ladderEndLayer);
			if (hit.collider != null) {
				dy = Mathf.Min (dy, hit.distance);
				SetState(State.Idle);
			}
		} else if (dy < 0) {
			var hit = Physics2D.Raycast(new Vector2(center.x, min.y), Vector2.down, Mathf.Abs(dy), tileLayer);
			if (hit.collider != null) {
				dy = Mathf.Max (dy, -hit.distance);
				SetState(State.Idle);
			} else if (Physics2D.OverlapPoint (new Vector2(center.x, max.y), ladderLayer) != null) {
				hit = Physics2D.Raycast(center, Vector2.down, Mathf.Abs(dy), ladderLayer);
				if (hit.collider == null) {
					dy = Mathf.Max (dy, -hit.distance);
					anim.SetBool("grounded", false);
					SetState(State.Fall);
				}
			}
		}

		transform.Translate(new Vector2(0.0f, dy));
	}
	
	/**
	 *  Physics
	 **/

	bool IsGrounded() {
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

	bool OverLadder() {
		var x = box.bounds.center.x;
		var y = box.bounds.min.y;

		var hit = Physics2D.Raycast (new Vector2 (x, y), Vector2.down, 1.0f, ladderEndLayer);
		if (hit.collider != null) {
			transform.Translate(Vector2.right * (hit.collider.bounds.center.x - x));
			return true;
		}

		return false;
	}

	bool OnLadder() {
		var col = Physics2D.OverlapPoint (box.bounds.center, ladderLayer);
		if (col != null) {
			transform.Translate(Vector2.right * (col.bounds.center.x - box.bounds.center.x));
			return true;
		}
		return false;
	}

	// TODO : use mathf.approx
	float MoveHorizontal(float dx) {
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
					ret = Mathf.Min (ret, hit.distance);
				}
			}
		} else if (dx < 0) {
			for (int i = (int)min.y + 1; i <= (int)max.y - 1; i += 1) {
				var hit = Physics2D.Raycast (new Vector2 (min.x, i), Vector2.left, Mathf.Abs(dx), tileLayer);
				if (hit.collider != null) {
					ret = Mathf.Max (ret, -hit.distance);
				}
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

		float ret = dy;
		if (dy > 0) {
			for (int i = (int)min.x + 1; i <= (int)max.x - 1; i += 1) {
				var hit = Physics2D.Raycast (new Vector2 (i, max.y), Vector2.up, Mathf.Abs(dy), tileLayer);
				if (hit.collider != null) {
					ret = Mathf.Min (ret, hit.distance);
				}
			}
		} else if (dy < 0) {
			for (int i = (int)min.x + 1; i <= (int)max.x - 1; i += 1) {
				var hit = Physics2D.Raycast (new Vector2 (i, min.y), Vector2.down, Mathf.Abs(dy), tileLayer | ladderEndLayer);
				if (hit.collider != null) {
					ret = Mathf.Max (ret, -hit.distance);
				}
			}
		}

		return ret;
	}

	void ApplyDamage(int dmg) {
		if (!invuln) {
			health = Mathf.Max (0, health - Mathf.Abs(dmg));
			invuln = true;
			StartCoroutine(BeSafe());
			hbar.SendMessage("SetHealth", health);
		}

		if (health <= 0) {
			cont.SendMessage("Reload");
		}
	}

	IEnumerator BeSafe() {
		var c = rend.material.color;

		for (int i = 0; i < 24; i++) {
			c.a = (int)c.a ^ 1;
			rend.material.color = c;
			yield return new WaitForSeconds(0.0625f);
		}

		c.a = 1.0f;
		rend.material.color = c;
		invuln = false;	
	}

	void ApplyHealth(int amt) {
		health = Mathf.Min (48, health + Mathf.Abs(amt));
		hbar.SendMessage("SetHealth", health);
	}
	
}
