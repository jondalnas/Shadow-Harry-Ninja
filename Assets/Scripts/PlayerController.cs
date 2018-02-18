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
	public int maximumHitsTakenByBlock = 3;
	public float blockCooldownTime = 2f;

	float coolDownTimer;
	float hairLength;
	float pubeAttackTime = 0;
	bool retracting;
	GameObject holding;
	float attack;
	bool blocking;
	int hitsTakenByBlock;
	float blockCooldownTimer;

	[HideInInspector]
	public float stun;

	int direction;
	bool held;

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

		blockCooldownTimer = blockCooldownTime;
	}

	private void Update() {
		//Attack timer update
		coolDownTimer += Time.deltaTime;
	}

	void FixedUpdate() {
		stun -= Time.deltaTime;
		if (stun > 0) return;

		direction = direction == 0 ? 1 : direction;

		//Update direction
		direction = (int)((transform.position.x - PlayerScores.enemy.transform.position.x) * Vector3.right).normalized.x;

		transform.localScale = new Vector3(-direction, transform.localScale.y, transform.localScale.z);

		//Only calculate movement if player isn't being held
		if (!held) {
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
					rb.velocity = new Vector2(rb.velocity.x * 0.9f, rb.velocity.y);

					//If player is too slow in air, then set players x velocity to zero
					if (Mathf.Abs(rb.velocity.x) < 10f) rb.velocity = new Vector2(0, rb.velocity.y);
				}
			}
		}

		//Attack

		//Blocking
		blockCooldownTimer += Time.deltaTime;
		if (Input.GetAxis("Vertical") < 0 && blockCooldownTimer > blockCooldownTime) {
			if (!blocking) {
				anim.SetBool("Block", true);
				hitsTakenByBlock = 0;
				hairLength = 0;
				blocking = true;
				retracting = true;
				attack = -1;
			}
		} else if (Input.GetButtonUp("Vertical") && blocking) {
			blockCooldownTimer = 0;
			blocking = false;
			anim.SetBool("Block", false);
		}

		if (hitsTakenByBlock >= maximumHitsTakenByBlock) {
			hitsTakenByBlock = 0;
			blockCooldownTimer = 0;
			blocking = false;
			anim.SetBool("Block", false);
		}

		//Hair attack
		if (attack == -1 && Input.GetButton("Upper body") && !retracting && !blocking) {
			attack = 0;
			anim.SetTrigger("Attack");
		}

		//If hair attack is active and its animation isn't playing, then quit attack
		if (attack == 0 && !anim.GetCurrentAnimatorStateInfo(anim.GetLayerIndex("Attack Layer")).IsName("Player Attack") && !Input.GetButton("Upper body")) {
			attack = -1;
		}

		//Pube attack
		if (attack != 0 && (pubeAttackTime < maximumPubeAttackTime && (Input.GetButton("Lower body") || (pubeAttackTime > 0 && pubeAttackTime < minimumPubeAttackTime && !retracting))) && grounded && !retracting && !blocking) {
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
					holding.SendMessage("Released");
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
		} else if (holding != null) {
			holding.SendMessage("Released");
			holding = null;
		}

		if (attack == 1) {
			srPubeHair.size = Vector2.right * hairLength + Vector2.up * srPubeHair.size.y;
			pubeFist.transform.localPosition = new Vector3(hairLength, pubeFist.transform.localPosition.y, pubeFist.transform.localPosition.z);
		}
	}

	public void Grab(GameObject holding) {
		retracting = true;
		this.holding = holding;

		holding.SendMessage("IsBeingHeld");

		hairLength = Mathf.Abs((transform.position - PlayerScores.enemy.transform.position).x / 4.0f)-0.12f;

		anim.SetBool("Grab", true);
	}

	public void IsBeingHeld() {
		held = true;
	}

	public void Released() {
		held = false;
	}

	public void Hit(GameObject enemy) {
		if (coolDownTimer < coolDown) return;
		coolDownTimer = 0;

		anim.SetTrigger("Hit");

		enemy.GetComponent<EnemyController>().TakeDamage(headAttackDamage, Input.GetAxis("Vertical") > 0.0f?(new Vector2(0.1f * -direction, 1.2f) * headAttackForce):(new Vector2(1f * -direction, 0.5f) * headAttackForce));
	}

	public void TakeDamage(float damage, Vector3 direction) {
		if (!blocking) {
			PlayerScores.hurtPlayer(damage);
			rb.AddForce(direction);
		} else {
			PlayerScores.enemy.GetComponent<EnemyController>().stun = 0.2f;
			hitsTakenByBlock++;
		}
	}
}
