using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour {
	public Sprite[] backgrounds;
	public Sprite[] floor;

	void Start() {
		if (GameObject.Find("-SAVE-") != null) {
			transform.Find("Background").GetComponent<SpriteRenderer>().sprite = backgrounds[GameObject.Find("-SAVE-").GetComponent<Menu>().level];
			transform.GetComponent<SpriteRenderer>().sprite = floor[GameObject.Find("-SAVE-").GetComponent<Menu>().level];
		}
	}
}
