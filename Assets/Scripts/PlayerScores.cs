using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScores : MonoBehaviour {
	public static float playerHP, playerMaxHP = 100.0f;
	public static float enemyHP, enemyMaxHP = 100.0f;

	public static GameObject player, enemy;

	public float healthLoosingRate = 10f;

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
			if (playerHP < targetPlayerHealth) {
				playerHP = targetPlayerHealth;
			}
		}

		if (enemyHP > targetEnemyHealth) {
			enemyHP -= healthLoosingRate*Time.deltaTime;
			if (enemyHP < targetEnemyHealth) {
				enemyHP = targetEnemyHealth;
			}
		}
	}

	public static void hurtPlayer(float damage) {
		targetPlayerHealth -= damage;
	}

	public static void hurtEnemy(float damage) {
		targetEnemyHealth -= damage;
	}
}
