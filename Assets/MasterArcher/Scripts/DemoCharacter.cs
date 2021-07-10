using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoCharacter : MonoBehaviour {

	/// <summary>
	/// Demo character used in scene menu for random shooting.
	/// This is a simplified class derivated form "EnemyController" class.
	/// </summary>

	[Header("Public GamePlay settings")]
	private float baseShootAngle = 61.5f; 			
	private float shootAngleError = 0;				


	[Header("Linked GameObjects")]
	//Reference to game objects (childs and prefabs)
	public GameObject[] weapons;
	public GameObject enemyTurnPivot;
	public GameObject enemyShootPosition;

	[Header("Audio Clips")]
	public AudioClip[] shootSfx;
	public AudioClip[] hitSfx;
	private bool canShoot;


	//Init
	void Awake () {
		canShoot = true;
		shootAngleError = 1.5f;
	}


	void Start () {
		StartCoroutine(reactiveEnemyShoot ());
	}



	/// <summary>
	/// FSM
	/// </summary>
	void Update () {

		if (canShoot)
			StartCoroutine(shootArrow ());
	}


	/// <summary>
	/// shoot routine
	/// </summary>
	IEnumerator shootArrow() {

		if (!canShoot)
			yield break;

		canShoot = false;

		//wait a little
		yield return new WaitForSeconds (1.95f);

		//we need to rotate enemy body to a random/calculated rotation angle
		float targetAngle = Random.Range(55, 75) * 1; 	//important! (originate from 65)
		float t = 0;
		while (t < 1) {
			t += Time.deltaTime;
			enemyTurnPivot.transform.rotation = Quaternion.Euler (0, 0, Mathf.SmoothStep (0, targetAngle, t));
			yield return 0;
		}
			
		print ("Fire!");

		//play shoot sound
		playSfx(shootSfx[Random.Range(0, shootSfx.Length)]);

		//shoot calculations
		GameObject ea = Instantiate(weapons[Random.Range(0, weapons.Length)], enemyShootPosition.transform.position, Quaternion.Euler(0, 0, -45)) as GameObject;
		ea.name = "EnemyProjectile";
		ea.GetComponent<MainLauncherController> ().ownerID = 2;
		ea.GetComponent<MainLauncherController> ().playerShootVector = new Vector3 (1, 1.1f, 0);

		float finalShootAngle = baseShootAngle + Random.Range (-shootAngleError, shootAngleError);
		ea.GetComponent<MainLauncherController> ().enemyShootAngle = finalShootAngle;
		print("Final enemy shoot angle: " + finalShootAngle);

		//at the end
		StartCoroutine(reactiveEnemyShoot ());

		//reset body rotation
		StartCoroutine(resetBodyRotation());
	}


	/// <summary>
	/// tunr enemy body to default rotation
	/// </summary>
	IEnumerator resetBodyRotation() {

		yield return new WaitForSeconds(0.5f);
		float currentRotationAngle = enemyTurnPivot.transform.eulerAngles.z;
		float t = 0;
		while (t < 1) {
			t += Time.deltaTime * 2;
			enemyTurnPivot.transform.rotation = Quaternion.Euler (0, 0, Mathf.SmoothStep (currentRotationAngle, 0, t));
			yield return 0;
		}

	}


	/// <summary>
	/// Enable this to shoot again
	/// </summary>
	IEnumerator reactiveEnemyShoot() {
		yield return new WaitForSeconds (5);
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
		
}
