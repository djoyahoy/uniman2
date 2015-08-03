using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	void Reload() {
		Application.LoadLevel(Application.loadedLevel);
	}

}
