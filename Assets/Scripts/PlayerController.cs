using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	public int movementSpeed;
	public float animationDeadZone = 0.2f;
	public GameObject hair;
	public float hairSpeed = 0.01f;
	public float minimumAttackTime = 0.15f;

	float hairLength;
	float attackTime = 0;

	Rigidbody2D rb;
	Animator anim;
	SpriteRenderer srHair;
	GameObject fist;

	void Start () {
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		srHair = hair.GetComponent<SpriteRenderer>();
		fist = hair.transform.GetChild(0).gameObject;
	}

	private void Update() {
		//Attack

	}

	void FixedUpdate() {
		//Movment calculations
		if (attackTime == 0) {
			rb.velocity = new Vector2(Input.GetAxis("Horizontal") * movementSpeed, rb.velocity.y);
			if (Mathf.Abs(rb.velocity.x) > animationDeadZone) {
				anim.SetBool("Walking", true);

				if (rb.velocity.x > 0)
					anim.SetFloat("Speed", 1);
				else
					anim.SetFloat("Speed", -1);
			} else anim.SetBool("Walking", false);
		} else anim.SetBool("Walking", false);

		//Attack
		if (Input.GetButton("Lower body") || (attackTime > 0 && attackTime < minimumAttackTime)) {
			hairLength += hairSpeed;
			attackTime += Time.deltaTime;
			hair.SetActive(true);
		} else {
			hairLength = 0;
			attackTime = 0;
			hair.SetActive(false);
		}
		srHair.size = Vector2.right*hairLength+Vector2.up*srHair.size.y;

		fist.transform.localPosition = new Vector3(hairLength, fist.transform.localPosition.y, fist.transform.localPosition.z);
	}
}
