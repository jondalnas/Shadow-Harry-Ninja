using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	GameObject player;
	GameObject enemy;
	GameObject gameCamera;

	// Use this for initialization
	void Start () {
		player = GameObject.Find("Player");
		enemy = GameObject.Find("Enemy");
		gameCamera = Camera.main.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		float distance = Mathf.Abs(player.transform.position.x - player.transform.position.x);

		distance /= 6.5f;
		distance *= 2.5f;
		gameCamera.GetComponent<Camera>().orthographicSize = distance;
	}
}
