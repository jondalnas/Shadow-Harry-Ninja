using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {
	GameObject player;
	GameObject enemy;

	float playerHealthBarPosition;
	float playerHealthBarWidth;
	float enemyHealthBarPosition;
	float enemyHealthBarWidth;

	void Start () {
		player = transform.Find("Player").gameObject;
		enemy = transform.Find("Enemy").gameObject;

		playerHealthBarPosition = player.GetComponent<RectTransform>().position.x;
		enemyHealthBarPosition = enemy.GetComponent<RectTransform>().position.x;
		playerHealthBarWidth = player.GetComponent<RectTransform>().rect.width;
		enemyHealthBarWidth = enemy.GetComponent<RectTransform>().rect.width;
	}
	
	void Update () {
		player.GetComponent<RawImage>().uvRect = new Rect(1-PlayerScores.playerHP / PlayerScores.playerMaxHP, 0, PlayerScores.playerHP / PlayerScores.playerMaxHP, 1);
		player.GetComponent<RectTransform>().localScale = new Vector3(PlayerScores.playerHP / PlayerScores.playerMaxHP, 1f, 1f);

		enemy.GetComponent<RawImage>().uvRect = new Rect(1-PlayerScores.enemyHP / PlayerScores.enemyMaxHP, 0, PlayerScores.enemyHP / PlayerScores.enemyMaxHP, 1);
		enemy.GetComponent<RectTransform>().localScale = new Vector3(1 - PlayerScores.enemyHP / PlayerScores.enemyMaxHP - 1, 1f, 1f);
	}
}
