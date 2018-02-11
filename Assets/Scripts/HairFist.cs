﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairFist : MonoBehaviour {

	// Use this for initialization
	void Start() {

	}

	// Update is called once per frame
	void Update() {

	}

	void OnTriggerEnter2D(Collider2D col) {
		if (col.gameObject.CompareTag("Enemy")) {
			Debug.Log(col.tag);
			transform.parent.parent.GetComponent<PlayerController>().Grab(col.gameObject);
		}
	}
}