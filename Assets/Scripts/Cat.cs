using UnityEngine;
using System.Collections;

public class Cat : MonoBehaviour {

	private enum State {
		Idle,
		Fight,
		Shoot,
		Exploding,
	};

	public Animator anim;
	public BoxCollider2D box;
	public BoxCollider2D playerBox;
	public GameObject player;
	public Rigidbody2D bullet;
	public GameObject explode;
	public GameObject healthGlobe;

	private Vector3 initPos;
	private int health;
	private State state;
	private uint shootCtr;
	
	void Start () {
		initPos = transform.position;
		health = 12;
		state = State.Idle;
		shootCtr = 30;
	}
	
	void FixedUpdate() {
		switch (state) {
		case State.Fight:
			if (++shootCtr >= 70) {
				anim.SetTrigger("shuffle");
				shootCtr = 0;
				state = State.Shoot;
			}
			break;
		case State.Shoot:
			StartCoroutine(GenBullets(Random.Range(4, 9)));
			state = State.Fight;
			break;
		}
	}

	void LateUpdate() {
		if (box.bounds.Intersects (playerBox.bounds)) {
			player.SendMessage("ApplyDamage", 12);
		}
	}

	IEnumerator GenBullets(int amt) {
		for (int i = 0; i < amt; i++) {
			yield return new WaitForSeconds(0.03125f);

			var rb = Instantiate(bullet, transform.position, transform.rotation) as Rigidbody2D;
			Physics2D.IgnoreCollision(rb.GetComponent<BoxCollider2D>(), box);
			
			rb.AddForce(new Vector2(-256.0f, -256.0f), ForceMode2D.Impulse);
		}
	}

	void Activate() {
		if (state.Equals (State.Idle)) {
			state = State.Fight;
		}
	}
	
	void ApplyDamage(int dmg) {
		health = Mathf.Max (0, health - Mathf.Abs(dmg));
		if (health <= 0) {
			state = State.Exploding;
			StartCoroutine(Exploder());
		}
	}

	void Reset() {
		Debug.Log("HERE");
		transform.position = initPos;
		health = 12;
		state = State.Idle;
		shootCtr = 30;
	}

	IEnumerator Exploder() {
		for (int i = 0; i < 10; i++) {
			var v = new Vector3(Random.Range(-16.0f, 32.0f), Random.Range(-16.0f, 16.0f), 0.0f);
			Instantiate(explode, transform.position + v, transform.rotation);
			yield return new WaitForSeconds(0.0625f);
		}

		if (Random.Range(0, 100) >= 90) {
			Instantiate(healthGlobe, transform.position, transform.rotation);
		}

		Destroy(gameObject);
	}

}
