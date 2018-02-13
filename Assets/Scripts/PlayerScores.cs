using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScores : MonoBehaviour {
	public static float playerHP, playerMaxHP = 100.0f;
	public static float enemyHP, enemyMaxHP = 100.0f;

	public static GameObject player, enemy;

	// Use this for initialization
	void Start () {
		playerHP = playerMaxHP;
		enemyHP = enemyMaxHP;

		player = GameObject.Find("Player");
		enemy = GameObject.Find("Enemy");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
