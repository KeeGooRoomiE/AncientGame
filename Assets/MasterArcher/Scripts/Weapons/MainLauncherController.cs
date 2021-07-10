using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainLauncherController : MonoBehaviour {

	/// <summary>
	/// Manages all things related to player and enemy weapons after shot. Including starting force, rotation and collisions.
	/// </summary>

	//we need to know if this weapon is for player or the enemy
	//ID 0 = player
	//ID 1 = enemy
	//ID 2 = Demo character
	internal int ownerID;	

	/// <summary>
	/// Player specific variables
	/// </summary>
	//We set this variable from PlayerController class upon shoot command (releasing the touch)
	internal Vector3 playerShootVector;

	/// <summary>
	/// Enemy specific variables
	/// </summary>
	internal float enemyShootAngle;	 		// shooting angle (set from EnemyController class upon shoot command) 

	//only check for collision after a few seconds passed after the shot
	private float timeOfShot;					//time of the creation of this projectile
	private float collisionCheckDelay = 0.1f;	//seconds.

	//reference to player and enemy game objects
	private GameObject enemy;
	private GameObject player;

	//effect objects
	public GameObject bloodFx;
	public GameObject trailFx;
	public GameObject explosionFx;

	//all available arrow types (Each with their unique behaviours)
	public enum arrowTypes { Arrow, Grenade, Sword, Axe, Bomb }
	public arrowTypes arrowType = arrowTypes.Arrow;

	public static int damage;		//damage this weapon deals to target
	private bool stopUpdate;		//we need to stop this weapon after collision
	private bool bypassCode;		//we use this for the segments of our class that requires an enemy, but the current gameMode does not need an enemy

	//special weapon-specific flags
	//private bool bombExplosionByPlayer;		//flag to set if explosion has been done by player input
	//private bool bombExplosionByEnemy;		//flag to set if explosion has been done by enemy AI


	void Awake () {

		//First of all, check if there is an enemy in this game mode.
		if (!GameModeController.isEnemyRequired ()) {
			bypassCode = true;
		}

		//init flags
		//bombExplosionByPlayer = false;
		//bombExplosionByEnemy = false;

		enemy = GameObject.FindGameObjectWithTag ("enemy");
		player = GameObject.FindGameObjectWithTag ("Player");

		switch (arrowType) {
		case arrowTypes.Arrow:
			damage = MasterWeaponManager.arrowDamage;
			break;
		case arrowTypes.Axe:
			damage = MasterWeaponManager.axeDamage;
			break;
		case arrowTypes.Bomb:
			damage = MasterWeaponManager.bombDamage;
			break;
		case arrowTypes.Grenade:
			damage = MasterWeaponManager.grenadeDamage;
			break;
		case arrowTypes.Sword:
			damage = MasterWeaponManager.swordDamage;
			break;
		}
	}


	void Start () {

		//set shoot time
		timeOfShot = Time.time;

		stopUpdate = false;

		//if the owner of this weapon is player, just add force to shoot it, 
		//as all calculations has already been done by payerController
		if (ownerID == 0 || ownerID == 2) {
			
			//add force and let the weapon fly
			if (playerShootVector.magnitude > 0)
				GetComponent<Rigidbody> ().AddForce (playerShootVector, ForceMode.Impulse);
			
		} else if (ownerID == 1) {
			enemyShoot();
		}

		/*GetComponent<BoxCollider> ().enabled = false;
		yield return new WaitForSeconds (collisionCheckDelay);
		GetComponent<BoxCollider> ().enabled = true;*/

		//we can destroy the arrow after a short time (if it is not already destroyed)
		Destroy (gameObject, 45);
	}

	
	void Update () {

		if (arrowType == arrowTypes.Arrow) {
			manageArrowRotation ();
		}

		if (arrowType == arrowTypes.Axe) {
			rotateWeapon (500);
		}

		if (arrowType == arrowTypes.Bomb) {
			rotateWeapon (150);
			explodeOnTouch ();
		}

		//Only for birdhunt & appleshot game modes.
		//destroy the arrow after 2 seconds or after exiting the view
		if (bypassCode) {
			if (Time.time > timeOfShot + 2 || transform.position.x > 12.5f || transform.position.y > 8.5f) {
				//GameController.isArrowInScene = false;
				Destroy (gameObject);
			}
		}

		//bug prevention
		if(transform.position.y < -8)
			Destroy (gameObject);
	}


	/// <summary>
	/// Enemy object just commands the shot sequence. All calculations for the actual arrow happens here.
	/// </summary>
	private void enemyShoot() {

		// get initial and target positions
		Vector3 pos = transform.position;
		Vector3 target = enemy.GetComponent<EnemyController>().target.transform.position;

		// get distance
		float dist = Vector3.Distance (pos, target);
		print("Enemy shoot force: " + dist);

		// calculate initival velocity required for the arrow to move through distance
		float Vi = Mathf.Sqrt ((50+dist) * -Physics.gravity.y / (Mathf.Sin (Mathf.Deg2Rad * enemyShootAngle * 1.05f)));
		float Vy, Vx;

		Vx = Vi * Mathf.Cos (Mathf.Deg2Rad * enemyShootAngle);
		Vy = Vi * Mathf.Sin (Mathf.Deg2Rad * enemyShootAngle);

		// create the velocity vector
		Vector3 localVelocity = new Vector3 (Vx, Vy, 0);
		Vector3 globalVelocity = transform.TransformVector (localVelocity);

		// shoot the arrow
		GetComponent<Rigidbody> ().velocity = globalVelocity;

		//add a little wind (stochastic destination) to make the enemy weapon more realistic
		GetComponent<Rigidbody>().AddForce(new Vector3(EnemyController.fakeWindPower, 0, 0), ForceMode.Force);
	}


	/// <summary>
	/// Manages the arrow rotation based on its position on the move curve.
	/// </summary>
	private Vector3 v; 		//velocity
	private float zDir;		
	private float yDir;
	void manageArrowRotation() {

		if (GetComponent<Rigidbody> ().velocity != Vector3.zero && !stopUpdate) {
			v = GetComponent<Rigidbody> ().velocity;

			if (ownerID == 0 || ownerID == 2) {
				
				zDir = (Mathf.Atan2 (v.y, v.x) * Mathf.Rad2Deg) + 180;
				yDir = Mathf.Atan2 (v.z, v.x) * Mathf.Rad2Deg;
				transform.eulerAngles = new Vector3 (0, -yDir, zDir);

			} else if (ownerID == 1) {
				
				zDir = (Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg) + 180;
				yDir = Mathf.Atan2(v.z, v.x) * Mathf.Rad2Deg;
				transform.eulerAngles = new Vector3(0, -yDir, zDir);

			}
		}
	}


	void rotateWeapon(float rotationSpeed) {

		if (GetComponent<Rigidbody> ().velocity != Vector3.zero && !stopUpdate) {
			transform.eulerAngles = new Vector3 (0, 180, Time.timeSinceLevelLoad * rotationSpeed);
		}
	}


	/// <summary>
	/// Some weapons like bomb can still be controlled after shot. 
	/// If the bomb hit the target directly, it deals minor damage. 
	/// But if the explosion hits the taregt, the damage will be major.
	/// </summary>
	void explodeOnTouch () {

		if (Input.GetKeyDown (KeyCode.Mouse0)) {

			//bombExplosionByPlayer = true;
			GameObject exp = Instantiate(explosionFx, transform.position + new Vector3(0,0, -1.5f), Quaternion.Euler(0, 0, 0)) as GameObject;
			exp.name = "Explosion";

			//check explosion distance with target
			float distanceToTarget = Vector3.Distance(transform.position, enemy.transform.position);
			print ("Bomb - distanceToTarget: " + distanceToTarget);
			if (distanceToTarget <= MasterWeaponManager.bombExplosionRadius) {
				//apply explosion damage
				//enemy.GetComponent<EnemyController> ().enemyCurrentHealth -= MasterWeaponManager.bombExplosionDamage;
				enemy.GetComponent<EnemyController> ().gotLastHit = true;
				enemy.GetComponent<EnemyController> ().playRandomHitSound();
			}

			Destroy (gameObject, 0.01f);

			//GameController.isArrowInScene = false;
			enemy.GetComponent<EnemyController> ().changeTurns();

		}
	}


	/// <summary>
	/// Check for collisions
	/// </summary>
	private bool isChecking = false;
	IEnumerator OnCollisionEnter(Collision other) {

		if (Time.time < timeOfShot + collisionCheckDelay) {
			print ("Can't check for collision at this moment!");	
			yield break;
		}

		//check for collision just once
		if (isChecking)
			yield break;

		isChecking = true;
		string oTag = other.collider.gameObject.tag;	//tag of the object we had collision with

		//Collision with all non-player & non-enemy objects
		if ( (oTag == "ground" || oTag == "rock" || oTag == "bird") && (gameObject.tag == "arrow" || gameObject.tag == "axe" || gameObject.tag == "bomb") ) {
			
			//disable the arrow
			stopUpdate = true;
			GetComponent<Rigidbody>().useGravity = false;
			GetComponent<Rigidbody>().isKinematic = true;

			if(GetComponent<BoxCollider> ())
				GetComponent<BoxCollider> ().enabled = false;

			if(GetComponent<SphereCollider> ())
				GetComponent<SphereCollider> ().enabled = false;
			
			trailFx.SetActive (false);

			//if this is a bomb weapon, we need an explosion after collision
			if (gameObject.tag == "bomb") {
				GameObject exp = Instantiate(explosionFx, other.contacts[0].point + new Vector3(0,0, -1.5f), Quaternion.Euler(0, 0, 0)) as GameObject;
				exp.name = "Explosion";
				Destroy (gameObject, 0.01f);
			}

			//GameController.isArrowInScene = false;

			//We only change turns if this game more requires an enemy
			if (!bypassCode) {

				//if (ownerID == 0)
					//enemy.GetComponent<EnemyController> ().changeTurns ();
				//else if (ownerID == 1)
					//player.GetComponent<PlayerController> ().changeTurns ();
			}

		}



		//Collision with enemy
		//if ( (oTag == "enemyBody" || oTag == "enemyLeg" || oTag == "enemyHead") && 
		//	 (gameObject.tag == "arrow" || gameObject.tag == "axe" || gameObject.tag == "bomb") ) {

		//	//disable the arrow
		//	stopUpdate = true;
		//	GetComponent<Rigidbody>().useGravity = false;
		//	GetComponent<Rigidbody>().isKinematic = true;

		//	if(GetComponent<BoxCollider> ())
		//		GetComponent<BoxCollider> ().enabled = false;

		//	if(GetComponent<SphereCollider> ())
		//		GetComponent<SphereCollider> ().enabled = false;

		//	trailFx.SetActive (false);
		//	//GameController.isArrowInScene = false;
		//	transform.parent = other.collider.gameObject.transform;

		//	//create blood fx
		//	if (gameObject.tag == "arrow" || gameObject.tag == "axe") {
		//		GameObject blood = Instantiate (bloodFx, other.contacts [0].point + new Vector3 (0, 0, -1.5f), Quaternion.Euler (0, 0, 0)) as GameObject;
		//		blood.name = "BloodFX";
		//		Destroy (blood, 1.5f);
		//	}

		//	//if this is a bomb weapon, we need an explosion after collision
		//	if (gameObject.tag == "bomb") {
		//		GameObject exp = Instantiate(explosionFx, other.contacts[0].point + new Vector3(0,0, -1.5f), Quaternion.Euler(0, 0, 0)) as GameObject;
		//		exp.name = "Explosion";
		//		Destroy (gameObject, 0.01f);
		//	}

		//	//manage victim's helath status
		//	//enemy.GetComponent<EnemyController> ().enemyCurrentHealth -= damage;

		//	//save enemy state for lastHit. will be used if we need to move the enemy after getting hit
		//	enemy.GetComponent<EnemyController> ().gotLastHit = true;

		//	//play hit sfx
		//	enemy.GetComponent<EnemyController> ().playRandomHitSound();

		//	//change the turn
		//	enemy.GetComponent<EnemyController> ().changeTurns();
		//}


		//Collision with player
		if ((oTag == "playerBody" || oTag == "playerLeg" || oTag == "playerHead") && 
			(gameObject.tag == "arrow" || gameObject.tag == "axe" || gameObject.tag == "bomb") ) {

			if (bypassCode)
				yield break;

			//disable the arrow
			stopUpdate = true;
			GetComponent<Rigidbody>().useGravity = false;
			GetComponent<Rigidbody>().isKinematic = true;

			if(GetComponent<BoxCollider> ())
				GetComponent<BoxCollider> ().enabled = false;

			if(GetComponent<SphereCollider> ())
				GetComponent<SphereCollider> ().enabled = false;

			trailFx.SetActive (false);
			//GameController.isArrowInScene = false;
			transform.parent = other.collider.gameObject.transform;

			//create blood fx
			GameObject blood = Instantiate(bloodFx, other.contacts[0].point + new Vector3(0,0, -1.5f), Quaternion.Euler(0, 0, 0)) as GameObject;
			blood.name = "BloodFX";
			Destroy (blood, 1.5f);

			//manage victim's helath status
			//player.GetComponent<PlayerController> ().playerCurrentHealth -= damage;

			//play hit sfx
			//player.GetComponent<PlayerController> ().playRandomHitSound();

			//change the turn
			player.GetComponent<PlayerController> ().changeTurns();
		}


		//Special case - collision with birds
		if(oTag == "bird") {
			other.collider.gameObject.GetComponent<BirdsController> ().die ();
			Destroy (gameObject);
		}


		//enable collision detection again
		yield return new WaitForSeconds(0.5f);
		isChecking = false;
	}

}
