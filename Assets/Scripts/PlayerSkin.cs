using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkin : MonoBehaviour {
	public Sprite[] sprite;
	public Sprite[] arm;
	public Sprite[] leg;

	SpriteRenderer player, frontArm, backArm, frontLeg, backLeg;

	void Start () {
		player = transform.Find("Player Sprite").GetComponent<SpriteRenderer>();
		frontArm = transform.Find("Player Sprite").Find("Player Arm Front").GetComponent<SpriteRenderer>();
		backArm = transform.Find("Player Sprite").Find("Player Arm Behind").GetComponent<SpriteRenderer>();
		frontLeg = transform.Find("Player Sprite").Find("Player Leg Front").GetComponent<SpriteRenderer>();
		backLeg = transform.Find("Player Sprite").Find("Player Leg Behind").GetComponent<SpriteRenderer>();

		if (GameObject.Find("-SAVE-") != null) {
			if (gameObject.tag.Equals("Player")) ChangeCharacter(GameObject.Find("-SAVE-").GetComponent<Menu>().playerOne);
			if (gameObject.tag.Equals("Enemy")) ChangeCharacter(GameObject.Find("-SAVE-").GetComponent<Menu>().playerTwo);
		}
	}
	
	public void ChangeCharacter(int index) {
		player.sprite = sprite[index];
		frontArm.sprite = arm[index];
		backArm.sprite = arm[index];
		frontLeg.sprite = leg[index];
		backLeg.sprite = leg[index];
	}
}
