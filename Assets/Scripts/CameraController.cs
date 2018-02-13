using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	public float minimumSize = 5.0f, maximumSize = 25.0f;
	public float distancePadding = 5.0f;
	public float cameraZoomSpeed = 0.5f;
	
	GameObject gameCamera;
	float scaleFactorX;
	float scaleFactorY;
	float realSize;

	public static bool TOO_FAR_AWAY;
	public static bool DO_NOT_UPDATE_CAMERA;

	// Use this for initialization
	void Start() {
		gameCamera = Camera.main.gameObject;

		scaleFactorX = (Camera.main.GetComponent<Camera>().ViewportToWorldPoint(Vector3.right).x * 2.0f) / Camera.main.GetComponent<Camera>().orthographicSize;
		scaleFactorY = (Camera.main.GetComponent<Camera>().ViewportToWorldPoint(Vector3.up).y-Camera.main.transform.position.y) / Camera.main.GetComponent<Camera>().orthographicSize;
	}
	
	void FixedUpdate() {
		if (DO_NOT_UPDATE_CAMERA) return;

		//Camera Size
		float distance = PlayerScores.player.transform.position.x - PlayerScores.enemy.transform.position.x;

		float size;

		//Size based on x distance
		float sizeX = Mathf.Abs(distance + distancePadding * ((distance < 0) ? -1 : 1)) / scaleFactorX;

		float sizeY = ((PlayerScores.enemy.transform.position.y>PlayerScores.player.transform.position.y?PlayerScores.enemy.transform.position.y:PlayerScores.player.transform.position.y)+8) / scaleFactorY * 0.5f;
		
		if (sizeX < sizeY) size = sizeY;
		else size = sizeX;

		size = Mathf.Clamp(size, minimumSize, maximumSize);

		if (sizeX >= maximumSize) TOO_FAR_AWAY = true;
		else TOO_FAR_AWAY = false;
		
		if (realSize < size) {
			realSize = size;
		} else {
			realSize -= cameraZoomSpeed * Time.deltaTime;

			if (realSize < size) realSize = size;
		}

		gameCamera.GetComponent<Camera>().orthographicSize = realSize;

		//Camera Position
		gameCamera.transform.position = new Vector3((PlayerScores.player.transform.position.x - distance / 2.0f), Camera.main.GetComponent<Camera>().orthographicSize * scaleFactorY-5, gameCamera.transform.position.z);
	}
}
