using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

	public Animator anim;

	void Update () {
		if (anim.GetCurrentAnimatorStateInfo(0).IsName("Done")) {
			Destroy(gameObject);
		}
	}

}
