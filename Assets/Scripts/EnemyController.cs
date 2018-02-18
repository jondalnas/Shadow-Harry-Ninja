using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {
	public float movementSpeed;
	public float maximumSpeed = 20.0f;
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
	float move;
	float distanceToPlayer;
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
	}

	private void Update() {
		//Attack timer update
		coolDownTimer += Time.deltaTime;
	}

	void FixedUpdate() {
		stun -= Time.deltaTime;
		if (stun > 0) return;

		//Update direction
		direction = (int) ((transform.position.x - PlayerScores.player.transform.position.x) * Vector3.right).normalized.x;

		direction = direction==0?1:direction;

		transform.localScale = new Vector3(-direction, transform.localScale.y, transform.localScale.z);

		AIUpdate();

		//Only calculate movement if player isn't being held
		if (!held) {
			//Movment calculations
			if (attack != 1) {
				if (!(move > 0 && rb.velocity.x + move * movementSpeed > maximumSpeed) && !(move < 0 && rb.velocity.x + move * movementSpeed < -maximumSpeed))
					rb.velocity += new Vector2(move * movementSpeed, 0);

				//If the enemy moves off screen, then set the enemies movement to 0
				if (CameraController.TOO_FAR_AWAY && ((rb.velocity.x < 0 && direction < 0) || (rb.velocity.x > 0 && direction > 0))) rb.velocity = new Vector2(0, rb.velocity.y);

				//Only play animation if enemy is moving
				if (Mathf.Abs(rb.velocity.x) > animationDeadZone) {
					anim.SetBool("Walking", true);

					if (rb.velocity.x > 0)
						anim.SetFloat("Speed", 1);
					else
						anim.SetFloat("Speed", -1);
				} else anim.SetBool("Walking", false);
			} else anim.SetBool("Walking", false);

			//Calculating if there is ground under enemy
			if (Physics2D.Raycast(foot.transform.position, Vector2.down, 0.1f)) {
				anim.SetTrigger("Grounded");
				jumping = false;
				grounded = true;
			} else
				grounded = false;

			//Jumping
			if (attack != 1) {
				if (Jump())
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

			/*if (!grounded) {
				//If enemy isn't grounded and there is no controller input, then make enemy stop
				if (Input.GetAxis("Horizontal") == 0) {
					rb.velocity = new Vector2(rb.velocity.x * 0.9f, rb.velocity.y);

					//If enemy is too slow in air, then set enemies x velocity to zero
					if (Mathf.Abs(rb.velocity.x) < 10f) rb.velocity = new Vector2(0, rb.velocity.y);
				}
			}*/
		}

		//Attack
		upperBodyTimer += Time.deltaTime;
		playerAttackTimer += Time.deltaTime;
		if (Input.GetButtonDown("Upper body")) playerAttackCount++;

		//Blocking
		blockCooldownTimer += Time.deltaTime;
		if (Block() && blockCooldownTimer > blockCooldownTime) {
			if (!blocking) {
				anim.SetBool("Block", true);
				hitsTakenByBlock = 0;
				hairLength = 0;
				blocking = true;
				retracting = true;
				attack = -1;
			}
		} else if (!Block() && blocking) {
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
		if (attack == -1 && UpperBody() && !retracting) {
			attack = 0;
			anim.SetTrigger("Attack");
		}

		//If hair attack is active and its animation isn't playing, then quit attack
		if (attack == 0 && !anim.GetCurrentAnimatorStateInfo(anim.GetLayerIndex("Attack Layer")).IsName("Player Attack") && !UpperBody()) {
			attack = -1;
		}

		//Pube attack
		if (attack != 0 && (pubeAttackTime < maximumPubeAttackTime && (LowerBody() || (pubeAttackTime > 0 && pubeAttackTime < minimumPubeAttackTime && !retracting))) && grounded && !retracting) {
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
				//Make sure that the enemy doesn't hold down the button
				if (!LowerBody()) {
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

		hairLength = Mathf.Abs((transform.position - PlayerScores.player.transform.position).x / 4.0f) - 0.12f;

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

		enemy.GetComponent<PlayerController>().TakeDamage(headAttackDamage, new Vector2(1f * -direction, 0.5f) * headAttackForce);
	}

	public void TakeDamage(float damage, Vector3 direction) {
		if (!blocking) {
			PlayerScores.hurtEnemy(damage);
			rb.AddForce(direction);
		} else {
			PlayerScores.enemy.GetComponent<EnemyController>().stun = 0.2f;
			hitsTakenByBlock++;
		}
	}


	public void AIUpdate() {
		distanceToPlayer = Mathf.Abs(transform.position.x - PlayerScores.player.transform.position.x);

		if (distanceToPlayer > 10)
			move = -direction*0.75f;
		else {

		}
	}

	float upperBodyTimer;
	public bool UpperBody() {
		if (upperBodyTimer > 0.5f) {
			upperBodyTimer = 0;

			return distanceToPlayer < 3;
		} else
			return false;
	}

	public bool LowerBody() {
		return distanceToPlayer > 15 || (retracting && hairLength > 0);
	}

	public bool Jump() {
		return false;
	}

	private int playerAttackCount;
	private float playerAttackTimer;
	private bool isBlocking;
	public bool Block() {
		if (playerAttackTimer > 1) {
			if (playerAttackCount > 3) isBlocking = true;
			else isBlocking = false;

			playerAttackTimer = 0;
			playerAttackCount = 0;
		}

		return isBlocking;
	}
}
