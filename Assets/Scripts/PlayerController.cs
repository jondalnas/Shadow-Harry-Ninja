using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	public int movementSpeed;
	public float animationDeadZone = 0.2f;

	Rigidbody2D rb;
	Animator anim;

	void Start () {
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
	}

	private void Update() {
		//Attack

	}

	void FixedUpdate() {
		//Movment calculations
		rb.velocity = new Vector2(Input.GetAxis("Horizontal") * movementSpeed, rb.velocity.y);
		if (Mathf.Abs(rb.velocity.x) > animationDeadZone) {
			anim.SetBool("Walking", true);

			if (rb.velocity.x > 0)
				anim.SetFloat("Speed", 1);
			else
				anim.SetFloat("Speed", -1);
		} else
			anim.SetBool("Walking", false);
	}
}
