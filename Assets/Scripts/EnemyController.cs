using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
	public float movementSpeed;
	public float maximumPlayerSpeed = 20.0f;
	public float animationDeadZone = 0.2f;
	public GameObject hair;
	public float hairSpeed = 0.01f;
	public float minimumAttackTime = 0.15f;
	public float maximumAttackTime = 1f;

	float hairLength;
	float attackTime = 0;
	bool retracting;
	float move;

	Rigidbody2D rb;
	Animator anim;
	SpriteRenderer srHair;
	GameObject fist;

	void Start() {
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		srHair = hair.GetComponent<SpriteRenderer>();
		fist = hair.transform.GetChild(0).gameObject;
	}

	private void Update() {
		//Attack
	}

	void FixedUpdate() {
		//Update direction
		int direction = (int)((transform.position.x - PlayerScores.player.transform.position.x) * Vector3.right).normalized.x;

		transform.localScale = new Vector3(-direction, transform.localScale.y, transform.localScale.z);

		//Movment calculations
		if (attackTime == 0) {
			if (Mathf.Abs(rb.velocity.x) < maximumPlayerSpeed) rb.velocity += new Vector2(move * movementSpeed, 0);

			if (Mathf.Abs(rb.velocity.x) > animationDeadZone) {
				anim.SetBool("Walking", true);

				if (rb.velocity.x > 0)
					anim.SetFloat("Speed", 1);
				else
					anim.SetFloat("Speed", -1);
			} else anim.SetBool("Walking", false);
		} else anim.SetBool("Walking", false);

		//Attack
		if (attackTime < maximumAttackTime && (/*Input.GetButton("Lower body")*/ false || (attackTime > 0 && attackTime < minimumAttackTime))) {
			hairLength += hairSpeed;
			attackTime += Time.deltaTime;
			hair.SetActive(true);
		} else {
			if (!Input.GetButton("Lower body")) attackTime = 0;
			retracting = true;
		}

		//Retract hair
		if (retracting) {
			hairLength -= hairSpeed * 3;

			if (hairLength < 0) {
				retracting = false;
				hairLength = 0;
				hair.SetActive(false);
			}
		}

		srHair.size = Vector2.right * hairLength + Vector2.up * srHair.size.y;

		fist.transform.localPosition = new Vector3(hairLength, fist.transform.localPosition.y, fist.transform.localPosition.z);
	}
}
