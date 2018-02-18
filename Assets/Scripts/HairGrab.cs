using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairGrab : MonoBehaviour {

	// Use this for initialization
	void Start() {

	}

	// Update is called once per frame
	void Update() {

	}

	void OnTriggerEnter2D(Collider2D col) {
		if (col.gameObject.layer==LayerMask.NameToLayer("Character")) {
			if (transform.parent.parent.CompareTag("Player")) transform.parent.parent.GetComponent<PlayerController>().Grab(col.gameObject);
			if (transform.parent.parent.CompareTag("Enemy")) transform.parent.parent.GetComponent<EnemyController>().Grab(col.gameObject);
		}
	}
}