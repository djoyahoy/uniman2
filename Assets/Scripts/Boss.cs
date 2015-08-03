using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour {

	private enum Face {
		Left,
		Right,
	};

	private enum State {
		Idle,
		Fall,
		Enrage,
		Release,
		Jump,
		Exploding,
	};

	public Animator anim;
	public BoxCollider2D box;
	public GameObject player;
	public BoxCollider2D playerBox;
	public GameObject hbar;
	public GameObject explode;
	public GameObject shield;
	public GameObject rain;

	private State state;
	private Face face;
	private float vert;
	private float horiz;
	private int health;
	private uint jumpCtr;
	private uint enrageCtr;
	private uint releaseCtr;
	private GameObject curShield;
	
	void Start () {
		state = State.Idle;
		face = Face.Left;
		vert = 0.0f;
		horiz = 0.0f;
		health = 48;
		jumpCtr = 0;
		enrageCtr = 0;
		releaseCtr = 0;
		curShield = null;
	}

	void FixedUpdate() {
		if (player.transform.position.x <= transform.position.x) {
			Flip(Face.Left);
		} else {
			Flip (Face.Right);
		}


		switch(state) {
		case State.Fall:
			FixedFall();
			break;
		case State.Enrage:
			FixedEnrage();
			break;
		case State.Jump:
			FixedJump();
			break;
		case State.Release:
			FixedRelease();
			break;
		}
	}

	void LateUpdate() {
		if (box.bounds.Intersects (playerBox.bounds)) {
			player.SendMessage("ApplyDamage", 8);
		}
	}

	void FixedFall() {
		bool col = false;
		transform.Translate (new Vector2 (CC2D.MoveHorizontal (box, horiz, out col), 0.0f));
		if (col) {
			if (face.Equals(Face.Left)) {
				Flip(Face.Right);
			} else {
				Flip (Face.Left);
			}
		}
		
		vert = Mathf.Max (vert - 0.25f, -10.0f);
		transform.Translate (new Vector2 (0.0f, CC2D.MoveVertical (box, vert)));
		if (CC2D.IsGrounded (box)) {
			horiz = 0.0f;
			vert = 0.0f;
			anim.SetBool("fall", false);
			anim.SetTrigger("enrage");
			curShield = Instantiate(shield, transform.position, transform.rotation) as GameObject;
			state = State.Enrage;
		}
	}

	void FixedEnrage() {
		if (++enrageCtr >= 60) {
			enrageCtr = 0;
			anim.SetTrigger("release");
			state = State.Release;

			if (curShield != null) {
				if (face.Equals(Face.Left)) {
					curShield.SendMessage("Move", -3.0f);
				} else {
					curShield.SendMessage("Move", 3.0f);
				}
				Destroy(curShield, 3.0f);
			}
		}
	}

	void FixedRelease() {
		if (++releaseCtr >= 20) {
			releaseCtr = 0;
			if (face.Equals(Face.Left)) {
				horiz = -1.0f;
			} else {
				horiz = 1.0f;
			}
			anim.SetBool("fall", true);
			state = State.Jump;
			Instantiate(rain);
		}
	}

	void FixedJump() {
		bool col = false;
		transform.Translate (new Vector2 (CC2D.MoveHorizontal (box, horiz, out col), 0.0f));
		if (col) {
			if (face.Equals(Face.Left)) {
				Flip(Face.Right);
			} else {
				Flip (Face.Left);
			}
		}
		
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
			s.x = 0.75f;
		} else {
			s.x = -0.75f;
		}
		transform.localScale = s;
		face = f;
	}

	void ApplyDamage(int dmg) {
		if (state.Equals(State.Idle)) {
			return;
		}

		dmg *= 2;
		health = Mathf.Max (0, health - Mathf.Abs(dmg));
		hbar.SendMessage("SetHealth", health);
		if (health <= 0) {
			state = State.Exploding;
			StartCoroutine(Exploder());
		}
	}

	IEnumerator Exploder() {
		for (int i = 0; i < 10; i++) {
			var v = new Vector3(Random.Range(-16.0f, 32.0f), Random.Range(-16.0f, 16.0f), 0.0f);
			Instantiate(explode, transform.position + v, transform.rotation);
			yield return new WaitForSeconds(0.0625f);
		}
		Destroy(gameObject);
	}

	void Activate() {
		if (state.Equals(State.Idle)) {
			anim.SetTrigger("enrage");
			curShield = Instantiate(shield, transform.position, transform.rotation) as GameObject;
			state = State.Enrage;
		}
	}
}
