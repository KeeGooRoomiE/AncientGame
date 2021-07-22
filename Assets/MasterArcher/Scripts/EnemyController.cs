using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

	/// <summary>
	/// Main enemy controller class.
	/// This class handles enemy difficulty, enemy health, shoot AI, body rotation, movement and dying sequences.
	/// </summary>

	//Difficulty settings
	public enum enemySkillLevels { easy, normal, hard, Robinhood }
	public enemySkillLevels enemySkill = enemySkillLevels.easy;

	[Header("Public GamePlay settings")]
	//public int enemyHealth = 100;					//initial (full) health. can be edited.
	public float baseShootAngle = 61.5f; 			//Very important! - avoid editing this value! (it has been calculated based on the size/shape/weight of the arrow)
	private float shootAngleError = 0;				//We use this to give some erros to enemy shoots. Setting this to 0 will results in accurate shoots
	public static float fakeWindPower = 0;				//We use this if we need to add more randomness to enemy shots.
	//internal int enemyCurrentHealth;				//not editable.
	//public static bool isEnemyDead;					//flag for gameover event
	//public bool moveAfterEachHit = true;			//if set to true, enemy will move a little after getting hit by player
	internal bool gotLastHit = false;               //will set to ture if enemy got hit in the previous round (by player)
	public float targetAngle;
	public GameObject target;
	public float timeToPierce;
	public float timeToShot;
	public float timeToBackRot;
	public float timeToNextShot;

	[Header("Linked GameObjects")]
	//Reference to game objects (childs and prefabs)
	public GameObject arrow;
	public GameObject enemyTurnPivot;
	public GameObject enemyShootPosition;
	public BotController parent;
	//Hidden gameobjects
	//private GameObject gc;							//game controller object
	//private GameObject cam;							//main camera

	[Header("Audio Clips")]
	public AudioClip[] shootSfx;
	public AudioClip[] hitSfx;

	//Enemy shoot settings
	private bool canShoot;


	//Init
	void Awake () {

		//First of all, check if we need an enemy with the current game mode
		if (!GameModeController.isEnemyRequired ()) {
			gameObject.SetActive (false);
		}
			
		//enemyCurrentHealth = enemyHealth;
		canShoot = false;
		//isEnemyDead = false;
		gotLastHit = false;
		//gc = GameObject.FindGameObjectWithTag ("GameController");
		//cam = GameObject.FindGameObjectWithTag ("MainCamera");

		//Increase difficulty by decreasing the enemy error when shooting
		//Please note that "shootAngleError" is not editable. If you want to change the precision, you need to edit "fakeWindPower"
		switch (enemySkill) {
		case enemySkillLevels.easy:
			shootAngleError = 1.5f;
			fakeWindPower = Random.Range(0, 20);
			break;
		case enemySkillLevels.normal:
			shootAngleError = 0.7f;
			fakeWindPower = Random.Range(0, 7);
			break;
		case enemySkillLevels.hard:
			shootAngleError = 0.4f;
			fakeWindPower = Random.Range(0, 3);
			break;
		case enemySkillLevels.Robinhood:
			shootAngleError = 0;
			fakeWindPower = 0;
			break;
		}

		setStartingPosition ();
	}


	void Start () {
		StartCoroutine(reactiveEnemyShoot ());
	}


	/// <summary>
	/// move the enemy to a random position, each time the game begins
	/// </summary>
	void setStartingPosition() {
		//float randomX = Random.Range (15, 60);
		//transform.position = new Vector3 (randomX, transform.position.y, transform.position.z);
	}


	/// <summary>
	/// FSM
	/// </summary>
	void Update () {

		//if the game has not started yet, or the game is finished, just return
		//if (!GameController.gameIsStarted || GameController.gameIsFinished)
		//	return;

		//Check if this object is dead or alive
		//if (enemyCurrentHealth <= 0) {
		//	print ("Enemy is dead...");
		//	enemyCurrentHealth = 0;
		//	isEnemyDead = true;
		//	return;
		//}

		//if this is not our turn, just return
		//if (!GameController.enemysTurn)
		//	return;

		////if we already have an arrow in scene, we can not shoot another one!
		//if (GameController.isArrowInScene)
		//	return;

		////if we have killed the player, but the controller has not finished the game yet
		//if (GameController.noMoreShooting)
		//	return;

		if (canShoot)
			StartCoroutine(shootArrow ());
	}


	/// <summary>
	/// This function will be called when this object is hit by an arrow. It will check if this is still alive after the hit.
	/// if ture, changes the turn. if not, this is dead and game should finish.
	/// </summary>
	public void changeTurns() {

		//print ("enemyCurrentHealth: " + enemyCurrentHealth);
	
		//if(enemyCurrentHealth > 0)
			//StartCoroutine(gc.GetComponent<GameController> ().roundTurnManager ());
		//else
			//GameController.noMoreShooting = true;
	}


	/// <summary>
	/// Making a shot by unit
	/// </summary>
	IEnumerator shootArrow() {

		if (!canShoot)
			yield break;

		canShoot = false;

        #region //unit movement
        /*
		////if enemy needs to move a little after getting hit in the last round...
		////we can do this to prevent player from using the same setting (power + angle) to hit the enemy again...
		//if (gotLastHit) {
		//	yield return new WaitForSeconds (2);
		//	float curPosX = transform.position.x;
		//	float newPosX = transform.position.x + Random.Range (4, 9);

		//	float a = 0;
		//	while (a < 1) {
		//		a += Time.deltaTime;
		//		transform.position = new Vector3 (Mathf.Lerp (curPosX, newPosX, a), transform.position.y, transform.position.z);
		//		yield return 0;
		//	}

		//	if (a >= 1) {
		//		//reset lasthit state
		//		gotLastHit = false;
		//	}
		//}
		*/
        #endregion

        /*
		 * Updates time taken to pierce(before shot, after checking target position)
		 * After that time unit will rotate his part to make a shot
		 * Edit values in AIController
		 */

        parent.UpdateTimetoPierce();
		print("timeToPierce = "+timeToPierce);
		yield return new WaitForSeconds (timeToPierce);

		/*
		 * Unit rotation value to a time-destination lerp step-by-step
		 */

		//angle of rotation equals distance between unit and target(sometimes w/ adding damping)
		targetAngle = Vector3.Distance(transform.position, target.transform.position);// * -Physics.gravity.y;//Random.Range(55, 75); //* -1; 	//important! (originate from 65)
		print("targetAngle = " + targetAngle);
		float t = 0;
		while (t < 1) {
			t += Time.deltaTime;
			enemyTurnPivot.transform.rotation = Quaternion.Euler (0, 0, Mathf.SmoothStep (0, targetAngle, t));
			yield return 0;
		}

		/*
		 * Updates time taken to shot(before actual shot, after rotating to a pierce)
		 * After that time unit will create a bullet
		 * Edit values in AIController
		 */

		parent.UpdateTimetoShot();
		print("timeToShot = " + timeToShot);		//log
		yield return new WaitForSeconds(timeToShot);

		playSfx(shootSfx[Random.Range(0, shootSfx.Length)]);

		GameObject ea = Instantiate(arrow, enemyShootPosition.transform.position, Quaternion.Euler(0, 0, enemyTurnPivot.transform.rotation.z)) as GameObject;
		ea.name = "EnemyProjectile";
		ea.GetComponent<MainLauncherController> ().ownerID = 1;

		float finalShootAngle = baseShootAngle + Random.Range (-shootAngleError, shootAngleError);
		ea.GetComponent<MainLauncherController> ().enemyShootAngle = finalShootAngle;

		StartCoroutine(reactiveEnemyShoot());
		StartCoroutine(resetBodyRotation());
	}

	/// <summary>
	/// Sets unit rotation to default values
	/// </summary>
	IEnumerator resetBodyRotation() {

		/*
		 * Updates time taken to rotate to default(after actual shot, after rotating to a pierce)
		 * After that time unit will rotate itself to default state
		 * Edit values in AIController
		 */

		parent.UpdateTimetoBackRot();
		print("timeToBackRot = " + timeToBackRot);
		yield return new WaitForSeconds(timeToBackRot);
		enemyTurnPivot.transform.eulerAngles = new Vector3(0, 0, 0);

	}


	/// <summary>
	/// Enable unit to shot again
	/// </summary>
	IEnumerator reactiveEnemyShoot() {
		/*
		 * Updates time taken to able unit make a shot(after set to default rotation)
		 * After that time unit will make himself able to shot again
		 * Edit values in AIController
		 */

		parent.UpdateTimeToReload();
		print("timeToNextShot = " + timeToNextShot);
		yield return new WaitForSeconds (timeToNextShot);
		canShoot = true;
	}


	/// <summary>
	/// Plays the sfx.
	/// </summary>
	void playSfx ( AudioClip _clip  ){
		GetComponent<AudioSource>().clip = _clip;
		if(!GetComponent<AudioSource>().isPlaying) {
			GetComponent<AudioSource>().Play();
		}
	}


	/// <summary>
	/// Play a sfx when enemy is hit by an arrow
	/// </summary>
	public void playRandomHitSound (){

		int rndIndex = Random.Range (0, hitSfx.Length);
		playSfx(hitSfx[rndIndex]);
	}
}
