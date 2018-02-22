using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour {
	public int playerOne;
	public int playerTwo;
	public int level;

	public int currentlyPicking;

	void Start() {
		Object.DontDestroyOnLoad(this);
	}

	public void SelectPlayer(int index) {
		if (currentlyPicking == 0) {
			playerOne = index;

			GameObject.Find("Select Character").GetComponent<Text>().text = "Select P2";
		} else if (currentlyPicking == 1) {
			playerTwo = index;

			GameObject.Find("Play").transform.parent.Find("Level Select").gameObject.SetActive(true);
			GameObject.Find("Play").SetActive(false);
		}

		currentlyPicking++;
	}

	public void LoadLevel(int index) {
		level = index;

		SceneManager.LoadScene(1);
	}

	public void ResetPlayerSelect() {
		currentlyPicking = 0;

		transform.Find("Select Character").GetComponent<Text>().text = "Select P1";
	}

	public void MainMenu() {
		Destroy(GameObject.Find("-SAVE-"));
		PlayerScores.IS_DEAD = false;
		SceneManager.LoadScene(0);
	}

	public void Exit() {
		Application.Quit();
	}
}
