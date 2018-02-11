using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	public float minimumSize = 5.0f, maximumSize = 25.0f;
	public float distancePadding = 5.0f;

	GameObject player;
	GameObject enemy;
	GameObject gameCamera;
	float scaleFactor;

	public static bool TOO_FAR_AWAY;

	// Use this for initialization
	void Start () {
		player = GameObject.Find("Player");
		enemy = GameObject.Find("Enemy");
		gameCamera = Camera.main.gameObject;

		scaleFactor = (Camera.main.GetComponent<Camera>().ViewportToWorldPoint(Vector3.right).x*2.0f)/Camera.main.GetComponent<Camera>().orthographicSize;
	}
	
	// Update is called once per frame
	void Update () {
		//Camera Size
		float distance = player.transform.position.x - enemy.transform.position.x;

		float size = Mathf.Abs(distance - distancePadding) /scaleFactor;
		size = Mathf.Clamp(size, minimumSize, maximumSize);
		if (size == maximumSize) TOO_FAR_AWAY = true;
		else TOO_FAR_AWAY = false;
		gameCamera.GetComponent<Camera>().orthographicSize = size;

		//Camera Position
		gameCamera.transform.position = new Vector3((player.transform.position.x - distance / 2.0f), gameCamera.transform.position.y, gameCamera.transform.position.z);
	}
}
