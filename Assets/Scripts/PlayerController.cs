using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	public float movementSpeed;
	public float maximumPlayerSpeed = 20.0f;
	public float animationDeadZone = 0.2f;
	public float jumpTime = 1.0f;
	public float jumpHeight = 10f;
	public float jumpDelay = 0.1f;
	public GameObject pubeHair;
	public GameObject headHair;
	public float hairSpeed = 0.01f;
	public float minimumPubeAttackTime = 0.15f;
	public float maximumPubeAttackTime = 1f;
	public float releaseDistance = 2.5f;
	public float headAttackDamage = 5.0f;
	public float headAttackForce = 5.0f;
	public float coolDown = 0.1f;

	float coolDownTimer;
	float hairLength;
	float pubeAttackTime = 0;
	bool retracting;
	GameObject holding;
	float attack;

	int direction;

	bool jumping;
	float jumpTimer;
	float jumpDelayTimer;
	bool grounded;
	bool endJump;

	Rigidbody2D rb;
	Animator anim;

	SpriteRenderer srPubeHair;
	GameObject pubeFist;
	SpriteRenderer srHeadHair;
	GameObject headFist;
	GameObject foot;

	void Start() {
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();

		srPubeHair = pubeHair.GetComponent<SpriteRenderer>();
		pubeFist = pubeHair.transform.GetChild(0).gameObject;
		srHeadHair = headHair.GetComponent<SpriteRenderer>();
		headFist = headHair.transform.GetChild(0).gameObject;
		foot = transform.Find("Foot").gameObject;
	}

	private void Update() {
		//Attack timer update
		coolDownTimer += Time.deltaTime;
	}

	void FixedUpdate() {
		//Update direction
		direction = (int) ((transform.position.x - PlayerScores.enemy.transform.position.x) * Vector3.right).normalized.x;

		transform.localScale = new Vector3(-direction, transform.localScale.y, transform.localScale.z);

		//Movment calculations
		if (attack != 1) {
			float move = Input.GetAxis("Horizontal");
			if (!(move > 0 && rb.velocity.x + move * movementSpeed > maximumPlayerSpeed) && !(move < 0 && rb.velocity.x + move * movementSpeed < -maximumPlayerSpeed))
				rb.velocity += new Vector2(move * movementSpeed, 0);

			//If the play moves off screen, then set the players movement to 0
			if (CameraController.TOO_FAR_AWAY && ((rb.velocity.x < 0 && direction < 0) || (rb.velocity.x > 0 && direction > 0))) rb.velocity = new Vector2(0, rb.velocity.y);

			//Only play animation if player is moving
			if (Mathf.Abs(rb.velocity.x) > animationDeadZone) {
				anim.SetBool("Walking", true);

				if (rb.velocity.x > 0)
					anim.SetFloat("Speed", 1);
				else
					anim.SetFloat("Speed", -1);
			} else anim.SetBool("Walking", false);
		} else anim.SetBool("Walking", false);

		//Calculating if there is ground under player
		if (Physics2D.Raycast(foot.transform.position, Vector2.down, 0.1f)) {
			anim.SetTrigger("Grounded");
			jumping = false;
			grounded = true;
		} else
			grounded = false;

		//Jumping
		if (attack != 1) {
			if (Input.GetButton("Jump"))
				jumpDelayTimer += Time.deltaTime;
			else {
				endJump = true;
				jumpTimer = 0;
				jumpDelayTimer = 0;
			}

			if (jumpDelayTimer > jumpDelay) {
				if (jumpTimer < jumpTime && (!endJump || grounded)) {
					jumpTimer += Time.deltaTime;
					rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
					endJump = false;
					jumping = true;

					if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Player Jump")) {
						anim.ResetTrigger("Grounded");
						anim.SetTrigger("Jumping");
					}
				}
			}
		}

		if (!grounded) {
			//If player isn't grounded and there is no controller input, then make player stop
			if (Input.GetAxis("Horizontal") == 0) {
				rb.velocity = new Vector2(rb.velocity.x*0.9f, rb.velocity.y);
				
				//If player is too slow in air, then set players x velocity to zero
				if (Mathf.Abs(rb.velocity.x) < 10f) rb.velocity = new Vector2(0, rb.velocity.y);
			}
		}

		//Attack

		//Hair attack
		if (attack == -1 && Input.GetButton("Upper body")) {
			attack = 0;
			anim.SetTrigger("Attack");
		}

		//If hair attack is active and its animation isn't playing, then quit attack
		if (attack == 0 && !anim.GetCurrentAnimatorStateInfo(anim.GetLayerIndex("Attack Layer")).IsName("Player Attack") && !Input.GetButton("Upper body")) {
			attack = -1;
		}

		//Pube attack
		if (attack != 0 && (pubeAttackTime < maximumPubeAttackTime && (Input.GetButton("Lower body") || (pubeAttackTime > 0 && pubeAttackTime < minimumPubeAttackTime && !retracting))) && grounded && !retracting) {
			attack = 1;
			hairLength += hairSpeed;
			pubeAttackTime += Time.deltaTime;
			pubeHair.SetActive(true);
		} else if (attack == 1)
			retracting = true;

		//Retract hair
		if (retracting) {
			hairLength -= hairSpeed * 2;

			if (holding != null) {
				if ((transform.position - holding.transform.position).sqrMagnitude < releaseDistance * releaseDistance) {
					holding = null;
				} else {
					holding.transform.position -= Vector3.right * hairSpeed * 2 * 4 * -direction;
				}
			}

			//If hair length is 0, then grabbing is done
			if (hairLength < 0) {
				//Make sure that the player doesn't hold down the button
				if (!Input.GetButton("Lower body")) {
					pubeAttackTime = 0;
					retracting = false;
				}

				hairLength = 0;
				anim.SetBool("Grab", false);
				pubeHair.SetActive(false);
				attack = -1;
			}
		}

		if (attack == 1) {
			srPubeHair.size = Vector2.right * hairLength + Vector2.up * srPubeHair.size.y;
			pubeFist.transform.localPosition = new Vector3(hairLength, pubeFist.transform.localPosition.y, pubeFist.transform.localPosition.z);
		}
	}

	public void Grab(GameObject holding) {
		retracting = true;
		this.holding = holding;

		hairLength = Mathf.Abs((transform.position - PlayerScores.enemy.transform.position).x / 4.0f)-0.12f;

		anim.SetBool("Grab", true);
	}

	public void Hit(GameObject enemy) {
		if (coolDownTimer < coolDown) return;
		coolDownTimer = 0;

		PlayerScores.enemyHP -= headAttackDamage;

		if (Input.GetAxis("Vertical") > 0.0f) {
			enemy.GetComponent<Rigidbody2D>().AddForce(new Vector2(0.1f * -direction, 1.2f) * headAttackForce);
			jumpDelayTimer = jumpDelay;
		} else {
			enemy.GetComponent<Rigidbody2D>().AddForce(new Vector2(1f * -direction, 0.5f) * headAttackForce);
		}
	}
}
