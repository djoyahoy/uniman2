using UnityEngine;
using System.Collections;

public class Rabbit : MonoBehaviour {

	private enum Face {
		Left,
		Right,
	};

	private enum State {
		Idle,
		Fall,
		Jump,
		Shoot,
	};

	public Animator anim;
	public BoxCollider2D box;
	public GameObject player;
	public BoxCollider2D playerBox;
	public float gravity;
	public float maxGravity;
	public Rigidbody2D bullet;
	public GameObject explode;
	public GameObject healthGlobe;

	private Vector3 initPos;
	private int health;
	private Face face;
	private State state;
	private float horiz;
	private float vert;
	private uint shootCtr;
	private uint jumpCtr;

	void Start () {
		initPos = transform.position;
		health = 3;
		face = Face.Left;
		state = State.Idle;
		horiz = 0.0f;
		vert = 0.0f;
		shootCtr = 0;
		jumpCtr = 0;
	}

	void Update() {
		if (player.transform.position.x >= transform.position.x) {
			Flip (Face.Right);
		} else {
			Flip (Face.Left);
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
		case State.Jump:
			FixedJump();
			break;
		case State.Shoot:
			FixedShoot();
			break;
		}
	}

	void LateUpdate() {
		if (box.bounds.Intersects (playerBox.bounds)) {
			player.SendMessage("ApplyDamage", 4);
		}
	}
	
	void FixedIdle() {
		if (!CC2D.IsGrounded (box)) {
			anim.SetBool("fall", true);
			state = State.Fall;
		}
	}

	void FixedFall() {
		transform.Translate (new Vector2 (CC2D.MoveHorizontal (box, horiz), 0.0f));

		vert = Mathf.Max (vert + gravity, maxGravity);
		transform.Translate (new Vector2 (0.0f, CC2D.MoveVertical (box, vert)));
		if (CC2D.IsGrounded (box)) {
			horiz = 0.0f;
			vert = 0.0f;
			anim.SetBool("fall", false);
			state = State.Shoot;
		}
	}

	void FixedShoot() {
		if (++shootCtr % 60 == 0) {
			var d = player.transform.position - transform.position;
			var at = Mathf.Atan2(d.y, d.x);

			var rb = Instantiate(bullet, transform.position, transform.rotation) as Rigidbody2D;
			Physics2D.IgnoreCollision(rb.GetComponent<BoxCollider2D>(), box);

			rb.AddForce(new Vector2(Mathf.Cos(at), Mathf.Sin(at)) * 256.0f, ForceMode2D.Impulse);
		}

		if (shootCtr % 239 == 0) {
			shootCtr = 0;

			if (face.Equals(Face.Left)) {
				horiz = -1.5f;
			} else {
				horiz = 1.5f;
			}

			anim.SetBool("fall", true);
			state = State.Jump;
		}
	}

	void FixedJump() {
		transform.Translate (new Vector2 (CC2D.MoveHorizontal (box, horiz), 0.0f));
		
		vert = 3.0f;
		transform.Translate (new Vector2 (0.0f, CC2D.MoveVertical (box, vert)));
		
		if (++jumpCtr >= 12) {
			jumpCtr = 0;
			anim.SetBool("fall", true);
			state = State.Fall;
		}
	}

	void Flip(Face f) {
		var s = transform.localScale;
		if (f.Equals (Face.Left)) {
			s.x = 1.0f;
		} else {
			s.x = -1.0f;
		}
		transform.localScale = s;
		face = f;
	}

	void Activate() {
		if (state.Equals (State.Idle)) {
			if (face.Equals(Face.Left)) {
				horiz = -1.5f;
			} else {
				horiz = 1.5f;
			}
			state = State.Jump;
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
		health = 3;
		face = Face.Left;
		horiz = 0.0f;
		vert = 0.0f;
		shootCtr = 0;
		jumpCtr = 0;

		if (state.Equals(State.Fall) || state.Equals(State.Jump)) {
			anim.SetBool("fall", false);
		}
		state = State.Idle;
	}

}
