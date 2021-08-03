using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour {

	/// <summary>
	/// Main player controller class.
	/// This class is responsible for player inputs, rotation, health-management, shooting arrows and helper-dots creation.
	/// </summary>

	[Header("Public GamePlay settings")]
	public bool useHelper = false;					//use helper dots when player is aiming to shoot
	public int baseShootPower = 30;					//base power. edit with care.
	//public int playerHealth = 100;					//starting (full) health. can be edited.
	[SerializeField] private int minShootPower = 15;					//powers lesser than this amount are ignored. (used to cancel shoots)
	//internal int playerCurrentHealth;				//real-time health. not editable.
	//public static bool isPlayerDead;				//flag for gameover event

	[Header("Linked GameObjects")]
	//Reference to game objects (childs and prefabs)
	public GameObject arrow;
	//public GameObject trajectoryHelper;
	public GameObject playerTurnPivot;
	public GameObject playerShootPosition;
	public int playerAmmo;
	private bool canShot = true;
	//public GameObject infoPanel;
	//public GameObject UiDynamicPower;
	//public GameObject UiDynamicDegree;
	//Hidden gameobjects
	private GameObject gc;	//game controller object
	private GameObject cam; //main camera
	[SerializeField] TextMeshPro text;

	//[Header("Audio Clips")]
	//public AudioClip[] shootSfx;
	//public AudioClip[] hitSfx;


	//private settings
	private Vector2 icp; 							//initial Click Position
	private Ray inputRay;
	private RaycastHit hitInfo;
	private float inputPosX;
	private float inputPosY;
	private Vector2 inputDirection;
	private float distanceFromFirstClick;
	private float shootPower;
	private float shootDirection;
	private Vector3 shootDirectionVector;

	//helper trajectory variables
	private float helperCreationDelay = 0.12f;
	private bool canCreateHelper;
	private float helperShowDelay = 0.2f;
	private float helperShowTimer;
	private bool helperDelayIsDone;


	/// <summary>
	/// Init
	/// </summary>
	void Awake () {
		icp = new Vector2 (0, 0);
		//infoPanel.SetActive (false);
		shootDirectionVector = new Vector3(0,0,0);
		//playerCurrentHealth = playerHealth;
		//isPlayerDead = false;

		//gc = GameObject.FindGameObjectWithTag ("GameController");
		//cam = GameObject.FindGameObjectWithTag ("MainCamera");

		//canCreateHelper = true;
		//helperShowTimer = 0;
		//helperDelayIsDone = false;
	}

	public void SetPlayerShotForce(string val)
	{
		baseShootPower = int.Parse(val);
	}

	public void SetPlayerAmmo(string val)
	{
		playerAmmo = int.Parse(val);
	}

	public void SetPlayerMinShotForce(string val)
	{
		minShootPower = int.Parse(val);
	}

	void Update () {

		if (playerAmmo <= 0)
        {
			canShot = false;
        } else
        {
			canShot = true;
        }

		text.text = playerAmmo.ToString();

		//Player pivot turn manager
		if (Input.GetMouseButton (0)) {

			//able to make shot or pierce
			if (canShot) {
				turnPlayerBody();
			}
		}

		//if ammo left is bigger than zero, make a shot
		if (canShot == true)
		{
			//register the initial Click Position
			if (Input.GetMouseButtonDown(0))
			{
				icp = new Vector2(inputPosX, inputPosY);
				print("icp: " + icp);
				print("icp magnitude: " + icp.magnitude);
			}

			//clear the initial Click Position
			if (Input.GetMouseButtonUp(0))
			{

				//only shoot if there is enough power applied to the shoot
				if (shootPower >= minShootPower)
				{
					shootArrow();
					playerAmmo -= 1;
				}
				else
				{
					//reset body rotation
					StartCoroutine(resetBodyRotation());
				}

				//reset variables
				icp = new Vector2(0, 0);
				//infoPanel.SetActive (false);
				//helperShowTimer = 0;
				helperDelayIsDone = false;
			}
		}
	}


	/// <summary>
	/// This function will be called when this object is hit by an arrow. It will check if this is still alive after the hit.
	/// if ture, changes the turn. if not, this is dead and game should finish.
	/// </summary>
	public void changeTurns() {

		//print ("playerCurrentHealth: " + playerCurrentHealth);

		//if (playerCurrentHealth > 0)
		//	StartCoroutine (gc.GetComponent<GameController> ().roundTurnManager ());
		//else
		//	GameController.noMoreShooting = true;
		
	}


	/// <summary>
	/// When player is aiming, we need to turn the body of the player based on the angle of icp and current input position
	/// </summary>
	void turnPlayerBody() {
		
		inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(inputRay, out hitInfo, 50)) {
			// determine the position on the screen
			inputPosX = this.hitInfo.point.x;
			inputPosY = this.hitInfo.point.y;
			//print("Pos X-Y: " + inputPosX + " / " + inputPosY);

			// set the bow's angle to the arrow
			inputDirection = new Vector2(icp.x - inputPosX, icp.y - inputPosY);
			//print("Dir X-Y: " + inputDirection.x + " / " + inputDirection.y);

			shootDirection = Mathf.Atan2(inputDirection.y, inputDirection.x) * Mathf.Rad2Deg;

			//for an optimal experience, we need to limit the rotation to 0 ~ 90 euler angles.
			//so...
			if (shootDirection > 90)
				shootDirection = 90;
			if (shootDirection < 0)
				shootDirection = 0;

			//apply the rotation
			playerTurnPivot.transform.eulerAngles = new Vector3(0, 0, shootDirection);

			//calculate shoot power
			distanceFromFirstClick = inputDirection.magnitude / 4;
			shootPower = Mathf.Clamp(distanceFromFirstClick, 0, 1) * 100;
			//print ("distanceFromFirstClick: " + distanceFromFirstClick);
			//print("shootPower: " + shootPower);

			//modify camera cps - next update
			//CameraController.cps = 5 + (shootPower / 100);

			//show informations on the UI text elements
			//UiDynamicDegree.GetComponent<TextMesh>().text = ((int)shootDirection).ToString();
			//UiDynamicPower.GetComponent<TextMesh> ().text = ((int)shootPower).ToString() + "%";

			//if (useHelper) {
			//	//create trajectory helper points, while preventing them to show when we start to click/touch
			//	if (shootPower > minShootPower && helperDelayIsDone)
			//		StartCoroutine (shootTrajectoryHelper ());
			//}
		}
	}


	/// <summary>
	/// Shoot sequence.
	/// For the player controller, we just need to instantiate the arrow object, apply the shoot power to it, and watch is fly.
	/// There is no AI involved with player arrows. It just fly based on the initial power and angle.
	/// </summary>
	void shootArrow() {

		//set the unique flag for arrow in scene.
		//GameController.isArrowInScene = true;

		//play shoot sound
		//playSfx(shootSfx[Random.Range(0, shootSfx.Length)]);

		//add to shoot counter
		//GameController.playerArrowShot++;

		GameObject arr = Instantiate(arrow, playerShootPosition.transform.position, Quaternion.Euler(0, 180, shootDirection * -1)) as GameObject;
		arr.name = "PlayerProjectile";
		arr.GetComponent<MainLauncherController> ().ownerID = 0;

		shootDirectionVector = Vector3.Normalize (inputDirection);
		shootDirectionVector = new Vector3 (Mathf.Clamp (shootDirectionVector.x, 0, 1), Mathf.Clamp (shootDirectionVector.y, 0, 1), shootDirectionVector.z);

		arr.GetComponent<MainLauncherController> ().playerShootVector = shootDirectionVector * ( (shootPower + baseShootPower) / 50);

		print("shootPower: " + shootPower + " --- " + "shootDirectionVector: " + shootDirectionVector);

		//cam.GetComponent<CameraController> ().targetToFollow = arr;

		//reset body rotation
		StartCoroutine(resetBodyRotation());
	}


	/// <summary>
	/// tunr player body to default rotation
	/// </summary>
	IEnumerator resetBodyRotation() {
		
		//yield return new WaitForSeconds(1.5f);
		//playerTurnPivot.transform.eulerAngles = new Vector3(0, 0, 0);

		yield return new WaitForSeconds(0.25f);
		float currentRotationAngle = playerTurnPivot.transform.eulerAngles.z;
		float t = 0;
		while (t < 1) {
			t += Time.deltaTime * 3;
			playerTurnPivot.transform.rotation = Quaternion.Euler (0, 0, Mathf.SmoothStep (currentRotationAngle, 0, t));
			yield return 0;
		}

	}


	/// <summary>
	/// Create helper dots that shows the possible fly path of the actual arrow
	/// </summary>
	//IEnumerator shootTrajectoryHelper() {

	//	if (!canCreateHelper)
	//		yield break;

	//	canCreateHelper = false;

	//	GameObject t = Instantiate(trajectoryHelper, playerShootPosition.transform.position, Quaternion.Euler(0, 180, shootDirection * -1)) as GameObject;

	//	shootDirectionVector = Vector3.Normalize (inputDirection);
	//	shootDirectionVector = new Vector3 (Mathf.Clamp (shootDirectionVector.x, 0, 1), Mathf.Clamp (shootDirectionVector.y, 0, 1), shootDirectionVector.z);
	//	//print("shootPower: " + shootPower + " --- " + "shootDirectionVector: " + shootDirectionVector);

	//	t.GetComponent<Rigidbody>().AddForce( shootDirectionVector * ( (shootPower + baseShootPower) / 50) , ForceMode.Impulse);

	//	yield return new WaitForSeconds (helperCreationDelay);
	//	canCreateHelper = true;
	//}


	/// <summary>
	/// Plays the sfx.
	/// </summary>
	//void playSfx ( AudioClip _clip  ){
	//	GetComponent<AudioSource>().clip = _clip;
	//	if(!GetComponent<AudioSource>().isPlaying) {
	//		GetComponent<AudioSource>().Play();
	//	}
	//}


	/// <summary>
	/// Play a sfx when player is hit by an arrow
	/// </summary>
	//public void playRandomHitSound (){

	//	int rndIndex = Random.Range (0, hitSfx.Length);
	//	playSfx(hitSfx[rndIndex]);
	//}

}

