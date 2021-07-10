using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	/// <summary>
	/// Main camera manager. Handles camera movement, smooth follow, starting demo, and limiters.
	/// Note that this is the game-play camera. All UI rendering is done by UICamera in another thread.
	/// </summary>

	public static float	cps = 6; 			//camera's projection size

	public bool performStartMove = true;	//should camera moves towards enemy and back to player, when we just start the game?1
	internal bool startMoveIsDoneFlag;		//flag to set when the starting animation has been performed

	internal Vector3 cameraStartingPos;
	internal Vector3 cameraCurrentPos;

	internal GameObject targetToLock;		//the target object we need to have a static view on
	internal GameObject targetToFollow;		//the target we need to follow as it moves

	//reference to game objects
	private GameObject player;
	private GameObject enemy;


	void Awake () {

		//First of all, check if there is an enemy in this game mode.
		if (!GameModeController.isEnemyRequired ()) {
			performStartMove = false;
		}

		GetComponent<Camera> ().orthographicSize = cps;
		cameraStartingPos = new Vector3 (0, 0, -10);
		cameraCurrentPos = cameraStartingPos;
		transform.position = cameraStartingPos;
		targetToLock = null;
		targetToFollow = null;
		startMoveIsDoneFlag = false;

		player = GameObject.FindGameObjectWithTag ("Player");
		enemy = GameObject.FindGameObjectWithTag ("enemy");
	}


	void Start () {

		if (performStartMove) {
			//start the demo, by moving towards enemy's position and then back to player
			StartCoroutine (runDemo ());
		} else {
			//no need to perform the demo. ativate the game immediately.
			startMoveIsDoneFlag = true;
	//		GameController.gameIsStarted = true;
		}

	}


	/// <summary>
	/// Move the camera to enemy location, wait a little and then back to player location.
	/// </summary>
	IEnumerator runDemo() {

		//wait a little
		yield return new WaitForSeconds(0.5f);

		float cameraSpeed = 0.30f;
		float t = 0;
		while (t < 1) {
			t += Time.deltaTime * cameraSpeed;
			transform.position = new Vector3 (	Mathf.SmoothStep (cameraStartingPos.x, enemy.transform.position.x, t),
												transform.position.y,
												transform.position.z);
			yield return 0;
		}

		//wait a few while
		if(t >= 1)
			yield return new WaitForSeconds(1.0f);

		//back to player
		t = 0;
		while (t < 1) {
			t += Time.deltaTime * cameraSpeed;
			transform.position = new Vector3 (	Mathf.SmoothStep (enemy.transform.position.x, player.transform.position.x, t),
												transform.position.y,
												transform.position.z);
			yield return 0;
		}

		if (t >= 1) {
			//start the game
			startMoveIsDoneFlag = true;
			//GameController.gameIsStarted = true;
		}

	}


	/// <summary>
	/// FSM
	/// </summary>
	void Update () {

		//if the game has not started yet, or the game is finished, just return
		//if (!GameController.gameIsStarted || GameController.gameIsFinished)
			//return;

		//Not implemented
		handleCps ();

		//follow the target (if any)
		if (targetToFollow) {
			StartCoroutine (smoothFollow (targetToFollow.transform.position));
		}

	}
		

	/// <summary>
	/// set limiters
	/// </summary>
	void LateUpdate () {
		//Limit camera's movement
		if (transform.position.y < 0) {
			transform.position = new Vector3 (transform.position.x, 0, transform.position.z);
		}

		if (transform.position.y > 25) {
			transform.position = new Vector3 (transform.position.x, 25, transform.position.z);
		}
	}


	/// <summary>
	/// Smooth follow the target object.
	/// </summary>
	[Range(0, 0.5f)]
	public float followSpeedDelay = 0.1f;
	private float xVelocity = 0.0f;
	private float yVelocity = 0.0f;
	IEnumerator smoothFollow(Vector3 p) {

		//Only follow weapons as target if there is an enemy in this game mode!
		if (!GameModeController.isEnemyRequired ()) {
			yield break;
		}

		float posX = Mathf.SmoothDamp(transform.position.x, p.x, ref xVelocity, followSpeedDelay);
		float posY = Mathf.SmoothDamp(transform.position.y, p.y - 2, ref yVelocity, followSpeedDelay);
		transform.position = new Vector3(posX, posY, transform.position.z);

		//always save camera's current pos in an external variable, for later use
		cameraCurrentPos = transform.position;

		yield return 0;
	}


	/// <summary>
	/// Not implemented yet !
	/// </summary>
	void handleCps() {

		//limiters
		if (cps < 5) cps = 5;
		if (cps > 6) cps = 6;

		GetComponent<Camera> ().orthographicSize = cps;
	}


	/// <summary>
	/// move the camera to a given position
	/// </summary>
	/// <returns>The to position.</returns>
	/// <param name="from">From.</param>
	/// <param name="to">To.</param>
	/// <param name="speed">Speed.</param>
	public IEnumerator goToPosition(Vector3 from, Vector3 to, float speed) {

		//Only change position if there is an enemy in this game mode!
		if (!GameModeController.isEnemyRequired ()) {
			yield break;
		}

		float t = 0;
		while (t < 1) {
			t += Time.deltaTime * speed;
			transform.position = new Vector3 (	Mathf.SmoothStep (from.x, to.x, t),
												Mathf.SmoothStep (from.y, to.y, t),
												transform.position.z);
			yield return 0;
		}

		if(t >= 1) {
			//always save camera's current pos in an external variable, for later use
			cameraCurrentPos = transform.position;
		}
	}


}
