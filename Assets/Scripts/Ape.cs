using UnityEngine;
using System.Collections;

public class Ape : MonoBehaviour {

	private enum State {
		Idle,
		Prep,
		Jump,
		Fall,
		Done,
	};

	public Animator anim;
	public BoxCollider2D box;
	public GameObject player;
	public BoxCollider2D playerBox;
	public float gravity;
	public float maxGravity;
	public GameObject explode;
	public GameObject healthGlobe;

	private Vector3 initPos;
	private int health;
	private State state;
	private uint prepCtr;
	private uint jumpCtr;
	private float horiz;
	private float vert;
	
	void Start () {
		initPos = transform.position;
		health = 3;
		state = State.Idle;
		prepCtr = 0;
		jumpCtr = 0;
		horiz = 0.0f;
		vert = 0.0f;
	}

	void FixedUpdate () {
		switch (state) {
		case State.Prep:
			if (++prepCtr >= 60 || player.transform.position.x + 16 >= transform.position.x) {
				horiz = -1.5f;
				anim.SetTrigger("jump");
				state = State.Jump;
			}
			break;
		case State.Jump:
			FixedJump ();
			break;
		case State.Fall:
			FixedFall ();
			break;
		}
	}

	void LateUpdate() {
		if (box.bounds.Intersects (playerBox.bounds)) {
			player.SendMessage("ApplyDamage", 8);
		}
	}

	void FixedJump () {
		transform.Translate (new Vector2 (horiz, 0.0f));
		
		vert = 4.0f;
		transform.Translate (new Vector2 (0.0f, vert));
		
		if (++jumpCtr >= 10) {
			jumpCtr = 0;
			state = State.Fall;
		}
	}

	void FixedFall () {
		transform.Translate (new Vector2 (CC2D.MoveHorizontal (box, horiz), 0.0f));
		
		vert = Mathf.Max (vert + gravity, maxGravity);
		transform.Translate (new Vector2 (0.0f, CC2D.MoveVertical (box, vert)));
		if (CC2D.IsGrounded (box)) {
			horiz = 0.0f;
			vert = 0.0f;
			anim.SetTrigger("done");
			state = State.Done;
		}
	}

	void Activate () {
		if (state.Equals(State.Idle)) {
			state = State.Prep;
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
		transform.position.Set (initPos.x, initPos.y, initPos.z);
		health = 3;
		state = State.Idle;
		prepCtr = 0;
		jumpCtr = 0;
		horiz = 0.0f;
		vert = 0.0f;
	}
}
