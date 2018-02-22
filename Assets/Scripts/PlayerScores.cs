using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScores : MonoBehaviour {
	public static float playerHP, playerMaxHP = 100.0f;
	public static float enemyHP, enemyMaxHP = 100.0f;

	public static GameObject player, enemy;

	public static bool IS_DEAD;

	public float healthLoosingRate = 10f;
	public GameObject gameEnd;

	private static float targetPlayerHealth, targetEnemyHealth;
	
	void Start () {
		playerHP = playerMaxHP;
		enemyHP = enemyMaxHP;
		targetPlayerHealth = playerMaxHP;
		targetEnemyHealth = enemyMaxHP;

		player = GameObject.Find("Player");
		enemy = GameObject.Find("Enemy");
	}
	
	void Update () {
		if (playerHP > targetPlayerHealth) {
			playerHP -= healthLoosingRate * Time.deltaTime;

			if (targetPlayerHealth <= 0) Die();
			if (playerHP < targetPlayerHealth) {
				playerHP = targetPlayerHealth;
			}
		}

		if (enemyHP > targetEnemyHealth) {
			enemyHP -= healthLoosingRate*Time.deltaTime;

			if (targetEnemyHealth <= 0) Die();
			if (enemyHP < targetEnemyHealth) {
				enemyHP = targetEnemyHealth;
			}
		}
	}

	private void Die() {
		gameEnd.SetActive(true);

		GameObject.Find("-SOUND-").GetComponent<AudioSource>().mute = true;

		if (targetPlayerHealth <= 0) gameEnd.transform.Find("Text").GetComponent<Text>().text = "YOU LOSE!";
		if (targetEnemyHealth <= 0) gameEnd.transform.Find("Text").GetComponent<Text>().text = "YOU WIN!";

		IS_DEAD = true;
	}

	public static void hurtPlayer(float damage) {
		targetPlayerHealth -= damage;
	}

	public static void hurtEnemy(float damage) {
		targetEnemyHealth -= damage;
	}
}
